using System;
using System.Collections.Generic;
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
  public class Vector3Test
  {
    [Test]
    public void Constructors()
    {
      Vector3 v = new Vector3();
      Assert.AreEqual(0.0, v.X);
      Assert.AreEqual(0.0, v.Y);
      Assert.AreEqual(0.0, v.Z);

      v = new Vector3(2.3f);
      Assert.AreEqual(2.3f, v.X);
      Assert.AreEqual(2.3f, v.Y);
      Assert.AreEqual(2.3f, v.Z);

      v = new Vector3(1.0f, 2.0f, 3.0f);
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(2.0f, v.Y);
      Assert.AreEqual(3.0f, v.Z);
    }


    [Test]
    public void Properties()
    {
      Vector3 v = new Vector3();
      v.X = 1.0f;
      v.Y = 2.0f;
      v.Z = 3.0f;
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(2.0f, v.Y);
      Assert.AreEqual(3.0f, v.Z);
      Assert.AreEqual(new Vector3(1.0f, 2.0f, 3.0f), v);
    }


    [Test]
    public void HashCode()
    {
      Vector3 v = new Vector3(1.0f, 2.0f, 3.0f);
      Assert.AreNotEqual(Vector3.One.GetHashCode(), v.GetHashCode());
    }


    [Test]
    public void EqualityOperators()
    {
      Vector3 a = new Vector3(1.0f, 2.0f, 3.0f);
      Vector3 b = new Vector3(1.0f, 2.0f, 3.0f);
      Vector3 c = new Vector3(-1.0f, 2.0f, 3.0f);
      Vector3 d = new Vector3(1.0f, -2.0f, 3.0f);
      Vector3 e = new Vector3(1.0f, 2.0f, -3.0f);

      Assert.IsTrue(a == b);
      Assert.IsFalse(a == c);
      Assert.IsFalse(a == d);
      Assert.IsFalse(a == e);
      Assert.IsFalse(a != b);
      Assert.IsTrue(a != c);
      Assert.IsTrue(a != d);
      Assert.IsTrue(a != e);
    }


    [Test]
    public void ComparisonOperators()
    {
      Vector3 a = new Vector3(1.0f, 1.0f, 1.0f);
      Vector3 b = new Vector3(0.5f, 0.5f, 0.5f);
      Vector3 c = new Vector3(1.0f, 0.5f, 0.5f);
      Vector3 d = new Vector3(0.5f, 1.0f, 0.5f);
      Vector3 e = new Vector3(0.5f, 0.5f, 1.0f);

      Assert.IsTrue(a.IsGreaterThen(b));
      Assert.IsFalse(a.IsGreaterThen(c));
      Assert.IsFalse(a.IsGreaterThen(d));
      Assert.IsFalse(a.IsGreaterThen(e));

      Assert.IsTrue(b.IsLessThen(a));
      Assert.IsFalse(c.IsLessThen(a));
      Assert.IsFalse(d.IsLessThen(a));
      Assert.IsFalse(e.IsLessThen(a));

      Assert.IsTrue(a.IsGreaterOrEqual(b));
      Assert.IsTrue(a.IsGreaterOrEqual(c));
      Assert.IsTrue(a.IsGreaterOrEqual(d));
      Assert.IsTrue(a.IsGreaterOrEqual(e));

      Assert.IsFalse(b.IsGreaterOrEqual(a));
      Assert.IsFalse(b.IsGreaterOrEqual(c));
      Assert.IsFalse(b.IsGreaterOrEqual(d));
      Assert.IsFalse(b.IsGreaterOrEqual(e));

      Assert.IsTrue(b.IsLessOrEqual(a));
      Assert.IsTrue(c.IsLessOrEqual(a));
      Assert.IsTrue(d.IsLessOrEqual(a));
      Assert.IsTrue(e.IsLessOrEqual(a));

      Assert.IsFalse(a.IsLessOrEqual(b));
      Assert.IsFalse(c.IsLessOrEqual(b));
      Assert.IsFalse(d.IsLessOrEqual(b));
      Assert.IsFalse(e.IsLessOrEqual(b));
    }


    [Test]
    public void AreEqual()
    {
      float originalEpsilon = Numeric.EpsilonF;
      Numeric.EpsilonF = 1e-8f;

      Vector3 u = new Vector3(1.0f, 2.0f, 3.0f);
      Vector3 v = new Vector3(1.000001f, 2.000001f, 3.000001f);
      Vector3 w = new Vector3(1.00000001f, 2.00000001f, 3.00000001f);

      AssertExt.AreNumericallyEqual(u, u);
      Assert.IsFalse(MathHelper.AreNumericallyEqual(u, v));
      AssertExt.AreNumericallyEqual(u, w);

      Numeric.EpsilonF = originalEpsilon;
    }


    [Test]
    public void AreEqualWithEpsilon()
    {
      float epsilon = 0.001f;
      Vector3 u = new Vector3(1.0f, 2.0f, 3.0f);
      Vector3 v = new Vector3(1.002f, 2.002f, 3.002f);
      Vector3 w = new Vector3(1.0001f, 2.0001f, 3.0001f);

      AssertExt.AreNumericallyEqual(u, u, epsilon);
      Assert.IsFalse(MathHelper.AreNumericallyEqual(u, v, epsilon));
      AssertExt.AreNumericallyEqual(u, w, epsilon);
    }


    [Test]
    public void IsNumericallyZero()
    {
      float originalEpsilon = Numeric.EpsilonF;
      Numeric.EpsilonF = 1e-8f;

      Vector3 u = new Vector3(0.0f, 0.0f, 0.0f);
      Vector3 v = new Vector3(1e-9f, -1e-9f, 1e-9f);
      Vector3 w = new Vector3(1e-7f, 1e-7f, -1e-7f);

      Assert.IsTrue(u.IsNumericallyZero());
      Assert.IsTrue(v.IsNumericallyZero());
      Assert.IsFalse(w.IsNumericallyZero());

      Numeric.EpsilonF = originalEpsilon;
    }


    [Test]
    public void TestEquals()
    {
      Vector3 v0 = new Vector3(678.0f, 234.8f, -123.987f);
      Vector3 v1 = new Vector3(678.0f, 234.8f, -123.987f);
      Vector3 v2 = new Vector3(67.0f, 234.8f, -123.987f);
      Vector3 v3 = new Vector3(678.0f, 24.8f, -123.987f);
      Vector3 v4 = new Vector3(678.0f, 234.8f, 123.987f);
      Assert.IsTrue(v0.Equals(v0));
      Assert.IsTrue(v0.Equals(v1));
      Assert.IsFalse(v0.Equals(v2));
      Assert.IsFalse(v0.Equals(v3));
      Assert.IsFalse(v0.Equals(v4));
      Assert.IsFalse(v0.Equals(v0.ToString()));
    }


    [Test]
    public void AdditionOperator()
    {
      Vector3 a = new Vector3(1.0f, 2.0f, 3.0f);
      Vector3 b = new Vector3(2.0f, 3.0f, 4.0f);
      Vector3 c = a + b;
      Assert.AreEqual(new Vector3(3.0f, 5.0f, 7.0f), c);
    }


    [Test]
    public void Addition()
    {
      Vector3 a = new Vector3(1.0f, 2.0f, 3.0f);
      Vector3 b = new Vector3(2.0f, 3.0f, 4.0f);
      Vector3 c = Vector3.Add(a, b);
      Assert.AreEqual(new Vector3(3.0f, 5.0f, 7.0f), c);
    }


    [Test]
    public void SubtractionOperator()
    {
      Vector3 a = new Vector3(1.0f, 2.0f, 3.0f);
      Vector3 b = new Vector3(10.0f, -10.0f, 0.5f);
      Vector3 c = a - b;
      Assert.AreEqual(new Vector3(-9.0f, 12.0f, 2.5f), c);
    }


    [Test]
    public void Subtraction()
    {
      Vector3 a = new Vector3(1.0f, 2.0f, 3.0f);
      Vector3 b = new Vector3(10.0f, -10.0f, 0.5f);
      Vector3 c = Vector3.Subtract(a, b);
      Assert.AreEqual(new Vector3(-9.0f, 12.0f, 2.5f), c);
    }


    [Test]
    public void MultiplicationOperator()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 6.0f;
      float s = 13.3f;

      Vector3 v = new Vector3(x, y, z);

      Vector3 u = v * s;
      Assert.AreEqual(x * s, u.X);
      Assert.AreEqual(y * s, u.Y);
      Assert.AreEqual(z * s, u.Z);

      u = s * v;
      Assert.AreEqual(x * s, u.X);
      Assert.AreEqual(y * s, u.Y);
      Assert.AreEqual(z * s, u.Z);
    }


    [Test]
    public void Multiplication()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 6.0f;
      float s = 13.3f;

      Vector3 v = new Vector3(x, y, z);

      Vector3 u = s * v;
      Assert.AreEqual(x * s, u.X);
      Assert.AreEqual(y * s, u.Y);
      Assert.AreEqual(z * s, u.Z);
    }


    [Test]
    public void MultiplicationOperatorVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;
      float z1 = 6.0f;

      float x2 = 34.0f;
      float y2 = 1.2f;
      float z2 = -6.0f;

      Vector3 v = new Vector3(x1, y1, z1);
      Vector3 w = new Vector3(x2, y2, z2);

      Assert.AreEqual(new Vector3(x1 * x2, y1 * y2, z1 * z2), v * w);
    }


    [Test]
    public void MultiplicationVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;
      float z1 = 6.0f;

      float x2 = 34.0f;
      float y2 = 1.2f;
      float z2 = -6.0f;

      Vector3 v = new Vector3(x1, y1, z1);
      Vector3 w = new Vector3(x2, y2, z2);

      Assert.AreEqual(new Vector3(x1 * x2, y1 * y2, z1 * z2), Vector3.Multiply(v, w));
    }


    [Test]
    public void DivisionOperator()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 7.0f;
      float s = 13.3f;

      Vector3 v = new Vector3(x, y, z);
      Vector3 u = v / s;
      AssertExt.AreNumericallyEqual(x / s, u.X);
      AssertExt.AreNumericallyEqual(y / s, u.Y);
      AssertExt.AreNumericallyEqual(z / s, u.Z);
    }


    [Test]
    public void Division()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 7.0f;
      float s = 13.3f;

      Vector3 v = new Vector3(x, y, z);
      Vector3 u = Vector3.Divide(v, s);
      AssertExt.AreNumericallyEqual(x / s, u.X);
      AssertExt.AreNumericallyEqual(y / s, u.Y);
      AssertExt.AreNumericallyEqual(z / s, u.Z);
    }


    [Test]
    public void DivisionVectorOperatorVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;
      float z1 = 6.0f;

      float x2 = 34.0f;
      float y2 = 1.2f;
      float z2 = -6.0f;

      Vector3 v = new Vector3(x1, y1, z1);
      Vector3 w = new Vector3(x2, y2, z2);

      Assert.AreEqual(new Vector3(x1 / x2, y1 / y2, z1 / z2), v / w);
    }


    [Test]
    public void DivisionVector()
    {
      float x1 = 23.4f;
      float y1 = -11.0f;
      float z1 = 6.0f;

      float x2 = 34.0f;
      float y2 = 1.2f;
      float z2 = -6.0f;

      Vector3 v = new Vector3(x1, y1, z1);
      Vector3 w = new Vector3(x2, y2, z2);

      Assert.AreEqual(new Vector3(x1 / x2, y1 / y2, z1 / z2), Vector3.Divide(v, w));
    }


    [Test]
    public void NegationOperator()
    {
      Vector3 a = new Vector3(1.0f, 2.0f, 3.0f);
      Assert.AreEqual(new Vector3(-1.0f, -2.0f, -3.0f), -a);
    }


    [Test]
    public void Negation()
    {
      Vector3 a = new Vector3(1.0f, 2.0f, 3.0f);
      Assert.AreEqual(new Vector3(-1.0f, -2.0f, -3.0f), Vector3.Negate(a));
    }


    [Test]
    public void Constants()
    {
      Assert.AreEqual(0.0, Vector3.Zero.X);
      Assert.AreEqual(0.0, Vector3.Zero.Y);
      Assert.AreEqual(0.0, Vector3.Zero.Z);

      Assert.AreEqual(1.0, Vector3.One.X);
      Assert.AreEqual(1.0, Vector3.One.Y);
      Assert.AreEqual(1.0, Vector3.One.Z);

      Assert.AreEqual(1.0, Vector3.UnitX.X);
      Assert.AreEqual(0.0, Vector3.UnitX.Y);
      Assert.AreEqual(0.0, Vector3.UnitX.Z);

      Assert.AreEqual(0.0, Vector3.UnitY.X);
      Assert.AreEqual(1.0, Vector3.UnitY.Y);
      Assert.AreEqual(0.0, Vector3.UnitY.Z);

      Assert.AreEqual(0.0, Vector3.UnitZ.X);
      Assert.AreEqual(0.0, Vector3.UnitZ.Y);
      Assert.AreEqual(1.0, Vector3.UnitZ.Z);
    }


    [Test]
    public void IndexerRead()
    {
      Vector3 v = new Vector3(1.0f, -10e10f, 0.0f);
      Assert.AreEqual(1.0f, v.GetComponentByIndex(0));
      Assert.AreEqual(-10e10f, v.GetComponentByIndex(1));
      Assert.AreEqual(0.0f, v.GetComponentByIndex(2));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerReadException()
    {
      Vector3 v = new Vector3(1.0f, -10e10f, 0.0f);
      Assert.AreEqual(1.0, v.GetComponentByIndex(-1));
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerReadException2()
    {
      Vector3 v = new Vector3(1.0f, -10e10f, 0.0f);
      Assert.AreEqual(1.0f, v.GetComponentByIndex(3));
    }


    [Test]
    public void IndexerWrite()
    {
      Vector3 v = Vector3.Zero;
      v.SetComponentByIndex(0, 1.0f);
      v.SetComponentByIndex(1, -10e10f);
      v.SetComponentByIndex(2, 0.1f);
      Assert.AreEqual(1.0f, v.X);
      Assert.AreEqual(-10e10f, v.Y);
      Assert.AreEqual(0.1f, v.Z);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerWriteException()
    {
      Vector3 v = new Vector3(1.0f, -10e10f, 0.0f);
      v.SetComponentByIndex(-1, 0.0f);
    }


    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void IndexerWriteException2()
    {
      Vector3 v = new Vector3(1.0f, -10e10f, 0.0f);
      v.SetComponentByIndex(3, 0.0f);
    }


    [Test]
    public void IsNaN()
    {
      const int numberOfRows = 3;
      Assert.IsFalse(new Vector3().IsNaN());

      for (int i = 0; i < numberOfRows; i++)
      {
        Vector3 v = new Vector3();
        v.SetComponentByIndex(i, float.NaN);
        Assert.IsTrue(v.IsNaN());
      }
    }


    [Test]
    public void IsNormalized()
    {
      float originalEpsilon = Numeric.EpsilonF;
      Numeric.EpsilonF = 1e-7f;

      Vector3 arbitraryVector = new Vector3(1.0f, 0.001f, 0.001f);
      Assert.IsFalse(arbitraryVector.IsNumericallyNormalized());

      Vector3 normalizedVector = new Vector3(1.00000001f, 0.00000001f, 0.000000001f);
      Assert.IsTrue(normalizedVector.IsNumericallyNormalized());
      Numeric.EpsilonF = originalEpsilon;
    }


    [Test]
    public void Length()
    {
      Assert.AreEqual(1.0, Vector3.UnitX.Length());
      Assert.AreEqual(1.0, Vector3.UnitY.Length());
      Assert.AreEqual(1.0, Vector3.UnitZ.Length());

      float x = -1.9f;
      float y = 2.1f;
      float z = 10.0f;
      float length = (float)Math.Sqrt(x * x + y * y + z * z);
      Vector3 v = new Vector3(x, y, z);
      Assert.AreEqual(length, v.Length());
    }


    [Test]
    public void Length2()
    {
      Vector3 v = new Vector3(1.0f, 2.0f, 3.0f);
      v.SetLength(0.5f);
      AssertExt.AreNumericallyEqual(0.5f, v.Length());
    }


    [Test]
    [ExpectedException(typeof(MathematicsException))]
    public void LengthException()
    {
      Vector3 v = Vector3.Zero;
      v.SetLength(0.5f);
    }


    [Test]
    public void LengthSquared()
    {
      Assert.AreEqual(1.0, Vector3.UnitX.LengthSquared());
      Assert.AreEqual(1.0, Vector3.UnitY.LengthSquared());
      Assert.AreEqual(1.0, Vector3.UnitZ.LengthSquared());

      float x = -1.9f;
      float y = 2.1f;
      float z = 10.0f;
      float length = x * x + y * y + z * z;
      Vector3 v = new Vector3(x, y, z);
      Assert.AreEqual(length, v.LengthSquared());
    }


    [Test]
    public void Normalized()
    {
      Vector3 v = new Vector3(3.0f, -1.0f, 23.0f);
      Vector3 normalized = v.Normalized();
      Assert.AreEqual(new Vector3(3.0f, -1.0f, 23.0f), v);
      Assert.IsFalse(v.IsNumericallyNormalized());
      Assert.IsTrue(normalized.IsNumericallyNormalized());
    }


    [Test]
    public void Normalize()
    {
      Vector3 v = new Vector3(3.0f, -1.0f, 23.0f);
      v.Normalize();
      Assert.IsTrue(v.IsNumericallyNormalized());
    }


    [Test]
    public void TryNormalize()
    {
      Vector3 v = Vector3.Zero;
      bool normalized = v.TryNormalize();
      Assert.IsFalse(normalized);

      v = new Vector3(1, 2, 3);
      normalized = v.TryNormalize();
      Assert.IsTrue(normalized);
      Assert.AreEqual(new Vector3(1, 2, 3).Normalized(), v);

      v = new Vector3(0, -1, 0);
      normalized = v.TryNormalize();
      Assert.IsTrue(normalized);
      Assert.AreEqual(new Vector3(0, -1, 0).Normalized(), v);
    }


    [Test]
    public void OrthogonalVectors()
    {
      Vector3 v = Vector3.UnitX;
      Vector3 orthogonal1 = v.Orthonormal1();
      Vector3 orthogonal2 = v.Orthonormal2();
      Assert.IsTrue(Numeric.IsZero(Vector3.Dot(v, orthogonal1)));
      Assert.IsTrue(Numeric.IsZero(Vector3.Dot(v, orthogonal2)));
      Assert.IsTrue(Numeric.IsZero(Vector3.Dot(orthogonal1, orthogonal2)));

      v = Vector3.UnitY;
      orthogonal1 = v.Orthonormal1();
      orthogonal2 = v.Orthonormal2();
      Assert.IsTrue(Numeric.IsZero(Vector3.Dot(v, orthogonal1)));
      Assert.IsTrue(Numeric.IsZero(Vector3.Dot(v, orthogonal2)));
      Assert.IsTrue(Numeric.IsZero(Vector3.Dot(orthogonal1, orthogonal2)));

      v = Vector3.UnitZ;
      orthogonal1 = v.Orthonormal1();
      orthogonal2 = v.Orthonormal2();
      Assert.IsTrue(Numeric.IsZero(Vector3.Dot(v, orthogonal1)));
      Assert.IsTrue(Numeric.IsZero(Vector3.Dot(v, orthogonal2)));
      Assert.IsTrue(Numeric.IsZero(Vector3.Dot(orthogonal1, orthogonal2)));

      v = new Vector3(23.0f, 44.0f, 21.0f);
      orthogonal1 = v.Orthonormal1();
      orthogonal2 = v.Orthonormal2();
      Assert.IsTrue(Numeric.IsZero(Vector3.Dot(v, orthogonal1)));
      Assert.IsTrue(Numeric.IsZero(Vector3.Dot(v, orthogonal2)));
      Assert.IsTrue(Numeric.IsZero(Vector3.Dot(orthogonal1, orthogonal2)));
    }


    [Test]
    public void DotProduct()
    {
      // 0�
      Assert.AreEqual(1.0, Vector3.Dot(Vector3.UnitX, Vector3.UnitX));
      Assert.AreEqual(1.0, Vector3.Dot(Vector3.UnitY, Vector3.UnitY));
      Assert.AreEqual(1.0, Vector3.Dot(Vector3.UnitZ, Vector3.UnitZ));

      // 180�
      Assert.AreEqual(-1.0, Vector3.Dot(Vector3.UnitX, -Vector3.UnitX));
      Assert.AreEqual(-1.0, Vector3.Dot(Vector3.UnitY, -Vector3.UnitY));
      Assert.AreEqual(-1.0, Vector3.Dot(Vector3.UnitZ, -Vector3.UnitZ));

      // 90�
      Assert.AreEqual(0.0, Vector3.Dot(Vector3.UnitX, Vector3.UnitY));
      Assert.AreEqual(0.0, Vector3.Dot(Vector3.UnitY, Vector3.UnitZ));
      Assert.AreEqual(0.0, Vector3.Dot(Vector3.UnitX, Vector3.UnitZ));

      // 45�
      float angle = (float)Math.Acos(Vector3.Dot(new Vector3(1f, 1f, 0f).Normalized(), Vector3.UnitX));
      AssertExt.AreNumericallyEqual(MathHelper.ToRadians(45), angle);
      angle = (float)Math.Acos(Vector3.Dot(new Vector3(0f, 1f, 1f).Normalized(), Vector3.UnitY));
      AssertExt.AreNumericallyEqual(MathHelper.ToRadians(45), angle);
      angle = (float)Math.Acos(Vector3.Dot(new Vector3(1f, 0f, 1f).Normalized(), Vector3.UnitZ));
      AssertExt.AreNumericallyEqual(MathHelper.ToRadians(45), angle);
    }


    [Test]
    public void CrossProduct()
    {
      Vector3 result = Vector3.Cross(Vector3.UnitX, Vector3.UnitY);
      Assert.IsTrue(result == Vector3.UnitZ);

      result = Vector3.Cross(Vector3.UnitY, Vector3.UnitZ);
      Assert.IsTrue(result == Vector3.UnitX);

      result = Vector3.Cross(Vector3.UnitZ, Vector3.UnitX);
      Assert.IsTrue(result == Vector3.UnitY);
    }


    [Test]
    public void GetAngle()
    {
      Vector3 x = Vector3.UnitX;
      Vector3 y = Vector3.UnitY;
      Vector3 halfvector = x + y;

      // 90�
      AssertExt.AreNumericallyEqual((float)Math.PI / 4f, MathHelper.GetAngle(x, halfvector));

      // 45�
      AssertExt.AreNumericallyEqual((float)Math.PI / 2f, MathHelper.GetAngle(x, y));
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void GetAngleException()
    {
      MathHelper.GetAngle(Vector3.UnitX, Vector3.Zero);
    }


    [Test]
    public void CrossProductMatrix()
    {
      Vector3 v = new Vector3(-1.0f, -2.0f, -3.0f);
      Vector3 w = new Vector3(4.0f, 5.0f, 6.0f);
      Matrix33F m = v.ToCrossProductMatrix();
      Assert.AreEqual(Vector3.Cross(v, w), v.ToCrossProductMatrix() * w);
    }


    [Test]
    public void ExplicitFromXnaCast()
    {
      Vector3 xna = new Vector3(6, 7, 8);
      Vector3 v = (Vector3)xna;

      Assert.AreEqual(xna.X, v.X);
      Assert.AreEqual(xna.Y, v.Y);
      Assert.AreEqual(xna.Z, v.Z);
    }


    [Test]
    public void ExplicitFloatArrayCast2()
    {
      float x = 23.4f;
      float y = -11.0f;
      float z = 0.0f;
      float[] values = (new Vector3(x, y, z)).ToArray();
      Assert.AreEqual(x, values[0]);
      Assert.AreEqual(y, values[1]);
      Assert.AreEqual(z, values[2]);
      Assert.AreEqual(3, values.Length);
    }


    [Test]
    public void ImplicitVector3Cast()
    {
      float x = 23.5f;
      float y = 0.0f;
      float z = -11.0f;
      Vector3 vector3D = new Vector3(x, y, z);
      Assert.AreEqual(x, vector3D.GetComponentByIndex(0));
      Assert.AreEqual(y, vector3D.GetComponentByIndex(1));
      Assert.AreEqual(z, vector3D.GetComponentByIndex(2));
    }


    [Test]
    public void ProjectTo()
    {
      // Project (1, 1, 1) to axes
      Vector3 v = Vector3.One;
      Vector3 projection = MathHelper.ProjectTo(v, Vector3.UnitX);
      Assert.AreEqual(Vector3.UnitX, projection);
      projection = MathHelper.ProjectTo(v, Vector3.UnitY);
      Assert.AreEqual(Vector3.UnitY, projection);
      projection = MathHelper.ProjectTo(v, Vector3.UnitZ);
      Assert.AreEqual(Vector3.UnitZ, projection);

      // Project axes to (1, 1, 1)
      Vector3 expected = Vector3.One / 3.0f;
      projection = MathHelper.ProjectTo(Vector3.UnitX, v);
      Assert.AreEqual(expected, projection);
      projection = MathHelper.ProjectTo(Vector3.UnitY, v);
      Assert.AreEqual(expected, projection);
      projection = MathHelper.ProjectTo(Vector3.UnitZ, v);
      Assert.AreEqual(expected, projection);
    }


    [Test]
    public void Clamp1()
    {
      Vector3 clamped = new Vector3(-10f, 1f, 100f);
      clamped = MathHelper.Clamp(clamped, -100f, 100f);
      Assert.AreEqual(-10f, clamped.X);
      Assert.AreEqual(1f, clamped.Y);
      Assert.AreEqual(100f, clamped.Z);
    }


    [Test]
    public void Clamp2()
    {
      Vector3 clamped = new Vector3(-10, 1, 100);
			clamped = MathHelper.Clamp(clamped, -1, 0);
      Assert.AreEqual(-1, clamped.X);
      Assert.AreEqual(0, clamped.Y);
      Assert.AreEqual(0, clamped.Z);
    }


    [Test]
    public void ClampStatic1()
    {
      Vector3 clamped = new Vector3(-10f, 1f, 100f);
      clamped = MathHelper.Clamp(clamped, -100f, 100f);
      Assert.AreEqual(-10f, clamped.X);
      Assert.AreEqual(1f, clamped.Y);
      Assert.AreEqual(100f, clamped.Z);
    }


    [Test]
    public void ClampStatic2()
    {
      Vector3 clamped = new Vector3(-10, 1, 100);
      clamped = MathHelper.Clamp(clamped, -1, 0);
      Assert.AreEqual(-1, clamped.X);
      Assert.AreEqual(0, clamped.Y);
      Assert.AreEqual(0, clamped.Z);
    }


    [Test]
    public void Min()
    {
      Vector3 v1 = new Vector3(1.0f, 2.0f, 2.5f);
      Vector3 v2 = new Vector3(-1.0f, 2.0f, 3.0f);
      Vector3 min = MathHelper.Min(v1, v2);
      Assert.AreEqual(new Vector3(-1.0f, 2.0f, 2.5f), min);
    }


    [Test]
    public void Max()
    {
      Vector3 v1 = new Vector3(1.0f, 2.0f, 3.0f);
      Vector3 v2 = new Vector3(-1.0f, 2.1f, 3.0f);
      Vector3 max = MathHelper.Max(v1, v2);
      Assert.AreEqual(new Vector3(1.0f, 2.1f, 3.0f), max);
    }


    [Test]
    public void SerializationXml()
    {
      Vector3 v1 = new Vector3(1.0f, 2.0f, 3.0f);
      Vector3 v2;
      string fileName = "SerializationVector3.xml";

      if (File.Exists(fileName))
        File.Delete(fileName);

      XmlSerializer serializer = new XmlSerializer(typeof(Vector3));
      StreamWriter writer = new StreamWriter(fileName);
      serializer.Serialize(writer, v1);
      writer.Close();

      serializer = new XmlSerializer(typeof(Vector3));
      FileStream fileStream = new FileStream(fileName, FileMode.Open);
      v2 = (Vector3)serializer.Deserialize(fileStream);
      Assert.AreEqual(v1, v2);
    }


    [Test]
    [Ignore("Binary serialization not supported in PCL version.")]
    public void SerializationBinary()
    {
      Vector3 v1 = new Vector3(0.1f, -0.2f, 2);
      Vector3 v2;
      string fileName = "SerializationVector3.bin";

      if (File.Exists(fileName))
        File.Delete(fileName);

      FileStream fs = new FileStream(fileName, FileMode.Create);

      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize(fs, v1);
      fs.Close();

      fs = new FileStream(fileName, FileMode.Open);
      formatter = new BinaryFormatter();
      v2 = (Vector3)formatter.Deserialize(fs);
      fs.Close();

      Assert.AreEqual(v1, v2);
    }


    [Test]
    public void SerializationXml2()
    {
      Vector3 v1 = new Vector3(0.1f, -0.2f, 2);
      Vector3 v2;

      string fileName = "SerializationVector3_DataContractSerializer.xml";

      if (File.Exists(fileName))
        File.Delete(fileName);

      var serializer = new DataContractSerializer(typeof(Vector3));
      using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
      using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8))
        serializer.WriteObject(writer, v1);

      using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      using (var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas()))
        v2 = (Vector3)serializer.ReadObject(reader);

      Assert.AreEqual(v1, v2);
    }


    [Test]
    public void SerializationJson()
    {
      Vector3 v1 = new Vector3(0.1f, -0.2f, 2);
      Vector3 v2;

      string fileName = "SerializationVector3.json";

      if (File.Exists(fileName))
        File.Delete(fileName);

      var serializer = new DataContractJsonSerializer(typeof(Vector3));
      using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
        serializer.WriteObject(stream, v1);

      using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        v2 = (Vector3)serializer.ReadObject(stream);

      Assert.AreEqual(v1, v2);
    }


    [Test]
    public void AbsoluteStatic()
    {
      Vector3 v = new Vector3(-1, -2, -3);
      Vector3 absoluteV = MathHelper.Absolute(v);

      Assert.AreEqual(1, absoluteV.X);
      Assert.AreEqual(2, absoluteV.Y);
      Assert.AreEqual(3, absoluteV.Z);

      v = new Vector3(1, 2, 3);
      absoluteV = MathHelper.Absolute(v);
      Assert.AreEqual(1, absoluteV.X);
      Assert.AreEqual(2, absoluteV.Y);
      Assert.AreEqual(3, absoluteV.Z);
    }


    [Test]
    public void Absolute()
    {
      Vector3 v = MathHelper.Absolute(new Vector3(-1, -2, -3));

      Assert.AreEqual(1, v.X);
      Assert.AreEqual(2, v.Y);
      Assert.AreEqual(3, v.Z);

      v = MathHelper.Absolute(new Vector3(1, 2, 3));
      Assert.AreEqual(1, v.X);
      Assert.AreEqual(2, v.Y);
      Assert.AreEqual(3, v.Z);
    }


    [Test]
    public void GetLargestComponent()
    {
      Vector3 v = new Vector3(-1, -2, -3);
      Assert.AreEqual(-1, v.LargestComponent());

      v = new Vector3(10, 20, -30);
      Assert.AreEqual(20, v.LargestComponent());

      v = new Vector3(-1, 20, 30);
      Assert.AreEqual(30, v.LargestComponent());
    }


    [Test]
    public void GetIndexOfLargestComponent()
    {
      Vector3 v = new Vector3(-1, -2, -3);
      Assert.AreEqual(0, v.IndexOfLargestComponent());

      v = new Vector3(10, 20, -30);
      Assert.AreEqual(1, v.IndexOfLargestComponent());

      v = new Vector3(-1, 20, 30);
      Assert.AreEqual(2, v.IndexOfLargestComponent());
    }


    [Test]
    public void GetSmallestComponent()
    {
      Vector3 v = new Vector3(-4, -2, -3);
      Assert.AreEqual(-4, v.SmallestComponent());

      v = new Vector3(10, 0, 3);
      Assert.AreEqual(0, v.SmallestComponent());

      v = new Vector3(-1, 20, -3);
      Assert.AreEqual(-3, v.SmallestComponent());
    }


    [Test]
    public void GetIndexOfSmallestComponent()
    {
      Vector3 v = new Vector3(-4, -2, -3);
      Assert.AreEqual(0, v.IndexOfSmallestComponent());

      v = new Vector3(10, 0, 3);
      Assert.AreEqual(1, v.IndexOfSmallestComponent());

      v = new Vector3(-1, 20, -3);
      Assert.AreEqual(2, v.IndexOfSmallestComponent());
    }
  }
}