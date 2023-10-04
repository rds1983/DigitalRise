// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DigitalRise.Graphics
{
  partial class GraphicsHelper
  {
    // Note: Cube map faces are left-handed! Therefore +Z is actually -Z.
    private static readonly Vector3[] CubeMapForwardDirections =
    { 
      Vector3.UnitX, -Vector3.UnitX, 
      Vector3.UnitY, -Vector3.UnitY,
      -Vector3.UnitZ, Vector3.UnitZ   // Switch Z because cube maps are left-handed.
    };


    private static readonly Vector3[] CubeMapUpDirections =
    { 
      Vector3.UnitY, Vector3.UnitY,
      Vector3.UnitZ, -Vector3.UnitZ,
      Vector3.UnitY, Vector3.UnitY
    };


    /// <summary>
    /// Gets the camera forward direction for rendering into a cube map face.
    /// </summary>
    /// <param name="cubeMapFace">The cube map face.</param>
    /// <returns>
    /// The camera forward direction required to render the content of the
    /// given cube map face.
    /// </returns>
    public static Vector3 GetCubeMapForwardDirection(CubeMapFace cubeMapFace)
    {
      return CubeMapForwardDirections[(int)cubeMapFace];
    }


    /// <summary>
    /// Gets the camera up direction for rendering into a cube map face.
    /// </summary>
    /// <param name="cubeMapFace">The cube map face.</param>
    /// <returns>
    /// The camera up direction required to render the content of the
    /// given cube map face.
    /// </returns>
    public static Vector3 GetCubeMapUpDirection(CubeMapFace cubeMapFace)
    {
      return CubeMapUpDirections[(int)cubeMapFace];
    }
  }
}
