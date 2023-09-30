// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using DigitalRise.Animation.Traits;
using DigitalRise.Mathematics.Algebra;


namespace DigitalRise.Animation
{
  /// <summary>
  /// Animates a <see langword="Vector2F"/> value from/to/by a certain value.
  /// </summary>
  /// <inheritdoc/>
  public class Vector2FFromToByAnimation : FromToByAnimation<Vector2F>
  {
    /// <inheritdoc/>
    public override IAnimationValueTraits<Vector2F> Traits
    {
      get { return Vector2FTraits.Instance; }
    }
  }
}
