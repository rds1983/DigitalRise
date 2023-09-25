using System;
using System.Linq;
using DigitalRune.Animation;
using DigitalRune.Animation.Character;
using DigitalRune.Geometry;
using DigitalRune.Graphics.SceneGraph;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AssetManagementBase;


namespace Samples.Animation
{
  [Sample(SampleCategory.Animation,
    @"This sample shows how to use a SkeletonMapper to retarget an animation from the
Dude skeleton (left) to the Marine skeleton (right).",
    @"",
    60)]
  public class SkeletonMappingSample : CharacterAnimationSample
  {
    private readonly MeshNode _dudeMeshNode;
    private readonly MeshNode _marineMeshNode;
    private SkeletonMapper _skeletonMapper;


    public SkeletonMappingSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      // Get dude model and start animation on the dude.
      var modelNode = AssetManager.LoadDRModel(GraphicsService, "Dude/Dude.drmdl");
      _dudeMeshNode = modelNode.FindFirstMeshNode().Clone();
      _dudeMeshNode.PoseLocal = new Pose(new Vector3F(-0.5f, 0, 0), Matrix33F.CreateRotationY(ConstantsF.Pi));
      SampleHelper.EnablePerPixelLighting(_dudeMeshNode);
      GraphicsScreen.Scene.Children.Add(_dudeMeshNode);

      var animations = _dudeMeshNode.Mesh.Animations;
      var loopingAnimation = new AnimationClip<SkeletonPose>(animations.Values.First())
      {
        LoopBehavior = LoopBehavior.Cycle,
        Duration = TimeSpan.MaxValue,
      };
      AnimationService.StartAnimation(loopingAnimation, (IAnimatableProperty)_dudeMeshNode.SkeletonPose);

      // Get marine model - do not start any animations on the marine model.
      modelNode = AssetManager.LoadDRModel(GraphicsService, "Marine/PlayerMarine.drmdl");
      _marineMeshNode = modelNode.FindFirstMeshNode().Clone();
      _marineMeshNode.PoseLocal = new Pose(new Vector3F(0.5f, 0, 0), Matrix33F.CreateRotationY(ConstantsF.Pi));
      SampleHelper.EnablePerPixelLighting(_marineMeshNode);
      GraphicsScreen.Scene.Children.Add(_marineMeshNode);

      CreateSkeletonMapper();
    }


    private void CreateSkeletonMapper()
    {
      // Create an empty skeleton mapper that can map bone transforms between the dude and 
      // the marine model. 
      _skeletonMapper = new SkeletonMapper(_dudeMeshNode.SkeletonPose, _marineMeshNode.SkeletonPose);

      // A skeleton mapper manages a collection of bone mappers, that map individual bones. 
      // Without the bone mappers, the SkeletonMapper does nothing.
      // Setting up the right bone mappers is not trivial if the skeletons are very different.
      //
      // Here are a few tips:
      // 
      // Display the skeletons of both models (as in the BindPoseSample.cs).
      // Compare the bones of the skeletons and find out which bones and which bone chains should
      // map to which bones or bone chains of the other model.
      //
      // Use a DirectBoneMapper for the first bones in the pelvis. Set MapAbsoluteTransforms to 
      // false and set MapTranslations to true to map translations (hip swing). If the models 
      // have different size then adapt ScaleAToB manually to scale the mapped translations.
      //
      // ChainBoneMappers can be used to map single bones or bone chains. In the case below
      // several spine bones of the dude are mapped to a single spine bone of the marine.
      //
      // Chain bone mappers need a start and end bone (which is excluded and only defines the 
      // end of the chain). Therefore, use DirectBoneMappers for end bones (hands, feet, head).
      //
      // If the arm bind poses are very different (e.g. a bind pose with horizontal arms vs a bind
      // pose with lowered arms), you must use ChainBoneMappers for the upper arm and lower arm
      // bones.
      //
      // Experiment until you find a good mapping. In some cases it is necessary to define one
      // bone mapping to map from the first to the second skeleton and use a different bone mapping
      // to map in the reverse direction. Set BoneMapper.Direction if a specific bone mapper
      // instance should only be used for a specific mapping direction.

      _skeletonMapper.BoneMappers.Add(
        new DirectBoneMapper(2, 2)
        {
          MapAbsoluteTransforms = false,
          MapTranslations = true,
          ScaleAToB = 1f,
        });


      // Spine:
      _skeletonMapper.BoneMappers.Add(new ChainBoneMapper(4, 7, 3, 4));

      // Clavicle
      _skeletonMapper.BoneMappers.Add(new DirectBoneMapper(13, 8) { MapAbsoluteTransforms = false, MapTranslations = false, });
      _skeletonMapper.BoneMappers.Add(new DirectBoneMapper(32, 14) { MapAbsoluteTransforms = false, MapTranslations = false, });

      // Left Leg
      _skeletonMapper.BoneMappers.Add(new ChainBoneMapper(51, 52, 17, 18));
      _skeletonMapper.BoneMappers.Add(new ChainBoneMapper(52, 53, 18, 19));
      _skeletonMapper.BoneMappers.Add(new DirectBoneMapper(53, 19) { MapAbsoluteTransforms = false, MapTranslations = false, });

      // Right Leg
      _skeletonMapper.BoneMappers.Add(new ChainBoneMapper(55, 56, 22, 23));
      _skeletonMapper.BoneMappers.Add(new ChainBoneMapper(56, 57, 23, 24));
      _skeletonMapper.BoneMappers.Add(new DirectBoneMapper(57, 24) { MapAbsoluteTransforms = false, MapTranslations = false, });

      // Left Arm
      _skeletonMapper.BoneMappers.Add(new ChainBoneMapper(14, 15, 8, 9));
      _skeletonMapper.BoneMappers.Add(new ChainBoneMapper(15, 16, 9, 10));
      _skeletonMapper.BoneMappers.Add(new DirectBoneMapper(16, 10) { MapTranslations = false, });

      // Right Arm
      _skeletonMapper.BoneMappers.Add(new ChainBoneMapper(33, 34, 13, 15));
      _skeletonMapper.BoneMappers.Add(new ChainBoneMapper(34, 35, 14, 16));
      _skeletonMapper.BoneMappers.Add(new DirectBoneMapper(35, 15) { MapTranslations = false, });

      // Neck, Head
      _skeletonMapper.BoneMappers.Add(new ChainBoneMapper(7, 8, 4, 5));
      _skeletonMapper.BoneMappers.Add(new DirectBoneMapper(8, 5) { MapAbsoluteTransforms = true, MapTranslations = false, });
    }


    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      // <Space> --> Reset skeleton pose of dude.
      if (InputService.IsDown(Keys.Space))
        _dudeMeshNode.SkeletonPose.ResetBoneTransforms();

      // Map the bone transforms of the dude skeleton pose to the marine skeleton pose.
      _skeletonMapper.MapAToB();
    }
  }
}
