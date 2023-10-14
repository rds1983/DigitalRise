using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;


namespace DigitalRise.Mathematics.Algebra.Tests
{
  [TestFixture]
  public class Vector2Test
  {
    [Test]
    public void Constructors()
    {
      Vector2 v = new Vector2();
      Assert.AreEqual(0.0, v.X);
      Assert.AreEqual(0.0, v.Y);

      v = new Vector2(2.3f);
      Assert.AreEqual(2.3f, v.X);
      Assert.AreEqual(2.3f, v.Y);

      v = new Vector2(1.0f, 2.0f);
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(2.0f, v.Y);
    }


    [Test]
    public void Properties()
    {
      Vector2 v = new Vector2();
      v.X = 1.0f;
      v.Y = 2.0f;
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(2.0f, v.Y);
      Assert.AreEqual(new Vector2(1.0f, 2.0f), v);
    }


    [Test]
    public void HashCode()
    {
      Vector2 v = new Vector2(1.0f, 2.0f);
      Assert.AreNotEqual(Vector2.One.GetHashCode(), v.GetHashCode());
    }


    [Test]
    public void EqualityOperators()
    {
      Vector2 a = new Vector2(1.0f, 2.0f);
      Vector2 b = new Vector2(1.0f, 2.0f);
      Vector2 c = new Vector2(-1.0f, 2.0f);
      Vector2 d = new Vector2(1.0f, -2.0f);

      Assert.IsTrue(a == b);
      Assert.IsFalse(a == c);
      Assert.IsFalse(a == d);
      Assert.IsFalse(a != b);
      Assert.IsTrue(a != c);
      Assert.IsTrue(a != d);
    }


    [Test]
    public void ComparisonOperators()
    {
      Vector2 a = new Vector2(1.0f, 1.0f);
      Vector2 b = new Vector2(0.5f, 0.5f);
      Vector2 c = new Vector2(1.0f, 0.5f);
      Vector2 d = new Vector2(0.5f, 1.0f);

      Assert.IsTrue(a.IsGreaterThen(b));
      Assert.IsFalse(a.IsGreaterThen(c));
      Assert.IsFalse(a.IsGreaterThen(d));

      Assert.IsTrue(b.IsLessThen(a));
      Assert.IsFalse(c.IsLessThen(a));
      Assert.IsFalse(d.IsLessThen(a));

      Assert.IsTrue(a.IsGreaterOrEqual(b));
      Assert.IsTrue(a.IsGreaterOrEqual(c));
      Assert.IsTrue(a.IsGreaterOrEqual(d));

      Assert.IsFalse(b.IsGreaterOrEqual(a));
      Assert.IsFalse(b.IsGreaterOrEqual(c));
      Assert.IsFalse(b.IsGreaterOrEqual(d));

      Assert.IsTrue(b.IsLessOrEqual(a));
      Assert.IsTrue(c.IsLessOrEqual(a));
      Assert.IsTrue(d.IsLessOrEqual(a));

      Assert.IsFalse(a.IsLessOrEqual(b));
      Assert.IsFalse(c.IsLessOrEqual(b));
      Assert.IsFalse(d.IsLessOrEqual(b));
    }


    [Test]
    public void AreEqual()
    {
      float originalEpsilon = Numeric.EpsilonF;
      Numeric.EpsilonF = 1e-8f;

      Vector2 u = new Vector2(1.0f, 2.0f);
      Vector2 v = new Vector2(1.000001f, 2.000001f);
      Vector2 w = new Vector2(1.00000001f, 2.00000001f);

      AssertExt.AreNumericallyEqual(u, u);
      Assert.IsFalse(MathHelper.AreNumericallyEqual(u, v));
      AssertExt.AreNumericallyEqual(u, w);

      Numeric.EpsilonF = originalEpsilon;
    }


    [Test]
    public void TestEquals()
    {
      Vector2 v0 = new Vector2(678.0f, 234.8f);
      Vector2 v1 = new Vector2(678.0f, 234.8f);
      Vector2 v2 = new Vector2(67.0f, 234.8f);
      Vector2 v3 = new Vector2(678.0f, 24.8f);
      Assert.IsTrue(v0.Equals(v0));
      Assert.IsTrue(v0.Equals(v1));
      Assert.IsFalse(v0.Equals(v2));
      Assert.IsFalse(v0.Equals(v3));
      Assert.IsFalse(v0.Equals(v0.ToString()));
    }


    [Test]
    public void AdditionOperator()
    {
      Vector2 a = new Vector2(1.0f, 2.0f);
      Vector2 b = new Vector2(2.0f, 3.0f);
      Vector2 c = a + b;
      Assert.AreEqual(new Vector2(3.0f, 5.0f), c);
    }


    [Test]
    public void Addition()
    {
      Vector2 a = new Vector2(1.0f, 2.0f);
      Vector2 b = new Vector2(2.0f, 3.0f);
      Vector2 c = Vector2.Add(a, b);
      Assert.AreEqual(new Vector2(3.0f, 5.0f), c);
    }


    [Test]
    public void SubtractionOperator()
    {
      Vector2 a = new Vector2(1.0f, 2.0f);
      Vector2 b = new Vector2(10.0f, -10.0f);
      Vector2 c = a - b;
      Assert.AreEqual(new Vector2(-9.0f, 12.0f), c);
    }


    [Test]
    public void Subtraction()
    {
      Vector2 a = new Vector2(1.0f, 2.0f);
      Vector2 b = new Vector2(10.0f, -10.0f);
      Vector2 c = Vector2.Subtract(a, b);
      Assert.AreEqual(new Vector2(-9.0f, 12.0f), c);
    }


    [Test]
    public void MultiplicationOperator()
    {
      float x = 23.4f;
      float y = -11.0f;
      float s = 13.3f;

      Vector2 v = new Vector2(x, y);

      Vector2 u = v * s;
      Assert.AreEqual(x * s, u.X);
      Assert.AreEqual(y * s, u.Y);

      u = s * v;
      Assert.AreEqual(x * s, u.X);
      Assert.AreEqual(y * s, u.Y);
    }


    [Test]
    public void Multiplication()
    {
      float x = 23.4f;
      float y = -11.0f;
      float s = 13.3f;

      Vector2 v = new Vector2(x, y);

      Vector2 u = s * v;
      Assert.AreEqual(x * s, u.X);
      Assert.AreEqual(y * s, u.Y);
    }


    [Test]
    public void MultiplicationOperatorVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;

      float x2 = 34.0f;
      float y2 = 1.2f;

      Vector2 v = new Vector2(x1, y1);
      Vector2 w = new Vector2(x2, y2);

      Assert.AreEqual(new Vector2(x1 * x2, y1 * y2), v * w);
    }


    [Test]
    public void MultiplicationVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;

      float x2 = 34.0f;
      float y2 = 1.2f;

      Vector2 v = new Vector2(x1, y1);
      Vector2 w = new Vector2(x2, y2);

      Assert.AreEqual(new Vector2(x1 * x2, y1 * y2), Vector2.Multiply(v, w));
    }


    [Test]
    public void DivisionOperator()
    {
      float x = 23.4f;
      float y = -11.0f;
      float s = 13.3f;

      Vector2 v = new Vector2(x, y);
      Vector2 u = v / s;
      AssertExt.AreNumericallyEqual(x / s, u.X);
      AssertExt.AreNumericallyEqual(y / s, u.Y);
    }


    [Test]
    public void Division()
    {
      float x = 23.4f;
      float y = -11.0f;
      float s = 13.3f;

      Vector2 v = new Vector2(x, y);
      Vector2 u = Vector2.Divide(v, s);
      AssertExt.AreNumericallyEqual(x / s, u.X);
      AssertExt.AreNumericallyEqual(y / s, u.Y);
    }


    [Test]
    public void DivisionVectorOperatorVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;

      float x2 = 34.0f;
      float y2 = 1.2f;

      Vector2 v = new Vector2(x1, y1);
      Vector2 w = new Vector2(x2, y2);

      Assert.AreEqual(new Vector2(x1 / x2, y1 / y2), v / w);
    }


    [Test]
    public void DivisionVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;

      float x2 = 34.0f;
      float y2 = 1.2f;

      Vector2 v = new Vector2(x1, y1);
      Vector2 w = new Vector2(x2, y2);

      Assert.AreEqual(new Vector2(x1 / x2, y1 / y2), Vector2.Divide(v, w));
    }


    [Test]
    public void NegationOperator()
    {
      Vector2 a = new Vector2(1.0f, 2.0f);
      Assert.AreEqual(new Vector2(-1.0f, -2.0f), -a);
    }


    [Test]
    public void Negation()
    {
      Vector2 a = new Vector2(1.0f, 2.0f);
      Assert.AreEqual(new Vector2(-1.0f, -2.0f), Vector2.Negate(a));
    }


    [Test]
    public void Constants()
    {
      Assert.AreEqual(0.0, Vector2.Zero.X);
      Assert.AreEqual(0.0, Vector2.Zero.Y);

      Assert.AreEqual(1.0, Vector2.One.X);
      Assert.AreEqual(1.0, Vector2.One.Y);

      Assert.AreEqual(1.0, Vector2.UnitX.X);
      Assert.AreEqual(0.0, Vector2.UnitX.Y);

      Assert.AreEqual(0.0, Vector2.UnitY.X);
      Assert.AreEqual(1.0, Vector2.UnitY.Y);
    }


    [Test]
    public void IndexerRead()
    {
      Vector2 v = new Vector2(1.0f, -10e10f);
      Assert.AreEqual(1.0f, v.GetComponentByIndex(0));
      Assert.AreEqual(-10e10f, v.GetComponentByIndex(1));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerReadException()
    {
      Vector2 v = new Vector2(1.0f, -10e10f);
      Assert.AreEqual(1.0, v.GetComponentByIndex(-1));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerReadException2()
    {
      Vector2 v = new Vector2(1.0f, -10e10f);
      Assert.AreEqual(1.0f, v.GetComponentByIndex(2));
    }


    [Test]
    public void IndexerWrite()
    {
      Vector2 v = Vector2.Zero;
      v.SetComponentByIndex(0, 1.0f);
      v.SetComponentByIndex(1, -10e10f);
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(-10e10f, v.Y);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerWriteException()
    {
      Vector2 v = new Vector2(1.0f, -10e10f);
      v.SetComponentByIndex(-1, 0.0f);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerWriteException2()
    {
      Vector2 v = new Vector2(1.0f, -10e10f);
      v.SetComponentByIndex(2, 0.0f);
    }


    [Test]
    public void IsNaN()
    {
      const int numberOfRows = 2;
      Assert.IsFalse(new Vector2().IsNaN());

      for (int i = 0; i < numberOfRows; i++)
      {
        Vector2 v = new Vector2();
        v.SetComponentByIndex(i, float.NaN);
        Assert.IsTrue(v.IsNaN());
      }
    }


    [Test]
    public void Length()
    {
      Assert.AreEqual(1.0, Vector2.UnitX.Length());
      Assert.AreEqual(1.0, Vector2.UnitY.Length());

      float x = -1.9f;
      float y = 2.1f;
      float length = (float)Math.Sqrt(x * x + y * y);
      Vector2 v = new Vector2(x, y);
      Assert.AreEqual(length, v.Length());
    }


    [Test]
    public void LengthSquared()
    {
      Assert.AreEqual(1.0, Vector2.UnitX.LengthSquared());
      Assert.AreEqual(1.0, Vector2.UnitY.LengthSquared());

      float x = -1.9f;
      float y = 2.1f;
      float length = x * x + y * y;
      Vector2 v = new Vector2(x, y);
      Assert.AreEqual(length, v.LengthSquared());
    }


    [Test]
    public void TryNormalize()
    {
      Vector2 v = Vector2.Zero;
      bool normalized = v.TryNormalize();
      Assert.IsFalse(normalized);

      v = new Vector2(1, 2);
      normalized = v.TryNormalize();
      Assert.IsTrue(normalized);
      Assert.AreEqual(new Vector2(1, 2).Normalized(), v);

      v = new Vector2(0, -1);
      normalized = v.TryNormalize();
      Assert.IsTrue(normalized);
      Assert.AreEqual(new Vector2(0, -1).Normalized(), v);
    }


    [Test]
    public void DotProduct()
    {
      // 0�
      Assert.AreEqual(1.0, Vector2.Dot(Vector2.UnitX, Vector2.UnitX));
      Assert.AreEqual(1.0, Vector2.Dot(Vector2.UnitY, Vector2.UnitY));

      // 180�
      Assert.AreEqual(-1.0, Vector2.Dot(Vector2.UnitX, -Vector2.UnitX));
      Assert.AreEqual(-1.0, Vector2.Dot(Vector2.UnitY, -Vector2.UnitY));

      // 90�
      Assert.AreEqual(0.0, Vector2.Dot(Vector2.UnitX, Vector2.UnitY));

      // 45�
      float angle = (float)Math.Acos(Vector2.Dot(new Vector2(1f, 1f).Normalized(), Vector2.UnitX));
      AssertExt.AreNumericallyEqual(MathHelper.ToRadians(45), angle);
      angle = (float)Math.Acos(Vector2.Dot(new Vector2(1f, 1f).Normalized(), Vector2.UnitY));
      AssertExt.AreNumericallyEqual(MathHelper.ToRadians(45), angle);
    }


    [Test]
    public void Min()
    {
      Vector2 v1 = new Vector2(1.0f, 2.0f);
      Vector2 v2 = new Vector2(-1.0f, 2.0f);
      Vector2 min = Vector2.Min(v1, v2);
      Assert.AreEqual(new Vector2(-1.0f, 2.0f), min);
    }


    [Test]
    public void Max()
    {
      Vector2 v1 = new Vector2(1.0f, 2.0f);
      Vector2 v2 = new Vector2(-1.0f, 2.1f);
      Vector2 max = Vector2.Max(v1, v2);
      Assert.AreEqual(new Vector2(1.0f, 2.1f), max);
    }


    [Test]
    public void SerializationXml()
    {
      Vector2 v1 = new Vector2(1.0f, 2.0f);
      Vector2 v2;
      string fileName = "SerializationVector2.xml";

      if (File.Exists(fileName))
        File.Delete(fileName);

      XmlSerializer serializer = new XmlSerializer(typeof(Vector2));
      StreamWriter writer = new StreamWriter(fileName);
      serializer.Serialize(writer, v1);
      writer.Close();

      serializer = new XmlSerializer(typeof(Vector2));
      FileStream fileStream = new FileStream(fileName, FileMode.Open);
      v2 = (Vector2)serializer.Deserialize(fileStream);
      Assert.AreEqual(v1, v2);
    }


    [Test]
    public void SerializationXml2()
    {
      Vector2 v1 = new Vector2(0.1f, -0.2f);
      Vector2 v2;

      string fileName = "SerializationVector2_DataContractSerializer.xml";

      if (File.Exists(fileName))
        File.Delete(fileName);

      var serializer = new DataContractSerializer(typeof(Vector2));
      using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
      using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8))
        serializer.WriteObject(writer, v1);

      using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      using (var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas()))
        v2 = (Vector2)serializer.ReadObject(reader);

      Assert.AreEqual(v1, v2);
    }


    [Test]
    public void SerializationJson()
    {
      Vector2 v1 = new Vector2(0.1f, -0.2f);
      Vector2 v2;

      string fileName = "SerializationVector2.json";

      if (File.Exists(fileName))
        File.Delete(fileName);

      var serializer = new DataContractJsonSerializer(typeof(Vector2));
      using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
        serializer.WriteObject(stream, v1);

      using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        v2 = (Vector2)serializer.ReadObject(stream);

      Assert.AreEqual(v1, v2);
    }
  }
}