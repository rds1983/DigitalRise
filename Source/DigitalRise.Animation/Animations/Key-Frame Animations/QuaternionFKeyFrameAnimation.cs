// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using DigitalRise.Animation.Traits;
using DigitalRise.Mathematics.Algebra;


namespace DigitalRise.Animation
{
  /// <summary>
  /// Animates a <see cref="QuaternionF"/> value using key frames.
  /// </summary>
  /// <inheritdoc/>
  public class QuaternionFKeyFrameAnimation : KeyFrameAnimation<QuaternionF>
  {
    /// <inheritdoc/>
    public override IAnimationValueTraits<QuaternionF> Traits
    {
      get { return QuaternionFTraits.Instance; }
    }
  }
}
