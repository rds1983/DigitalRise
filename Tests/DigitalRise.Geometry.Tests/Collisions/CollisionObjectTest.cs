using DigitalRise.Geometry.Shapes;
using Microsoft.Xna.Framework;
using NUnit.Framework;


namespace DigitalRise.Geometry.Collisions.Tests
{
  [TestFixture]
  public class CollisionObjectTest
  {
    [Test]
    public void AxisAlignedBoundingBox()
    {
      CollisionObject obj = new CollisionObject(new GeometricObject(new SphereShape(0.3f), new Pose(new Vector3(1, 2, 3))));

      Assert.AreEqual(new Aabb(new Vector3(0.7f, 1.7f, 2.7f), new Vector3(1.3f, 2.3f, 3.3f)),
                      obj.GeometricObject.Aabb);
    }


    [Test]
    public void ConstructorTest()
    {
      Assert.AreEqual(typeof(EmptyShape), new CollisionObject().GeometricObject.Shape.GetType());
      Assert.AreEqual(Pose.Identity, new CollisionObject().GeometricObject.Pose);
    }


    //[Test]
    //public void ToStringTest()
    //{
    //  Assert.AreEqual("CollisionObject{Name=\"\"}", new CollisionObject().ToString());
    //  Assert.AreEqual("CollisionObject{Name=\"Cube1\"}", new CollisionObject { Name = "Cube1" }.ToString());
    //}
  }
}
