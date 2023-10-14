using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Mathematics.Interpolation.Tests
{
  [TestFixture]
  public class BSplineSegment3FTest
  {
    [Test]
    public void GetLength()
    {
      BSplineSegment3F b = new BSplineSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        Point2 = new Vector3(4, 5, 6),
        Point3 = new Vector3(7, 8, 19),
        Point4 = new Vector3(10, 2, 12),
      };

      float lowerBound = (b.Point2 - b.Point1).Length();
      float upperBound = (b.Point2 - b.Point1).Length() + (b.Point3 - b.Point2).Length() + (b.Point4 - b.Point3).Length();
      Assert.Less(lowerBound, b.GetLength(0, 1, 100, Numeric.EpsilonF));
      Assert.Greater(upperBound, b.GetLength(0, 1, 100, Numeric.EpsilonF));

      float length1 = b.GetLength(0, 1, 20, Numeric.EpsilonF);

      float approxLength = 0;
      const float step = 0.0001f;
      for (float u = 0; u <= 1.0f; u += step)
        approxLength += (b.GetPoint(u) - b.GetPoint(u + step)).Length();

      AssertExt.AreNumericallyEqual(approxLength, length1, 0.01f);
      AssertExt.AreNumericallyEqual(b.GetLength(0, 1, 100, Numeric.EpsilonF), b.GetLength(0, 0.5f, 100, Numeric.EpsilonF) + b.GetLength(0.5f, 1, 100, Numeric.EpsilonF));
      AssertExt.AreNumericallyEqual(b.GetLength(0, 1, 100, Numeric.EpsilonF), b.GetLength(1, 0, 100, Numeric.EpsilonF));
    }


    [Test]
    public void Flatten()
    {
      var s = new BSplineSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        Point2 = new Vector3(4, 5, 6),
        Point3 = new Vector3(7, 8, 19),
        Point4 = new Vector3(10, 2, 12),
      };
      var points = new List<Vector3>();
      var tolerance = 0.01f;
      s.Flatten(points, 10, tolerance);
      Assert.IsTrue(points.Contains(s.GetPoint(0)));
      Assert.IsTrue(points.Contains(s.GetPoint(1)));
      var curveLength = s.GetLength(0, 1, 10, tolerance);
      Assert.IsTrue(CurveHelper.GetLength(points) >= curveLength - tolerance * points.Count / 2);
      Assert.IsTrue(CurveHelper.GetLength(points) <= curveLength);
    }
  }
}