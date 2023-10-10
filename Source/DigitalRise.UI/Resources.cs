using AssetManagementBase;

namespace DigitalRise.UI
{
	internal static class Resources
	{
		public static readonly AssetManager AssetManager = AssetManager.CreateResourceAssetManager(typeof(Resources).Assembly, "Resources");
	}
}
