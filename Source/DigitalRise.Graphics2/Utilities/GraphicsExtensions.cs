﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DigitalRise.Rendering.Vertices;

using VertexPosition = DigitalRise.Rendering.Vertices.VertexPosition;

namespace DigitalRise.Utilities
{
	internal static class GraphicsExtensions
	{
		private class ColorInfo
		{
			public Color Color { get; set; }
			public string Name { get; set; }
		}

		private static readonly Dictionary<string, ColorInfo> _colors = new Dictionary<string, ColorInfo>();

		static GraphicsExtensions()
		{
			var type = typeof(Color);

			var colors = type.GetRuntimeProperties();

			foreach (var c in colors)
			{
				if (!c.GetMethod.IsStatic &&
					c.PropertyType != typeof(Color))
				{
					continue;
				}

				var value = (Color)c.GetValue(null, null);
				_colors[c.Name.ToLower()] = new ColorInfo
				{
					Color = value,
					Name = c.Name
				};
			}
		}

		public static string ToHexString(this Color c)
		{
			return string.Format("#{0}{1}{2}{3}",
				c.R.ToString("X2"),
				c.G.ToString("X2"),
				c.B.ToString("X2"),
				c.A.ToString("X2"));
		}

		public static string GetColorName(this Color color)
		{
			foreach (var c in _colors)
			{
				if (c.Value.Color == color)
				{
					return c.Value.Name;
				}
			}

			return null;
		}

		public static Color? FromName(this string name)
		{
			if (name.StartsWith("#"))
			{
				name = name.Substring(1);
				uint u;
				if (uint.TryParse(name, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out u))
				{
					// Parsed value contains color in RGBA form
					// Extract color components

					byte r, g, b, a;

					unchecked
					{
						r = (byte)(u >> 24);
						g = (byte)(u >> 16);
						b = (byte)(u >> 8);
						a = (byte)u;
					}

					return new Color(r, g, b, a);
				}
			}
			else
			{
				ColorInfo result;
				if (_colors.TryGetValue(name.ToLower(), out result))
				{
					return result.Color;
				}
			}

			return null;
		}

		public static Point Size(this Rectangle r)
		{
			return new Point(r.Width, r.Height);
		}

		public static void SetSize(ref Rectangle r, Point size)
		{
			r.Width = size.X;
			r.Height = size.Y;
		}

		public static Rectangle Add(this Rectangle r, Point p)
		{
			return new Rectangle(r.X + p.X, r.Y + p.Y, r.Width, r.Height);
		}

		public static VertexBuffer CreateVertexBuffer<T>(this T[] vertices) where T : struct, IVertexType
		{
			var device = DR.GraphicsDevice;
			var result = new VertexBuffer(device,
				new T().VertexDeclaration,
				vertices.Length,
				BufferUsage.None);
			result.SetData(vertices);

			return result;
		}

		public static IndexBuffer CreateIndexBuffer(this ushort[] indices)
		{
			var device = DR.GraphicsDevice;
			var result = new IndexBuffer(device,
				IndexElementSize.SixteenBits,
				indices.Length,
				BufferUsage.None);
			result.SetData(indices);

			return result;
		}

		public static IndexBuffer CreateIndexBuffer(this int[] indices)
		{
			var device = DR.GraphicsDevice;
			var result = new IndexBuffer(device,
				IndexElementSize.ThirtyTwoBits,
				indices.Length,
				BufferUsage.None);
			result.SetData(indices);

			return result;
		}

		public static IEnumerable<Vector3> GetPositions(this VertexPositionNormalTexture[] vertices) => (from v in vertices select v.Position);
		public static IEnumerable<Vector3> GetPositions(this VertexPositionTexture[] vertices) => (from v in vertices select v.Position);
		public static IEnumerable<Vector3> GetPositions(this VertexPositionNormal[] vertices) => (from v in vertices select v.Position);
		public static IEnumerable<Vector3> GetPositions(this VertexPosition[] vertices) => (from v in vertices select v.Position);

		public static EffectParameter FindParameterByName(this Effect effect, string name)
		{
			foreach (var par in effect.Parameters)
			{
				if (par.Name == name)
				{
					return par;
				}
			}

			return null;
		}
	}
}