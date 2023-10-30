// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;

namespace DigitalRise.Graphics.Rendering
{
	// Data required by DrawBillboard()
	internal struct BillboardArgs
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector3 Axis;
		public BillboardOrientation Orientation;
		public float Angle;
		public Vector2 Size;
		public float Softness;
		public Vector3 Color;
		public float Alpha;
		public float ReferenceAlpha;
		public float AnimationTime;
		public float BlendMode;
	}


	// Data required by DrawRibbon()
	internal struct RibbonArgs
	{
		public Vector3 Position;
		public Vector3 Axis;
		public float Size;
		public float Softness;
		public Vector3 Color;
		public float Alpha;
		public float AnimationTime;
		public float ReferenceAlpha;
		public float BlendMode;
		public float TextureCoordinateU;
	}


	/// <summary>
	/// Renders billboards and particles in batches.
	/// </summary>
	/// <remarks>
	/// Billboards and particles are written into a dynamic vertex buffer. When the buffer size is 
	/// exceeded then the data is submitted and the batch is restarted.
	/// </remarks>
	interface IBillboardBatch : IDisposable
	{
		/// <summary>
		/// Begins a new batch.
		/// </summary>
		/// <param name="context">The render context.</param>
		void Begin(RenderContext context);


		/// <summary>
		/// Submits the current batch.
		/// </summary>
		void End();


		/// <summary>
		/// Adds a billboard to the batch.
		/// </summary>
		/// <param name="billboard">The billboard.</param>
		/// <param name="texture">The packed texture.</param>
		void DrawBillboard(ref BillboardArgs billboard, PackedTexture texture);


		/// <summary>
		/// Adds a ribbon segment to the batch.
		/// </summary>
		/// <param name="p0">The segment start.</param>
		/// <param name="p1">The segment end.</param>
		/// <param name="texture">The packed texture.</param>
		void DrawRibbon(ref RibbonArgs p0, ref RibbonArgs p1, PackedTexture texture);
	}
}
