// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using Microsoft.Xna.Framework;
using System;


namespace DigitalRise.Mathematics.Statistics
{
  /// <summary>
  /// A distribution that returns random positions on a line segment.
  /// </summary>
  public class LineSegmentDistribution : Distribution<Vector3>
  {
    /// <summary>
    /// Gets or sets the start position of the line segment.
    /// </summary>
    /// <value>The start position. The default is (-1, -1, -1).</value>
    public Vector3 Start
    {
      get { return _start; }
      set { _start = value; }
    }
    private Vector3 _start = new Vector3(-1);


    /// <summary>
    /// Gets or sets the end position of the line segment.
    /// </summary>
    /// <value>The end position. The default is (1, 1, 1).</value>
    public Vector3 End
    {
      get { return _end; }
      set { _end = value; }
    }
    private Vector3 _end = new Vector3(1);


    /// <inheritdoc/>
    public override Vector3 Next(Random random)
    {
      if (random == null)
        throw new ArgumentNullException("random");

      return _start + (_end - _start) * (float)random.NextDouble();
    }
  }
}
