using System.Reflection;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DigitalRise
{
	public static class DRBase
	{
		public static string Version
		{
			get
			{
				var assembly = typeof(DRBase).Assembly;
				var name = new AssemblyName(assembly.FullName);

				return name.Version.ToString();
			}
		}

		private static Game _game;

		public static Game Game
		{
			get
			{
				if (_game == null)
				{
					throw new Exception("DRBase.Game is null. Please, set it to the Game instance before using DigitalRise Engine.");
				}

				return _game;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (_game == value)
				{
					return;
				}

				_game = value;
			}
		}

		public static GraphicsDevice GraphicsDevice
		{
			get => Game.GraphicsDevice;
		}
	}
}
