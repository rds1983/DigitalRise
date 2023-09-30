using DigitalRise.Mathematics.Interpolation;
using NUnit.Framework;


namespace DigitalRise.Mathematics.Functions.Tests
{
  [TestFixture]
  public class EaseInOutSmoothStepDTest
  {
    [Test]
    public void Compute()
    {
      Assert.IsTrue(Numeric.AreEqual(0, InterpolationHelper.EaseInOutSmoothStep(-1)));
      Assert.IsTrue(Numeric.AreEqual(0, InterpolationHelper.EaseInOutSmoothStep(0)));
      Assert.Greater(0.5, InterpolationHelper.EaseInOutSmoothStep(0.3));
      Assert.IsTrue(Numeric.AreEqual(0.5, InterpolationHelper.EaseInOutSmoothStep(0.5)));
      Assert.Less(0.5, InterpolationHelper.EaseInOutSmoothStep(0.6));
      Assert.IsTrue(Numeric.AreEqual(1, InterpolationHelper.EaseInOutSmoothStep(1)));
      Assert.IsTrue(Numeric.AreEqual(1, InterpolationHelper.EaseInOutSmoothStep(2)));
    }
  }
}
