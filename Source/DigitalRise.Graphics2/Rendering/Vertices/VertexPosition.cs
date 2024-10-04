using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DigitalRise.Rendering.Vertices
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VertexPosition : IVertexType
	{
		#region Private Properties

		VertexDeclaration IVertexType.VertexDeclaration
		{
			get
			{
				return VertexDeclaration;
			}
		}

		#endregion

		#region Public Variables

		public Vector3 Position;

		public float X
		{
			get => Position.X;
			set => Position.X = value;
		}

		public float Y
		{
			get => Position.Y;
			set => Position.Y = value;
		}

		public float Z
		{
			get => Position.Z;
			set => Position.Z = value;
		}

		#endregion

		#region Public Static Variables

		public static readonly VertexDeclaration VertexDeclaration;

		#endregion

		#region Private Static Constructor

		static VertexPosition()
		{
			VertexDeclaration = new VertexDeclaration(
				new VertexElement[]
				{
					new VertexElement(
						0,
						VertexElementFormat.Vector3,
						VertexElementUsage.Position,
						0
					)
				}
			);
		}

		#endregion

		#region Public Constructor

		public VertexPosition(Vector3 position)
		{
			Position = position;
		}

		public VertexPosition(float x, float y, float z) : this(new Vector3(x, y, z))
		{
		}

		#endregion

		#region Public Static Operators and Override Methods

		public override int GetHashCode()
		{
			// TODO: Fix GetHashCode
			return 0;
		}

		public override string ToString()
		{
			return (
				"{{Position:" + Position.ToString() +
				"}}"
			);
		}

		public static bool operator ==(VertexPosition left, VertexPosition right)
		{
			return left.Position == right.Position;
		}

		public static bool operator !=(VertexPosition left, VertexPosition right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != base.GetType())
			{
				return false;
			}
			return (this == ((VertexPosition)obj));
		}

		#endregion
	}
}
