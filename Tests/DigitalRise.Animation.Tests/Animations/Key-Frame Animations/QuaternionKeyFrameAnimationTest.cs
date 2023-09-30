using DigitalRise.Animation.Traits;
using NUnit.Framework;


namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class QuaternionKeyFrameAnimationTest
  {
    [Test]
    public void TraitsTest()
    {
      var animationEx = new QuaternionKeyFrameAnimation();
      Assert.AreEqual(QuaternionTraits.Instance, animationEx.Traits);
    }
  }
}
