using DigitalRise.Mathematics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Animation.Traits.Tests
{
  [TestFixture]
  public class Vector4TraitsTest
  {
    [Test]
    public void IdentityTest()
    {
      var traits = Vector4Traits.Instance;
      var value = new Vector4(-1, -2, 3, 1);
      Assert.AreEqual(value, traits.Add(value, traits.Identity()));
      Assert.AreEqual(value, traits.Add(traits.Identity(), value));
    }


    [Test]
    public void MultiplyTest()
    {
      var traits = Vector4Traits.Instance;
      var value = new Vector4(-1, -2, 3, 1);
      AssertExt.AreNumericallyEqual(Vector4.Zero, traits.Multiply(value, 0));
      AssertExt.AreNumericallyEqual(value, traits.Multiply(value, 1));
      AssertExt.AreNumericallyEqual((value + value), traits.Multiply(value, 2));
      AssertExt.AreNumericallyEqual((value + value + value), traits.Multiply(value, 3));
      AssertExt.AreNumericallyEqual((-value), traits.Multiply(value, -1));
      AssertExt.AreNumericallyEqual((-value - value), traits.Multiply(value, -2));
      AssertExt.AreNumericallyEqual((-value - value - value), traits.Multiply(value, -3));
    }


    [Test]
    public void FromByTest()
    {
      // IAnimationValueTraits<T> is used in a from-by animation to a add a relative offset to
      // the start value.

      var traits = Vector4Traits.Instance;
      var from = new Vector4(-1, -2, 3, 1);
      var by = new Vector4(4, -5, 6, 1);

      var to = traits.Add(from, by);
      AssertExt.AreNumericallyEqual((by + from), to);

      AssertExt.AreNumericallyEqual(from, traits.Add(to, traits.Inverse(by)));
      AssertExt.AreNumericallyEqual(by, traits.Add(traits.Inverse(from), to));
    }


    [Test]
    public void CycleOffsetTest()
    {
      // IAnimationValueTraits<T> is used in a cyclic animation to a add the cycle offset in
      // each iteration.

      var traits = Vector4Traits.Instance;
      var first = new Vector4(1, 2, 3, 1);    // Animation value of first key frame.
      var last = new Vector4(-4, 5, -6, 5);   // Animation value of last key frame.
      var cycleOffset = traits.Add(traits.Inverse(first), last);

      // Cycle offset should be the difference between last and first key frame.
      AssertExt.AreNumericallyEqual(last, traits.Add(first, cycleOffset));
      AssertExt.AreNumericallyEqual(last, (cycleOffset + first));

      // Check multiple cycles (post-loop).
      AssertExt.AreNumericallyEqual(last, traits.Add(first, traits.Multiply(cycleOffset, 1)));
      AssertExt.AreNumericallyEqual((cycleOffset + cycleOffset + last), traits.Add(first, traits.Multiply(cycleOffset, 3)));

      // Check multiple cycles (pre-loop).
      AssertExt.AreNumericallyEqual(first, traits.Add(last, traits.Multiply(cycleOffset, -1)));
      AssertExt.AreNumericallyEqual((first - cycleOffset - cycleOffset), traits.Add(last, traits.Multiply(cycleOffset, -3)));
    }


    [Test]
    public void InterpolationTest()
    {
      var traits = Vector4Traits.Instance;
      var value0 = new Vector4(1, 2, 3, 1);
      var value1 = new Vector4(-4, 5, -6, 5);
      AssertExt.AreNumericallyEqual(value0, traits.Interpolate(value0, value1, 0.0f));
      AssertExt.AreNumericallyEqual(value1, traits.Interpolate(value0, value1, 1.0f));
      AssertExt.AreNumericallyEqual((0.25f * value0 + 0.75f * value1), traits.Interpolate(value0, value1, 0.75f));
    }
  }
}
