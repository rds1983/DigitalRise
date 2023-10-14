using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Graphics.Tests.Data
{
  [TestFixture]
  public class FogTest
  {
    [Test]
    public void DensitiesAndHeightFalloff()
    {
      var fog = new Fog();

      fog.Height0 = 0;
      fog.Height1 = 0;
      fog.Density = 123;
      Assert.AreEqual(fog.Density, fog.Density0);
      Assert.AreEqual(fog.Density, fog.Density1);

      fog.Height0 = -77;
      fog.Height1 = 123;
      fog.Density = 0.77f;
      fog.HeightFalloff = 0.023f;
      var d0 = fog.Density0;
      var d1 = fog.Density1;

      fog.Density = 123;
      fog.HeightFalloff = 0.12312321312f;

      fog.Density0 = d0;
      fog.Density1 = d1;
      AssertExt.AreNumericallyEqual(0.77f, fog.Density);
      AssertExt.AreNumericallyEqual(0.023f, fog.HeightFalloff);

      fog.Density = 12;
      fog.HeightFalloff = 0; 
      Assert.AreEqual(fog.Density0, fog.Density1);
    }
  }
}
