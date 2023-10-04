using DigitalRise.Geometry.Shapes;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Plane = DigitalRise.Geometry.Shapes.Plane;

namespace DigitalRise.Geometry.Collisions.Algorithms.Tests
{
  [TestFixture]
  public class PlanePointAlgorithmTest
  {
    [Test]
    public void TestAlgorithm()
    {
      var plane = new Plane(Vector3.UnitY, new Vector3(100, -10, 0));

      // Separation
      Assert.AreEqual(11, GeometryHelper.GetDistance(plane, new Vector3(1, 1, 1)));
      Assert.AreEqual(new Vector3(1, -10, 1), GeometryHelper.GetClosestPoint(plane, new Vector3(1, 1, 1)));

      // On Plane
      Assert.AreEqual(0, GeometryHelper.GetDistance(plane, new Vector3(1, -10, 1)));
      Assert.AreEqual(new Vector3(1, -10, 1), GeometryHelper.GetClosestPoint(plane, new Vector3(1, -10, 1)));
      
      // Contained
      Assert.AreEqual(-90, GeometryHelper.GetDistance(plane, new Vector3(1, -100, 1)));
      Assert.AreEqual(new Vector3(1, -10, 1), GeometryHelper.GetClosestPoint(plane, new Vector3(1, -100, 1)));

      // Are Points On Opposite Plane Sides
      Assert.AreEqual(true, GeometryHelper.ArePointsOnOppositeSides(new Vector3(-1, 2, 3), new Vector3(10, -1, 4),
                                                                            new Vector3(2, 0, 0), new Vector3(2, 3, 1), new Vector3(2, -100, 12)).Value);
      Assert.AreEqual(false, GeometryHelper.ArePointsOnOppositeSides(new Vector3(2.1f, 2, 3), new Vector3(10, -1, 4),
                                                                            new Vector3(2, 0, 0), new Vector3(2, 3, 1), new Vector3(2, -100, 12)).Value);
      Assert.AreEqual(null, GeometryHelper.ArePointsOnOppositeSides(new Vector3(2, 2, 3), new Vector3(10, -1, 4),
                                                                            new Vector3(2, 0, 0), new Vector3(2, 3, 1), new Vector3(2, -100, 12)));
    }
  }
}
