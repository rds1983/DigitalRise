using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Animation.Traits.Tests
{
  [TestFixture]
  public class SingleTraitsTest
  {
    [Test]
    public void IdentityTest()
    {
      var traits = SingleTraits.Instance;
      var value = 123.45f;
      Assert.AreEqual(value, traits.Add(value, traits.Identity()));
      Assert.AreEqual(value, traits.Add(traits.Identity(), value));
    }


    [Test]
    public void MultiplyTest()
    {
      var traits = SingleTraits.Instance;
      var value = -123.45f;
      AssertExt.AreNumericallyEqual(0, traits.Multiply(value, 0));
      AssertExt.AreNumericallyEqual(value, traits.Multiply(value, 1));
      AssertExt.AreNumericallyEqual(value + value, traits.Multiply(value, 2));
      AssertExt.AreNumericallyEqual(value + value + value, traits.Multiply(value, 3));
      AssertExt.AreNumericallyEqual(-value, traits.Multiply(value, -1));
      AssertExt.AreNumericallyEqual(-value - value, traits.Multiply(value, -2));
      AssertExt.AreNumericallyEqual(-value - value - value, traits.Multiply(value, -3));
    }


    [Test]
    public void FromByTest()
    {
      // IAnimationValueTraits<T> is used in a from-by animation to a add a relative offset to
      // the start value.

      var traits = SingleTraits.Instance;
      var from = -123.45f;
      var by = 98.76f;

      var to = traits.Add(from, by);
      AssertExt.AreNumericallyEqual(by + from, to);

      AssertExt.AreNumericallyEqual(from, traits.Add(to, traits.Inverse(by)));
      AssertExt.AreNumericallyEqual(by, traits.Add(traits.Inverse(from), to));
    }


    [Test]
    public void CycleOffsetTest()
    {
      // IAnimationValueTraits<T> is used in a cyclic animation to a add the cycle offset in
      // each iteration.

      var traits = SingleTraits.Instance;
      var first = 456.78f;    // Animation value of first key frame.
      var last = 321.45f;     // Animation value of last key frame.
      var cycleOffset = traits.Add(traits.Inverse(first), last);

      // Cycle offset should be the difference between last and first key frame.
      AssertExt.AreNumericallyEqual(last, traits.Add(first, cycleOffset));
      AssertExt.AreNumericallyEqual(last, cycleOffset + first);

      // Check multiple cycles (post-loop).
      AssertExt.AreNumericallyEqual(last, traits.Add(first, traits.Multiply(cycleOffset, 1)));
      AssertExt.AreNumericallyEqual(cycleOffset + cycleOffset + last, traits.Add(first, traits.Multiply(cycleOffset, 3)));

      // Check multiple cycles (pre-loop).
      AssertExt.AreNumericallyEqual(first, traits.Add(last, traits.Multiply(cycleOffset, -1)));
      AssertExt.AreNumericallyEqual(first - cycleOffset - cycleOffset, traits.Add(last, traits.Multiply(cycleOffset, -3)));
    }


    [Test]
    public void InterpolationTest()
    {
      using (var setEpsilon = new SetEpsilonF(1E-04f))
      {
        var traits = SingleTraits.Instance;
        var value0 = 456.78f;
        var value1 = -321.45f;
        AssertExt.AreNumericallyEqual(value0, traits.Interpolate(value0, value1, 0.0f));
        AssertExt.AreNumericallyEqual(value1, traits.Interpolate(value0, value1, 1.0f));
        AssertExt.AreNumericallyEqual(0.25f * value0 + 0.75f * value1, traits.Interpolate(value0, value1, 0.75f));
      }
    }
  }
}
