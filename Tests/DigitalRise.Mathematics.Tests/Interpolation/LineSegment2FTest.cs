using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;


namespace DigitalRise.Mathematics.Interpolation.Tests
{
  [TestFixture]
  public class LineSegment2FTest
  {
    [Test]
    public void Test()
    {
      var s = new LineSegment2F
      {
        Point1 = new Vector2(1, 2),
        Point2 = new Vector2(-1, 9),
      };

      AssertExt.AreNumericallyEqual(s.Point1, s.GetPoint(0));
      AssertExt.AreNumericallyEqual(s.Point2, s.GetPoint(1));
      AssertExt.AreNumericallyEqual(s.Point1 * 0.7f + s.Point2 * 0.3f, s.GetPoint(0.3f));
    }


    [Test]
    public void GetTangent()
    {
      var s = new LineSegment2F
      {
        Point1 = new Vector2(1, 2),
        Point2 = new Vector2(-1, 9),
      };

      AssertExt.AreNumericallyEqual(s.Point2 - s.Point1, s.GetTangent(0));
      AssertExt.AreNumericallyEqual(s.Point2 - s.Point1, s.GetTangent(0.3f));
      AssertExt.AreNumericallyEqual(s.Point2 - s.Point1, s.GetTangent(1));
    }


    [Test]
    public void GetLength()
    {
      var s = new LineSegment2F
      {
        Point1 = new Vector2(1, 2),
        Point2 = new Vector2(-1, 9),
      };

      AssertExt.AreNumericallyEqual((s.Point2 - s.Point1).Length(), s.GetLength(0, 1, 100, Numeric.EpsilonF));
      AssertExt.AreNumericallyEqual((s.Point2 - s.Point1).Length() * 0.3f, s.GetLength(0.6f, 0.3f, 100, Numeric.EpsilonF));
      AssertExt.AreNumericallyEqual((s.Point2 - s.Point1).Length() * 0.3f, s.GetLength(0.1f, 0.4f, 100, Numeric.EpsilonF));
    }


    [Test]
    public void Flatten()
    {
      var s = new LineSegment2F
      {
        Point1 = new Vector2(1, 2),
        Point2 = new Vector2(-1, 9),
      };
      var points = new List<Vector2>();
      s.Flatten(points, 1, 1);
      Assert.AreEqual(2, points.Count);
      Assert.IsTrue(points.Contains(s.Point1));
      Assert.IsTrue(points.Contains(s.Point2));
    }
  }
}