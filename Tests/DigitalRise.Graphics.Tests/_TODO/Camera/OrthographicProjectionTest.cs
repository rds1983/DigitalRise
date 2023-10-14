using DigitalRise.Mathematics.Algebra;
using NUnit.Framework;
using NUnit.Utils;


namespace DigitalRise.Graphics.Tests
{
  [TestFixture]
  public class OrthographicProjectionTest
  {
    [Test]
    public void SetProjectionTest()
    {
      OrthographicProjection projection = new OrthographicProjection();
      projection.Set(4, 3, 2, 10);

      OrthographicProjection camera2 = new OrthographicProjection();
      camera2.Set(4, 3);
      camera2.Near = 2;
      camera2.Far = 10;

      OrthographicProjection camera3 = new OrthographicProjection
      {
        Left = -2,
        Right = 2,
        Bottom = -1.5f,
        Top = 1.5f,
        Near = 2,
        Far = 10,
      };

      Matrix44F expected = Matrix44F.CreateOrthographic(4, 3, 2, 10);
      AssertExt.AreNumericallyEqual(expected, projection);
      AssertExt.AreNumericallyEqual(expected, camera2);
      AssertExt.AreNumericallyEqual(expected, camera3.ToMatrix44F());
    }

    [Test]
    public void SetProjectionOffCenterTest()
    {
      OrthographicProjection projection = new OrthographicProjection();
      projection.SetOffCenter(0, 4, 1, 4, 2, 10);

      OrthographicProjection camera2 = new OrthographicProjection();
      camera2.SetOffCenter(0, 4, 1, 4);
      camera2.Near = 2;
      camera2.Far = 10;

      Projection camera3 = new OrthographicProjection
      {
        Left = 0,
        Right = 4,
        Bottom = 1,
        Top = 4,
        Near = 2,
        Far = 10,
      };

      Matrix44F expected = Matrix44F.CreateOrthographicOffCenter(0, 4, 1, 4, 2, 10);
      AssertExt.AreNumericallyEqual(expected, projection);
      AssertExt.AreNumericallyEqual(expected, camera2);
      AssertExt.AreNumericallyEqual(expected, camera3.ToMatrix44F());
    }
  }
}