﻿using System.Linq;
using DigitalRise.Animation.Character;
using DigitalRise.Geometry;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AssetManagementBase;


namespace Samples.Animation
{
  [Sample(SampleCategory.Animation,
    "This sample shows how to use a TwoJointIKSolver for foot placement.",
    "",
    74)]
  [Controls(@"Sample
  Press <4>-<9> on the numpad to move the target.
  Press <Space> to limit the rotation of the foot.")]
  public class TwoJointIKSample : CharacterAnimationSample
  {
    private readonly MeshNode _meshNode;

    private Vector3 _targetPosition = new Vector3(0, 0.2f, 0.2f);
    private readonly TwoJointIKSolver _ikSolver;


    public TwoJointIKSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      var modelNode = AssetManager.LoadDRModel(GraphicsService, "Dude/Dude.drmdl");
      _meshNode = modelNode.FindFirstMeshNode().Clone();
      _meshNode.PoseLocal = new Pose(new Vector3(0, 0, 0));
      SampleHelper.EnablePerPixelLighting(_meshNode);
      GraphicsScreen.Scene.Children.Add(_meshNode);

      // Create the IK solver. The TwoJointIkSolver is usually used for arms and legs.
      // it modifies two bones and supports limits for the second bone. 
      _ikSolver = new TwoJointIKSolver
      {
        SkeletonPose = _meshNode.SkeletonPose,

        // The chain starts at the upper leg.
        RootBoneIndex = 55,

        // The second bone modified bone is the lower leg.
        HingeBoneIndex = 56,

        // The chain ends at the foot bone.
        TipBoneIndex = 57,

        // The direction of the hinge axis (in bone space).
        HingeAxis = -Vector3.UnitZ,

        // The hinge limits.
        MinHingeAngle = 0,
        MaxHingeAngle = ConstantsF.PiOver2,

        // The offset from the ankle to the bottom of the foot.
        TipOffset = new Vector3(0.23f, 0, 0),
      };
    }


    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      // ----- Move target if <NumPad4-9> are pressed.
      Vector3 translation = new Vector3();
      if (InputService.IsDown(Keys.NumPad4))
        translation.X -= 1;
      if (InputService.IsDown(Keys.NumPad6))
        translation.X += 1;
      if (InputService.IsDown(Keys.NumPad8))
        translation.Y += 1;
      if (InputService.IsDown(Keys.NumPad5))
        translation.Y -= 1;
      if (InputService.IsDown(Keys.NumPad9))
        translation.Z += 1;
      if (InputService.IsDown(Keys.NumPad7))
        translation.Z -= 1;

      translation = translation * deltaTime;
      _targetPosition += translation;

      // Convert target world space position to model space. - The IK solvers work in model space.
      Vector3 localTargetPosition = _meshNode.PoseWorld.ToLocalPosition(_targetPosition);

      // Reset the affected bones. This is optional. It removes unwanted twist from the bones.
      _meshNode.SkeletonPose.ResetBoneTransforms(_ikSolver.RootBoneIndex, _ikSolver.TipBoneIndex);

      _ikSolver.Target = localTargetPosition;

      // We can set a target orientation for the tip bone. This can be used to place the
      // foot correctly on an inclined plane.
      if (InputService.IsDown(Keys.Space))
        _ikSolver.TipBoneOrientation = _meshNode.SkeletonPose.GetBonePoseAbsolute(56).Rotation;
      else
        _ikSolver.TipBoneOrientation = null;

      // Let IK solver update the bones.
      _ikSolver.Solve(deltaTime);

      // Draws the IK target.
      var debugRenderer = GraphicsScreen.DebugRenderer;
      debugRenderer.Clear();
      debugRenderer.DrawAxes(new Pose(_targetPosition), 0.1f, false);
    }
  }
}
