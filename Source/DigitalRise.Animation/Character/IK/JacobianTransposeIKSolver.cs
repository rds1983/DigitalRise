// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Animation.Character
{
  /// <summary>
  /// Modifies a skeleton using the Jacobian Transpose method.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This <see cref="IKSolver"/> uses the Jacobian Transpose algorithm to modify a bone chain to
  /// reach the <see cref="IKSolver.Target"/> position. <see cref="RootBoneIndex"/> determines the
  /// first bone in the chain. <see cref="TipBoneIndex"/> determines the last bone that is included
  /// in the chain. This IK solver rotates all bones in the chain, so that the tip of the bone chain
  /// reaches the <see cref="IKSolver.Target"/> position.
  /// </para>
  /// <para>
  /// This solver uses an iterative algorithm. <see cref="NumberOfIterations"/> limits the maximal
  /// number allowed iterations. The algorithm ends early if the distance between the
  /// <see cref="IKSolver.Target"/> and the tip of the chain is less than
  /// <see cref="AllowedDeviation"/>. In each iteration, the solver computes "forces" that pull the
  /// tip of the bone chain to the target. It then makes an Euler integration step to move the bones
  /// as determined by the computed forces. <see cref="StepSize"/> determines the time step of the
  /// numeric integration. If this value is too large, the solver becomes unstable. If this value
  /// too small, the solver needs many iterations to reach the target. A good value for a specific
  /// application must be determined by experimentation.
  /// </para>
  /// <para>
  /// <strong>Bone rotation limits: </strong><br/>
  /// Per default, the <see cref="IKSolver"/> assumes that the bones can rotate in any direction
  /// without rotation limits. If the bone rotations should be limited (e.g. "do not rotate about
  /// the y axis" or "do not rotate more than 45�"), then a <see cref="LimitBoneTransforms"/>
  /// callback must be set. The 
  /// <see cref="LimitBoneTransforms"/> callback must be a method that checks the current bone
  /// rotations and removes any invalid rotations. See <see cref="LimitBoneTransforms"/> for more
  /// details.
  /// </para>
  /// <para>
  /// See also <see cref="IKSolver"/> for more general information. 
  /// </para>
  /// <para>
  /// <strong>Caution:</strong><br/>
  /// This IK solver allocates heap memory and creates garbage. If garbage collector performance is
  /// important (e.g. on the Xbox 360 or Windows Phone 7), do not use this IK solver.
  /// </para>
  /// </remarks>
  public class JacobianTransposeIKSolver : IKSolver
  {
    // Jacobian Transpose method is described in Game Programming Gems 4.
    // Derivation of Jacobian can be found in book "Advanced Animation and Rendering Techniques"
    // by Watt & Watt.


    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    private bool _isDirty = true;

    // A list that is re-used in each OnSolve(). It does not store data between calls.
    private readonly List<SrtTransform> _originalTransforms = new List<SrtTransform>();
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <summary>
    /// Gets or sets the index of the root bone.
    /// </summary>
    /// <value>The index of the root bone.</value>
    public int RootBoneIndex
    {
      get { return _rootBoneIndex; }
      set
      {
        if (_rootBoneIndex != value)
        {
          _rootBoneIndex = value;
          OnInvalidate();
        }
      }
    }
    private int _rootBoneIndex;


    /// <summary>
    /// Gets or sets the index of the tip bone.
    /// </summary>
    /// <value>The index of the tip bone.</value>
    public int TipBoneIndex
    {
      get { return _tipBoneIndex; }
      set
      {
        if (_tipBoneIndex != value)
        {
          _tipBoneIndex = value;
          OnInvalidate();
        }
      }
    }
    private int _tipBoneIndex;


    /// <summary>
    /// Gets or sets the tip offset in tip bone space.
    /// </summary>
    /// <value>
    /// The tip offset in tip bone space. The default is a zero vector.
    /// </value>
    /// <remarks>
    /// If this offset is zero, the IK solver will try to move the origin of the tip bone to the
    /// <see cref="IKSolver.Target"/>. For example, if the solver is used for an arm and the hand
    /// bone is the tip bone, then the character will "grab" the target with the wrist where the
    /// hand bone starts. The <see cref="TipOffset"/> should be set to the offset from the wrist to
    /// the hand center. Then the target will be grabbed correctly with the hand center and not the
    /// wrist.
    /// </remarks>
    public Vector3 TipOffset
    {
      get { return _tipOffset; }
      set
      {
        if (_tipOffset != value)
        {
          _tipOffset = value;
          OnInvalidate();
        }
      }
    }
    private Vector3 _tipOffset;


    /// <summary>
    /// Gets or sets the number of iterations.
    /// </summary>
    /// <value>The number of iterations. The default is 100.</value>
    public int NumberOfIterations { get; set; }


    /// <summary>
    /// Gets or sets the allowed distance error.
    /// </summary>
    /// <value>The allowed distance error. The default is 0.01.</value>
    /// <remarks>
    /// <para>
    /// This solver uses an iterative algorithm. The algorithm ends early if the distance between
    /// the <see cref="IKSolver.Target"/> and the tip of the chain is less than 
    /// <see cref="AllowedDeviation"/>.
    /// </para>
    /// </remarks>
    public float AllowedDeviation { get; set; }


    /// <summary>
    /// Gets or sets the size of the Euler integration step.
    /// </summary>
    /// <value>The size of the Euler integration step. The default is 0.01.</value>
    public float StepSize { get; set; }


    /// <summary>
    /// Gets or sets the a callback that enforces rotation limits.
    /// </summary>
    /// <value>
    /// The callback that enforces rotation limits. The default is <see langword="null"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// If this property is <see langword="null"/>, the bone rotations are not limited. If the bone
    /// rotations should be limited, this property must be set to a method that enforces the bone
    /// limits: The method should simply check the bones and rotate the bones back to the allowed
    /// range. 
    /// </para>
    /// <para>
    /// This method is called after each iteration. The method should check and correct the 
    /// rotation of all bones in the chain.
    /// </para>
    /// </remarks>
    public Action LimitBoneTransforms { get; set; }
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="JacobianTransposeIKSolver"/> class.
    /// </summary>
    public JacobianTransposeIKSolver()
    {
      NumberOfIterations = 100;
      AllowedDeviation = 0.01f;
      StepSize = 0.01f;
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <summary>
    /// Called when the <see cref="SkeletonPose"/> was exchanged.
    /// </summary>
    protected override void OnInvalidate()
    {
      _isDirty = true;
      base.OnInvalidate();
    }


    /// <summary>
    /// Called when <see cref="IKSolver.Solve"/> is called.
    /// </summary>
    /// <param name="deltaTime">The current time step (in seconds).</param>
    protected override void OnSolve(float deltaTime)
    {
      if (NumberOfIterations <= 0)
        return;

      if (_isDirty)
        if (!SkeletonPose.IsAncestorOrSelf(RootBoneIndex, TipBoneIndex))
          throw new ArgumentException("The RootBoneIndex and the TipBoneIndex do not form a valid bone chain.");

      _isDirty = false;

      var skeleton = SkeletonPose.Skeleton;
      bool requiresBlending = RequiresBlending();
      float maxRotationAngle;
      bool requiresLimiting = RequiresLimiting(deltaTime, out maxRotationAngle);
      if (requiresBlending || requiresLimiting)
      {
        // Remember original bone transforms for interpolation with the result at the end.
        // Transforms are stored from tip to root (reverse order!).
        _originalTransforms.Clear();

        int boneIndex = TipBoneIndex;
        while (true)
        {
          _originalTransforms.Add(SkeletonPose.GetBoneTransform(boneIndex));

          if (boneIndex == RootBoneIndex)
            break;

          boneIndex = skeleton.GetParent(boneIndex);
        }
      }

      int numberOfBones = SkeletonPose.GetNumberOfBones(RootBoneIndex, TipBoneIndex);

      // The transposed jacobian matrix.
      var jacobianTransposed = new float[numberOfBones, 6];

      // The force vector (3 linear and 3 angular (torque) entries).
      var force = new float[6];

      // The rotation axes of the bones.
      Vector3[] axes = new Vector3[numberOfBones];

      float toleranceSquared = AllowedDeviation * AllowedDeviation;

      // In each iteration we compute the jacobian matrix, compute the bone velocities
      // an make an euler integration step.
      for (int iteration = 0; iteration < NumberOfIterations; iteration++)
      {
        var tipBoneAbsolute = SkeletonPose.GetBonePoseAbsolute(TipBoneIndex);
        var tipAbsolute = tipBoneAbsolute.ToParentPosition(TipOffset);
        var targetToTip = tipAbsolute - Target;
        if (targetToTip.LengthSquared() < toleranceSquared)
        {
          if (iteration == 0)
            return;

          break;
        }

        // Loop from tip to root and fill Jacobian.
        // (See description of Jacobian Transpose method to see how the rotation axes and
        // Jacobian entries must look like).
        var currentBoneIndex = TipBoneIndex;
        int exitBoneIndex = RootBoneIndex >= 0 ? skeleton.GetParent(RootBoneIndex) : -1;
        int i = numberOfBones;
        do
        {
          i--;

          // Compute rotation axis. 
          Vector3 currentJointAbsolute = SkeletonPose.GetBonePoseAbsolute(currentBoneIndex).Translation;
          Vector3 jointToTarget = Target - currentJointAbsolute;
          Vector3 jointToTip = tipAbsolute - currentJointAbsolute;
          axes[i] = Vector3.Cross(jointToTarget, jointToTip);
          if (!axes[i].TryNormalize())
            axes[i] = Vector3.UnitX;   // TODO: What should we really do in this case?

          Vector3 jacobianColumnUpperPart = Vector3.Cross(jointToTip, axes[i]);

          // Fill J.
          jacobianTransposed[i, 0] = jacobianColumnUpperPart.X;
          jacobianTransposed[i, 1] = jacobianColumnUpperPart.Y;
          jacobianTransposed[i, 2] = jacobianColumnUpperPart.Z;
          jacobianTransposed[i, 3] = axes[i].X;
          jacobianTransposed[i, 4] = axes[i].Y;
          jacobianTransposed[i, 5] = axes[i].Z;

          currentBoneIndex = skeleton.GetParent(currentBoneIndex);

        } while (currentBoneIndex != exitBoneIndex && currentBoneIndex >= 0);

        Debug.Assert(i == 0);

        // Set the force.
        force[0] = targetToTip.X;
        force[1] = targetToTip.Y;
        force[2] = targetToTip.Z;
        force[3] = 0;
        force[4] = 0;
        force[5] = 0;

				// Compute pseudo velocities.
				var velocities = new float[numberOfBones];
        for (int j = 0; j < numberOfBones; j++)
        {
          velocities[j] = 0;
          for (int k = 0; k < 6; k++)
            velocities[j] += jacobianTransposed[j, k] * force[k];
        }

        // Euler integration step.
        currentBoneIndex = TipBoneIndex;
        i = numberOfBones;
        do
        {
          i--;

          // Rotation axis for this bone.
          Vector3 axis = axes[i];
          // Angle is computed using Euler integration with an arbitrary step size.
          float angle = velocities[i] * StepSize;

          // Apply rotation.
          Quaternion rotationChange = MathHelper.CreateRotation(axis, angle);
          SkeletonPose.RotateBoneAbsolute(currentBoneIndex, rotationChange);

          currentBoneIndex = skeleton.GetParent(currentBoneIndex);
        } while (currentBoneIndex != exitBoneIndex && currentBoneIndex >= 0);

        // Call delegate that checks bone limits.
        if (LimitBoneTransforms != null)
          LimitBoneTransforms();
      }

      if (requiresBlending || requiresLimiting)
      {
        // Apply weight and the angular velocity limit.
        int boneIndex = TipBoneIndex;
        int i = 0;
        while (true)
        {
          var originalTransform = _originalTransforms[i];
          var targetTransform = SkeletonPose.GetBoneTransform(boneIndex);

          // Apply weight.
          if (requiresBlending)
            BlendBoneTransform(ref originalTransform, ref targetTransform);

          // Apply angular velocity limit.
          if (requiresLimiting)
            LimitBoneTransform(ref originalTransform, ref targetTransform, maxRotationAngle);

          SkeletonPose.SetBoneTransform(boneIndex, targetTransform);

          if (boneIndex == RootBoneIndex)
            break;

          boneIndex = skeleton.GetParent(boneIndex);
          i++;
        }
      }

      _originalTransforms.Clear();
    }
    #endregion
  }
}
