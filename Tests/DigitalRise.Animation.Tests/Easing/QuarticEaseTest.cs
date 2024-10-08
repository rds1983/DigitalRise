﻿using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Animation.Easing.Tests
{
  [TestFixture]
  public class QuarticEaseTest : BaseEasingFunctionTest<QuarticEase>
  {
    [SetUp]
    public void Setup()
    {
      EasingFunction = new QuarticEase();
    }


    [Test]
    public void EaseInTest()
    {
      EasingFunction.Mode = EasingMode.EaseIn;
      TestEase();
    }


    [Test]
    public void EaseOutTest()
    {
      EasingFunction.Mode = EasingMode.EaseOut;
      TestEase();
    }


    [Test]
    public void EaseInOutTest()
    {
      EasingFunction.Mode = EasingMode.EaseInOut;
      TestEase();

      // Check center.
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));
    }
  }
}

