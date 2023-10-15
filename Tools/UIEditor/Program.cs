using System;

namespace UIEditor
{
	internal class Program
	{
		static void Main(string[] args)
		{
			try
			{
				using (var studio = new EditorGame(args))
				{
					studio.Run();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}