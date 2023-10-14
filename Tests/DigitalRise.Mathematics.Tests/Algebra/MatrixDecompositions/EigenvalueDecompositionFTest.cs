using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;


namespace DigitalRise.Mathematics.Algebra.Tests
{
  [TestFixture]
  public class EigenvalueDecompositionFTest
  {
    [Test]
    public void Test1()
    {
      Matrix33F a = new Matrix33F(new float[,] {{ 1, -1, 4 }, 
                                           { 3, 2, -1 },
                                           { 2, 1, -1}});
      EigenvalueDecompositionF d = new EigenvalueDecompositionF(a);

      AssertExt.AreNumericallyEqual(a * d.V, d.V * d.D);
    }


    [Test]
    public void Test2()
    {
      Matrix33F a = new Matrix33F(new float[,] {{ 0, 1, 2 }, 
                                           { 1, 4, 3 },
                                           { 2, 3, 5}});
      EigenvalueDecompositionF d = new EigenvalueDecompositionF(a);

      AssertExt.AreNumericallyEqual(a, d.V * d.D * d.V.Transposed);
    }

    private static bool IsNaN(Vector3 v)
    {
			return float.IsNaN(v.X) && float.IsNaN(v.Y) && float.IsNaN(v.Z);
    }

    private static bool IsNaN(Matrix33F v)
    {
      for(var i = 0; i < 9; ++i)
      {
        if (!float.IsNaN(v[i]))
        {
          return false;
        }
      }

      return true;
    }


    [Test]
    public void TestWithNaNValues()
    {
      Matrix33F a = new Matrix33F(new [,] {{ 0, float.NaN, 2 }, 
                                       { 1, 4, 3 },
                                        { 2, 3, 5}});
      
      var d = new EigenvalueDecompositionF(a);
      Assert.IsTrue(IsNaN(d.RealEigenvalues));
			Assert.IsTrue(IsNaN(d.ImaginaryEigenvalues));
      Assert.IsTrue(IsNaN(d.V));

      d = new EigenvalueDecompositionF(new Matrix33F(float.NaN));
			Assert.IsTrue(IsNaN(d.RealEigenvalues));
			Assert.IsTrue(IsNaN(d.ImaginaryEigenvalues));
			Assert.IsTrue(IsNaN(d.V));
    }
  }
}
