using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Linq;
using glTFLoader.Schema;
using System.Numerics;
using static glTFLoader.Schema.Accessor;
using System.IO;
using System.Runtime.InteropServices;
using DigitalRise.Mathematics;

namespace DigitalRise
{
	internal static class Utility
	{
		private static readonly int[] ComponentsCount = new[]
		{
			1,
			2,
			3,
			4,
			4,
			9,
			16
		};

		private static readonly int[] ComponentSizes = new[]
		{
			sizeof(sbyte),
			sizeof(byte),
			sizeof(short),
			sizeof(ushort),
			0,	// There's no such component
			sizeof(uint),
			sizeof(float)
		};

		public static int GetComponentCount(this TypeEnum type) => ComponentsCount[(int)type];
		public static int GetComponentSize(this ComponentTypeEnum type) => ComponentSizes[(int)type - 5120];

		public static bool HasAttribute(this MeshPrimitive primitive, string prefix)
		{
			return (from p in primitive.Attributes.Keys where p.StartsWith(prefix) select p).FirstOrDefault() != null;
		}

		public static int FindAttribute(this MeshPrimitive primitive, string prefix)
		{
			var key = (from p in primitive.Attributes.Keys where p.StartsWith(prefix) select p).FirstOrDefault();
			if (string.IsNullOrEmpty(key))
			{
				throw new Exception($"Couldn't find mandatory primitive attribute {prefix}.");
			}

			return primitive.Attributes[key];
		}

		static bool IsFinite(float v)
		{
			return !float.IsInfinity(v) && !float.IsNaN(v);
		}

		static bool IsFinite(this Vector3 v)
		{
			return IsFinite(v.X) && IsFinite(v.Y) && IsFinite(v.Z);
		}

		private static Vector3 Normalize(Vector3 v)
		{
			float length = v.Length();
			if (length > 0)
				length = 1.0f / length;

			v.X *= length;
			v.Y *= length;
			v.Z *= length;

			return v;
		}

		/// <summary>
		/// Clamps the specified value.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="value">The value which should be clamped.</param>
		/// <param name="min">The min limit.</param>
		/// <param name="max">The max limit.</param>
		/// <returns>
		/// <paramref name="value"/> clamped to the interval
		/// [<paramref name="min"/>, <paramref name="max"/>].
		/// </returns>
		/// <remarks>
		/// Values within the limits are not changed. Values exceeding the limits are cut off.
		/// </remarks>
		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			if (min.CompareTo(max) > 0)
			{
				// min and max are swapped.
				var dummy = max;
				max = min;
				min = dummy;
			}

			if (value.CompareTo(min) < 0)
				value = min;
			else if (value.CompareTo(max) > 0)
				value = max;

			return value;
		}

		private static Vector3[] StoreNormals(Vector3[] normals, IList<int> indices, bool clockwiseOrder)
		{
			var result = new Vector3[normals.Length];
			for (int i = 0; i < normals.Length; i++)
			{
				normals[i] = clockwiseOrder ? -Normalize(normals[i]) : Normalize(normals[i]);
			}

			for(var i = 0; i < indices.Count; i++)
			{
				result[i] = normals[indices[i]];
			}
			
			return result;
		}

		public static Vector3[] ComputeNormalsWeightedByAngle(IList<Vector3> positions, IList<int> indices, bool clockwiseOrder)
		{
			int numberOfVertices = positions.Count;
			int numberOfFaces = indices.Count / 3;
			Vector3[] normals = new Vector3[numberOfVertices];

			for (int face = 0; face < numberOfFaces; face++)
			{
				int i0 = indices[face * 3 + 0];
				int i1 = indices[face * 3 + 1];
				int i2 = indices[face * 3 + 2];

				if (i0 == -1 || i1 == -1 || i2 == -1)
					continue;

				if (i0 >= numberOfVertices || i1 >= numberOfVertices || i2 >= numberOfVertices)
					throw new IndexOutOfRangeException("Index exceeds number of vertices.");

				Vector3 p0 = positions[i0];
				Vector3 p1 = positions[i1];
				Vector3 p2 = positions[i2];

				Vector3 u = p1 - p0;
				Vector3 v = p2 - p0;

				Vector3 n = Normalize(Vector3.Cross(u, v));

				// Corner 0:
				Vector3 a = Normalize(u);
				Vector3 b = Normalize(v);
				float w0 = Vector3.Dot(a, b);
				w0 = Clamp(w0, -1, 1);
				w0 = (float)Math.Acos(w0);

				// Corner 1:
				Vector3 c = Normalize(p2 - p1);
				Vector3 d = Normalize(p0 - p1);
				float w1 = Vector3.Dot(c, d);
				w1 = Clamp(w1, -1, 1);
				w1 = (float)Math.Acos(w1);

				// Corner 2:
				Vector3 e = Normalize(p0 - p2);
				Vector3 f = Normalize(p1 - p2);
				float w2 = Vector3.Dot(e, f);
				w2 = Clamp(w2, -1, 1);
				w2 = (float)Math.Acos(w2);

				normals[i0] += n * w0;
				normals[i1] += n * w1;
				normals[i2] += n * w2;
			}

			return StoreNormals(normals, indices, clockwiseOrder);
		}

		public static void CalculateTangentFrames(IList<Vector3> positions,
																							IList<int> indices,
																							IList<Vector3> normals,
																							IList<Vector2> textureCoords,
																							out Vector3[] tangents,
																							out Vector3[] bitangents)
		{
			// Lengyel, Eric. “Computing Tangent Space Basis Vectors for an Arbitrary Mesh”. 
			// Terathon Software 3D Graphics Library, 2001.
			// http://www.terathon.com/code/tangent.html

			// Hegde, Siddharth. "Messing with Tangent Space". Gamasutra, 2007. 
			// http://www.gamasutra.com/view/feature/129939/messing_with_tangent_space.php

			var numVerts = positions.Count;
			var numIndices = indices.Count;

			var tan1 = new Vector3[numVerts];
			var tan2 = new Vector3[numVerts];

			for (var index = 0; index < numIndices; index += 3)
			{
				var i1 = indices[index + 0];
				var i2 = indices[index + 1];
				var i3 = indices[index + 2];

				var w1 = textureCoords[i1];
				var w2 = textureCoords[i2];
				var w3 = textureCoords[i3];

				var s1 = w2.X - w1.X;
				var s2 = w3.X - w1.X;
				var t1 = w2.Y - w1.Y;
				var t2 = w3.Y - w1.Y;

				var denom = s1 * t2 - s2 * t1;
				if (Math.Abs(denom) < float.Epsilon)
				{
					// The triangle UVs are zero sized one dimension.
					//
					// So we cannot calculate the vertex tangents for this
					// one trangle, but maybe it can with other trangles.
					continue;
				}

				var r = 1.0f / denom;
				Debug.Assert(IsFinite(r), "Bad r!");

				var v1 = positions[i1];
				var v2 = positions[i2];
				var v3 = positions[i3];

				var x1 = v2.X - v1.X;
				var x2 = v3.X - v1.X;
				var y1 = v2.Y - v1.Y;
				var y2 = v3.Y - v1.Y;
				var z1 = v2.Z - v1.Z;
				var z2 = v3.Z - v1.Z;

				var sdir = new Vector3()
				{
					X = (t2 * x1 - t1 * x2) * r,
					Y = (t2 * y1 - t1 * y2) * r,
					Z = (t2 * z1 - t1 * z2) * r,
				};

				var tdir = new Vector3()
				{
					X = (s1 * x2 - s2 * x1) * r,
					Y = (s1 * y2 - s2 * y1) * r,
					Z = (s1 * z2 - s2 * z1) * r,
				};

				tan1[i1] += sdir;
				Debug.Assert(tan1[i1].IsFinite(), "Bad tan1[i1]!");
				tan1[i2] += sdir;
				Debug.Assert(tan1[i2].IsFinite(), "Bad tan1[i2]!");
				tan1[i3] += sdir;
				Debug.Assert(tan1[i3].IsFinite(), "Bad tan1[i3]!");

				tan2[i1] += tdir;
				Debug.Assert(tan2[i1].IsFinite(), "Bad tan2[i1]!");
				tan2[i2] += tdir;
				Debug.Assert(tan2[i2].IsFinite(), "Bad tan2[i2]!");
				tan2[i3] += tdir;
				Debug.Assert(tan2[i3].IsFinite(), "Bad tan2[i3]!");
			}

			tangents = new Vector3[numVerts];
			bitangents = new Vector3[numVerts];

			// At this point we have all the vectors accumulated, but we need to average
			// them all out. So we loop through all the final verts and do a Gram-Schmidt
			// orthonormalize, then make sure they're all unit length.
			for (var i = 0; i < numVerts; i++)
			{
				var n = normals[i];
				Debug.Assert(n.IsFinite(), "Bad normal! Normal vector must be finite.");
				Debug.Assert(n.Length() >= 0.9999f, "Bad normal! Normal vector must be normalized. (Actual length = " + n.Length() + ")");

				var t = tan1[i];
				if (t.LengthSquared() < float.Epsilon)
				{
					// TODO: Ideally we could spit out a warning to the
					// content logging here!

					// We couldn't find a good tanget for this vertex.
					//
					// Rather than set them to zero which could produce
					// errors in other parts of the pipeline, we just take        
					// a guess at something that may look ok.

					t = Vector3.Cross(n, Vector3.UnitX);
					if (t.LengthSquared() < float.Epsilon)
						t = Vector3.Cross(n, Vector3.UnitY);

					tangents[i] = Vector3.Normalize(t);
					bitangents[i] = Vector3.Cross(n, tangents[i]);
					continue;
				}

				// Gram-Schmidt orthogonalize
				// TODO: This can be zero can cause NaNs on 
				// normalize... how do we fix this?
				var tangent = t - n * Vector3.Dot(n, t);
				tangent = Vector3.Normalize(tangent);
				Debug.Assert(tangent.IsFinite(), "Bad tangent!");
				tangents[i] = tangent;

				// Calculate handedness
				var w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0F) ? -1.0F : 1.0F;
				Debug.Assert(IsFinite(w), "Bad handedness!");

				// Calculate the bitangent
				var bitangent = Vector3.Cross(n, tangent) * w;
				Debug.Assert(bitangent.IsFinite(), "Bad bitangent!");
				bitangents[i] = bitangent;
			}
		}

		public static byte[] ToBytes(this Stream input)
		{
			var ms = new MemoryStream();
			input.CopyTo(ms);

			return ms.ToArray();
		}

		private unsafe static int WriteData<T>(this Stream output, List<BufferView> bufferViews, List<Accessor> accessors, void *ptr, int length)
		{
			var pos = (int)output.Position;

			// Write data to the binary buffer
			var bytes = new byte[length * Marshal.SizeOf(typeof(T))];
			Marshal.Copy(new IntPtr(ptr), bytes, 0, bytes.Length);

			output.Write(bytes);

			// Create new buffer view
			bufferViews.Add(new BufferView { ByteOffset = pos, ByteLength = bytes.Length });

			// Create new accessor
			accessors.Add(new Accessor
			{ 
				ComponentType = ComponentTypeEnum.FLOAT, 
				Type = TypeEnum.VEC3,
				Count = length,
				BufferView = bufferViews.Count - 1,
			});

			return accessors.Count - 1;
		}

		public unsafe static int WriteData(this Stream output, List<BufferView> bufferViews, List<Accessor> accessors, Vector3[] data)
		{
			fixed(void *ptr = data)
			{
				return output.WriteData<Vector3>(bufferViews, accessors, ptr, data.Length);
			}
		}

		public unsafe static int WriteData(this Stream output, List<BufferView> bufferViews, List<Accessor> accessors, Vector2[] data)
		{
			fixed (void* ptr = data)
			{
				return output.WriteData<Vector2>(bufferViews, accessors, ptr, data.Length);
			}
		}
	}
}
