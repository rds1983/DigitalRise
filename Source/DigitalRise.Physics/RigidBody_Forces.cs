// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using DigitalRise.Mathematics.Algebra;
using DigitalRise.Physics.ForceEffects;
using Microsoft.Xna.Framework;

namespace DigitalRise.Physics
{
  public partial class RigidBody
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    // Force effects must use different AddForce() method, so that the user set forces are not deleted.
    // Accumulated external force (gravity, explosions, etc.) without constraint forces (from hinges, contacts, ...)
    // AccumulatedForce = UserForce + force of force effects
    // AccumulatedForce is not reset at the end of a time step.
    internal Vector3 AccumulatedForce;
    internal Vector3 AccumulatedTorque;

    // User can set a UserForce with AddForce(). This force is not cleared between sub-timesteps.
    // But it is cleared at the end of the time step. To apply a permanent force call AddForce()
    // each frame or use a ForceEffect
    internal Vector3 UserForce;
    internal Vector3 UserTorque;
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <summary>
    /// Clears all forces that were set with <see cref="AddForce(Vector3, Vector3)"/>,
    /// <see cref="AddForce(Vector3)"/> or <see cref="AddTorque"/>.
    /// </summary>
    /// <remarks>
    /// Forces are automatically cleared at the end of a simulation time step.
    /// </remarks>
    public void ClearForces()
    {
      UserForce = new Vector3();
      UserTorque = new Vector3();
    }


    /// <overloads>
    /// <summary>
    /// Applies a force to the rigid body.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Applies a force to the rigid body.
    /// </summary>
    /// <param name="forceWorld">The force in world space.</param>
    /// <param name="positionWorld">
    /// The position where the force is applied in world space.
    /// </param>
    /// <remarks>
    /// The force will influence the body in the next time step. The same force is applied for all
    /// internal sub time steps and the forces will be cleared at the end of a time step. If a
    /// permanent force should act on the rigid body, the method 
    /// <see cref="AddForce(Vector3,Vector3)"/> must be called before each time step - or a 
    /// <see cref="ForceEffect"/> can be used instead.
    /// </remarks>
    public void AddForce(Vector3 forceWorld, Vector3 positionWorld)
    {
      Vector3 radius = positionWorld - PoseCenterOfMass.Position;
      UserForce += forceWorld;
      UserTorque += Vector3.Cross(radius, forceWorld);
    }


    /// <summary>
    /// Applies a force at the center of mass of the rigid body.
    /// </summary>
    /// <param name="forceWorld">The force in world space.</param>
    /// <remarks>
    /// The force will influence the body in the next time step. The same force is applied for all
    /// internal sub time steps and the forces will be cleared at the end of a time step. If a
    /// permanent force should act on the rigid body, the method 
    /// <see cref="AddForce(Vector3)"/> must be called before each time step - or a 
    /// <see cref="ForceEffect"/> can be used instead.
    /// </remarks>
    public void AddForce(Vector3 forceWorld)
    {
      UserForce += forceWorld;
    }


    /// <summary>
    /// Applies a torque at the center of mass of the rigid body.
    /// </summary>
    /// <param name="torqueWorld">The torque in world space.</param>
    /// <remarks>
    /// The torque will influence the body in the next time step. The same torque is applied for all
    /// internal sub time steps and the torques will be cleared at the end of a time step. If a
    /// permanent torque should act on the rigid body, the method <see cref="AddTorque"/> must be
    /// called before each time step - or a <see cref="ForceEffect"/> can be used instead.
    /// </remarks>
    public void AddTorque(Vector3 torqueWorld)
    {
      UserTorque += torqueWorld;
    }
    #endregion
  }
}
