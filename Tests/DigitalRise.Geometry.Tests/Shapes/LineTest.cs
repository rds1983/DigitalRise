using System;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Geometry.Shapes.Tests
{
  [TestFixture]
  public class LineTest
  {
    [Test]
    public void Constructor()
    {
      Assert.AreEqual(new Vector3(), new Line().PointOnLine);
      Assert.AreEqual(new Vector3(), new Line().Direction);

      Assert.AreEqual(new Vector3(), new Line(Vector3.Zero, Vector3.Zero).PointOnLine);
      Assert.AreEqual(new Vector3(), new Line(Vector3.Zero, Vector3.Zero).Direction);

      Assert.AreEqual(new Vector3(10, 20, 30), new Line(new Vector3(10, 20, 30), new Vector3(11, 22, 33)).PointOnLine);
      Assert.AreEqual(new Vector3(11, 22, 33), new Line(new Vector3(10, 20, 30), new Vector3(11, 22, 33)).Direction);

      Assert.AreEqual(new Vector3(10, 20, 30), new Line(new LineShape(new Vector3(10, 20, 30), new Vector3(11, 22, 33).Normalized())).PointOnLine);
      Assert.AreEqual(new Vector3(11, 22, 33).Normalized(), new Line(new LineShape(new Vector3(10, 20, 30), new Vector3(11, 22, 33).Normalized())).Direction);
    }


    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ConstructorArgumentNullException()
    {
      new Line(null);
    }


    [Test]
    public void EqualsTest()
    {
      Assert.IsTrue(new Line().Equals(new Line()));
      Assert.IsTrue(new Line().Equals(new Line(Vector3.Zero, Vector3.Zero)));
      Assert.IsTrue(new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).Equals(new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6))));
      Assert.IsTrue(new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).Equals((object)new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6))));
      Assert.IsFalse(new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).Equals(new Line(new Vector3(0, 2, 3), new Vector3(4, 5, 6))));
      Assert.IsFalse(new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).Equals(new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6))));
      Assert.IsFalse(new Line().Equals(null));

      Assert.IsTrue(new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6)) == new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6)));
      Assert.IsTrue(new Line(new Vector3(1, 2, 4), new Vector3(4, 5, 6)) != new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6)));
    }


    [Test]
    public void GetHashCodeTest()
    {
      Assert.AreEqual(new Line().GetHashCode(), new Line().GetHashCode());
      Assert.AreEqual(new Line().GetHashCode(), new Line(Vector3.Zero, Vector3.Zero).GetHashCode());
      Assert.AreEqual(new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetHashCode(), new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetHashCode());
      Assert.AreNotEqual(new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetHashCode(), new Line(new Vector3(0, 2, 3), new Vector3(4, 5, 6)).GetHashCode());
    }


    [Test]
    public void PositiveUniformScaling()
    {
      using (var setEpsilon = new SetEpsilonF(1e-02f))
      {
        Vector3 point0 = new Vector3(10, 20, -40);
        Vector3 point1 = new Vector3(-22, 34, 45);
        Line line = new Line(point0, (point1 - point0).Normalized());

        Vector3 scale = new Vector3(3.5f);
        point0 *= scale;
        point1 *= scale;
        line.Scale(ref scale);

        Vector3 dummy;
        Assert.IsTrue(GeometryHelper.GetClosestPoint(line, point0, out dummy));
        Assert.IsTrue(GeometryHelper.GetClosestPoint(line, point1, out dummy));
      }
    }


    [Test]
    public void NegativeUniformScaling()
    {
      using (var setEpsilon = new SetEpsilonF(1e-02f))
      {
        Vector3 point0 = new Vector3(10, 20, -40);
        Vector3 point1 = new Vector3(-22, 34, 45);
        Line line = new Line(point0, (point1 - point0).Normalized());

        Vector3 scale = new Vector3(-3.5f);
        point0 *= scale;
        point1 *= scale;
        line.Scale(ref scale);

        Vector3 dummy;
        Assert.IsTrue(GeometryHelper.GetClosestPoint(line, point0, out dummy));
        Assert.IsTrue(GeometryHelper.GetClosestPoint(line, point1, out dummy));
      }
    }


    [Test]
    public void NonuniformScaling()
    {
      Vector3 scale = new Vector3(1, 2, 3);
      Assert.That(() => new Line().Scale(ref scale), Throws.Exception.TypeOf<NotSupportedException>());
    }


    [Test]
    public void ToWorld()
    {
      using (var setEpsilon = new SetEpsilonF(1e-03f))
      {
        Vector3 point0 = new Vector3(10, 20, -40);
        Vector3 point1 = new Vector3(-22, 34, 45);
        Line line = new Line(point0, (point1 - point0).Normalized());

        Pose pose = new Pose(new Vector3(-5, 100, -20), Matrix33F.CreateRotation(new Vector3(1, 2, 3), 0.123f));
        point0 = pose.ToWorldPosition(point0);
        point1 = pose.ToWorldPosition(point1);
        line.ToWorld(ref pose);

        Vector3 dummy;
        Assert.IsTrue(GeometryHelper.GetClosestPoint(line, point0, out dummy));
        Assert.IsTrue(GeometryHelper.GetClosestPoint(line, point1, out dummy));
      }
    }


    [Test]
    public void ToLocal()
    {
      using (var setEpsilon = new SetEpsilonF(1e-04f))
      {
        Vector3 point0 = new Vector3(10, 20, -40);
        Vector3 point1 = new Vector3(-22, 34, 45);
        Line line = new Line(point0, (point1 - point0).Normalized());

        Pose pose = new Pose(new Vector3(-5, 100, -20), Matrix33F.CreateRotation(new Vector3(1, 2, 3), 0.123f));
        point0 = pose.ToLocalPosition(point0);
        point1 = pose.ToLocalPosition(point1);
        line.ToLocal(ref pose);

        Vector3 dummy;
        Assert.IsTrue(GeometryHelper.GetClosestPoint(line, point0, out dummy));
        Assert.IsTrue(GeometryHelper.GetClosestPoint(line, point1, out dummy));
      }
    }
  }
}
