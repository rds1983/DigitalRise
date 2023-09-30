// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using DigitalRise.Animation.Traits;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Interpolation;


namespace DigitalRise.Animation
{
  /// <summary>
  /// Animates a point in 2D space that follows a predefined path.
  /// </summary>
  /// <inheritdoc/>
  public class Path2FAnimation : PathAnimation<Vector2F, PathKey2F, Path2F>
  {
    /// <inheritdoc/>
    public override IAnimationValueTraits<Vector2F> Traits
    {
      get { return Vector2FTraits.Instance; }
    }


    /// <overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="Path2FAnimation"/> class.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Initializes a new instance of the <see cref="Path2FAnimation"/> class.
    /// </summary>
    public Path2FAnimation()
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Path2FAnimation"/> class with the given path.
    /// </summary>
    /// <param name="path">The 2D path.</param>
    public Path2FAnimation(Path2F path)
    {
      Path = path;
    }
  }
}
