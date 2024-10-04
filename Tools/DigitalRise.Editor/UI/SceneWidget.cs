﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using DigitalRise;
using DigitalRise.Rendering;
using DigitalRise.Modelling;
using System.Collections.Generic;
using VertexPosition = DigitalRise.Rendering.Vertices.VertexPosition;
using DigitalRise.Utilities;
using static DigitalRise.Utilities.CameraInputController;
using DigitalRise.Rendering.Lights;
using DigitalRise.Standard;
using DigitalRise.Editor.Utility;
using Microsoft.Build.Construction;
using Myra.Graphics2D;

namespace DigitalRise.Editor.UI
{
	public class SceneWidget : Widget
	{
		private const int GridSize = 200;

		private static readonly Dictionary<Type, Texture2D> _typesIcons = new Dictionary<Type, Texture2D>
		{
			[typeof(BaseLight)] = DigitalRise.Editor.Resources.IconDirectionalLight,
			[typeof(Camera)] = DigitalRise.Editor.Resources.IconCamera
		};

		private Scene _scene;
		private readonly ForwardRenderer _renderer = new ForwardRenderer();
		private CameraInputController _controller;
		private MeshNode _gridMesh;
		private DigitalRise.Modelling.ModelMesh _waterMarker;
		private ModelInstance _modelMarker;
		private Vector3? _touchDownStart;
		private readonly bool[] _keysDown = new bool[256];

		public Scene Scene
		{
			get => _scene;
			set
			{
				if (_scene == value) return;

				_scene = value;
				_controller = _scene == null ? null : new CameraInputController(_scene.Camera);
			}
		}

		public ProjectInSolution Project { get; set; }

		public ForwardRenderer Renderer { get => _renderer; }
		public RenderStatistics RenderStatistics;
		public Instrument Instrument { get; } = new Instrument();

		private MeshNode GridMesh
		{
			get
			{
				if (_gridMesh == null)
				{
					var vertices = new List<VertexPosition>();
					var indices = new List<short>();

					short idx = 0;
					for (var x = -GridSize; x <= GridSize; ++x)
					{
						vertices.Add(new VertexPosition
						{
							Position = new Vector3(x, 0, -GridSize)
						});

						vertices.Add(new VertexPosition
						{
							Position = new Vector3(x, 0, GridSize)
						});

						indices.Add(idx);
						++idx;
						indices.Add(idx);
						++idx;
					}

					for (var z = -GridSize; z <= GridSize; ++z)
					{
						vertices.Add(new VertexPosition
						{
							Position = new Vector3(-GridSize, 0, z)
						});

						vertices.Add(new VertexPosition
						{
							Position = new Vector3(GridSize, 0, z)
						});

						indices.Add(idx);
						++idx;
						indices.Add(idx);
						++idx;
					}

					var mesh = new Mesh(vertices.ToArray(), indices.ToArray(), PrimitiveType.LineList);

					_gridMesh = new MeshNode
					{
						Mesh = mesh,
						Material = new ColorMaterial
						{
							Color = Color.Green,
						},
					};
				}

				return _gridMesh;
			}
		}


		private static bool IsMouseLeftButtonDown
		{
			get
			{
				var mouseState = Mouse.GetState();
				return (mouseState.LeftButton == ButtonState.Pressed);
			}
		}

		public SceneWidget()
		{
			ClipToBounds = true;
			AcceptsKeyboardFocus = true;
		}

		/*		private Vector3? CalculateMarkerPosition()
				{
					// Build viewport
					var bounds = ActualBounds;
					var p = ToGlobal(bounds.Location);
					bounds.X = p.X;
					bounds.Y = p.Y;
					var viewport = new Viewport(bounds.X, bounds.Y, bounds.Width, bounds.Height);

					// Determine marker position
					var nearPoint = new Vector3(Desktop.MousePosition.X, Desktop.MousePosition.Y, 0);
					var farPoint = new Vector3(Desktop.MousePosition.X, Desktop.MousePosition.Y, 1);

					nearPoint = viewport.Unproject(nearPoint, Renderer.Projection, Renderer.View, Matrix.Identity);
					farPoint = viewport.Unproject(farPoint, Renderer.Projection, Renderer.View, Matrix.Identity);

					var direction = (farPoint - nearPoint);
					direction.Normalize();

					var ray = new Ray(nearPoint, direction);

					// Firstly determine whether we intersect zero height terrain rectangle
					var bb = MathUtils.CreateBoundingBox(0, Scene.Terrain.Size.X, 0, 0, 0, Scene.Terrain.Size.Y);
					var intersectDist = ray.Intersects(bb);
					if (intersectDist == null)
					{
						return null;
					}

					var markerPosition = nearPoint + direction * intersectDist.Value;

					// Now determine where we intersect terrain rectangle with real height
					var height = Scene.Terrain.GetHeight(markerPosition.X, markerPosition.Z);
					bb = MathUtils.CreateBoundingBox(0, Scene.Terrain.Size.X, height, height, 0, Scene.Terrain.Size.Y);
					intersectDist = ray.Intersects(bb);
					if (intersectDist == null)
					{
						return null;
					}

					markerPosition = nearPoint + direction * intersectDist.Value;

					return markerPosition;
				}

				private void UpdateMarker()
				{
					if (Scene == null || Scene.Terrain == null || Instrument.Type == InstrumentType.None)
					{
						return;
					}

					Scene.Marker.Position = CalculateMarkerPosition();
					Scene.Marker.Radius = Instrument.Radius;
				}*/

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			_keysDown[(int)k] = true;
		}

		public override void OnKeyUp(Keys k)
		{
			base.OnKeyUp(k);

			_keysDown[(int)k] = false;
		}

		public override void OnLostKeyboardFocus()
		{
			base.OnLostKeyboardFocus();

			for(var i = 0; i < _keysDown.Length; i++)
			{
				_keysDown[i] = false;
			}
		}

		private void UpdateKeyboard()
		{
			// Manage camera input controller
			_controller.SetControlKeyState(ControlKeys.Left, _keysDown[(int)Keys.A]);
			_controller.SetControlKeyState(ControlKeys.Right, _keysDown[(int)Keys.D]);
			_controller.SetControlKeyState(ControlKeys.Forward, _keysDown[(int)Keys.W]);
			_controller.SetControlKeyState(ControlKeys.Backward, _keysDown[(int)Keys.S]);
			_controller.SetControlKeyState(ControlKeys.Up, _keysDown[(int)Keys.Up]);
			_controller.SetControlKeyState(ControlKeys.Down, _keysDown[(int)Keys.Down]);
			_controller.Update();
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (Scene == null)
			{
				return;
			}

			UpdateKeyboard();

			var bounds = ActualBounds;
			var device = DR.GraphicsDevice;
			var oldViewport = device.Viewport;

			var p = ToGlobal(bounds.Location);
			bounds.X = p.X;
			bounds.Y = p.Y;

			// Save scissor as it would be destroyed on exception
			var scissor = device.ScissorRectangle;

			try
			{
				device.Viewport = new Viewport(bounds.X, bounds.Y, bounds.Width, bounds.Height);

				var camera = StudioGame.MainForm.CurrentCamera;
				_renderer.AddNode(Scene);

				if (DigitalRiseEditorOptions.ShowGrid)
				{
					_renderer.AddNode(GridMesh);
				}

				// Selected object
				var selectionNode = _3DUtils.GetSelectionNode(StudioGame.MainForm.SelectedObject as SceneNode);
				if (selectionNode != null && camera == Scene.Camera)
				{
					_renderer.AddNode(selectionNode);
				}

				// Draw lights' icons'
				Scene.Iterate(n =>
				{
					foreach(var pair in _typesIcons)
					{
						if (pair.Key.IsAssignableFrom(n.GetType()))
						{
							BillboardNode node;
							if (n.Tag == null)
							{
								node = new BillboardNode
								{
									Texture = pair.Value
								};

								n.Tag = node;
							}

							node = (BillboardNode)n.Tag;
							node.Translation = n.Translation;
							_renderer.AddNode(node);

							break;
						}
					}
				});

				_renderer.Render(camera);

				RenderStatistics = _renderer.Statistics;

				device.Viewport = new Viewport(bounds.Right - 160, bounds.Y, 160, 160);

				var m = DigitalRise.Editor.Resources.ModelAxises;
				var c = Scene.Camera.Clone();

				// Make the gizmo placed always in front of the camera
				c.Translation = Vector3.Zero;
				m.Translation = c.Direction * 2;

				_renderer.AddNode(m);
				_renderer.Render(c);

				//				UpdateMarker();
				/*				_renderer.Begin();
								_renderer.DrawScene(Scene);

								if (_waterMarker != null)
								{
									_renderer.DrawMesh(_waterMarker, Scene.Camera);
								}

								if (_modelMarker != null)
								{
									_renderer.DrawModel(_modelMarker, Scene.Camera);
								}

								_renderer.End();*/
			}
			catch (Exception ex)
			{
				DR.GraphicsDevice.ScissorRectangle = scissor;
				var font = DigitalRise.Editor.Resources.ErrorFont;
				var message = ex.ToString();
				var sz = font.MeasureString(message);

				bounds = ActualBounds;
				var pos = new Vector2(bounds.X + (bounds.Width - sz.X) / 2,
					bounds.Y + (bounds.Height - sz.Y) / 2);

				pos.X = (int)pos.X;
				pos.Y = (int)pos.Y;
				context.DrawString(font, message, pos, Color.Red);
			}
			finally
			{
				device.Viewport = oldViewport;
			}

			if (DigitalRiseEditorOptions.DrawShadowMap)
			{
				context.Draw(_renderer.ShadowMap, new Rectangle(0, 0, 512, 512), Color.White);
			}
		}

		protected override void OnPlacedChanged()
		{
			base.OnPlacedChanged();

			if (Desktop == null)
			{
				return;
			}

			Desktop.TouchUp += Desktop_TouchUp;
		}

		private void Desktop_TouchUp(object sender, EventArgs e)
		{
			_controller.SetTouchState(TouchType.Move, false);
			_controller.SetTouchState(TouchType.Rotate, false);

			/*			if (Instrument.Type == InstrumentType.Water && _touchDownStart != null && Scene.Marker.Position != null)
						{
							GetWaterMarkerPos(out Vector3 startPos, out float sizeX, out float sizeZ);

							if (sizeX > 0 && sizeZ > 0)
							{
								var waterTile = new WaterTile(startPos.X, startPos.Z, Scene.DefaultWaterLevel, sizeX, sizeZ);
								Scene.WaterTiles.Add(waterTile);
							}

							_touchDownStart = null;
							_waterMarker = null;
						}*/
		}

		/*		private void UpdateTerrainHeight(Point pos, float power)
				{
					var height = Scene.Terrain.GetHeightByHeightPos(pos);
					height += power;
					Scene.Terrain.SetHeightByHeightPos(pos, height);
				}

				private void UpdateTerrainSplatMap(Point splatPos, SplatManChannel channel, float power)
				{
					var splatValue = Scene.Terrain.GetSplatValue(splatPos, channel);
					splatValue += power * 0.5f;
					Scene.Terrain.SetSplatValue(splatPos, channel, splatValue);
				}

				private void ApplyLowerRaise()
				{
					var power = Instrument.Power;
					var radius = Scene.Marker.Radius;
					var markerPos = Scene.Marker.Position.Value;

					var topLeft = Scene.Terrain.ToHeightPosition(markerPos.X - radius, markerPos.Z - radius);
					var bottomRight = Scene.Terrain.ToHeightPosition(markerPos.X + radius, markerPos.Z + radius);

					for (var x = topLeft.X; x <= bottomRight.X; ++x)
					{
						for (var y = topLeft.Y; y <= bottomRight.Y; ++y)
						{
							var heightPos = new Point(x, y);
							var terrainPos = Scene.Terrain.HeightToTerrainPosition(heightPos);
							var dist = Vector2.Distance(new Vector2(markerPos.X, markerPos.Z), terrainPos);

							if (dist > radius)
							{
								continue;
							}

							switch (Instrument.Type)
							{
								case InstrumentType.None:
									break;
								case InstrumentType.RaiseTerrain:
									UpdateTerrainHeight(heightPos, power);
									break;
								case InstrumentType.LowerTerrain:
									UpdateTerrainHeight(heightPos, -power);
									break;
							}
						}
					}
				}

				private void ApplyTerrainPaint()
				{
					var power = Instrument.Power;
					var radius = Scene.Marker.Radius;
					var markerPos = Scene.Marker.Position.Value;

					var topLeft = Scene.Terrain.ToSplatPosition(markerPos.X - radius, markerPos.Z - radius);
					var bottomRight = Scene.Terrain.ToSplatPosition(markerPos.X + radius, markerPos.Z + radius);

					for (var x = topLeft.X; x <= bottomRight.X; ++x)
					{
						for (var y = topLeft.Y; y <= bottomRight.Y; ++y)
						{
							var splatPos = new Point(x, y);
							var terrainPos = Scene.Terrain.SplatToTerrainPosition(splatPos);
							var dist = Vector2.Distance(new Vector2(markerPos.X, markerPos.Z), terrainPos);

							if (dist > radius)
							{
								continue;
							}

							switch (Instrument.Type)
							{
								case InstrumentType.PaintTexture1:
									UpdateTerrainSplatMap(splatPos, SplatManChannel.First, power);
									break;
								case InstrumentType.PaintTexture2:
									UpdateTerrainSplatMap(splatPos, SplatManChannel.Second, power);
									break;
								case InstrumentType.PaintTexture3:
									UpdateTerrainSplatMap(splatPos, SplatManChannel.Third, power);
									break;
								case InstrumentType.PaintTexture4:
									UpdateTerrainSplatMap(splatPos, SplatManChannel.Fourth, power);
									break;
							}
						}
					}
				}

				private void ApplyPaintInstrument()
				{
					if (Instrument.Type == InstrumentType.RaiseTerrain || Instrument.Type == InstrumentType.LowerTerrain)
					{
						ApplyLowerRaise();
					}
					else
					{
						ApplyTerrainPaint();
					}
				}*/

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			var mouseState = Mouse.GetState();
			_controller.SetMousePosition(new Point(mouseState.X, mouseState.Y));

			/*if (Instrument.Type == InstrumentType.Model)
			{
				if (Scene.Marker.Position != null)
				{
					if (_modelMarker == null || _modelMarker != Instrument.Model)
					{
						_modelMarker = Instrument.Model;
					}

					var pos = Scene.Marker.Position.Value;
					pos.Y = -_modelMarker.BoundingBox.Min.Y;
					pos.Y += Scene.Terrain.GetHeight(pos.X, pos.Z);

					_modelMarker.Transform = Matrix.CreateTranslation(pos);
				} else
				{
					_modelMarker = null;
				}
			}*/
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			_modelMarker = null;
		}

		/*		public override void OnTouchDown()
				{
					base.OnTouchDown();

					if (!IsMouseLeftButtonDown || Scene.Marker.Position == null)
					{
						return;
					}

					if (Instrument.IsPaintInstrument)
					{
						ApplyPaintInstrument();
					}
					else if (Instrument.Type == InstrumentType.Water)
					{
						_touchDownStart = Scene.Marker.Position.Value;
					}
					else if (Instrument.Type == InstrumentType.Model)
					{
						var pos = Scene.Marker.Position.Value;

						var model = Instrument.Model;
						pos.Y = -model.BoundingBox.Min.Y;
						pos.Y += Scene.Terrain.GetHeight(pos.X, pos.Z);

						model.Transform = Matrix.CreateTranslation(pos);

						Scene.Models.Add(model);
					}
				}

				private void GetWaterMarkerPos(out Vector3 startPos, out float sizeX, out float sizeZ)
				{
					var markerPos = Scene.Marker.Position.Value;

					startPos = new Vector3(Math.Min(markerPos.X, _touchDownStart.Value.X),
						Scene.DefaultWaterLevel,
						Math.Min(markerPos.Z, _touchDownStart.Value.Z));

					sizeX = Math.Abs(markerPos.X - _touchDownStart.Value.X);
					sizeZ = Math.Abs(markerPos.Z - _touchDownStart.Value.Z);
				}*/

		public override void OnTouchMoved()
		{
			base.OnTouchMoved();

			var mouseState = Mouse.GetState();
			if (Instrument.Type == InstrumentType.None)
			{
				_controller.SetTouchState(TouchType.Move, mouseState.LeftButton == ButtonState.Pressed);
			}

			_controller.SetTouchState(TouchType.Rotate, mouseState.RightButton == ButtonState.Pressed);
			_controller.Update();

			/*if (!IsMouseLeftButtonDown || Scene.Marker.Position == null)
			{
				return;
			}

			if (Instrument.IsPaintInstrument)
			{
				ApplyPaintInstrument();
			}
			else if (Instrument.Type == InstrumentType.Water)
			{
				if (_touchDownStart != null)
				{
					GetWaterMarkerPos(out Vector3 startPos, out float sizeX, out float sizeZ);
					if (sizeX > 0 && sizeZ > 0)
					{
						if (_waterMarker == null)
						{
							_waterMarker = new Mesh(PrimitiveMeshes.SquarePositionFromZeroToOne, Material.CreateSolidMaterial(Color.Green));
						}

						_waterMarker.Transform = Matrix.CreateScale(sizeX, 0, sizeZ) * Matrix.CreateTranslation(startPos);
					}
				}
			}*/
		}
	}
}
