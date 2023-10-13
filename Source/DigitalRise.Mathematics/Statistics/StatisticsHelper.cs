// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Collections.Generic;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;

namespace DigitalRise.Mathematics.Statistics
{
  /// <summary>
  /// Provides helper methods for statistical tasks.
  /// </summary>
  public static class StatisticsHelper
  {
    /// <overloads>
    /// <summary>
    /// Computes the covariance matrix for a list of points.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Computes the covariance matrix for a list of 3-dimensional points (single-precision).
    /// </summary>
    /// <param name="points">The points.</param>
    /// <returns>The covariance matrix.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="points"/> is <see langword="null"/>.
    /// </exception>
    public static Matrix33F ComputeCovarianceMatrix(IList<Vector3> points)
    {
      // Notes: See "Real-Time Collision Detection" p. 93

      if (points == null)
        throw new ArgumentNullException("points");

      int numberOfPoints = points.Count;
      float oneOverNumberOfPoints = 1f / numberOfPoints;

      // Compute the center of mass.
      Vector3 centerOfMass = Vector3.Zero;
      for (int i = 0; i < numberOfPoints; i++)
        centerOfMass += points[i];
      centerOfMass *= oneOverNumberOfPoints;

      // Compute covariance matrix.
      float c00 = 0;
      float c11 = 0;
      float c22 = 0;
      float c01 = 0;
      float c02 = 0;
      float c12 = 0;

      for (int i = 0; i < numberOfPoints; i++)
      {
        // Translate points so that center of mass is at origin.
        Vector3 p = points[i] - centerOfMass;

        // Compute covariance of translated point.
        c00 += p.X * p.X;
        c11 += p.Y * p.Y;
        c22 += p.Z * p.Z;
        c01 += p.X * p.Y;
        c02 += p.X * p.Z;
        c12 += p.Y * p.Z;
      }
      c00 *= oneOverNumberOfPoints;
      c11 *= oneOverNumberOfPoints;
      c22 *= oneOverNumberOfPoints;
      c01 *= oneOverNumberOfPoints;
      c02 *= oneOverNumberOfPoints;
      c12 *= oneOverNumberOfPoints;

      Matrix33F covarianceMatrix = new Matrix33F(c00, c01, c02,
                                                 c01, c11, c12,
                                                 c02, c12, c22);
      return covarianceMatrix;
    }
  }
}
