﻿using System;
using NUnit.Framework;


namespace DigitalRise.Mathematics.Analysis.Tests
{
  [TestFixture]
  public class ImprovedNewtonRaphsonMethodDTest
  {
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ConstructorShouldThrowWhenFirstParamIsNull()
    {
      new ImprovedNewtonRaphsonMethodD(null, x => x);
    }


    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ConstructorShouldThrowWhenSecondParamIsNull()
    {
      new ImprovedNewtonRaphsonMethodD(x => x, null);
    }


    [Test]
    public void FindRootTest1()
    {
      Func<double, double> polynomial = x => (x - 1)*(x - 10)*(x - 18);
      Func<double, double> computeDerivative = x => 3 * x * x - 58 * x + 208;

      ImprovedNewtonRaphsonMethodD rootFinder = new ImprovedNewtonRaphsonMethodD(polynomial, computeDerivative);

      rootFinder.EpsilonX = Numeric.EpsilonD / 100;

      double xRoot = rootFinder.FindRoot(0, 2);
      Assert.IsTrue(Numeric.AreEqual(0, polynomial(xRoot)));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      xRoot = rootFinder.FindRoot(4, 10);
      Assert.IsTrue(Numeric.AreEqual(0, polynomial(xRoot)));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      xRoot = rootFinder.FindRoot(10, 4);
      Assert.IsTrue(Numeric.AreEqual(0, polynomial(xRoot)));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      xRoot = rootFinder.FindRoot(10, 12);
      Assert.IsTrue(Numeric.AreEqual(0, polynomial(xRoot)));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      xRoot = rootFinder.FindRoot(2, 0);
      Assert.IsTrue(Numeric.AreEqual(0, polynomial(xRoot)));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      xRoot = rootFinder.FindRoot(0, 3);
      Assert.IsTrue(Numeric.AreEqual(0, polynomial(xRoot)));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      xRoot = rootFinder.FindRoot(3, 0);
      Assert.IsTrue(Numeric.AreEqual(0, polynomial(xRoot)));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      xRoot = rootFinder.FindRoot(-6, 2);
      Assert.IsTrue(Numeric.AreEqual(0, polynomial(xRoot)));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      xRoot = rootFinder.FindRoot(-1, 1);
      Assert.IsTrue(Numeric.AreEqual(0, polynomial(xRoot)));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      xRoot = rootFinder.FindRoot(-1, 1);
      Assert.IsTrue(Numeric.AreEqual(0, polynomial(xRoot)));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      xRoot = rootFinder.FindRoot(0.9, 9.9);
      Assert.IsTrue(Numeric.AreEqual(0, polynomial(xRoot)));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      xRoot = rootFinder.FindRoot(2, 3);
      Assert.IsTrue(double.IsNaN(xRoot));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      xRoot = rootFinder.FindRoot(3, 2);
      Assert.IsTrue(double.IsNaN(xRoot));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);

      rootFinder.MaxNumberOfIterations = 1;
      xRoot = rootFinder.FindRoot(0, 1000);
      Assert.IsTrue(double.IsNaN(xRoot));
      Console.WriteLine("NumberOfIterations: {0}", rootFinder.NumberOfIterations);
    }
  }
}
