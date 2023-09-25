using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using AssetManagementBase;
using DigitalRune.Animation.Character;
using DigitalRune.Character.Skeleton_Animations;
using DigitalRune.Geometry;
using DigitalRune.Graphics.Effects;
using DigitalRune.Mathematics.Algebra;
using glTFLoader;
using glTFLoader.Schema;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using static glTFLoader.Schema.AnimationChannelTarget;

namespace DigitalRune.Graphics.SceneGraph
{
	internal class GltfLoader
	{
		struct VertexElementInfo
		{
			public VertexElementFormat Format;
			public VertexElementUsage Usage;
			public int UsageIndex;
			public int AccessorIndex;

			public VertexElementInfo(VertexElementFormat format, VertexElementUsage usage, int accessorIndex, int usageIndex)
			{
				Format = format;
				Usage = usage;
				AccessorIndex = accessorIndex;
				UsageIndex = usageIndex;
			}
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct VertexPositionNormalTextureSkin
		{
			public Vector3 Position;
			public Vector3 Normal;
			public Vector2 Texture;
			public byte i1, i2, i3, i4;
			public float w1, w2, w3, w4;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct VertexNormalPosition
		{
			public Vector3 Normal;
			public Vector3 Position;
		}

		private struct PathInfo
		{
			public int Sampler;
			public PathEnum Path;

			public PathInfo(int sampler, PathEnum path)
			{
				Sampler = sampler;
				Path = path;
			}
		}

		private IGraphicsService _graphicsService;
		private AssetManager _assetManager;
		private string _assetName;
		private Gltf _gltf;
		private readonly Dictionary<int, byte[]> _bufferCache = new Dictionary<int, byte[]>();
		private readonly List<Mesh> _meshes = new List<Mesh>();
		private readonly List<SceneNode> _nodes = new List<SceneNode>();
		private ModelNode _model;
		private readonly Dictionary<int, Skeleton> _skinCache = new Dictionary<int, Skeleton>();

		private byte[] FileResolver(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				using (var stream = _assetManager.Open(_assetName))
				{
					return Interface.LoadBinaryBuffer(stream);
				}
			}

			return _assetManager.ReadAsByteArray(path);
		}

		private byte[] GetBuffer(int index)
		{
			byte[] result;
			if (_bufferCache.TryGetValue(index, out result))
			{
				return result;
			}

			result = _gltf.LoadBinaryBuffer(index, path => FileResolver(path));
			_bufferCache[index] = result;

			return result;
		}

		private ArraySegment<byte> GetAccessorData(int accessorIndex)
		{
			var accessor = _gltf.Accessors[accessorIndex];
			if (accessor.BufferView == null)
			{
				throw new NotSupportedException("Accessors without buffer index arent supported");
			}

			var bufferView = _gltf.BufferViews[accessor.BufferView.Value];
			var buffer = GetBuffer(bufferView.Buffer);

			var size = accessor.Type.GetComponentCount() * accessor.ComponentType.GetComponentSize();
			return new ArraySegment<byte>(buffer, bufferView.ByteOffset + accessor.ByteOffset, accessor.Count * size);
		}

		private T[] GetAccessorAs<T>(int accessorIndex)
		{
			var type = typeof(T);
			if (type != typeof(float) && type != typeof(Vector3) && type != typeof(Vector4) && type != typeof(Quaternion) && type != typeof(Matrix))
			{
				throw new NotSupportedException("Only float/Vector3/Vector4 types are supported");
			}

			var accessor = _gltf.Accessors[accessorIndex];
			if (accessor.Type == Accessor.TypeEnum.SCALAR && type != typeof(float))
			{
				throw new NotSupportedException("Scalar type could be converted only to float");
			}

			if (accessor.Type == Accessor.TypeEnum.VEC3 && type != typeof(Vector3))
			{
				throw new NotSupportedException("VEC3 type could be converted only to Vector3");
			}

			if (accessor.Type == Accessor.TypeEnum.VEC4 && type != typeof(Vector4) && type != typeof(Quaternion))
			{
				throw new NotSupportedException("VEC4 type could be converted only to Vector4 or Quaternion");
			}

			if (accessor.Type == Accessor.TypeEnum.MAT4 && type != typeof(Matrix))
			{
				throw new NotSupportedException("MAT4 type could be converted only to Matrix");
			}

			var bytes = GetAccessorData(accessorIndex);

			var count = bytes.Count / Marshal.SizeOf(typeof(T));
			var result = new T[count];

			GCHandle handle = GCHandle.Alloc(result, GCHandleType.Pinned);
			try
			{
				IntPtr pointer = handle.AddrOfPinnedObject();
				Marshal.Copy(bytes.Array, bytes.Offset, pointer, bytes.Count);
			}
			finally
			{
				if (handle.IsAllocated)
				{
					handle.Free();
				}
			}

			return result;
		}

		private VertexElementFormat GetAccessorFormat(int index)
		{
			var accessor = _gltf.Accessors[index];

			switch (accessor.Type)
			{
				case Accessor.TypeEnum.VEC2:
					if (accessor.ComponentType == Accessor.ComponentTypeEnum.FLOAT)
					{
						return VertexElementFormat.Vector2;
					}
					break;
				case Accessor.TypeEnum.VEC3:
					if (accessor.ComponentType == Accessor.ComponentTypeEnum.FLOAT)
					{
						return VertexElementFormat.Vector3;
					}
					break;
				case Accessor.TypeEnum.VEC4:
					if (accessor.ComponentType == Accessor.ComponentTypeEnum.FLOAT)
					{
						return VertexElementFormat.Vector4;
					}
					else if (accessor.ComponentType == Accessor.ComponentTypeEnum.UNSIGNED_BYTE)
					{
						return VertexElementFormat.Byte4;
					}
					else if (accessor.ComponentType == Accessor.ComponentTypeEnum.UNSIGNED_SHORT)
					{
						return VertexElementFormat.Short4;
					}
					break;
			}

			throw new NotSupportedException($"Accessor of type {accessor.Type} and component type {accessor.ComponentType} isn't supported");
		}

		private static Matrix CreateTransform(Vector3 translation, Vector3 scale, Quaternion rotation)
		{
			return Matrix.CreateFromQuaternion(rotation) *
				Matrix.CreateScale(scale) *
				Matrix.CreateTranslation(translation);
		}

		private static Matrix LoadTransform(JObject data)
		{
			var scale = data.OptionalVector3("scale", Vector3.One);
			var translation = data.OptionalVector3("translation", Vector3.Zero);
			var rotation = data.OptionalVector4("rotation", Vector4.Zero);

			var quaternion = new Quaternion(rotation.X,
				rotation.Y, rotation.Z, rotation.W);

			return CreateTransform(translation, scale, quaternion);
		}

		private void LoadMeshes()
		{
			foreach (var gltfMesh in _gltf.Meshes)
			{
				var mesh = new Mesh();
				var boundingBox = new BoundingBox();
				foreach (var primitive in gltfMesh.Primitives)
				{
					if (primitive.Mode != MeshPrimitive.ModeEnum.TRIANGLES)
					{
						throw new NotSupportedException($"Primitive mode {primitive.Mode} isn't supported.");
					}

					// Read vertex declaration
					var vertexInfos = new List<VertexElementInfo>();
					int? vertexCount = null;
					foreach (var pair in primitive.Attributes)
					{
						var accessor = _gltf.Accessors[pair.Value];
						var newVertexCount = accessor.Count;
						if (vertexCount != null && vertexCount.Value != newVertexCount)
						{
							throw new NotSupportedException($"Vertex count changed. Previous value: {vertexCount}. New value: {newVertexCount}");
						}

						vertexCount = newVertexCount;

						var element = new VertexElementInfo();
						if (pair.Key == "POSITION")
						{
							element.Usage = VertexElementUsage.Position;
						}
						else if (pair.Key == "NORMAL")
						{
							element.Usage = VertexElementUsage.Normal;
						}
						else if (pair.Key == "_TANGENT")
						{
							element.Usage = VertexElementUsage.Tangent;
						}
						else if (pair.Key == "_BINORMAL")
						{
							element.Usage = VertexElementUsage.Binormal;
						}
						else if (pair.Key.StartsWith("TEXCOORD_"))
						{
							element.Usage = VertexElementUsage.TextureCoordinate;
							element.UsageIndex = int.Parse(pair.Key.Substring(9));
						}
						else if (pair.Key.StartsWith("JOINTS_"))
						{
							element.Usage = VertexElementUsage.BlendIndices;
							element.UsageIndex = int.Parse(pair.Key.Substring(7));
						}
						else if (pair.Key.StartsWith("WEIGHTS_"))
						{
							element.Usage = VertexElementUsage.BlendWeight;
							element.UsageIndex = int.Parse(pair.Key.Substring(8));
						}
						else if (pair.Key.StartsWith("COLOR_"))
						{
							element.Usage = VertexElementUsage.Color;
							element.UsageIndex = int.Parse(pair.Key.Substring(6));
						}
						else
						{
							throw new Exception($"Attribute of type '{pair.Key}' isn't supported.");
						}

						element.Format = GetAccessorFormat(pair.Value);
						element.AccessorIndex = pair.Value;

						vertexInfos.Add(element);
					}

					if (vertexCount == null)
					{
						throw new NotSupportedException("Vertex count is not set");
					}

					var vertexElements = new VertexElement[vertexInfos.Count];
					var offset = 0;
					for (var i = 0; i < vertexInfos.Count; ++i)
					{
						vertexElements[i] = new VertexElement(offset, vertexInfos[i].Format, vertexInfos[i].Usage, vertexInfos[i].UsageIndex);
						offset += vertexInfos[i].Format.GetSize();
					}

					var vd = new VertexDeclaration(vertexElements);
					var vertexBuffer = new VertexBuffer(_graphicsService.GraphicsDevice, vd, vertexCount.Value, BufferUsage.None);

					// Set vertex data
					var vertexData = new byte[vertexCount.Value * vd.VertexStride];
					var positions = new List<Vector3>();
					offset = 0;
					for (var i = 0; i < vertexInfos.Count; ++i)
					{
						var sz = vertexInfos[i].Format.GetSize();
						var data = GetAccessorData(vertexInfos[i].AccessorIndex);

						for (var j = 0; j < vertexCount.Value; ++j)
						{
							Array.Copy(data.Array, data.Offset + j * sz, vertexData, j * vd.VertexStride + offset, sz);

							if (vertexInfos[i].Usage == VertexElementUsage.Position)
							{
								unsafe
								{
									fixed (byte* bptr = &data.Array[data.Offset + j * sz])
									{
										Vector3* vptr = (Vector3*)bptr;
										positions.Add(*vptr);
									}
								}
							}
						}

						offset += sz;
					}

					var newBoundingBox = BoundingBox.CreateFromPoints(positions);
					boundingBox = BoundingBox.CreateMerged(boundingBox, newBoundingBox);

					/*					var vertices = new VertexPositionNormalTexture[vertexCount.Value];
										unsafe
										{
											fixed(VertexPositionNormalTexture *ptr = vertices)
											{
												Marshal.Copy(vertexData, 0, new IntPtr(ptr), vertexData.Length);
											}
										}*/

					vertexBuffer.SetData(vertexData);

					if (primitive.Indices == null)
					{
						throw new NotSupportedException("Meshes without indices arent supported");
					}

					var indexAccessor = _gltf.Accessors[primitive.Indices.Value];
					if (indexAccessor.Type != Accessor.TypeEnum.SCALAR)
					{
						throw new NotSupportedException("Only scalar index buffer are supported");
					}

					if (indexAccessor.ComponentType != Accessor.ComponentTypeEnum.SHORT &&
						indexAccessor.ComponentType != Accessor.ComponentTypeEnum.UNSIGNED_SHORT &&
						indexAccessor.ComponentType != Accessor.ComponentTypeEnum.UNSIGNED_INT)
					{
						throw new NotSupportedException($"Index of type {indexAccessor.ComponentType} isn't supported");
					}

					var indexData = GetAccessorData(primitive.Indices.Value);

					var elementSize = (indexAccessor.ComponentType == Accessor.ComponentTypeEnum.SHORT ||
						indexAccessor.ComponentType == Accessor.ComponentTypeEnum.UNSIGNED_SHORT) ?
						IndexElementSize.SixteenBits : IndexElementSize.ThirtyTwoBits;
					var indexBuffer = new IndexBuffer(_graphicsService.GraphicsDevice, elementSize, indexAccessor.Count, BufferUsage.None);
					indexBuffer.SetData(0, indexData.Array, indexData.Offset, indexData.Count);

					var subMesh = new Submesh
					{
						VertexBuffer = vertexBuffer,
						IndexBuffer = indexBuffer,
						VertexCount = vertexBuffer.VertexCount,
						PrimitiveCount = indexBuffer.IndexCount / 3
					};

					mesh.Submeshes.Add(subMesh);
					if (primitive.Material != null)
					{
						var gltfMaterial = _gltf.Materials[primitive.Material.Value];
						if (gltfMaterial.PbrMetallicRoughness != null)
						{
							if (gltfMaterial.PbrMetallicRoughness.BaseColorTexture != null)
							{
								var gltfTexture = _gltf.Textures[gltfMaterial.PbrMetallicRoughness.BaseColorTexture.Index];
								if (gltfTexture.Source != null)
								{
									var image = _gltf.Images[gltfTexture.Source.Value];

									if (image.BufferView.HasValue)
									{
										throw new Exception("Embedded images arent supported.");
									}
									else if (image.Uri.StartsWith("data:image/"))
									{
										throw new Exception("Embedded images with uri arent supported.");
									}
									else
									{
										// Create default material
										var material = new Material
										{
											Name = image.Uri
										};

										var opaqueData = new Dictionary<string, object>
										{
											["DiffuseColor"] = gltfMaterial.PbrMetallicRoughness.BaseColorFactor.ToVector3().ToXna(),
											["SpecularColor"] = new Vector3(0.1f),
											["SpecularPower"] = 10.0f,
											["Texture"] = _assetManager.LoadTexture2D(_graphicsService.GraphicsDevice, image.Uri)
										};
										var binding = new BasicEffectBinding(_graphicsService, opaqueData);
										material["Default"] = binding;

										subMesh.SetMaterial(material);
									}
								}
							}

						}
					}
				}

				/*				mesh.BoundingShape = new BoxShape(boundingBox.Max.X - boundingBox.Min.X,
								boundingBox.Max.Y - boundingBox.Min.Y,
								boundingBox.Max.Z - boundingBox.Min.Z);*/

				_meshes.Add(mesh);
			}
		}

		private Skeleton CreateSkeleton()
		{
			var boneParents = new List<int>();
			var boneNames = new List<string>();
			var boneTransforms = new List<SrtTransform>();

			for (var i = 0; i < _nodes.Count; ++i)
			{
				var node = _nodes[i];
				var parentIndex = node.Parent != null ? _nodes.IndexOf(node.Parent) : -1;

				boneParents.Add(parentIndex);
				boneNames.Add(node.Name);
				boneTransforms.Add(new SrtTransform(node.ScaleWorld, node.PoseLocal.Orientation, node.PoseLocal.Position));
			}

			var result = new Skeleton(boneParents, boneNames, boneTransforms);

			return result;
		}

		private void SetSkin(MeshNode meshNode, int skinId)
		{
			var gltfSkin = _gltf.Skins[skinId];

			var pose = SkeletonPose.Create(meshNode.Mesh.Skeleton);

			meshNode.Mesh.Skeleton.BoneOrder = gltfSkin.Joints;
			meshNode.SkeletonPose = pose;
		}

		private void LoadAllNodes()
		{
			// First run - load all nodes
			for (var i = 0; i < _gltf.Nodes.Length; ++i)
			{
				var gltfNode = _gltf.Nodes[i];

				SceneNode node;
				if (gltfNode.Mesh != null)
				{
					var meshNode = new MeshNode(_meshes[gltfNode.Mesh.Value]);
					node = meshNode;
				}
				else
				{
					node = new SceneNode();
				}

				node.Name = gltfNode.Name;

				var translation = gltfNode.Translation != null ? gltfNode.Translation.ToVector3() : Vector3F.Zero;
				var scale = gltfNode.Scale != null ? gltfNode.Scale.ToVector3() : Vector3F.One;
				var rotation = gltfNode.Rotation != null ? gltfNode.Rotation.ToQuaternion() : QuaternionF.Identity;

				node.PoseLocal = new Pose(translation, rotation);
				node.ScaleLocal = scale;

				if (gltfNode.Matrix != null)
				{
					var matrix = gltfNode.Matrix.ToMatrix();

					if (matrix != Matrix44F.Identity)
					{
						matrix.Decompose(out scale, out rotation, out translation);

						node.PoseLocal = new Pose(translation, rotation);
						node.ScaleLocal = scale;
					}
				}

				_nodes.Add(node);
			}

			// Second run - set children
			for (var i = 0; i < _gltf.Nodes.Length; ++i)
			{
				var gltfNode = _gltf.Nodes[i];
				var node = _nodes[i];

				if (gltfNode.Children != null)
				{
					foreach (var childIndex in gltfNode.Children)
					{
						if (node.Children == null)
						{
							node.Children = new SceneNodeCollection();
						}
						node.Children.Add(_nodes[childIndex]);
					}
				}
			}

			// Third run - set skeletons and skins
			Skeleton skeleton = null;
			for (var i = 0; i < _gltf.Nodes.Length; ++i)
			{
				var gltfNode = _gltf.Nodes[i];
				var node = _nodes[i];

				if (gltfNode.Skin != null)
				{
					if (skeleton == null)
					{
						skeleton = CreateSkeleton();
					}

					var meshNode = (MeshNode)node;
					meshNode.Mesh.Skeleton = skeleton;
					SetSkin(meshNode, gltfNode.Skin.Value);
				}
			}
		}

		public ModelNode Load(AssetManager manager, IGraphicsService graphicsService, string assetName)
		{
			_graphicsService = graphicsService;
			_meshes.Clear();
			_nodes.Clear();
			_skinCache.Clear();
			_model = new ModelNode
			{
				Name = assetName
			};

			_assetManager = manager;
			_assetName = assetName;
			using (var stream = manager.Open(assetName))
			{
				_gltf = Interface.LoadModel(stream);
			}

			LoadMeshes();
			LoadAllNodes();

			var scene = _gltf.Scenes[_gltf.Scene.Value];
			foreach (var node in scene.Nodes)
			{
				if (_model.Children == null)
				{
					_model.Children = new SceneNodeCollection();
				}

				_model.Children.Add(_nodes[node]);
			}

			if (_gltf.Animations != null)
			{
				foreach (var gltfAnimation in _gltf.Animations)
				{
					var animation = new Dictionary<int, SkeletonKeyFrameAnimationData>();

					var channelsDict = new Dictionary<int, List<PathInfo>>();
					foreach (var channel in gltfAnimation.Channels)
					{
						if (!channelsDict.TryGetValue(channel.Target.Node.Value, out List<PathInfo> targets))
						{
							targets = new List<PathInfo>();
							channelsDict[channel.Target.Node.Value] = targets;
						}

						targets.Add(new PathInfo(channel.Sampler, channel.Target.Path));
					}

					foreach (var pair in channelsDict)
					{
						var nodeAnimation = new SkeletonKeyFrameAnimationData();

						foreach (var pathInfo in pair.Value)
						{
							var sampler = gltfAnimation.Samplers[pathInfo.Sampler];

							nodeAnimation.Times = GetAccessorAs<float>(sampler.Input);
							switch (pathInfo.Path)
							{
								case PathEnum.translation:
									nodeAnimation.Translations = GetAccessorAs<Vector3>(sampler.Output);
									break;
								case PathEnum.rotation:
									nodeAnimation.Rotations = GetAccessorAs<Quaternion>(sampler.Output);
									break;
								case PathEnum.scale:
									nodeAnimation.Scales = GetAccessorAs<Vector3>(sampler.Output);
									break;
								case PathEnum.weights:
									break;
							}
						}

						var node = _nodes[pair.Key];

						// Make transforms relative to the default
						for (var i = 0; i < nodeAnimation.Times.Length; i++)
						{
							if (nodeAnimation.Translations != null)
							{
								nodeAnimation.Translations[i] -= node.PoseLocal.Position.ToXna();
							}

							if (nodeAnimation.Rotations != null)
							{
								var defaultRotation = QuaternionF.CreateRotation(node.PoseLocal.Orientation);
								defaultRotation.Conjugate();
								var b = new QuaternionF(nodeAnimation.Rotations[i].W, nodeAnimation.Rotations[i].X, nodeAnimation.Rotations[i].Y, nodeAnimation.Rotations[i].Z);
								nodeAnimation.Rotations[i] = (defaultRotation * b).ToXna();
							}

							if (nodeAnimation.Scales != null)
							{
								nodeAnimation.Scales[i] /= node.ScaleLocal.ToXna();
							}
						}

						animation[pair.Key] = nodeAnimation;
					}

					var meshNode = _model.FindFirstMeshNode();
					if (meshNode.Mesh.Animations == null)
					{
						meshNode.Mesh.Animations = new Dictionary<string, SkeletonKeyFrameAnimation>();
					}

					var id = gltfAnimation.Name ?? "(default)";
					meshNode.Mesh.Animations[id] = SkeletonKeyFrameAnimation.FromData(animation);
				}
			}

			return _model;
		}
	}
}