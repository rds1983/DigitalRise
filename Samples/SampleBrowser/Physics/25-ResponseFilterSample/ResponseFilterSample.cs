using DigitalRise.Geometry;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Statistics;
using DigitalRise.Physics;
using DigitalRise.Physics.ForceEffects;
using Microsoft.Xna.Framework;

namespace Samples.Physics
{
  [Sample(SampleCategory.Physics,
    @"This sample demonstrates collision response filtering. Capsules and boxes are dropped
at random positions. Collision response between capsules and boxes is disabled by using
a custom filter class.",
    @"",
    25)]
  public class ResponseFilterSample : PhysicsSample
  {
    public ResponseFilterSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      // Add basic force effects.
      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());

      // Only disable collision response if you need collision detection info but no collision
      // response. If you can disable collision detection too, use 
      //   Simulation.CollisionDomain.CollisionDetection.CollisionFilter 
      // instead - this is more efficient!
      // (In this sample, a custom filter implementation is used. DigitalRise.Physics provides
      // a standard filter implementation: DigitalRise.Physics.CollisionResponseFilter.)
      Simulation.ResponseFilter = new MyCollisionResponseFilter();

      // Add a ground plane.
      RigidBody groundPlane = new RigidBody(new PlaneShape(Vector3.UnitY, 0))
      {
        Name = "GroundPlane",           // Names are not required but helpful for debugging.
        MotionType = MotionType.Static,
      };
      Simulation.RigidBodies.Add(groundPlane);

      // ----- Add boxes at random poses.
      BoxShape boxShape = new BoxShape(0.5f, 0.8f, 1.2f);
      for (int i = 0; i < 20; i++)
      {
        Vector3 position = RandomHelper.Random.NextVector3(-3, 3);
        position.Y = 5;
        Quaternion orientation = RandomHelper.Random.NextQuaternion();

        RigidBody body = new RigidBody(boxShape)
        {
          Pose = new Pose(position, orientation),
        };
        Simulation.RigidBodies.Add(body);
      }

      // ----- Add capsules at random poses.
      CapsuleShape capsuleShape = new CapsuleShape(0.3f, 1.2f);
      for (int i = 0; i < 20; i++)
      {
        Vector3 position = RandomHelper.Random.NextVector3(-3, 3);
        position.Y = 5;
        Quaternion orientation = RandomHelper.Random.NextQuaternion();

        RigidBody body = new RigidBody(capsuleShape)
        {
          Pose = new Pose(position, orientation),
        };
        Simulation.RigidBodies.Add(body);
      }
    }
  }
}
