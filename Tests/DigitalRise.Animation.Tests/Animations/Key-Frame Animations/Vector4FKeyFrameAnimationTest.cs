using DigitalRise.Animation.Traits;
using NUnit.Framework;


namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class Vector4FKeyFrameAnimationTest
  {
    [Test]
    public void TraitsTest()
    {
      var animationEx = new Vector4FKeyFrameAnimation();
      Assert.AreEqual(Vector4FTraits.Instance, animationEx.Traits);
    }
  }
}
