using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;


namespace DigitalRise.Mathematics.Interpolation.Tests
{
  [TestFixture]
  public class CardinalSegment3FTest
  {
    [Test]
    public void GetPoint()
    {
      CardinalSegment3F c = new CardinalSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        Point2 = new Vector3(10, 3, 6),
        Point3 = new Vector3(7, 8, 19),
        Point4 = new Vector3(10, 2, 12),
        Tension = 0.3f
      };

      HermiteSegment3F h = new HermiteSegment3F
      {
        Point1 = c.Point2,
        Tangent1 = (1 - c.Tension) * (c.Point3 - c.Point1) * 0.5f,
        Tangent2 = (1 - c.Tension) * (c.Point4 - c.Point2) * 0.5f,
        Point2 = c.Point3,
      };

      AssertExt.AreNumericallyEqual(c.Point2, c.GetPoint(0));
      AssertExt.AreNumericallyEqual(c.Point3, c.GetPoint(1));
      AssertExt.AreNumericallyEqual(h.GetPoint(0.33f), c.GetPoint(0.33f));
    }


    [Test]
    public void GetTangent()
    {
      CardinalSegment3F c = new CardinalSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        Point2 = new Vector3(10, 3, 6),
        Point3 = new Vector3(7, 8, 19),
        Point4 = new Vector3(10, 2, 12),
        Tension = 0.3f
      };

      HermiteSegment3F h = new HermiteSegment3F
      {
        Point1 = c.Point2,
        Tangent1 = (1 - c.Tension) * (c.Point3 - c.Point1) * 0.5f,
        Tangent2 = (1 - c.Tension) * (c.Point4 - c.Point2) * 0.5f,
        Point2 = c.Point3,
      };

      AssertExt.AreNumericallyEqual(h.Tangent1, c.GetTangent(0));
      AssertExt.AreNumericallyEqual(h.Tangent2, c.GetTangent(1));
      AssertExt.AreNumericallyEqual(h.GetTangent(0.7f), c.GetTangent(0.7f));
    }


    [Test]
    public void GetLength()
    {
      CardinalSegment3F c = new CardinalSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        Point2 = new Vector3(10, 3, 6),
        Point3 = new Vector3(7, 8, 19),
        Point4 = new Vector3(10, 2, 12),
        Tension = 0.3f
      };

      HermiteSegment3F h = new HermiteSegment3F
      {
        Point1 = c.Point2,
        Tangent1 = (1 - c.Tension) * (c.Point3 - c.Point1) * 0.5f,
        Tangent2 = (1 - c.Tension) * (c.Point4 - c.Point2) * 0.5f,
        Point2 = c.Point3,
      };

      float length1 = c.GetLength(0, 1, 20, Numeric.EpsilonF);
      float length2 = h.GetLength(0, 1, 20, Numeric.EpsilonF);
      AssertExt.AreNumericallyEqual(length1, length2);

      float approxLength = 0;
      const float step = 0.0001f;
      for (float u = 0; u <= 1.0f; u += step)
        approxLength += (c.GetPoint(u) - c.GetPoint(u + step)).Length();

      AssertExt.AreNumericallyEqual(approxLength, length1, 0.01f);
      AssertExt.AreNumericallyEqual(c.GetLength(0, 1, 100, Numeric.EpsilonF), c.GetLength(0, 0.5f, 100, Numeric.EpsilonF) + c.GetLength(0.5f, 1, 100, Numeric.EpsilonF));
      AssertExt.AreNumericallyEqual(c.GetLength(0, 1, 100, Numeric.EpsilonF), c.GetLength(1, 0, 100, Numeric.EpsilonF));
    }


    [Test]
    public void Flatten()
    {
      var s = new CardinalSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        Point2 = new Vector3(10, 3, 6),
        Point3 = new Vector3(7, 8, 19),
        Point4 = new Vector3(10, 2, 12),
        Tension = 0.3f
      };
      var points = new List<Vector3>();
      var tolerance = 0.01f;
      s.Flatten(points, 10, tolerance);
      Assert.IsTrue(points.Contains(s.Point2));
      Assert.IsTrue(points.Contains(s.Point3));
      var curveLength = s.GetLength(0, 1, 10, tolerance);
      Assert.IsTrue(CurveHelper.GetLength(points) >= curveLength - tolerance * points.Count / 2);
      Assert.IsTrue(CurveHelper.GetLength(points) <= curveLength);
    }
  }
}