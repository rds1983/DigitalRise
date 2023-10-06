using System.Collections.Generic;
using System;
using System.IO;
using glTFLoader;
using glTFLoader.Schema;
using System.Numerics;
using System.Runtime.InteropServices;

using static glTFLoader.Schema.Accessor;

namespace DigitalRise
{
	internal class GltfProcessor
	{
		private string _input;
		private Gltf _gltf;
		private readonly Dictionary<int, byte[]> _bufferCache = new Dictionary<int, byte[]>();

		private static void Log(string message) => Console.WriteLine(message);
		private byte[] FileResolver(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				using (var stream = File.OpenRead(_input))
				{
					return Interface.LoadBinaryBuffer(stream);
				}
			}

			var folder = Path.GetDirectoryName(_input);
			using (var stream = File.OpenRead(Path.Combine(folder, path)))
			{
				return stream.ToBytes();
			}
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

		private void GenerateTangentFrames()
		{
			foreach (var gltfMesh in _gltf.Meshes)
			{
				var meshName = gltfMesh.Name ?? "null";

				for(var primitiveIndex = 0; primitiveIndex < gltfMesh.Primitives.Length; primitiveIndex++)
				{
					var primitive = gltfMesh.Primitives[primitiveIndex];
					var hasNormals = primitive.HasAttribute("NORMAL");
					var hasTangents = primitive.HasAttribute("_TANGENT");
					var hasBinormals = primitive.HasAttribute("_BINORMAL");

					if (!hasTangents || !hasBinormals)
					{
						var hasPositions = primitive.HasAttribute("POSITION");
						var hasTexCoords = primitive.HasAttribute("TEXCOORD_");

						if (!hasPositions)
						{
							Log($"Warning: can't generate the tangent frames, since {primitive}'s primitive of Mesh {meshName} lacks positions.");
							return;
						}

						if (!hasNormals)
						{
							Log($"Warning: {primitive}'s primitive of Mesh {meshName} lacks normals. It would be generated");
						}

						if (!hasTexCoords)
						{
							Log($"Warning: {primitive}'s primitive of Mesh {meshName} lacks texCoords. Using default zero to generate the tangent frames");
						}


						var positions = GetAccessorAs<Vector3>(primitive.FindAttribute("POSITION"));

						Vector2[] texCoords;

						if (hasTexCoords)
						{
							texCoords = GetAccessorAs<Vector2>(primitive.FindAttribute("TEXCOORD_"));
						} else
						{
							texCoords = new Vector2[positions.Length];
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

						var indices = new List<int>();
						if (indexAccessor.ComponentType == ComponentTypeEnum.SHORT)
						{
							var data = GetAccessorAs<short>(primitive.Indices.Value);
							for (var i = 0; i < data.Length; ++i)
							{
								indices.Add(data[i]);
							}
						}
						else if (indexAccessor.ComponentType == ComponentTypeEnum.UNSIGNED_SHORT)
						{
							var data = GetAccessorAs<ushort>(primitive.Indices.Value);
							for (var i = 0; i < data.Length; ++i)
							{
								indices.Add(data[i]);
							}
						}
						else
						{
							var data = GetAccessorAs<uint>(primitive.Indices.Value);
							for (var i = 0; i < data.Length; ++i)
							{
								indices.Add((int)data[i]);
							}
						}

						Vector3[] normals;
						if (!hasNormals)
						{
							normals = Utility.ComputeNormalsWeightedByAngle(positions, indices, false);
						}
						else
						{
							normals = GetAccessorAs<Vector3>(primitive.FindAttribute("NORMAL"));
						}

						Vector3[] tangents, bitangents;
						Utility.CalculateTangentFrames(positions, indices, normals, texCoords, out tangents, out bitangents);

						var bufferViews = new List<BufferView>(_gltf.BufferViews);
						var accessors = new List<Accessor>(_gltf.Accessors);
						using (var ms = new MemoryStream())
						{
							ms.Write(GetBuffer(0));

							if (!hasTexCoords)
							{
								primitive.Attributes["TEXCOORD_0"] = ms.WriteData(bufferViews, accessors, texCoords);
							}

							if (!hasNormals)
							{
								primitive.Attributes["NORMAL"] = ms.WriteData(bufferViews, accessors, normals);
							}

							if (!hasTangents)
							{
								primitive.Attributes["_TANGENT"] = ms.WriteData(bufferViews, accessors, tangents);
							}

							if (!hasBinormals)
							{
								primitive.Attributes["_BINORMAL"] = ms.WriteData(bufferViews, accessors, bitangents);
							}

							_bufferCache[0] = ms.ToArray();
							_gltf.Buffers[0].ByteLength = _bufferCache[0].Length;
						}

						_gltf.BufferViews = bufferViews.ToArray();
						_gltf.Accessors = accessors.ToArray();
					}
				}
			}
		}

		private void UnwindIndices()
		{
			foreach (var gltfMesh in _gltf.Meshes)
			{
				foreach (var primitive in gltfMesh.Primitives)
				{
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

					// Flip winding
					var indexData = GetAccessorData(primitive.Indices.Value);
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

						System.Buffer.BlockCopy(data, 0, indexData.Array, indexData.Offset, indexData.Count);
					}
					else if(indexAccessor.ComponentType == ComponentTypeEnum.SHORT)
					{
						var data = new short[indexData.Count / 2];
						System.Buffer.BlockCopy(indexData.Array, indexData.Offset, data, 0, indexData.Count);

						for (var i = 0; i < data.Length / 3; i++)
						{
							var temp = data[i * 3];
							data[i * 3] = data[i * 3 + 2];
							data[i * 3 + 2] = temp;
						}

						System.Buffer.BlockCopy(data, 0, indexData.Array, indexData.Offset, indexData.Count);
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

						System.Buffer.BlockCopy(data, 0, indexData.Array, indexData.Offset, indexData.Count);
					}
				}
			}
		}

		private void PremultiplyVertexColors()
		{
			foreach (var gltfMesh in _gltf.Meshes)
			{
				foreach (var primitive in gltfMesh.Primitives)
				{
					var hasColors = primitive.HasAttribute("COLOR");
					if (!hasColors)
					{
						continue;
					}
				}
			}
		}

		public Gltf Process(string file, string output, bool genTangentFrames,
			bool unwindIndices, bool premultiply, float? scale)
		{
			_bufferCache.Clear();
			_input = file;

			if (string.IsNullOrEmpty(output))
			{
				output = Path.ChangeExtension(file, "glb");
			}

			using (var stream = File.OpenRead(file))
			{
				_gltf = Interface.LoadModel(stream);
			}

			// Load all buffers and erase their uris(required for SaveBinaryModel to work)
			for(var i = 0; i < _gltf.Buffers.Length; ++i)
			{
				GetBuffer(i);
				_gltf.Buffers[i].Uri = null;
			}

			if (genTangentFrames)
			{
				GenerateTangentFrames();
			}

			if (unwindIndices)
			{
				UnwindIndices();
			}

			if (premultiply)
			{
				PremultiplyVertexColors();
			}

			if (scale != null)
			{
				foreach(var node in _gltf.Nodes)
				{
					if (node.Mesh != null)
					{
						for (var i = 0; i < node.Scale.Length; ++i)
						{
							node.Scale[i] *= scale.Value;
						}
					}
				}
			}

			Interface.SaveBinaryModel(_gltf, GetBuffer(0), output);

			return _gltf;
		}
	}
}
