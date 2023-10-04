using System;
using DigitalRise.Geometry.Shapes;
using Microsoft.Xna.Framework;
using NUnit.Framework;


namespace DigitalRise.Geometry.Collisions.Algorithms.Tests
{
  [TestFixture]
  public class CcdTest
  {
    [Test]
    public void TestException()
    {
      var sphere0 = new SphereShape(1);
      var geo0 = new GeometricObject(sphere0);
      var geo1 = new GeometricObject(sphere0);
      var co0 = new CollisionObject(geo0);
      var co1 = new CollisionObject(geo1);

      geo0.Pose = Pose.Identity;
      geo1.Pose = new Pose(new Vector3(10, 0, 0));

      float toi = CcdHelper.GetTimeOfImpactLinearSweep(co0, new Pose(new Vector3(4, 0, 0)), co1, new Pose(new Vector3(6, 0, 0)), 0.01f);
      Assert.AreEqual(1, toi);

      toi = CcdHelper.GetTimeOfImpactLinearCA(co0, new Pose(new Vector3(4, 0, 0)), co1, new Pose(new Vector3(6, 0, 0)), 0.01f, new CollisionDetection());
      Assert.AreEqual(1, toi);

      toi = CcdHelper.GetTimeOfImpactCA(co0, new Pose(new Vector3(4, 0, 0)), co1, new Pose(new Vector3(6, 0, 0)), 0.01f, new CollisionDetection());
      Assert.AreEqual(1, toi);

      toi = CcdHelper.GetTimeOfImpactLinearSweep(co0, new Pose(new Vector3(4f, 0, 0)), co1, new Pose(new Vector3(6f, 0, 0)), 0.01f);
      Assert.AreEqual(1, toi);

      toi = CcdHelper.GetTimeOfImpactLinearCA(co0, new Pose(new Vector3(4f, 0, 0)), co1, new Pose(new Vector3(6f, 0, 0)), 0.01f, new CollisionDetection());
      Assert.AreEqual(1, toi);

      toi = CcdHelper.GetTimeOfImpactCA(co0, new Pose(new Vector3(4f, 0, 0)), co1, new Pose(new Vector3(6f, 0, 0)), 0.01f, new CollisionDetection());
      Assert.AreEqual(1, toi);

      toi = CcdHelper.GetTimeOfImpactLinearSweep(co0, new Pose(new Vector3(7f, 0, 0)), co1, new Pose(new Vector3(4f, 0, 0)), 0.01f);
      Assert.IsTrue(toi > 0 && toi < 1);

      toi = CcdHelper.GetTimeOfImpactLinearCA(co0, new Pose(new Vector3(7f, 0, 0)), co1, new Pose(new Vector3(6f, 0, 0)), 0.01f, new CollisionDetection());
      Assert.IsTrue(toi > 0 && toi < 1);

      toi = CcdHelper.GetTimeOfImpactCA(co0, new Pose(new Vector3(7f, 0, 0)), co1, new Pose(new Vector3(4f, 0, 0)), 0.01f, new CollisionDetection());
      Assert.IsTrue(toi > 0 && toi < 1);

      // Moving away
      toi = CcdHelper.GetTimeOfImpactLinearSweep(co0, new Pose(new Vector3(-1f, 0, 0)), co1, new Pose(new Vector3(11f, 0, 0)), 0.01f);
      Assert.AreEqual(1, toi);

      toi = CcdHelper.GetTimeOfImpactLinearCA(co0, new Pose(new Vector3(-1f, 0, 0)), co1, new Pose(new Vector3(11f, 0, 0)), 0.01f, new CollisionDetection());
      Assert.AreEqual(1, toi);

      toi = CcdHelper.GetTimeOfImpactCA(co0, new Pose(new Vector3(-1f, 0, 0)), co1, new Pose(new Vector3(11f, 0, 0)), 0.01f, new CollisionDetection());
      Assert.AreEqual(1, toi);

      // Touching at start. => GetTimeOfImpact result is invalid when objects touch at the start.
      //geo0.Pose = new Pose(new Vector3(9, 0, 0));
      //toi = CcdHelper.GetTimeOfImpactLinearSweep(co0, new Pose(new Vector3(10f, 0, 0)), co1, new Pose(new Vector3(5f, 0, 0)), 0.01f);
      //Assert.AreEqual(0, toi);

      //toi = CcdHelper.GetTimeOfImpactLinearCA(co0, new Pose(new Vector3(10f, 0, 0)), co1, new Pose(new Vector3(5f, 0, 0)), 0.01f, new CollisionDetection());
      //Assert.AreEqual(0, toi);

      //toi = CcdHelper.GetTimeOfImpactCA(co0, new Pose(new Vector3(10f, 0, 0)), co1, new Pose(new Vector3(5f, 0, 0)), 0.01f, new CollisionDetection());
      //Assert.AreEqual(0, toi);
    }
  }
}