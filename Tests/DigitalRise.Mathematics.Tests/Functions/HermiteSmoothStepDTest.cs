using DigitalRise.Mathematics.Interpolation;
using NUnit.Framework;


namespace DigitalRise.Mathematics.Functions.Tests
{
  [TestFixture]
  public class HermiteSmoothStepDTest
  {
    [Test]
    public void Compute()
    {
      Assert.IsTrue(Numeric.AreEqual(0, InterpolationHelper.HermiteSmoothStep(-1)));
      Assert.IsTrue(Numeric.AreEqual(0, InterpolationHelper.HermiteSmoothStep(0)));
      Assert.IsTrue(Numeric.AreEqual(0.5, InterpolationHelper.HermiteSmoothStep(0.5)));
      Assert.IsTrue(Numeric.AreEqual(1, InterpolationHelper.HermiteSmoothStep(1)));
      Assert.IsTrue(Numeric.AreEqual(1, InterpolationHelper.HermiteSmoothStep(2)));
      Assert.IsTrue(Numeric.AreEqual(1 - InterpolationHelper.HermiteSmoothStep(1-0.3), InterpolationHelper.HermiteSmoothStep(0.3)));
      Assert.Greater(InterpolationHelper.HermiteSmoothStep(1 - 0.3), InterpolationHelper.HermiteSmoothStep(0.3));
    }
  }
}
