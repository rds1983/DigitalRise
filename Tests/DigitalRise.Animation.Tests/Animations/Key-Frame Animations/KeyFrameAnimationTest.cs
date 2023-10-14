using System;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Interpolation;
using DigitalRise.Mathematics.Statistics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class KeyFrameAnimationTest
  {
    // Note: The base class KeyFrameAnimation is tested using the QuaternionKeyFrameAnimation
    // because quaternions (or transformation matrices) are the difficult case. Animation values of 
    // type quaternions are concatenated using the multiplication. The multiplication of quaternions 
    // is not commutative, therefore the order of the values is important. 


    private Random _random;


    [SetUp]
    public void Setup()
    {
      _random = new Random(12345);
    }


    [Test]
    public void DefaultValues()
    {
      var animation = new QuaternionKeyFrameAnimation();
      Assert.IsNotNull(animation.KeyFrames);
      Assert.IsEmpty(animation.KeyFrames);
      Assert.IsTrue(animation.EnableInterpolation);
    }


    [Test]
    public void Sorting()
    {
      var animation = new QuaternionKeyFrameAnimation();
      animation.KeyFrames.Add(new KeyFrame<Quaternion> { Time = TimeSpan.FromSeconds(2) });
      animation.KeyFrames.Add(new KeyFrame<Quaternion> { Time = TimeSpan.FromSeconds(-1) });
      animation.KeyFrames.Add(new KeyFrame<Quaternion> { Time = TimeSpan.FromSeconds(0) });
      animation.KeyFrames.Add(new KeyFrame<Quaternion> { Time = TimeSpan.FromSeconds(4) });
      animation.KeyFrames.Sort();

      Assert.AreEqual(TimeSpan.FromSeconds(-1), animation.KeyFrames[0].Time);
      Assert.AreEqual(TimeSpan.FromSeconds(0), animation.KeyFrames[1].Time);
      Assert.AreEqual(TimeSpan.FromSeconds(2), animation.KeyFrames[2].Time);
      Assert.AreEqual(TimeSpan.FromSeconds(4), animation.KeyFrames[3].Time);
    }


    [Test]
    public void EmptyAnimation()
    {
      var animation = new QuaternionKeyFrameAnimation();

      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();

      Assert.AreEqual(defaultSource, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget));
      Assert.AreEqual(defaultSource, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));
    }


    [Test]
    public void AnimationWithOneKeyFrame()
    {
      var keyFrame = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(1.0), _random.NextQuaternion());
      var keyFrameAnimation = new QuaternionKeyFrameAnimation();
      keyFrameAnimation.KeyFrames.Add(keyFrame);
      var animation = new AnimationClip<Quaternion>();
      animation.Animation = keyFrameAnimation;
      animation.Duration = TimeSpan.MaxValue;

      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();
      Assert.AreEqual(keyFrame.Value, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget)); // Pre loop
      Assert.AreEqual(keyFrame.Value, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget)); // Key frame
      Assert.AreEqual(keyFrame.Value, animation.GetValue(TimeSpan.FromSeconds(2.0), defaultSource, defaultTarget)); // Post loop
    }


    [Test]
    public void SamplingKeyFrames()
    {
      var keyFrame0 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(1.0), _random.NextQuaternion());
      var keyFrame1 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(2.0), _random.NextQuaternion());
      var keyFrame2 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(3.0), _random.NextQuaternion());
      var animation = new QuaternionKeyFrameAnimation();
      animation.KeyFrames.Add(keyFrame0);
      animation.KeyFrames.Add(keyFrame1);
      animation.KeyFrames.Add(keyFrame2);

      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();

      // Without interpolation
      animation.EnableInterpolation = false;
      AssertExt.AreNumericallyEqual(keyFrame0.Value, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(keyFrame0.Value, animation.GetValue(TimeSpan.FromSeconds(1.75), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(keyFrame1.Value, animation.GetValue(TimeSpan.FromSeconds(2.0), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(keyFrame1.Value, animation.GetValue(TimeSpan.FromSeconds(2.75), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(keyFrame2.Value, animation.GetValue(TimeSpan.FromSeconds(3.0), defaultSource, defaultTarget));

      // With interpolation
      animation.EnableInterpolation = true;
      AssertExt.AreNumericallyEqual(keyFrame0.Value, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));
      var expected = InterpolationHelper.Lerp(keyFrame0.Value, keyFrame1.Value, 0.75f);
      AssertExt.AreNumericallyEqual(expected, animation.GetValue(TimeSpan.FromSeconds(1.75), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(keyFrame1.Value, animation.GetValue(TimeSpan.FromSeconds(2.0), defaultSource, defaultTarget));
      expected = InterpolationHelper.Lerp(keyFrame1.Value, keyFrame2.Value, 0.75f);
      AssertExt.AreNumericallyEqual(expected, animation.GetValue(TimeSpan.FromSeconds(2.75), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(keyFrame2.Value, animation.GetValue(TimeSpan.FromSeconds(3.0), defaultSource, defaultTarget));
    }


    [Test]
    public void ShouldReturnTheFirstOrLastKeyFrame()
    {
      var keyFrame0 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(1.0), _random.NextQuaternion());
      var keyFrame1 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(2.0), _random.NextQuaternion());
      var keyFrame2 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(3.0), _random.NextQuaternion());
      var animation = new QuaternionKeyFrameAnimation();
      animation.KeyFrames.Add(keyFrame0);
      animation.KeyFrames.Add(keyFrame1);
      animation.KeyFrames.Add(keyFrame2);

      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();

      // Pre loop
      AssertExt.AreNumericallyEqual(keyFrame0.Value, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget));

      // Post loop
      AssertExt.AreNumericallyEqual(keyFrame2.Value, animation.GetValue(TimeSpan.FromSeconds(3.75), defaultSource, defaultTarget));
    }


    #region ----- KeyFrameAnimation<T> -----

    // In the original version, the KeyFrameAnimation<T> directly support pre- and post-loop
    // behaviors. But this was removed in favor of the more general AnimationClip<T>.
    // In the following tests QuaternionKeyFrameAnimation is wrapped using a AnimationClip<T>.

    [Test]
    public void CyclicLoopBehavior()
    {
      var keyFrame0 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(0.0), _random.NextQuaternion());
      var keyFrame1 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(1.0), _random.NextQuaternion());
      var keyFrame2 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(2.0), _random.NextQuaternion());
      var animation = new QuaternionKeyFrameAnimation();
      animation.KeyFrames.Add(keyFrame0);
      animation.KeyFrames.Add(keyFrame1);
      animation.KeyFrames.Add(keyFrame2);

      var animationClip = new AnimationClip<Quaternion> { Animation = animation };
      animationClip.LoopBehavior = LoopBehavior.Cycle;
      animationClip.Duration = TimeSpan.MaxValue;
      animationClip.ClipOffset = TimeSpan.FromSeconds(-1);

      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();

      // Pre loop
      var expected = InterpolationHelper.Lerp(keyFrame1.Value, keyFrame2.Value, 0.25f);
      AssertExt.AreNumericallyEqual(expected, animationClip.GetValue(TimeSpan.FromSeconds(0.25), defaultSource, defaultTarget));

      // Post loop
      expected = InterpolationHelper.Lerp(keyFrame1.Value, keyFrame2.Value, 0.75f);
			AssertExt.AreNumericallyEqual(expected, animationClip.GetValue(TimeSpan.FromSeconds(4.75), defaultSource, defaultTarget));
    }


    [Test]
    public void CyclicOffsetLoopBehavior()
    {
      var keyFrame0 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(0.0), _random.NextQuaternion());
      var keyFrame1 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(1.0), _random.NextQuaternion());
      var keyFrame2 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(2.0), _random.NextQuaternion());
      var animation = new QuaternionKeyFrameAnimation();
      animation.KeyFrames.Add(keyFrame0);
      animation.KeyFrames.Add(keyFrame1);
      animation.KeyFrames.Add(keyFrame2);

      var animationClip = new AnimationClip<Quaternion> { Animation = animation };
      animationClip.LoopBehavior = LoopBehavior.CycleOffset;
      animationClip.Duration = TimeSpan.MaxValue;
      animationClip.ClipOffset = TimeSpan.FromSeconds(-1);

      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();

      // Pre loop
      var cycleOffset = keyFrame2.Value * keyFrame0.Value.Inverse();
      var expected = InterpolationHelper.Lerp(keyFrame1.Value, keyFrame2.Value, 0.25f);
      AssertExt.AreNumericallyEqual(expected, cycleOffset * animationClip.GetValue(TimeSpan.FromSeconds(0.25), defaultSource, defaultTarget));

      // Post loop
      expected = cycleOffset * cycleOffset * InterpolationHelper.Lerp(keyFrame1.Value, keyFrame2.Value, 0.75f);
      AssertExt.AreNumericallyEqual(expected, animationClip.GetValue(TimeSpan.FromSeconds(6.75), defaultSource, defaultTarget));
    }


    [Test]
    public void OscillateLoopBehavior()
    {
      var keyFrame0 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(1.0), _random.NextQuaternion());
      var keyFrame1 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(2.0), _random.NextQuaternion());
      var keyFrame2 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(3.0), _random.NextQuaternion());
      var animation = new QuaternionKeyFrameAnimation();
      animation.KeyFrames.Add(keyFrame0);
      animation.KeyFrames.Add(keyFrame1);
      animation.KeyFrames.Add(keyFrame2);

      var animationClip = new AnimationClip<Quaternion> { Animation = animation };
      animationClip.LoopBehavior = LoopBehavior.Oscillate;
      animationClip.Duration = TimeSpan.MaxValue;
      animationClip.ClipStart = TimeSpan.FromSeconds(1.0);
      animationClip.ClipEnd = TimeSpan.FromSeconds(3.0);
      animationClip.ClipOffset = TimeSpan.FromSeconds(-11.0);

      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();

      // Pre loop
      var expected = InterpolationHelper.Lerp(keyFrame0.Value, keyFrame1.Value, 0.75f);
      AssertExt.AreNumericallyEqual(expected, animationClip.GetValue(TimeSpan.FromSeconds(10.25), defaultSource, defaultTarget));

      expected = InterpolationHelper.Lerp(keyFrame1.Value, keyFrame2.Value, 0.25f);
			AssertExt.AreNumericallyEqual(expected, animationClip.GetValue(TimeSpan.FromSeconds(8.25), defaultSource, defaultTarget));

      // Post loop
      expected = InterpolationHelper.Lerp(keyFrame0.Value, keyFrame1.Value, 0.25f);
			AssertExt.AreNumericallyEqual(expected, animationClip.GetValue(TimeSpan.FromSeconds(14.75), defaultSource, defaultTarget));

      expected = InterpolationHelper.Lerp(keyFrame1.Value, keyFrame2.Value, 0.75f);
			AssertExt.AreNumericallyEqual(expected, animationClip.GetValue(TimeSpan.FromSeconds(16.75), defaultSource, defaultTarget));
    }


    [Test]
    public void CyclicAnimationWithZeroLength()
    {
      var keyFrame0 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(1.0), _random.NextQuaternion());
      var keyFrame1 = new KeyFrame<Quaternion>(TimeSpan.FromSeconds(1.0), _random.NextQuaternion());
      var animation = new QuaternionKeyFrameAnimation();
      animation.KeyFrames.Add(keyFrame0);
      animation.KeyFrames.Add(keyFrame1);

      var animationClip = new AnimationClip<Quaternion> { Animation = animation };
      animationClip.LoopBehavior = LoopBehavior.Cycle;
      animationClip.Duration = TimeSpan.MaxValue;

      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();

      // Just ensure that calls don't crash. (Ignore actual result because ambiguous.)
      Assert.That(() => { animationClip.GetValue(TimeSpan.FromSeconds(0.25), defaultSource, defaultTarget); }, Throws.Nothing);
      Assert.That(() => { animationClip.GetValue(TimeSpan.FromSeconds(4.75), defaultSource, defaultTarget); }, Throws.Nothing);           
    }
    #endregion    
  }
}
