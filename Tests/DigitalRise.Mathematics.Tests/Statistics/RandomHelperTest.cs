using System;
using NUnit.Framework;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;

namespace DigitalRise.Mathematics.Statistics.Tests
{
  [TestFixture]
  public class RandomHelperTest
  {
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RandomShouldThrowArgumentNullException()
    {
      RandomHelper.Random = null;
    }


    [Test]
    public void RandomDouble()
    {
      double random = RandomHelper.Random.NextDouble(10.0, 11.0);
      Assert.IsTrue(10.0 <= random);
      Assert.IsTrue(random <= 11.0);

      // Must not throw exception.
      RandomHelper.NextDouble(null, 10.0, 11.0);
    }


    [Test]
    public void RandomFloat()
    {
      float random = RandomHelper.Random.NextFloat(20.0f, 21.0f);
      Assert.IsTrue(20.0f <= random);
      Assert.IsTrue(random <= 21.0f);

      // Must not throw exception.
      RandomHelper.NextFloat(null, 10.0f, 11.0f);
    }


    [Test]
    public void RandomInt()
    {
      int random = RandomHelper.Random.NextInteger(10, 20);
      Assert.IsTrue(10 <= random);
      Assert.IsTrue(random <= 20);

      random = RandomHelper.Random.NextInteger(0, 0);
      Assert.AreEqual(0, random);

      random = RandomHelper.Random.NextInteger(-20, -20);
      Assert.AreEqual(-20, random);

      // Must not throw exception.
      RandomHelper.NextInteger(null, 10, 11);
    }


    [Test]
    public void RandomByte()
    {
      int random = RandomHelper.Random.NextByte();
      Assert.IsTrue(0 <= random);
      Assert.IsTrue(random <= 255);

      // Must not throw exception.
      RandomHelper.NextByte(null);
    }


    [Test]
    public void RandomBool()
    {
      bool random = RandomHelper.Random.NextBool();
      bool b2 = RandomHelper.NextBool(null);

      // Must not throw exception.
      RandomHelper.NextBool(null);
    }


    [Test]
    public void RandomNumberGenerator()
    {
      Random random = RandomHelper.Random;
      Assert.IsNotNull(random);

      random = new Random();
      RandomHelper.Random = random;
      Assert.AreSame(random, RandomHelper.Random);
    }


    [Test]
    public void RandomVector2()
    {
      Vector2 vector = RandomHelper.Random.NextVector2(-20.0f, -10.0f);
      Assert.IsTrue(-20.0f <= vector.X && vector.X <= -10.0f);
      Assert.IsTrue(-20.0f <= vector.Y && vector.Y <= -10.0f);

      vector = RandomHelper.Random.NextVector2(1.0f, 1.0f);
      Assert.AreEqual(1.0f, vector.X);
      Assert.AreEqual(1.0f, vector.Y);

      // Must not throw exception.
      RandomHelper.NextVector2(null, 1, 3);
    }


    [Test]
    public void RandomVector3()
    {
      Vector3 vector = RandomHelper.Random.NextVector3(-20.0f, -10.0f);
      Assert.IsTrue(-20.0f <= vector.X && vector.X <= -10.0f);
      Assert.IsTrue(-20.0f <= vector.Y && vector.Y <= -10.0f);
      Assert.IsTrue(-20.0f <= vector.Z && vector.Z <= -10.0f);

      vector = RandomHelper.Random.NextVector3(1.0f, 1.0f);
      Assert.AreEqual(1.0f, vector.X);
      Assert.AreEqual(1.0f, vector.Y);
      Assert.AreEqual(1.0f, vector.Z);

      // Must not throw exception.
      RandomHelper.NextVector3(null, 1, 3);
    }


    [Test]
    public void RandomVector4()
    {
      Vector4 vector = RandomHelper.Random.NextVector4(-20.0f, -10.0f);
      Assert.IsTrue(-20.0f <= vector.X && vector.X <= -10.0f);
      Assert.IsTrue(-20.0f <= vector.Y && vector.Y <= -10.0f);
      Assert.IsTrue(-20.0f <= vector.Z && vector.Z <= -10.0f);
      Assert.IsTrue(-20.0f <= vector.W && vector.W <= -10.0f);

      vector = RandomHelper.Random.NextVector4(-1.0f, -1.0f);
      Assert.AreEqual(-1.0f, vector.X);
      Assert.AreEqual(-1.0f, vector.Y);
      Assert.AreEqual(-1.0f, vector.Z);
      Assert.AreEqual(-1.0f, vector.W);

      // Must not throw exception.
      RandomHelper.NextVector4(null, 1, 3);
    }

    
    [Test]
    public void RandomMatrix22F()
    {
      Matrix22F matrix = RandomHelper.Random.NextMatrix22F(10.0f, 100.0f);
      for (int i = 0; i < 4; i++)
      {
        Assert.IsTrue(10.0f <= matrix[i] && matrix[i] <= 100.0f);
      }

      // Must not throw exception.
      RandomHelper.NextMatrix22F(null, 1, 3);
    }


    [Test]
    public void RandomMatrix33F()
    {
      Matrix33F matrix = RandomHelper.Random.NextMatrix33F(10.0f, 100.0f);
      for (int i = 0; i < 9; i++)
      {
        Assert.IsTrue(10.0f <= matrix[i] && matrix[i] <= 100.0f);
      }

      // Must not throw exception.
      RandomHelper.NextMatrix33F(null, 1, 3);
    }


    [Test]
    public void RandomMatrix44F()
    {
      Matrix44F matrix = RandomHelper.Random.NextMatrix44F(-2.0f, 0.5f);
      for (int i = 0; i < 16; i++)
      {
        Assert.IsTrue(-2.0f <= matrix[i] && matrix[i] <= 0.5f);
      }

      // Must not throw exception.
      RandomHelper.NextMatrix44F(null, 1, 3);
    }


    [Test]
    public void RandomFromDistribution()
    {
      Distribution<float> distribution = new ConstValueDistribution<float>(123.456f);
      float f = RandomHelper.Next(null, distribution);
      Assert.AreEqual(123.456f, f);

      var random = new Random(123456);
      distribution = new UniformDistributionF(1.0f, 2.0f);
      f = random.Next(distribution);
      Assert.IsTrue(1.0f <= f && f <= 2.0f);
    }
  }
}
