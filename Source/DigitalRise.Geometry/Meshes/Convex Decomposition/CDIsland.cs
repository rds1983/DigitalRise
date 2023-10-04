// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System.Diagnostics;
using DigitalRise.Geometry.Shapes;
using Microsoft.Xna.Framework;

namespace DigitalRise.Geometry.Meshes
{
  /// <summary>
  /// Describes a group of triangles that create one convex part.
  /// </summary>
  [DebuggerDisplay("Island {Id}: Triangles={Triangles.Count}")]
  internal sealed class CDIsland
  {
    // A unique number
    public int Id;

    // The triangles.
    public CDTriangle[] Triangles;

    // The AABB enclosing all triangles.
    public Aabb Aabb;

    // The convex hull vertices.
    public Vector3[] Vertices;

    // The convex hull. It must be either null or up-to-date. 
    public ConvexHullBuilder ConvexHullBuilder;
  }
}
