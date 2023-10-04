using Microsoft.Xna.Framework;
using NUnit.Framework;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Animation.Traits.Tests
{
  [TestFixture]
  public class Vector3TraitsTest
  {
    [Test]
    public void IdentityTest()
    {
      var traits = Vector3Traits.Instance;
      var value = new Vector3(-1, -2, 3);
      Assert.AreEqual(value, traits.Add(value, traits.Identity()));
      Assert.AreEqual(value, traits.Add(traits.Identity(), value));
    }

    
    [Test]
    public void MultiplyTest()
    {
      var traits = Vector3Traits.Instance;
      var value = new Vector3(-1, -2, 3);
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)Vector3.Zero, (Vector3)traits.Multiply(value, 0)));
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)value, (Vector3)traits.Multiply(value, 1)));
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)(value + value), (Vector3)traits.Multiply(value, 2)));
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)(value + value + value), (Vector3)traits.Multiply(value, 3)));
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)(-value), (Vector3)traits.Multiply(value, -1)));
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)(-value - value), (Vector3)traits.Multiply(value, -2)));
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)(-value - value - value), (Vector3)traits.Multiply(value, -3)));
    }


    [Test]
    public void FromByTest()
    {
      // IAnimationValueTraits<T> is used in a from-by animation to a add a relative offset to
      // the start value.

      var traits = Vector3Traits.Instance;
      var from = new Vector3(-1, -2, 3);
      var by = new Vector3(4, -5, 6);

      var to = traits.Add(from, by);
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)(by + from), (Vector3)to));

      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)from, (Vector3)traits.Add(to, traits.Inverse(by))));
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)by, (Vector3)traits.Add(traits.Inverse(from), to)));
    }


    [Test]
    public void CycleOffsetTest()
    {
      // IAnimationValueTraits<T> is used in a cyclic animation to a add the cycle offset in
      // each iteration.

      var traits = Vector3Traits.Instance;
      var first = new Vector3(1, 2, 3);    // Animation value of first key frame.
      var last = new Vector3(-4, 5, -6);   // Animation value of last key frame.
      var cycleOffset = traits.Add(traits.Inverse(first), last);

      // Cycle offset should be the difference between last and first key frame.
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)last, (Vector3)traits.Add(first, cycleOffset)));
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)last, (Vector3)(cycleOffset + first)));

      // Check multiple cycles (post-loop).
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)last, (Vector3)traits.Add(first, traits.Multiply(cycleOffset, 1))));
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)(cycleOffset + cycleOffset + last), (Vector3)traits.Add(first, traits.Multiply(cycleOffset, 3))));

      // Check multiple cycles (pre-loop).
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)first, (Vector3)traits.Add(last, traits.Multiply(cycleOffset, -1))));
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)(first - cycleOffset - cycleOffset), (Vector3)traits.Add(last, traits.Multiply(cycleOffset, -3))));
    }


    [Test]
    public void InterpolationTest()
    {
      var traits = Vector3Traits.Instance;
      var value0 = new Vector3(1, 2, 3);
      var value1 = new Vector3(-4, 5, -6);
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)value0, (Vector3)traits.Interpolate(value0, value1, 0.0f)));
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)value1, (Vector3)traits.Interpolate(value0, value1, 1.0f)));
      Assert.IsTrue(MathHelper.AreNumericallyEqual((Vector3)(0.25f * value0 + 0.75f * value1), (Vector3)traits.Interpolate(value0, value1, 0.75f)));
    }
  }
}
