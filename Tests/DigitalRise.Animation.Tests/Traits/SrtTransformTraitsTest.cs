﻿using System;
using DigitalRise.Animation.Character;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Statistics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Animation.Traits.Tests
{
  [TestFixture]
  public class SrtTransformTraitsTest
  {
    private Random _random;

    private SrtTransform NextRandomValue()
    {
      return new SrtTransform(new Vector3(_random.NextFloat(-10, 10)), _random.NextQuaternion(), _random.NextVector3(-10, 10));
    }


    [SetUp]
    public void Setup()
    {
      _random = new Random(123456);
    }


    [Test]
    public void IdentityTest()
    {
      var traits = SrtTransformTraits.Instance;
      var value = NextRandomValue();
      Assert.AreEqual(value, traits.Add(value, traits.Identity()));
      Assert.AreEqual(value, traits.Add(traits.Identity(), value));
    }


    [Test]
    public void MultiplyTest()
    {
      using (var setEpsilon = new SetEpsilonF(1E-04f))
      {
        var traits = SrtTransformTraits.Instance;
        var value = NextRandomValue();
        AssertExt.AreNumericallyEqual(SrtTransform.Identity, traits.Multiply(value, 0));
        AssertExt.AreNumericallyEqual(value, traits.Multiply(value, 1));
        AssertExt.AreNumericallyEqual(value * value, traits.Multiply(value, 2));
        AssertExt.AreNumericallyEqual(value * value * value, traits.Multiply(value, 3));
        AssertExt.AreNumericallyEqual(value.Inverse, traits.Multiply(value, -1));
        AssertExt.AreNumericallyEqual(value.Inverse * value.Inverse, traits.Multiply(value, -2));
        AssertExt.AreNumericallyEqual(value.Inverse * value.Inverse * value.Inverse, traits.Multiply(value, -3));
      }
    }


    [Test]
    public void FromByTest()
    {
      // IAnimationValueTraits<T> is used in a from-by animation to a add a relative offset to
      // the start value.

      var traits = SrtTransformTraits.Instance;
      var from = NextRandomValue();
      var by = NextRandomValue();

      var to = traits.Add(from, by);
      AssertExt.AreNumericallyEqual(by * from, to);

      AssertExt.AreNumericallyEqual(from, traits.Add(to, traits.Inverse(by)));
      AssertExt.AreNumericallyEqual(by, traits.Add(traits.Inverse(from), to));
    }


    [Test]
    public void CycleOffsetTest()
    {
      using (var setEpsilon = new SetEpsilonF(1E-03f))
      {
        // IAnimationValueTraits<T> is used in a cyclic animation to a add the cycle offset in
        // each iteration.

        var traits = SrtTransformTraits.Instance;
        var first = NextRandomValue();    // Animation value of first key frame.
        var last = NextRandomValue();     // Animation value of last key frame.
        var cycleOffset = traits.Add(traits.Inverse(first), last);

        // Cycle offset should be the difference between last and first key frame.
        AssertExt.AreNumericallyEqual(last, traits.Add(first, cycleOffset));
        AssertExt.AreNumericallyEqual(last, cycleOffset * first);

        // Check multiple cycles (post-loop).
        AssertExt.AreNumericallyEqual(last, traits.Add(first, traits.Multiply(cycleOffset, 1)));
        AssertExt.AreNumericallyEqual(cycleOffset * cycleOffset * last, traits.Add(first, traits.Multiply(cycleOffset, 3)));

        // Check multiple cycles (pre-loop).
        AssertExt.AreNumericallyEqual(first, traits.Add(last, traits.Multiply(cycleOffset, -1)));
        AssertExt.AreNumericallyEqual(cycleOffset.Inverse * cycleOffset.Inverse * first, traits.Add(last, traits.Multiply(cycleOffset, -3)));
      }
    }


    [Test]
    public void InterpolationTest()
    {
      var traits = SrtTransformTraits.Instance;
      var value0 = NextRandomValue();
      var value1 = NextRandomValue();
      AssertExt.AreNumericallyEqual(value0, traits.Interpolate(value0, value1, 0.0f));
      Assert.IsTrue(SrtTransform.AreNumericallyEqual(value1, traits.Interpolate(value0, value1, 1.0f))
                    || SrtTransform.AreNumericallyEqual(new SrtTransform(value1.Scale, -value1.Rotation, value1.Translation), traits.Interpolate(value0, value1, 1.0f)));
      AssertExt.AreNumericallyEqual(SrtTransform.Interpolate(value0, value1, 0.75f), traits.Interpolate(value0, value1, 0.75f));
    }


    [Test]
    public void BlendTest()
    {
      var traits = SrtTransformTraits.Instance;

      var value0 = NextRandomValue();
      var value1 = NextRandomValue();
      var value2 = NextRandomValue();
      var w0 = 0.3f;
      var w1 = 0.4f;
      var w2 = 1 - w0 - w1;

      SrtTransform result = new SrtTransform();
      traits.BeginBlend(ref result);
      traits.BlendNext(ref result, ref value0, w0);
      traits.BlendNext(ref result, ref value1, w1);
      traits.BlendNext(ref result, ref value2, w2);
      traits.EndBlend(ref result);

      AssertExt.AreNumericallyEqual(value0.Scale * w0 + value1.Scale * w1 + value2.Scale * w2, result.Scale);
      AssertExt.AreNumericallyEqual(value0.Translation * w0 + value1.Translation * w1 + value2.Translation * w2, result.Translation);

      Quaternion expected;
      expected = value0.Rotation * w0;
      // Consider "selective negation" when blending quaternions!
      if (Quaternion.Dot(expected, value1.Rotation) < 0)
        value1.Rotation = -value1.Rotation;
      expected += value1.Rotation * w1;
      if (Quaternion.Dot(expected, value2.Rotation) < 0)
        value2.Rotation = -value2.Rotation;
      expected += value2.Rotation * w2;
      expected.Normalize();

      AssertExt.AreNumericallyEqual(expected, result.Rotation);
    }
  }
}
