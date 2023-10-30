﻿using System;
using System.IO;
using System.Reflection;

namespace DigitalRise.LevelEditor
{
	public static class Utility
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
	}
}