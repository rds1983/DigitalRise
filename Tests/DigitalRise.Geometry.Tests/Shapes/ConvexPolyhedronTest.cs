using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Statistics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Geometry.Shapes.Tests
{
  [TestFixture]
  public class ConvexPolyhedronTest
  {
    [Test]
    public void EmptyConvexPolyhedron()
    {
      ConvexPolyhedron convexPolyhedron = new ConvexPolyhedron(Enumerable.Empty<Vector3>());
      Assert.AreEqual(0, convexPolyhedron.Vertices.Count);
      Assert.AreEqual(Vector3.Zero, convexPolyhedron.InnerPoint);
      Assert.AreEqual(new Aabb(), convexPolyhedron.GetAabb(Pose.Identity));
    }


    [Test]
    public void OnePoint()
    {
      Vector3 point = new Vector3(1, 0, 0);
      ConvexPolyhedron convexPolyhedron = new ConvexPolyhedron(new[] { point });
      Assert.AreEqual(1, convexPolyhedron.Vertices.Count);
      Assert.AreEqual(point, convexPolyhedron.InnerPoint);
      Assert.AreEqual(new Aabb(point, point), convexPolyhedron.GetAabb(Pose.Identity));
    }


    [Test]
    public void TwoPoints()
    {
      Vector3 point0 = new Vector3(1, 0, 0);
      Vector3 point1 = new Vector3(10, 0, 0);
      ConvexPolyhedron convexPolyhedron = new ConvexPolyhedron(new[] { point0, point1 });
      Assert.AreEqual(2, convexPolyhedron.Vertices.Count);
      Assert.AreEqual((point0 + point1) / 2, convexPolyhedron.InnerPoint);
      Assert.AreEqual(new Aabb(point0, point1), convexPolyhedron.GetAabb(Pose.Identity));
    }


    [Test]
    public void ThreePoints()
    {
      Vector3 point0 = new Vector3(1, 1, 1);
      Vector3 point1 = new Vector3(2, 1, 1);
      Vector3 point2 = new Vector3(1, 2, 1);
      ConvexPolyhedron convexPolyhedron = new ConvexPolyhedron(new[] { point0, point1, point2 });
      Assert.AreEqual(3, convexPolyhedron.Vertices.Count);
      Assert.AreEqual((point0 + point1 + point2) / 3, convexPolyhedron.InnerPoint);
      Assert.AreEqual(new Aabb(new Vector3(1, 1, 1), new Vector3(2, 2, 1)), convexPolyhedron.GetAabb(Pose.Identity));
    }


    [Test]
    public void GetSupportPoint()
    {
      ConvexPolyhedron emptyConvexPolyhedron = new ConvexPolyhedron(Enumerable.Empty<Vector3>());
      Assert.AreEqual(new Vector3(0, 0, 0), emptyConvexPolyhedron.GetSupportPoint(new Vector3(1, 0, 0)));
      Assert.AreEqual(new Vector3(0, 0, 0), emptyConvexPolyhedron.GetSupportPoint(new Vector3(0, 1, 0)));
      Assert.AreEqual(new Vector3(0, 0, 0), emptyConvexPolyhedron.GetSupportPoint(new Vector3(0, 0, 1)));
      Assert.AreEqual(new Vector3(0, 0, 0), emptyConvexPolyhedron.GetSupportPoint(new Vector3(1, 1, 1)));

      Vector3 p0 = new Vector3(2, 0, 0);
      Vector3 p1 = new Vector3(-1, -1, -2);
      Vector3 p2 = new Vector3(0, 2, -3);
      AssertExt.AreNumericallyEqual(p0, new ConvexPolyhedron(new[] { p0, p1, p2 }).GetSupportPoint(new Vector3(1, 0, 0)));
      AssertExt.AreNumericallyEqual(p2, new ConvexPolyhedron(new[] { p0, p1, p2 }).GetSupportPoint(new Vector3(0, 1, 0)));
      AssertExt.AreNumericallyEqual(p2, new ConvexPolyhedron(new[] { p0, p1, p2 }).GetSupportPoint(new Vector3(0, 0, -1)));
      AssertExt.AreNumericallyEqual(p1, new ConvexPolyhedron(new[] { p0, p1, p2 }).GetSupportPoint(new Vector3(-1, 0, 1)));
    }


    [Test]
    public void ToStringTest()
    {
      Assert.AreEqual("ConvexPolyhedron { Count = 3 }", new ConvexPolyhedron(new[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) }).ToString());
    }


    [Test]
    public void Clone()
    {
      ConvexPolyhedron convexPolyhedron = new ConvexPolyhedron(
        new[]
        {
          new Vector3(0, 0, 0),
          new Vector3(1, 0, 0),
          new Vector3(0, 2, 0),
          new Vector3(0, 0, 3),
          new Vector3(1, 5, 0),
          new Vector3(0, 1, 7),
        });
      ConvexPolyhedron clone = convexPolyhedron.Clone() as ConvexPolyhedron;
      Assert.IsNotNull(clone);

      for (int i = 0; i < clone.Vertices.Count; i++)
        Assert.AreEqual(convexPolyhedron.Vertices[i], clone.Vertices[i]);

      Assert.AreEqual(convexPolyhedron.GetAabb(Pose.Identity), clone.GetAabb(Pose.Identity));
      Assert.AreEqual(convexPolyhedron.InnerPoint, clone.InnerPoint);
      Assert.AreEqual(convexPolyhedron.GetSupportPoint(new Vector3(1,1,1)), clone.GetSupportPoint(new Vector3(1, 1, 1)));

      Assert.AreEqual(convexPolyhedron.GetAabb(Pose.Identity).Minimum, clone.GetAabb(Pose.Identity).Minimum);
      Assert.AreEqual(convexPolyhedron.GetAabb(Pose.Identity).Maximum, clone.GetAabb(Pose.Identity).Maximum);
    }


    //[Test]
    //public void SerializationXml()
    //{
    //  var a = new ConvexPolyhedron(
    //    new[]
    //    {
    //      new Vector3(0, 0, 0),
    //      new Vector3(1, 0, 0),
    //      new Vector3(0, 2, 0),
    //      new Vector3(0, 0, 3),
    //      new Vector3(1, 5, 0),
    //      new Vector3(0, 1, 7),
    //    });

    //  // Serialize object.
    //  var stream = new MemoryStream();
    //  var serializer = new XmlSerializer(typeof(Shape));
    //  serializer.Serialize(stream, a);

    //  // Output generated xml. Can be manually checked in output window.
    //  stream.Position = 0;
    //  var xml = new StreamReader(stream).ReadToEnd();
    //  Trace.WriteLine("Serialized Object:\n" + xml);

    //  // Deserialize object.
    //  stream.Position = 0;
    //  var deserializer = new XmlSerializer(typeof(Shape));
    //  var b = (ConvexPolyhedron)deserializer.Deserialize(stream);

    //  for (int i = 0; i < b.Vertices.Count; i++)
    //    Assert.AreEqual(a.Vertices[i], b.Vertices[i]);

    //  Assert.AreEqual(a.GetAabb(Pose.Identity), b.GetAabb(Pose.Identity));
    //  Assert.AreEqual(a.InnerPoint, b.InnerPoint);
    //  Assert.AreEqual(a.GetSupportPoint(new Vector3(1, 1, 1)), b.GetSupportPoint(new Vector3(1, 1, 1)));
    //}


    [Test]
    [Ignore("Binary serialization not supported in PCL version.")]
    public void SerializationBinary()
    {
      var a = new ConvexPolyhedron(
        new[]
        {
          new Vector3(0, 0, 0),
          new Vector3(1, 0, 0),
          new Vector3(0, 2, 0),
          new Vector3(0, 0, 3),
          new Vector3(1, 5, 0),
          new Vector3(0, 1, 7),
        });

      // Serialize object.
      var stream = new MemoryStream();
      var formatter = new BinaryFormatter();
      formatter.Serialize(stream, a);

      // Deserialize object.
      stream.Position = 0;
      var deserializer = new BinaryFormatter();
      var b = (ConvexPolyhedron)deserializer.Deserialize(stream);

      for (int i = 0; i < b.Vertices.Count; i++)
        Assert.AreEqual(a.Vertices[i], b.Vertices[i]);

      Assert.AreEqual(a.GetAabb(Pose.Identity), b.GetAabb(Pose.Identity));
      Assert.AreEqual(a.InnerPoint, b.InnerPoint);
      Assert.AreEqual(a.GetSupportPoint(new Vector3(1, 1, 1)), b.GetSupportPoint(new Vector3(1, 1, 1)));
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

      ConvexPolyhedron convex = new ConvexPolyhedron(points);

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
    public void RandomConvexPolyhedron()
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

        //var convexHull = GeometryHelper.CreateConvexHull(points);
        ConvexPolyhedron convex = new ConvexPolyhedron(points);

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
      AssertExt.AreNumericallyEqual(
        MathHelper.ProjectTo(expected, direction).Length(),
        MathHelper.ProjectTo(actual, direction).Length(),
        0.01);
    }
  }
}
