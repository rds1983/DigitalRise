using DigitalRise;
using DigitalRise.GameBase;
using DigitalRise.Graphics;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.LevelEditor.Utility;
using System;
using System.ComponentModel;

namespace DigitalRise.LevelEditor
{
    // Adds distance and height-based fog. Fog is disabled by default.
    public class FogObject : GameObject
	{
		private readonly IServiceProvider _services;


		[Browsable(false)]
		public FogNode FogNode { get; private set; }


		/// <summary>
		/// Gets or sets a value indicating whether the fog node is attached to the camera.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if fog node is attached to the camera; otherwise, 
		/// <see langword="false"/>.
		/// </value>
		/// <remarks>
		/// Optionally, we can move the fog node with the camera node. If camera and 
		/// fog are independent, then the camera can fly up and "escape" the height-based 
		/// fog. If camera and fog move together, then the fog will always have the
		/// same height at the horizon (e.g. to hide the horizon).
		/// </remarks>
		public bool AttachToCamera
		{
			get { return _attachToCamera; }
			set
			{
				if (value == _attachToCamera)
					return;

				_attachToCamera = value;

				if (FogNode != null)
				{
					// Remove fog node from existing parent and re-add.
					if (FogNode.Parent != null)
						FogNode.Parent.Children.Remove(FogNode);

					AddFogNodeToScene();
				}
			}
		}
		private bool _attachToCamera;


		public FogObject(IServiceProvider services)
		{
			_services = services;
			Name = "Fog";
		}


		// OnLoad() is called when the GameObject is added to the IGameObjectService.
		protected override void OnLoad()
		{
			FogNode = new FogNode(new Fog())
			{
				IsEnabled = false,
				Name = "Fog",
			};

			AddFogNodeToScene();
		}


		private void AddFogNodeToScene()
		{
			var scene = _services.GetService<IScene>();
			if (!_attachToCamera)
			{
				scene.Children.Add(FogNode);
			}
			else
			{
				var cameraNode = ((Scene)scene).GetSceneNode("PlayerCamera");
				if (cameraNode.Children == null)
					cameraNode.Children = new SceneNodeCollection();

				cameraNode.Children.Add(FogNode);
			}
		}


		// OnUnload() is called when the GameObject is removed from the IGameObjectService.
		protected override void OnUnload()
		{
			FogNode.Parent.Children.Remove(FogNode);
			FogNode.Dispose(false);
			FogNode = null;
		}
	}
}
