using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Samples
{
	internal static class Utility
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

		/// <summary>
		/// Gets a human-readable exception message for an exception instance.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns>A exception message which can be displayed in a message box.</returns>
		public static string GetExceptionMessage(Exception exception)
		{
			var stringBuilder = new StringBuilder();

			// Exception on main thread.
			stringBuilder.AppendLine("An unexpected error has occurred.");
			WriteMessagesAndStackTraces(stringBuilder, exception);

			return stringBuilder.ToString();
		}


		private static void WriteMessagesAndStackTraces(StringBuilder stringBuilder, Exception exception)
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Exception text:");
			while (exception != null)
			{
				stringBuilder.AppendLine(exception.Message);
				stringBuilder.AppendLine(exception.StackTrace);
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Inner exception:");

				// Continue with inner exception.
				exception = exception.InnerException;
			}

			stringBuilder.AppendLine("-");
		}

		public static T GetService<T>(this IServiceProvider servies) => (T)servies.GetService(typeof(T));
	}
}
