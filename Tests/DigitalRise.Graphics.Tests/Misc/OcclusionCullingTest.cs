using DigitalRise.Geometry.Shapes;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Graphics.Tests
{
  [TestFixture]
  public class OcclusionCullingTest
  {
    [Test]
    public void GetBoundsOrthographic()
    {
      // Get bounds of AABB in clip space. (Used in OcclusionCulling.fx.)
      Vector3 cameraPosition = new Vector3(100, -200, 345);
      Vector3 cameraForward = new Vector3(1, 2, 3).Normalized();
      Vector3 cameraUp = new Vector3(-1, 0.5f, -2).Normalized();

      Matrix44F view = Matrix44F.CreateLookAt(cameraPosition, cameraPosition + cameraForward, cameraUp);
      Matrix44F proj = Matrix44F.CreateOrthographic(16, 9, -10, 100);
      Matrix44F viewProj = proj * view;

      Vector3 center = new Vector3();
      Vector3 halfExtent = new Vector3();
      Aabb aabb = new Aabb(center - halfExtent, center + halfExtent);
      Aabb aabb0, aabb1;
      GetBoundsOrtho(aabb, viewProj, out aabb0);
      GetBoundsOrthoSmart(aabb, viewProj, out aabb1);
      AssertExt.AreNumericallyEqual(aabb0, aabb1);

      center = new Vector3(-9, 20, -110);
      halfExtent = new Vector3(5, 2, 10);
      aabb = new Aabb(center - halfExtent, center + halfExtent);
      GetBoundsOrtho(aabb, viewProj, out aabb0);
      GetBoundsOrthoSmart(aabb, viewProj, out aabb1);
      AssertExt.AreNumericallyEqual(aabb0, aabb1);
    }


    private void GetBoundsOrtho(Aabb aabbWorld, Matrix44F viewProj, out Aabb aabbClip)
    {
      Vector3 minimum = aabbWorld.Minimum;
      Vector3 maximum = aabbWorld.Maximum;

      Vector3 v0 = (viewProj * new Vector4(minimum.X, minimum.Y, minimum.Z, 1)).XYZ();
      Vector3 minimumClip = v0;
      Vector3 maximumClip = v0;
      Vector3 v1 = (viewProj * new Vector4(maximum.X, minimum.Y, minimum.Z, 1)).XYZ();
      minimumClip = MathHelper.Min(minimumClip, v1);
      maximumClip = MathHelper.Max(maximumClip, v1);
      Vector3 v2 = (viewProj * new Vector4(minimum.X, maximum.Y, minimum.Z, 1)).XYZ();
      minimumClip = MathHelper.Min(minimumClip, v2);
      maximumClip = MathHelper.Max(maximumClip, v2);
      Vector3 v3 = (viewProj * new Vector4(maximum.X, maximum.Y, minimum.Z, 1)).XYZ();
      minimumClip = MathHelper.Min(minimumClip, v3);
      maximumClip = MathHelper.Max(maximumClip, v3);
      Vector3 v4 = (viewProj * new Vector4(minimum.X, minimum.Y, maximum.Z, 1)).XYZ();
      minimumClip = MathHelper.Min(minimumClip, v4);
      maximumClip = MathHelper.Max(maximumClip, v4);
      Vector3 v5 = (viewProj * new Vector4(maximum.X, minimum.Y, maximum.Z, 1)).XYZ();
      minimumClip = MathHelper.Min(minimumClip, v5);
      maximumClip = MathHelper.Max(maximumClip, v5);
      Vector3 v6 = (viewProj * new Vector4(minimum.X, maximum.Y, maximum.Z, 1)).XYZ();
      minimumClip = MathHelper.Min(minimumClip, v6);
      maximumClip = MathHelper.Max(maximumClip, v6);
      Vector3 v7 = (viewProj * new Vector4(maximum.X, maximum.Y, maximum.Z, 1)).XYZ();
      minimumClip = MathHelper.Min(minimumClip, v7);
      maximumClip = MathHelper.Max(maximumClip, v7);

      aabbClip.Minimum = minimumClip;
      aabbClip.Maximum = maximumClip;
    }


    private void GetBoundsOrthoSmart(Aabb aabbWorld, Matrix44F viewProj, out Aabb aabbClip)
    {
      Vector3 minimum = aabbWorld.Minimum;
      Vector3 maximum = aabbWorld.Maximum;
      Vector3 extent = maximum - minimum;

      Vector3 v0 = (viewProj * new Vector4(minimum.X, minimum.Y, minimum.Z, 1)).XYZ();
      Vector3 minimumClip = v0;
      Vector3 maximumClip = v0;

      Vector3 d0 = extent.X * viewProj.GetColumn(0).XYZ();
      Vector3 d1 = extent.Y * viewProj.GetColumn(1).XYZ();
      Vector3 d2 = extent.Z * viewProj.GetColumn(2).XYZ();

      Vector3 v1 = v0 + d0;
      minimumClip = MathHelper.Min(minimumClip, v1);
      maximumClip = MathHelper.Max(maximumClip, v1);
      Vector3 v2 = v0 + d1;
      minimumClip = MathHelper.Min(minimumClip, v2);
      maximumClip = MathHelper.Max(maximumClip, v2);
      Vector3 v3 = v0 + d2;
      minimumClip = MathHelper.Min(minimumClip, v3);
      maximumClip = MathHelper.Max(maximumClip, v3);
      Vector3 v4 = v1 + d1;
      minimumClip = MathHelper.Min(minimumClip, v4);
      maximumClip = MathHelper.Max(maximumClip, v4);
      Vector3 v5 = v1 + d2;
      minimumClip = MathHelper.Min(minimumClip, v5);
      maximumClip = MathHelper.Max(maximumClip, v5);
      Vector3 v6 = v2 + d2;
      minimumClip = MathHelper.Min(minimumClip, v6);
      maximumClip = MathHelper.Max(maximumClip, v6);
      Vector3 v7 = v4 + d2;
      minimumClip = MathHelper.Min(minimumClip, v7);
      maximumClip = MathHelper.Max(maximumClip, v7);

      aabbClip.Minimum = minimumClip;
      aabbClip.Maximum = maximumClip;
    }


    [Test]
    public void GetBoundsPerspective()
    {
      // Get bounds of AABB in clip space. (Used in OcclusionCulling.fx.)
      // Note: Z = 0 or negative is handled conservatively. Point (0, 0, 0) is returned.
      Vector3 cameraPosition = new Vector3(100, -200, 345);
      Vector3 cameraForward = new Vector3(1, 2, 3).Normalized();
      Vector3 cameraUp = new Vector3(-1, 0.5f, -2).Normalized();

      Matrix44F view = Matrix44F.CreateLookAt(cameraPosition, cameraPosition + cameraForward, cameraUp);
      Matrix44F proj = Matrix44F.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 16.0f / 9.0f, 1, 100);
      Matrix44F viewProj = proj * view;

      // Empty AABB at center of near plane.
      Vector3 center = cameraPosition + cameraForward;
      Vector3 halfExtent = new Vector3();
      Aabb aabb = new Aabb(center - halfExtent, center + halfExtent);
      Aabb aabb0, aabb1;
      GetBoundsPersp(aabb, viewProj, out aabb0);
      GetBoundsPerspSmart(aabb, viewProj, out aabb1);
      AssertExt.AreNumericallyEqual(aabb0, aabb1);

      // AABB inside frustum.
      center = view.Inverse.TransformPosition(new Vector3(2, -3, -50));
      halfExtent = new Vector3(1, 6, 10);
      aabb = new Aabb(center - halfExtent, center + halfExtent);
      GetBoundsPersp(aabb, viewProj, out aabb0);
      GetBoundsPerspSmart(aabb, viewProj, out aabb1);
      AssertExt.AreNumericallyEqual(aabb0, aabb1);

      // Behind camera.
      center = view.Inverse.TransformPosition(new Vector3(2, -3, 50));
      halfExtent = new Vector3(1, 6, 10);
      aabb = new Aabb(center - halfExtent, center + halfExtent);
      GetBoundsPersp(aabb, viewProj, out aabb0);
      GetBoundsPerspSmart(aabb, viewProj, out aabb1);
      AssertExt.AreNumericallyEqual(aabb0, aabb1);

      // Camera inside AABB.
      center = view.Inverse.TransformPosition(new Vector3(2, -3, -50));
      halfExtent = new Vector3(100, 100, 100);
      aabb = new Aabb(center - halfExtent, center + halfExtent);
      GetBoundsPersp(aabb, viewProj, out aabb0);
      GetBoundsPerspSmart(aabb, viewProj, out aabb1);
      AssertExt.AreNumericallyEqual(aabb0, aabb1);
    }


    private void GetBoundsPersp(Aabb aabbWorld, Matrix44F viewProj, out Aabb aabbClip)
    {
      Vector3 minimum = aabbWorld.Minimum;
      Vector3 maximum = aabbWorld.Maximum;

      Vector4 v0 = (viewProj * new Vector4(minimum.X, minimum.Y, minimum.Z, 1));
      Vector3 v;
      if (v0.Z < Numeric.EpsilonF)
        v = new Vector3();
      else
        v = v0.XYZ() / v0.W;

      Vector3 minimumClip = v;
      Vector3 maximumClip = v;

      Vector4 v1 = (viewProj * new Vector4(maximum.X, minimum.Y, minimum.Z, 1));
      if (v1.Z < 0)
        v = new Vector3();
      else
        v = v1.XYZ() / v1.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      Vector4 v2 = (viewProj * new Vector4(minimum.X, maximum.Y, minimum.Z, 1));
      if (v2.Z < 0)
        v = new Vector3();
      else
        v = v2.XYZ() / v2.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      Vector4 v3 = (viewProj * new Vector4(maximum.X, maximum.Y, minimum.Z, 1));
      if (v3.Z < 0)
        v = new Vector3();
      else
        v = v3.XYZ() / v3.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      Vector4 v4 = (viewProj * new Vector4(minimum.X, minimum.Y, maximum.Z, 1));
      if (v4.Z < 0)
        v = new Vector3();
      else
        v = v4.XYZ() / v4.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      Vector4 v5 = (viewProj * new Vector4(maximum.X, minimum.Y, maximum.Z, 1));
      if (v5.Z < 0)
        v = new Vector3();
      else
        v = v5.XYZ() / v5.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      Vector4 v6 = (viewProj * new Vector4(minimum.X, maximum.Y, maximum.Z, 1));
      if (v6.Z < 0)
        v = new Vector3();
      else
        v = v6.XYZ() / v6.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      Vector4 v7 = (viewProj * new Vector4(maximum.X, maximum.Y, maximum.Z, 1));
      if (v7.Z < 0)
        v = new Vector3();
      else
        v = v7.XYZ() / v7.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      aabbClip.Minimum = minimumClip;
      aabbClip.Maximum = maximumClip;
    }


    private void GetBoundsPerspSmart(Aabb aabbWorld, Matrix44F viewProj, out Aabb aabbClip)
    {
      Vector3 minimum = aabbWorld.Minimum;
      Vector3 maximum = aabbWorld.Maximum;
      Vector3 extent = maximum - minimum;

      Vector4 v0 = viewProj * new Vector4(minimum.X, minimum.Y, minimum.Z, 1);
      Vector4 d0 = extent.X * viewProj.GetColumn(0);
      Vector4 d1 = extent.Y * viewProj.GetColumn(1);
      Vector4 d2 = extent.Z * viewProj.GetColumn(2);

      Vector4 v1 = v0 + d0;
      Vector4 v2 = v0 + d1;
      Vector4 v3 = v0 + d2;
      Vector4 v4 = v1 + d1;
      Vector4 v5 = v1 + d2;
      Vector4 v6 = v2 + d2;
      Vector4 v7 = v4 + d2;

      Vector3 v;
      if (v0.Z < 0)
        v = new Vector3();
      else
        v = v0.XYZ() / v0.W;

      Vector3 minimumClip = v;
      Vector3 maximumClip = v;

      if (v1.Z < 0)
        v = new Vector3();
      else
        v = v1.XYZ() / v1.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      if (v2.Z < 0)
        v = new Vector3();
      else
        v = v2.XYZ() / v2.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      if (v3.Z < 0)
        v = new Vector3();
      else
        v = v3.XYZ() / v3.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      if (v4.Z < 0)
        v = new Vector3();
      else
        v = v4.XYZ() / v4.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      if (v5.Z < 0)
        v = new Vector3();
      else
        v = v5.XYZ() / v5.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      if (v6.Z < 0)
        v = new Vector3();
      else
        v = v6.XYZ() / v6.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      if (v7.Z < 0)
        v = new Vector3();
      else
        v = v7.XYZ() / v7.W;

      minimumClip = MathHelper.Min(minimumClip, v);
      maximumClip = MathHelper.Max(maximumClip, v);

      aabbClip.Minimum = minimumClip;
      aabbClip.Maximum = maximumClip;
    }
  }
}
