using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Reflection;

namespace DigitalRise.Studio.Utility
{
	public static class CommonUtils
	{
#if FNA
		public const string EffectsPrefix = "FNA/bin/";
#elif DIRECTX
		public const string EffectsPrefix = "MonoGameDX11/bin/";
#endif

		public static string ExecutingAssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().Location;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}

		public static T GetService<T>(this IServiceProvider servies) => (T)servies.GetService(typeof(T));

		public static Color ToColor(this Vector3 v)
		{
			return new Color(v);
		}

		public static Vector3 ToVector3(this Color c)
		{
			return new Vector3(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f);
		}
	}
}