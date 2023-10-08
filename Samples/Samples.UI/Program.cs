using System;
using System.Diagnostics;


namespace Samples
{
  internal static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    private static void Main(string[] args)
    {
      if (Debugger.IsAttached)
      {
        // The debugger is attached:
        // Run the XNA game and let the debugger handle any exception messages.
        using (var game = new SampleGame())
          game.Run();
      }
      else
      {
        // The debugger is NOT attached:
        // Run the XNA game and use a MessageBox to display any exception messages.
        try
        {
          using (var game = new SampleGame())
            game.Run();
        }
        catch (Exception exception)
        {
          string message = Utility.GetExceptionMessage(exception);
          Console.WriteLine(message);
        }
      }
    }
  }
}
