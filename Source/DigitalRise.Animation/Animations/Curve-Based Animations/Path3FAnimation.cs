// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using DigitalRise.Animation.Traits;
using DigitalRise.Mathematics.Interpolation;
using Microsoft.Xna.Framework;

namespace DigitalRise.Animation
{
  /// <summary>
  /// Animates a point in 3D space that follows a predefined path.
  /// </summary>
  /// <inheritdoc/>
  public class Path3FAnimation : PathAnimation<Vector3, PathKey3F, Path3F>
  {
    /// <inheritdoc/>
    public override IAnimationValueTraits<Vector3> Traits
    {
      get { return Vector3Traits.Instance; }
    }
  

    /// <overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="Path3FAnimation"/> class.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Initializes a new instance of the <see cref="Path3FAnimation"/> class.
    /// </summary>
    public Path3FAnimation()
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Path3FAnimation"/> class with the given path.
    /// </summary>
    /// <param name="path">The 3D path.</param>
    public Path3FAnimation(Path3F path)
    {
      Path = path;
    }
  }
}
