﻿using System;
using System.Linq;
using DigitalRise.Animation;
using DigitalRise.Animation.Character;
using DigitalRise.GameBase;
using DigitalRise.Geometry;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using CommonServiceLocator;
using Microsoft.Xna.Framework.Content;
using AssetManagementBase;
using DigitalRise.Graphics;

namespace Samples
{
  // Loads a skinned model and starts an animation.
  public class DudeObject : GameObject
  {
    private readonly IServiceLocator _services;
    private readonly string _assetName;
    private Pose _defaultPose;
    private ModelNode _modelNode;
    

    public Pose Pose 
    { 
      get { return _modelNode != null ? _modelNode.PoseWorld : _defaultPose; }
      set
      {
        _defaultPose = value;
        if (_modelNode != null)
          _modelNode.PoseWorld = value;
      }
    }


    public AnimationController AnimationController { get; private set; }


    public DudeObject(IServiceLocator services) 
      : this(services, "Dude/dude.drmdl")
    {
    }


    public DudeObject(IServiceLocator services, string assetName)
    {
      _services = services;
      _assetName = assetName;
      _defaultPose = new Pose(new Vector3F(-1, 0, 0), Matrix33F.CreateRotationY(ConstantsF.Pi));
    }


    // OnLoad() is called when the GameObject is added to the IGameObjectService.
    protected override void OnLoad()
    {
      var contentManager = _services.GetInstance<AssetManager>();
      var graphicsService = _services.GetInstance<IGraphicsService>();

      _modelNode = contentManager.LoadDRModel(graphicsService, _assetName).Clone();
      _modelNode.PoseWorld = _defaultPose;
      SampleHelper.EnablePerPixelLighting(_modelNode);

      var scene = _services.GetInstance<IScene>();
      scene.Children.Add(_modelNode);

      // Create looping animation.
      var meshNode = _modelNode.FindFirstMeshNode();   // The dude model has a single mesh node as its child.
      var animations = meshNode.Mesh.Animations;
      var animationClip = new AnimationClip<SkeletonPose>(animations.Values.First())
      {
        LoopBehavior = LoopBehavior.Cycle,  // Repeat animation...
        Duration = TimeSpan.MaxValue,       // ...forever.
      };

      // Start animation.
      var animationService = _services.GetInstance<IAnimationService>();
      AnimationController = animationService.StartAnimation(animationClip, (IAnimatableProperty)meshNode.SkeletonPose);
      AnimationController.UpdateAndApply();
    }


    // OnUnload() is called when the GameObject is removed from the IGameObjectService.
    protected override void OnUnload()
    {
      AnimationController.Stop();
      AnimationController.Recycle();

      _modelNode.Parent.Children.Remove(_modelNode);
      _modelNode.Dispose(false);
      _modelNode = null;
    }
  }
}