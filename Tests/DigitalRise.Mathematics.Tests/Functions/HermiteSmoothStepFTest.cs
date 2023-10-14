using DigitalRise.Mathematics.Interpolation;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Mathematics.Functions.Tests
{
  [TestFixture]
  public class HermiteSmoothStepFTest
  {
    [Test]
    public void Compute()
    {
      AssertExt.AreNumericallyEqual(0, InterpolationHelper.HermiteSmoothStep(-1));
      AssertExt.AreNumericallyEqual(0, InterpolationHelper.HermiteSmoothStep(0));
      AssertExt.AreNumericallyEqual(0.5f, InterpolationHelper.HermiteSmoothStep(0.5f));
      AssertExt.AreNumericallyEqual(1, InterpolationHelper.HermiteSmoothStep(1));
      AssertExt.AreNumericallyEqual(1, InterpolationHelper.HermiteSmoothStep(2));
      AssertExt.AreNumericallyEqual(1 - InterpolationHelper.HermiteSmoothStep(1f-0.3f), InterpolationHelper.HermiteSmoothStep(0.3f));
      Assert.Greater(InterpolationHelper.HermiteSmoothStep(1f - 0.3f), InterpolationHelper.HermiteSmoothStep(0.3f));
    }
  }
}
