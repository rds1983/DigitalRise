using System;
using System.Collections.Generic;
using System.Diagnostics;
using DigitalRise.Mathematics.Algebra;
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
      StatisticsHelper.ComputeCovarianceMatrix((List<Vector3F>)null);
    }


    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ComputeCovarianceMatrix3DWithArgumentNull()
    {
      StatisticsHelper.ComputeCovarianceMatrix((List<Vector3F>)null);
    }


    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ComputeCovarianceMatrixFWithArgumentNull()
    {
      StatisticsHelper.ComputeCovarianceMatrix((List<VectorF>)null);
    }


    [Test]
    public void ComputeCovarianceMatrix3FWithEmptyList()
    {
      var result = StatisticsHelper.ComputeCovarianceMatrix(new List<Vector3F>());
      foreach (var element in result.ToList(MatrixOrder.RowMajor))
        Assert.IsNaN(element);
    }


    [Test]
    public void ComputeCovarianceMatrix3DWithEmptyList()
    {
      var result = StatisticsHelper.ComputeCovarianceMatrix(new List<Vector3F>());
      foreach (var element in result.ToList(MatrixOrder.RowMajor))
        Assert.IsNaN(element);
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ComputeCovarianceMatrixFWithEmptyList()
    {
      var result = StatisticsHelper.ComputeCovarianceMatrix(new List<VectorF>());
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ComputeCovarianceMatrixFWithVectorsOfDifferentLength()
    {
      List<VectorF> points = new List<VectorF>(new[]
      {
        new VectorF(new float[] { -1, -2, 1 }),
        new VectorF(new float[] { 2, -1, 3 }),
        new VectorF(new float[] { 2, -1, 2, 3 }),
      }); 
      var result = StatisticsHelper.ComputeCovarianceMatrix(points);
    }
  }
}