using System;
using System.Collections.Generic;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using NUnit.Utils;



namespace DigitalRise.Mathematics.Interpolation.Tests
{
  [TestFixture]
  public class InterpolationHelperTest
  {
    [Test]
    public void Lerp()
    {
      Assert.AreEqual(1.0f, InterpolationHelper.Lerp(1.0f, 2.0f, 0.0f));
      Assert.AreEqual(1.5f, InterpolationHelper.Lerp(1.0f, 2.0f, 0.5f));
      Assert.AreEqual(2.0f, InterpolationHelper.Lerp(1.0f, 2.0f, 1.0f));
      Assert.AreEqual(1.5f, InterpolationHelper.Lerp(2.0f, 1.0f, 0.5f));

      Assert.AreEqual(1.0, InterpolationHelper.Lerp(1.0, 2.0, 0.0));
      Assert.AreEqual(1.5, InterpolationHelper.Lerp(1.0, 2.0, 0.5));
      Assert.AreEqual(2.0, InterpolationHelper.Lerp(1.0, 2.0, 1.0));
      Assert.AreEqual(1.5, InterpolationHelper.Lerp(2.0, 1.0, 0.5));
    }


    [Test]
    public void LerpVector2()
    {
      Vector2 v = new Vector2(1.0f, 10.0f);
      Vector2 w = new Vector2(2.0f, 20.0f);
      Vector2 lerp0 = InterpolationHelper.Lerp(v, w, 0.0f);
      Vector2 lerp1 = InterpolationHelper.Lerp(v, w, 1.0f);
      Vector2 lerp05 = InterpolationHelper.Lerp(v, w, 0.5f);
      Vector2 lerp025 = InterpolationHelper.Lerp(v, w, 0.25f);

      Assert.AreEqual(v, lerp0);
      Assert.AreEqual(w, lerp1);
      Assert.AreEqual(new Vector2(1.5f, 15.0f), lerp05);
      Assert.AreEqual(new Vector2(1.25f, 12.5f), lerp025);
    }


    [Test]
    public void LerpVector3()
    {
      Vector3 v = new Vector3(1.0f, 10.0f, 100.0f);
      Vector3 w = new Vector3(2.0f, 20.0f, 200.0f);
      Vector3 lerp0 = InterpolationHelper.Lerp(v, w, 0.0f);
      Vector3 lerp1 = InterpolationHelper.Lerp(v, w, 1.0f);
      Vector3 lerp05 = InterpolationHelper.Lerp(v, w, 0.5f);
      Vector3 lerp025 = InterpolationHelper.Lerp(v, w, 0.25f);

      Assert.AreEqual(v, lerp0);
      Assert.AreEqual(w, lerp1);
      Assert.AreEqual(new Vector3(1.5f, 15.0f, 150.0f), lerp05);
      Assert.AreEqual(new Vector3(1.25f, 12.5f, 125.0f), lerp025);
    }


    [Test]
    public void LerpVector4()
    {
      Vector4 v = new Vector4(1.0f, 10.0f, 100.0f, 1000.0f);
      Vector4 w = new Vector4(2.0f, 20.0f, 200.0f, 2000.0f);
      Vector4 lerp0 = InterpolationHelper.Lerp(v, w, 0.0f);
      Vector4 lerp1 = InterpolationHelper.Lerp(v, w, 1.0f);
      Vector4 lerp05 = InterpolationHelper.Lerp(v, w, 0.5f);
      Vector4 lerp025 = InterpolationHelper.Lerp(v, w, 0.25f);

      Assert.AreEqual(v, lerp0);
      Assert.AreEqual(w, lerp1);
      Assert.AreEqual(new Vector4(1.5f, 15.0f, 150.0f, 1500.0f), lerp05);
      Assert.AreEqual(new Vector4(1.25f, 12.5f, 125.0f, 1250.0f), lerp025);
    }


    [Test]
    public void LerpQuaternion()
    {
      // Warning: The not all results are not verified
      Quaternion q1 = new Quaternion(2.0f, 3.0f, 4.0f, 1.0f).Normalized();
      Quaternion q2 = new Quaternion(4.0f, 6.0f, 8.0f, 2.0f).Normalized();
      Quaternion lerp = InterpolationHelper.Lerp(q1, q2, 0.75f);
      Assert.IsTrue(lerp.IsNumericallyNormalized());

      lerp = InterpolationHelper.Lerp(q1, q2, 0);
      AssertExt.AreNumericallyEqual(q1, lerp);

      lerp = InterpolationHelper.Lerp(q1, q2, 1);
      AssertExt.AreNumericallyEqual(q2, lerp);

      q1 = Quaternion.Identity;
      q2 = MathHelper.CreateRotation(Vector3.UnitZ, (float) Math.PI / 2);
      lerp = InterpolationHelper.Lerp(q1, q2, 0.5f);
      Vector3 v = lerp.Rotate(Vector3.UnitX);
      Vector3 result = new Vector3(1.0f, 1.0f, 0.0f).Normalized();
      AssertExt.AreNumericallyEqual(result, v);

      q1 = Quaternion.Identity;
      q2 = MathHelper.CreateRotation(Vector3.UnitY, (float) Math.PI / 2);
      lerp = InterpolationHelper.Lerp(q1, q2, 0.5f);
      v = lerp.Rotate(Vector3.UnitZ);
      result = new Vector3(1.0f, 0.0f, 1.0f).Normalized();
      AssertExt.AreNumericallyEqual(result, v);

      q1 = Quaternion.Identity;
      q2 = MathHelper.CreateRotation(Vector3.UnitX, (float) Math.PI / 2);
      lerp = InterpolationHelper.Lerp(q1, q2, 0.5f);
      v = lerp.Rotate(Vector3.UnitY);
      result = new Vector3(0.0f, 1.0f, 1.0f).Normalized();
      AssertExt.AreNumericallyEqual(result, v);

      q1 = new Quaternion(0.0f, 0.0f, 0.0f, -1.0f);
      q2 = MathHelper.CreateRotation(-Vector3.UnitZ, (float) -Math.PI / 2);
      lerp = InterpolationHelper.Lerp(q1, q2, 0.5f);
      v = lerp.Rotate(Vector3.UnitX);
      result = new Vector3(1.0f, 1.0f, 0.0f).Normalized();
      AssertExt.AreNumericallyEqual(result, v);
    }


    [Test]
    public void StepSinglePrecision()
    {
      Assert.AreEqual(1, InterpolationHelper.Step(1, 2, 0, StepInterpolation.Left));
      Assert.AreEqual(2, InterpolationHelper.Step(1, 2, 0.5f, StepInterpolation.Left));
      Assert.AreEqual(2, InterpolationHelper.Step(1, 2, 1, StepInterpolation.Left));

      Assert.AreEqual(1, InterpolationHelper.Step(1, 2, 0, StepInterpolation.Right));
      Assert.AreEqual(1, InterpolationHelper.Step(1, 2, 0.5f, StepInterpolation.Right));
      Assert.AreEqual(2, InterpolationHelper.Step(1, 2, 1, StepInterpolation.Right));

      Assert.AreEqual(1, InterpolationHelper.Step(1, 2, 0, StepInterpolation.Centered));
      Assert.AreEqual(1, InterpolationHelper.Step(1, 2, 0.3f, StepInterpolation.Centered));
      Assert.AreEqual(2, InterpolationHelper.Step(1, 2, 0.6f, StepInterpolation.Centered));
      Assert.AreEqual(2, InterpolationHelper.Step(1, 2, 1, StepInterpolation.Centered));
      Assert.AreEqual(1, InterpolationHelper.Step(1, 10, 0.4f, StepInterpolation.Centered));
      Assert.AreEqual(10, InterpolationHelper.Step(1, 10, 0.6f, StepInterpolation.Centered));
    }


    [Test]
    public void StepDoublePrecision()
    {
      Assert.AreEqual(1, InterpolationHelper.Step(1.0, 2.0, 0.0, StepInterpolation.Left));
      Assert.AreEqual(2, InterpolationHelper.Step(1.0, 2.0, 0.5, StepInterpolation.Left));
      Assert.AreEqual(2, InterpolationHelper.Step(1.0, 2.0, 1.0, StepInterpolation.Left));

      Assert.AreEqual(1, InterpolationHelper.Step(1.0, 2.0, 0.0, StepInterpolation.Right));
      Assert.AreEqual(1, InterpolationHelper.Step(1.0, 2.0, 0.5, StepInterpolation.Right));
      Assert.AreEqual(2, InterpolationHelper.Step(1.0, 2.0, 1.0, StepInterpolation.Right));

      Assert.AreEqual(1, InterpolationHelper.Step(1.0, 2.0, 0.0, StepInterpolation.Centered));
      Assert.AreEqual(1, InterpolationHelper.Step(1.0, 2.0, 0.3, StepInterpolation.Centered));
      Assert.AreEqual(2, InterpolationHelper.Step(1.0, 2.0, 0.6, StepInterpolation.Centered));
      Assert.AreEqual(2, InterpolationHelper.Step(1.0, 2.0, 1.0, StepInterpolation.Centered));
      Assert.AreEqual(1, InterpolationHelper.Step(1.0, 10.0, 0.4, StepInterpolation.Centered));
      Assert.AreEqual(10, InterpolationHelper.Step(1.0, 10.0, 0.6, StepInterpolation.Centered));
    }


    //[Test]
    //public void Hermite()
    //{
    //  Vector2 v1 = new Vector2(1.0f, 2.0f);
    //  Vector2 v2 = new Vector2(-1.0f, 2.1f);
    //  Vector2 t1 = new Vector2(2.0f, 1.0f); t1.Normalize();
    //  Vector2 t2 = new Vector2(0.0f, 2.0f); t2.Normalize();

    //  Vector2 hermite = Vector2.Hermite(v1, t1, v2, t2, 0.0f);
    //  AssertExt.AreNumericallyEqual(v1, hermite);

    //  hermite = Vector2.Hermite(v1, t1, v2, t2, 1.0f);
    //  AssertExt.AreNumericallyEqual(v2, hermite);
    //}


    //[Test]
    //public void CatmullRom()
    //{
    //  Vector2 v1 = new Vector2(1.0f, 2.0f);
    //  Vector2 v2 = new Vector2(-1.0f, 2.1f);
    //  Vector2 v3 = new Vector2(2.0f, 1.0f);
    //  Vector2 v4 = new Vector2(0.0f, 2.0f);

    //  Vector2 catmullRom = Vector2.CatmullRom(v1, v2, v3, v4, 0.0f);
    //  AssertExt.AreNumericallyEqual(v2, catmullRom);

    //  catmullRom = Vector2.CatmullRom(v1, v2, v3, v4, 1.0f);
    //  AssertExt.AreNumericallyEqual(v3, catmullRom);

    //  Vector2 t2 = (v3 - v1) / 2.0f;
    //  Vector2 t3 = (v4 - v2) / 2.0f;
    //  Vector2 hermite = Vector2.Hermite(v2, t2, v3, t3, 0.3f);
    //  catmullRom = Vector2.CatmullRom(v1, v2, v3, v4, 0.3f);
    //  AssertExt.AreNumericallyEqual(hermite, catmullRom);

    //  hermite = Vector2.Hermite(v2, t2, v3, t3, 0.6f);
    //  catmullRom = Vector2.CatmullRom(v1, v2, v3, v4, 0.6f);
    //  AssertExt.AreNumericallyEqual(hermite, catmullRom);
    //}


    //[Test]
    //public void Hermite()
    //{
    //  Vector3 v1 = new Vector3(1.0f, 2.0f, 3.0f);
    //  Vector3 v2 = new Vector3(-1.0f, 2.1f, 3.0f);
    //  Vector3 t1 = new Vector3(2.0f, 1.0f, 5.0f); t1.Normalize();
    //  Vector3 t2 = new Vector3(0.0f, 2.0f, -3.0f); t2.Normalize();

    //  Vector3 hermite = Vector3.Hermite(v1, t1, v2, t2, 0.0f);
    //  AssertExt.AreNumericallyEqual(v1, hermite);

    //  hermite = Vector3.Hermite(v1, t1, v2, t2, 1.0f);
    //  AssertExt.AreNumericallyEqual(v2, hermite);
    //}


    //[Test]
    //public void CatmullRom()
    //{
    //  Vector3 v1 = new Vector3(1.0f, 2.0f, 3.0f);
    //  Vector3 v2 = new Vector3(-1.0f, 2.1f, 3.0f);
    //  Vector3 v3 = new Vector3(2.0f, 1.0f, 5.0f);
    //  Vector3 v4 = new Vector3(0.0f, 2.0f, -3.0f);

    //  Vector3 catmullRom = Vector3.CatmullRom(v1, v2, v3, v4, 0.0f);
    //  AssertExt.AreNumericallyEqual(v2, catmullRom);

    //  catmullRom = Vector3.CatmullRom(v1, v2, v3, v4, 1.0f);
    //  AssertExt.AreNumericallyEqual(v3, catmullRom);

    //  Vector3 t2 = (v3 - v1) / 2.0f;
    //  Vector3 t3 = (v4 - v2) / 2.0f;
    //  Vector3 hermite = Vector3.Hermite(v2, t2, v3, t3, 0.3f);
    //  catmullRom = Vector3.CatmullRom(v1, v2, v3, v4, 0.3f);
    //  AssertExt.AreNumericallyEqual(hermite, catmullRom);

    //  hermite = Vector3.Hermite(v2, t2, v3, t3, 0.6f);
    //  catmullRom = Vector3.CatmullRom(v1, v2, v3, v4, 0.6f);
    //  AssertExt.AreNumericallyEqual(hermite, catmullRom);
    //}




    //[Test]
    //public void Hermite()
    //{
    //  Vector4 v1 = new Vector4(1.0f, 2.0f, 3.0f, 1.0f);
    //  Vector4 v2 = new Vector4(-1.0f, 2.1f, 3.0f, 1.0f);
    //  Vector4 t1 = new Vector4(2.0f, 1.0f, 5.0f, 0.0f); t1.Normalize();
    //  Vector4 t2 = new Vector4(0.0f, 2.0f, -3.0f, 0.0f); t2.Normalize();

    //  Vector4 hermite = Vector4.Hermite(v1, t1, v2, t2, 0.0f);
    //  AssertExt.AreNumericallyEqual(v1, hermite);

    //  hermite = Vector4.Hermite(v1, t1, v2, t2, 1.0f);
    //  AssertExt.AreNumericallyEqual(v2, hermite);
    //}


    //[Test]
    //public void CatmullRom()
    //{
    //  Vector4 v1 = new Vector4(1.0f, 2.0f, 3.0f, 1.0f);
    //  Vector4 v2 = new Vector4(-1.0f, 2.1f, 3.0f, 1.0f);
    //  Vector4 v3 = new Vector4(2.0f, 1.0f, 5.0f, 1.0f);
    //  Vector4 v4 = new Vector4(0.0f, 2.0f, -3.0f, 1.0f);

    //  Vector4 catmullRom = Vector4.CatmullRom(v1, v2, v3, v4, 0.0f);
    //  AssertExt.AreNumericallyEqual(v2, catmullRom);

    //  catmullRom = Vector4.CatmullRom(v1, v2, v3, v4, 1.0f);
    //  AssertExt.AreNumericallyEqual(v3, catmullRom);

    //  Vector4 t2 = (v3 - v1) / 2.0f;
    //  Vector4 t3 = (v4 - v2) / 2.0f;
    //  Vector4 hermite = Vector4.Hermite(v2, t2, v3, t3, 0.3f);
    //  catmullRom = Vector4.CatmullRom(v1, v2, v3, v4, 0.3f);
    //  AssertExt.AreNumericallyEqual(hermite, catmullRom);

    //  hermite = Vector4.Hermite(v2, t2, v3, t3, 0.6f);
    //  catmullRom = Vector4.CatmullRom(v1, v2, v3, v4, 0.6f);
    //  AssertExt.AreNumericallyEqual(hermite, catmullRom);
    //}


    [Test]
    public void CosineInterpolationSinglePrecision()
    {
      Assert.AreEqual(1.0f, InterpolationHelper.CosineInterpolation(1.0f, 2.0f, 0.0f));
      Assert.AreEqual(1.5f, InterpolationHelper.CosineInterpolation(1.0f, 2.0f, 0.5f));
      Assert.AreEqual(2.0f, InterpolationHelper.CosineInterpolation(1.0f, 2.0f, 1.0f));
      Assert.AreEqual(1.5f, InterpolationHelper.CosineInterpolation(2.0f, 1.0f, 0.5f));
    }


    [Test]
    public void CosineInterpolationDoublePrecision()
    {
      Assert.AreEqual(1.0, InterpolationHelper.CosineInterpolation(1.0, 2.0, 0.0));
      Assert.AreEqual(1.5, InterpolationHelper.CosineInterpolation(1.0, 2.0, 0.5));
      Assert.AreEqual(2.0, InterpolationHelper.CosineInterpolation(1.0, 2.0, 1.0));
      Assert.AreEqual(1.5, InterpolationHelper.CosineInterpolation(2.0, 1.0, 0.5));
    }


    [Test]
    public void CosineInterpolationVector2()
    {
      Vector2 v = new Vector2(1.0f, 10.0f);
      Vector2 w = new Vector2(2.0f, 20.0f);
      Vector2 lerp0 = InterpolationHelper.CosineInterpolation(v, w, 0.0f);
      Vector2 lerp1 = InterpolationHelper.CosineInterpolation(v, w, 1.0f);
      Vector2 lerp05 = InterpolationHelper.CosineInterpolation(v, w, 0.5f);

      Assert.AreEqual(v, lerp0);
      Assert.AreEqual(w, lerp1);
      Assert.AreEqual(new Vector2(1.5f, 15.0f), lerp05);
    }


    [Test]
    public void CosineInterpolationVector3()
    {
      Vector3 v = new Vector3(1.0f, 10.0f, 100.0f);
      Vector3 w = new Vector3(2.0f, 20.0f, 200.0f);
      Vector3 lerp0 = InterpolationHelper.CosineInterpolation(v, w, 0.0f);
      Vector3 lerp1 = InterpolationHelper.CosineInterpolation(v, w, 1.0f);
      Vector3 lerp05 = InterpolationHelper.CosineInterpolation(v, w, 0.5f);

      Assert.AreEqual(v, lerp0);
      Assert.AreEqual(w, lerp1);
      Assert.AreEqual(new Vector3(1.5f, 15.0f, 150.0f), lerp05);
    }


    [Test]
    public void CosineInterpolationVector4()
    {
      Vector4 v = new Vector4(1.0f, 10.0f, 100.0f, 1000.0f);
      Vector4 w = new Vector4(2.0f, 20.0f, 200.0f, 2000.0f);
      Vector4 lerp0 = InterpolationHelper.CosineInterpolation(v, w, 0.0f);
      Vector4 lerp1 = InterpolationHelper.CosineInterpolation(v, w, 1.0f);
      Vector4 lerp05 = InterpolationHelper.CosineInterpolation(v, w, 0.5f);

      Assert.AreEqual(v, lerp0);
      Assert.AreEqual(w, lerp1);
      Assert.AreEqual(new Vector4(1.5f, 15.0f, 150.0f, 1500.0f), lerp05);
    }


    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PolynomialInterpolationSinglePrecisionException()
    {
      InterpolationHelper.PolynomialInterpolation(null, 0);
    }



    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PolynomialInterpolationDoublePrecisionException()
    {
      InterpolationHelper.PolynomialInterpolation(null, 0);
    }


    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void PolynomialInterpolationSinglePrecisionException2()
    {
      InterpolationHelper.PolynomialInterpolation(new List<Vector2>(), 0);
    }


    [Test]
    [ExpectedException(typeof(MathematicsException))]
    public void PolynomialInterpolationSinglePrecisionException3()
    {
      // Error: 2 identical x values.
      var points = new[] { new Vector2(0, 1), new Vector2(0, 4), new Vector2(5, -1) };
      InterpolationHelper.PolynomialInterpolation(points, 0);
    }


    [Test]
    public void PolynomialInterpolationSinglePrecision()
    {
      var points = new[] { new Vector2(0, 1), new Vector2(3, 4), new Vector2(5, -1) };

      AssertExt.AreNumericallyEqual(points[0].Y, InterpolationHelper.PolynomialInterpolation(points, points[0].X));
      AssertExt.AreNumericallyEqual(points[1].Y, InterpolationHelper.PolynomialInterpolation(points, points[1].X));
      AssertExt.AreNumericallyEqual(points[2].Y, InterpolationHelper.PolynomialInterpolation(points, points[2].X));
    }


    [Test]
    public void SlerpSinglePrecision()
    {
      // Warning: The not all results are not verified
      Quaternion q1 = new Quaternion(2.0f, 3.0f, 4.0f, 1.0f).Normalized();
      Quaternion q2 = new Quaternion(6.0f, 8.0f, 2.0f, 4.0f).Normalized();
      Quaternion slerp = InterpolationHelper.Slerp(q1, q2, 0.75f);
      Assert.IsTrue(slerp.IsNumericallyNormalized());

      slerp = InterpolationHelper.Slerp(q1, q2, 0);
      Assert.IsTrue(slerp.IsNumericallyNormalized());
      AssertExt.AreNumericallyEqual(q1, slerp);

      slerp = InterpolationHelper.Slerp(q1, q2, 1);
      Assert.IsTrue(slerp.IsNumericallyNormalized());
      AssertExt.AreNumericallyEqual(q2, slerp);
    }


    [Test]
    public void SlerpZSinglePrecision()
    {
      Quaternion q1 = Quaternion.Identity;
      Quaternion q2 = MathHelper.CreateRotation(Vector3.UnitZ, (float)Math.PI / 2);
      Quaternion slerp = InterpolationHelper.Slerp(q1, q2, 0.5f);
      Assert.IsTrue(slerp.IsNumericallyNormalized());
      Vector3 v = slerp.Rotate(Vector3.UnitX);
      Vector3 result = new Vector3(1.0f, 1.0f, 0.0f).Normalized();
      AssertExt.AreNumericallyEqual(result, v);
    }


    [Test]
    public void SlerpYSinglePrecision()
    {
      Quaternion q1 = Quaternion.Identity;
      Quaternion q2 = MathHelper.CreateRotation(Vector3.UnitY, (float) Math.PI / 2);
      Quaternion slerp = InterpolationHelper.Slerp(q1, q2, 0.5f);
      Assert.IsTrue(slerp.IsNumericallyNormalized());
      Vector3 v = slerp.Rotate(Vector3.UnitZ);
      Vector3 result = new Vector3(1.0f, 0.0f, 1.0f).Normalized();
      AssertExt.AreNumericallyEqual(result, v);
    }


    [Test]
    public void SlerpXSinglePrecision()
    {
      Quaternion q1 = Quaternion.Identity;
      Quaternion q2 = MathHelper.CreateRotation(Vector3.UnitX, (float) Math.PI / 2);
      Quaternion slerp = InterpolationHelper.Slerp(q1, q2, 0.5f);
      Assert.IsTrue(slerp.IsNumericallyNormalized());
      Vector3 v = slerp.Rotate(Vector3.UnitY);
      Vector3 result = new Vector3(0.0f, 1.0f, 1.0f).Normalized();
      AssertExt.AreNumericallyEqual(result, v);
    }


    [Test]
    public void SlerpNegatedSinglePrecision()
    {
      Quaternion q1 = new Quaternion(0.0f, 0.0f, 0.0f, -1.0f);
      Quaternion q2 = MathHelper.CreateRotation(-Vector3.UnitZ, (float) -Math.PI / 2);
      Quaternion slerp = InterpolationHelper.Slerp(q1, q2, 0.5f);
      Assert.IsTrue(slerp.IsNumericallyNormalized());
      Vector3 v = slerp.Rotate(Vector3.UnitX);
      Vector3 result = new Vector3(1.0f, 1.0f, 0.0f).Normalized();
      AssertExt.AreNumericallyEqual(result, v);
    }


    [Test]
    public void SlerpGeneralSinglePrecision()
    {
      Quaternion q1 = MathHelper.CreateRotation(-Vector3.UnitY, (float) Math.PI / 2);
      Quaternion q2 = MathHelper.CreateRotation(Vector3.UnitZ, (float) Math.PI / 2);
      Quaternion slerp = InterpolationHelper.Slerp(q1, q2, 0.5f);
      Assert.IsTrue(slerp.IsNumericallyNormalized());
      Vector3 v = slerp.Rotate(Vector3.UnitX);
      Vector3 result = new Vector3(1.0f / 3.0f, 2.0f / 3.0f, 2.0f / 3.0f);  // I hope this is correct.
      AssertExt.AreNumericallyEqual(result, v);

      q1 = MathHelper.CreateRotation(-Vector3.UnitY, (float) Math.PI / 2);
      q2 = MathHelper.CreateRotation(-Vector3.UnitZ, (float) -Math.PI / 2);
      slerp = InterpolationHelper.Slerp(q1, q2, 0.5f);
      Assert.IsTrue(slerp.IsNumericallyNormalized());
      v = slerp.Rotate(Vector3.UnitX);
      result = new Vector3(1.0f / 3.0f, 2.0f / 3.0f, 2.0f / 3.0f);  // I hope this is correct.
      AssertExt.AreNumericallyEqual(result, v);
    }


    [Test]
    public void SquadSinglePrecision()
    {
      Quaternion q0 = MathHelper.CreateRotation(new Vector3(1, 1, 1), 0.3f);
      Quaternion q1 = MathHelper.CreateRotation(new Vector3(1, 0, 1), 0.4f);
      Quaternion q2 = MathHelper.CreateRotation(new Vector3(1, 0, -1), -0.6f);
      Quaternion q3 = MathHelper.CreateRotation(new Vector3(0, 1, 1), 0.2f);

      Quaternion q, a, b, p;
      Quaternion expected;

      InterpolationHelper.SquadSetup(q0, q1, q2, q3, out q, out a, out b, out p);

      // t = 0
      Quaternion result = InterpolationHelper.Squad(q, a, b, p, 0.0f);
      AssertExt.AreNumericallyEqual(q1, result);

      // t = 1.0f
      result = InterpolationHelper.Squad(q, a, b, p, 1.0f);
      AssertExt.AreNumericallyEqual(q2, result);

      // Check series (just for debugging)
      Quaternion r1, r2, r3, r4, r5, r6, r7, r8, r9;
      r1 = InterpolationHelper.Squad(q, a, b, p, 0.1f);
      r2 = InterpolationHelper.Squad(q, a, b, p, 0.2f);
      r3 = InterpolationHelper.Squad(q, a, b, p, 0.3f);
      r4 = InterpolationHelper.Squad(q, a, b, p, 0.4f);
      r5 = InterpolationHelper.Squad(q, a, b, p, 0.5f);
      r6 = InterpolationHelper.Squad(q, a, b, p, 0.6f);
      r7 = InterpolationHelper.Squad(q, a, b, p, 0.7f);
      r8 = InterpolationHelper.Squad(q, a, b, p, 0.8f);
      r9 = InterpolationHelper.Squad(q, a, b, p, 0.9f);

      // q0 = q1, q2 = q3
      InterpolationHelper.SquadSetup(q1, q1, q2, q2, out q, out a, out b, out p);
      result = InterpolationHelper.Squad(q, a, b, p, 0.5f);
      expected = InterpolationHelper.Slerp(q1, q2, 0.5f);
      AssertExt.AreNumericallyEqual(expected, result);
    }


    [Test]
    public void HermiteSmoothStepD()
    {
      AssertExt.AreNumericallyEqual(0, InterpolationHelper.HermiteSmoothStep(-1.0f));
      AssertExt.AreNumericallyEqual(0, InterpolationHelper.HermiteSmoothStep(0.0f));
      AssertExt.AreNumericallyEqual(0.5, InterpolationHelper.HermiteSmoothStep(0.5f));
      AssertExt.AreNumericallyEqual(1, InterpolationHelper.HermiteSmoothStep(1.0f));
      AssertExt.AreNumericallyEqual(1, InterpolationHelper.HermiteSmoothStep(2.0f));
      AssertExt.AreNumericallyEqual(1 - InterpolationHelper.HermiteSmoothStep(1 - 0.3f), InterpolationHelper.HermiteSmoothStep(0.3f));
      Assert.Greater(InterpolationHelper.HermiteSmoothStep(1 - 0.3f), InterpolationHelper.HermiteSmoothStep(0.3f));
    }


    [Test]
    public void HermiteSmoothStepF()
    {
      AssertExt.AreNumericallyEqual(0, InterpolationHelper.HermiteSmoothStep(-1.0));
      AssertExt.AreNumericallyEqual(0, InterpolationHelper.HermiteSmoothStep(0.0));
      AssertExt.AreNumericallyEqual(0.5, InterpolationHelper.HermiteSmoothStep(0.5));
      AssertExt.AreNumericallyEqual(1, InterpolationHelper.HermiteSmoothStep(1.0));
      AssertExt.AreNumericallyEqual(1, InterpolationHelper.HermiteSmoothStep(2.0));
      AssertExt.AreNumericallyEqual(1 - InterpolationHelper.HermiteSmoothStep(1 - 0.3), InterpolationHelper.HermiteSmoothStep(0.3));
      Assert.Greater(InterpolationHelper.HermiteSmoothStep(1 - 0.3), InterpolationHelper.HermiteSmoothStep(0.3));
    }


    [Test]
    public void EaseInOutSmoothStepF()
    {
      AssertExt.AreNumericallyEqual(0, InterpolationHelper.EaseInOutSmoothStep(-1.0f));
      AssertExt.AreNumericallyEqual(0, InterpolationHelper.EaseInOutSmoothStep(0.0f));
      AssertExt.AreNumericallyEqual(0.5, InterpolationHelper.EaseInOutSmoothStep(0.5f));
      AssertExt.AreNumericallyEqual(1, InterpolationHelper.EaseInOutSmoothStep(1.0f));
      AssertExt.AreNumericallyEqual(1, InterpolationHelper.EaseInOutSmoothStep(2.0f));
      AssertExt.AreNumericallyEqual(1 - InterpolationHelper.EaseInOutSmoothStep(1 - 0.3f), InterpolationHelper.EaseInOutSmoothStep(0.3f));
      Assert.Greater(InterpolationHelper.EaseInOutSmoothStep(1 - 0.3f), InterpolationHelper.EaseInOutSmoothStep(0.3f));

      Assert.Greater(0.5f, InterpolationHelper.EaseInOutSmoothStep(0.3f));
      Assert.Less(0.5f, InterpolationHelper.EaseInOutSmoothStep(0.6f));
    }


    [Test]
    public void EaseInOutSmoothStepD()
    {
      AssertExt.AreNumericallyEqual(0, InterpolationHelper.EaseInOutSmoothStep(-1.0));
      AssertExt.AreNumericallyEqual(0, InterpolationHelper.EaseInOutSmoothStep(0.0));
      AssertExt.AreNumericallyEqual(0.5, InterpolationHelper.EaseInOutSmoothStep(0.5));
      AssertExt.AreNumericallyEqual(1, InterpolationHelper.EaseInOutSmoothStep(1.0));
      AssertExt.AreNumericallyEqual(1, InterpolationHelper.EaseInOutSmoothStep(2.0));
      AssertExt.AreNumericallyEqual(1 - InterpolationHelper.EaseInOutSmoothStep(1 - 0.3), InterpolationHelper.EaseInOutSmoothStep(0.3));
      Assert.Greater(InterpolationHelper.EaseInOutSmoothStep(1 - 0.3), InterpolationHelper.EaseInOutSmoothStep(0.3));

      Assert.Greater(0.5f, InterpolationHelper.EaseInOutSmoothStep(0.3));
      Assert.Less(0.5f, InterpolationHelper.EaseInOutSmoothStep(0.6));
    }
  }  
}
