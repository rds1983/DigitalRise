using System;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Mathematics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Geometry.Collisions.Algorithms.Tests
{
  [TestFixture]
  public class PlaneRayAlgorithmTest
  {
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void TestException0()
    {
      new PlaneRayAlgorithm(new CollisionDetection()).HaveContact(
        new CollisionObject(new GeometricObject(new PlaneShape())),
        new CollisionObject(new GeometricObject(new SphereShape())));
    }

    [Test]
    public void Test1()
    {
      PlaneRayAlgorithm algo = new PlaneRayAlgorithm(new CollisionDetection());

      // Plane in xz plane.
      CollisionObject a = new CollisionObject(new GeometricObject
      {
        Shape = new PlaneShape(Vector3.UnitY, 1),
        Pose = new Pose(new Vector3(0, -1, 0)),
      });

      // Ray
      CollisionObject b = new CollisionObject(new GeometricObject
      {
        Shape = new RayShape(new Vector3(0, 0, 0), new Vector3(-1, 0, 0), 100),
        Pose = new Pose(new Vector3(0, 1, 0)),
      });

      // Separated
      Assert.AreEqual(false, algo.HaveContact(a, b));
      Assert.AreEqual(false, algo.HaveContact(b, a));
      Assert.AreEqual(0, algo.GetContacts(a, b).Count);
      Assert.AreEqual(-1, algo.GetClosestPoints(a, b)[0].PenetrationDepth);

      // Contained
      ((GeometricObject)b.GeometricObject).Pose = new Pose(new Vector3(0, -1, 0));
      Assert.AreEqual(true, algo.HaveContact(a, b));
      Assert.AreEqual(true, algo.HaveContact(b, a));
      Assert.AreEqual(1, algo.GetContacts(a, b).Count);
      Assert.AreEqual(0, algo.GetClosestPoints(a, b)[0].PenetrationDepth);

      // Touching
      ((GeometricObject)b.GeometricObject).Pose = new Pose(new Vector3(0, 0, 0));
      Assert.AreEqual(true, algo.HaveContact(a, b));
      Assert.AreEqual(true, algo.HaveContact(b, a));
      Assert.AreEqual(1, algo.GetContacts(a, b).Count);
      Assert.AreEqual(0, algo.GetClosestPoints(a, b)[0].PenetrationDepth);


      // Shooting into plane.
      ((GeometricObject)b.GeometricObject).Pose = new Pose(new Vector3(0, 1, 0), MathHelper.CreateRotationZ(ConstantsF.PiOver2));
      Assert.AreEqual(1, algo.GetContacts(a, b).Count);
      Assert.AreEqual(1, algo.GetContacts(b, a)[0].PenetrationDepth);
      AssertExt.AreNumericallyEqual(new Vector3(-1, 0, 0), algo.GetContacts(b, a)[0].PositionALocal);
      Assert.AreEqual(1, algo.GetClosestPoints(a, b)[0].PenetrationDepth);
    }

    [Test]
    public void Test2()
    {
      // Test following: A ray touches a plane at the ray-end. Then the plane moves so that
      // they are separated.

      PlaneRayAlgorithm algo = new PlaneRayAlgorithm(new CollisionDetection());

      // Plane in xz plane.
      CollisionObject plane = new CollisionObject(new GeometricObject
      {
        Shape = new PlaneShape(Vector3.UnitY, 1),
        Pose = new Pose(new Vector3(0, -1, 0)),
      });

      // Ray
      CollisionObject ray = new CollisionObject(new GeometricObject
      {
        Shape = new RayShape(new Vector3(0, 0, 0), new Vector3(0, -1, 0), 10),
        Pose = new Pose(new Vector3(0, 10, 0)),
      });

      ContactSet contacts = algo.GetContacts(plane, ray);
      Assert.AreEqual(true, contacts.HaveContact);
      Assert.AreEqual(1, contacts.Count);
      Assert.AreEqual(10, contacts[0].PenetrationDepth);

      // Move plane less than contact position tolerance, but into a separated state.
      ((GeometricObject)plane.GeometricObject).Pose = new Pose(new Vector3(0, -1.001f, 0));
      algo.UpdateClosestPoints(contacts, 0);
      Assert.AreEqual(false, contacts.HaveContact);
      Assert.AreEqual(1, contacts.Count);
      AssertExt.AreNumericallyEqual(-0.001f, contacts[0].PenetrationDepth);

    }
  }
}
