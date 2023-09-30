// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using DigitalRise.Animation.Traits;
using DigitalRise.Mathematics.Algebra;


namespace DigitalRise.Animation
{
  /// <summary>
  /// Animates a <see cref="Vector4F"/> value using key frames.
  /// </summary>
  /// <inheritdoc/>
  public class Vector4FKeyFrameAnimation : KeyFrameAnimation<Vector4F>
  {
    /// <inheritdoc/>
    public override IAnimationValueTraits<Vector4F> Traits
    {
      get { return Vector4FTraits.Instance; }
    }
  }
}
