﻿using DigitalRise.Geometry;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Statistics;
using DigitalRise.Physics;
using DigitalRise.Physics.ForceEffects;
using DigitalRise.Physics.Materials;
using Microsoft.Xna.Framework;

namespace Samples.Physics
{
  [Sample(SampleCategory.Physics,
    "This sample demonstrates how to create a material with surface motion to simulate a conveyor belt.",
    "",
    9)]
  public class SurfaceMotionSample : PhysicsSample
  {
    public SurfaceMotionSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      // Add basic force effects.
      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());

      // Add a ground plane.
      RigidBody groundPlane = new RigidBody(new PlaneShape(Vector3.UnitY, 0))
      {
        Name = "GroundPlane",           // Names are not required but helpful for debugging.
        MotionType = MotionType.Static,
      };
      Simulation.RigidBodies.Add(groundPlane);

      // Create a material with surface motion.
      UniformMaterial material = new UniformMaterial("ConveyorBelt", true)  // Important: The second parameter enables the surface
      {                                                                     // motion. It has to be set to true in the constructor!
        SurfaceMotion = new Vector3(-1, 0, 0),  // The surface motion relative to the object.
      };

      // Create conveyor belt.
      RigidBody conveyorBelt = new RigidBody(new BoxShape(8, 0.51f, 1.1f), null, material)
      {
        Pose = new Pose(new Vector3(0, 0.25f, 0)),

        // If the conveyor belt is dynamic, it would "drive away" ;-).
        // Therefore, we make it static or kinematic so that it stays in place.
        MotionType = MotionType.Kinematic,
      };
      Simulation.RigidBodies.Add(conveyorBelt);

      // Two static boxes on the sides.
      RigidBody body0 = new RigidBody(new BoxShape(8, 0.5f, 0.8f))
      {
        Pose = new Pose(new Vector3(0, 0.25f, -0.6f - 0.4f))
      };
      Simulation.RigidBodies.Add(body0);

      RigidBody body1 = new RigidBody(new BoxShape(8, 0.5f, 0.8f))
      {
        Pose = new Pose(new Vector3(0, 0.25f, 0.6f + 0.4f))
      };
      Simulation.RigidBodies.Add(body1);

      // Add a few random boxes at the top of the conveyor.
      BoxShape boxShape = new BoxShape(0.6f, 0.6f, 0.6f);
      for (int i = 0; i < 20; i++)
      {
        Vector3 randomPosition = new Vector3(
          RandomHelper.Random.NextFloat(-4, 4),
          RandomHelper.Random.NextFloat(1, 3),
          RandomHelper.Random.NextFloat(-1, 1));
        Quaternion randomOrientation = RandomHelper.Random.NextQuaternion();

        RigidBody body = new RigidBody(boxShape)
        {
          Pose = new Pose(randomPosition, randomOrientation),
        };

        Simulation.RigidBodies.Add(body);
      }
    }
  }
}
