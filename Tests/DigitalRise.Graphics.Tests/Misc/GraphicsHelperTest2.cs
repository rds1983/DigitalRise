using System;
using DigitalRise.Geometry;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using NUnit.Utils;
using MathHelper = DigitalRise.Mathematics.MathHelper;


namespace DigitalRise.Graphics.Tests
{
  [TestFixture]
  public class GraphicsHelperTest2
  {
    [Test]
    public void ProjectTest()
    {
      Viewport viewport = new Viewport(0, 0, 640, 480);
      PerspectiveProjection projection = new PerspectiveProjection();
      projection.SetFieldOfView(MathHelper.ToRadians(60), viewport.AspectRatio, 10, 1000);
      Matrix44F view = Matrix44F.CreateLookAt(new Vector3(0, 0, 0), new Vector3(0, 0, -1), Vector3.Up);

      AssertExt.AreNumericallyEqual(new Vector3(320, 240, 0), viewport.Project(new Vector3(0, 0, -10), projection, view));
      AssertExt.AreNumericallyEqual(new Vector3(0, 0, 0), viewport.Project(new Vector3(projection.Left, projection.Top, -10), projection, view));
      AssertExt.AreNumericallyEqual(new Vector3(640, 0, 0), viewport.Project(new Vector3(projection.Right, projection.Top, -10), projection, view));
      AssertExt.AreNumericallyEqual(new Vector3(0, 480, 0), viewport.Project(new Vector3(projection.Left, projection.Bottom, -10), projection, view));
      AssertExt.AreNumericallyEqual(new Vector3(640, 480, 0), viewport.Project(new Vector3(projection.Right, projection.Bottom, -10), projection, view));

      Vector3[] farCorners = new Vector3[4];
      GraphicsHelper.GetFrustumFarCorners(projection, farCorners);
      AssertExt.AreNumericallyEqual(new Vector3(320, 240, 1), viewport.Project(new Vector3(0, 0, -1000), projection, view));
      AssertExt.AreNumericallyEqual(new Vector3(0, 0, 1), viewport.Project((Vector3)farCorners[0], projection, view), 1e-4f);
      AssertExt.AreNumericallyEqual(new Vector3(640, 0, 1), viewport.Project((Vector3)farCorners[1], projection, view), 1e-4f);
      AssertExt.AreNumericallyEqual(new Vector3(0, 480, 1), viewport.Project((Vector3)farCorners[2], projection, view), 1e-4f);
      AssertExt.AreNumericallyEqual(new Vector3(640, 480, 1), viewport.Project((Vector3)farCorners[3], projection, view), 1e-4f);
    }



    [Test]
    public void UnprojectTest()
    {
      using (var setEpsilon = new SetEpsilonF(1e-02f))
      {
        Viewport viewport = new Viewport(0, 0, 640, 480);
        PerspectiveProjection projection = new PerspectiveProjection();
        projection.SetFieldOfView(MathHelper.ToRadians(60), viewport.AspectRatio, 10, 1000);
        Matrix44F view = Matrix44F.CreateLookAt(new Vector3(0, 0, 0), new Vector3(0, 0, -1), Vector3.Up);

        AssertExt.AreNumericallyEqual(new Vector3(0, 0, -10), viewport.Unproject(new Vector3(320, 240, 0), projection, view));
        AssertExt.AreNumericallyEqual(new Vector3(projection.Left, projection.Top, -10), viewport.Unproject(new Vector3(0, 0, 0), projection, view));
        AssertExt.AreNumericallyEqual(new Vector3(projection.Right, projection.Top, -10), viewport.Unproject(new Vector3(640, 0, 0), projection, view));
        AssertExt.AreNumericallyEqual(new Vector3(projection.Left, projection.Bottom, -10), viewport.Unproject(new Vector3(0, 480, 0), projection, view));
        AssertExt.AreNumericallyEqual(new Vector3(projection.Right, projection.Bottom, -10), viewport.Unproject(new Vector3(640, 480, 0), projection, view));

        Vector3[] farCorners = new Vector3[4];
        GraphicsHelper.GetFrustumFarCorners(projection, farCorners);
        AssertExt.AreNumericallyEqual((Vector3)farCorners[0], viewport.Unproject(new Vector3(0, 0, 1), projection, view));
        AssertExt.AreNumericallyEqual((Vector3)farCorners[1], viewport.Unproject(new Vector3(640, 0, 1), projection, view));
        AssertExt.AreNumericallyEqual((Vector3)farCorners[2], viewport.Unproject(new Vector3(0, 480, 1), projection, view));
        AssertExt.AreNumericallyEqual((Vector3)farCorners[3], viewport.Unproject(new Vector3(640, 480, 1), projection, view));
      }
    }


    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void GetScreenSizeException()
    {
      var viewport = new Viewport(10, 10, 200, 100);
      var geometricObject = new GeometricObject(new SphereShape());
      GraphicsHelper.GetScreenSize(null, viewport, geometricObject);
    }


    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void GetScreenSizeException2()
    {
      var cameraNode = new CameraNode(new Camera(new PerspectiveProjection()));
      var viewport = new Viewport(10, 10, 200, 100);
      GraphicsHelper.GetScreenSize(cameraNode, viewport, null);
    }


    [Test]
    public void GetScreenSizeWithPerspective()
    {
      // Camera
      var projection = new PerspectiveProjection();
      projection.SetFieldOfView(MathHelper.ToRadians(90), 2.0f / 1.0f, 1.0f, 100f);
      var camera = new Camera(projection);
      var cameraNode = new CameraNode(camera);
      cameraNode.PoseWorld = new Pose(new Vector3(123, 456, -789), Matrix33F.CreateRotation(new Vector3(1, -2, 3), MathHelper.ToRadians(75)));

      // 2:1 viewport
      var viewport = new Viewport(10, 10, 200, 100);
      
      // Test object
      var shape = new SphereShape();
      var geometricObject = new GeometricObject(shape);

      // Empty sphere at camera position.
      shape.Radius = 0;
      geometricObject.Pose = cameraNode.PoseWorld;
      Vector2 screenSize = GraphicsHelper.GetScreenSize(cameraNode, viewport, geometricObject);
      Assert.AreEqual(0, screenSize.X);
      Assert.AreEqual(0, screenSize.Y);

      // Empty sphere centered at near plane.
      shape.Radius = 0;
      geometricObject.Pose = cameraNode.PoseWorld * new Pose(new Vector3(0.123f, -0.543f, -1));
      screenSize = GraphicsHelper.GetScreenSize(cameraNode, viewport, geometricObject);
      Assert.AreEqual(0, screenSize.X);
      Assert.AreEqual(0, screenSize.Y);

      // Create sphere which as a bounding sphere of ~1 unit diameter:
      // Since the bounding sphere is based on the AABB, we need to make the 
      // actual sphere a bit smaller.
      shape.Radius = 1 / (2 * (float)Math.Sqrt(3)) + Numeric.EpsilonF;

      // Sphere at camera position.
      geometricObject.Pose = cameraNode.PoseWorld;
      screenSize = GraphicsHelper.GetScreenSize(cameraNode, viewport, geometricObject);
      Assert.Greater(screenSize.X, 200);
      Assert.Greater(screenSize.Y, 100);

      // Sphere at near plane.
      geometricObject.Pose = cameraNode.PoseWorld * new Pose(new Vector3(0.123f, -0.543f, -1));
      screenSize = GraphicsHelper.GetScreenSize(cameraNode, viewport, geometricObject);
      AssertExt.AreNumericallyEqual(screenSize.X, 50.0f, 10f);
      AssertExt.AreNumericallyEqual(screenSize.Y, 50.0f, 10f);

      // Double distance --> half size
      geometricObject.Pose = cameraNode.PoseWorld * new Pose(new Vector3(0.123f, -0.543f, -2));
      screenSize = GraphicsHelper.GetScreenSize(cameraNode, viewport, geometricObject);
      AssertExt.AreNumericallyEqual(screenSize.X, 25.0f, 5f);
      AssertExt.AreNumericallyEqual(screenSize.Y, 25.0f, 5f);
    }


    [Test]
    public void GetScreenSizeWithOrthographic()
    {
      // Camera
      var projection = new OrthographicProjection();
      projection.SetOffCenter(0, 4, 0, 2);
      var camera = new Camera(projection);
      var cameraNode = new CameraNode(camera);
      cameraNode.PoseWorld = new Pose(new Vector3(123, 456, -789), Matrix33F.CreateRotation(new Vector3(1, -2, 3), MathHelper.ToRadians(75)));

      // 2:1 viewport
      var viewport = new Viewport(10, 10, 200, 100);

      // Test object
      var shape = new SphereShape();
      var geometricObject = new GeometricObject(shape);

      // Empty sphere at camera position.
      shape.Radius = 0;
      geometricObject.Pose = cameraNode.PoseWorld;
      Vector2 screenSize = GraphicsHelper.GetScreenSize(cameraNode, viewport, geometricObject);
      Assert.AreEqual(0, screenSize.X);
      Assert.AreEqual(0, screenSize.Y);

      // Empty sphere centered at near plane.
      shape.Radius = 0;
      geometricObject.Pose = cameraNode.PoseWorld * new Pose(new Vector3(0.123f, -0.543f, -1));
      screenSize = GraphicsHelper.GetScreenSize(cameraNode, viewport, geometricObject);
      Assert.AreEqual(0, screenSize.X);
      Assert.AreEqual(0, screenSize.Y);

      // Create sphere which as a bounding sphere of ~1 unit diameter:
      // Since the bounding sphere is based on the AABB, we need to make the 
      // actual sphere a bit smaller.
      shape.Radius = 1 / (2 * (float)Math.Sqrt(3)) + Numeric.EpsilonF;

      // Sphere at camera position.
      geometricObject.Pose = cameraNode.PoseWorld;
      screenSize = GraphicsHelper.GetScreenSize(cameraNode, viewport, geometricObject);
      AssertExt.AreNumericallyEqual(screenSize.X, 50.0f, 10f);
      AssertExt.AreNumericallyEqual(screenSize.Y, 50.0f, 10f);

      // Sphere at near plane.
      geometricObject.Pose = cameraNode.PoseWorld * new Pose(new Vector3(0.123f, -0.543f, -1));
      screenSize = GraphicsHelper.GetScreenSize(cameraNode, viewport, geometricObject);
      AssertExt.AreNumericallyEqual(screenSize.X, 50.0f, 10f);
      AssertExt.AreNumericallyEqual(screenSize.Y, 50.0f, 10f);

      // Double distance --> same size
      geometricObject.Pose = cameraNode.PoseWorld * new Pose(new Vector3(0.123f, -0.543f, -2));
      screenSize = GraphicsHelper.GetScreenSize(cameraNode, viewport, geometricObject);
      AssertExt.AreNumericallyEqual(screenSize.X, 50.0f, 10f);
      AssertExt.AreNumericallyEqual(screenSize.Y, 50.0f, 10f);
    }
  }
}
