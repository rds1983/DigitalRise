﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DigitalRise.Rendering.Vertices;
using DigitalRise.Utilities;
using VertexPosition = DigitalRise.Rendering.Vertices.VertexPosition;

namespace DigitalRise.Rendering
{
	public class Mesh : IDisposable
	{
		public VertexBuffer VertexBuffer { get; private set; }
		public IndexBuffer IndexBuffer { get; private set; }
		public PrimitiveType PrimitiveType { get; }
		public int PrimitiveCount { get; private set; }
		public int VertexCount => VertexBuffer.VertexCount;
		public BoundingBox BoundingBox { get; }
		public bool HasNormals { get; }
		public Matrix[] BonesTransforms { get; set; }

		public bool HasBones => BonesTransforms != null && BonesTransforms.Length > 0;

		public Mesh(VertexBuffer vertexBuffer, IndexBuffer indexBuffer, IEnumerable<Vector3> positions, PrimitiveType primitiveType = PrimitiveType.TriangleList)
		{
			VertexBuffer = vertexBuffer ?? throw new ArgumentNullException(nameof(vertexBuffer));
			IndexBuffer = indexBuffer ?? throw new ArgumentNullException(nameof(indexBuffer));
			PrimitiveType = primitiveType;
			BoundingBox = BoundingBox.CreateFromPoints(positions);
			HasNormals = (from el in VertexBuffer.VertexDeclaration.GetVertexElements() where el.VertexElementUsage == VertexElementUsage.Normal select el).Count() > 0;

			switch (primitiveType)
			{
				case PrimitiveType.TriangleList:
					PrimitiveCount = IndexBuffer.IndexCount / 3;
					break;
				case PrimitiveType.LineList:
					PrimitiveCount = IndexBuffer.IndexCount / 2;
					break;

				default:
					throw new NotSupportedException($"Primitive type {primitiveType} isn't supported yet.");
			}
		}

		public Mesh(VertexPositionNormalTexture[] vertices, ushort[] indices, PrimitiveType primitiveType = PrimitiveType.TriangleList) :
			this(vertices.CreateVertexBuffer(), indices.CreateIndexBuffer(), vertices.GetPositions(), primitiveType)
		{
		}

		public Mesh(VertexPositionNormalTexture[] vertices, int[] indices, PrimitiveType primitiveType = PrimitiveType.TriangleList) :
			this(vertices.CreateVertexBuffer(), indices.CreateIndexBuffer(), vertices.GetPositions(), primitiveType)
		{
		}

		public Mesh(VertexPositionTexture[] vertices, ushort[] indices, PrimitiveType primitiveType = PrimitiveType.TriangleList) :
			this(vertices.CreateVertexBuffer(), indices.CreateIndexBuffer(), vertices.GetPositions(), primitiveType)
		{
		}

		public Mesh(VertexPositionTexture[] vertices, int[] indices, PrimitiveType primitiveType = PrimitiveType.TriangleList) :
			this(vertices.CreateVertexBuffer(), indices.CreateIndexBuffer(), vertices.GetPositions(), primitiveType)
		{
		}

		public Mesh(VertexPositionNormal[] vertices, ushort[] indices, PrimitiveType primitiveType = PrimitiveType.TriangleList) :
			this(vertices.CreateVertexBuffer(), indices.CreateIndexBuffer(), vertices.GetPositions(), primitiveType)
		{
		}

		public Mesh(VertexPositionNormal[] vertices, int[] indices, PrimitiveType primitiveType = PrimitiveType.TriangleList) :
			this(vertices.CreateVertexBuffer(), indices.CreateIndexBuffer(), vertices.GetPositions(), primitiveType)
		{
		}

		public Mesh(VertexPosition[] vertices, ushort[] indices, PrimitiveType primitiveType = PrimitiveType.TriangleList) :
			this(vertices.CreateVertexBuffer(), indices.CreateIndexBuffer(), vertices.GetPositions(), primitiveType)
		{
		}

		public Mesh(VertexPosition[] vertices, int[] indices, PrimitiveType primitiveType = PrimitiveType.TriangleList) :
			this(vertices.CreateVertexBuffer(), indices.CreateIndexBuffer(), vertices.GetPositions(), primitiveType)
		{
		}

		public void Dispose()
		{
			if (VertexBuffer != null)
			{
				VertexBuffer.Dispose();
				VertexBuffer = null;
			}

			if (IndexBuffer != null)
			{
				IndexBuffer.Dispose();
				IndexBuffer = null;
			}
		}
	}
}
