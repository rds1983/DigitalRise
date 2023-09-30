using DigitalRise.Animation.Traits;
using NUnit.Framework;


namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class QuaternionFKeyFrameAnimationTest
  {
    [Test]
    public void TraitsTest()
    {
      var animationEx = new QuaternionFKeyFrameAnimation();
      Assert.AreEqual(QuaternionFTraits.Instance, animationEx.Traits);
    }
  }
}
