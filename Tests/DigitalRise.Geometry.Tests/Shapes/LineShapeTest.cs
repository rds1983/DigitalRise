using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Geometry.Shapes.Tests
{
  [TestFixture]
  public class LineShapeTest
  {
    [Test]
    public void Constructor()
    {
      Assert.AreEqual(new Vector3(), new LineShape().PointOnLine);
      Assert.AreEqual(new Vector3(1, 0, 0), new LineShape().Direction);

      Vector3 pointOnLine = new Vector3(1, 2, 3);
      var direction = new Vector3(4, 5, 6).Normalized();
      LineShape line = new LineShape(pointOnLine, direction);
      Assert.AreEqual(pointOnLine, line.PointOnLine);
      Assert.AreEqual(direction, line.Direction);

      line = new LineShape(new Line(pointOnLine, direction));
      Assert.AreEqual(pointOnLine, line.PointOnLine);
      Assert.AreEqual(direction, line.Direction);
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ConstructorException()
    {
      new LineShape(new Vector3(1, 2, 3), new Vector3());
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ConstructorException2()
    {
      new LineShape(new Line(new Vector3(1, 2, 3), new Vector3(0.1f, 0, 0)));
    }


    [Test]
    public void InnerPoint()
    {
      Vector3 pointOnLine = new Vector3(1, 2, 3);
      var direction = new Vector3(3, 2, 1).Normalized();
      LineShape line = new LineShape(pointOnLine, direction);
      Assert.AreEqual(pointOnLine, line.InnerPoint);
    }


    [Test]
    public void TestProperties()
    {
      LineShape l = new LineShape();
      Assert.AreEqual(new Vector3(), l.PointOnLine);
      Assert.AreEqual(new Vector3(1, 0, 0), l.Direction);

      Vector3 pointOnLine = new Vector3(1, 2, 3);
      l.PointOnLine = pointOnLine;
      Assert.AreEqual(pointOnLine, l.PointOnLine);
      Assert.AreEqual(new Vector3(1, 0, 0), l.Direction);

      var direction = new Vector3(4, 5, 6).Normalized();
      l.Direction = direction;
      Assert.AreEqual(pointOnLine, l.PointOnLine);
      Assert.AreEqual(direction, l.Direction);
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void DirectionException()
    {
      LineShape l = new LineShape();
      l.Direction = new Vector3();
    }


    [Test]
    public void GetAxisAlignedBoundingBox()
    {
      float nInf = float.NegativeInfinity;
      float pInf = float.PositiveInfinity;
      Assert.AreEqual(new Aabb(new Vector3(nInf, 0, 0), new Vector3(pInf, 0, 0)), new LineShape().GetAabb(Pose.Identity));
      Assert.AreEqual(new Aabb(new Vector3(nInf), new Vector3(pInf)),
                     new LineShape().GetAabb(new Pose(new Vector3(10, 100, -13),
                                                                         MathHelper.CreateRotation(new Vector3(1, 1, 1), 0.7f))));
      Assert.AreEqual(new Aabb(new Vector3(11, nInf, 1003), new Vector3(11, pInf, 1003)),
                     new LineShape(new Vector3(1, 2, 3), new Vector3(0, -1, 0)).GetAabb(new Pose(new Vector3(10, 100, 1000),
                                                                   Quaternion.Identity)));
      // TODO: Test rotations.
    }


    [Test]
    public void GetMesh()
    {
      var l = new LineShape(new Vector3(1, 2, 3), Vector3.UnitY);
      LineShape.MeshSize = 10;
      var m = l.GetMesh(0, 1);
      Assert.AreEqual(1, m.NumberOfTriangles);
      Triangle t = m.GetTriangle(0);
      Assert.IsTrue(l.PointOnLine - LineShape.MeshSize / 2 * Vector3.UnitY == t.Vertex0);
      Assert.IsTrue(l.PointOnLine + LineShape.MeshSize / 2 * Vector3.UnitY == t.Vertex2);
    }


    [Test]
    public void Clone()
    {
      LineShape line = new LineShape(new Vector3(1, 2, 3), new Vector3(2, 3, 4).Normalized());
      LineShape clone = line.Clone() as LineShape;
      Assert.IsNotNull(clone);
      Assert.AreEqual(line.PointOnLine, clone.PointOnLine);
      Assert.AreEqual(line.Direction, clone.Direction);
      Assert.AreEqual(line.GetAabb(Pose.Identity).Minimum, clone.GetAabb(Pose.Identity).Minimum);
      Assert.AreEqual(line.GetAabb(Pose.Identity).Maximum, clone.GetAabb(Pose.Identity).Maximum);
    }


    [Test]
    public void SerializationXml()
    {
      var a = new LineShape(new Vector3(1, 2, 3), new Vector3(2, 3, 4).Normalized());

      // Serialize object.
      var stream = new MemoryStream();
      var serializer = new XmlSerializer(typeof(Shape));
      serializer.Serialize(stream, a);

      // Output generated xml. Can be manually checked in output window.
      stream.Position = 0;
      var xml = new StreamReader(stream).ReadToEnd();
      Trace.WriteLine("Serialized Object:\n" + xml);

      // Deserialize object.
      stream.Position = 0;
      var deserializer = new XmlSerializer(typeof(Shape));
      var b = (LineShape)deserializer.Deserialize(stream);

      Assert.AreEqual(a.PointOnLine, b.PointOnLine);
      Assert.AreEqual(a.Direction, b.Direction);
    }


    [Test]
    [Ignore("Binary serialization not supported in PCL version.")]
    public void SerializationBinary()
    {
      var a = new LineShape(new Vector3(1, 2, 3), new Vector3(2, 3, 4).Normalized());

      // Serialize object.
      var stream = new MemoryStream();
      var formatter = new BinaryFormatter();
      formatter.Serialize(stream, a);

      // Deserialize object.
      stream.Position = 0;
      var deserializer = new BinaryFormatter();
      var b = (LineShape)deserializer.Deserialize(stream);

      Assert.AreEqual(a.PointOnLine, b.PointOnLine);
      Assert.AreEqual(a.Direction, b.Direction);
    }
  }
}
