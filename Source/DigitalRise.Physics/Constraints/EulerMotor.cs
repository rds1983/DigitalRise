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
  /// Defines a motor that controls the relative orientation of two constrained bodies using Euler
  /// angle.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The motor pushes both bodies until the relative orientation of the constraint anchor on the
  /// second body relative to the constraint anchor on the first body is equal to the three Euler
  /// angles defined in <see cref="TargetAngles"/>. The motor acts like a damped-spring that rotates 
  /// the bodies (controlled by <see cref="SpringConstant"/> and <see cref="DampingConstant"/>).
  /// The three Euler angles are defined as in the <see cref="AngularLimit"/>.
  /// </para>
  /// <para>
  /// The target orientation is defined using Euler angles. In contrast, 
  /// <see cref="QuaternionMotor"/> is a motor that controls the orientation where the target 
  /// orientation is defined using a <see cref="Quaternion"/>.
  /// </para>
  /// <para>
  /// <strong>Euler Angles:</strong>
  /// The <see cref="EulerMotor"/> uses the same Euler angles as the <see cref="AngularLimit"/>.
  /// </para>
  /// <para>
  /// The Euler angles are computed for following order of rotations: The first rotations
  /// is about the x-axis. The second rotation is about the rotated y-axis after the first 
  /// rotation. The last rotation is about the final z-axis.
  /// </para>
  /// <para>
  /// The Euler angles are unique if the second angle is less than +/- 90°. The limits for the
  /// rotation angles are [-180°, 180°] for the first and the third angle. And the limit for the
  /// second angle is [-90°, 90°].
  /// </para>
  /// <para>
  /// Use <see cref="ConstraintHelper.GetEulerAngles"/> to get the Euler angles of a given rotation.
  /// </para>
  /// </remarks>
  public class EulerMotor : Constraint
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    private Vector3 _minImpulseLimits;
    private Vector3 _maxImpulseLimits;
    private readonly Constraint1D[] _constraints =
    {
      new Constraint1D(), 
      new Constraint1D(), 
      new Constraint1D(),
    };
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <summary>
    /// Gets or sets the constraint anchor orientation on <see cref="Constraint.BodyA"/> in local 
    /// space of <see cref="Constraint.BodyA"/>.
    /// </summary>
    /// <value>
    /// The constraint anchor orientation on <see cref="Constraint.BodyA"/> in local space of 
    /// <see cref="Constraint.BodyA"/>.
    /// </value>
    public Matrix33F AnchorOrientationALocal
    {
      get { return _anchorOrientationALocal; }
      set
      {
        if (_anchorOrientationALocal != value)
        {
          _anchorOrientationALocal = value;
          OnChanged();
        }
      }
    }
    private Matrix33F _anchorOrientationALocal = Matrix33F.Identity;


    /// <summary>
    /// Gets or sets the constraint anchor orientation on <see cref="Constraint.BodyB"/> in local 
    /// space of <see cref="Constraint.BodyB"/>.
    /// </summary>
    /// <value>
    /// The constraint anchor orientation on <see cref="Constraint.BodyB"/> in local space of 
    /// <see cref="Constraint.BodyB"/>.
    /// </value>
    public Matrix33F AnchorOrientationBLocal
    {
      get { return _anchorOrientationBLocal; }
      set
      {
        if (_anchorOrientationBLocal != value)
        {
          _anchorOrientationBLocal = value;
          OnChanged();
        }
      }
    }
    private Matrix33F _anchorOrientationBLocal = Matrix33F.Identity;


    /// <summary>
    /// Gets or sets the target angles.
    /// </summary>
    /// <value>
    /// The target values for the three Euler angles in radians: (angle0, angle1, angle2). an angle
    /// can be set to <see cref="float.NaN"/> to disable the motor on this axis.
    /// </value>
    /// <remarks>
    /// <para>
    /// The <see cref="EulerMotor"/> uses the same Euler angles as the <see cref="AngularLimit"/>.
    /// </para>
    /// <para>
    /// The Euler angles are computed for following order of rotations: The first rotations
    /// is about the x-axis. The second rotation is about the rotated y-axis after the first 
    /// rotation. The last rotation is about the final z-axis.
    /// </para>
    /// <para>
    /// The Euler angles are unique if the second angle is less than +/- 90°. The limits for the
    /// rotation angles are [-180°, 180°] for the first and the third angle. And the limit for the
    /// second angle is [-90°, 90°].
    /// </para>
    /// <para>
    /// Use <see cref="ConstraintHelper.GetEulerAngles"/> to get the Euler angles of a given 
    /// rotation.
    /// </para>
    /// </remarks>
    public Vector3 TargetAngles
    {
      get { return _targetAngles; }
      set
      {
        if (_targetAngles != value)
        {
          _targetAngles = value;
          OnChanged();
        }
      }
    }
    private Vector3 _targetAngles;


    /// <summary>
    /// Gets or sets the spring constant.
    /// </summary>
    /// <value>The spring constant. The default value is 6000.</value>
    public float SpringConstant
    {
      get { return _springConstant; }
      set
      {
        if (_springConstant != value)
        {
          _springConstant = value;
          OnChanged();
        }
      }
    }
    private float _springConstant = 6000;


    /// <summary>
    /// Gets or sets the damping constant.
    /// </summary>
    /// <value>The damping constant. The default value is 900.</value>
    public float DampingConstant
    {
      get { return _dampingConstant; }
      set
      {
        if (_dampingConstant != value)
        {
          _dampingConstant = value;
          OnChanged();
        }
      }
    }
    private float _dampingConstant = 900;


    /// <summary>
    /// Gets or sets the maximal force that is applied by this motor.
    /// </summary>
    /// <value>The maximal force. The default value is +∞.</value>
    /// <remarks>
    /// This property defines the maximal force that the motor can apply.
    /// </remarks>
    public float MaxForce
    {
      get { return _maxForce; }
      set
      {
        if (_maxForce != value)
        {
          _maxForce = value;
          OnChanged();
        }
      }
    }
    private float _maxForce = float.PositiveInfinity;


    /// <inheritdoc/>
    public override Vector3 LinearConstraintImpulse
    {
      get
      {
        return Vector3.Zero; 
      }
    }


    /// <inheritdoc/>
    public override Vector3 AngularConstraintImpulse
    {
      get
      {        
        return _constraints[0].ConstraintImpulse * _constraints[0].JAngB
             + _constraints[1].ConstraintImpulse * _constraints[1].JAngB
             + _constraints[2].ConstraintImpulse * _constraints[2].JAngB;
      }
    }


    //public float MinVelocity { get; set; }


    /// <summary>
    /// Gets or sets the maximal velocity.
    /// </summary>
    /// <value>The maximal velocity. The default value is +∞.</value>
    /// <remarks>
    /// The motor will not create a velocity larger than this limit.
    /// </remarks>
    public float MaxVelocity
    {
      get { return _maxVelocity; }
      set
      {
        if (_maxVelocity != value)
        {
          _maxVelocity = value;
          OnChanged();
        }
      }
    }
    private float _maxVelocity = float.PositiveInfinity;
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <inheritdoc/>
    protected override void OnSetup()
    {
      var deltaTime = Simulation.Settings.Timing.FixedTimeStep;
      float errorReduction = ConstraintHelper.ComputeErrorReduction(deltaTime, SpringConstant, DampingConstant);
      float softness = ConstraintHelper.ComputeSoftness(deltaTime, SpringConstant, DampingConstant);

      // Get anchor orientations in world space.
      Matrix33F anchorOrientationA = BodyA.Pose.Orientation * AnchorOrientationALocal;
      Matrix33F anchorOrientationB = BodyB.Pose.Orientation * AnchorOrientationBLocal;

      Matrix33F relativeOrientation = anchorOrientationA.Transposed * anchorOrientationB;
      Vector3 angles = ConstraintHelper.GetEulerAngles(relativeOrientation);

      // The constraint axes: See OneNote for a detailed derivation of these non-intuitive axes.
      var xA = anchorOrientationA.GetColumn(0);  // Anchor x-axis on A.
      var zB = anchorOrientationB.GetColumn(2);  // Anchor z-axis on B.
      Vector3 constraintAxisY = Vector3.Cross(zB, xA);
      Vector3 constraintAxisX = Vector3.Cross(constraintAxisY, zB);
      Vector3 constraintAxisZ = Vector3.Cross(xA, constraintAxisY);

      SetupConstraint(0, angles.X, TargetAngles.X, constraintAxisX, deltaTime, errorReduction, softness);
      SetupConstraint(1, angles.Y, TargetAngles.Y, constraintAxisY, deltaTime, errorReduction, softness);
      SetupConstraint(2, angles.Z, TargetAngles.Z, constraintAxisZ, deltaTime, errorReduction, softness);

      // No warmstarting.
      _constraints[0].ConstraintImpulse = 0;
      _constraints[1].ConstraintImpulse = 0;
      _constraints[2].ConstraintImpulse = 0;
    }


    private void SetupConstraint(int index, float angle, float targetAngle, Vector3 axis,
                                 float deltaTime, float errorReduction, float softness)
    {
      // Note: Cached constraint impulses are reset in Warmstart() if necessary.

      Constraint1D constraint = _constraints[index];
      Simulation simulation = Simulation;

      // ----- Error correction
      float deviation = targetAngle - angle;

      if (Numeric.IsZero(deviation))
      {
        // deviation is 0 or targetAngle is NaN. Motor is off.

        _minImpulseLimits.SetComponentByIndex(index, 0);
        _maxImpulseLimits.SetComponentByIndex(index, 0);
        constraint.ConstraintImpulse = 0;
        return;
      }

      float fullCorrectionSpeed = deviation / deltaTime;
      float targetVelocity = fullCorrectionSpeed * errorReduction;
      //float minSpeed = MathHelper.Clamp(MinVelocity, -Math.Abs(fullCorrectionSpeed), Math.Abs(fullCorrectionSpeed));
      //if (targetVelocity >= 0 && targetVelocity < minSpeed)
      //  targetVelocity = minSpeed;
      //else if (targetVelocity <= 0 && targetVelocity > -minSpeed)
      //  targetVelocity = -minSpeed;
      float maxErrorCorrectionVelocity = simulation.Settings.Constraints.MaxErrorCorrectionVelocity;
      targetVelocity = MathHelper.Clamp(targetVelocity, -maxErrorCorrectionVelocity, maxErrorCorrectionVelocity);
      targetVelocity = MathHelper.Clamp(targetVelocity, -MaxVelocity, MaxVelocity);

      constraint.TargetRelativeVelocity = targetVelocity;

      // ----- Impulse limits
      float impulseLimit = MaxForce * deltaTime;
      _minImpulseLimits.SetComponentByIndex(index, -impulseLimit);
      _maxImpulseLimits.SetComponentByIndex(index, impulseLimit);

      // Note: Softness must be set before!
      // TODO: Add softness to parameters.
      constraint.Softness = softness / deltaTime;
      constraint.Prepare(BodyA, BodyB, Vector3.Zero, -axis, Vector3.Zero, axis);
    }


    /// <inheritdoc/>
    protected override bool OnApplyImpulse()
    {
      Vector3 impulse = new Vector3();
      impulse.X = ApplyImpulse(0);
      impulse.Y = ApplyImpulse(1);
      impulse.Z = ApplyImpulse(2);

      return impulse.LengthSquared() > Simulation.Settings.Constraints.MinConstraintImpulseSquared;
    }


    private float ApplyImpulse(int index)
    {
      if (_minImpulseLimits.GetComponentByIndex(index) != 0)
      {
        Constraint1D constraint = _constraints[index];
        float relativeVelocity = constraint.GetRelativeVelocity(BodyA, BodyB);
        float impulse = constraint.SatisfyConstraint(
          BodyA,
          BodyB,
          relativeVelocity,
          _minImpulseLimits.GetComponentByIndex(index),
          _maxImpulseLimits.GetComponentByIndex(index));

        return impulse;
      }

      return 0;
    }


    /// <inheritdoc/>
    protected override void OnChanged()
    {
      // Delete cached data.
      _constraints[0].ConstraintImpulse = 0;
      _constraints[1].ConstraintImpulse = 0;
      _constraints[2].ConstraintImpulse = 0;

      base.OnChanged();
    }
    #endregion
  }
}
