using System.Collections.Generic;
using System.Linq;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Statistics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Mathematics.Interpolation.Tests
{
  [TestFixture]
  public class CurveHelperTest
  {
    [Test]
    public void GetParameter()
    {
      BezierSegment1F b = new BezierSegment1F()
      {
        Point1 = 1,
        ControlPoint1 = 3,
        ControlPoint2 = 4,
        Point2 = 8,
      };

      AssertExt.AreNumericallyEqual(0, CurveHelper.GetParameter(b, 1, 100));
      AssertExt.AreNumericallyEqual(1, CurveHelper.GetParameter(b, 8, 100));
      AssertExt.AreNumericallyEqual(0.3f, CurveHelper.GetParameter(b, b.GetPoint(0.3f), 100));
      AssertExt.AreNumericallyEqual(0.4f, CurveHelper.GetParameter(b, b.GetPoint(0.4f), 100));
      AssertExt.AreNumericallyEqual(0.5f, CurveHelper.GetParameter(b, b.GetPoint(0.5f), 100));
      AssertExt.AreNumericallyEqual(0.6f, CurveHelper.GetParameter(b, b.GetPoint(0.6f), 100));
      AssertExt.AreNumericallyEqual(0.9f, CurveHelper.GetParameter(b, b.GetPoint(0.9f), 100));

      Assert.IsFalse(Numeric.AreEqual(0.9f, CurveHelper.GetParameter(b, b.GetPoint(0.9f), 1))); // limited iterations.

      for (int i=0; i<1000; i++)
      {
        float u = RandomHelper.Random.NextFloat(0, 1);
        float point = b.GetPoint(u);
        AssertExt.AreNumericallyEqual(u, CurveHelper.GetParameter(b, point, 100), 0.0001);
      }

      for (int i = 0; i < 1000; i++)
      {
        float u = RandomHelper.Random.NextFloat(0, 1);
        float point = b.GetPoint(u);
        AssertExt.AreNumericallyEqual(u, CurveHelper.GetParameter(b, point, 100), 0.01f);
      }
    }


    [Test]
    public void Flatten2FLengthZero()
    {
      var s = new BezierSegment2F
      {
        Point1 = new Vector2(1, 1),
        ControlPoint1 = new Vector2(1, 1),
        ControlPoint2 = new Vector2(1, 1),
        Point2 = new Vector2(1, 1),
      };
      var points = new List<Vector2>();
      s.Flatten(points, 1, 1);
      Assert.AreEqual(0, points.Count);
    }


    [Test]
    public void Flatten3FLengthZero()
    {
      var s = new BezierSegment3F
      {
        Point1 = new Vector3(1, 1, 2),
        ControlPoint1 = new Vector3(1, 1, 2),
        ControlPoint2 = new Vector3(1, 1, 2),
        Point2 = new Vector3(1, 1, 2),
      };
      var points = new List<Vector3>();
      s.Flatten(points, 1, 1);
      Assert.AreEqual(0, points.Count);
    }


    [Test]
    public void Flatten2FLargeTolerance()
    {
      var s = new BezierSegment2F
      {
        Point1 = new Vector2(1, 1),
        ControlPoint1 = new Vector2(2, 2),
        ControlPoint2 = new Vector2(3, 3),
        Point2 = new Vector2(4, 4),
      };
      var points = new List<Vector2>();
      s.Flatten(points, 1, 10);
      Assert.AreEqual(2, points.Count);
      Assert.IsTrue(points.Contains(s.Point1));
      Assert.IsTrue(points.Contains(s.Point2));
    }


    [Test]
    public void Flatten3FLargeTolerance()
    {
      var s = new BezierSegment3F
      {
        Point1 = new Vector3(1),
        ControlPoint1 = new Vector3(2),
        ControlPoint2 = new Vector3(3),
        Point2 = new Vector3(4),
      };
      var points = new List<Vector3>();
      s.Flatten(points, 1, 10);
      Assert.AreEqual(2, points.Count);
      Assert.IsTrue(points.Contains(s.Point1));
      Assert.IsTrue(points.Contains(s.Point2));
    }


    [Test]
    public void GetLengthOfLineSegments2F()
    {
      var points = new[]
      {
        new Vector2(1, 2), new Vector2(12, 13),
        new Vector2(1231, 2.2f), new Vector2(5, 122),
        new Vector2(-11, 2), new Vector2(-1, 123),
        new Vector2(-123.123f, 122), new Vector2(-2312, -123),
      };
      var length = (points[1] - points[0]).Length()
                   + (points[3] - points[2]).Length()
									 + (points[5] - points[4]).Length()
									 + (points[7] - points[6]).Length();
      Assert.AreEqual(length, CurveHelper.GetLength(points.ToList()));
    }


    [Test]
    public void GetLengthOfLineSegments3F()
    {
      var points = new[]
      {
        new Vector3(1, 2, 0.123f), new Vector3(12, 13, 123),
        new Vector3(1231, 2.2f, 0.123f), new Vector3(5, 122, 123),
        new Vector3(-11, 2, 0.123f), new Vector3(-1, 123, 123),
        new Vector3(-123.123f, 122, 0.123f), new Vector3(-2312, -123, 123),
      };
      var length = (points[1] - points[0]).Length()
                   + (points[3] - points[2]).Length()
									 + (points[5] - points[4]).Length()
									 + (points[7] - points[6]).Length();
      Assert.AreEqual(length, CurveHelper.GetLength(points.ToList()));
    }
  }
}
