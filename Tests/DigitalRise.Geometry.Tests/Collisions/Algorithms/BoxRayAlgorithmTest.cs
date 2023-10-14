using System;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Mathematics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Geometry.Collisions.Algorithms.Tests
{
  [TestFixture]
  public class BoxRayAlgorithmTest
  {
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void TestException0()
    {
      new RayBoxAlgorithm(new CollisionDetection()).HaveContact(
        new CollisionObject(new GeometricObject(new RayShape())),
        new CollisionObject(new GeometricObject(new SphereShape())));
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void TestException1()
    {
      new RayBoxAlgorithm(new CollisionDetection()).HaveContact(
        new CollisionObject(new GeometricObject(new BoxShape())),
        new CollisionObject(new GeometricObject(new PointShape())));
    }


    [Test]
    public void ComputeCollision()
    {
      RayBoxAlgorithm algo = new RayBoxAlgorithm(new CollisionDetection());

      CollisionObject ray = new CollisionObject(new GeometricObject
                  {
                    Shape = new RayShape(new Vector3(0, 0, 0), new Vector3(-1, 0, 0), 10),
                    Pose = new Pose(new Vector3(0, 0, 0)),
                  });

      CollisionObject box = new CollisionObject(new GeometricObject
                  {
                    Shape = new BoxShape(2, 4, 8),
                  });

      ContactSet set;

      set = algo.GetClosestPoints(ray, box);
      Assert.AreEqual(true, algo.HaveContact(ray, box));
      Assert.AreEqual(true, algo.HaveContact(box, ray));
      Assert.AreEqual(0, set[0].PenetrationDepth);
      Assert.AreEqual(0, algo.GetContacts(box, ray)[0].PenetrationDepth);
      Assert.AreEqual(new Vector3(), algo.GetContacts(box, ray)[0].Position);

      // Hit + x face.
      ((GeometricObject)box.GeometricObject).Pose = new Pose(new Vector3(-1, 2, -4));
      ((GeometricObject)ray.GeometricObject).Pose = new Pose(new Vector3(1, 0.5f, -2));
      set = algo.GetClosestPoints(ray, box);
      Assert.AreEqual(true, algo.HaveContact(ray, box));
      Assert.AreEqual(true, algo.HaveContact(box, ray));
      Assert.AreEqual(1, set[0].PenetrationDepth);
      Assert.AreEqual(1, algo.GetContacts(box, ray)[0].PenetrationDepth);
      Assert.AreEqual(-Vector3.UnitX, algo.GetContacts(ray, box)[0].Normal);
      Assert.AreEqual(new Vector3(0, 0.5f, -2), algo.GetContacts(box, ray)[0].Position);

      ((GeometricObject)ray.GeometricObject).Pose = new Pose(new Vector3(3, 2, -2));
      ((RayShape) ray.GeometricObject.Shape).Direction = new Vector3(-1, 1, 0).Normalized();
      algo.UpdateContacts(set, 0);
      Assert.AreEqual(false, algo.HaveContact(ray, box));
      Assert.AreEqual(false, algo.HaveContact(box, ray));
      Assert.AreEqual(0, set.Count);
      AssertExt.AreNumericallyEqual(-(float) Math.Sqrt(0.5*0.5 + 0.5*0.5), algo.GetClosestPoints(ray, box)[0].PenetrationDepth);

      // Face is separating plane.
      ((GeometricObject)ray.GeometricObject).Pose = new Pose(new Vector3(3, 2, -2));
      ((RayShape) ray.GeometricObject.Shape).Direction = new Vector3(0, 1, 0);
      algo.UpdateContacts(set, 0);
      Assert.AreEqual(false, algo.HaveContact(ray, box));
      Assert.AreEqual(false, algo.HaveContact(box, ray));
      Assert.AreEqual(0, set.Count);

      // Hit -x face.
      ((GeometricObject)ray.GeometricObject).Pose = new Pose(new Vector3(-5, -1, 0));
      ((RayShape) ray.GeometricObject.Shape).Direction = new Vector3(1, 1, 0).Normalized();
      set = set.Swapped;
      algo.UpdateContacts(set, 0);
      Assert.AreEqual(true, algo.HaveContact(ray, box));
      Assert.AreEqual(true, algo.HaveContact(box, ray));
      AssertExt.AreNumericallyEqual((float)Math.Sqrt(3*3 + 3*3), set[0].PenetrationDepth);
      Assert.AreEqual(-Vector3.UnitX, set[0].Normal);
    }
  }
}
