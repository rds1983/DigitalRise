using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Interpolation;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Geometry.Tests
{

  [TestFixture]
  public class PoseTest
  {
    [Test]
    public void Test1()
    {
      Pose p = Pose.Identity;

      Assert.AreEqual(Matrix44F.Identity, p.ToMatrix44F());
      Assert.AreEqual(Matrix33F.Identity, p.Orientation);
      Assert.AreEqual(Vector3.Zero, p.Position);

      p.Position = new Vector3(1, 2, 3);

      p.Orientation = Matrix33F.CreateRotation(new Vector3(3, -4, 9), 0.49f);
      AssertExt.AreNumericallyEqual(new Vector4(p.ToWorldDirection(Vector3.UnitX), 0), p * new Vector4(1, 0, 0, 0));
      AssertExt.AreNumericallyEqual(new Vector4(p.ToWorldDirection(Vector3.UnitY), 0), p * new Vector4(0, 1, 0, 0));
      AssertExt.AreNumericallyEqual(new Vector4(p.ToWorldDirection(Vector3.UnitZ), 0), p * new Vector4(0, 0, 1, 0));
      AssertExt.AreNumericallyEqual(new Vector4(p.ToWorldPosition(Vector3.UnitX), 1), p * new Vector4(1, 0, 0, 1));
      AssertExt.AreNumericallyEqual(new Vector4(p.ToWorldPosition(Vector3.UnitY), 1), p * new Vector4(0, 1, 0, 1));
      AssertExt.AreNumericallyEqual(new Vector4(p.ToWorldPosition(Vector3.UnitZ), 1), p * new Vector4(0, 0, 1, 1));
      AssertExt.AreNumericallyEqual(new Vector4(p.ToLocalDirection(Vector3.UnitX), 0), p.Inverse * new Vector4(1, 0, 0, 0));
      AssertExt.AreNumericallyEqual(new Vector4(p.ToLocalDirection(Vector3.UnitY), 0), p.Inverse * new Vector4(0, 1, 0, 0));
      AssertExt.AreNumericallyEqual(new Vector4(p.ToLocalDirection(Vector3.UnitZ), 0), p.Inverse * new Vector4(0, 0, 1, 0));
      AssertExt.AreNumericallyEqual(new Vector4(p.ToLocalPosition(Vector3.UnitX), 1), p.Inverse * new Vector4(1, 0, 0, 1));
      AssertExt.AreNumericallyEqual(new Vector4(p.ToLocalPosition(Vector3.UnitY), 1), p.Inverse * new Vector4(0, 1, 0, 1));
      AssertExt.AreNumericallyEqual(new Vector4(p.ToLocalPosition(Vector3.UnitZ), 1), p.Inverse * new Vector4(0, 0, 1, 1));

      Pose p2 = Pose.FromMatrix(new Matrix44F(p.Orientation, Vector3.Zero));
      AssertExt.AreNumericallyEqual(p.Orientation, p2.Orientation);
      AssertExt.AreNumericallyEqual(p2.Position, Vector3.Zero);

      Matrix44F m = p2;
      m.SetColumn(3, new Vector4(p.Position, 1));
      p2 = Pose.FromMatrix(m);
      AssertExt.AreNumericallyEqual(p.Orientation, p2.Orientation);
      Assert.AreEqual(p.Position, p2.Position);
      //AssertExt.AreNumericallyEqual(p.Position, p2.Position);

      // Test other constructors.
      Assert.AreEqual(Vector3.Zero, new Pose(MathHelper.CreateRotationX(0.3f)).Position);
      Assert.AreEqual(Matrix33F.CreateRotationX(0.3f), new Pose(Matrix33F.CreateRotationX(0.3f)).Orientation);
      Assert.AreEqual(new Vector3(1, 2, 3), new Pose(new Vector3(1, 2, 3)).Position);
      Assert.AreEqual(Matrix33F.Identity, new Pose(new Vector3(1, 2, 3)).Orientation);
    }


    [Test]
    public void IsValid()
    {
      Matrix44F inValidPose = new Matrix44F(new float[,] { {1, 2, 3, 0},
                                                           {4, 5, 6, 0},
                                                           {7, 8, 9, 0},
                                                           {0, 0, 0, 1},
                                                         });

      Assert.IsFalse(Pose.IsValid(inValidPose));

      Assert.IsTrue(Pose.IsValid(Matrix44F.CreateRotationZ(0.3f)));


      inValidPose = new Matrix44F(new float[,] { {1, 0, 0, 0},
                                                 {0, 1, 0, 0},
                                                 {0, 0, 1, 0},
                                                 {0, 1, 0, 1},
                                                });
      Assert.IsFalse(Pose.IsValid(inValidPose));

      inValidPose = new Matrix44F(new float[,] { {1, 0, 0, 0},
                                                 {0, 1, 0, 0},
                                                 {0, 0, -1, 0},
                                                 {0, 1, 0, 1},
                                                });
      Assert.IsFalse(Pose.IsValid(inValidPose));
    }


    [Test]
    public void Equals()
    {
      Pose p1 = new Pose(new Vector3(1, 2, 3), MathHelper.CreateRotationY(0.3f));
      Pose p2 = new Pose(new Vector3(1, 2, 3), MathHelper.CreateRotationY(0.3f));

      Assert.AreEqual(p1, p2);
      Assert.IsTrue(p1.Equals((object)p2));
      Assert.IsTrue(p1.Equals(p2));
      Assert.IsFalse(p1.Equals(p2.ToMatrix44F()));
    }


    [Test]
    public void GetHashCodeTest()
    {
      Pose p1 = new Pose(new Vector3(1, 2, 3), MathHelper.CreateRotationY(0.3f));
      Pose p2 = new Pose(new Vector3(1, 2, 3), MathHelper.CreateRotationY(0.3f));

      Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());

      p1 = new Pose(new Vector3(1, 2, 3), MathHelper.CreateRotationY(0.3f));
      p2 = new Pose(new Vector3(2, 1, 3), MathHelper.CreateRotationY(0.3f));
      Assert.AreNotEqual(p1.GetHashCode(), p2.GetHashCode());

      // Too bad two rotation matrices that differ only by the sign of the angle
      // (+/- angle with same axis) have the same hashcodes. See KB -> .NET --> GetHashCode
      //p1 = new Pose(new Vector3(1, 2, 3), MathHelper.CreateRotationY(0.3f));
      //p2 = new Pose(new Vector3(1, 2, 3), MathHelper.CreateRotationY(-0.3f));
      //Assert.AreNotEqual(p1.GetHashCode(), p2.GetHashCode());
    }


    [Test]
    public void Multiply()
    {
      Pose p1 = new Pose(new Vector3(1, 2, 3), MathHelper.CreateRotationY(0.3f));
      Pose p2 = new Pose(new Vector3(-4, 5, -6), MathHelper.CreateRotationZ(-0.1f));

      Assert.IsTrue(MathHelper.AreNumericallyEqual(
                      p1.ToMatrix44F() * p2.ToMatrix44F() * new Vector4(1, 2, 3, 1),
                      Pose.Multiply(Pose.Multiply(p1, p2), new Vector4(1, 2, 3, 1))));
    }


    [Test]
    public void MultiplyOperator()
    {
      Pose p1 = new Pose(new Vector3(1, 2, 3), MathHelper.CreateRotationY(0.3f));
      Pose p2 = new Pose(new Vector3(-4, 5, -6), MathHelper.CreateRotationZ(-0.1f));

      Assert.IsTrue(MathHelper.AreNumericallyEqual(
                      p1.ToMatrix44F() * p2.ToMatrix44F() * new Vector4(1, 2, 3, 1),
                      p1 * p2 * new Vector4(1, 2, 3, 1)));
    }


    [Test]
    public void Interpolate()
    {
      Pose p1 = new Pose(new Vector3(1, 2, 3), MathHelper.CreateRotationY(0.3f));
      Pose p2 = new Pose(new Vector3(-4, 5, -6), MathHelper.CreateRotationZ(-0.1f));

      AssertExt.AreNumericallyEqual(p1.Position, Pose.Interpolate(p1, p2, 0).Position);
      AssertExt.AreNumericallyEqual(p1.Orientation, Pose.Interpolate(p1, p2, 0).Orientation);

      AssertExt.AreNumericallyEqual(p2.Position, Pose.Interpolate(p1, p2, 1).Position);
      AssertExt.AreNumericallyEqual(p2.Orientation, Pose.Interpolate(p1, p2, 1).Orientation);

      AssertExt.AreNumericallyEqual(InterpolationHelper.Lerp(p1.Position, p2.Position, 0.3f), Pose.Interpolate(p1, p2, 0.3f).Position);
      Assert.IsTrue(
        MathHelper.AreNumericallyEqual(
          InterpolationHelper.Lerp(MathHelper.CreateRotation(p1.Orientation), MathHelper.CreateRotation(p2.Orientation), 0.3f),
          MathHelper.CreateRotation(Pose.Interpolate(p1, p2, 0.3f).Orientation)));
    }


    [Test]
    public void AreNumericallyEqual()
    {
      var a = new Pose(new Vector3(1, 2, 3), new Matrix33F(1, 2, 3, 4, 5, 6, 7, 8, 9));
      var b = a;

      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a));

      b = AddToAllComponents(a, Numeric.EpsilonF / 10);
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));

      b = AddToAllComponents(a, Numeric.EpsilonF * 10);
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF * 100));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF * 100));

      b = a;
      b.Position.X -= 0.001f;
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, 0.01f));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, 0.01f));

      b = a;
      b.Position.Y -= 0.001f;
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, 0.01f));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, 0.01f));

      b = a;
      b.Position.Z -= 0.001f;
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, 0.01f));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, 0.01f));

      b = a;
      b.Orientation.M00 -= 0.001f;
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, 0.01f));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, 0.01f));

      b = a;
      b.Orientation.M01 -= 0.001f;
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, 0.01f));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, 0.01f));

      b = a;
      b.Orientation.M02 -= 0.001f;
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, 0.01f));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, 0.01f));

      b = a;
      b.Orientation.M10 -= 0.001f;
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, 0.01f));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, 0.01f));

      b = a;
      b.Orientation.M11 -= 0.001f;
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, 0.01f));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, 0.01f));

      b = a;
      b.Orientation.M12 -= 0.001f;
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, 0.01f));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, 0.01f));

      b = a;
      b.Orientation.M20 -= 0.001f;
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, 0.01f));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, 0.01f));

      b = a;
      b.Orientation.M21 -= 0.001f;
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, 0.01f));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, 0.01f));

      b = a;
      b.Orientation.M22 -= 0.001f;
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(a, b, Numeric.EpsilonF));
      Assert.AreEqual(false, Pose.AreNumericallyEqual(b, a, Numeric.EpsilonF));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(a, b, 0.01f));
      Assert.AreEqual(true, Pose.AreNumericallyEqual(b, a, 0.01f));
    }


    private Pose AddToAllComponents(Pose pose, float epsilon)
    {
      var result = pose;
      result.Position.X += epsilon;
      result.Position.Y += epsilon;
      result.Position.Z += epsilon;

      result.Orientation.M00 += epsilon;
      result.Orientation.M01 += epsilon;
      result.Orientation.M02 += epsilon;
      result.Orientation.M10 += epsilon;
      result.Orientation.M11 += epsilon;
      result.Orientation.M12 += epsilon;
      result.Orientation.M20 += epsilon;
      result.Orientation.M21 += epsilon;
      result.Orientation.M22 += epsilon;

      return result;
    }


    [Test]
    public void SerializationXml()
    {
      Pose pose1 = new Pose(new Vector3(1, 2, 3), new Matrix33F(4, 5, 6, 7, 8, 9, 10, 11, 12));
      Pose pose2;

      string fileName = "SerializationPose.xml";

      if (File.Exists(fileName))
        File.Delete(fileName);

      var serializer = new DataContractSerializer(typeof(Pose));
      using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
      using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8))
        serializer.WriteObject(writer, pose1);

      using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      using (var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas()))
        pose2 = (Pose)serializer.ReadObject(reader);

      Assert.AreEqual(pose1, pose2);
    }


    [Test]
    public void SerializationJson()
    {
      Pose pose1 = new Pose(new Vector3(1, 2, 3), new Matrix33F(4, 5, 6, 7, 8, 9, 10, 11, 12));
      Pose pose2;

      string fileName = "SerializationPose.json";

      if (File.Exists(fileName))
        File.Delete(fileName);

      var serializer = new DataContractJsonSerializer(typeof(Pose));
      using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
        serializer.WriteObject(stream, pose1);

      using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        pose2 = (Pose)serializer.ReadObject(stream);

      Assert.AreEqual(pose1, pose2);
    }
  }
}
