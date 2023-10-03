using System;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using NUnit.Framework;


namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class Vector2FromToByAnimationTest
  {
    [Test]
    public void CheckDefaultValues()
    {
      var animation = new Vector2FromToByAnimation();
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
      var animation = new Vector2FromToByAnimation();
      animation.From = null;
      animation.To = null;
      animation.By = null;
      Assert.AreEqual(new Vector2(0.0f, 0.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(5.0f, 50.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(10.0f, 100.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
    }


    [Test]
    public void AnimateFrom()
    {
      var animation = new Vector2FromToByAnimation();
      animation.From = new Vector2(2.0f, 20.0f);
      animation.To = null;
      animation.By = null;
      Assert.AreEqual(new Vector2(2.0f, 20.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector2(1.0f, 10.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(1.5f, 15.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector2(1.0f, 10.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(1.0f, 10.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector2(1.0f, 10.0f), new Vector2(10.0f, 100.0f)));
    }


    [Test]
    public void AnimateTo()
    {
      var animation = new Vector2FromToByAnimation();
      animation.From = null;
      animation.To = new Vector2(2.0f, 20.0f);
      animation.By = null;
      Assert.AreEqual(new Vector2(0.0f, 0.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(1.0f, 10.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(2.0f, 20.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
    }


    [Test]
    public void AnimateFromTo()
    {
      var animation = new Vector2FromToByAnimation();
      animation.From = new Vector2(2.0f, 20.0f);
      animation.To = new Vector2(4.0f, 40.0f);
      animation.By = null;
      Assert.AreEqual(new Vector2(2.0f, 20.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(3.0f, 30.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(4.0f, 40.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));

      animation.By = new Vector2(10.0f, 100.0f);
      Assert.AreEqual(new Vector2(2.0f, 20.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(3.0f, 30.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(4.0f, 40.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
    }


    [Test]
    public void ShouldIgnoreByIfFromToIsSet()
    {
      var animation = new Vector2FromToByAnimation();
      animation.From = new Vector2(2.0f, 20.0f);
      animation.To = new Vector2(4.0f, 40.0f);
      animation.By = new Vector2(10.0f, 100.0f);
      Assert.AreEqual(new Vector2(2.0f, 20.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(3.0f, 30.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(4.0f, 40.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
    }


    [Test]
    public void ShouldIgnoreByIfToIsSet()
    {
      var animation = new Vector2FromToByAnimation();
      animation.From = null;
      animation.To = new Vector2(4.0f, 40.0f);
      animation.By = new Vector2(10.0f, 100.0f);
      Assert.AreEqual(new Vector2(0.0f, 0.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(2.0f, 20.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(4.0f, 40.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
    }


    [Test]
    public void AnimateFromBy()
    {
      var animation = new Vector2FromToByAnimation();
      animation.From = new Vector2(2.0f, 20.0f);
      animation.To = null;
      animation.By = new Vector2(10.0f, 100.0f);
      Assert.AreEqual(new Vector2(2.0f, 20.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(7.0f, 70.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(12.0f, 120.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));

      animation.By = new Vector2(-1.0f, -10.0f);
      Assert.AreEqual(new Vector2(2.0f, 20.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(1.5f, 15.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(1.0f, 10.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
    }


    [Test]
    public void AnimateBy()
    {
      var animation = new Vector2FromToByAnimation();
      animation.From = null;
      animation.To = null;
      animation.By = new Vector2(1.0f, 10.0f);
      Assert.AreEqual(new Vector2(0.0f, 0.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(0.5f, 5.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(1.0f, 10.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));

      animation.By = new Vector2(-1.0f, -10.0f);
      Assert.AreEqual(new Vector2(0.0f, 0.0f), animation.GetValue(TimeSpan.FromSeconds(0.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(-0.5f, -5.0f), animation.GetValue(TimeSpan.FromSeconds(0.5), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
      Assert.AreEqual(new Vector2(-1.0f, -10.0f), animation.GetValue(TimeSpan.FromSeconds(1.0), new Vector2(0.0f, 0.0f), new Vector2(10.0f, 100.0f)));
    }
  }
}
