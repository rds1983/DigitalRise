// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;

namespace DigitalRise.Animation.Character
{
  // Animated bones often only need Rotation, or Rotation + Translation. Therefore,
  // we have different bone key frame types to save memory.

  internal enum BoneKeyFrameType
  {
    R,
    RT,
    SRT
  }


  internal struct BoneKeyFrameR
  {
    public TimeSpan Time;
    public Quaternion Rotation;
  }


  internal struct BoneKeyFrameRT
  {
    public TimeSpan Time;
    public Quaternion Rotation;
    public Vector3 Translation;
  }


  internal struct BoneKeyFrameSRT
  {
    public TimeSpan Time;
    public SrtTransform Transform;
  }
}
