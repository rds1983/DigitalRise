using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRise.Geometry;
using DigitalRise.Geometry.Meshes;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Graphics;
using DigitalRise.Graphics.Effects;
using DigitalRise.Graphics.Rendering;
using DigitalRise.Graphics.SceneGraph;
using DigitalRise.Mathematics;
using DigitalRise.Physics.Constraints;
using DigitalRise.Physics.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using MathHelper = DigitalRise.Mathematics.MathHelper;


namespace Samples
{
	/// <summary>
	/// Provides useful helper and extension methods.
	/// </summary>
	public static class SampleHelper
	{
		public const int DefaultSpacing = 8;
		public const int DefaultLabelWidth = 250;

		//--------------------------------------------------------------
		#region Graphics
		//--------------------------------------------------------------

		/// <overloads>
		/// <summary>
		/// Creates a graphics mesh with the triangle mesh data of the given shape.
		/// </summary>
		/// </overloads>    
		/// <summary>
		/// Creates a graphics mesh with the triangle mesh data of the given shape and a default 
		/// material.
		/// </summary>
		/// <param name="assetManager">The contentManager manager.</param>
		/// <param name="graphicsService">The graphics service.</param>
		/// <param name="shape">The shape.</param>
		/// <returns>The graphics mesh.</returns>
		public static Mesh CreateMesh(IGraphicsService graphicsService, Shape shape)
		{
			return CreateMesh(graphicsService, shape, new Vector3(1), new Vector3(1), 100);
		}


		/// <summary>
		/// Creates a graphics mesh with the triangle mesh data of the given shape and the given
		/// diffuse and specular material properties.
		/// </summary>
		/// <param name="assetManager">The contentManager manager.</param>
		/// <param name="graphicsService">The graphics service.</param>
		/// <param name="shape">The shape.</param>
		/// <param name="diffuse">The diffuse material color.</param>
		/// <param name="specular">The specular material color.</param>
		/// <param name="specularPower">The specular power of the material.</param>
		/// <returns>The graphics mesh.</returns>
		public static Mesh CreateMesh(IGraphicsService graphicsService, Shape shape,
		  Vector3 diffuse, Vector3 specular, float specularPower)
		{
			// Create a DigitalRise.Geometry.Meshes.TriangleMesh from the shape and 
			// convert this to a DigitalRise.Graphics.Mesh.
			TriangleMesh triangleMesh = shape.GetMesh(0.01f, 4);

			Submesh submesh = CreateSubmeshWithTexCoords(
			  graphicsService.GraphicsDevice,
			  triangleMesh,
			  MathHelper.ToRadians(70));

			var mesh = CreateMesh(graphicsService, submesh, diffuse, specular, specularPower);

			// Set bounding shape to a box that is equal to the AABB of the shape.
			var aabb = shape.GetAabb(Pose.Identity);
			var boxShape = new BoxShape(aabb.Extent);
			var center = aabb.Center;
			if (center.IsNumericallyZero())
				mesh.BoundingShape = boxShape;
			else
				mesh.BoundingShape = new TransformedShape(new GeometricObject(boxShape, new Pose(center)));

			return mesh;
		}


		/// <summary>
		/// Creates a graphics mesh with the triangle mesh data of the given shape and the given
		/// diffuse and specular material properties.
		/// </summary>
		/// <param name="assetManager">The contentManager manager.</param>
		/// <param name="graphicsService">The graphics service.</param>
		/// <param name="submesh">The submesh.</param>
		/// <param name="diffuse">The diffuse material color.</param>
		/// <param name="specular">The specular material color.</param>
		/// <param name="specularPower">The specular power of the material.</param>
		/// <returns>The graphics mesh.</returns>
		/// <remarks>
		/// This method does not set the bounding shape of the mesh. (The default is an infinite shape
		/// which is not optimal for performance.)
		/// </remarks>
		public static Mesh CreateMesh(IGraphicsService graphicsService, Submesh submesh,
		  Vector3 diffuse, Vector3 specular, float specularPower)
		{
			Mesh mesh = new Mesh();
			mesh.Submeshes.Add(submesh);

			// Build material.
			// We could load a predefined material (*.drmat file)
			// with the content manager.
			//var material = contentManager.Load<Material>("MyMaterialName");

			// Alternatively, we can load some effects and build the material here:
			Material material = new Material();

			// We need an EffectBinding for each render pass. 
			// The "Default" pass uses a BasicEffectBinding (which is an EffectBinding
			// for the XNA BasicEffect). 
			// Note: The "Default" pass is not used by the DeferredLightingScreen, so
			// we could ignore this pass in this sample project.
			BasicEffectBinding defaultEffectBinding = new BasicEffectBinding(graphicsService, null)
			{
				LightingEnabled = true,
				TextureEnabled = true,
				VertexColorEnabled = false
			};
			defaultEffectBinding.Set("Texture", graphicsService.GetDefaultTexture2DWhite());
			defaultEffectBinding.Set("DiffuseColor", new Vector4((Vector3)diffuse, 1));
			defaultEffectBinding.Set("SpecularColor", (Vector3)specular);
			defaultEffectBinding.Set("SpecularPower", specularPower);
			material.Add("Default", defaultEffectBinding);

			// EffectBinding for the "ShadowMap" pass.
			// Note: EffectBindings which are used in a Material must be marked with 
			// the EffectParameterHint Material.
			EffectBinding shadowMapEffectBinding = new EffectBinding(
			  graphicsService,
			  graphicsService.GetStockEffect("DigitalRise\\Materials\\ShadowMap"),
			  null,
			  EffectParameterHint.Material);
			material.Add("ShadowMap", shadowMapEffectBinding);

			// EffectBinding for the "GBuffer" pass.
			EffectBinding gBufferEffectBinding = new EffectBinding(
			  graphicsService,
			  graphicsService.GetStockEffect("DigitalRise\\Materials\\GBuffer"),
			  null,
			  EffectParameterHint.Material);
			gBufferEffectBinding.Set("SpecularPower", specularPower);
			material.Add("GBuffer", gBufferEffectBinding);

			// EffectBinding for the "Material" pass.
			EffectBinding materialEffectBinding = new EffectBinding(
			  graphicsService,
			  graphicsService.GetStockEffect("DigitalRise\\Materials\\Material"),
			  null,
			  EffectParameterHint.Material);
			materialEffectBinding.Set("DiffuseTexture", graphicsService.GetDefaultTexture2DWhite());
			materialEffectBinding.Set("DiffuseColor", (Vector3)diffuse);
			materialEffectBinding.Set("SpecularColor", (Vector3)specular);
			material.Add("Material", materialEffectBinding);

			// Assign this material to the submesh.
			submesh.SetMaterial(material);

			return mesh;
		}


		/// <summary>
		/// Creates a submesh to draw a triangle mesh, similar to 
		/// <see cref="MeshHelper.CreateSubmesh(GraphicsDevice,TriangleMesh,float)"/>, but in addition
		/// adds dummy texture coordinates because they are required by many shader.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device.</param>
		/// <param name="mesh">The mesh.</param>
		/// <param name="angleLimit">
		/// The angle limit for normal vectors in radians. Normals are only merged if the angle between
		/// the triangle normals is equal to or less than the angle limit. Set this value to -1 to
		/// disable the angle limit (all normals of one vertex are merged). 
		/// </param>
		/// <returns>The submesh, or <see langword="null"/> if the mesh is empty.</returns>
		public static Submesh CreateSubmeshWithTexCoords(GraphicsDevice graphicsDevice, TriangleMesh mesh, float angleLimit)
		{
			var numberOfTriangles = mesh.NumberOfTriangles;
			if (numberOfTriangles == 0)
				return null;

			var submesh = new Submesh
			{
				PrimitiveType = PrimitiveType.TriangleList,
				PrimitiveCount = numberOfTriangles,
				VertexCount = numberOfTriangles * 3,
			};

			// Create vertex data for a triangle list.
			var vertices = new VertexPositionNormalTexture[submesh.VertexCount];

			// Create vertex normals. 
			var normals = mesh.ComputeNormals(false, angleLimit);

			for (int i = 0; i < numberOfTriangles; i++)
			{
				var i0 = mesh.Indices[i * 3 + 0];
				var i1 = mesh.Indices[i * 3 + 1];
				var i2 = mesh.Indices[i * 3 + 2];

				var v0 = mesh.Vertices[i0];
				var v1 = mesh.Vertices[i1];
				var v2 = mesh.Vertices[i2];

				Vector3 n0, n1, n2;
				if (angleLimit < 0)
				{
					// If the angle limit is negative, ComputeNormals() returns one normal per vertex.
					n0 = normals[i0];
					n1 = normals[i1];
					n2 = normals[i2];
				}
				else
				{
					// If the angle limits is >= 0, ComputeNormals() returns 3 normals per triangle.
					n0 = normals[i * 3 + 0];
					n1 = normals[i * 3 + 1];
					n2 = normals[i * 3 + 2];
				}

				// Add new vertex data.
				// DigitalRise.Geometry uses counter-clockwise front faces. XNA uses
				// clockwise front faces (CullMode.CullCounterClockwiseFace) per default. 
				// Therefore we change the vertex orientation of the triangles. 
				vertices[i * 3 + 0] = new VertexPositionNormalTexture((Vector3)v0, (Vector3)n0, new Vector2());
				vertices[i * 3 + 1] = new VertexPositionNormalTexture((Vector3)v2, (Vector3)n2, new Vector2());  // v2 instead of v1!
				vertices[i * 3 + 2] = new VertexPositionNormalTexture((Vector3)v1, (Vector3)n1, new Vector2());
			}

			// Create a vertex buffer.
			submesh.VertexBuffer = new VertexBuffer(
			  graphicsDevice,
			  typeof(VertexPositionNormalTexture),
			  vertices.Length,
			  BufferUsage.None);
			submesh.VertexBuffer.SetData(vertices);

			return submesh;
		}


		/// <summary>
		/// Enables the per-pixel lighting for all contained meshes.
		/// </summary>
		/// <param name="node">The scene node.</param>
		public static void EnablePerPixelLighting(SceneNode node)
		{
			var effectBindings = node.MeshNodes()
									 .SelectMany(meshNode => meshNode.Mesh.Materials)
									 .SelectMany(material => material.EffectBindings);

			foreach (var effectBinding in effectBindings)
			{
				if (effectBinding is BasicEffectBinding)
					((BasicEffectBinding)effectBinding).PreferPerPixelLighting = true;
				else if (effectBinding is SkinnedEffectBinding)
					((SkinnedEffectBinding)effectBinding).PreferPerPixelLighting = true;
			}
		}
		#endregion


		//--------------------------------------------------------------
		#region Physics Constraints
		//--------------------------------------------------------------

		/// <summary>
		/// Visualizes the constraints of the ragdoll (for debugging).
		/// </summary>
		/// <param name="debugRenderer">The debug renderer.</param>
		/// <param name="ragdoll">The ragdoll.</param>
		/// <param name="scale">
		/// A scale factor that determines the size of the drawn elements.
		/// </param>
		/// <param name="drawOverScene">
		/// If set to <see langword="true"/> the object is drawn over the graphics scene (depth-test 
		/// disabled).
		/// </param>
		/// <remarks>
		/// Currently, only <see cref="TwistSwingLimit" />s and <see cref="AngularLimit" />s are
		/// supported.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="debugRenderer" /> or <paramref name="ragdoll" /> is <see langword="null" />.
		/// </exception>
		public static void DrawConstraints(this DebugRenderer debugRenderer, Ragdoll ragdoll, float scale, bool drawOverScene)
		{
			if (debugRenderer == null)
				throw new ArgumentNullException("debugRenderer");
			if (ragdoll == null)
				throw new ArgumentNullException("ragdoll");

			// Render information for each limit.
			foreach (Constraint limit in ragdoll.Limits)
			{
				// Get the ball joint constraint that connects the two bodies of the limit.
				BallJoint joint = null;
				foreach (Constraint constraint in ragdoll.Joints)
				{
					if (constraint.BodyA == limit.BodyA && constraint.BodyB == limit.BodyB
						|| constraint.BodyA == limit.BodyB && constraint.BodyB == limit.BodyA)
					{
						joint = constraint as BallJoint;
						break;
					}
				}

				// Skip this limit if no joint was found.
				if (joint == null)
					continue;

				TwistSwingLimit twistSwingLimit = limit as TwistSwingLimit;
				if (twistSwingLimit != null)
				{
					DrawTwistSwingLimit(debugRenderer, joint, twistSwingLimit, scale, drawOverScene);
					continue;
				}

				AngularLimit angularLimit = limit as AngularLimit;
				if (angularLimit != null)
				{
					DrawAngularLimit(debugRenderer, joint, angularLimit, scale, drawOverScene);
					continue;
				}
			}
		}


		/// <summary>
		/// Visualizes the <see cref="TwistSwingLimit"/> of a <see cref="BallJoint"/>.
		/// </summary>
		/// <param name="debugRenderer">The debug renderer.</param>
		/// <param name="joint">The joint.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="scale">
		/// A scale factor that determines the size of the drawn elements.
		/// </param>
		/// <param name="drawOverScene">
		/// If set to <see langword="true"/> the object is drawn over the graphics scene (depth-test 
		/// disabled).
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="debugRenderer" />, <paramref name="joint" />,  or <paramref name="limit" />
		/// is <see langword="null" />.
		/// </exception>
		public static void DrawTwistSwingLimit(this DebugRenderer debugRenderer, BallJoint joint, TwistSwingLimit limit, float scale, bool drawOverScene)
		{
			if (debugRenderer == null)
				throw new ArgumentNullException("debugRenderer");
			if (joint == null)
				throw new ArgumentNullException("joint");
			if (limit == null)
				throw new ArgumentNullException("limit");

			// ----- Draw swing cone.
			// The tip of the swing cone:
			Vector3 coneTip = joint.BodyA.Pose.ToWorldPosition(joint.AnchorPositionALocal);

			// The first point on the swing cone:
			var previousConePoint = limit.GetPointOnCone(0, coneTip, scale);

			// Draw swing cone.
			const int numberOfSegments = 24;
			const float segmentAngle = ConstantsF.TwoPi / numberOfSegments;
			Color color = Color.Violet;
			for (int i = 0; i < numberOfSegments; i++)
			{
				var conePoint = limit.GetPointOnCone((i + 1) * segmentAngle, coneTip, scale);

				// Line from cone tip to cone base.
				debugRenderer.DrawLine(coneTip, conePoint, color, drawOverScene);

				// Line on the cone base.
				debugRenderer.DrawLine(previousConePoint, conePoint, color, drawOverScene);

				previousConePoint = conePoint;
			}

			// ----- Draw twist axis.      
			// The x-axis is the twist direction. 
			Vector3 twistAxis = Vector3.UnitX;
			// The twist axis relative to body B.
			Vector3 twistAxisDirectionBLocal = limit.AnchorOrientationBLocal * twistAxis;
			// The twist axis relative to world space.
			Vector3 twistAxisDirection = limit.BodyB.Pose.ToWorldDirection(twistAxisDirectionBLocal);
			// (A similar computation is used in DrawArc() below.)

			// Line in twist direction.
			debugRenderer.DrawLine(coneTip, coneTip + twistAxisDirection * scale, Color.Red, drawOverScene);

			// A transformation that converts from constraint anchor space to world space.
			Pose constraintToWorld = limit.BodyA.Pose * new Pose(limit.AnchorOrientationALocal);

			// Draw an arc that visualizes the twist limits.
			DrawArc(debugRenderer, constraintToWorld, coneTip, Vector3.UnitX, Vector3.UnitY, limit.Minimum.X, limit.Maximum.X, scale, Color.Red, drawOverScene);
		}


		/// <summary>
		/// Visualizes the <see cref="AngularLimit"/> of a <see cref="BallJoint"/>.
		/// </summary>
		/// <param name="debugRenderer">The debug renderer.</param>
		/// <param name="joint">The joint.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="scale">
		/// A scale factor that determines the size of the drawn elements.
		/// </param>
		/// <param name="drawOverScene">
		/// If set to <see langword="true"/> the object is drawn over the graphics scene (depth-test 
		/// disabled).
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="debugRenderer" />, <paramref name="joint" />,  or <paramref name="limit" />
		/// is <see langword="null" />.
		/// </exception>
		public static void DrawAngularLimit(this DebugRenderer debugRenderer, BallJoint joint, AngularLimit limit, float scale, bool drawOverScene)
		{
			if (debugRenderer == null)
				throw new ArgumentNullException("debugRenderer");
			if (joint == null)
				throw new ArgumentNullException("joint");
			if (limit == null)
				throw new ArgumentNullException("limit");

			Vector3 jointPosition = joint.BodyA.Pose.ToWorldPosition(joint.AnchorPositionALocal);

			// A transformation that converts from constraint anchor space to world space.
			Pose constraintToWorld = limit.BodyA.Pose * new Pose(limit.AnchorOrientationALocal);

			// Draw an arc for each rotation axis. 
			DrawArc(debugRenderer, constraintToWorld, jointPosition, Vector3.UnitX, Vector3.UnitY, limit.Minimum.X, limit.Maximum.X, scale, Color.Red, drawOverScene);
			DrawArc(debugRenderer, constraintToWorld, jointPosition, Vector3.UnitY, Vector3.UnitX, limit.Minimum.Y, limit.Maximum.Y, scale, Color.Green, drawOverScene);
			DrawArc(debugRenderer, constraintToWorld, jointPosition, Vector3.UnitZ, Vector3.UnitX, limit.Minimum.Z, limit.Maximum.Z, scale, Color.Blue, drawOverScene);
		}


		/// <summary>
		/// Draws an arc to visualize a rotation limit about an axis.
		/// </summary>
		/// <param name="debugRenderer">The debug renderer.</param>
		/// <param name="constraintToWorld">
		/// A transformation that transforms from constraint anchor space to world space.
		/// </param>
		/// <param name="center">The center of the circle.</param>
		/// <param name="axis">The rotation axis.</param>
		/// <param name="direction">A direction vector (e.g. the direction of a bone).</param>
		/// <param name="minimum">The minimum angle.</param>
		/// <param name="maximum">The maximum angle.</param>
		/// <param name="scale">The scale.</param>
		/// <param name="color">The color.</param>
		/// <param name="drawOverScene">
		/// If set to <see langword="true"/> the object is drawn over the graphics scene (depth-test 
		/// disabled).
		/// </param>
		private static void DrawArc(this DebugRenderer debugRenderer, Pose constraintToWorld, Vector3 center, Vector3 axis, Vector3 direction, float minimum, float maximum, float scale, Color color, bool drawOverScene)
		{
			if (minimum == 0 && maximum == 0)
				return;

			// Line from circle center to start of arc.
			Vector3 previousArcPoint = center + scale * constraintToWorld.ToWorldDirection(MathHelper.CreateRotation(axis, minimum).Rotate(direction));
			debugRenderer.DrawLine(center, previousArcPoint, color, drawOverScene);

			// Draw arc.
			int numberOfSegments = (int)Math.Max((maximum - minimum) / (ConstantsF.Pi / 24), 1);
			float segmentAngle = (maximum - minimum) / numberOfSegments;
			for (int i = 0; i < numberOfSegments; i++)
			{
				Vector3 arcPoint = center + scale * constraintToWorld.ToWorldDirection(MathHelper.CreateRotation(axis, minimum + (i + 1) * segmentAngle).Rotate(direction));
				debugRenderer.DrawLine(previousArcPoint, arcPoint, color, drawOverScene);
				previousArcPoint = arcPoint;
			}

			// Line from end of arc to circle center.
			debugRenderer.DrawLine(previousArcPoint, center, color, drawOverScene);
		}
		#endregion


		//--------------------------------------------------------------
		#region GUI
		//--------------------------------------------------------------

		// Following methods add UI controls to a given parent panel or parent control.
		// This is used to quickly add controls to the Options window.

		// Adds a TabItem to a TabControl.
		public static MultipleItemsContainerBase AddTabItem(TabControl parent, string title, int index)
		{
			var tabItem = new TabItem
			{

				Text = title,
				Content = new Label { Text = title },
			};
			var scrollViewer = new ScrollViewer
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				ShowHorizontalScrollBar = false
			};
			tabItem.Content = scrollViewer;
			var panel = new VerticalStackPanel
			{
				Spacing = DefaultSpacing
			};
			scrollViewer.Content = panel;

			if (index < 0)
				parent.Items.Add(tabItem);
			else
				parent.Items.Insert(Math.Min(index, parent.Items.Count), tabItem);

			return panel;
		}


		public static MultipleItemsContainerBase AddTabItem(TabControl parent, string title)
		{
			return AddTabItem(parent, title, -1);
		}


		public static MultipleItemsContainerBase AddGroupBox(MultipleItemsContainerBase parent, string title)
		{
/*			var groupBox = new GroupBox
			{
				Title = title,
				Margin = new Thickness(0, 0, 0, Margin),
				HorizontalAlignment = HorizontalAlignment.Stretch,

				// For GroupBoxes inside TabControls we need a different style 
				// (different title background color).
				TitleLabelStyle = "GroupBoxTitleInTabPage",
			};
			parent.Widgets.Add(groupBox);*/

			var panel = new VerticalStackPanel
			{
				Spacing = DefaultSpacing
			};

			parent.Widgets.Add(panel);

			return panel;
		}


		public static TextButton AddButton(MultipleItemsContainerBase parent, string title, Action clickHandler, string toolTip)
		{
			var button = new TextButton
			{
				Text = title,
			};
			button.Click += (s, e) => clickHandler();

			parent.Widgets.Add(button);
			return button;
		}


		public static CheckBox AddCheckBox(MultipleItemsContainerBase parent, string title, bool defaultValue, Action<bool> isCheckedHandler, string toolTip)
		{
			var checkBox = new CheckBox
			{
				Text = title,
				IsChecked = defaultValue,
			};
			checkBox.Click += (s, e) => isCheckedHandler(((CheckBox)s).IsChecked);
			parent.Widgets.Add(checkBox);
			return checkBox;
		}


		public static CheckBox AddCheckBox(MultipleItemsContainerBase parent, string title, bool defaultValue, Action<bool> isCheckedHandler)
		{
			return AddCheckBox(parent, title, defaultValue, isCheckedHandler, null);
		}


		public static void AddDropDown<T>(MultipleItemsContainerBase parent, string title, IList<T> items, int selectedIndex, Action<T> selectedIndexChangedHandler, string toolTip)
		{
			var horizontalStackPanel = new HorizontalStackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Spacing = DefaultSpacing
			};

			parent.Widgets.Add(horizontalStackPanel);

			horizontalStackPanel.Widgets.Add(new Label
			{
				Text = title + ": ",
				Margin = new Thickness(0, 4, 0, 0),
				Width = DefaultLabelWidth,
			});

			var dropDownButton = new ComboBox
			{
				SelectedIndex = selectedIndex,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};

			StackPanel.SetProportionType(dropDownButton, ProportionType.Fill);
			horizontalStackPanel.Widgets.Add(dropDownButton);

			foreach (var item in items)
			{
				dropDownButton.Items.Add(new ListItem(item.ToString()));
			}

			dropDownButton.SelectedIndexChanged += (s, e) =>
			{
				if (dropDownButton.SelectedIndex == null)
				{
					return;
				}

				selectedIndexChangedHandler(items[dropDownButton.SelectedIndex.Value]);
			};
		}


		public static void AddDropDown<T>(MultipleItemsContainerBase parent, string title, IList<T> items, int selectedIndex, Action<T> selectedIndexChangedHandler)
		{
			AddDropDown(parent, title, items, selectedIndex, selectedIndexChangedHandler, null);
		}


		public static MultipleItemsContainerBase AddSlider(MultipleItemsContainerBase parent, string title, string format, float min, float max,
									 float defaultValue, Action<float> valueChangedHandler, string toolTip)
		{
			var horizontalStackPanel = new HorizontalStackPanel
			{
				Spacing = DefaultSpacing
			};

			parent.Widgets.Add(horizontalStackPanel);

			var textBlock = new Label
			{
				Text = title + ": " + defaultValue.ToString(format),
				Width = DefaultLabelWidth,
			};
			horizontalStackPanel.Widgets.Add(textBlock);

			var slider = new HorizontalSlider
			{
				Minimum = min,
				Maximum = max,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Tag = title
			};

			StackPanel.SetProportionType(slider, ProportionType.Fill);
			horizontalStackPanel.Widgets.Add(slider);

			// Coerce value to integers?
			if (format == "F0")
			{
/*				valueProperty.Changing += (s, e) =>
				{
					e.CoercedValue = (float)Math.Round(e.CoercedValue);
				};*/
			}

			slider.Value = defaultValue;

			slider.ValueChanged += (s, e) =>
			{
				valueChangedHandler(slider.Value);
				textBlock.Text = title + ": " + slider.Value.ToString(format);
			};

			return horizontalStackPanel;
		}


		public static MultipleItemsContainerBase AddSlider(MultipleItemsContainerBase parent, string title, string format, float min, float max,
									 float defaultValue, Action<float> valueChangedHandler)
		{
			return AddSlider(parent, title, format, min, max, defaultValue, valueChangedHandler, null);
		}
		#endregion


		//--------------------------------------------------------------
		#region Miscellaneous
		//--------------------------------------------------------------

		/// <summary>
		/// Gets a human-readable exception message for an exception instance.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns>A exception message which can be displayed in a message box.</returns>
		public static string GetExceptionMessage(Exception exception)
		{
			var stringBuilder = new StringBuilder();

			// Exception on main thread.
			stringBuilder.AppendLine("An unexpected error has occurred.");
			WriteMessagesAndStackTraces(stringBuilder, exception);

			return stringBuilder.ToString();
		}


		private static void WriteMessagesAndStackTraces(StringBuilder stringBuilder, Exception exception)
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Exception text:");
			while (exception != null)
			{
				stringBuilder.AppendLine(exception.Message);
				stringBuilder.AppendLine(exception.StackTrace);
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Inner exception:");

				// Continue with inner exception.
				exception = exception.InnerException;
			}

			stringBuilder.AppendLine("-");
		}
		#endregion
	}
}
