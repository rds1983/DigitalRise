using Newtonsoft.Json;
using DigitalRise.Rendering;
using System.ComponentModel;

namespace DigitalRise.Standard
{
	public class MeshNode : MeshNodeBase
	{
		[Browsable(false)]
		[JsonIgnore]
		public Mesh Mesh { get; set; }

		protected override Mesh RenderMesh => Mesh;
	}
}
