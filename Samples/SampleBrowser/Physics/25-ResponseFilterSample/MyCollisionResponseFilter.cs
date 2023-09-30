using System;
using DigitalRise.Collections;
using DigitalRise.Geometry.Partitioning;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Physics;


namespace Samples.Physics
{
  /// <summary>
  /// A custom collision response filter.
  /// </summary>
  /// <remarks>
  /// Any class implementing IPairFilter of RigidBody can be used as a collision response filter.
  /// </remarks>
  public class MyCollisionResponseFilter : IPairFilter<RigidBody>
  {
    public bool Filter(Pair<RigidBody> pair)
    {
      // Just to demonstrate the effect of a collision response filter:
      // Disable collision response between capsule and box.
      // Enable collision response for all other body pairs.

      if (pair.First.Shape is CapsuleShape && pair.Second.Shape is BoxShape)
        return false;
      if (pair.First.Shape is BoxShape && pair.Second.Shape is CapsuleShape)
        return false;

      return true;
    }

    // Not required:
    public event EventHandler<EventArgs> Changed { add { } remove { } }
  }
}