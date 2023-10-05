using System;
using DigitalRise.Animation.Traits;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using NUnit.Framework;


namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class SrtAnimationTest
  {
    [Test]
    public void TraitsTest()
    {
      var animationEx = new SrtAnimation();
      Assert.AreEqual(SrtTransformTraits.Instance, animationEx.Traits);
    }


    [Test]
    public void GetTotalDurationTest()
    {
      var animation = new AnimationClip<Vector3>
      {
        Animation = new Vector3FromToByAnimation
        {
          Duration = TimeSpan.FromSeconds(6.0),
        },
        Delay = TimeSpan.FromSeconds(10),
        Speed = 2,
        FillBehavior = FillBehavior.Hold,
      };

      var animation2 = new AnimationClip<Quaternion>
      {
        Animation = new QuaternionFromToByAnimation
        {
          Duration = TimeSpan.FromSeconds(5.0),
        },
        Delay = TimeSpan.Zero,
        Speed = 1,
        FillBehavior = FillBehavior.Hold,
      };

      var animationEx = new SrtAnimation();
      Assert.AreEqual(TimeSpan.FromSeconds(0.0), animationEx.GetTotalDuration());

      animationEx = new SrtAnimation();
      animationEx.Scale = animation;
      Assert.AreEqual(TimeSpan.FromSeconds(13.0), animationEx.GetTotalDuration());

      animationEx = new SrtAnimation();
      animationEx.Translation = animation;
      Assert.AreEqual(TimeSpan.FromSeconds(13.0), animationEx.GetTotalDuration());

      animationEx = new SrtAnimation();
      animationEx.Rotation = animation2;
      Assert.AreEqual(TimeSpan.FromSeconds(5.0), animationEx.GetTotalDuration());

      animationEx = new SrtAnimation();
      animationEx.Scale = animation;
      animationEx.Rotation = animation2;
      Assert.AreEqual(TimeSpan.FromSeconds(13.0), animationEx.GetTotalDuration());

      animationEx = new SrtAnimation();
      animationEx.Rotation = animation2;
      animationEx.Translation = animation;
      Assert.AreEqual(TimeSpan.FromSeconds(13.0), animationEx.GetTotalDuration());
    }
  }
}
