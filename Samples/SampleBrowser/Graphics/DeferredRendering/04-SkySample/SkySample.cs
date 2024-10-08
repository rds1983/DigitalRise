﻿using System;
using DigitalRise.Geometry;
using DigitalRise.Graphics.PostProcessing;
using DigitalRise.Graphics.Rendering;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using DigitalRise.Mathematics.Statistics;
using DigitalRise.Physics.ForceEffects;
using Microsoft.Xna.Framework;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace Samples.Graphics
{
  [Sample(SampleCategory.Graphics,
    @"This sample shows how to create a dynamic sky.",
    @"",
    104)]
  public class SkySample : Sample
  {
    // Random notes:
    // On Xbox, you might see a lot of banding when it gets dark. This happens
    // because HdrBlendable has less precision on the Xbox. To avoid banding:
    // - Decrease HdrFilter.MaxExposure. 
    // - Increase ScatteringSkyNode.SunIntensity.
    //
    // Possible optimizations:
    // Do not render the sky in every frame. Render it into a cube map instead
    // and only render the cube map using a SkyboxNode.
    // If you animate the sun, do not move the sun in every frame. Update the sky
    // with a lower frequency. Only update the sky when the user is moving to hide
    // sudden changes of the sun position and the shadows.


    private readonly DeferredGraphicsScreen _graphicsScreen;


    public SkySample(Microsoft.Xna.Framework.Game game)
      : base(game)
    {
      SampleFramework.IsMouseVisible = false;

      _graphicsScreen = new DeferredGraphicsScreen(Services);
      _graphicsScreen.DrawReticle = true;
      GraphicsService.Screens.Insert(0, _graphicsScreen);
      GameObjectService.Objects.Add(new DeferredGraphicsOptionsObject(Services));

      Services.AddService(typeof(DebugRenderer), _graphicsScreen.DebugRenderer);
      Services.AddService(typeof(IScene), _graphicsScreen.Scene);

      // Add gravity and damping to the physics Simulation.
      Simulation.ForceEffects.Add(new Gravity());
      Simulation.ForceEffects.Add(new Damping());

      // Add a custom game object which controls the camera.
      var cameraGameObject = new CameraObject(Services);
      GameObjectService.Objects.Add(cameraGameObject);
      _graphicsScreen.ActiveCameraNode = cameraGameObject.CameraNode;

      GameObjectService.Objects.Add(new GrabObject(Services));
      GameObjectService.Objects.Add(new GroundObject(Services));
      GameObjectService.Objects.Add(new DudeObject(Services));
      GameObjectService.Objects.Add(new ObjectCreatorObject(Services));
      GameObjectService.Objects.Add(new LavaBallsObject(Services));
      GameObjectService.Objects.Add(new FogObject(Services));
      GameObjectService.Objects.Add(new StaticObject(Services, "Barrier/Barrier.drmdl", 0.9f, new Pose(new Vector3(0, 0, -2))));
      GameObjectService.Objects.Add(new StaticObject(Services, "Barrier/Cylinder.drmdl", 0.9f, new Pose(new Vector3(3, 0, 0), MathHelper.CreateRotationY(MathHelper.ToRadians(-20)))));

      // The DynamicSkyObject creates the dynamic sky and lights.
      GameObjectService.Objects.Add(new DynamicSkyObject(Services));

      // Add a few palm trees.
      Random random = new Random(12345);
      for (int i = 0; i < 10; i++)
      {
        Vector3 position = new Vector3(random.NextFloat(-3, -8), 0, random.NextFloat(0, -5));
        Matrix33F orientation = Matrix33F.CreateRotationY(random.NextFloat(0, ConstantsF.TwoPi));
        float scale = random.NextFloat(0.5f, 1.2f);
        GameObjectService.Objects.Add(new StaticObject(Services, "PalmTree/palm_tree.drmdl", scale, new Pose(position, orientation)));
      }

      // Add a grain filter to add some noise in the night.
      _graphicsScreen.PostProcessors.Add(new GrainFilter(GraphicsService)
      {
        IsAnimated = true,
        LuminanceThreshold = 0.3f,
        ScaleWithLuminance = true,
        Strength = 0.04f,
        GrainScale = 1.5f,
      });
    }


    public override void Update(GameTime gameTime)
    {
      // This sample clears the debug renderer each frame.
      _graphicsScreen.DebugRenderer.Clear();
    }
  }
}