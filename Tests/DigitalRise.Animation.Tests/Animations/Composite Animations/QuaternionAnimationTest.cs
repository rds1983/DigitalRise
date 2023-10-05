using System;
using DigitalRise.Animation.Traits;
using Microsoft.Xna.Framework;
using NUnit.Framework;


namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class QuaternionAnimationTest
  {
    [Test]
    public void TraitsTest()
    {
      var animationEx = new QuaternionAnimation();
      Assert.AreEqual(QuaternionTraits.Instance, animationEx.Traits);
    }


    [Test]
    public void GetTotalDurationTest()
    {
      var animation = new AnimationClip<float>
      {
        Animation = new SingleFromToByAnimation
        {
          From = 100,
          To = 200,
          Duration = TimeSpan.FromSeconds(6.0),
        },
        Delay = TimeSpan.FromSeconds(10),
        Speed = 2,
        FillBehavior = FillBehavior.Hold,
      };

      var animation2 = new AnimationClip<float>
      {
        Animation = new SingleFromToByAnimation
        {
          From = 10,
          To = 20,
          Duration = TimeSpan.FromSeconds(5.0),
        },
        Delay = TimeSpan.Zero,
        Speed = 1,
        FillBehavior = FillBehavior.Hold,
      };

      var animationEx = new QuaternionAnimation();
      Assert.AreEqual(TimeSpan.FromSeconds(0.0), animationEx.GetTotalDuration());

      animationEx = new QuaternionAnimation();
      animationEx.W = animation;
      Assert.AreEqual(TimeSpan.FromSeconds(13.0), animationEx.GetTotalDuration());

      animationEx = new QuaternionAnimation();
      animationEx.X = animation;
      Assert.AreEqual(TimeSpan.FromSeconds(13.0), animationEx.GetTotalDuration());

      animationEx = new QuaternionAnimation();
      animationEx.Y = animation;
      Assert.AreEqual(TimeSpan.FromSeconds(13.0), animationEx.GetTotalDuration());

      animationEx = new QuaternionAnimation();
      animationEx.Z = animation;
      Assert.AreEqual(TimeSpan.FromSeconds(13.0), animationEx.GetTotalDuration());

      animationEx = new QuaternionAnimation();
      animationEx.W = animation;
      animationEx.X = animation2;
      Assert.AreEqual(TimeSpan.FromSeconds(13.0), animationEx.GetTotalDuration());

      animationEx = new QuaternionAnimation();
      animationEx.Y = animation2;
      animationEx.Z = animation;
      Assert.AreEqual(TimeSpan.FromSeconds(13.0), animationEx.GetTotalDuration());
    }


    [Test]
    public void GetValueTest()
    {
      var animation = new AnimationClip<float>
      {
        Animation = new SingleFromToByAnimation
        {
          From = 100,
          To = 200,
          Duration = TimeSpan.FromSeconds(6.0),
        },
        Delay = TimeSpan.FromSeconds(10),
        Speed = 2,
        FillBehavior = FillBehavior.Hold,
      };

      var animation2 = new AnimationClip<float>
      {
        Animation = new SingleFromToByAnimation
        {
          From = 10,
          To = 20,
          Duration = TimeSpan.FromSeconds(5.0),
        },
        Delay = TimeSpan.FromSeconds(0),
        Speed = 1,
        FillBehavior = FillBehavior.Hold,
      };


      var animation3 = new AnimationClip<float>
      {
        Animation = new SingleFromToByAnimation
        {
          From = 5,
          To = -5,
          Duration = TimeSpan.FromSeconds(10),
        },
        Delay = TimeSpan.FromSeconds(5),
        Speed = 1,
        FillBehavior = FillBehavior.Hold,
      };

      var animation4 = new AnimationClip<float>
      {
        Animation = new SingleFromToByAnimation
        {
          From = 1000,
          To = 1100,
          Duration = TimeSpan.FromSeconds(5.0),
        },
        Delay = TimeSpan.FromSeconds(5),
        Speed = 1,
        FillBehavior = FillBehavior.Stop,
      };

      var animationEx = new QuaternionAnimation
      {
        W = animation,
        X = animation2,
        Y = animation3,
        Z = animation4,
      };

      var defaultSource = new Quaternion(1, 2, 3, 4);
      var defaultTarget = new Quaternion(5, 6, 7, 8);

      var result = animationEx.GetValue(TimeSpan.FromSeconds(0.0), defaultSource, defaultTarget);
      Assert.AreEqual(defaultSource.W, result.W); // animation has not started.
      Assert.AreEqual(10.0f, result.X);           // animation2 has started.
      Assert.AreEqual(defaultSource.Y, result.Y); // animation3 has not started.
      Assert.AreEqual(defaultSource.Z, result.Z); // animation4 has not started.

      result = animationEx.GetValue(TimeSpan.FromSeconds(5.0), defaultSource, defaultTarget);
      Assert.AreEqual(defaultSource.W, result.W); // animation has not started.
      Assert.AreEqual(20.0f, result.X);           // animation2 has ended.
      Assert.AreEqual(5, result.Y);               // animation3 has started.
      Assert.AreEqual(1000, result.Z);            // animation4 has started.

      result = animationEx.GetValue(TimeSpan.FromSeconds(5.0), defaultSource, defaultTarget);
      Assert.AreEqual(defaultSource.W, result.W); // animation has not started.
      Assert.AreEqual(20.0f, result.X);           // animation2 has ended.
      Assert.AreEqual(5, result.Y);               // animation3 has started.
      Assert.AreEqual(1000, result.Z);            // animation4 has started.

      result = animationEx.GetValue(TimeSpan.FromSeconds(13.0), defaultSource, defaultTarget);
      Assert.AreEqual(200, result.W);             // animation has ended.
      Assert.AreEqual(20.0f, result.X);           // animation2 is filling.
      Assert.AreEqual(-3, result.Y);              // animation3 is active.
      Assert.AreEqual(defaultSource.Z, result.Z); // animation4 is stopped.
    }
  }
}
