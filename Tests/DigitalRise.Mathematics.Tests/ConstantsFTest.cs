using System;
using NUnit.Framework;
using NUnit.Utils;

namespace DigitalRise.Mathematics.Tests
{
  [TestFixture]
  public class ConstantsFTest
  {
    [Test]
    public void Constants()
    {
      AssertExt.AreNumericallyEqual((float)Math.E, ConstantsF.E);
      AssertExt.AreNumericallyEqual((float)Math.Log10(Math.E), ConstantsF.Log10OfE);
      AssertExt.AreNumericallyEqual((float)Math.Log(Math.E) / (float)Math.Log(2), ConstantsF.Log2OfE);
      AssertExt.AreNumericallyEqual(1 / (float)Math.PI, ConstantsF.OneOverPi);
      AssertExt.AreNumericallyEqual((float)Math.PI, ConstantsF.Pi);
      AssertExt.AreNumericallyEqual((float)Math.PI / 2f, ConstantsF.PiOver2);
      AssertExt.AreNumericallyEqual((float)Math.PI / 4f, ConstantsF.PiOver4);
      AssertExt.AreNumericallyEqual((float)Math.PI * 2f, ConstantsF.TwoPi);
    }
  }
}
