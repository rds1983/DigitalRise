using System;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Interpolation;
using DigitalRise.Mathematics.Statistics;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class QuaternionFromToByAnimationTest
  {
    private Random _random;


    [SetUp]
    public void Setup()
    {
      _random = new Random(12345);
    }


    [Test]
    public void CheckDefaultValues()
    {
      var animation = new QuaternionFromToByAnimation();
      Assert.AreEqual(TimeSpan.FromSeconds(1.0), animation.Duration);
      Assert.AreEqual(FillBehavior.Hold, animation.FillBehavior);
      Assert.IsNull(animation.TargetProperty);
      Assert.IsFalse(animation.From.HasValue);
      Assert.IsFalse(animation.To.HasValue);
      Assert.IsFalse(animation.By.HasValue);
      Assert.IsFalse(animation.IsAdditive);
      Assert.IsNull(animation.EasingFunction);
    }


    [Test]
    public void AnimateUsingDefaults()
    {
      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();

      var animation = new QuaternionFromToByAnimation();
      animation.From = null;
      animation.To = null;
      animation.By = null;
      Assert.AreEqual(defaultSource, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(InterpolationHelper.Lerp(defaultSource, defaultTarget, 0.75f), animation.GetValue(TimeSpan.FromSeconds(0.75), defaultSource, defaultTarget));
      Assert.AreEqual(defaultTarget, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));
    }


    [Test]
    public void AnimateFrom()
    {
      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();
      var from = _random.NextQuaternion();

      var animation = new QuaternionFromToByAnimation();
      animation.From = from;
      animation.To = null;
      animation.By = null;
      Assert.AreEqual(from, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(InterpolationHelper.Lerp(from, defaultSource, 0.75f), animation.GetValue(TimeSpan.FromSeconds(0.75), defaultSource, defaultTarget));
      Assert.AreEqual(defaultSource, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));
    }


    [Test]
    public void AnimateTo()
    {
      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();
      var to = _random.NextQuaternion();

      var animation = new QuaternionFromToByAnimation();
      animation.From = null;
      animation.To = to;
      animation.By = null;
      Assert.AreEqual(defaultSource, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(InterpolationHelper.Lerp(defaultSource, to, 0.75f), animation.GetValue(TimeSpan.FromSeconds(0.75), defaultSource, defaultTarget));
      Assert.AreEqual(to, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));
    }


    [Test]
    public void AnimateFromTo()
    {
      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();
      var from = _random.NextQuaternion();
      var to = _random.NextQuaternion();
      var by = _random.NextQuaternion();

      var animation = new QuaternionFromToByAnimation();
      animation.From = from;
      animation.To = to;
      animation.By = null;
      Assert.AreEqual(from, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(InterpolationHelper.Lerp(from, to, 0.75f), animation.GetValue(TimeSpan.FromSeconds(0.75), defaultSource, defaultTarget));
      Assert.AreEqual(to, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));
    }


    [Test]
    public void ShouldIgnoreByIfFromToIsSet()
    {
      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();
      var from = _random.NextQuaternion();
      var to = _random.NextQuaternion();
      var by = _random.NextQuaternion();

      var animation = new QuaternionFromToByAnimation();
      animation.From = from;
      animation.To = to;
      animation.By = by;
      Assert.AreEqual(from, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(InterpolationHelper.Lerp(from, to, 0.75f), animation.GetValue(TimeSpan.FromSeconds(0.75), defaultSource, defaultTarget));
      Assert.AreEqual(to, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));
    }


    [Test]
    public void ShouldIgnoreByIfToIsSet()
    {
      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();
      var to = _random.NextQuaternion();
      var by = _random.NextQuaternion();

      var animation = new QuaternionFromToByAnimation();
      animation.From = null;
      animation.To = to;
      animation.By = by;
      Assert.AreEqual(defaultSource, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(InterpolationHelper.Lerp(defaultSource, to, 0.75f), animation.GetValue(TimeSpan.FromSeconds(0.75), defaultSource, defaultTarget));
      Assert.AreEqual(to, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));
    }


    [Test]
    public void AnimateFromBy()
    {
      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();
      var from = _random.NextQuaternion();
      var by = _random.NextQuaternion();

      var animation = new QuaternionFromToByAnimation();
      animation.From = from;
      animation.To = null;
      animation.By = by;
      Assert.AreEqual(from, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(InterpolationHelper.Lerp(from, by * from, 0.75f), animation.GetValue(TimeSpan.FromSeconds(0.75), defaultSource, defaultTarget));
      Assert.AreEqual(by * from, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));

      animation.By = by.Inverse();
      Assert.AreEqual(from, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(InterpolationHelper.Lerp(from, by.Inverse() * from, 0.75f), animation.GetValue(TimeSpan.FromSeconds(0.75), defaultSource, defaultTarget));
      Assert.AreEqual(by.Inverse() * from, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));
    }


    [Test]
    public void AnimateBy()
    {
      var defaultSource = _random.NextQuaternion();
      var defaultTarget = _random.NextQuaternion();
      var by = _random.NextQuaternion();

      var animation = new QuaternionFromToByAnimation();
      animation.From = null;
      animation.To = null;
      animation.By = by;
      Assert.AreEqual(defaultSource, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(InterpolationHelper.Lerp(defaultSource, by * defaultSource, 0.75f), animation.GetValue(TimeSpan.FromSeconds(0.75), defaultSource, defaultTarget));
      Assert.AreEqual(by * defaultSource, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));

      animation.By = by.Inverse();
      Assert.AreEqual(defaultSource, animation.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget));
      AssertExt.AreNumericallyEqual(InterpolationHelper.Lerp(defaultSource, by.Inverse() * defaultSource, 0.75f), animation.GetValue(TimeSpan.FromSeconds(0.75), defaultSource, defaultTarget));
      Assert.AreEqual(by.Inverse() * defaultSource, animation.GetValue(TimeSpan.FromSeconds(1.0), defaultSource, defaultTarget));
    }
  }
}