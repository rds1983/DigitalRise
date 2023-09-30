// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using DigitalRise.Animation.Traits;
using Microsoft.Xna.Framework;


namespace DigitalRise.Animation
{
  /// <summary>
  /// Animates a <see langword="Quaternion"/> value from/to/by a certain value.
  /// (Only available in the XNA-compatible build.)
  /// </summary>
  /// <inheritdoc/>
  public class QuaternionFromToByAnimation : FromToByAnimation<Quaternion>
  {
    /// <inheritdoc/>
    public override IAnimationValueTraits<Quaternion> Traits
    {
      get { return QuaternionTraits.Instance; }
    }
  }
}
