using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;


namespace DigitalRise.Geometry.Shapes.Tests
{
  [TestFixture]
  public class LineSegmentTest
  {
    [Test]
    public void Constructor()
    {
      Assert.AreEqual(new Vector3(), new LineSegment().Start);
      Assert.AreEqual(new Vector3(), new LineSegment().End);

      Assert.AreEqual(new Vector3(), new LineSegment(Vector3.Zero, Vector3.Zero).Start);
      Assert.AreEqual(new Vector3(), new LineSegment(Vector3.Zero, Vector3.Zero).End);

      Assert.AreEqual(new Vector3(10, 20, 30), new LineSegment(new Vector3(10, 20, 30), new Vector3(11, 22, 33)).Start);
      Assert.AreEqual(new Vector3(11, 22, 33), new LineSegment(new Vector3(10, 20, 30), new Vector3(11, 22, 33)).End);

      Assert.AreEqual(new Vector3(10, 20, 30), new LineSegment(new LineSegmentShape(new Vector3(10, 20, 30), new Vector3(11, 22, 33))).Start);
      Assert.AreEqual(new Vector3(11, 22, 33), new LineSegment(new LineSegmentShape(new Vector3(10, 20, 30), new Vector3(11, 22, 33))).End);
    }


    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ConstructorArgumentNullException()
    {
      new LineSegment(null);
    }


    [Test]
    public void EqualsTest()
    {
      Assert.IsTrue(new LineSegment().Equals(new LineSegment()));
      Assert.IsTrue(new LineSegment().Equals(new LineSegment(Vector3.Zero, Vector3.Zero)));
      Assert.IsTrue(new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).Equals(new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6))));
      Assert.IsTrue(new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).Equals((object)new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6))));
      Assert.IsFalse(new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).Equals(new LineSegment(new Vector3(0, 2, 3), new Vector3(4, 5, 6))));
      Assert.IsFalse(new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).Equals(new Line(new Vector3(1, 2, 3), new Vector3(4, 5, 6))));
      Assert.IsFalse(new LineSegment().Equals(null));

      Assert.IsTrue(new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6)) == new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6)));
      Assert.IsTrue(new LineSegment(new Vector3(1, 2, 4), new Vector3(4, 5, 6)) != new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6)));
    }


    [Test]
    public void LengthTest()
    {
      Assert.AreEqual(2, new LineSegment(new Vector3(1, 2, 3), new Vector3(3, 2, 3)).Length);
    }


    [Test]
    public void LengthSquaredTest()
    {
      Assert.AreEqual(4, new LineSegment(new Vector3(1, 2, 3), new Vector3(3, 2, 3)).LengthSquared());
    }


    [Test]
    public void GetHashCodeTest()
    {
      Assert.AreEqual(new LineSegment().GetHashCode(), new LineSegment().GetHashCode());
      Assert.AreEqual(new LineSegment().GetHashCode(), new LineSegment(Vector3.Zero, Vector3.Zero).GetHashCode());
      Assert.AreEqual(new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetHashCode(), new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetHashCode());
      Assert.AreNotEqual(new LineSegment(new Vector3(1, 2, 3), new Vector3(4, 5, 6)).GetHashCode(), new LineSegment(new Vector3(0, 2, 3), new Vector3(4, 5, 6)).GetHashCode());
    }
  }
}
