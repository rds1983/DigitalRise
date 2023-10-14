using DigitalRise.Mathematics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Graphics.Tests
{
  [TestFixture]
  public class MiscTest
  {
    [TestCase(1, 0, 0)]
    [TestCase(1, 0, 0.1f)]
    [TestCase(0, 1, 0)]
    [TestCase(0, 0, 1)]
    [TestCase(1, 1, 0)]
    [TestCase(1, 0, 1)]
    [TestCase(0, 1, 1)]
    [TestCase(1, 1, 1)]
    [TestCase(1, -2, 3)]
    public void AnisotropicGaussianTest(float x, float y, float z)
    {
      // Validate code in Blur.fx.
      // Reference: "Screen Space Anisotropic Blurred Soft Shadows"
      Vector3 normalView = new Vector3(x, y, z);
      normalView.Normalize();

      Vector3 axisMajor0, axisMinor0;
      float radiusMajor0, radiusMinor0;
      GetEllipseCoefficients(normalView, out axisMajor0, out axisMinor0, out radiusMajor0, out radiusMinor0);
      Assert.AreEqual(0.0f, axisMajor0.Z);
      Assert.AreEqual(0.0f, axisMinor0.Z);
      Assert.IsTrue(axisMajor0.IsNumericallyNormalized());
      Assert.IsTrue(axisMinor0.IsNumericallyNormalized());

      Vector3 axisMajor1, axisMinor1;
      float radiusMajor1, radiusMinor1;
      GetEllipseCoefficients_Optimized(normalView, out axisMajor1, out axisMinor1, out radiusMajor1, out radiusMinor1);
      AssertExt.AreNumericallyEqual(axisMajor0, axisMajor1);
      AssertExt.AreNumericallyEqual(axisMinor0, axisMinor1);
      Assert.AreEqual(radiusMajor0, radiusMajor1);
      Assert.AreEqual(radiusMinor0, radiusMinor1);
    }


    private static void GetEllipseCoefficients(Vector3 normalView, out Vector3 axisMajor, out Vector3 axisMinor, out float radiusMajor, out float radiusMinor)
    {
      axisMinor = new Vector3(normalView.X, normalView.Y, 0);
      if (!axisMinor.TryNormalize())
        axisMinor = new Vector3(0, 1, 0);

      Vector3 normalScreen = new Vector3(0, 0, 1); // The normal vector of the screen.
      axisMajor = Vector3.Cross(axisMinor, normalScreen);
      radiusMinor = Vector3.Dot(normalView, normalScreen);
      radiusMajor = 1;
    }


    private static void GetEllipseCoefficients_Optimized(Vector3 normalView, out Vector3 axisMajor, out Vector3 axisMinor, out float radiusMajor, out float radiusMinor)
    {
      Vector2 axisMinor2D;
      if (normalView.X != 0)
      {
        axisMinor2D = new Vector2(normalView.X, normalView.Y).Normalized();
      }
      else
      {
        axisMinor2D = new Vector2(0, 1);
      }

      axisMinor.X = axisMinor2D.X;
      axisMinor.Y = axisMinor2D.Y;
      axisMinor.Z = 0;

      axisMajor.X = axisMinor2D.Y;
      axisMajor.Y = -axisMinor2D.X;
      axisMajor.Z = 0;

      radiusMinor = normalView.Z;
      radiusMajor = 1;
    }
  }
}
