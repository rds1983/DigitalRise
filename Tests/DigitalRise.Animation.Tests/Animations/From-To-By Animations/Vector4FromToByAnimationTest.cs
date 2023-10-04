using System;
using DigitalRise.Mathematics;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class Vector4FromToByAnimationTest
  {
    [Test]
    public void CheckDefaultValues()
    {
      var animation = new Vector4FromToByAnimation();
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
      var animation = new Vector4FromToByAnimation();
      animation.From = null;
      animation.To = null;
      animation.By = null;
      Assert.AreEqual(new Vector4(0.0f, 0.0f, 0.0f, 0.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.5f, 5.0f, 50.0f, 500.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(1.0f, 10.0f, 100.0f, 1000.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
    }


    [Test]
    public void AnimateFrom()
    {
      var animation = new Vector4FromToByAnimation();
      animation.From = new Vector4(0.2f, 2.0f, 20.0f, 200.0f);
      animation.To = null;
      animation.By = null;
      Assert.AreEqual(new Vector4(0.2f, 2.0f, 20.0f, 200.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector4(0.0f, 1.0f, 10.0f, 100.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.1f, 1.5f, 15.0f, 150.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector4(0.0f, 1.0f, 10.0f, 100.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.0f, 1.0f, 10.0f, 100.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector4(0.0f, 1.0f, 10.0f, 100.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
    }


    [Test]
    public void AnimateTo()
    {
      var animation = new Vector4FromToByAnimation();
      animation.From = null;
      animation.To = new Vector4(0.2f, 2.0f, 20.0f, 200.0f);
      animation.By = null;
      Assert.AreEqual(new Vector4(0.0f, 0.0f, 0.0f, 0.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.1f, 1.0f, 10.0f, 100.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.2f, 2.0f, 20.0f, 200.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
    }


    [Test]
    public void AnimateFromTo()
    {
      var animation = new Vector4FromToByAnimation();
      animation.From = new Vector4(0.2f, 2.0f, 20.0f, 200.0f);
      animation.To = new Vector4(0.4f, 4.0f, 40.0f, 400.0f);
      animation.By = null;
      Assert.AreEqual(new Vector4(0.2f, 2.0f, 20.0f, 200.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.3f, 3.0f, 30.0f, 300.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.4f, 4.0f, 40.0f, 400.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));

      animation.By = new Vector4(1.0f, 10.0f, 100.0f, 1000.0f);
      Assert.AreEqual(new Vector4(0.2f, 2.0f, 20.0f, 200.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.3f, 3.0f, 30.0f, 300.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.4f, 4.0f, 40.0f, 400.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
    }


    [Test]
    public void ShouldIgnoreByIfFromToIsSet()
    {
      var animation = new Vector4FromToByAnimation();
      animation.From = new Vector4(0.2f, 2.0f, 20.0f, 200.0f);
      animation.To = new Vector4(0.4f, 4.0f, 40.0f, 400.0f);
      animation.By = new Vector4(1.0f, 10.0f, 100.0f, 1000.0f);
      Assert.AreEqual(new Vector4(0.2f, 2.0f, 20.0f, 200.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.3f, 3.0f, 30.0f, 300.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.4f, 4.0f, 40.0f, 400.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
    }


    [Test]
    public void ShouldIgnoreByIfToIsSet()
    {
      var animation = new Vector4FromToByAnimation();
      animation.From = null;
      animation.To = new Vector4(0.4f, 4.0f, 40.0f, 400.0f);
      animation.By = new Vector4(1.0f, 10.0f, 100.0f, 1000.0f);
      Assert.AreEqual(new Vector4(0.0f, 0.0f, 0.0f, 0.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.2f, 2.0f, 20.0f, 200.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.4f, 4.0f, 40.0f, 400.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
    }


    [Test]
    public void AnimateFromBy()
    {
      var animation = new Vector4FromToByAnimation();
      animation.From = new Vector4(0.2f, 2.0f, 20.0f, 200.0f);
      animation.To = null;
      animation.By = new Vector4(1.0f, 10.0f, 100.0f, 1000.0f);
      Assert.AreEqual(new Vector4(0.2f, 2.0f, 20.0f, 200.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.IsTrue(MathHelper.AreNumericallyEqual(new Vector4(0.7f, 7.0f, 70.0f, 700.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f))));
      Assert.AreEqual(new Vector4(1.2f, 12.0f, 120.0f, 1200.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));

      animation.By = new Vector4(-0.1f, -1.0f, -10.0f, -100.0f);
      Assert.AreEqual(new Vector4(0.2f, 2.0f, 20.0f, 200.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.15f, 1.5f, 15.0f, 150.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.1f, 1.0f, 10.0f, 100.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
    }


    [Test]
    public void AnimateBy()
    {
      var animation = new Vector4FromToByAnimation();
      animation.From = null;
      animation.To = null;
      animation.By = new Vector4(0.1f, 1.0f, 10.0f, 100.0f);
      Assert.AreEqual(new Vector4(0.0f, 0.0f, 0.0f, 0.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.05f, 0.5f, 5.0f, 50.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(0.1f, 1.0f, 10.0f, 100.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));

      animation.By = new Vector4(-0.1f, -1.0f, -10.0f, -100.0f);
      Assert.AreEqual(new Vector4(0.0f, 0.0f, 0.0f, 0.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(-0.05f, -0.5f, -5.0f, -50.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
      Assert.AreEqual(new Vector4(-0.1f, -1.0f, -10.0f, -100.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(1.0f, 10.0f, 100.0f, 1000.0f)));
    }
  }
}
