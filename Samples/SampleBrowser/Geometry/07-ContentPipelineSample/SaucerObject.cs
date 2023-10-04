using System;
using DigitalRise.GameBase;
using DigitalRise.Geometry;
using DigitalRise.Geometry.Collisions;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using CommonServiceLocator;
using AssetManagementBase;
using DigitalRise.Graphics;
using Microsoft.Xna.Framework;

namespace Samples
{
	// Represents the "Saucer" model.
  public class SaucerObject : GameObject
  {
    private readonly IServiceLocator _services;
    private ModelNode _modelNode;
    private GeometricObject _geometricObject;
    private CollisionObject _collisionObject;


    // The collision object which can be used for contact queries.
    public CollisionObject CollisionObject
    {
      get { return _collisionObject; }
    }


    // The position and orientation of the saucer.
    public Pose Pose
    {
      get { return _modelNode.PoseWorld; }
      set
      {
        _modelNode.PoseWorld = value;
        _geometricObject.Pose = value;
      }
    }


    public SaucerObject(IServiceLocator services)
    {
      _services = services;
    }


    protected override void OnLoad()
    {
			var assetManager = _services.GetInstance<AssetManager>();
			var graphicsService = _services.GetInstance<IGraphicsService>();

      // ----- Graphics
      // Load graphics model (created using the ModelWithCollisionMeshProcessor).
      var sharedModelNode = assetManager.LoadDRModel(graphicsService, "Saucer/saucer.drmdl");

      // Let's create a clone because we do not want to change the shared Saucer 
      // instance stored in the content manager.
      _modelNode = sharedModelNode.Clone();

      _modelNode.PoseWorld = new Pose(Vector3.Zero, Matrix33F.CreateRotationY(-ConstantsF.PiOver2));

      // The collision shape is stored in the UserData.
      var shape = (Shape)_modelNode.UserData;

      // Add model to the scene for rendering.
      var scene = _services.GetInstance<IScene>();
      scene.Children.Add(_modelNode);

      // ----- Collision Detection
      // Create a collision object and add it to the collision domain.
      _geometricObject = new GeometricObject(shape, _modelNode.PoseWorld);
      _collisionObject = new CollisionObject(_geometricObject);

      // Important: We do not need detailed contact information when a collision
      // is detected. The information of whether we have contact or not is sufficient.
      // Therefore, we can set the type to "Trigger". This increases the performance 
      // dramatically.
      _collisionObject.Type = CollisionObjectType.Trigger;

      var collisionDomain = _services.GetInstance<CollisionDomain>();
      collisionDomain.CollisionObjects.Add(_collisionObject);
    }


    protected override void OnUnload()
    {
      // Remove the collision object from the collision domain.
      var collisionDomain = _collisionObject.Domain;
      collisionDomain.CollisionObjects.Remove(_collisionObject);

      // Detach objects to avoid any "memory leaks".
      _collisionObject.GeometricObject = null;
      _geometricObject.Shape = Shape.Empty;

      // Remove the model from the scene.
      _modelNode.Parent.Children.Remove(_modelNode);
      _modelNode.Dispose(false);
    }


    protected override void OnUpdate(TimeSpan deltaTime)
    {
    }
  }
}
