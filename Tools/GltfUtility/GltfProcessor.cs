﻿using System.Collections.Generic;
using System;
using System.IO;
using glTFLoader;
using glTFLoader.Schema;
using System.Numerics;
using System.Runtime.InteropServices;

using static glTFLoader.Schema.Accessor;

namespace DigitalRune
{
	internal class GltfProcessor
	{
		private string _input;
		private Gltf _gltf;
		private readonly Dictionary<int, byte[]> _bufferCache = new Dictionary<int, byte[]>();

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

				foreach (var primitive in gltfMesh.Primitives)
				{
					var hasNormals = primitive.HasAttribute("NORMAL");
					var hasTangents = primitive.HasAttribute("_TANGENT");
					var hasBinormals = primitive.HasAttribute("_BINORMAL");

					if (!hasTangents || !hasBinormals)
					{
						var positions = GetAccessorAs<Vector3>(primitive.FindAttribute("POSITION"));
						var normals = GetAccessorAs<Vector3>(primitive.FindAttribute("NORMAL"));
						var texCoords = GetAccessorAs<Vector2>(primitive.FindAttribute("TEXCOORD_"));
						
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
							for(var i = 0; i < data.Length; ++i)
							{
								indices.Add(data[i]);
							}
						} else if(indexAccessor.ComponentType == ComponentTypeEnum.UNSIGNED_SHORT)
						{
							var data = GetAccessorAs<ushort>(primitive.Indices.Value);
							for (var i = 0; i < data.Length; ++i)
							{
								indices.Add(data[i]);
							}
						} else
						{
							var data = GetAccessorAs<uint>(primitive.Indices.Value);
							for (var i = 0; i < data.Length; ++i)
							{
								indices.Add((int)data[i]);
							}
						}

						Vector3[] tangents, bitangents;
						Utility.CalculateTangentFrames(positions, indices, normals, texCoords, out tangents, out bitangents);

						var bufferViews = new List<BufferView>(_gltf.BufferViews);
						var accessors = new List<Accessor>(_gltf.Accessors);
						using (var ms = new MemoryStream())
						{
							ms.Write(GetBuffer(0));
							
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

		public void Process(string file, string output, bool genTangentFrames)
		{
			_input = file;
			
			if (string.IsNullOrEmpty(output))
			{
				output = file;
			}

			using (var stream = File.OpenRead(file))
			{
				_gltf = Interface.LoadModel(stream);
			}

			if (genTangentFrames)
			{
				GenerateTangentFrames();
			}

			_gltf.Buffers[0].Uri = null;
			Interface.SaveBinaryModel(_gltf, _bufferCache[0], Path.ChangeExtension(file, "glb"));
		}
	}
}