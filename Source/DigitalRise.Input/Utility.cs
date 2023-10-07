using Microsoft.Xna.Framework;
using System;

namespace DigitalRise.Input
{
	internal static class Utility
	{
		public static Vector2 Absolute(this Vector2 v) => new Vector2(Math.Abs(v.X), Math.Abs(v.Y));

		public static bool IsLessThen(this Vector2 a, Vector2 b) => a.X < b.X && a.Y < b.Y;
	}
}
