// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Globalization;
using DigitalRise.Geometry.Meshes;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Geometry.Shapes
{
  /// <summary>
  /// Represents a circle in the xy plane centered at the origin.
  /// </summary>
  /// <remarks>
  /// The front face is visible from the positive z half-space.
  /// </remarks>
  [Serializable]
  public class CircleShape : ConvexShape
  {
    // TODO: Optimize: AABB could be more efficiently than the default implementation.

    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------
    #endregion


    //--------------------------------------------------------------
    #region Properties
    //--------------------------------------------------------------

    /// <summary>
    /// Gets an inner point (center of the circle).
    /// </summary>
    /// <value>The center of the circle (0, 0, 0).</value>
    /// <remarks>
    /// This point is a "deep" inner point of the shape (in local space). 
    /// </remarks>
    public override Vector3 InnerPoint
    {
      get { return Vector3.Zero; }
    }


    /// <summary>
    /// Gets or sets the radius.
    /// </summary>
    /// <value>The radius.</value>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> is negative.
    /// </exception>
    public float Radius
    {
      get { return _radius; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException("value", "The radius must be greater than or equal to 0.");

        if (_radius != value)
        {
          _radius = value;
          OnChanged(ShapeChangedEventArgs.Empty);
        }
      }
    }
    private float _radius;
    #endregion


    //--------------------------------------------------------------
    #region Creation and Cleanup
    //--------------------------------------------------------------

    /// <overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="CircleShape"/> class.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Initializes a new instance of the <see cref="CircleShape"/> class.
    /// </summary>
    /// <remarks>
    /// Creates a circle with radius = 0.
    /// </remarks>
    public CircleShape()
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="CircleShape"/> class with the given radius.
    /// </summary>
    /// <param name="radius">The radius.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="radius"/> is negative.
    /// </exception>
    public CircleShape(float radius)
    {
      if (radius < 0)
        throw new ArgumentOutOfRangeException("radius", "The radius must be greater than or equal to 0.");

      _radius = radius;
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    #region ----- Cloning -----

    /// <inheritdoc/>
    protected override Shape CreateInstanceCore()
    {
      return new CircleShape();
    }


    /// <inheritdoc/>
    protected override void CloneCore(Shape sourceShape)
    {
      var source = (CircleShape)sourceShape;
      _radius = source.Radius;
    }
    #endregion


    /// <inheritdoc/>
    public override Aabb GetAabb(Vector3 scale, Pose pose)
    {
      if (scale.X == scale.Y && scale.Y == scale.Z)
      {
        // Uniform scaling.

        // Get world axes in local space. They are equal to the rows of the orientation matrix.
        Matrix33F rotationMatrix = pose.Orientation;
        Vector3 worldX = rotationMatrix.GetRow(0);
        Vector3 worldY = rotationMatrix.GetRow(1);
        Vector3 worldZ = rotationMatrix.GetRow(2);

        // Get extreme points along positive axes.
        Vector3 halfExtent = MathHelper.Absolute(
          new Vector3(
            Vector3.Dot(GetSupportPointNormalized(worldX), worldX),
            Vector3.Dot(GetSupportPointNormalized(worldY), worldY),
            Vector3.Dot(GetSupportPointNormalized(worldZ), worldZ)));

        // Apply scale.
        halfExtent *= Math.Abs(scale.X);

        Vector3 minimum = pose.Position - halfExtent;
        Vector3 maximum = pose.Position + halfExtent;

        Debug.Assert(minimum.IsLessOrEqual(maximum));

        return new Aabb(minimum, maximum);
      }
      else
      {
        // Non-uniform scaling.
        return base.GetAabb(scale, pose);
      }
    }


    /// <summary>
    /// Gets a support point for a given direction.
    /// </summary>
    /// <param name="direction">
    /// The direction for which to get the support point. The vector does not need to be normalized.
    /// The result is undefined if the vector is a zero vector.
    /// </param>
    /// <returns>A support point regarding the given direction.</returns>
    /// <remarks>
    /// <para>
    /// A support point regarding a direction is an extreme point of the shape that is furthest away
    /// from the center regarding the given direction. This point is not necessarily unique.
    /// </para>
    /// </remarks>
    public override Vector3 GetSupportPoint(Vector3 direction)
    {
      direction.Z = 0;

      if (!direction.TryNormalize())
        direction = Vector3.UnitX;

      return direction * _radius;
    }


    /// <summary>
    /// Gets a support point for a given normalized direction vector.
    /// </summary>
    /// <param name="directionNormalized">
    /// The normalized direction vector for which to get the support point.
    /// </param>
    /// <returns>A support point regarding the given direction.</returns>
    /// <remarks>
    /// A support point regarding a direction is an extreme point of the shape that is furthest away
    /// from the center regarding the given direction. This point is not necessarily unique.
    /// </remarks>
    public override Vector3 GetSupportPointNormalized(Vector3 directionNormalized)
    {
      directionNormalized.Z = 0;

      if (!directionNormalized.TryNormalize())
        directionNormalized = Vector3.UnitX;

      return directionNormalized * _radius;
    }


    /// <summary>
    /// Gets the volume of this shape.
    /// </summary>
    /// <param name="relativeError">Not used.</param>
    /// <param name="iterationLimit">Not used</param>
    /// <returns>0</returns>
    public override float GetVolume(float relativeError, int iterationLimit)
    {
      return 0;
    }


    /// <summary>
    /// Called when a mesh should be generated for the shape.
    /// </summary>
    /// <param name="absoluteDistanceThreshold">The absolute distance threshold.</param>
    /// <param name="iterationLimit">The iteration limit.</param>
    /// <returns>The triangle mesh for this shape.</returns>
    protected override TriangleMesh OnGetMesh(float absoluteDistanceThreshold, int iterationLimit)
    {
      // Estimate required segment angle for given accuracy. 
      // (Easy to derive from simple drawing of a circle segment with a triangle used to represent
      // the segment.)
      float alpha = (float)Math.Acos((_radius - absoluteDistanceThreshold) / _radius) * 2;
      int numberOfSegments = (int)Math.Ceiling(ConstantsF.TwoPi / alpha);
      alpha = ConstantsF.TwoPi / numberOfSegments;

      Vector3 r0 = new Vector3(_radius, 0, 0);
      Quaternion rotation = MathHelper.CreateRotationZ(alpha);

      TriangleMesh mesh = new TriangleMesh();

      for (int i = 1; i <= numberOfSegments; i++)
      {
        Vector3 r1 = rotation.Rotate(r0);

        mesh.Add(new Triangle
        {
          Vertex0 = Vector3.Zero,
          Vertex1 = r0,
          Vertex2 = r1,
        }, true);
        r0 = r1;
      }

      return mesh;
    }

    
    /// <summary>
    /// Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"/> that represents the current <see cref="Object"/>.
    /// </returns>
    public override string ToString()
    {
      return String.Format(CultureInfo.InvariantCulture, "CircleShape {{ Radius = {0} }}", _radius);
    }
    #endregion
  }
}
