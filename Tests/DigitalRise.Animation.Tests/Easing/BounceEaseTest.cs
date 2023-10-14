using DigitalRise.Mathematics;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Animation.Easing.Tests
{
  [TestFixture]
  public class BounceEaseTest : BaseEasingFunctionTest<BounceEase>
  {
    [SetUp]
    public void Setup()
    {
      EasingFunction = new BounceEase();
    }


    [Test]
    public void EaseInTest()
    {
      EasingFunction.Mode = EasingMode.EaseIn;
      TestEase();

      EasingFunction.Bounces = 4;
      EasingFunction.Bounciness = 4;
      TestEase();

      EasingFunction.Bounces = 0;
      EasingFunction.Bounciness = 1;
      TestEase();

      EasingFunction.Bounces = -1;
      EasingFunction.Bounciness = 0;
      TestEase();
    }


    [Test]
    public void EaseOutTest()
    {
      EasingFunction.Mode = EasingMode.EaseOut;
      TestEase();

      EasingFunction.Bounces = 4;
      EasingFunction.Bounciness = 4;
      TestEase();

      EasingFunction.Bounces = 0;
      EasingFunction.Bounciness = 1;
      TestEase();

      EasingFunction.Bounces = -1;
      EasingFunction.Bounciness = 0;
      TestEase();
    }


    [Test]
    public void EaseInOutTest()
    {
      EasingFunction.Mode = EasingMode.EaseInOut;
      TestEase();

      // Check center.
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));

      EasingFunction.Bounces = 4;
      EasingFunction.Bounciness = 4;
      TestEase();
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));

      EasingFunction.Bounces = 0;
      EasingFunction.Bounciness = 1;
      TestEase();
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));

      EasingFunction.Bounces = -1;
      EasingFunction.Bounciness = 0;
      TestEase();
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));
    }
  }
}

