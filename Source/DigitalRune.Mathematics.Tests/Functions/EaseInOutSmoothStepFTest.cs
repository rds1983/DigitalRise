using DigitalRune.Mathematics.Interpolation;
using NUnit.Framework;


namespace DigitalRune.Mathematics.Functions.Tests
{
  [TestFixture]
  public class EaseInOutSmoothStepFTest
  {
    [Test]
    public void Compute()
    {
      Assert.IsTrue(Numeric.AreEqual(0, InterpolationHelper.EaseInOutSmoothStep(-1)));
      Assert.IsTrue(Numeric.AreEqual(0, InterpolationHelper.EaseInOutSmoothStep(0)));
      Assert.Greater(0.5f, InterpolationHelper.EaseInOutSmoothStep(0.3f));
      Assert.IsTrue(Numeric.AreEqual(0.5f, InterpolationHelper.EaseInOutSmoothStep(0.5f)));
      Assert.Less(0.5f, InterpolationHelper.EaseInOutSmoothStep(0.6f));
      Assert.IsTrue(Numeric.AreEqual(1, InterpolationHelper.EaseInOutSmoothStep(1)));
      Assert.IsTrue(Numeric.AreEqual(1, InterpolationHelper.EaseInOutSmoothStep(2)));
    }
  }
}
