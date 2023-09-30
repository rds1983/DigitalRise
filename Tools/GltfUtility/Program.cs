using System;
using System.Reflection;
using DigitalRise;
using DigitalRise.CommandLine;

namespace EffectFarm
{
	class Program
	{
		// See system error codes: http://msdn.microsoft.com/en-us/library/windows/desktop/ms681382.aspx
		private const int ERROR_SUCCESS = 0;
		private const int ERROR_BAD_ARGUMENTS = 160;        // 0x0A0
		private const int ERROR_UNHANDLED_EXCEPTION = 574;  // 0x23E

		static SwitchValueArgument<string> _fileArgument;
		static SwitchValueArgument<string> _outputArgument;
		static SwitchArgument _tangentArgument;
		static SwitchArgument _unwindArgument;
		static SwitchArgument _premultiplyArgument;
		static SwitchValueArgument<float> _scaleArgument;
		static SwitchArgument _helpArgument;

		public static string Version
		{
			get
			{
				var assembly = typeof(Program).Assembly;
				var name = new AssemblyName(assembly.FullName);

				return name.Version.ToString();
			}
		}

		static void Log(string message)
		{
			Console.WriteLine(message);
		}

		static CommandLineParser ConfigureCommandLine()
		{
			var parser = new CommandLineParser
			{
				AllowUnknownArguments = false,
				Description = "This command-line tool that allows to edit gltf/glb files(version 2).",
				HelpHeader = string.Empty,
				HelpFooter = string.Empty
			};

			var categoryMandatory = "Mandatory";
			var categoryOptional = "Optional";

			_fileArgument = new SwitchValueArgument<string>("file",
					new ValueArgument<string>("file", "The input/output gltf/glb(version 2) file."),
					"Defines the input/output file that will be processed",
					null, 
					new[] {'f'})
			{
				Category = categoryMandatory
			};

			parser.Arguments.Add(_fileArgument);

			_outputArgument = new SwitchValueArgument<string>("output",
					new ValueArgument<string>("output", "Name of the output file."),
					"Defines the file name where output will be written",
					null,
					new[] { 'o' })
			{
				Category = categoryOptional,
				IsOptional = true
			};
			parser.Arguments.Add(_outputArgument);

			_tangentArgument = new SwitchArgument("tangent",
				"Determines whether to generate tangent frames.",
				null,
				new[] { 't' })
			{
				Category = categoryOptional,
				IsOptional = true
			};
			parser.Arguments.Add(_tangentArgument);

			_unwindArgument = new SwitchArgument("unwind",
				"Unwind indices.",
				null,
				new[] { 'u' })
			{
				Category = categoryOptional,
				IsOptional = true
			};
			parser.Arguments.Add(_unwindArgument);

			_premultiplyArgument = new SwitchArgument("premultiply",
				"Premultiply vertex colors.",
				null,
				new[] { 'p' })
			{
				Category = categoryOptional,
				IsOptional = true
			};
			parser.Arguments.Add(_premultiplyArgument);

			_scaleArgument = new SwitchValueArgument<float>("scale",
					new ValueArgument<float>("scale", "Scale size."),
					"Defines the scale that should be applied to the model",
					null,
					new[] { 's' })
			{
				Category = categoryOptional,
				IsOptional = true
			};
			parser.Arguments.Add(_scaleArgument);

			// --help, -h, -?
			_helpArgument = new SwitchArgument(
					"help",
					"Shows help information.",
					null,
					new[] { 'h', '?' })
			{
				Category = "Help",
				IsOptional = true,
			};
			parser.Arguments.Add(_helpArgument);

			return parser;
		}

		static int Process(string[] args)
		{
			var parser = ConfigureCommandLine();
			ParseResult parseResult;
			try
			{
				parseResult = parser.Parse(args);

				if (parseResult.ParsedArguments[_helpArgument] != null)
				{
					// Show help and exit.
					Console.WriteLine(parser.GetHelp());
					return ERROR_SUCCESS;
				}

				parser.ThrowIfMandatoryArgumentIsMissing(parseResult);
			}
			catch (CommandLineParserException exception)
			{
				Console.Error.WriteLine("ERROR");
				Console.Error.WriteLineIndented(exception.Message, 4);
				Console.Error.WriteLine();
				Console.Out.WriteLine("SYNTAX");
				Console.Out.WriteLineIndented(parser.GetSyntax(), 4);
				Console.Out.WriteLine();
				Console.Out.WriteLine("Try 'GltfUtility --help' for more information.");
				return ERROR_BAD_ARGUMENTS;
			}

			var processor = new GltfProcessor();

			var file = ((ArgumentResult<string>)parseResult.ParsedArguments[_fileArgument]).Values[0];
			var output = ((ArgumentResult<string>)parseResult.ParsedArguments[_outputArgument])?.Values[0];
			var genTangentFrames = parseResult.ParsedArguments[_tangentArgument] != null;
			var unwindIndices = parseResult.ParsedArguments[_unwindArgument] != null;
			var premultiply = parseResult.ParsedArguments[_premultiplyArgument] != null;
			float? scale = null;
			if (parseResult.ParsedArguments[_scaleArgument] != null)
			{
				scale = ((ArgumentResult<float>)parseResult.ParsedArguments[_scaleArgument]).Values[0];
			}

			processor.Process(file, output, genTangentFrames, unwindIndices, premultiply, scale);

			return ERROR_SUCCESS;
		}

		static int Main(string[] args)
		{
			try
			{
				return Process(args);
			}
			catch (Exception ex)
			{
				Log(ex.ToString());
				return ERROR_UNHANDLED_EXCEPTION;
			}
		}
	}
}