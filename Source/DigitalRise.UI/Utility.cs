using System;

namespace DigitalRise
{
	internal static class Utility
	{
		public static int MouseWheelScrollDelta = 120;
		public static int MouseWheelScrollLines = 3;
		public static bool IsClipboardSupported = false;

		public static void SetClipboardText(string text)
		{
		}

		public static string GetClipboardText()
		{
			return string.Empty;
		}
	}
}
