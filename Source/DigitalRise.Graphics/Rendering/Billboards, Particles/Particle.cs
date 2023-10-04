// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System.Runtime.InteropServices;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;

namespace DigitalRise.Graphics.Rendering
{
  /// <summary>
  /// Represents a billboard/particle in the billboard renderer. (The structure contains only 
  /// varying (per-particle) data. Uniform data is stored in 
  /// <see cref="BillboardNode"/>/<see cref="Billboard"/> or <see cref="ParticleSystemData"/>.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  internal struct Particle
  {
    public bool IsAlive;
    public Vector3 Position;
    public Vector3 Normal;
    public Vector3 Axis;
    public Vector2 Size;
    public float Angle;
    public Vector3 Color;
    public float Alpha;
    public float AnimationTime;
    public float BlendMode;
  }
}
