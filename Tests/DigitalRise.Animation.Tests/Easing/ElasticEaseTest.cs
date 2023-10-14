using DigitalRise.Mathematics;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Animation.Easing.Tests
{
  [TestFixture]
  public class ElasticEaseTest : BaseEasingFunctionTest<ElasticEase>
  {
    [SetUp]
    public void Setup()
    {
      EasingFunction = new ElasticEase();
    }


    [Test]
    public void EaseInTest()
    {
      EasingFunction.Mode = EasingMode.EaseIn;
      TestEase();

      EasingFunction.Oscillations = 4;
      EasingFunction.Springiness = 4;
      TestEase();

      EasingFunction.Oscillations = 0;
      EasingFunction.Springiness = 0;
      TestEase();

      EasingFunction.Oscillations = -1;
      EasingFunction.Springiness = -1;
      TestEase();
    }


    [Test]
    public void EaseOutTest()
    {
      EasingFunction.Mode = EasingMode.EaseOut;
      TestEase();

      EasingFunction.Oscillations = 4;
      EasingFunction.Springiness = 4;
      TestEase();

      EasingFunction.Oscillations = 0;
      EasingFunction.Springiness = 0;
      TestEase();

      EasingFunction.Oscillations = -1;
      EasingFunction.Springiness = -1;
      TestEase();
    }


    [Test]
    public void EaseInOutTest()
    {
      EasingFunction.Mode = EasingMode.EaseInOut;
      TestEase();

      // Check center.
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));

      EasingFunction.Oscillations = 4;
      EasingFunction.Springiness = 4;
      TestEase();
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));

      EasingFunction.Oscillations = 0;
      EasingFunction.Springiness = 0;
      TestEase();
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));

      EasingFunction.Oscillations = -1;
      EasingFunction.Springiness = -1;
      TestEase();
      AssertExt.AreNumericallyEqual(0.5f, EasingFunction.Ease(0.5f));
    }
  }
}

