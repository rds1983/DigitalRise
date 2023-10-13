using System;
using System.Collections.Generic;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using NUnit.Framework;


namespace DigitalRise.Mathematics.Statistics.Tests
{
  [TestFixture]
  public class StatisticsHelperTest
  {
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ComputeCovarianceMatrix3FWithArgumentNull()
    {
      StatisticsHelper.ComputeCovarianceMatrix((List<Vector3>)null);
    }


    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ComputeCovarianceMatrix3DWithArgumentNull()
    {
      StatisticsHelper.ComputeCovarianceMatrix((List<Vector3>)null);
    }


    [Test]
    public void ComputeCovarianceMatrix3FWithEmptyList()
    {
      var result = StatisticsHelper.ComputeCovarianceMatrix(new List<Vector3>());
      foreach (var element in result.ToList(MatrixOrder.RowMajor))
        Assert.IsNaN(element);
    }


    [Test]
    public void ComputeCovarianceMatrix3DWithEmptyList()
    {
      var result = StatisticsHelper.ComputeCovarianceMatrix(new List<Vector3>());
      foreach (var element in result.ToList(MatrixOrder.RowMajor))
        Assert.IsNaN(element);
    }
  }
}