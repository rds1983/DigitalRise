﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;


namespace DigitalRise.Mathematics.Interpolation.Tests
{
  [TestFixture]
  public class HermiteSegment3FTest
  {
    [Test]
    public void GetPoint()
    {
      HermiteSegment3F s = new HermiteSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        Tangent1 = (new Vector3(10, 3, 6) - new Vector3(1, 2, 3)) * 3,
        Tangent2 = (new Vector3(10, 2, 12) - new Vector3(7, 8, 19)) * 3,
        Point2 = new Vector3(10, 2, 12),
      };

      BezierSegment3F b = new BezierSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        ControlPoint1 = new Vector3(10, 3, 6),
        ControlPoint2 = new Vector3(7, 8, 19),
        Point2 = new Vector3(10, 2, 12),
      };

      AssertExt.AreNumericallyEqual(s.Point1, s.GetPoint(0));
      AssertExt.AreNumericallyEqual(s.Point2, s.GetPoint(1));
      AssertExt.AreNumericallyEqual(b.GetPoint(0.33f), s.GetPoint(0.33f));
    }


    [Test]
    public void GetTangent()
    {
      HermiteSegment3F s = new HermiteSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        Tangent1 = (new Vector3(10, 3, 6) - new Vector3(1, 2, 3)) * 3,
        Tangent2 = (new Vector3(10, 2, 12) - new Vector3(7, 8, 19)) * 3,
        Point2 = new Vector3(10, 2, 12),
      };

      BezierSegment3F b = new BezierSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        ControlPoint1 = new Vector3(10, 3, 6),
        ControlPoint2 = new Vector3(7, 8, 19),
        Point2 = new Vector3(10, 2, 12),
      };

      AssertExt.AreNumericallyEqual(s.Tangent1, s.GetTangent(0));
      AssertExt.AreNumericallyEqual(s.Tangent2, s.GetTangent(1));
      AssertExt.AreNumericallyEqual(b.GetTangent(0.7f), s.GetTangent(0.7f));
    }


    [Test]
    public void GetLength()
    {
      HermiteSegment3F s = new HermiteSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        Tangent1 = (new Vector3(10, 3, 6) - new Vector3(1, 2, 3)) * 3,
        Tangent2 = (new Vector3(10, 2, 12) - new Vector3(7, 8, 19)) * 3,
        Point2 = new Vector3(10, 2, 12),
      };

      BezierSegment3F b = new BezierSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        ControlPoint1 = new Vector3(10, 3, 6),
        ControlPoint2 = new Vector3(7, 8, 19),
        Point2 = new Vector3(10, 2, 12),
      };

      float length1 = s.GetLength(0, 1, 20, Numeric.EpsilonF);
      float length2 = b.GetLength(0, 1, 20, Numeric.EpsilonF);
      AssertExt.AreNumericallyEqual(length1, length2);

      float approxLength = 0;
      const float step = 0.0001f;
      for (float u = 0; u <= 1.0f; u += step)
        approxLength += (s.GetPoint(u) - s.GetPoint(u + step)).Length();

      AssertExt.AreNumericallyEqual(approxLength, length1, 0.01f);
      AssertExt.AreNumericallyEqual(s.GetLength(0, 1, 100, Numeric.EpsilonF), s.GetLength(0, 0.5f, 100, Numeric.EpsilonF) + s.GetLength(0.5f, 1, 100, Numeric.EpsilonF));
      AssertExt.AreNumericallyEqual(s.GetLength(0, 1, 100, Numeric.EpsilonF), s.GetLength(1, 0, 100, Numeric.EpsilonF));
    }


    [Test]
    public void Flatten()
    {
      var s = new HermiteSegment3F
      {
        Point1 = new Vector3(1, 2, 3),
        Tangent1 = (new Vector3(10, 3, 6) - new Vector3(1, 2, 3)) * 3,
        Tangent2 = (new Vector3(10, 2, 12) - new Vector3(7, 8, 19)) * 3,
        Point2 = new Vector3(10, 2, 12),
      };
      var points = new List<Vector3>();
      var tolerance = 0.01f;
      s.Flatten(points, 10, tolerance);
      Assert.IsTrue(points.Contains(s.Point1));
      Assert.IsTrue(points.Contains(s.Point2));
      var curveLength = s.GetLength(0, 1, 10, tolerance);
      Assert.IsTrue(CurveHelper.GetLength(points) >= curveLength - tolerance * points.Count / 2);
      Assert.IsTrue(CurveHelper.GetLength(points) <= curveLength);
    }
  }
}