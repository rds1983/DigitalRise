using System;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Mathematics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Geometry.Collisions.Algorithms.Tests
{
  [TestFixture]
  public class GjkTest
  {
    [Test]
    [ExpectedException(typeof(GeometryException))]
    public void ShouldThrowWhenContactQuery()
    {
      new Gjk(new CollisionDetection()).GetContacts(new CollisionObject(), new CollisionObject());
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ShouldThrowWhenShapesAreNotConvex()
    {
      new Gjk(new CollisionDetection()).GetClosestPoints(new CollisionObject(), new CollisionObject());
    }


    [Test]
    public void ComputeCollision()
    {
      Gjk algo = new Gjk(new CollisionDetection());

      CollisionObject a = new CollisionObject
      {
        GeometricObject = new GeometricObject(new TriangleShape(new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1)), Pose.Identity),
      };

      CollisionObject b = new CollisionObject
      {
        GeometricObject = new GeometricObject(new SphereShape(1), Pose.Identity),
      };

      ContactSet set;

      set = algo.GetClosestPoints(a, b);
      Assert.AreEqual(true, algo.HaveContact(a, b));
      Assert.AreEqual(0, set[0].PenetrationDepth);

      ((GeometricObject)b.GeometricObject).Pose = new Pose(new Vector3(2, 0.1f, 0.2f));
      algo.UpdateClosestPoints(set, 0);
      Assert.AreEqual(false, algo.HaveContact(a, b));
      AssertExt.AreNumericallyEqual(-1, set[0].PenetrationDepth, 0.001f);
      AssertExt.AreNumericallyEqual(new Vector3(0, 0.1f, 0.2f), set[0].PositionAWorld, 0.01f);
      AssertExt.AreNumericallyEqual(new Vector3(1, 0.1f, 0.2f), set[0].PositionBWorld, 0.01f);
    }
  }
}