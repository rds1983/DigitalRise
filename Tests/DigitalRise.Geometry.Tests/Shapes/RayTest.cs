using System;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Geometry.Shapes.Tests
{
  [TestFixture]
  public class RayTest
  {
    [Test]
    public void Constructor()
    {
      Assert.AreEqual(new Vector3(), new Ray().Origin);
      Assert.AreEqual(new Vector3(), new Ray().Direction);
      Assert.AreEqual(0, new Ray().Length);

      Vector3 origin = Vector3.Zero;
      Vector3 direction = Vector3.Zero;
      float length = 0;
      Assert.AreEqual(origin, new Ray(origin, direction, length).Origin);
      Assert.AreEqual(direction, new Ray(origin, direction, length).Direction);
      Assert.AreEqual(length, new Ray(origin, direction, length).Length);

      origin = new Vector3(10, 20, 30);
      direction = new Vector3(1, 2, 3).Normalized();
      length = 10;
      Assert.AreEqual(origin, new Ray(origin, direction, length).Origin);
      Assert.AreEqual(direction, new Ray(origin, direction, length).Direction);
      Assert.AreEqual(length, new Ray(origin, direction, length).Length);

      Assert.AreEqual(origin, new Ray(new RayShape(origin, direction, length)).Origin);
      Assert.AreEqual(direction, new Ray(new RayShape(origin, direction, length)).Direction);
      Assert.AreEqual(length, new Ray(new RayShape(origin, direction, length)).Length);
    }


    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ConstructorArgumentNullException()
    {
      new Ray(null);
    }


    [Test]
    public void EqualsTest()
    {
      Assert.IsTrue(new Ray().Equals(new Ray()));
      Assert.IsTrue(new Ray().Equals(new Ray(Vector3.Zero, Vector3.Zero, 0)));

      Vector3 origin = new Vector3(10, 20, 30);
      Vector3 direction = new Vector3(1, 2, 3).Normalized();
      float length = 10;
      Assert.IsTrue(new Ray(origin, direction, length).Equals(new Ray(origin, direction, length)));
      Assert.IsTrue(new Ray(origin, direction, length).Equals((object)new Ray(origin, direction, length)));
      Assert.IsFalse(new Ray(new Vector3(1, 2, 3), new Vector3(4, 5, 6).Normalized(), length).Equals(new Ray(new Vector3(0, 2, 3), new Vector3(4, 5, 6).Normalized(), length)));
      Assert.IsFalse(new Ray(new Vector3(1, 2, 3), new Vector3(4, 5, 6).Normalized(), length).Equals(new Ray(new Vector3(1, 2, 3), new Vector3(4, 5, 5).Normalized(), length)));
      Assert.IsFalse(new Ray(new Vector3(1, 2, 3), new Vector3(4, 5, 6).Normalized(), length).Equals(new Ray(new Vector3(1, 2, 3), new Vector3(4, 5, 6).Normalized(), length + 1)));
      Assert.IsFalse(new Ray(new Vector3(1, 2, 3), new Vector3(4, 5, 6).Normalized(), length).Equals(new RayShape(new Vector3(1, 2, 3), new Vector3(4, 5, 6).Normalized(), length)));
      Assert.IsFalse(new Ray().Equals(null));

      Assert.IsTrue(new Ray(origin, direction, length) == new Ray(origin, direction, length));
      Assert.IsTrue(new Ray(origin, direction, length) != new Ray(origin * 2, direction, length));
      Assert.IsTrue(new Ray(origin, direction, length) != new Ray(origin, direction * 0.9f, length));
      Assert.IsTrue(new Ray(origin, direction, length) != new Ray(origin, direction, length + 1));
    }


    [Test]
    public void GetHashCodeTest()
    {
      Assert.AreEqual(new Ray().GetHashCode(), new Ray().GetHashCode());
      Assert.AreEqual(new Ray().GetHashCode(), new Ray(Vector3.Zero, Vector3.Zero, 0).GetHashCode());

      Vector3 origin = new Vector3(10, 20, 30);
      Vector3 direction = new Vector3(1, 2, 3).Normalized();
      float length = 10;
      Assert.AreEqual(new Ray(origin, direction, length).GetHashCode(), new Ray(origin, direction, length).GetHashCode());
      Assert.AreNotEqual(new Ray(origin, direction, length).GetHashCode(), new Ray(origin * 2, direction, length).GetHashCode());
      Assert.AreNotEqual(new Ray(origin, direction, length).GetHashCode(), new Ray(origin, direction * 0.9f, length).GetHashCode());
      Assert.AreNotEqual(new Ray(origin, direction, length).GetHashCode(), new Ray(origin, direction, length * 10).GetHashCode());
    }


    [Test]
    public void ToStringTest()
    {
      Assert.IsTrue(new Ray().ToString().StartsWith("Ray"));
    }


    [Test]
    public void PositiveUniformScaling()
    {
      Vector3 origin = new Vector3(1, 2, 3);
      Vector3 direction = new Vector3(-2, 3, -5).Normalized();
      float length = 100;
      Ray ray = new Ray(origin, direction, length);
      Vector3 pointOnRay = ray.Origin + ray.Direction * 10;
      
      Vector3 scale = new Vector3(3.5f);
      origin *= scale;
      direction = (direction * scale).Normalized();
      length *= scale.X;
      pointOnRay *= scale;
      ray.Scale(ref scale);

      Assert.AreEqual(origin, ray.Origin);
      Assert.AreEqual(direction, ray.Direction);
      Assert.AreEqual(length, ray.Length);
      AssertExt.AreNumericallyEqual(pointOnRay, ray.Origin + ray.Direction * 10 * 3.5f);
    }


    [Test]
    public void NegativeUniformScaling()
    {
      Vector3 origin = new Vector3(1, 2, 3);
      Vector3 direction = new Vector3(-2, 3, -5).Normalized();
      float length = 100;
      Ray ray = new Ray(origin, direction, length);
      Vector3 endPoint = ray.Origin + ray.Direction * ray.Length;

      Vector3 scale = new Vector3(-3.5f);
      origin *= scale;
      direction = (direction * scale).Normalized();
      length *= Math.Abs(scale.X);
      endPoint *= scale;
      ray.Scale(ref scale);

      Assert.AreEqual(origin, ray.Origin);
      Assert.AreEqual(direction, ray.Direction);
      Assert.AreEqual(length, ray.Length);
      AssertExt.AreNumericallyEqual(endPoint, ray.Origin + ray.Direction * ray.Length);
    }


    [Test]
    public void NonuniformScaling()
    {
      using (var setEpsilon = new SetEpsilonF(1E-04f))
      {
        Vector3 scale = new Vector3(1, 2, 3);
        Vector3 origin = new Vector3(1, 2, 3);
        Vector3 direction = new Vector3(-2, 3, -5).Normalized();
        float length = 100;
        Ray ray = new Ray(origin, direction, length);
        Vector3 endPoint = ray.Origin + ray.Direction * ray.Length;

        ray.Scale(ref scale);

        origin = origin * scale;
        endPoint = endPoint * scale;

        Assert.AreEqual(origin, ray.Origin);
        Assert.AreEqual((endPoint - origin).Normalized(), ray.Direction);
        Assert.AreEqual((endPoint - origin).Length(), ray.Length);
        AssertExt.AreNumericallyEqual(endPoint, ray.Origin + ray.Direction * ray.Length);
      }
    }


    [Test]
    public void ToWorld()
    {
      using (var setEpsilon = new SetEpsilonF(1E-04f))
      {
        Vector3 startPoint = new Vector3(10, 20, -40);
        Vector3 endPoint = new Vector3(-22, 34, 45);
        Ray ray = new Ray(startPoint, (endPoint - startPoint).Normalized(), (endPoint - startPoint).Length());

        Pose pose = new Pose(new Vector3(-5, 100, -20), Matrix33F.CreateRotation(new Vector3(1, 2, 3), 0.123f));
        startPoint = pose.ToWorldPosition(startPoint);
        endPoint = pose.ToWorldPosition(endPoint);
        ray.ToWorld(ref pose);

        AssertExt.AreNumericallyEqual(startPoint, ray.Origin);
        AssertExt.AreNumericallyEqual(endPoint, ray.Origin + ray.Direction * ray.Length);
      }
    }


    [Test]
    public void ToLocal()
    {
      Vector3 startPoint = new Vector3(10, 20, -40);
      Vector3 endPoint = new Vector3(-22, 34, 45);
      Ray ray = new Ray(startPoint, (endPoint - startPoint).Normalized(), (endPoint - startPoint).Length());

      Pose pose = new Pose(new Vector3(-5, 100, -20), Matrix33F.CreateRotation(new Vector3(1, 2, 3), 0.123f));
      startPoint = pose.ToLocalPosition(startPoint);
      endPoint = pose.ToLocalPosition(endPoint);
      ray.ToLocal(ref pose);

      AssertExt.AreNumericallyEqual(startPoint, ray.Origin);
      AssertExt.AreNumericallyEqual(endPoint, ray.Origin + ray.Direction * ray.Length);
    }
  }
}
