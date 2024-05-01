using System;
using DigitalRise.GameBase;
using DigitalRise.Geometry;
using DigitalRise.Graphics;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MathHelper = DigitalRise.Mathematics.MathHelper;
using Microsoft.Xna.Framework.Input.Touch;
using DigitalRise.Input;
using System.ComponentModel;
using DigitalRise.LevelEditor.Utility;

namespace DigitalRise.LevelEditor
{
    public class CameraObject : GameObject
	{
		// Some constants for motion control.
		private const float LinearVelocityMagnitude = 5f;
		private const float AngularVelocityMagnitude = 0.1f;
		private const float ThumbStickFactor = 15;
		private const float SpeedBoost = 20;

		private readonly IServiceProvider _services;
		private readonly IInputService _inputService;

		private float _farDistance;

		// Position and Orientation of camera.
		private Vector3 _defaultPosition = new Vector3(0, 2, 5);
		private float _defaultYaw;
		private float _defaultPitch;
		private float _currentYaw;
		private float _currentPitch;


		// This property is null while the CameraObject is not added to the game
		// object service.
		[Browsable(false)]
		public CameraNode CameraNode { get; private set; }

		public bool IsEnabled { get; set; }


		public CameraObject(IServiceProvider services)
		  : this(services, 1000)
		{
		}


		public CameraObject(IServiceProvider services, float farDistance)
		{
			Name = "Camera";

			_services = services;
			_inputService = services.GetService<IInputService>();

			IsEnabled = true;
			_farDistance = farDistance;

#if MONOGAME || WINDOWS_PHONE
			TouchPanel.EnabledGestures |= GestureType.FreeDrag;
#endif
		}


		// OnLoad() is called when the GameObject is added to the IGameObjectService.
		protected override void OnLoad()
		{
			// Create a camera node.
			CameraNode = new CameraNode(new Camera(new PerspectiveProjection()))
			{
				Name = "PlayerCamera"
			};

			// Add to scene.
			// (This is usually optional. Since cameras do not have a visual representation,
			// it  makes no difference if the camera is actually part of the scene graph or
			// not. - Except when other scene nodes are attached to the camera. In this case
			// the camera needs to be in the scene.)
			var scene = _services.GetService<IScene>();
			if (scene != null)
				scene.Children.Add(CameraNode);

			ResetPose();
			ResetProjection();
		}


		// OnUnload() is called when the GameObject is removed from the IGameObjectService.
		protected override void OnUnload()
		{
			if (CameraNode.Parent != null)
				CameraNode.Parent.Children.Remove(CameraNode);

			CameraNode.Dispose(false);
			CameraNode = null;
		}


		public void ResetPose(Vector3 position, float yaw, float pitch)
		{
			_defaultPosition = position;
			_defaultYaw = yaw;
			_defaultPitch = pitch;

			ResetPose();
		}


		public void ResetPose()
		{
			_currentYaw = _defaultYaw;
			_currentPitch = _defaultPitch;

			if (IsLoaded)
			{
				// Also update SceneNode.LastPose - this is required for some effect, like
				// object motion blur. 
				CameraNode.SetLastPose(true);

				CameraNode.PoseWorld = new Pose(
				  _defaultPosition,
				  MathHelper.CreateRotationY(_currentYaw) * MathHelper.CreateRotationX(_currentPitch));
			}
		}


		public void ResetProjection()
		{
			if (IsLoaded)
			{
				var graphicsService = _services.GetService<IGraphicsService>();
				var projection = (PerspectiveProjection)CameraNode.Camera.Projection;
				projection.SetFieldOfView(
				  ConstantsF.PiOver4,
				  graphicsService.GraphicsDevice.Viewport.AspectRatio,
				  0.1f,
				  _farDistance);
			}
		}


		// OnUpdate() is called once per frame.
		protected override void OnUpdate(TimeSpan deltaTime)
		{
			// Mouse centering (controlled by the MenuComponent) is disabled if the game
			// is inactive or if the GUI is active. In these cases, we do not want to move
			// the player.
			if (!IsEnabled)
				return;

			float deltaTimeF = (float)deltaTime.TotalSeconds;

			// Compute new orientation from mouse movement, gamepad and touch.
			Vector2 mousePositionDelta = _inputService.MousePositionDelta;
			GamePadState gamePadState = _inputService.GetGamePadState(LogicalPlayerIndex.One);
			Vector2 touchDelta = Vector2.Zero;
#if MONOGAME || WINDOWS_PHONE
			foreach (var gesture in _inputService.Gestures)
			{
				if (gesture.GestureType == GestureType.FreeDrag)
				{
					touchDelta += (Vector2)gesture.Delta;

					// If we have touch input, we ignore the mouse movement
					mousePositionDelta = Vector2.Zero;
				}
			}
#endif

#if WINDOWS_PHONE || IOS
      // On Windows Phone touch input also sets the mouse input. --> Ignore mouse data.
      mousePositionDelta = Vector2.Zero;
#endif

			if (_inputService.MouseState.RightButton == ButtonState.Pressed)
			{
				float deltaYaw = -mousePositionDelta.X - touchDelta.X - gamePadState.ThumbSticks.Right.X * ThumbStickFactor;
				_currentYaw += deltaYaw * deltaTimeF * AngularVelocityMagnitude;

				float deltaPitch = -mousePositionDelta.Y - touchDelta.Y + gamePadState.ThumbSticks.Right.Y * ThumbStickFactor;
				_currentPitch += deltaPitch * deltaTimeF * AngularVelocityMagnitude;

				// Limit the pitch angle to +/- 90°.
				_currentPitch = MathHelper.Clamp(_currentPitch, -ConstantsF.PiOver2, ConstantsF.PiOver2);
			}

			// Reset camera position if <Home> or <Right Stick> is pressed.
			if (_inputService.IsPressed(Keys.Home, false)
				|| _inputService.IsPressed(Buttons.RightStick, false, LogicalPlayerIndex.One))
			{
				ResetPose();
			}

			// Compute new orientation of the camera.
			Quaternion orientation = MathHelper.CreateRotationY(_currentYaw) * MathHelper.CreateRotationX(_currentPitch);

			// Create velocity from <W>, <A>, <S>, <D> and <R>, <F> keys. 
			// <R> or DPad up is used to move up ("rise"). 
			// <F> or DPad down is used to move down ("fall").
			Vector3 velocity = Vector3.Zero;
			KeyboardState keyboardState = _inputService.KeyboardState;
			if (keyboardState.IsKeyDown(Keys.W))
				velocity.Z--;
			if (keyboardState.IsKeyDown(Keys.S))
				velocity.Z++;
			if (keyboardState.IsKeyDown(Keys.A))
				velocity.X--;
			if (keyboardState.IsKeyDown(Keys.D))
				velocity.X++;
			if (keyboardState.IsKeyDown(Keys.R) || gamePadState.DPad.Up == ButtonState.Pressed)
				velocity.Y++;
			if (keyboardState.IsKeyDown(Keys.F) || gamePadState.DPad.Down == ButtonState.Pressed)
				velocity.Y--;

			// Add velocity from gamepad sticks.
			velocity.X += gamePadState.ThumbSticks.Left.X;
			velocity.Z -= gamePadState.ThumbSticks.Left.Y;

			// Rotate the velocity vector from view space to world space.
			velocity = orientation.Rotate(velocity);

			if (keyboardState.IsKeyDown(Keys.LeftShift))
				velocity *= SpeedBoost;

			// Multiply the velocity by time to get the translation for this frame.
			Vector3 translation = velocity * LinearVelocityMagnitude * deltaTimeF;

			// Update SceneNode.LastPoseWorld - this is required for some effects, like
			// camera motion blur. 
			CameraNode.LastPoseWorld = CameraNode.PoseWorld;

			// Set the new camera pose.
			CameraNode.PoseWorld = new Pose(
			  CameraNode.PoseWorld.Position + translation,
			  orientation);
		}
	}
}
