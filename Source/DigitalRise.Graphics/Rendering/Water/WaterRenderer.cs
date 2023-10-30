// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using DigitalRise.Geometry;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MathHelper = DigitalRise.Mathematics.MathHelper;
using Plane = DigitalRise.Geometry.Shapes.Plane;


namespace DigitalRise.Graphics.Rendering
{
	/// <summary>
	/// Renders <see cref="WaterNode"/>s.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Water is rendered using a refraction effect. Therefore, the renderer needs a texture which
	/// contains the current scene. This texture should be set in
	/// <see cref="RenderContext.SourceTexture">RenderContext.SourceTexture</see>. If no such texture
	/// is provided, the renderer uses the current render target, sets a new
	/// <see cref="RenderContext.RenderTarget">RenderContext.RenderTarget</see> and restores the depth
	/// buffer.
	/// </para>
	/// <para>
	/// Currently, vertex displacement from <see cref="WaterWaves"/> and water flow from
	/// <see cref="WaterFlow"/> are mutually exclusive. Only one of those effects is rendered.
	/// </para>
	/// <para>
	/// If the <see cref="WaterNode"/> represents an infinite plane with waves, the renderer uses a
	/// "projected grid" to render the water surface. The grid parameters can be changed in 
	/// <see cref="ProjectedGridParameters"/>.
	/// </para>
	/// <para>
	/// <strong>Render Target and Viewport:</strong><br/>
	/// This renderer renders into the current render target and viewport of the graphics device.
	/// </para>
	/// </remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
	public class WaterRenderer : SceneNodeRenderer
	{
		private enum ShaderType
		{
			Simple,
			Foam,
			Flow,
			FoamFlow,
			Displacement,
			DisplacementFoam,
			DisplacementFoamCaustics,
			ProjectedGrid,
			ProjectedGridFoam,
			ProjectedGridFoamCaustics
		}

		private enum ReflectionType
		{
			None,
			Planar,
			Cube
		}

		private const int EffectsCount = 256;

		private static EffectData[] _effectCache = new EffectData[EffectsCount];

		private class EffectData
		{
			public readonly Effect _effect;
			public readonly EffectParameter _parameterView;
			public readonly EffectParameter _parameterProjection;
			public readonly EffectParameter _parameterCameraParameters;
			public readonly EffectParameter _parameterViewportSize;
			public readonly EffectParameter _parameterTime;

			public readonly EffectParameter _parameterAmbientLight;
			public readonly EffectParameter _parameterDirectionalLightDirection;
			public readonly EffectParameter _parameterDirectionalLightIntensity;

			public readonly EffectParameter _parameterGBuffer0;
			public readonly EffectParameter _parameterRefractionTexture;

			public readonly EffectParameter _parameterPushedBackCameraPosition;
			public readonly EffectParameter _parameterNearCorners;
			public readonly EffectParameter _parameterProjectedGridParameters;

			public readonly EffectParameter _parameterWorld;
			public readonly EffectParameter _parameterSurfaceLevel;

			public readonly EffectParameter _parameterReflectionTypeParameters;
			public readonly EffectParameter _parameterReflectionMatrix;

			public readonly EffectParameter _parameterReflectionTextureSize;
			public readonly EffectParameter _parameterPlanarReflectionMap;
			public readonly EffectParameter _parameterCubeReflectionMap;

			public readonly EffectParameter _parameterNormalMap0;
			public readonly EffectParameter _parameterNormalMap1;
			public readonly EffectParameter _parameterNormalMap0Parameters;
			public readonly EffectParameter _parameterNormalMap1Parameters;
			public readonly EffectParameter _parameterSpecularParameters;
			public readonly EffectParameter _parameterReflectionParameters;
			public readonly EffectParameter _parameterRefractionParameters;
			public readonly EffectParameter _parameterUnderwaterFogParameters;
			public readonly EffectParameter _parameterFresnelParameters;
			public readonly EffectParameter _parameterIntersectionSoftness;
			public readonly EffectParameter _parameterWaterColor;
			public readonly EffectParameter _parameterScatterColor;
			public readonly EffectParameter _parameterFoamMap;
			public readonly EffectParameter _parameterFoamParameters0;
			public readonly EffectParameter _parameterFoamParameters1;
			public readonly EffectParameter _parameterCausticsSampleCount;
			public readonly EffectParameter _parameterCausticsParameters;

			public readonly EffectParameter _parameterWaveMapParameters;
			public readonly EffectParameter _parameterWaveMapSize;
			public readonly EffectParameter _parameterDisplacementTexture;
			public readonly EffectParameter _parameterWaveNormalMap;

			public readonly EffectParameter _parameterFlowParameters0;
			public readonly EffectParameter _parameterFlowParameters1;
			public readonly EffectParameter _parameterFlowMapTextureMatrix;
			public readonly EffectParameter _parameterFlowMapWorldMatrix;
			public readonly EffectParameter _parameterFlowMap;
			public readonly EffectParameter _parameterNoiseMap;

			public readonly EffectParameter _parameterFogColor0;
			public readonly EffectParameter _parameterFogColor1;
			public readonly EffectParameter _parameterFogHeights;
			public readonly EffectParameter _parameterFogParameters;
			public readonly EffectParameter _parameterFogScatteringSymmetry;

			public readonly EffectParameter _parameterCameraMisc;

			public EffectData(Effect effect)
			{
				_effect = effect;

				_parameterView = _effect.Parameters["View"];
				_parameterProjection = _effect.Parameters["Projection"];
				_parameterCameraParameters = _effect.Parameters["CameraParameters"];
				_parameterViewportSize = _effect.Parameters["ViewportSize"];
				_parameterTime = _effect.Parameters["Time"];

				_parameterAmbientLight = _effect.Parameters["AmbientLight"];
				_parameterDirectionalLightDirection = _effect.Parameters["DirectionalLightDirection"];
				_parameterDirectionalLightIntensity = _effect.Parameters["DirectionalLightIntensity"];

				_parameterGBuffer0 = _effect.Parameters["GBuffer0"];
				_parameterRefractionTexture = _effect.Parameters["RefractionTexture"];

				_parameterPushedBackCameraPosition = _effect.Parameters["PushedBackCameraPosition"];
				_parameterNearCorners = _effect.Parameters["NearCorners"];

				_parameterWorld = _effect.Parameters["World"];
				_parameterSurfaceLevel = _effect.Parameters["SurfaceLevel"];
				_parameterProjectedGridParameters = _effect.Parameters["ProjectedGridParameters"];

				_parameterReflectionTypeParameters = _effect.Parameters["ReflectionTypeParameters"];
				_parameterReflectionMatrix = _effect.Parameters["ReflectionMatrix"];
				_parameterReflectionTextureSize = _effect.Parameters["ReflectionTextureSize"];
				_parameterPlanarReflectionMap = _effect.Parameters["PlanarReflectionMap"];
				_parameterCubeReflectionMap = _effect.Parameters["CubeReflectionMap"];

				_parameterNormalMap0 = _effect.Parameters["NormalMap0"];
				_parameterNormalMap1 = _effect.Parameters["NormalMap1"];
				_parameterNormalMap0Parameters = _effect.Parameters["NormalMap0Parameters"];
				_parameterNormalMap1Parameters = _effect.Parameters["NormalMap1Parameters"];
				_parameterSpecularParameters = _effect.Parameters["SpecularParameters"];
				_parameterReflectionParameters = _effect.Parameters["ReflectionParameters"];
				_parameterRefractionParameters = _effect.Parameters["RefractionParameters"];
				_parameterUnderwaterFogParameters = _effect.Parameters["UnderwaterFogParameters"];
				_parameterFresnelParameters = _effect.Parameters["FresnelParameters"];
				_parameterIntersectionSoftness = _effect.Parameters["IntersectionSoftness"];
				_parameterWaterColor = _effect.Parameters["WaterColor"];
				_parameterScatterColor = _effect.Parameters["ScatterColor"];
				_parameterFoamMap = _effect.Parameters["FoamMap"];
				_parameterFoamParameters0 = _effect.Parameters["FoamParameters0"];
				_parameterFoamParameters1 = _effect.Parameters["FoamParameters1"];
				_parameterCausticsSampleCount = _effect.Parameters["CausticsSampleCount"];
				_parameterCausticsParameters = _effect.Parameters["CausticsParameters"];

				_parameterWaveMapParameters = _effect.Parameters["WaveMapParameters"];
				_parameterWaveMapSize = _effect.Parameters["WaveMapSize"];
				_parameterDisplacementTexture = _effect.Parameters["DisplacementTexture"];
				_parameterWaveNormalMap = _effect.Parameters["WaveNormalMap"];
				_parameterFlowParameters0 = _effect.Parameters["FlowParameters0"];
				_parameterFlowParameters1 = _effect.Parameters["FlowParameters1"];
				_parameterFlowMapTextureMatrix = _effect.Parameters["FlowMapTextureMatrix"];
				_parameterFlowMapWorldMatrix = _effect.Parameters["FlowMapWorldMatrix"];
				_parameterFlowMap = _effect.Parameters["FlowMap"];
				_parameterNoiseMap = _effect.Parameters["NoiseMap"];

				_parameterFogColor0 = _effect.Parameters["FogColor0"];
				_parameterFogColor1 = _effect.Parameters["FogColor1"];
				_parameterFogHeights = _effect.Parameters["FogHeights"];
				_parameterFogParameters = _effect.Parameters["FogParameters"];
				_parameterFogScatteringSymmetry = _effect.Parameters["FogScatteringSymmetry"];

				_parameterCameraMisc = _effect.Parameters["CameraMisc"];
			}

			public void Apply(bool underwater = false)
			{
				if (underwater)
				{
					_effect.CurrentTechnique.Passes[1].Apply();
				}
				else
				{
					_effect.CurrentTechnique.Passes[0].Apply();
				}
			}
		}

		// TODO:
		// To remove pulsing problem:
		// - Try to scale normals down when 1 normal map is fully visible. (When both
		//   normals are visible and averaged, then the normals are less intense (because average).
		//   Maybe the pulsing is reduced if we find a way to keep a uniform normal intensity...)


		//--------------------------------------------------------------
		#region Fields
		//--------------------------------------------------------------

		private readonly IGraphicsService _graphicsService;
		private RebuildZBufferRenderer _defaultRebuildZBufferRenderer;

		// For rendering if WaterNode.Volume is null.
		private Submesh _boxSubmesh;
		private Submesh _quadSubmesh;

		// For projected grid.
		private Vector3[] _projectedGridNearCorners;

		// Noise texture for flow.
		private readonly Texture2D _noiseMap;
		private readonly List<int> _usedEffectsMap = new List<int>();
		#endregion


		//--------------------------------------------------------------
		#region Properties & Events
		//--------------------------------------------------------------

		/// <summary>
		/// Gets the projected grid parameters.
		/// </summary>
		/// <value>The projected grid parameters.</value>
		public ProjectedGridParameters ProjectedGridParameters { get; private set; }
		#endregion


		//--------------------------------------------------------------
		#region Creation & Cleanup
		//--------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance of the <see cref="WaterRenderer"/> class.
		/// </summary>
		/// <param name="graphicsService">The graphics service.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="graphicsService"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// The current graphics profile is Reach.
		/// </exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly")]
		public WaterRenderer(IGraphicsService graphicsService)
		{
			if (graphicsService == null)
				throw new ArgumentNullException("graphicsService");

			if (graphicsService.GraphicsDevice.GraphicsProfile == GraphicsProfile.Reach)
				throw new NotSupportedException("The WaterRenderer does not support the Reach profile.");

			_graphicsService = graphicsService;
			_noiseMap = NoiseHelper.GetNoiseTexture(graphicsService, 64, 5);

			ProjectedGridParameters = new ProjectedGridParameters(graphicsService.GraphicsDevice);
		}


		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					_noiseMap.Dispose();

					if (_quadSubmesh != null)
					{
						_quadSubmesh.Dispose();
						_quadSubmesh = null;
					}

					ProjectedGridParameters.Dispose();
				}
			}

			base.Dispose(disposing);
		}
		#endregion


		//--------------------------------------------------------------
		#region Methods
		//--------------------------------------------------------------

		private static int GetEffectKey(ShaderType shaderType, ReflectionType reflectionType, bool underwater, bool fog)
		{
			// 4 bits for shader type, 2 bits for reflection type, 1 bit for underwater & fog
			int result = 0;
			result |= ((int)shaderType) << 4;
			result |= ((int)reflectionType) << 2;
			if (underwater)
			{
				result |= 2;
			}

			if (fog)
			{
				result |= 1;
			}

			return result;
		}

		private static EffectData GetEffect(IGraphicsService service,
		  ShaderType shaderType, ReflectionType reflectionType, bool underwater, bool fog)
		{
			var key = GetEffectKey(shaderType, reflectionType, underwater, fog);
			if (_effectCache[key] != null)
			{
				return _effectCache[key];
			}

			var dict = new Dictionary<string, string>();

			switch (shaderType)
			{
				case ShaderType.Simple:
					break;
				case ShaderType.Foam:
					dict["FOAM"] = "1";
					break;
				case ShaderType.Flow:
					dict["FLOW"] = "1";
					break;
				case ShaderType.FoamFlow:
					dict["FOAM"] = "1";
					dict["FLOW"] = "1";
					break;
				case ShaderType.Displacement:
					dict["DISPLACEMENT"] = "1";
					break;
				case ShaderType.DisplacementFoam:
					dict["DISPLACEMENT"] = "1";
					dict["FOAM"] = "1";
					break;
				case ShaderType.DisplacementFoamCaustics:
					dict["DISPLACEMENT"] = "1";
					dict["FOAM"] = "1";
					dict["CAUSTICS"] = "1";
					break;
				case ShaderType.ProjectedGrid:
					dict["PROJECTED_GRID"] = "1";
					dict["DISPLACEMENT"] = "1";
					break;
				case ShaderType.ProjectedGridFoam:
					dict["PROJECTED_GRID"] = "1";
					dict["DISPLACEMENT"] = "1";
					dict["FOAM"] = "1";
					break;
				case ShaderType.ProjectedGridFoamCaustics:
					dict["PROJECTED_GRID"] = "1";
					dict["DISPLACEMENT"] = "1";
					dict["FOAM"] = "1";
					dict["CAUSTICS"] = "1";
					break;
			}

			if (reflectionType == ReflectionType.Planar)
			{
				dict["REFLECTION_PLANAR"] = "1";
			}
			else if (reflectionType == ReflectionType.Cube)
			{
				dict["REFLECTION_CUBE"] = "1";
			}

			if (underwater)
			{
				dict["UNDERWATER"] = "1";
			}

			if (fog)
			{
				dict["FOG"] = "1";
			}

			var effect = service.GetStockEffect("Water/Water", dict);
			_effectCache[key] = new EffectData(effect);
			return _effectCache[key];
		}

		/// <inheritdoc/>
		public override bool CanRender(SceneNode node, RenderContext context)
		{
			return node is WaterNode;
		}


		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly")]
		public override void Render(IList<SceneNode> nodes, RenderContext context, RenderOrder order)
		{
			ThrowIfDisposed();

			if (nodes == null)
				throw new ArgumentNullException("nodes");
			if (context == null)
				throw new ArgumentNullException("context");

			int numberOfNodes = nodes.Count;
			if (numberOfNodes == 0)
				return;

			context.ThrowIfCameraMissing();

			float deltaTime = (float)context.DeltaTime.TotalSeconds;

			var graphicsService = context.GraphicsService;
			var graphicsDevice = graphicsService.GraphicsDevice;
			var renderTargetPool = graphicsService.RenderTargetPool;

			var cameraNode = context.CameraNode;
			Projection projection = cameraNode.Camera.Projection;
			Pose view = cameraNode.PoseWorld.Inverse;

			// Around the camera we push the waves down to avoid that the camera cuts the near plane.
			// Get largest vector from camera to near plane corners.
			float nearPlaneRadius =
			  new Vector3(Math.Max(Math.Abs(projection.Right), Math.Abs(projection.Left)),
						   Math.Max(Math.Abs(projection.Top), Math.Abs(projection.Bottom)),
						   projection.Near
						  ).Length();

			var originalSourceTexture = context.SourceTexture;

			// Update SceneNode.LastFrame for all visible nodes.
			int frame = context.Frame;
			cameraNode.LastFrame = frame;

			var savedRenderState = new RenderStateSnapshot(graphicsDevice);

			// Water surface is opaque.
			graphicsDevice.BlendState = BlendState.Opaque;

			#region ----- Common Effect Parameters -----

			for (int i = 0; i < numberOfNodes; i++)
			{
				var node = nodes[i] as WaterNode;
				if (node == null)
					continue;

				bool isCameraUnderwater = node.EnableUnderwaterEffect && node.IsUnderwater(cameraNode.PoseWorld.Position);
				// Waves should not cut the near plane. --> Bend waves up or down if necessary.

				// Limits
				float upperLimit; // Waves must not move above this value.
				float lowerLimit; // Waves must not move below this value.

				if (!isCameraUnderwater)
				{
					// Bend waves down below the camera.
					upperLimit = cameraNode.PoseWorld.Position.Y - nearPlaneRadius;
					lowerLimit = -1e20f;

					if (node.EnableUnderwaterEffect)
					{
						if (node.Waves == null || node.Waves.DisplacementMap == null)
						{
							// No displacement. The wave bending stuff does not work because the surface
							// is usually not tessellated. We have to render the underwater geometry when
							// camera near plane might cut the water surface.
							if (node.Volume == null)
							{
								// Test water plane.
								isCameraUnderwater = (cameraNode.PoseWorld.Position.Y - nearPlaneRadius) < node.PoseWorld.Position.Y;
							}
							else
							{
								// Test water AABB.
								var aabb = node.Aabb;
								aabb.Minimum -= new Vector3(nearPlaneRadius);
								aabb.Maximum += new Vector3(nearPlaneRadius);
								isCameraUnderwater = GeometryHelper.HaveContact(aabb, cameraNode.PoseWorld.Position);
							}
						}
					}
				}
				else
				{
					// Camera is underwater, bend triangles up above camera.
					upperLimit = 1e20f;
					lowerLimit = cameraNode.PoseWorld.Position.Y + nearPlaneRadius;
				}

				var useProjectedGrid = true;
				if (node.Volume != null)
				{
					useProjectedGrid = false;
				}
				else if (node.Waves == null || node.Waves.DisplacementMap == null)
				{
					useProjectedGrid = false;
				}

				var fogNodes = context.Scene.Query<FogQuery>(cameraNode, context).FogNodes;
				var hasFog = HasFog(fogNodes);
				var effectData = GetEffectData(node, useProjectedGrid, isCameraUnderwater, hasFog);
				context.Validate(effectData._effect);

				effectData._parameterView.SetValue(view);
				effectData._parameterProjection.SetValue(projection);
				effectData._parameterCameraParameters.SetValue(new Vector4(
					(Vector3)cameraNode.PoseWorld.Position,
					cameraNode.Camera.Projection.Far));

				var viewport = graphicsDevice.Viewport;
				effectData._parameterViewportSize.SetValue(new Vector2(viewport.Width, viewport.Height));

				effectData._parameterTime.SetValue((float)context.Time.TotalSeconds);

				// Query ambient and directional lights.
				var lightQuery = context.Scene.Query<GlobalLightQuery>(cameraNode, context);
				Vector3 ambientLight = Vector3.Zero;
				if (lightQuery.AmbientLights.Count > 0)
				{
					var light = (AmbientLight)lightQuery.AmbientLights[0].Light;
					ambientLight = light.Color * light.Intensity * light.HdrScale;
				}

				effectData._parameterAmbientLight.SetValue((Vector3)ambientLight);

				Vector3 directionalLightDirection = new Vector3(0, -1, 0);
				Vector3 directionalLightIntensity = Vector3.Zero;
				if (lightQuery.DirectionalLights.Count > 0)
				{
					var lightNode = lightQuery.DirectionalLights[0];
					var light = (DirectionalLight)lightNode.Light;
					directionalLightDirection = -lightNode.PoseWorld.Orientation.GetColumn(2);
					directionalLightIntensity = light.Color * light.SpecularIntensity * light.HdrScale;
				}

				effectData._parameterDirectionalLightDirection.SetValue((Vector3)directionalLightDirection);
				effectData._parameterDirectionalLightIntensity.SetValue((Vector3)directionalLightIntensity);

				effectData._parameterGBuffer0.SetValue(context.GBuffer0);

				if (effectData._parameterNoiseMap != null)
					effectData._parameterNoiseMap.SetValue(_noiseMap);
				#endregion

				#region ----- Fog Parameters -----

				SetFogParameters(effectData, fogNodes, cameraNode, directionalLightDirection);
				#endregion

				effectData._parameterProjectedGridParameters.SetValue(new Vector3(
						ProjectedGridParameters.EdgeAttenuation,
						ProjectedGridParameters.DistanceAttenuationStart,
						ProjectedGridParameters.DistanceAttenuationEnd));

				// Node is visible in current frame.
				node.LastFrame = frame;

				var data = node.RenderData as WaterRenderData;
				if (data == null)
				{
					data = new WaterRenderData();
					node.RenderData = data;
				}

				var water = node.Water;

				#region ----- Wave bending -----

				// Bending fades in over interval [bendStart, bendEnd]:
				//   distance ≤ bendStart ............. Wave is bent up or down.
				//   bendStart < distance < bendEnd ... Lerp between normal wave and bent wave.
				//   distance ≥ bendEnd ............... Normal wave.
				float bendStart = 1 * nearPlaneRadius;
				float bendEnd = 10 * nearPlaneRadius;

				effectData._parameterCameraMisc.SetValue(new Vector4(upperLimit, lowerLimit, bendStart, bendEnd));
				#endregion

				// Update the submesh for the given water volume.
				data.UpdateSubmesh(graphicsService, node);

				#region ----- Scroll Normal Maps -----

				// We update the normal map offsets once(!) per frame.
				// Note: We could skip the offsets and compute all in the shader using absolute
				// time instead of deltaTime, but then the user cannot change the NormalMapVelocity
				// smoothly.
				if (data.LastNormalUpdateFrame != frame)
				{
					data.LastNormalUpdateFrame = frame;

					var baseVelocity = (node.Flow != null) ? node.Flow.BaseVelocity : Vector3.Zero;

					// Increase offset.
					// (Note: We have to subtract value and divide by scale because if the normal
					// should scroll to the right, we have to move the texcoords in the other direction.)
					data.NormalMapOffset0.X -= (water.NormalMap0Velocity.X + baseVelocity.X) * deltaTime / water.NormalMap0Scale;
					data.NormalMapOffset0.Y -= (water.NormalMap0Velocity.Z + baseVelocity.Y) * deltaTime / water.NormalMap0Scale;
					data.NormalMapOffset1.X -= (water.NormalMap1Velocity.X + baseVelocity.X) * deltaTime / water.NormalMap1Scale;
					data.NormalMapOffset1.Y -= (water.NormalMap1Velocity.Z + baseVelocity.Y) * deltaTime / water.NormalMap1Scale;

					// Keep only the fractional part to avoid overflow.
					data.NormalMapOffset0.X = MathHelper.Frac(data.NormalMapOffset0.X);
					data.NormalMapOffset0.Y = MathHelper.Frac(data.NormalMapOffset0.Y);
					data.NormalMapOffset1.X = MathHelper.Frac(data.NormalMapOffset1.X);
					data.NormalMapOffset1.Y = MathHelper.Frac(data.NormalMapOffset1.Y);
				}
				#endregion

				effectData._parameterSurfaceLevel.SetValue(node.PoseWorld.Position.Y);

				#region ----- Reflection Parameters -----

				var reflectionType = GetReflectionType(node);
				switch (reflectionType)
				{
					case ReflectionType.None:
						{
							// No reflection texture. The reflection shows only the ReflectionColor.
							effectData._parameterReflectionTypeParameters.SetValue(new Vector2(-1, 1));
							effectData._parameterReflectionParameters.SetValue(new Vector4(
								(Vector3)water.ReflectionColor,
								water.ReflectionDistortion));
						}
						break;
					case ReflectionType.Planar:
						{
							// Planar reflection.
							var renderToTexture = node.PlanarReflection.RenderToTexture;
							var texture = (Texture2D)renderToTexture.Texture;

							effectData._parameterReflectionTypeParameters.SetValue(new Vector2(0, 1));
							effectData._parameterReflectionMatrix.SetValue((Matrix)renderToTexture.TextureMatrix);
							effectData._parameterReflectionTextureSize.SetValue(new Vector2(texture.Width, texture.Height));
							if (effectData._parameterPlanarReflectionMap != null)
								effectData._parameterPlanarReflectionMap.SetValue(texture);

							effectData._parameterReflectionParameters.SetValue(new Vector4(
								(Vector3)water.ReflectionColor,
								water.ReflectionDistortion));
						}
						break;
					case ReflectionType.Cube:
						{
							// Cube map reflection.
							var rgbmEncoding = node.SkyboxReflection.Encoding as RgbmEncoding;
							float rgbmMax = 1;
							if (rgbmEncoding != null)
								rgbmMax = GraphicsHelper.ToGamma(rgbmEncoding.Max);
							else if (!(node.SkyboxReflection.Encoding is SRgbEncoding))
								throw new NotImplementedException("The reflected skybox must be encoded using sRGB or RGBM.");

							effectData._parameterReflectionTypeParameters.SetValue(new Vector2(1, rgbmMax));

							// Cube maps are left handed --> Sample with inverted z. (Otherwise, the 
							// cube map and objects or texts in it are mirrored.)
							var mirrorZ = Matrix44F.CreateScale(1, 1, -1);
							Matrix33F orientation = node.SkyboxReflection.PoseWorld.Orientation;
							effectData._parameterReflectionMatrix.SetValue((Matrix)(new Matrix44F(orientation, Vector3.Zero) * mirrorZ));

							if (effectData._parameterCubeReflectionMap != null)
								effectData._parameterCubeReflectionMap.SetValue(node.SkyboxReflection.Texture);

							effectData._parameterReflectionParameters.SetValue(new Vector4(
								(Vector3)(water.ReflectionColor * node.SkyboxReflection.Color),
								water.ReflectionDistortion));
						}
						break;
				}

				#endregion

				#region ----- Refraction Parameters -----

				// If we do not have a source texture, resolve the current render target
				// and immediately rebuilt it.
				if (context.SourceTexture == null && context.RenderTarget != null)
				{
					// Get RebuildZBufferRenderer from RenderContext.
					RebuildZBufferRenderer rebuildZBufferRenderer = null;
					object obj;
					if (context.Data.TryGetValue(RenderContextKeys.RebuildZBufferRenderer, out obj))
						rebuildZBufferRenderer = obj as RebuildZBufferRenderer;

					// If we didn't find the renderer in the context, use a default instance.
					if (rebuildZBufferRenderer == null)
					{
						if (_defaultRebuildZBufferRenderer == null)
							_defaultRebuildZBufferRenderer = new RebuildZBufferRenderer(graphicsService);

						rebuildZBufferRenderer = _defaultRebuildZBufferRenderer;
					}

					context.SourceTexture = context.RenderTarget;
					context.RenderTarget = renderTargetPool.Obtain2D(new RenderTargetFormat(context.RenderTarget));
					graphicsDevice.SetRenderTarget(context.RenderTarget);
					graphicsDevice.Viewport = context.Viewport;
					rebuildZBufferRenderer.Render(context, context.SourceTexture);
				}

				effectData._parameterRefractionTexture.SetValue(context.SourceTexture);
				effectData._parameterRefractionParameters.SetValue(new Vector4(
		  ((Vector3)water.RefractionColor),
		  water.RefractionDistortion));
				#endregion

				#region ----- Other Water Effect Parameters -----

				if (water.NormalMap0 != null)
				{
					if (effectData._parameterNormalMap0 != null)
						effectData._parameterNormalMap0.SetValue(water.NormalMap0);

					effectData._parameterNormalMap0Parameters.SetValue(new Vector4(
			1 / water.NormalMap0Scale,
			data.NormalMapOffset0.X,
			data.NormalMapOffset0.Y,
			water.NormalMap0Strength));
				}
				else
				{
					if (effectData._parameterNormalMap0 != null)
						effectData._parameterNormalMap0.SetValue(_graphicsService.GetDefaultNormalTexture());
					effectData._parameterNormalMap0Parameters.SetValue(new Vector4(1, 0, 0, 0));
				}

				if (water.NormalMap1 != null)
				{
					if (effectData._parameterNormalMap1 != null)
						effectData._parameterNormalMap1.SetValue(water.NormalMap1);

					effectData._parameterNormalMap1Parameters.SetValue(new Vector4(
			1 / water.NormalMap1Scale,
			data.NormalMapOffset1.X,
			data.NormalMapOffset1.Y,
			water.NormalMap1Strength));
				}
				else
				{
					if (effectData._parameterNormalMap1 != null)
						effectData._parameterNormalMap1.SetValue(_graphicsService.GetDefaultNormalTexture());
					effectData._parameterNormalMap1Parameters.SetValue(new Vector4(1, 0, 0, 0));
				}

				effectData._parameterSpecularParameters.SetValue(new Vector4((Vector3)water.SpecularColor, water.SpecularPower));
				effectData._parameterUnderwaterFogParameters.SetValue((Vector3)water.UnderwaterFogDensity);
				effectData._parameterFresnelParameters.SetValue(new Vector3(water.FresnelBias, water.FresnelScale, water.FresnelPower));
				effectData._parameterIntersectionSoftness.SetValue(water.IntersectionSoftness);

				// We apply some arbitrary scale factors to the water and scatter colors to
				// move the values into a similar range from the user's perspective.
				effectData._parameterWaterColor.SetValue((Vector3)water.WaterColor / 10);
				effectData._parameterScatterColor.SetValue((Vector3)water.ScatterColor);

				if (effectData._parameterFoamMap != null)
				{
					effectData._parameterFoamMap.SetValue(water.FoamMap);
					effectData._parameterFoamParameters0.SetValue(new Vector4(
			(Vector3)water.FoamColor,
			1 / water.FoamMapScale));

					effectData._parameterFoamParameters1.SetValue(new Vector4(
			water.FoamDistortion,
			water.FoamShoreIntersection,
			// Enable crest foam only if we have waves.
			node.Waves != null ? water.FoamCrestMin : float.MaxValue,
			water.FoamCrestMax));
				}

				effectData._parameterCausticsSampleCount.SetValue(water.CausticsSampleCount);
				effectData._parameterCausticsParameters.SetValue(new Vector4(
		  water.CausticsSampleOffset,
		  water.CausticsDistortion,
		  water.CausticsPower,
		  water.CausticsIntensity));
				#endregion

				#region ----- Wave Map -----

				var waves = node.Waves;

				// The displacement map can be null but the normal map must not be null.
				if (waves != null && waves.NormalMap != null)
				{
					// Type: 0 = Tiling, 1 = Clamp.
					float waveType;
					if (waves.IsTiling)
						waveType = 0;
					else
						waveType = 1;

					effectData._parameterWaveMapParameters.SetValue(new Vector4(
			1.0f / waves.TileSize,                          // Scale
			0.5f - waves.TileCenter.X / waves.TileSize,     // Offset X
			0.5f - waves.TileCenter.Z / waves.TileSize,     // Offset Y
			waveType));

					if (effectData._parameterDisplacementTexture != null)
					{
						if (waves.DisplacementMap != null)
							effectData._parameterDisplacementTexture.SetValue(waves.DisplacementMap);
						else
							effectData._parameterDisplacementTexture.SetValue(graphicsService.GetDefaultTexture2DBlack4F());
					}

					effectData._parameterWaveMapSize.SetValue(new Vector2(
			waves.NormalMap.Width,
			waves.NormalMap.Height));
					if (effectData._parameterWaveNormalMap != null)
						effectData._parameterWaveNormalMap.SetValue(waves.NormalMap);
				}
				else
				{
					effectData._parameterWaveMapParameters.SetValue(new Vector4(0, 0, 0, 0));
				}
				#endregion

				#region ----- Flow -----

				if (node.Flow != null)
				{
					var flow = node.Flow;
					float flowMapSpeed = (flow.FlowMap != null) ? flow.FlowMapSpeed : 0;
					effectData._parameterFlowParameters0.SetValue(new Vector4(flow.SurfaceSlopeSpeed, flowMapSpeed, flow.CycleDuration, flow.MaxSpeed));
					effectData._parameterFlowParameters1.SetValue(new Vector3(flow.MinStrength, 1 / flow.NoiseMapScale, flow.NoiseMapStrength));

					if (effectData._parameterFlowMap != null)
						effectData._parameterFlowMap.SetValue(flow.FlowMap);

					// Get world space (x, z) to texture space matrix.
					Aabb aabb = node.Shape.GetAabb();
					Vector3 extent = aabb.Extent;
					Matrix44F m = Matrix44F.CreateScale(1 / extent.X, 1, 1 / extent.Z)
								  * Matrix44F.CreateTranslation(-aabb.Minimum.X, 0, -aabb.Minimum.Z)
								  * Matrix44F.CreateScale(1 / node.ScaleLocal.X, 1, 1 / node.ScaleLocal.Z)
								  * node.PoseWorld.Inverse;

					// We use a 3x3 2d scale/rotation/translation matrix, ignoring the y component.
					effectData._parameterFlowMapTextureMatrix.SetValue(new Matrix(m.M00, m.M20, 0, 0,
															 m.M02, m.M22, 0, 0,
															 m.M03, m.M23, 1, 0,
															 0, 0, 0, 0));

					// Get local flow direction to world flow direction matrix.
					// We use a 2x2 2d rotation matrix, ignoring the y component.
					var r = node.PoseWorld.Orientation;
					effectData._parameterFlowMapWorldMatrix.SetValue(new Matrix(r.M00, r.M20, 0, 0,
														   r.M02, r.M22, 0, 0,
														   0, 0, 0, 0,
														   0, 0, 0, 0));
				}
				else
				{
					effectData._parameterFlowParameters0.SetValue(new Vector4(0, 0, 0, 0));
					effectData._parameterFlowParameters1.SetValue(new Vector3(0, 0, 0));
				}
				#endregion

				if (isCameraUnderwater)
					RenderUnderwaterGeometry(effectData, node, cameraNode);

				RenderSurface(effectData, node, cameraNode, isCameraUnderwater);
			}

			foreach (var effectKey in _usedEffectsMap)
			{
				var effectData = _effectCache[effectKey];

				// Reset texture effect parameters.
				effectData._parameterGBuffer0.SetValue((Texture2D)null);
				effectData._parameterRefractionTexture.SetValue((Texture2D)null);

				if (effectData._parameterPlanarReflectionMap != null)
					effectData._parameterPlanarReflectionMap.SetValue((Texture2D)null);

				if (effectData._parameterCubeReflectionMap != null)
					effectData._parameterCubeReflectionMap.SetValue((TextureCube)null);

				if (effectData._parameterNormalMap0 != null)
					effectData._parameterNormalMap0.SetValue((Texture2D)null);

				if (effectData._parameterNormalMap1 != null)
					effectData._parameterNormalMap1.SetValue((Texture2D)null);

				if (effectData._parameterDisplacementTexture != null)
					effectData._parameterDisplacementTexture.SetValue((Texture2D)null);

				if (effectData._parameterNoiseMap != null)
					effectData._parameterNoiseMap.SetValue((Texture2D)null);

				if (effectData._parameterWaveNormalMap != null)
					effectData._parameterWaveNormalMap.SetValue((Texture2D)null);

				if (effectData._parameterFlowMap != null)
					effectData._parameterFlowMap.SetValue((Texture2D)null);
			}

			_usedEffectsMap.Clear();

			// This seems to be necessary because the Displacement Texture (vertex texture!)
			// is not automatically removed from the texture stage, and the WaterWavesRenderer
			// cannot write into it. XNA Bug!?
			//      _passProjectedGrid.Apply();

			savedRenderState.Restore();

			// Restore original render context.
			if (originalSourceTexture == null)
			{
				// Current render target has been resolved and used as source texture.
				// A new render target (from pool) has been set. (See region "Refraction Parameters".)
				// --> Previous render target needs to be recycled.
				renderTargetPool.Recycle(context.SourceTexture);
			}

			context.SourceTexture = originalSourceTexture;
		}


		private void RenderUnderwaterGeometry(EffectData effectData, WaterNode node, CameraNode cameraNode)
		{
			var graphicsDevice = _graphicsService.GraphicsDevice;
			graphicsDevice.RasterizerState = RasterizerState.CullNone;
			//graphicsDevice.RasterizerState = GraphicsHelper.RasterizerStateWireFrame;
			graphicsDevice.DepthStencilState = DepthStencilState.None;

			var data = ((WaterRenderData)node.RenderData);
			var submesh = data.Submesh;
			if (submesh != null)
			{
				// User-defined volume
				effectData._parameterWorld.SetValue((Matrix)(
				  node.PoseWorld
				  * Matrix44F.CreateScale(node.ScaleWorld)
				  * data.SubmeshMatrix));
			}
			else
			{
				// Underwater rendering of infinite ocean.
				// --> Use a shared box around the camera.
				if (_boxSubmesh == null)
					_boxSubmesh = MeshHelper.GetBox(_graphicsService);

				// The box must not be clipped at near or far plane:
				// Make the box extent (size, size / 2, size) with size / 2 = (near + far) / 2.
				var projection = cameraNode.Camera.Projection;
				float size = projection.Near + projection.Far;

				// The box is centered on the camera. (Ignore camera orientation in case
				// the camera has a roll.)
				Vector3 position = cameraNode.PoseWorld.Position;

				// Top of box must go through water node origin, except when waves are 
				// rendered. (Waves are bent up or down at the near plane.)
				if (node.Waves == null || node.Waves.DisplacementMap == null)
					position.Y = node.PoseWorld.Position.Y - size / 4.0f;

				var world = Matrix44F.CreateTranslation(position) * Matrix44F.CreateScale(size, size / 2.0f, size);
				effectData._parameterWorld.SetValue((Matrix)world);

				submesh = _boxSubmesh;
			}

			// Underwater pass
			effectData.Apply(true);
			submesh.Draw();
		}


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		private void RenderSurface(EffectData effectData, WaterNode node, CameraNode cameraNode, bool isCameraUnderwater)
		{
			var graphicsDevice = _graphicsService.GraphicsDevice;
			var projection = cameraNode.Camera.Projection;

			graphicsDevice.RasterizerState = RasterizerState.CullNone;
			//graphicsDevice.RasterizerState = isCameraUnderwater ? RasterizerState.CullClockwise : RasterizerState.CullCounterClockwise;
			//graphicsDevice.RasterizerState = GraphicsHelper.RasterizerStateWireFrame;

			graphicsDevice.DepthStencilState = node.DepthBufferWriteEnable
											   ? DepthStencilState.Default
											   : DepthStencilState.DepthRead;

			// TODO: Support gamma corrected LDR rendering.
			//if (context.IsHdrEnabled()) _passGamma...

			if (node.Volume != null)
			{
				// ----- Render with user-defined water volume.
				var data = ((WaterRenderData)node.RenderData);

				effectData._parameterWorld.SetValue(
				  (Matrix)(node.PoseWorld
					* Matrix44F.CreateScale(node.ScaleWorld)
					* data.SubmeshMatrix));
				effectData.Apply();
				data.Submesh.Draw();
			}
			else if (node.Waves == null || node.Waves.DisplacementMap == null)
			{
				// ----- Infinite ocean without displacement map.
				// Draw using simple, gigantic quad.
				if (_quadSubmesh == null)
				{
					// This is the really lazy way to get a quad submesh. :-P
					var quadShape = new TransformedShape(new GeometricObject(
					  new RectangleShape(1, 1),
					  new Pose(Matrix33F.CreateRotationX(-ConstantsF.PiOver2)))).GetMesh(0.001f, 4);

					_quadSubmesh = MeshHelper.CreateSubmesh(graphicsDevice, quadShape, -1);
				}

				// Position the quad under the camera and choose a size large enough to cover everything.
				Vector3 position = cameraNode.PoseWorld.Position;
				position.Y = node.PoseWorld.Position.Y;

				// Add a bit to make sure that the surface is rendered above the underwater geometry.
				// Without the epsilon if the camera cuts the surface, there might be a horizontal ~1 pixel 
				// line when the surface quad ends before the underwater shape.
				position.Y += Numeric.EpsilonF * 10;

				float farPlaneRadius =
				  new Vector3(Math.Max(Math.Abs(projection.Right), Math.Abs(projection.Left)),
							   Math.Max(Math.Abs(projection.Top), Math.Abs(projection.Bottom)),
							   projection.Far
							  ).Length();
				float size = 2 * farPlaneRadius;

				Matrix44F world = Matrix44F.CreateTranslation(position) * Matrix44F.CreateScale(size, 1, size);
				effectData._parameterWorld.SetValue((Matrix)world);

				effectData.Apply();
				_quadSubmesh.Draw();
			}
			else
			{
				// ----- Use Projected Grid.
				if (SetProjectedGridParameters(effectData, node, cameraNode, isCameraUnderwater))
				{
					effectData.Apply();
					ProjectedGridParameters.Submesh.Draw();
				}
			}
		}

		private static ReflectionType GetReflectionType(WaterNode node)
		{
			if (node.PlanarReflection != null
					&& node.PlanarReflection.ActualIsEnabled
					&& node.PlanarReflection.RenderToTexture.Texture is Texture2D)
			{
				return ReflectionType.Planar;
			}
			else if (node.SkyboxReflection != null)
			{
				return ReflectionType.Cube;
			}

			return ReflectionType.None;
		}


		private EffectData GetEffectData(WaterNode node, bool useProjectedGrid, bool underwater, bool fog)
		{
			var reflectionType = GetReflectionType(node);

			var shaderType = ShaderType.Simple;
			if (!useProjectedGrid)
			{
				// Displacement mapping active if we have at least a wave normal map.
				if (node.Waves != null && node.Waves.NormalMap != null)
				{
					if (node.Water.CausticsIntensity > 0)
					{
						shaderType = ShaderType.DisplacementFoamCaustics;
					}
					else if (node.Water.FoamMap != null)
					{
						shaderType = ShaderType.DisplacementFoam;
					}
					else
					{
						shaderType = ShaderType.Displacement;
					}
				}
				else
				{
					if (node.Water.FoamMap != null && node.Flow != null)
					{
						shaderType = ShaderType.FoamFlow;
					}
					else if (node.Water.FoamMap != null)
					{
						shaderType = ShaderType.Foam;
					}
					else if (node.Flow != null)
					{
						shaderType = ShaderType.Flow;
					}
				}
			}
			else
			{
				if (node.Water.CausticsIntensity > 0)
				{
					shaderType = ShaderType.ProjectedGridFoamCaustics;
				}
				else if (node.Water.FoamMap != null)
				{
					shaderType = ShaderType.ProjectedGridFoam;
				}
				else
				{
					shaderType = ShaderType.ProjectedGrid;
				}
			}

			var key = GetEffectKey(shaderType, reflectionType, underwater, fog);
			var result = GetEffect(_graphicsService, shaderType, reflectionType, underwater, fog);

			if (!_usedEffectsMap.Contains(key))
			{
				_usedEffectsMap.Add(key);
			}

			return result;
		}

		// Returns false if the water is not visible.
		private bool SetProjectedGridParameters(EffectData effectData, WaterNode node, CameraNode cameraNode, bool isCameraUnderwater)
		{
			var projection = cameraNode.Camera.Projection;

			// Push projection camera back behind the original camera to get a borders
			// around the visible FOV.
			Pose cameraPose = cameraNode.PoseWorld;
			Matrix33F cameraOrientation = cameraPose.Orientation;
			Vector3 cameraUp = cameraOrientation.GetColumn(1);
			Vector3 cameraBack = cameraOrientation.GetColumn(2);

			Vector3 pushedBackCameraPosition = cameraPose.Position + cameraBack * ProjectedGridParameters.Offset;

			if (_projectedGridNearCorners == null)
				_projectedGridNearCorners = new Vector3[4];

			float seaLevel = node.PoseWorld.Position.Y;

			// Get view space vectors from camera to near plane.
			Vector3 rightTopDirection = new Vector3(projection.Right, projection.Top, -projection.Near);
			Vector3 leftTopDirection = new Vector3(projection.Left, projection.Top, -projection.Near);
			Vector3 leftBottomDirection = new Vector3(projection.Left, projection.Bottom, -projection.Near);
			Vector3 rightBottomDirection = new Vector3(projection.Right, projection.Bottom, -projection.Near);

			// Transform vectors to world space directions.
			rightTopDirection = cameraPose.ToWorldDirection(rightTopDirection);
			leftTopDirection = cameraPose.ToWorldDirection(leftTopDirection);
			leftBottomDirection = cameraPose.ToWorldDirection(leftBottomDirection);
			rightBottomDirection = cameraPose.ToWorldDirection(rightBottomDirection);

			// Projected grid is a quad which must cover the frustum near plane parts which show the
			// water surface. This part is bound by the horizon plane (horizontal plane through camera
			// position) and the water plane.
			// In the simple academic case, the projected grid quad is equal to the near plane quad.
			// This makes sense if the camera is looking down.
			if (!Numeric.IsZero(cameraUp.Y))
			{
				// Camera is NOT exactly looking down:
				// In general the camera will show part of the water and part of the sky and a lot projected
				// grid triangles are wasted.
				// For this case it is better to compute a projected grid quad which covers the necessary
				// camera near plane quad more tightly.

				// If camera is exactly at sea level, push it away a bit to avoid a degenerate case
				// where the whole grid is projected into infinity...
				if (Numeric.AreEqual(pushedBackCameraPosition.Y, seaLevel))
				{
					if (isCameraUnderwater)
						pushedBackCameraPosition.Y -= Numeric.EpsilonF;
					else
						pushedBackCameraPosition.Y += Numeric.EpsilonF;
				}

				// Convert near plane corners to world space.
				// Transform vectors to world space directions.
				Vector3 rightTopPosition = rightTopDirection + pushedBackCameraPosition;
				Vector3 leftTopPosition = leftTopDirection + pushedBackCameraPosition;
				Vector3 rightBottomPosition = rightBottomDirection + pushedBackCameraPosition;
				Vector3 leftBottomPosition = leftBottomDirection + pushedBackCameraPosition;

				// Get min and max y of the corners.
				float minY = Math.Min(rightTopPosition.Y, leftTopPosition.Y);
				float maxY = Math.Max(rightTopPosition.Y, leftTopPosition.Y);
				minY = Math.Min(minY, rightBottomPosition.Y);
				maxY = Math.Max(maxY, rightBottomPosition.Y);
				minY = Math.Min(minY, leftBottomPosition.Y);
				maxY = Math.Max(maxY, leftBottomPosition.Y);
				Debug.Assert(minY < maxY);

				// Vertically the projected grid is bound by the horizon plane and the water plane.
				Plane topPlane = new Plane(Vector3.UnitY, pushedBackCameraPosition.Y);
				Plane bottomPlane = new Plane(Vector3.UnitY, seaLevel);
				if (seaLevel > pushedBackCameraPosition.Y)
				{
					// Camera is under water.
					Mathematics.MathHelper.Swap(ref topPlane, ref bottomPlane);
				}

				Debug.Assert(bottomPlane.DistanceFromOrigin < topPlane.DistanceFromOrigin);

				// Move planes vertically so that they touch the near plane quad.
				bottomPlane.DistanceFromOrigin = Math.Max(minY, bottomPlane.DistanceFromOrigin);
				topPlane.DistanceFromOrigin = Math.Min(maxY, topPlane.DistanceFromOrigin);

				// Abort if camera is not looking at water surface.
				if (bottomPlane.DistanceFromOrigin > topPlane.DistanceFromOrigin)
					return false;

				// Horizontally the projected grid quad is bound by two vertical planes which go through
				// the camera origin and the left-most and right-most near plane quad corners.
				// These plane are parallel to the camera up axis.
				// (The left vector in this case is orthogonal to world up. It is not the camera left.)
				Vector3 left = Vector3.Cross(cameraBack, Vector3.UnitY);

				// Get left-most and right-most corner.
				Vector3 leftMostCorner = rightTopPosition;
				float leftMostDistance = Vector3.Dot(left, rightTopDirection);
				Vector3 rightMostCorner = rightTopPosition;
				float rightMostDistance = -leftMostDistance;

				float d = Vector3.Dot(left, leftTopDirection);
				if (d > leftMostDistance)
				{
					leftMostCorner = leftTopPosition;
					leftMostDistance = d;
				}
				if (-d > rightMostDistance)
				{
					rightMostCorner = leftTopPosition;
					rightMostDistance = -d;
				}

				d = Vector3.Dot(left, rightBottomDirection);
				if (d > leftMostDistance)
				{
					leftMostCorner = rightBottomPosition;
					leftMostDistance = d;
				}
				if (-d > rightMostDistance)
				{
					rightMostCorner = rightBottomPosition;
					rightMostDistance = -d;
				}

				d = Vector3.Dot(left, leftBottomDirection);
				if (d > leftMostDistance)
				{
					leftMostCorner = leftBottomPosition;
					//leftMostDistance = d;
				}
				if (-d > rightMostDistance)
				{
					rightMostCorner = leftBottomPosition;
					//rightMostDistance = -d;
				}

				// The projected grid must cover a quad on the near plane. Let's compute a rotated
				// camera with the same near plane (= identical camera forward/backward direction) but
				// an up vector which is parallel to the world up vector.
				Vector3 rotatedCameraUp = Vector3.Cross(left, cameraBack).Normalized();

				// The side planes go through the left/right-most points and through the camera origin.
				// They are parallel to the up vector of the rotated camera.
				Plane leftPlane = new Plane(pushedBackCameraPosition, leftMostCorner, leftMostCorner + rotatedCameraUp);
				Plane rightPlane = new Plane(pushedBackCameraPosition, rightMostCorner, rightMostCorner + rotatedCameraUp);

				// The 4 planes (water, horizon, left, right) can be cut with the near plane to get
				// the projected grid quad.
				Plane nearPlane = new Plane(leftTopPosition, rightTopPosition, leftBottomPosition);

				rightTopPosition = GeometryHelper.GetIntersection(nearPlane, topPlane, rightPlane);
				leftTopPosition = GeometryHelper.GetIntersection(nearPlane, topPlane, leftPlane);
				rightBottomPosition = GeometryHelper.GetIntersection(nearPlane, bottomPlane, rightPlane);
				leftBottomPosition = GeometryHelper.GetIntersection(nearPlane, bottomPlane, leftPlane);

				rightTopDirection = rightTopPosition - pushedBackCameraPosition;
				leftTopDirection = leftTopPosition - pushedBackCameraPosition;
				rightBottomDirection = rightBottomPosition - pushedBackCameraPosition;
				leftBottomDirection = leftBottomPosition - pushedBackCameraPosition;

				// Choose near corners such that the triangles where texCoord.y is lower are
				// rendered in the far for correct depth-sorting.
				if (pushedBackCameraPosition.Y > seaLevel)
				{
					if (rightTopDirection.Y < rightBottomDirection.Y)
					{
						Mathematics.MathHelper.Swap(ref rightTopDirection, ref rightBottomDirection);
						Mathematics.MathHelper.Swap(ref leftTopDirection, ref leftBottomDirection);
					}
				}
				else
				{
					if (rightTopDirection.Y > rightBottomDirection.Y)
					{
						Mathematics.MathHelper.Swap(ref rightTopDirection, ref rightBottomDirection);
						Mathematics.MathHelper.Swap(ref leftTopDirection, ref leftBottomDirection);
					}
				}
			}
			else
			{
				// Camera is looking down.
				if (pushedBackCameraPosition.Y < seaLevel)
				{
					// Underwater looking up. We have to swap top and bottom.
					Mathematics.MathHelper.Swap(ref rightTopDirection, ref rightBottomDirection);
					Mathematics.MathHelper.Swap(ref leftTopDirection, ref leftBottomDirection);
				}
			}

			_projectedGridNearCorners[0] = (Vector3)rightTopDirection;
			_projectedGridNearCorners[1] = (Vector3)leftTopDirection;
			_projectedGridNearCorners[2] = (Vector3)rightBottomDirection;
			_projectedGridNearCorners[3] = (Vector3)leftBottomDirection;

			effectData._parameterPushedBackCameraPosition.SetValue((Vector3)pushedBackCameraPosition);
			effectData._parameterNearCorners.SetValue(_projectedGridNearCorners);
			effectData._parameterWorld.SetValue(node.PoseWorld);
			return true;
		}

		private static bool HasFog(IList<FogNode> fogNodes)
		{
			FogNode fogNode = null;
			Fog fog = null;
			if (fogNodes.Count > 0)
			{
				fogNode = fogNodes[0];
				fog = fogNode.Fog;
			}

			if (fogNode == null || fog.Density <= Numeric.EpsilonF)
			{
				return false;
			}

			return true;
		}


		// Following code was taken from the FogRenderer and modified.
		// We have to keep this in sync with the FogRenderer.
		private void SetFogParameters(EffectData effectData, IList<FogNode> fogNodes, CameraNode cameraNode, Vector3 lightDirection)
		{
			FogNode fogNode = null;
			Fog fog = null;
			if (fogNodes.Count > 0)
			{
				fogNode = fogNodes[0];
				fog = fogNode.Fog;
			}

			if (fogNode == null || fog.Density <= Numeric.EpsilonF)
			{
				effectData._parameterFogParameters.SetValue(new Vector4());
				effectData._parameterFogHeights.SetValue(new Vector3());
				effectData._parameterFogColor0.SetValue(new Vector4());
				effectData._parameterFogColor1.SetValue(new Vector4());
				return;
			}

			// Compute actual density and falloff.
			float fogDensity = fog.Density;
			float heightFalloff = fog.HeightFalloff;
			effectData._parameterFogParameters.SetValue(new Vector4(fog.Start, fog.End, fogDensity, heightFalloff));
			effectData._parameterFogColor0.SetValue((Vector4)fog.Color0);
			effectData._parameterFogColor1.SetValue((Vector4)fog.Color1);

			// Compute world space reference heights. 
			var heightRef = cameraNode.PoseWorld.Position.Y - fogNode.PoseWorld.Position.Y;
			var height0 = fogNode.PoseWorld.Position.Y + fog.Height0;
			var height1 = fogNode.PoseWorld.Position.Y + fog.Height1;
			// Avoid division by zero in the shader.
			if (Numeric.AreEqual(height0, height1))
				height1 = height0 + 0.0001f;

			// !!! This parameter is different from the code in FogRenderer.cs. !!!
			effectData._parameterFogHeights.SetValue(new Vector3(heightRef, height0, height1));

			var scatteringSymmetry = fog.ScatteringSymmetry;

			// The scattering symmetry must disappear at the end of the twilight when the sun is
			// below the horizon. 
			float scatteringSymmetryStrength;
			const float limit = 0.2f;
			if (lightDirection.Y < 0)
				scatteringSymmetryStrength = 1;
			else if (lightDirection.Y > limit)
				scatteringSymmetryStrength = 0;
			else
				scatteringSymmetryStrength = 1 - lightDirection.Y / limit;

			// Use phase function.
			// Set parameters for phase function.
			effectData._parameterFogScatteringSymmetry.SetValue((Vector3)scatteringSymmetry * scatteringSymmetryStrength);
		}
		#endregion
	}
}
