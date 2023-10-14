using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Mathematics.Interpolation.Tests
{
  [TestFixture]
  public class CardinalSegment1FTest
  {
    [Test]
    public void GetPoint()
    {
      HermiteSegment1F h = new HermiteSegment1F
      {
        Point1 = 3,
        Tangent1 = (1 - 0.3f) * (7 - 1) * 0.5f,
        Tangent2 = (1 - 0.3f) * (8 - 3) * 0.5f,
        Point2 = 7,
      };

      CardinalSegment1F s = new CardinalSegment1F
      {
        Point1 = 1,
        Point2 = 3,
        Point3 = 7,
        Point4 = 8,
        Tension = 0.3f,
      };

      AssertExt.AreNumericallyEqual(3, s.GetPoint(0));
      AssertExt.AreNumericallyEqual(7, s.GetPoint(1));
      AssertExt.AreNumericallyEqual(h.GetPoint(0.3f), s.GetPoint(0.3f));
    }


    [Test]
    public void GetTangent()
    {
      HermiteSegment1F h = new HermiteSegment1F
      {
        Point1 = 3,
        Tangent1 = (1 - 0.3f) * (7 - 1) * 0.5f,
        Tangent2 = (1 - 0.3f) * (8 - 3) * 0.5f,
        Point2 = 7,
      };

      CardinalSegment1F s = new CardinalSegment1F
      {
        Point1 = 1,
        Point2 = 3,
        Point3 = 7,
        Point4 = 8,
        Tension = 0.3f,
      };

      AssertExt.AreNumericallyEqual(h.Tangent1, s.GetTangent(0));
      AssertExt.AreNumericallyEqual(h.Tangent2, s.GetTangent(1));
      AssertExt.AreNumericallyEqual(h.GetTangent(0.81f), s.GetTangent(0.81f));
    }


    [Test]
    public void GetLength()
    {
      CardinalSegment1F s = new CardinalSegment1F
      {
        Point1 = 1,
        Point2 = 3,
        Point3 = 7,
        Point4 = 8,
        Tension = 0.3f,
      };

      AssertExt.AreNumericallyEqual(4, s.GetLength(0, 1, 100, Numeric.EpsilonF));

      CardinalSegment1F sSymmetric = new CardinalSegment1F
      {
        Point1 = 1,
        Point2 = 3,
        Point3 = 7,
        Point4 = 9,
        Tension = 0.3f,
      };
      AssertExt.AreNumericallyEqual(2f, sSymmetric.GetLength(0.5f, 1, 100, Numeric.EpsilonF));
    }


    [Test]
    public void Flatten()
    {
      CardinalSegment1F s = new CardinalSegment1F
      {
        Point1 = 1,
        Point2 = 3,
        Point3 = 7,
        Point4 = 8,
        Tension = 0.3f,
      };
      var points = new List<float>();
      s.Flatten(points, 1, 1);
      Assert.AreEqual(2, points.Count);
      Assert.IsTrue(points.Contains(s.Point2));
      Assert.IsTrue(points.Contains(s.Point3));
    }
  }
}