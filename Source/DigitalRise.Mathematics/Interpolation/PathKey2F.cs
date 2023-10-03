// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using Microsoft.Xna.Framework;
using System;


namespace DigitalRise.Mathematics.Interpolation
{
  /// <summary>
  /// Defines a waypoint of a 2-dimensional path (singe-precision).
  /// </summary>
  /// <inheritdoc cref="Path2F"/>
  [Serializable]
  public class PathKey2F : CurveKey<float, Vector2>
  {
    private float _parameter;


    /// <summary>
    /// Gets the parameter.
    /// </summary>
    /// <returns>The parameter.</returns>
    protected override float GetParameter()
    {
      return _parameter;
    }


    /// <summary>
    /// Sets the parameter.
    /// </summary>
    /// <param name="value">The parameter</param>
    protected override void SetParameter(float value)
    {
      _parameter = value;
    }
  }
}
