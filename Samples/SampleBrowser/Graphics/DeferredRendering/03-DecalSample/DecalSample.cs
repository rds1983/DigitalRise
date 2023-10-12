using DigitalRise.Geometry;
using DigitalRise.Graphics.Rendering;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using DigitalRise.Physics.ForceEffects;
using Microsoft.Xna.Framework;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace Samples.Graphics
{
  [Sample(SampleCategory.Graphics,
    @"This sample shows decal rendering.",
    @"",
    103)]
  public class DecalSample : Sample
  {
    private readonly DeferredGraphicsScreen _graphicsScreen;


    public DecalSample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      SampleFramework.IsMouseVisible = false;

      _graphicsScreen = new DeferredGraphicsScreen(Services);
      _graphicsScreen.DrawReticle = true;
      GraphicsService.Screens.Insert(0, _graphicsScreen);
      GameObjectService.Objects.Add(new DeferredGraphicsOptionsObject(Services));

      Services.AddService(typeof(DebugRenderer), _graphicsScreen.DebugRenderer);
      Services.AddService(typeof(IScene), _graphicsScreen.Scene);

      // Add gravity and damping to the physics simulation.
      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());

      // Add a custom game object which controls the camera.
      var cameraGameObject = new CameraObject(Services);
      GameObjectService.Objects.Add(cameraGameObject);
      _graphicsScreen.ActiveCameraNode = cameraGameObject.CameraNode;

      GameObjectService.Objects.Add(new GrabObject(Services));
      GameObjectService.Objects.Add(new StaticSkyObject(Services));
      GameObjectService.Objects.Add(new GroundObject(Services));
      GameObjectService.Objects.Add(new ObjectCreatorObject(Services));
      GameObjectService.Objects.Add(new LavaBallsObject(Services));
      GameObjectService.Objects.Add(new FogObject(Services));

      // Add some static objects.
      GameObjectService.Objects.Add(new StaticObject(Services, "Barrier/Barrier.drmdl", 1, Pose.Identity));
      GameObjectService.Objects.Add(new StaticObject(Services, "Barrier/Cylinder.drmdl", 1, new Pose(new Vector3(3, 0, 1), MathHelper.CreateRotationY(MathHelper.ToRadians(-20)))));

      // Add a dynamic object.
      GameObjectService.Objects.Add(new DynamicObject(Services, 1));

      // Add some predefined decals.
      GameObjectService.Objects.Add(new EnvironmentDecalsObject(Services));
    }


    public override void Update(GameTime gameTime)
    {
      // This sample clears the debug renderer each frame.
      _graphicsScreen.DebugRenderer.Clear();
    }
  }
}