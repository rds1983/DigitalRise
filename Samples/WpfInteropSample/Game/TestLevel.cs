using AssetManagementBase;
using CommonServiceLocator;
using DigitalRise.Geometry;
using DigitalRise.Graphics;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;

namespace WpfInteropSample2
{
  // Creates a graphics screen and adds 3D models and lights to the scene graph.
  internal static class TestLevel
  {
    public static Scene Create()
    {
      var graphicsService = ServiceLocator.Current.GetInstance<IGraphicsService>();
      var assetManager = ServiceLocator.Current.GetInstance<AssetManager>();

      var screen = new MyGraphicsScreen(graphicsService, assetManager);
      graphicsService.Screens.Add(screen);

      AddLights(screen.Scene);

      var groundModel = assetManager.LoadDRModel(graphicsService, "Ground/Ground.drmdl").Clone();
      screen.Scene.Children.Add(groundModel);

      var tankModel = assetManager.LoadDRModel(graphicsService, "Tank/tank.drmdl").Clone();
      screen.Scene.Children.Add(tankModel);

      return screen.Scene;
    }


    // Add light sources for standard three-point lighting.
    private static void AddLights(Scene scene)
    {
      var ambientLight = new AmbientLight
      {
        Color = new Vector3(0.05333332f, 0.09882354f, 0.1819608f),
        Intensity = 1,
        HemisphericAttenuation = 0,
      };
      scene.Children.Add(new LightNode(ambientLight));

      var keyLight = new DirectionalLight
      {
        Color = new Vector3(1, 0.9607844f, 0.8078432f),
        DiffuseIntensity = 1,
        SpecularIntensity = 1,
      };
      var keyLightNode = new LightNode(keyLight)
      {
        Name = "KeyLight",
        Priority = 10,   // This is the most important light.
        PoseWorld = new Pose(QuaternionF.CreateRotation(Vector3.Forward, new Vector3(-0.5265408f, -0.5735765f, -0.6275069f))),
      };
      scene.Children.Add(keyLightNode);

      var fillLight = new DirectionalLight
      {
        Color = new Vector3(0.9647059f, 0.7607844f, 0.4078432f),
        DiffuseIntensity = 1,
        SpecularIntensity = 0,
      };
      var fillLightNode = new LightNode(fillLight)
      {
        Name = "FillLight",
        PoseWorld = new Pose(QuaternionF.CreateRotation(Vector3.Forward, new Vector3(0.7198464f, 0.3420201f, 0.6040227f))),
      };
      scene.Children.Add(fillLightNode);

      var backLight = new DirectionalLight
      {
        Color = new Vector3(0.3231373f, 0.3607844f, 0.3937255f),
        DiffuseIntensity = 1,
        SpecularIntensity = 1,
      };
      var backLightNode = new LightNode(backLight)
      {
        Name = "BackLight",
        PoseWorld = new Pose(QuaternionF.CreateRotation(Vector3.Forward, new Vector3(0.4545195f, -0.7660444f, 0.4545195f))),
      };
      scene.Children.Add(backLightNode);
    }
  }
}
