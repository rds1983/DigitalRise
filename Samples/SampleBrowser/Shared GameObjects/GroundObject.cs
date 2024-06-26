﻿using DigitalRise;
using DigitalRise.GameBase;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Physics;
using AssetManagementBase;
using DigitalRise.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Samples
{
	// Loads a ground plane model and creates a static rigid body for the ground plane.
  public class GroundObject : GameObject
  {
    private readonly IServiceProvider _services;
    private ModelNode _modelNode;
    private RigidBody _rigidBody;


    public GroundObject(IServiceProvider services)
    {
      _services = services;
      Name = "Ground";
    }


    // OnLoad() is called when the GameObject is added to the IGameObjectService.
    protected override void OnLoad()
    {
			// Load model.
			var assetManager = _services.GetService<AssetManager>();
			var graphicsService = _services.GetService<IGraphicsService>();
      _modelNode = assetManager.LoadDRModel(graphicsService, "Ground/Ground.drmdl").Clone();
			_modelNode.ScaleLocal = new Vector3(0.5f);

      foreach (var node in _modelNode.GetSubtree())
      {
        // Disable the CastsShadows flag for ground meshes. No need to render
        // this model into the shadow map. (This also avoids any shadow acne on 
        // the ground model.)
        node.CastsShadows = false;

        // If models will never move, set the IsStatic flag. This gives the engine 
        // more room for optimizations. Additionally, some effects, like certain 
        // decals, may only affect static geometry.
        node.IsStatic = true;
      }

      // Add model node to scene graph.
      var scene = _services.GetService<IScene>();
      scene.Children.Add(_modelNode);

      // Create rigid body.
      _rigidBody = new RigidBody(new PlaneShape(Vector3.UnitY, 0))
      {
        MotionType = MotionType.Static,
      };

      // Add rigid body to the physics simulation.
      var simulation = _services.GetService<Simulation>();
      simulation.RigidBodies.Add(_rigidBody);
    }


    // OnUnload() is called when the GameObject is removed from the IGameObjectService.
    protected override void OnUnload()
    {
      // Remove model and rigid body.
      _modelNode.Parent.Children.Remove(_modelNode);
      _modelNode.Dispose(false);
      _modelNode = null;

      _rigidBody.Simulation.RigidBodies.Remove(_rigidBody);
      _rigidBody = null;
    }
  }
}
