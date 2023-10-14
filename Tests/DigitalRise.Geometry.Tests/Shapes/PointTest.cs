using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using DigitalRise.Mathematics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Geometry.Shapes.Tests
{
  [TestFixture]
  public class PointTest
  {
    [Test]
    public void Constructor()
    {
      Assert.AreEqual(new Vector3(), new PointShape().Position);
      Assert.AreEqual(new Vector3(), new PointShape(Vector3.Zero).Position);
      Assert.AreEqual(new Vector3(1, 2, 3), new PointShape(new Vector3(1, 2, 3)).Position);
      Assert.AreEqual(new Vector3(1, 2, 3), new PointShape(1, 2, 3).Position);
    }


    [Test]
    public void InnerPoint()
    {
      Assert.AreEqual(new Vector3(1, 2, 3), new PointShape(new Vector3(1, 2, 3)).InnerPoint);
    }


    [Test]
    public void Position()
    {
      PointShape p = new PointShape();
      Assert.AreEqual(new Vector3(), p.Position);
      p.Position = new Vector3(1, 2, 3);
      Assert.AreEqual(new Vector3(1, 2, 3), p.Position);
    }


    [Test]
    public void GetAxisAlignedBoundingBox()
    {
      Assert.AreEqual(new Aabb(), new PointShape().GetAabb(Pose.Identity));
      Assert.AreEqual(new Aabb(new Vector3(10, 100, -13), new Vector3(10, 100, -13)),
                     new PointShape().GetAabb(new Pose(new Vector3(10, 100, -13),
                                                                         MathHelper.CreateRotation(new Vector3(1, 1, 1), 0.7f))));
      Assert.AreEqual(new Aabb(new Vector3(11, 102, 1003), new Vector3(11, 102, 1003)),
                     new PointShape(new Vector3(1, 2, 3)).GetAabb(new Pose(new Vector3(10, 100, 1000),
                                                                         Quaternion.Identity)));
      Quaternion rotation = MathHelper.CreateRotation(new Vector3(1, 1, 1), 0.7f);
      Vector3 worldPos = rotation.Rotate(new Vector3(1, 2, 3)) + new Vector3(10, 100, 1000);
      AssertExt.AreNumericallyEqual(worldPos, new PointShape(new Vector3(1, 2, 3)).GetAabb(new Pose(new Vector3(10, 100, 1000), rotation)).Minimum);
      AssertExt.AreNumericallyEqual(worldPos, new PointShape(new Vector3(1, 2, 3)).GetAabb(new Pose(new Vector3(10, 100, 1000), rotation)).Maximum);
    }

    [Test]
    public void GetMesh()
    {
      var p = new PointShape(1, 2, 3);

      var m = p.GetMesh(0, 1);
      Assert.AreEqual(1, m.NumberOfTriangles);
      
      Triangle t = m.GetTriangle(0);

      Assert.AreEqual(p.Position, t.Vertex0);
      Assert.AreEqual(p.Position, t.Vertex1);
      Assert.AreEqual(p.Position, t.Vertex2);
    }

    [Test]
    public void GetSupportPoint()
    {
      Assert.AreEqual(new Vector3(0, 0, 0), new PointShape().GetSupportPoint(new Vector3(1, 0, 0)));
      Assert.AreEqual(new Vector3(0, 0, 0), new PointShape().GetSupportPoint(new Vector3(0, 1, 0)));
      Assert.AreEqual(new Vector3(0, 0, 0), new PointShape().GetSupportPoint(new Vector3(0, 0, 1)));
      Assert.AreEqual(new Vector3(0, 0, 0), new PointShape().GetSupportPoint(new Vector3(1, 1, 1)));

      Vector3 pos = new Vector3(1, 2, 3);
      Assert.AreEqual(pos, new PointShape(pos).GetSupportPoint(new Vector3(1, 0, 0)));
      Assert.AreEqual(pos, new PointShape(pos).GetSupportPoint(new Vector3(0, 1, 0)));
      Assert.AreEqual(pos, new PointShape(pos).GetSupportPoint(new Vector3(0, 0, 1)));
      Assert.AreEqual(pos, new PointShape(pos).GetSupportPoint(new Vector3(1, 1, 1)));
    }


    //[Test]
    //public void GetSupportPointDistance()
    //{
    //  Assert.AreEqual(0, new PointShape().GetSupportPointDistance(new Vector3(1, 0, 0)));
    //  Assert.AreEqual(0, new PointShape().GetSupportPointDistance(new Vector3(0, 1, 0)));
    //  Assert.AreEqual(0, new PointShape().GetSupportPointDistance(new Vector3(0, 0, 1)));
    //  Assert.AreEqual(0, new PointShape().GetSupportPointDistance(new Vector3(1, 1, 1)));

    //  Assert.AreEqual(1, new PointShape(new Vector3(1, 2, 3)).GetSupportPointDistance(new Vector3(1, 0, 0)));
    //  Assert.AreEqual(2, new PointShape(new Vector3(1, 2, 3)).GetSupportPointDistance(new Vector3(0, 1, 0)));
    //  Assert.AreEqual(3, new PointShape(new Vector3(1, 2, 3)).GetSupportPointDistance(new Vector3(0, 0, 1)));
    //  AssertExt.AreNumericallyEqual(MathHelper.ProjectTo(new Vector3(1, 2, 3), new Vector3(1, 1, 1)).Length, new PointShape(new Vector3(1, 2, 3)).GetSupportPointDistance(new Vector3(1, 1, 1)));
    //}


    [Test]
    public void Clone()
    {
      PointShape point = new PointShape(new Vector3(1, 2, 3));
      PointShape clone = point.Clone() as PointShape;
      Assert.IsNotNull(clone);
      Assert.AreEqual(point.Position, clone.Position);
      Assert.AreEqual(point.GetAabb(Pose.Identity).Minimum, clone.GetAabb(Pose.Identity).Minimum);
      Assert.AreEqual(point.GetAabb(Pose.Identity).Maximum, clone.GetAabb(Pose.Identity).Maximum);
    }


    [Test]
    public void SerializationXml()
    {
      var a = new PointShape(11, 22, 33);

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
      var b = (PointShape)deserializer.Deserialize(stream);

      Assert.AreEqual(a.Position, b.Position);
    }


    [Test]
    [Ignore("Binary serialization not supported in PCL version.")]
    public void SerializationBinary()
    {
      var a = new PointShape(11, 22, 33);

      // Serialize object.
      var stream = new MemoryStream();
      var formatter = new BinaryFormatter();
      formatter.Serialize(stream, a);

      // Deserialize object.
      stream.Position = 0;
      var deserializer = new BinaryFormatter();
      var b = (PointShape)deserializer.Deserialize(stream);

      Assert.AreEqual(a.Position, b.Position);
    }
  }
}
