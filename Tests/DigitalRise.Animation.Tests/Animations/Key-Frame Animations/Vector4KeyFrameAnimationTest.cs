﻿using DigitalRise.Animation.Traits;
using NUnit.Framework;


namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class Vector4KeyFrameAnimationTest
  {
    [Test]
    public void TraitsTest()
    {
      var animationEx = new Vector4KeyFrameAnimation();
      Assert.AreEqual(Vector4Traits.Instance, animationEx.Traits);
    }
  }
}
