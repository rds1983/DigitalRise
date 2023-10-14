using System;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Animation.Easing.Tests
{
  [TestFixture]
  public class LogarithmicEaseTest : BaseEasingFunctionTest<LogarithmicEase>
  {
    [SetUp]
    public void Setup()
    {
      EasingFunction = new LogarithmicEase();
    }


    [Test]
    public void ShouldThrowWhenBaseIsInvalid()
    {
      Assert.That(() => EasingFunction.Base = -1, Throws.TypeOf<ArgumentOutOfRangeException>());
      Assert.That(() => EasingFunction.Base = 0, Throws.TypeOf<ArgumentOutOfRangeException>());
      Assert.That(() => EasingFunction.Base = 1, Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void EaseInTest()
    {
      EasingFunction.Mode = EasingMode.EaseIn;
      TestEase();

      EasingFunction.Base = 1.5f;
      TestEase();

      EasingFunction.Base = 4.0f;
      TestEase();

      EasingFunction.Base = 10.3f;
      TestEase();
    }


    [Test]
    public void EaseOutTest()
    {
      EasingFunction.Mode = EasingMode.EaseOut;
      TestEase();

      EasingFunction.Base = 1.5f;
      TestEase();

      EasingFunction.Base = 4.0f;
      TestEase();

      EasingFunction.Base = 10.3f;
      TestEase();
    }


    [Test]
    public void EaseInOutTest()
    {
      EasingFunction.Mode = EasingMode.EaseInOut;
      TestEase();

      // Check center.
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));

      EasingFunction.Base = 1.5f;
      TestEase();
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));

      EasingFunction.Base = 4.0f;
      TestEase();
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));

      EasingFunction.Base = 10.3f;
      TestEase();
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));
    }
  }
}

