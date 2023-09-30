using DigitalRise.Animation.Traits;
using NUnit.Framework;


namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class Vector2KeyFrameAnimationTest
  {
    [Test]
    public void TraitsTest()
    {
      var animationEx = new Vector2KeyFrameAnimation();
      Assert.AreEqual(Vector2Traits.Instance, animationEx.Traits);
    }
  }
}
