using DigitalRise.Mathematics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Geometry.Shapes.Tests
{
  [TestFixture]
  public class AabbTest
  {
    [Test]
    public void Constructor()
    {
      Assert.AreEqual(new Vector3(), new Aabb().Minimum);
      Assert.AreEqual(new Vector3(), new Aabb().Maximum);

      Assert.AreEqual(new Vector3(), new Aabb(Vector3.Zero, Vector3.Zero).Minimum);
      Assert.AreEqual(new Vector3(), new Aabb(Vector3.Zero, Vector3.Zero).Maximum);

      Assert.AreEqual(new Vector3(10, 20, 30), new Aabb(new Vector3(10, 20, 30), new Vector3(11, 22, 33)).Minimum);
      Assert.AreEqual(new Vector3(11, 22, 33), new Aabb(new Vector3(10, 20, 30), new Vector3(11, 22, 33)).Maximum);
    }


    [Test]
    public void TestProperties()
    {
      Aabb b = new Aabb();
      Assert.AreEqual(new Vector3(), b.Minimum);
      Assert.AreEqual(new Vector3(), b.Maximum);

      b.Minimum = new Vector3(-10, -20, -30);
      Assert.AreEqual(new Vector3(-10, -20, -30), b.Minimum);
      Assert.AreEqual(new Vector3(), b.Maximum);

      b.Maximum = new Vector3(100, 200, 300);
      Assert.AreEqual(new Vector3(-10, -20, -30), b.Minimum);
      Assert.AreEqual(new Vector3(100, 200, 300), b.Maximum);

      Assert.AreEqual(new Vector3(90f / 2, 180f / 2, 270f / 2), b.Center);
      Assert.AreEqual(new Vector3(110, 220, 330), b.Extent);
    }


    [Test]
    public void EqualsTest()
    {
      Assert.IsTrue(new Aabb().Equals(new Aabb()));
      Assert.IsTrue(new Aabb().Equals(new Aabb(Vector3.Zero, Vector3.Zero)));
      Assert.IsTrue(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).Equals(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6))));
      Assert.IsFalse(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).Equals(new Aabb(new Vector3(0, 2, 3), new Vector3(4, 5, 6))));
      Assert.IsFalse(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).Equals(new LineSegmentShape(new Vector3(1, 2, 3), new Vector3(4, 5, 6))));
      Assert.IsFalse(new Aabb().Equals(null));

      Assert.IsTrue(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)) == new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)));
      Assert.IsTrue(new Aabb(new Vector3(1, 2, 4), new Vector3(4, 5, 6)) != new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)));
    }


    [Test]
    public void AreNumericallyEqual()
    {
      AssertExt.AreNumericallyEqual(new Aabb(), new Aabb());
      Assert.IsTrue(Aabb.AreNumericallyEqual(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)), 
                                             new Aabb(new Vector3(1, 2, 3 + Numeric.EpsilonF / 2), new Vector3(4, 5, 6))));
      Assert.IsTrue(Aabb.AreNumericallyEqual(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)),
                                             new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6 + Numeric.EpsilonF / 2))));
      Assert.IsFalse(Aabb.AreNumericallyEqual(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)),
                                              new Aabb(new Vector3(1, 2, 3 + 10 * Numeric.EpsilonF), new Vector3(4, 5, 6))));

      AssertExt.AreNumericallyEqual(new Aabb(), new Aabb());
      Assert.IsTrue(Aabb.AreNumericallyEqual(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)),
                                             new Aabb(new Vector3(1, 2, 3.1f), new Vector3(4, 5, 6)),
                                             0.2f));
      Assert.IsTrue(Aabb.AreNumericallyEqual(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)),
                                             new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5.1f, 6)),
                                             0.2f));
      Assert.IsFalse(Aabb.AreNumericallyEqual(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)),
                                             new Aabb(new Vector3(1, 2, 3.3f), new Vector3(4, 5, 6)),
                                             0.2f));
    }


    [Test]
    public void GetAxisAlignedBoundingBox()
    {
      Assert.AreEqual(new Aabb(), new Aabb().GetAabb(Pose.Identity));
      Assert.AreEqual(new Aabb(new Vector3(10, 100, 1000), new Vector3(10, 100, 1000)),
                      new Aabb().GetAabb(new Pose(new Vector3(10, 100, 1000), Quaternion.Identity)));
      Assert.AreEqual(new Aabb(new Vector3(10, 100, 1000), new Vector3(10, 100, 1000)),
                      new Aabb().GetAabb(new Pose(new Vector3(10, 100, 1000), MathHelper.CreateRotation(new Vector3(1, 2, 3), 0.7f))));
      
      
      Aabb aabb = new Aabb(new Vector3(1, 10, 100), new Vector3(2, 20, 200));
      Assert.AreEqual(aabb, aabb.GetAabb(Pose.Identity));
      Assert.AreEqual(new Aabb(new Vector3(11, 110, 1100), new Vector3(12, 120, 1200)),
                      aabb.GetAabb(new Pose(new Vector3(10, 100, 1000), Quaternion.Identity)));
      // TODO: Test rotations.
    }


    [Test]
    public void Grow()
    {
      var a = new Aabb(new Vector3(1, 2, 3), new Vector3(3, 4, 5));
      a.Grow(new Aabb(new Vector3(1, 2, 3), new Vector3(3, 4, 5)));

      Assert.AreEqual(new Aabb(new Vector3(1, 2, 3), new Vector3(3, 4, 5)), a);

      a.Grow(new Aabb(new Vector3(-1, 2, 3), new Vector3(3, 4, 5)));
      Assert.AreEqual(new Aabb(new Vector3(-1, 2, 3), new Vector3(3, 4, 5)), a);

      a.Grow(new Aabb(new Vector3(1, 2, 3), new Vector3(3, 5, 5)));
      Assert.AreEqual(new Aabb(new Vector3(-1, 2, 3), new Vector3(3, 5, 5)), a);

      var geo = new GeometricObject(new SphereShape(3), new Pose(new Vector3(1, 0, 0)));
      a.Grow(geo);
      Assert.AreEqual(new Aabb(new Vector3(-2, -3, -3), new Vector3(4, 5, 5)), a);
    }


    [Test]
    public void GrowFromPoint()
    {
      var a = new Aabb(new Vector3(1, 2, 3), new Vector3(3, 4, 5));
      a.Grow(new Vector3(10, -20, -30));
      Assert.AreEqual(new Aabb(new Vector3(1, -20, -30), new Vector3(10, 4, 5)), a);
    }


    [Test]
    public void GetHashCodeTest()
    {
      Assert.AreEqual(new Aabb().GetHashCode(), new Aabb().GetHashCode());
      Assert.AreEqual(new Aabb().GetHashCode(), new Aabb(Vector3.Zero, Vector3.Zero).GetHashCode());
      Assert.AreEqual(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetHashCode(), new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetHashCode());
      Assert.AreNotEqual(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetHashCode(), new Aabb(new Vector3(0, 2, 3), new Vector3(4, 5, 6)).GetHashCode());
      Assert.AreNotEqual(new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetHashCode(), new LineSegmentShape(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetHashCode());
    }


    //[Test]
    //public void GetSupportPoint()
    //{
    //  Assert.AreEqual(new Vector3(0, 0, 0), new Aabb().GetSupportPoint(new Vector3(1, 0, 0)));
    //  Assert.AreEqual(new Vector3(0, 0, 0), new Aabb().GetSupportPoint(new Vector3(0, 1, 0)));
    //  Assert.AreEqual(new Vector3(0, 0, 0), new Aabb().GetSupportPoint(new Vector3(0, 0, 1)));
    //  Assert.AreEqual(new Vector3(0, 0, 0), new Aabb().GetSupportPoint(new Vector3(1, 1, 1)));

    //  Assert.AreEqual(new Vector3(4, 5, 6), new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetSupportPoint(new Vector3(1, 0, 0)));
    //  Assert.AreEqual(new Vector3(4, 5, 6), new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetSupportPoint(new Vector3(0, 1, 0)));
    //  Assert.AreEqual(new Vector3(4, 5, 6), new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetSupportPoint(new Vector3(0, 0, 1)));
    //  Assert.AreEqual(new Vector3(1, 5, 6), new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetSupportPoint(new Vector3(-1, 0, 0)));
    //  Assert.AreEqual(new Vector3(4, 2, 6), new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetSupportPoint(new Vector3(0, -1, 0)));
    //  Assert.AreEqual(new Vector3(4, 5, 3), new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetSupportPoint(new Vector3(0, 0, -1)));
    //  Assert.AreEqual(new Vector3(4, 5, 6), new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetSupportPoint(new Vector3(1, 1, 1)));
    //  Assert.AreEqual(new Vector3(1, 2, 3), new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetSupportPoint(new Vector3(-1, -1, -1)));
    //}


    //[Test]
    //public void GetSupportPointDistance()
    //{
    //  Assert.AreEqual(0, new Aabb().GetSupportPointDistance(new Vector3(1, 0, 0)));
    //  Assert.AreEqual(0, new Aabb().GetSupportPointDistance(new Vector3(0, 1, 0)));
    //  Assert.AreEqual(0, new Aabb().GetSupportPointDistance(new Vector3(0, 0, 1)));
    //  Assert.AreEqual(0, new Aabb().GetSupportPointDistance(new Vector3(1, 1, 1)));

    //  Aabb aabb = new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6));
    //  AssertExt.AreNumericallyEqual(4, aabb.GetSupportPointDistance(new Vector3(1, 0, 0)));
    //  AssertExt.AreNumericallyEqual(5, aabb.GetSupportPointDistance(new Vector3(0, 1, 0)));
    //  AssertExt.AreNumericallyEqual(6, aabb.GetSupportPointDistance(new Vector3(0, 0, 1)));
    //  AssertExt.AreNumericallyEqual(-1, aabb.GetSupportPointDistance(new Vector3(-1, 0, 0)));
    //  AssertExt.AreNumericallyEqual(-2, aabb.GetSupportPointDistance(new Vector3(0, -1, 0)));
    //  AssertExt.AreNumericallyEqual(-3, aabb.GetSupportPointDistance(new Vector3(0, 0, -1)));
    //  AssertExt.AreNumericallyEqual(Vector3.Dot(new Vector3(1, 2, 6), new Vector3(-1, -1, 0).Normalized()), aabb.GetSupportPointDistance(new Vector3(-1, -1, 0)));
    //  AssertExt.AreNumericallyEqual(MathHelper.ProjectTo(new Vector3(4, 5, 6), new Vector3(1, 1, 1)).Length, aabb.GetSupportPointDistance(new Vector3(1, 1, 1)));
    //}


    [Test]
    public void Scale()
    {
      Aabb aabb = new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6));
      aabb.Scale(new Vector3(-2, 3, 4));
      Assert.AreEqual(new Aabb(new Vector3(-8, 6, 12), new Vector3(-2, 15, 24)), aabb);

      aabb = new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6));
      aabb.Scale(new Vector3(2, -3, 4));
      Assert.AreEqual(new Aabb(new Vector3(2, -15, 12), new Vector3(8, -6, 24)), aabb);

      aabb = new Aabb(new Vector3(1, 2, 3), new Vector3(4, 5, 6));
      aabb.Scale(new Vector3(2, 3, -4));
      Assert.AreEqual(new Aabb(new Vector3(2, 6, -24), new Vector3(8, 15, -12)), aabb);
    }
  }
}
