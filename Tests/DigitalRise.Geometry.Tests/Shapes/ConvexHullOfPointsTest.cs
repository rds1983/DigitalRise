using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Statistics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Geometry.Shapes.Tests
{
  [TestFixture]
  public class ConvexHullOfPointsTest
  {
    [Test]
    public void EmptyConvexHullOfPoints()
    {
      ConvexHullOfPoints convexHullOfPoints = new ConvexHullOfPoints(Enumerable.Empty<Vector3>());
      Assert.AreEqual(0, convexHullOfPoints.Points.Count);
      Assert.AreEqual(Vector3.Zero, convexHullOfPoints.InnerPoint);
      Assert.AreEqual(new Aabb(), convexHullOfPoints.GetAabb(Pose.Identity));
    }


    [Test]
    public void OnePoint()
    {
      Vector3 point = new Vector3(1, 0, 0);
      ConvexHullOfPoints convexHullOfPoints = new ConvexHullOfPoints(new[] { point });
      Assert.AreEqual(1, convexHullOfPoints.Points.Count);
      Assert.AreEqual(point, convexHullOfPoints.InnerPoint);
      Assert.AreEqual(new Aabb(point, point), convexHullOfPoints.GetAabb(Pose.Identity));
    }


    [Test]
    public void TwoPoints()
    {
      Vector3 point0 = new Vector3(1, 0, 0);
      Vector3 point1 = new Vector3(10, 0, 0);
      ConvexHullOfPoints convexHullOfPoints = new ConvexHullOfPoints(new[] { point0, point1 });
      Assert.AreEqual(2, convexHullOfPoints.Points.Count);
      Assert.AreEqual((point0 + point1) / 2, convexHullOfPoints.InnerPoint);
      Assert.AreEqual(new Aabb(point0, point1), convexHullOfPoints.GetAabb(Pose.Identity));
    }


    [Test]
    public void ThreePoints()
    {
      Vector3 point0 = new Vector3(1, 1, 1);
      Vector3 point1 = new Vector3(2, 1, 1);
      Vector3 point2 = new Vector3(1, 2, 1);
      ConvexHullOfPoints convexHullOfPoints = new ConvexHullOfPoints(new[] { point0, point1, point2 });
      Assert.AreEqual(3, convexHullOfPoints.Points.Count);
      Assert.AreEqual((point0 + point1 + point2) / 3, convexHullOfPoints.InnerPoint);
      Assert.AreEqual(new Aabb(new Vector3(1, 1, 1), new Vector3(2, 2, 1)), convexHullOfPoints.GetAabb(Pose.Identity));
    }


    [Test]
    public void GetSupportPoint()
    {
      ConvexHullOfPoints emptyConvexHullOfPoints = new ConvexHullOfPoints(Enumerable.Empty<Vector3>());
      Assert.AreEqual(new Vector3(0, 0, 0), emptyConvexHullOfPoints.GetSupportPoint(new Vector3(1, 0, 0)));
      Assert.AreEqual(new Vector3(0, 0, 0), emptyConvexHullOfPoints.GetSupportPoint(new Vector3(0, 1, 0)));
      Assert.AreEqual(new Vector3(0, 0, 0), emptyConvexHullOfPoints.GetSupportPoint(new Vector3(0, 0, 1)));
      Assert.AreEqual(new Vector3(0, 0, 0), emptyConvexHullOfPoints.GetSupportPoint(new Vector3(1, 1, 1)));

      Vector3 p0 = new Vector3(2, 0, 0);
      Vector3 p1 = new Vector3(-1, -1, -2);
      Vector3 p2 = new Vector3(0, 2, -3);
      Assert.AreEqual(p0, new ConvexHullOfPoints(new[] { p0, p1, p2 }).GetSupportPoint(new Vector3(1, 0, 0)));
      Assert.AreEqual(p2, new ConvexHullOfPoints(new[] { p0, p1, p2 }).GetSupportPoint(new Vector3(0, 1, 0)));
      Assert.AreEqual(p2, new ConvexHullOfPoints(new[] { p0, p1, p2 }).GetSupportPoint(new Vector3(0, 0, -1)));
      Assert.AreEqual(p1, new ConvexHullOfPoints(new[] { p0, p1, p2 }).GetSupportPoint(new Vector3(-1, 0, 1)));
    }


    [Test]
    public void ToStringTest()
    {
      Assert.AreEqual("ConvexHullOfPoints { Count = 3 }", new ConvexHullOfPoints(new[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) }).ToString());
    }


    [Test]
    public void Clone()
    {
      ConvexHullOfPoints convexHullOfPoints = new ConvexHullOfPoints(
        new[]
        {
          new Vector3(0, 0, 0),
          new Vector3(1, 0, 0),
          new Vector3(0, 2, 0),
          new Vector3(0, 0, 3),
          new Vector3(1, 5, 0),
          new Vector3(0, 1, 7),
        });
      ConvexHullOfPoints clone = convexHullOfPoints.Clone() as ConvexHullOfPoints;
      Assert.IsNotNull(clone);

      for (int i = 0; i < clone.Points.Count; i++)
        Assert.AreEqual(convexHullOfPoints.Points[i], clone.Points[i]);

      Assert.AreEqual(convexHullOfPoints.GetAabb(Pose.Identity).Minimum, clone.GetAabb(Pose.Identity).Minimum);
      Assert.AreEqual(convexHullOfPoints.GetAabb(Pose.Identity).Maximum, clone.GetAabb(Pose.Identity).Maximum);
    }


    [Test]
    public void SimpleTetrahedron()
    {
      List<Vector3> points = new List<Vector3>
      {
        new Vector3(0, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(1, 0, 0),
        new Vector3(0, 0, 1),
      };

      ConvexHullOfPoints convex = new ConvexHullOfPoints(points);
      
      // Sample primary directions
      Vector3 right = new Vector3(2, 0, 0);
      AssertSupportPointsAreEquivalent(GetSupportPoint(right, points), convex.GetSupportPoint(right), right);
      Vector3 left = new Vector3(-2, 0, 0);
      AssertSupportPointsAreEquivalent(GetSupportPoint(left, points), convex.GetSupportPoint(left), left);
      Vector3 up = new Vector3(0, 2, 0);
      AssertSupportPointsAreEquivalent(GetSupportPoint(up, points), convex.GetSupportPoint(up), up);
      Vector3 down = new Vector3(0, -2, 0);
      AssertSupportPointsAreEquivalent(GetSupportPoint(down, points), convex.GetSupportPoint(down), down);
      Vector3 back = new Vector3(0, 0, 2);
      AssertSupportPointsAreEquivalent(GetSupportPoint(back, points), convex.GetSupportPoint(back), back);
      Vector3 front = new Vector3(0, 0, -2);
      AssertSupportPointsAreEquivalent(GetSupportPoint(front, points), convex.GetSupportPoint(front), front);

      // Sample random directions
      for (int i = 0; i < 10; i++)
      {
        Vector3 direction = RandomHelper.Random.NextVector3(-1, 1);
        Vector3 supportPoint = convex.GetSupportPoint(direction);
        Vector3 reference = GetSupportPoint(direction, points);

        // The support points can be different, e.g. if a an edge of face is normal to the 
        // direction. When projected onto the direction both support points must be at equal
        // distance.          
        AssertSupportPointsAreEquivalent(reference, supportPoint, direction);
      }
    }


    [Test]
    public void RandomConvexHullOfPoints()
    {
      // Use a fixed seed.
      RandomHelper.Random = new Random(12345);

      // Try polyhedra with 0, 1, 2, ... points.
      for (int numberOfPoints = 0; numberOfPoints < 100; numberOfPoints++)
      {
        List<Vector3> points = new List<Vector3>(numberOfPoints);

        // Create random polyhedra.
        for (int i = 0; i < numberOfPoints; i++)
          points.Add(
            new Vector3(
              RandomHelper.Random.NextFloat(-10, 10),
              RandomHelper.Random.NextFloat(-20, 20),
              RandomHelper.Random.NextFloat(-100, 100)));

        ConvexHullOfPoints convex = new ConvexHullOfPoints(points);

        // Sample primary directions
        Vector3 right = new Vector3(2, 0, 0);
        AssertSupportPointsAreEquivalent(GetSupportPoint(right, points), convex.GetSupportPoint(right), right);
        Vector3 left = new Vector3(-2, 0, 0);
        AssertSupportPointsAreEquivalent(GetSupportPoint(left, points), convex.GetSupportPoint(left), left);
        Vector3 up = new Vector3(0, 2, 0);
        AssertSupportPointsAreEquivalent(GetSupportPoint(up, points), convex.GetSupportPoint(up), up);
        Vector3 down = new Vector3(0, -2, 0);
        AssertSupportPointsAreEquivalent(GetSupportPoint(down, points), convex.GetSupportPoint(down), down);
        Vector3 back = new Vector3(0, 0, 2);
        AssertSupportPointsAreEquivalent(GetSupportPoint(back, points), convex.GetSupportPoint(back), back);
        Vector3 front = new Vector3(0, 0, -2);
        AssertSupportPointsAreEquivalent(GetSupportPoint(front, points), convex.GetSupportPoint(front), front);

        // Sample random directions
        for (int i = 0; i < 10; i++)
        {
          Vector3 direction = RandomHelper.Random.NextVector3(-1, 1);
          if (direction.IsNumericallyZero())
            continue;

          Vector3 supportPoint = convex.GetSupportPoint(direction);
          Vector3 reference = GetSupportPoint(direction, points);

          // The support points can be different, e.g. if a an edge of face is normal to the 
          // direction. When projected onto the direction both support points must be at equal
          // distance.          
          AssertSupportPointsAreEquivalent(reference, supportPoint, direction);
        }
      }
    }


    private Vector3 GetSupportPoint(Vector3 direction, List<Vector3> points)
    {
      // This is the default method that is used without the internal BSP tree.
      float maxDistance = float.NegativeInfinity;
      Vector3 supportVertex = new Vector3();
      int numberOfPoints = points.Count;
      for (int i = 0; i < numberOfPoints; i++)
      {
        float distance = Vector3.Dot(points[i], direction);
        if (distance > maxDistance)
        {
          supportVertex = points[i];
          maxDistance = distance;
        }
      }
      return supportVertex;
    }


    private void AssertSupportPointsAreEquivalent(Vector3 expected, Vector3 actual, Vector3 direction)
    {
      // The support points can be different, e.g. if a an edge of face is normal to the 
      // direction. When projected onto the direction both support points must be at equal
      // distance.          
      bool areEqual = Numeric.AreEqual(
        MathHelper.ProjectTo(expected, direction).Length(),
        MathHelper.ProjectTo(actual, direction).Length());

      Assert.IsTrue(areEqual);
    }
  }
}
