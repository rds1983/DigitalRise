// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using Microsoft.Xna.Framework;

namespace DigitalRise.Mathematics.Interpolation
{
  /// <summary>
  /// Defines a control point on a <see cref="Curve2F"/> (single-precision).
  /// </summary>
  /// <inheritdoc cref="Curve2F"/>
  [Serializable]
  public class CurveKey2F : CurveKey<float, Vector2>
  {
    /// <summary>
    /// Gets the parameter.
    /// </summary>
    /// <returns>The parameter.</returns>
    protected override float GetParameter()
    {
      return Point.X;
    }


    /// <summary>
    /// Sets the parameter.
    /// </summary>
    /// <param name="value">The parameter</param>
    protected override void SetParameter(float value)
    {
      Point = new Vector2(value, Point.Y);
    }
  }
}
