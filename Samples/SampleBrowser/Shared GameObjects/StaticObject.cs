using DigitalRise;
using DigitalRise.GameBase;
using DigitalRise.Geometry;
using DigitalRise.Geometry.Meshes;
using DigitalRise.Geometry.Partitioning;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Graphics;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Physics;
using AssetManagementBase;
using Microsoft.Xna.Framework;
using System;

namespace Samples
{
  // Loads a model and places it in the scene at a user-defined scale and position.
  // Optionally, a static rigid body is created using the triangle mesh. (Note: Using
  // triangle meshes for collision detection is the slowest method. It is not recommended
  // to use simpler collision shapes if possible.)
  public class StaticObject : GameObject
  {
    private readonly IServiceProvider _services;
    private readonly string _assetName;
    private readonly Vector3 _scale;
    private readonly Pose _pose;
    private readonly bool _castsShadows;
    private readonly bool _addRigidBody;
    private ModelNode _modelNode;
    private RigidBody _rigidBody;


    public StaticObject(IServiceProvider services, string assetName, float scale, Pose pose)
      : this(services, assetName, new Vector3(scale), pose, true, false)
    {
    }


    public StaticObject(IServiceProvider services, string assetName, Vector3 scale, Pose pose, bool castsShadows, bool addRigidBody)
    {
      _services = services;
      _assetName = assetName;
      _scale = scale;
      _pose = pose;
      _castsShadows = castsShadows;
      _addRigidBody = addRigidBody;
    }


    // OnLoad() is called when the GameObject is added to the IGameObjectService.
    protected override void OnLoad()
    {
      // Load model.
      var assetManager = _services.GetService<AssetManager>();
      _modelNode = assetManager.LoadDRModel(_services.GetService<IGraphicsService>(), _assetName).Clone();

      // Optional: Create rigid body using the triangle mesh of the model.
      if (_addRigidBody)
        CreateRigidBody();

      _modelNode.ScaleLocal = _scale;
      _modelNode.PoseLocal = _pose;

      foreach (var node in _modelNode.GetSubtree())
      {
        node.CastsShadows = _castsShadows;

        // If models will never move, set the IsStatic flag. This gives the engine 
        // more room for optimizations. Additionally, some effects, like certain 
        // decals, may only affect static geometry.
        node.IsStatic = true;
      }

      // Add model node to scene graph.
      var scene = _services.GetService<IScene>();
      scene.Children.Add(_modelNode);
    }


    private void CreateRigidBody()
    {
      var triangleMesh = new TriangleMesh();

      foreach (var meshNode in _modelNode.MeshNodes())
      {
        // Extract the triangle mesh from the DigitalRise Graphics Mesh instance. 
        var subTriangleMesh = new TriangleMesh();
        foreach (var submesh in meshNode.Mesh.Submeshes)
        {
          submesh.ToTriangleMesh(subTriangleMesh);
        }

        // Apply the transformation of the mesh node.
        subTriangleMesh.Transform(meshNode.PoseWorld * Matrix44F.CreateScale(meshNode.ScaleWorld));

        // Combine into final triangle mesh.
        triangleMesh.Add(subTriangleMesh);
      }

      // Create a collision shape that uses the mesh.
      var triangleMeshShape = new TriangleMeshShape(triangleMesh);

      // Optional: Assign a spatial partitioning scheme to the triangle mesh. (A spatial partition
      // adds an additional memory overhead, but it improves collision detection speed tremendously!)
      triangleMeshShape.Partition = new CompressedAabbTree
      {
        // The tree is automatically built using a mixed top-down/bottom-up approach. Bottom-up
        // building is slower but produces better trees. If the tree building takes too long,
        // we can lower the BottomUpBuildThreshold (default is 128).
        BottomUpBuildThreshold = 0,
      };

      _rigidBody = new RigidBody(triangleMeshShape, new MassFrame(), null)
      {
        Pose = _pose,
        Scale = _scale,
        MotionType = MotionType.Static
      };

      // Add rigid body to physics simulation and model to scene.
      var simulation = _services.GetService<Simulation>();
      simulation.RigidBodies.Add(_rigidBody);
    }


    // OnUnload() is called when the GameObject is removed from the IGameObjectService.
    protected override void OnUnload()
    {
      // Remove model from scene graph.
      _modelNode.Parent.Children.Remove(_modelNode);
      _modelNode.Dispose(false);
      _modelNode = null;

      if (_rigidBody != null)
      {
        _rigidBody.Simulation.RigidBodies.Remove(_rigidBody);
        _rigidBody = null;
      }
    }
  }
}
