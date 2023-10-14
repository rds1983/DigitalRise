using DigitalRise.Mathematics.Interpolation;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Mathematics.Functions.Tests
{
  [TestFixture]
  public class EaseInOutSmoothStepFTest
  {
    [Test]
    public void Compute()
    {
      AssertExt.AreNumericallyEqual(0, InterpolationHelper.EaseInOutSmoothStep(-1));
      AssertExt.AreNumericallyEqual(0, InterpolationHelper.EaseInOutSmoothStep(0));
      Assert.Greater(0.5f, InterpolationHelper.EaseInOutSmoothStep(0.3f));
      AssertExt.AreNumericallyEqual(0.5f, InterpolationHelper.EaseInOutSmoothStep(0.5f));
      Assert.Less(0.5f, InterpolationHelper.EaseInOutSmoothStep(0.6f));
      AssertExt.AreNumericallyEqual(1, InterpolationHelper.EaseInOutSmoothStep(1));
      AssertExt.AreNumericallyEqual(1, InterpolationHelper.EaseInOutSmoothStep(2));
    }
  }
}
