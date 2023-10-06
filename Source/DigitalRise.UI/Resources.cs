using AssetManagementBase;
using DigitalRise.UI.Rendering;

namespace DigitalRise.UI
{
	internal static class Resources
	{
		private static AssetManager _assetManagerResources = AssetManager.CreateResourceAssetManager(typeof(Resources).Assembly, "Resources");
		private static Theme _defaultTheme;

		public static Theme DefaultTheme
		{
			get
			{
				if (_defaultTheme != null)
				{
					return _defaultTheme;
				}

				_defaultTheme = _assetManagerResources.LoadTheme("DefaultTheme/Theme.xml");

				return _defaultTheme;
			}
		}
	}
}
