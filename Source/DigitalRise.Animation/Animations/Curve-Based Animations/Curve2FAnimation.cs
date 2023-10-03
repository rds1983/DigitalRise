// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using DigitalRise.Animation.Traits;
using DigitalRise.Mathematics.Interpolation;
using Microsoft.Xna.Framework;

namespace DigitalRise.Animation
{
  /// <summary>
  /// Animates a <see cref="Single"/> value using a predefined animation curve.
  /// </summary>
  /// <inheritdoc/>
  public class Curve2FAnimation : AnimationCurve<float, Vector2, CurveKey2F, Curve2F>
  {
    /// <inheritdoc/>
    public override IAnimationValueTraits<float> Traits
    {
      get { return SingleTraits.Instance; }
    }


    /// <overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="Curve2FAnimation"/> class.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Initializes a new instance of the <see cref="Curve2FAnimation"/> class.
    /// </summary>
    public Curve2FAnimation()
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Curve2FAnimation"/> class with the given curve.
    /// </summary>
    /// <param name="curve">The curve.</param>
    public Curve2FAnimation(Curve2F curve)
    {
      Curve = curve;
    }


    /// <inheritdoc/>
    protected override float GetValueFromPoint(Vector2 point)
    {
      return point.Y;
    }
  }
}
