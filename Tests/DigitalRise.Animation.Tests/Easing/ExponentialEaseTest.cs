using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Animation.Easing.Tests
{
  [TestFixture]
  public class ExponentialEaseTest : BaseEasingFunctionTest<ExponentialEase>
  {
    [SetUp]
    public void Setup()
    {
      EasingFunction = new ExponentialEase();
    }


    [Test]
    public void EaseInTest()
    {
      EasingFunction.Mode = EasingMode.EaseIn;
      TestEase();

      EasingFunction.Exponent = 3.5f;
      TestEase();

      EasingFunction.Exponent = 0.0f;
      TestEase();

      EasingFunction.Exponent = -2.3f;
      TestEase();
    }


    [Test]
    public void EaseOutTest()
    {
      EasingFunction.Mode = EasingMode.EaseOut;
      TestEase();

      EasingFunction.Exponent = 3.5f;
      TestEase();

      EasingFunction.Exponent = 0.0f;
      TestEase();

      EasingFunction.Exponent = -2.3f;
      TestEase();
    }


    [Test]
    public void EaseInOutTest()
    {
      EasingFunction.Mode = EasingMode.EaseInOut;
      TestEase();

			// Check center.
			AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));

      EasingFunction.Exponent = 3.5f;
      TestEase();
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));

      EasingFunction.Exponent = 0.0f;
      TestEase();
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));

      EasingFunction.Exponent = -2.3f;
      TestEase();
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));
    }
  }
}

