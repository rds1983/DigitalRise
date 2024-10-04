﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AssetManagementBase;
using glTFLoader;
using glTFLoader.Schema;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using DigitalRise.Standard;
using DigitalRise.Utilities;
using static glTFLoader.Schema.Accessor;
using static glTFLoader.Schema.AnimationChannelTarget;

namespace DigitalRise.Modelling
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

		private AssetManager _assetManager;
		private string _assetName;
		private Gltf _gltf;
		private readonly Dictionary<int, byte[]> _bufferCache = new Dictionary<int, byte[]>();
		private readonly List<List<ModelMesh>> _meshes = new List<List<ModelMesh>>();
		private ModelInstance _model;
		private readonly List<ModelNode> _allNodes = new List<ModelNode>();
		private readonly Dictionary<int, Skin> _skinCache = new Dictionary<int, Skin>();

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
			if (accessor.Type == TypeEnum.SCALAR && type != typeof(float))
			{
				throw new NotSupportedException("Scalar type could be converted only to float");
			}

			if (accessor.Type == TypeEnum.VEC3 && type != typeof(Vector3))
			{
				throw new NotSupportedException("VEC3 type could be converted only to Vector3");
			}

			if (accessor.Type == TypeEnum.VEC4 && type != typeof(Vector4) && type != typeof(Quaternion))
			{
				throw new NotSupportedException("VEC4 type could be converted only to Vector4 or Quaternion");
			}

			if (accessor.Type == TypeEnum.MAT4 && type != typeof(Matrix))
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
				case TypeEnum.VEC2:
					if (accessor.ComponentType == ComponentTypeEnum.FLOAT)
					{
						return VertexElementFormat.Vector2;
					}
					break;
				case TypeEnum.VEC3:
					if (accessor.ComponentType == ComponentTypeEnum.FLOAT)
					{
						return VertexElementFormat.Vector3;
					}
					break;
				case TypeEnum.VEC4:
					if (accessor.ComponentType == ComponentTypeEnum.FLOAT)
					{
						return VertexElementFormat.Vector4;
					}
					else if (accessor.ComponentType == ComponentTypeEnum.UNSIGNED_BYTE)
					{
						return VertexElementFormat.Byte4;
					}
					else if (accessor.ComponentType == ComponentTypeEnum.UNSIGNED_SHORT)
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

		private void LoadAnimationTransforms<T>(AnimationTransforms<T> animationTransforms, float[] times, AnimationSampler sampler)
		{
			var translations = GetAccessorAs<T>(sampler.Output);
			if (times.Length != translations.Length)
			{
				throw new NotSupportedException("Translation length is different from times length");
			}

			for (var i = 0; i < times.Length; ++i)
			{
				animationTransforms.Values.Add(new AnimationTransformKeyframe<T>(times[i], translations[i]));
			}

			animationTransforms.Interpolation = sampler.Interpolation;
		}


		private IndexBuffer CreateIndexBuffer(MeshPrimitive primitive)
		{
			if (primitive.Indices == null)
			{
				throw new NotSupportedException("Meshes without indices arent supported");
			}

			var indexAccessor = _gltf.Accessors[primitive.Indices.Value];
			if (indexAccessor.Type != TypeEnum.SCALAR)
			{
				throw new NotSupportedException("Only scalar index buffer are supported");
			}

			if (indexAccessor.ComponentType != ComponentTypeEnum.SHORT &&
				indexAccessor.ComponentType != ComponentTypeEnum.UNSIGNED_SHORT &&
				indexAccessor.ComponentType != ComponentTypeEnum.UNSIGNED_INT)
			{
				throw new NotSupportedException($"Index of type {indexAccessor.ComponentType} isn't supported");
			}

			var indexData = GetAccessorData(primitive.Indices.Value);

			var elementSize = (indexAccessor.ComponentType == ComponentTypeEnum.SHORT ||
				indexAccessor.ComponentType == ComponentTypeEnum.UNSIGNED_SHORT) ?
				IndexElementSize.SixteenBits : IndexElementSize.ThirtyTwoBits;

			var indexBuffer = new IndexBuffer(DR.GraphicsDevice, elementSize, indexAccessor.Count, BufferUsage.None);
			indexBuffer.SetData(0, indexData.Array, indexData.Offset, indexData.Count);

			// Since gltf uses ccw winding by default
			// We need to unwind it
			if (indexAccessor.ComponentType == ComponentTypeEnum.UNSIGNED_SHORT)
			{
				var data = new ushort[indexData.Count / 2];
				System.Buffer.BlockCopy(indexData.Array, indexData.Offset, data, 0, indexData.Count);

				for (var i = 0; i < data.Length / 3; i++)
				{
					var temp = data[i * 3];
					data[i * 3] = data[i * 3 + 2];
					data[i * 3 + 2] = temp;
				}

				indexBuffer.SetData(data);
			}
			else if (indexAccessor.ComponentType == ComponentTypeEnum.SHORT)
			{
				var data = new short[indexData.Count / 2];
				System.Buffer.BlockCopy(indexData.Array, indexData.Offset, data, 0, indexData.Count);

				for (var i = 0; i < data.Length / 3; i++)
				{
					var temp = data[i * 3];
					data[i * 3] = data[i * 3 + 2];
					data[i * 3 + 2] = temp;
				}

				indexBuffer.SetData(data);
			}
			else
			{
				var data = new uint[indexData.Count / 4];
				System.Buffer.BlockCopy(indexData.Array, indexData.Offset, data, 0, indexData.Count);

				for (var i = 0; i < data.Length / 3; i++)
				{
					var temp = data[i * 3];
					data[i * 3] = data[i * 3 + 2];
					data[i * 3 + 2] = temp;
				}

				indexBuffer.SetData(data);
			}

			return indexBuffer;
		}

		private void LoadMeshes()
		{
			foreach (var gltfMesh in _gltf.Meshes)
			{
				var meshes = new List<ModelMesh>();
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
						else if (pair.Key == "TANGENT" || pair.Key == "_TANGENT")
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
					var vertexBuffer = new VertexBuffer(DR.GraphicsDevice, vd, vertexCount.Value, BufferUsage.None);

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

					/*					var vertices = new VertexPositionNormalTexture[vertexCount.Value];
										unsafe
										{
											fixed(VertexPositionNormalTexture *ptr = vertices)
											{
												Marshal.Copy(vertexData, 0, new IntPtr(ptr), vertexData.Length);
											}
										}*/

					vertexBuffer.SetData(vertexData);

					var indexBuffer = CreateIndexBuffer(primitive);

					var material = new DefaultMaterial
					{
						DiffuseColor = Color.White,
					};

					var mesh = new ModelMesh(vertexBuffer, indexBuffer, positions, material);

					if (primitive.Material != null)
					{
						var gltfMaterial = _gltf.Materials[primitive.Material.Value];
						if (gltfMaterial.PbrMetallicRoughness != null)
						{
							material.DiffuseColor = new Color(gltfMaterial.PbrMetallicRoughness.BaseColorFactor.ToVector4());
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
										material.Id = image.Uri;
										material.SpecularFactor = 0.0f;
										material.SpecularPower = 250.0f;
										material.Texture = _assetManager.LoadTexture2D(DR.GraphicsDevice, image.Uri);
									}
								}
							}

						}
					}

					meshes.Add(mesh);
				}

				_meshes.Add(meshes);
			}
		}

		private Skin LoadSkin(int skinId)
		{
			if (_skinCache.TryGetValue(skinId, out Skin result))
			{
				return result;
			}

			var gltfSkin = _gltf.Skins[skinId];
			if (gltfSkin.Joints.Length > Constants.MaximumBones)
			{
				throw new Exception($"Skin {gltfSkin.Name} has {gltfSkin.Joints.Length} bones which exceeds maximum {Constants.MaximumBones}");
			}

			result = new Skin();
			foreach (var jointIndex in gltfSkin.Joints)
			{
				result.JointNodes.Add(_allNodes[jointIndex]);
			}

			result.Transforms = GetAccessorAs<Matrix>(gltfSkin.InverseBindMatrices.Value);
			Debug.WriteLine($"Skin {gltfSkin.Name} has {gltfSkin.Joints.Length} joints and {result.Transforms.Length} transforms");

			_skinCache[skinId] = result;

			return result;
		}

		private void LoadAllNodes()
		{
			// First run - load all nodes
			for (var i = 0; i < _gltf.Nodes.Length; ++i)
			{
				var gltfNode = _gltf.Nodes[i];

				var modelNode = new ModelNode(_model)
				{
					Id = gltfNode.Name,
					DefaultTranslation = gltfNode.Translation != null ? gltfNode.Translation.ToVector3() : Vector3.Zero,
					DefaultScale = gltfNode.Scale != null ? gltfNode.Scale.ToVector3() : Vector3.One,
					DefaultRotation = gltfNode.Rotation != null ? gltfNode.Rotation.ToQuaternion() : Quaternion.Identity
				};

				if (gltfNode.Matrix != null)
				{
					var matrix = gltfNode.Matrix.ToMatrix();

					if (matrix != Matrix.Identity)
					{
						matrix.Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation);

						modelNode.DefaultTranslation = translation;
						modelNode.DefaultScale = scale;
						modelNode.DefaultRotation = rotation;
					}
				}

				if (gltfNode.Mesh != null)
				{
					modelNode.Meshes.AddRange(_meshes[gltfNode.Mesh.Value]);
				}

				_allNodes.Add(modelNode);
			}

			// Second run - set children and skins
			for (var i = 0; i < _gltf.Nodes.Length; ++i)
			{
				var gltfNode = _gltf.Nodes[i];
				var modelNode = _allNodes[i];

				if (gltfNode.Skin != null)
				{
					modelNode.Skin = LoadSkin(gltfNode.Skin.Value);
					foreach (var mesh in modelNode.Meshes)
					{
						((DefaultMaterial)mesh.Material).Skinning = true;
					}
				}

				if (gltfNode.Children != null)
				{
					foreach (var childIndex in gltfNode.Children)
					{
						var childNode = _allNodes[childIndex];

						childNode.Parent = modelNode;
						modelNode.Children.Add(childNode);
					}
				}
			}
		}

		public ModelInstance Load(AssetManager manager, string assetName)
		{
			_meshes.Clear();
			_allNodes.Clear();
			_skinCache.Clear();
			_model = new ModelInstance();

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
				_model.RootNodes.Add(_allNodes[node]);
			}

			if (_gltf.Animations != null)
			{
				foreach (var gltfAnimation in _gltf.Animations)
				{
					var animation = new ModelAnimation
					{
						Id = gltfAnimation.Name
					};

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
						var nodeAnimation = new NodeAnimation(_allNodes[pair.Key]);

						foreach (var pathInfo in pair.Value)
						{
							var sampler = gltfAnimation.Samplers[pathInfo.Sampler];
							var times = GetAccessorAs<float>(sampler.Input);

							switch (pathInfo.Path)
							{
								case PathEnum.translation:
									LoadAnimationTransforms(nodeAnimation.Translations, times, sampler);
									break;
								case PathEnum.rotation:
									LoadAnimationTransforms(nodeAnimation.Rotations, times, sampler);
									break;
								case PathEnum.scale:
									LoadAnimationTransforms(nodeAnimation.Scales, times, sampler);
									break;
								case PathEnum.weights:
									break;
							}
						}

						animation.BoneAnimations.Add(nodeAnimation);
					}

					animation.UpdateStartEnd();

					var id = animation.Id ?? "(default)";
					_model.Animations[id] = animation;
				}
			}

			_model.UpdateBoundingBox();

			return _model;
		}
	}
}