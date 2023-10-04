// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Physics.Constraints
{
  /// <summary>
  /// Wraps a 1-dimensional constraint (a row in the constraint solver matrix).
  /// </summary>
  //[Obfuscation(Feature = "controlflow")]
  internal class Constraint1D
  {
    public Vector3 JLinA;
    public Vector3 JAngA;
    public Vector3 JLinB;
    public Vector3 JAngB;
    public Vector3 WJTLinA;
    public Vector3 WJTAngA;
    public Vector3 WJTLinB;
    public Vector3 WJTAngB;
    public float JWJTInverse;
    public float TargetRelativeVelocity;
    public float Softness;
    public float ConstraintImpulse;


    /// <summary>
    /// Initializes the 1-dimensional constraint.
    /// </summary>
    public void Prepare(RigidBody bodyA, RigidBody bodyB, Vector3 jLinA, Vector3 jAngA, Vector3 jLinB, Vector3 jAngB)
    {
      JLinA = jLinA;
      JAngA = jAngA;
      JLinB = jLinB;
      JAngB = jAngB;

      WJTLinA.X = bodyA.MassInverse * jLinA.X;
      WJTLinA.Y = bodyA.MassInverse * jLinA.Y;
      WJTLinA.Z = bodyA.MassInverse * jLinA.Z;
      Matrix33F matrix = bodyA.InertiaInverseWorld;
      WJTAngA.X = matrix.M00 * jAngA.X + matrix.M01 * jAngA.Y + matrix.M02 * jAngA.Z;
      WJTAngA.Y = matrix.M10 * jAngA.X + matrix.M11 * jAngA.Y + matrix.M12 * jAngA.Z;
      WJTAngA.Z = matrix.M20 * jAngA.X + matrix.M21 * jAngA.Y + matrix.M22 * jAngA.Z;
      WJTLinB.X = bodyB.MassInverse * jLinB.X;
      WJTLinB.Y = bodyB.MassInverse * jLinB.Y;
      WJTLinB.Z = bodyB.MassInverse * jLinB.Z;
      matrix = bodyB.InertiaInverseWorld;
      WJTAngB.X = matrix.M00 * jAngB.X + matrix.M01 * jAngB.Y + matrix.M02 * jAngB.Z;
      WJTAngB.Y = matrix.M10 * jAngB.X + matrix.M11 * jAngB.Y + matrix.M12 * jAngB.Z;
      WJTAngB.Z = matrix.M20 * jAngB.X + matrix.M21 * jAngB.Y + matrix.M22 * jAngB.Z;

      float JWJT = jLinA.X * WJTLinA.X + jLinA.Y * WJTLinA.Y + jLinA.Z * WJTLinA.Z
                   + jAngA.X * WJTAngA.X + jAngA.Y * WJTAngA.Y + jAngA.Z * WJTAngA.Z
                   + jLinB.X * WJTLinB.X + jLinB.Y * WJTLinB.Y + jLinB.Z * WJTLinB.Z
                   + jAngB.X * WJTAngB.X + jAngB.Y * WJTAngB.Y + jAngB.Z * WJTAngB.Z;

      JWJT += Softness;
      JWJTInverse = 1 / JWJT;
    }


    /// <summary>
    /// Applies the given constraint impulse.
    /// </summary>
    public void ApplyImpulse(RigidBody bodyA, RigidBody bodyB, float impulse)
    {
      var linearVelocityA = bodyA.LinearVelocity;
      linearVelocityA.X += WJTLinA.X * impulse;
      linearVelocityA.Y += WJTLinA.Y * impulse;
      linearVelocityA.Z += WJTLinA.Z * impulse;
      bodyA.LinearVelocity = linearVelocityA;

      var angularVelocityA = bodyA.AngularVelocity;
      angularVelocityA.X += WJTAngA.X * impulse;
      angularVelocityA.Y += WJTAngA.Y * impulse;
      angularVelocityA.Z += WJTAngA.Z * impulse;
      bodyA.AngularVelocity = angularVelocityA;

      var linearVelocityB = bodyB.LinearVelocity;
      linearVelocityB.X += WJTLinB.X * impulse;
      linearVelocityB.Y += WJTLinB.Y * impulse;
      linearVelocityB.Z += WJTLinB.Z * impulse;
      bodyB.LinearVelocity = linearVelocityB;

      var angularVelocityB = bodyB.AngularVelocity;
      angularVelocityB.X += WJTAngB.X * impulse;
      angularVelocityB.Y += WJTAngB.Y * impulse;
      angularVelocityB.Z += WJTAngB.Z * impulse;
      bodyB.AngularVelocity = angularVelocityB;
    }


    /// <summary>
    /// Gets the relative constraint velocity (ignoring error correction velocities).
    /// </summary>
    public float GetRelativeVelocity(RigidBody bodyA, RigidBody bodyB)
    {
      Vector3 linearVelocityA = bodyA.LinearVelocity;
      Vector3 angularVelocityA = bodyA.AngularVelocity;
      Vector3 linearVelocityB = bodyB.LinearVelocity;
      Vector3 angularVelocityB = bodyB.AngularVelocity;
      return JLinA.X * linearVelocityA.X + JLinA.Y * linearVelocityA.Y + JLinA.Z * linearVelocityA.Z
             + JAngA.X * angularVelocityA.X + JAngA.Y * angularVelocityA.Y + JAngA.Z * angularVelocityA.Z
             + JLinB.X * linearVelocityB.X + JLinB.Y * linearVelocityB.Y + JLinB.Z * linearVelocityB.Z
             + JAngB.X * angularVelocityB.X + JAngB.Y * angularVelocityB.Y + JAngB.Z * angularVelocityB.Z;
    }


    /// <summary>
    /// Satisfies a general constraint.
    /// </summary>
    public float SatisfyConstraint(RigidBody bodyA, RigidBody bodyB, float relativeVelocity, float minImpulseLimit, float maxImpulseLimit)
    {
      float impulse = JWJTInverse * (TargetRelativeVelocity - relativeVelocity - Softness * ConstraintImpulse);
      var oldCachedNormalImpulse = ConstraintImpulse;
      ConstraintImpulse = MathHelper.Clamp(ConstraintImpulse + impulse, minImpulseLimit, maxImpulseLimit);
      impulse = ConstraintImpulse - oldCachedNormalImpulse;

      ApplyImpulse(bodyA, bodyB, impulse);

      return impulse;
    }
  }
}
