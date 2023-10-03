using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using NUnit.Framework;


namespace DigitalRise.Animation.Traits.Tests
{
  [TestFixture]
  public class Vector2TraitsTest
  {
    [Test]
    public void IdentityTest()
    {
      var traits = Vector2Traits.Instance;
      var value = new Vector2(-1, 2);
      Assert.AreEqual(value, traits.Add(value, traits.Identity()));
      Assert.AreEqual(value, traits.Add(traits.Identity(), value));
    }


    [Test]
    public void MultiplyTest()
    {
      var traits = Vector2Traits.Instance;
      var value = new Vector2(-1, 2);
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual(Vector2.Zero, traits.Multiply(value, 0)));
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual(value, traits.Multiply(value, 1)));
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual((value + value), traits.Multiply(value, 2)));
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual((value + value + value), traits.Multiply(value, 3)));
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual((-value), traits.Multiply(value, -1)));
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual((-value - value), traits.Multiply(value, -2)));
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual((-value - value - value), traits.Multiply(value, -3)));
    }

    
    [Test]
    public void FromByTest()
    {
      // IAnimationValueTraits<T> is used in a from-by animation to a add a relative offset to
      // the start value.

      var traits = Vector2Traits.Instance;
      var from = new Vector2(-1, -2);
      var by = new Vector2(4, -5);

      var to = traits.Add(from, by);
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual((by + from), to));

      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual(from, traits.Add(to, traits.Inverse(by))));
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual(by, traits.Add(traits.Inverse(from), to)));
    }


    [Test]
    public void CycleOffsetTest()
    {
      // IAnimationValueTraits<T> is used in a cyclic animation to a add the cycle offset in
      // each iteration.

      var traits = Vector2Traits.Instance;
      var first = new Vector2(1, 2);    // Animation value of first key frame.
      var last = new Vector2(-4, 5);    // Animation value of last key frame.
      var cycleOffset = traits.Add(traits.Inverse(first), last);

      // Cycle offset should be the difference between last and first key frame.
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual(last, traits.Add(first, cycleOffset)));
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual(last, (cycleOffset + first)));

      // Check multiple cycles (post-loop).
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual(last, traits.Add(first, traits.Multiply(cycleOffset, 1))));
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual((cycleOffset + cycleOffset + last), traits.Add(first, traits.Multiply(cycleOffset, 3))));

      // Check multiple cycles (pre-loop).
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual(first, traits.Add(last, traits.Multiply(cycleOffset, -1))));
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual((first - cycleOffset - cycleOffset), traits.Add(last, traits.Multiply(cycleOffset, -3))));
    }


    [Test]
    public void InterpolationTest()
    {
      var traits = Vector2Traits.Instance;
      var value0 = new Vector2(2, 3);
      var value1 = new Vector2(5, -6);
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual(value0, traits.Interpolate(value0, value1, 0.0f)));
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual(value1, traits.Interpolate(value0, value1, 1.0f)));
      Assert.IsTrue(Mathematics.MathHelper.AreNumericallyEqual((0.25f * value0 + 0.75f * value1), traits.Interpolate(value0, value1, 0.75f)));
    }
  }
}
