using Microsoft.Xna.Framework;
using DigitalRise.Rendering;
using DigitalRise.Utilities;
using DigitalRise.Data.Meshes;

namespace DigitalRise.Primitives
{
	public class Box : PrimitiveMeshNode
	{
		private Vector3 _size = Vector3.One;

		public Vector3 Size
		{
			get => _size;

			set
			{
				if (value.EpsilonEquals(_size))
				{
					return;
				}

				_size = value;
				InvalidateMesh();
			}
		}

		protected override Mesh CreateMesh() => MeshHelper.CreateBox(Size, UScale, VScale, IsLeftHanded);
	}
}