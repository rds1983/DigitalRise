using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using DigitalRise.Mathematics;
using NUnit.Utils;


namespace DigitalRise.Mathematics.Algebra.Tests
{
  [TestFixture]
  public class Vector4Test
  {
    [Test]
    public void Constructors()
    {
      Vector4 v = new Vector4();
      Assert.AreEqual(0.0f, v.X);
      Assert.AreEqual(0.0f, v.Y);
      Assert.AreEqual(0.0f, v.Z);
      Assert.AreEqual(0.0f, v.W);

      v = new Vector4(2.3f);
      Assert.AreEqual(2.3f, v.X);
      Assert.AreEqual(2.3f, v.Y);
      Assert.AreEqual(2.3f, v.Z);
      Assert.AreEqual(2.3f, v.W);

      v = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(2.0f, v.Y);
      Assert.AreEqual(3.0f, v.Z);
      Assert.AreEqual(4.0f, v.W);

      v = new Vector4(new Vector3(1.0f, 2.0f, 3.0f), 4.0f);
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(2.0f, v.Y);
      Assert.AreEqual(3.0f, v.Z);
      Assert.AreEqual(4.0f, v.W);
    }


    [Test]
    public void Properties()
    {
      Vector4 v = new Vector4();
      v.X = 1.0f;
      v.Y = 2.0f;
      v.Z = 3.0f;
      v.W = 4.0f;
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(2.0f, v.Y);
      Assert.AreEqual(3.0f, v.Z);
      Assert.AreEqual(4.0f, v.W);
      Assert.AreEqual(new Vector4(1.0f, 2.0f, 3.0f, 4.0f), v);
    }


    [Test]
    public void HashCode()
    {
      Vector4 v = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Assert.AreNotEqual(Vector4.One.GetHashCode(), v.GetHashCode());
    }


    [Test]
    public void EqualityOperators()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 b = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 c = new Vector4(-1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 d = new Vector4(1.0f, -2.0f, 3.0f, 4.0f);
      Vector4 e = new Vector4(1.0f, 2.0f, -3.0f, 4.0f);
      Vector4 f = new Vector4(1.0f, 2.0f, 3.0f, -4.0f);

      Assert.IsTrue(a == b);
      Assert.IsFalse(a == c);
      Assert.IsFalse(a == d);
      Assert.IsFalse(a == e);
      Assert.IsFalse(a == f);
      Assert.IsFalse(a != b);
      Assert.IsTrue(a != c);
      Assert.IsTrue(a != d);
      Assert.IsTrue(a != e);
      Assert.IsTrue(a != f);
    }


    [Test]
    public void AreEqual()
    {
      float originalEpsilon = Numeric.EpsilonF;
      Numeric.EpsilonF = 1e-8f;

      Vector4 u = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 v = new Vector4(1.000001f, 2.000001f, 3.000001f, 4.000001f);
      Vector4 w = new Vector4(1.00000001f, 2.00000001f, 3.00000001f, 4.00000001f);

      AssertExt.AreNumericallyEqual(u, u);
      Assert.IsFalse(MathHelper.AreNumericallyEqual(u, v));
      AssertExt.AreNumericallyEqual(u, w);

      Numeric.EpsilonF = originalEpsilon;
    }


    [Test]
    public void TestEquals()
    {
      Vector4 v0 = new Vector4(678.0f, 234.8f, -123.987f, 4.0f);
      Vector4 v1 = new Vector4(678.0f, 234.8f, -123.987f, 4.0f);
      Vector4 v2 = new Vector4(67.0f, 234.8f, -123.987f, 4.0f);
      Vector4 v3 = new Vector4(678.0f, 24.8f, -123.987f, 4.0f);
      Vector4 v4 = new Vector4(678.0f, 234.8f, 123.987f, 4.0f);
      Vector4 v5 = new Vector4(678.0f, 234.8f, 123.987f, 4.1f);
      Assert.IsTrue(v0.Equals(v0));
      Assert.IsTrue(v0.Equals(v1));
      Assert.IsFalse(v0.Equals(v2));
      Assert.IsFalse(v0.Equals(v3));
      Assert.IsFalse(v0.Equals(v4));
      Assert.IsFalse(v0.Equals(v5));
      Assert.IsFalse(v0.Equals(v0.ToString()));
    }


    [Test]
    public void AdditionOperator()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 b = new Vector4(2.0f, 3.0f, 4.0f, 5.0f);
      Vector4 c = a + b;
      Assert.AreEqual(new Vector4(3.0f, 5.0f, 7.0f, 9.0f), c);
    }


    [Test]
    public void Addition()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 b = new Vector4(2.0f, 3.0f, 4.0f, 5.0f);
      Vector4 c = Vector4.Add(a, b);
      Assert.AreEqual(new Vector4(3.0f, 5.0f, 7.0f, 9.0f), c);
    }


    [Test]
    public void SubtractionOperator()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 b = new Vector4(10.0f, -10.0f, 0.5f, 2.5f);
      Vector4 c = a - b;
      Assert.AreEqual(new Vector4(-9.0f, 12.0f, 2.5f, 1.5f), c);
    }


    [Test]
    public void Subtraction()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 b = new Vector4(10.0f, -10.0f, 0.5f, 2.5f);
      Vector4 c = Vector4.Subtract(a, b);
      Assert.AreEqual(new Vector4(-9.0f, 12.0f, 2.5f, 1.5f), c);
    }


    [Test]
    public void MultiplicationOperator()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 6.0f;
      float w = 0.4f;
      float s = 13.3f;

      Vector4 v = new Vector4(x, y, z, w);

      Vector4 u = v * s;
      Assert.AreEqual(x * s, u.X);
      Assert.AreEqual(y * s, u.Y);
      Assert.AreEqual(z * s, u.Z);
      Assert.AreEqual(w * s, u.W);

      u = s * v;
      Assert.AreEqual(x * s, u.X);
      Assert.AreEqual(y * s, u.Y);
      Assert.AreEqual(z * s, u.Z);
      Assert.AreEqual(w * s, u.W);
    }


    [Test]
    public void Multiplication()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 6.0f;
      float w = 0.4f;
      float s = 13.3f;

      Vector4 v = new Vector4(x, y, z, w);

      Vector4 u = s * v;
      Assert.AreEqual(x * s, u.X);
      Assert.AreEqual(y * s, u.Y);
      Assert.AreEqual(z * s, u.Z);
      Assert.AreEqual(w * s, u.W);
    }


    [Test]
    public void MultiplicationOperatorVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;
      float z1 = 6.0f;
      float w1 = 0.2f;

      float x2 = 34.0f;
      float y2 = 1.2f;
      float z2 = -6.0f;
      float w2 = -0.2f;

      Vector4 v = new Vector4(x1, y1, z1, w1);
      Vector4 w = new Vector4(x2, y2, z2, w2);

      Assert.AreEqual(new Vector4(x1 * x2, y1 * y2, z1 * z2, w1 * w2), v * w);
    }


    [Test]
    public void MultiplicationVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;
      float z1 = 6.0f;
      float w1 = 0.2f;

      float x2 = 34.0f;
      float y2 = 1.2f;
      float z2 = -6.0f;
      float w2 = -0.2f;

      Vector4 v = new Vector4(x1, y1, z1, w1);
      Vector4 w = new Vector4(x2, y2, z2, w2);

      Assert.AreEqual(new Vector4(x1 * x2, y1 * y2, z1 * z2, w1 * w2), Vector4.Multiply(v, w));
    }


    [Test]
    public void DivisionOperator()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 7.0f;
      float w = 1.9f;
      float s = 13.3f;

      Vector4 v = new Vector4(x, y, z, w);
      Vector4 u = v / s;
      AssertExt.AreNumericallyEqual(x / s, u.X);
      AssertExt.AreNumericallyEqual(y / s, u.Y);
      AssertExt.AreNumericallyEqual(z / s, u.Z);
      AssertExt.AreNumericallyEqual(w / s, u.W);
    }


    [Test]
    public void Division()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 7.0f;
      float w = 1.9f;
      float s = 13.3f;

      Vector4 v = new Vector4(x, y, z, w);
      Vector4 u = Vector4.Divide(v, s);
      AssertExt.AreNumericallyEqual(x / s, u.X);
      AssertExt.AreNumericallyEqual(y / s, u.Y);
      AssertExt.AreNumericallyEqual(z / s, u.Z);
      AssertExt.AreNumericallyEqual(w / s, u.W);
    }


    [Test]
    public void DivisionVectorOperatorVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;
      float z1 = 6.0f;
      float w1 = 0.2f;

      float x2 = 34.0f;
      float y2 = 1.2f;
      float z2 = -6.0f;
      float w2 = -0.2f;

      Vector4 v = new Vector4(x1, y1, z1, w1);
      Vector4 w = new Vector4(x2, y2, z2, w2);

      Assert.AreEqual(new Vector4(x1 / x2, y1 / y2, z1 / z2, w1 / w2), v / w);
    }


    [Test]
    public void DivisionVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;
      float z1 = 6.0f;
      float w1 = 0.2f;

      float x2 = 34.0f;
      float y2 = 1.2f;
      float z2 = -6.0f;
      float w2 = -0.2f;

      Vector4 v = new Vector4(x1, y1, z1, w1);
      Vector4 w = new Vector4(x2, y2, z2, w2);

      Assert.AreEqual(new Vector4(x1 / x2, y1 / y2, z1 / z2, w1 / w2), Vector4.Divide(v, w));
    }


    [Test]
    public void NegationOperator()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Assert.AreEqual(new Vector4(-1.0f, -2.0f, -3.0f, -4.0f), -a);
    }


    [Test]
    public void Negation()
    {
      Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Assert.AreEqual(new Vector4(-1.0f, -2.0f, -3.0f, -4.0f), Vector4.Negate(a));
    }


    [Test]
    public void Constants()
    {
      Assert.AreEqual(0.0f, Vector4.Zero.X);
      Assert.AreEqual(0.0f, Vector4.Zero.Y);
      Assert.AreEqual(0.0f, Vector4.Zero.Z);
      Assert.AreEqual(0.0f, Vector4.Zero.W);

      Assert.AreEqual(1.0f, Vector4.One.X);
      Assert.AreEqual(1.0f, Vector4.One.Y);
      Assert.AreEqual(1.0f, Vector4.One.Z);
      Assert.AreEqual(1.0f, Vector4.One.W);

      Assert.AreEqual(1.0f, Vector4.UnitX.X);
      Assert.AreEqual(0.0f, Vector4.UnitX.Y);
      Assert.AreEqual(0.0f, Vector4.UnitX.Z);
      Assert.AreEqual(0.0f, Vector4.UnitX.W);

      Assert.AreEqual(0.0f, Vector4.UnitY.X);
      Assert.AreEqual(1.0f, Vector4.UnitY.Y);
      Assert.AreEqual(0.0f, Vector4.UnitY.Z);
      Assert.AreEqual(0.0f, Vector4.UnitX.W);

      Assert.AreEqual(0.0f, Vector4.UnitZ.X);
      Assert.AreEqual(0.0f, Vector4.UnitZ.Y);
      Assert.AreEqual(1.0f, Vector4.UnitZ.Z);
      Assert.AreEqual(0.0f, Vector4.UnitX.W);

      Assert.AreEqual(0.0f, Vector4.UnitW.X);
      Assert.AreEqual(0.0f, Vector4.UnitW.Y);
      Assert.AreEqual(0.0f, Vector4.UnitW.Z);
      Assert.AreEqual(1.0f, Vector4.UnitW.W);
    }


    [Test]
    public void IndexerRead()
    {
      Vector4 v = new Vector4(1.0f, -10e10f, 0.0f, 0.3f);
      Assert.AreEqual(1.0f, v.GetComponentByIndex(0));
      Assert.AreEqual(-10e10f, v.GetComponentByIndex(1));
      Assert.AreEqual(0.0f, v.GetComponentByIndex(2));
      Assert.AreEqual(0.3f, v.GetComponentByIndex(3));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerReadException()
    {
      Vector4 v = new Vector4(1.0f, -10e10f, 0.0f, 0.3f);
      Assert.AreEqual(1.0f, v.GetComponentByIndex(-1));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerReadException2()
    {
      Vector4 v = new Vector4(1.0f, -10e10f, 0.0f, 0.3f);
      Assert.AreEqual(1.0f, v.GetComponentByIndex(4));
    }


    [Test]
    public void IndexerWrite()
    {
      Vector4 v = Vector4.Zero;
      v.SetComponentByIndex(0, 1.0f);
      v.SetComponentByIndex(1, -10e10f);
      v.SetComponentByIndex(2, 0.1f);
      v.SetComponentByIndex(3, 0.3f);
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(-10e10f, v.Y);
      Assert.AreEqual(0.1f, v.Z);
      Assert.AreEqual(0.3f, v.W);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerWriteException()
    {
      Vector4 v = new Vector4(1.0f, -10e10f, 0.0f, 0.3f);
      v.SetComponentByIndex(-1, 0.0f);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerWriteException2()
    {
      Vector4 v = new Vector4(1.0f, -10e10f, 0.0f, 0.3f);
      v.SetComponentByIndex(4, 0.0f);
    }


    [Test]
    public void TestIsZero()
    {
      Vector4 nonZero = new Vector4(0.001f, 0.001f, 0.0f, 0.001f);
      Assert.IsFalse(MathHelper.AreNumericallyEqual(nonZero, Vector4.Zero));

      Vector4 zero = new Vector4(0.0000001f, 0.0000001f, 0.0f, 0.0000001f);
      AssertExt.AreNumericallyEqual(zero, Vector4.Zero);
    }


    [Test]
    public void Length()
    {
      Assert.AreEqual(1.0f, Vector4.UnitX.Length());
      Assert.AreEqual(1.0f, Vector4.UnitY.Length());
      Assert.AreEqual(1.0f, Vector4.UnitZ.Length());
      Assert.AreEqual(1.0f, Vector4.UnitW.Length());

      float x = -1.9f;
      float y = 2.1f;
      float z = 10.0f;
      float w = 1.0f;
      float length = (float)Math.Sqrt(x * x + y * y + z * z + w * w);
      Vector4 v = new Vector4(x, y, z, w);
      Assert.AreEqual(length, v.Length());
    }


    [Test]
    public void LengthSquared()
    {
      Assert.AreEqual(1.0f, Vector4.UnitX.LengthSquared());
      Assert.AreEqual(1.0f, Vector4.UnitY.LengthSquared());
      Assert.AreEqual(1.0f, Vector4.UnitZ.LengthSquared());
      Assert.AreEqual(1.0f, Vector4.UnitW.LengthSquared());

      float x = -1.9f;
      float y = 2.1f;
      float z = 10.0f;
      float w = 1.0f;
      float length = x * x + y * y + z * z + w * w;
      Vector4 v = new Vector4(x, y, z, w);
      Assert.AreEqual(length, v.LengthSquared());
    }


    [Test]
    public void DotProduct()
    {
      // 0�
      Assert.AreEqual(1.0f, Vector4.Dot(Vector4.UnitX, Vector4.UnitX));
      Assert.AreEqual(1.0f, Vector4.Dot(Vector4.UnitY, Vector4.UnitY));
      Assert.AreEqual(1.0f, Vector4.Dot(Vector4.UnitZ, Vector4.UnitZ));
      Assert.AreEqual(1.0f, Vector4.Dot(Vector4.UnitW, Vector4.UnitW));

      // 180�
      Assert.AreEqual(-1.0f, Vector4.Dot(Vector4.UnitX, -Vector4.UnitX));
      Assert.AreEqual(-1.0f, Vector4.Dot(Vector4.UnitY, -Vector4.UnitY));
      Assert.AreEqual(-1.0f, Vector4.Dot(Vector4.UnitZ, -Vector4.UnitZ));
      Assert.AreEqual(-1.0f, Vector4.Dot(Vector4.UnitW, -Vector4.UnitW));

      // 90�
      Assert.AreEqual(0.0f, Vector4.Dot(Vector4.UnitX, Vector4.UnitY));
      Assert.AreEqual(0.0f, Vector4.Dot(Vector4.UnitY, Vector4.UnitZ));
      Assert.AreEqual(0.0f, Vector4.Dot(Vector4.UnitZ, Vector4.UnitW));
      Assert.AreEqual(0.0f, Vector4.Dot(Vector4.UnitW, Vector4.UnitX));

      // 45�
      float angle = (float)Math.Acos(Vector4.Dot(new Vector4(1, 1, 0, 0).Normalized(), Vector4.UnitX));
      AssertExt.AreNumericallyEqual(MathHelper.ToRadians(45), angle);
      angle = (float)Math.Acos(Vector4.Dot(new Vector4(0, 1, 1, 0).Normalized(), Vector4.UnitY));
      AssertExt.AreNumericallyEqual(MathHelper.ToRadians(45), angle);
      angle = (float)Math.Acos(Vector4.Dot(new Vector4(1, 0, 1, 0).Normalized(), Vector4.UnitZ));
      AssertExt.AreNumericallyEqual(MathHelper.ToRadians(45), angle);
      angle = (float)Math.Acos(Vector4.Dot(new Vector4(1, 0, 0, 1).Normalized(), Vector4.UnitW));
      AssertExt.AreNumericallyEqual(MathHelper.ToRadians(45), angle);
    }


    [Test]
    public void XYZ()
    {
      Vector4 v4 = new Vector4(1.0f, 2.0f, 3.0f, 5.0f);
      Vector3 v3 = v4.XYZ();
      Assert.AreEqual(new Vector3(1.0f, 2.0f, 3.0f), v3);
    }


    [Test]
    public void HomogeneousDivide()
    {
      Vector4 v4 = new Vector4(1.0f, 2.0f, 3.0f, 1.0f);
      Vector3 v3 = MathHelper.HomogeneousDivide(v4);
      Assert.AreEqual(new Vector3(1.0f, 2.0f, 3.0f), v3);

      v4 = new Vector4(1.0f, 2.0f, 3.0f, 10.0f);
      v3 = MathHelper.HomogeneousDivide(v4);
      Assert.AreEqual(new Vector3(1.0f / 10.0f, 2.0f / 10.0f, 3.0f / 10.0f), v3);
    }


    [Test]
    public void Min()
    {
      Vector4 v1 = new Vector4(1.0f, 2.0f, 2.5f, 4.0f);
      Vector4 v2 = new Vector4(-1.0f, 2.0f, 3.0f, -2.0f);
      Vector4 min = Vector4.Min(v1, v2);
      Assert.AreEqual(new Vector4(-1.0f, 2.0f, 2.5f, -2.0f), min);
    }


    [Test]
    public void Max()
    {
      Vector4 v1 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 v2 = new Vector4(-1.0f, 2.1f, 3.0f, 8.0f);
      Vector4 max = Vector4.Max(v1, v2);
      Assert.AreEqual(new Vector4(1.0f, 2.1f, 3.0f, 8.0f), max);
    }


    [Test]
    public void SerializationXml()
    {
      Vector4 v1 = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
      Vector4 v2;
      string fileName = "SerializationVector4.xml";

      if (File.Exists(fileName))
        File.Delete(fileName);

      XmlSerializer serializer = new XmlSerializer(typeof(Vector4));
      StreamWriter writer = new StreamWriter(fileName);
      serializer.Serialize(writer, v1);
      writer.Close();

      serializer = new XmlSerializer(typeof(Vector4));
      FileStream fileStream = new FileStream(fileName, FileMode.Open);
      v2 = (Vector4)serializer.Deserialize(fileStream);
      Assert.AreEqual(v1, v2);
    }


    [Test]
    [Ignore("Binary serialization not supported in PCL version.")]
    public void SerializationBinary()
    {
      Vector4 v1 = new Vector4(0.1f, -0.2f, 2, 40);
      Vector4 v2;
      string fileName = "SerializationVector4.bin";

      if (File.Exists(fileName))
        File.Delete(fileName);

      FileStream fs = new FileStream(fileName, FileMode.Create);

      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize(fs, v1);
      fs.Close();

      fs = new FileStream(fileName, FileMode.Open);
      formatter = new BinaryFormatter();
      v2 = (Vector4)formatter.Deserialize(fs);
      fs.Close();

      Assert.AreEqual(v1, v2);
    }


    [Test]
    public void SerializationXml2()
    {
      Vector4 v1 = new Vector4(0.1f, -0.2f, 2, 40);
      Vector4 v2;

      string fileName = "SerializationVector4_DataContractSerializer.xml";

      if (File.Exists(fileName))
        File.Delete(fileName);

      var serializer = new DataContractSerializer(typeof(Vector4));
      using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
      using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8))
        serializer.WriteObject(writer, v1);

      using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      using (var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas()))
        v2 = (Vector4)serializer.ReadObject(reader);

      Assert.AreEqual(v1, v2);
    }


    [Test]
    public void SerializationJson()
    {
      Vector4 v1 = new Vector4(0.1f, -0.2f, 2, 40);
      Vector4 v2;

      string fileName = "SerializationVector4.json";

      if (File.Exists(fileName))
        File.Delete(fileName);

      var serializer = new DataContractJsonSerializer(typeof(Vector4));
      using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
        serializer.WriteObject(stream, v1);

      using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        v2 = (Vector4)serializer.ReadObject(stream);

      Assert.AreEqual(v1, v2);
    }
  }
}