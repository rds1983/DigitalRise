﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace DigitalRise.Mathematics.Interpolation
{
  /// <summary>
  /// Defines a 2-dimensional line segment (single-precision).
  /// </summary>
  /// <remarks>
  /// <para>
  /// A <see cref="LineSegment2F"/> performs linear interpolation (LERP) between two points.
  /// </para>
  /// <para>
  /// The curve function <i>point = C(parameter)</i> takes a scalar parameter and returns a point
  /// on the curve (see <see cref="GetPoint"/>). The curve parameter lies in the interval [0,1]; it 
  /// is also known as <i>interpolation parameter</i>, <i>interpolation factor</i> or <i>weight of 
  /// the target point</i>. <i>C(0)</i> returns the start point <see cref="Point1"/>; <i>C(1)</i> 
  /// returns the end point <see cref="Point2"/>.
  /// </para>
  /// </remarks>
  public class LineSegment2F : ICurve<float, Vector2>, IRecyclable
  {
    /// <summary>
    /// Gets or sets the start point.
    /// </summary>
    public Vector2 Point1 { get; set; }


    /// <summary>
    /// Gets or sets the end point.
    /// </summary>
    public Vector2 Point2 { get; set; }


    /// <summary>
    /// Computes a point on the curve.
    /// </summary>
    /// <param name="parameter">The curve parameter.</param>
    /// <returns>The curve point.</returns>
    public Vector2 GetPoint(float parameter)
    {
      return InterpolationHelper.Lerp(Point1, Point2, parameter);
    }


    /// <inheritdoc/>
    public Vector2 GetTangent(float parameter)
    {
      return Point2 - Point1;
    }


    /// <inheritdoc/>
    public float GetLength(float start, float end, int maxNumberOfIterations, float tolerance)
    {
      return (Point2 - Point1).Length() * Math.Abs(end - start);
    }


    /// <inheritdoc/>
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
    public void Flatten(ICollection<Vector2> points, int maxNumberOfIterations, float tolerance)
    {
      points.Add(Point1);
      points.Add(Point2);
    }


    //--------------------------------------------------------------
    #region Resource Pooling
    //--------------------------------------------------------------

    private static readonly ResourcePool<LineSegment2F> Pool = new ResourcePool<LineSegment2F>(
       () => new LineSegment2F(),                     // Create
       null,                                          // Initialize
       null                                           // Uninitialize
       );


    /// <summary>
    /// Creates an instance of the <see cref="LineSegment2F"/> class. (This method reuses a
    /// previously recycled instance or allocates a new instance if necessary.)
    /// </summary>
    /// <returns>
    /// A new or reusable instance of the <see cref="LineSegment2F"/> class.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method tries to obtain a previously recycled instance from a resource pool if resource
    /// pooling is enabled (see <see cref="ResourcePool.Enabled">ResourcePool.Enabled</see>). If no
    /// object is available, a new instance is automatically allocated on the heap. 
    /// </para>
    /// <para>
    /// The owner of the object should call <see cref="Recycle"/> when the instance is no longer 
    /// needed.
    /// </para>
    /// </remarks>
    public static LineSegment2F Create()
    {
      return Pool.Obtain();
    }


    /// <inheritdoc/>
    public void Recycle()
    {
      Point1 = new Vector2();
      Point2 = new Vector2();

      Pool.Recycle(this);
    }
    #endregion
  }
}
