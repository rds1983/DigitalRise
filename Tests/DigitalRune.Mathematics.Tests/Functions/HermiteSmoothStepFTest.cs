using DigitalRune.Mathematics.Interpolation;
using NUnit.Framework;


namespace DigitalRune.Mathematics.Functions.Tests
{
  [TestFixture]
  public class HermiteSmoothStepFTest
  {
    [Test]
    public void Compute()
    {
      Assert.IsTrue(Numeric.AreEqual(0, InterpolationHelper.HermiteSmoothStep(-1)));
      Assert.IsTrue(Numeric.AreEqual(0, InterpolationHelper.HermiteSmoothStep(0)));
      Assert.IsTrue(Numeric.AreEqual(0.5f, InterpolationHelper.HermiteSmoothStep(0.5f)));
      Assert.IsTrue(Numeric.AreEqual(1, InterpolationHelper.HermiteSmoothStep(1)));
      Assert.IsTrue(Numeric.AreEqual(1, InterpolationHelper.HermiteSmoothStep(2)));
      Assert.IsTrue(Numeric.AreEqual(1 - InterpolationHelper.HermiteSmoothStep(1f-0.3f), InterpolationHelper.HermiteSmoothStep(0.3f)));
      Assert.Greater(InterpolationHelper.HermiteSmoothStep(1f - 0.3f), InterpolationHelper.HermiteSmoothStep(0.3f));
    }
  }
}
