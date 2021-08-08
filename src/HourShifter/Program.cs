using System.Diagnostics.CodeAnalysis;
using CommandLine;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HourShifter
{
	internal class Program
	{
		[ExcludeFromCodeCoverage]
		static async Task<int> Main(string[] args)
		{
			int exitCode = Constants.SUCCESS_EXIT_CODE;

			ParserResult<Options> parserResult = Parser.Default.ParseArguments<Options>(args);

			parserResult
				.WithNotParsed((errors) =>
				{
					exitCode = Constants.FAILURE_EXIT_CODE;
				});

			await parserResult
				.WithParsedAsync(async (options) =>
				{
					if (!options.Quiet)
					{
						if (Enum.TryParse(options.LogLevel, out LogLevel logLevel))
						{
							LoggingContext.Current.SetLogLevel(logLevel);
						}
					}
					else
					{
						LoggingContext.Current.SetLogLevel(LogLevel.Silent);
					}

					if (!options.Hours.HasValue || options.Hours == 0)
					{
						LoggingContext.Current.Debug($"Using the default number of hours ({Constants.DEFAULT_HOURS})");
						options.Hours = Constants.DEFAULT_HOURS;
					}

					// Composition root
					string currentDirectory = Directory.GetCurrentDirectory();
					LoggingContext.Current.Debug($"The current working directory is {currentDirectory}.");

					IFileLoader fileLoader = new FileLoader(options, currentDirectory);
					IHourShifter hourShifter = new HourShifter(options, fileLoader);
					ProgramBootstrap bootstrap = new ProgramBootstrap(hourShifter);

					exitCode = await bootstrap.Run();
					LoggingContext.Current.Debug($"Returning exit code {exitCode}.");

					if (!options.Quiet)
					{
						Console.Write("Press any key to exit...");
						Console.ReadKey();
					}
				});

			LoggingContext.Current.Debug($"Returning exit code: {exitCode}");
			return exitCode;
		}
	}

	internal class ProgramBootstrap
	{
		private readonly IHourShifter _hourShifter;

		public ProgramBootstrap(IHourShifter hourShifter)
		{
			Guard.AgainstNull(hourShifter, nameof(hourShifter));

			_hourShifter = hourShifter;
		}

		public async Task<int> Run()
		{
			try
			{
				int filesShifted = await _hourShifter.Shift();

				LoggingContext.Current.Info($"Shifted {filesShifted} files.{Environment.NewLine}");
			}
			catch (Exception e)
			{
				LoggingContext.Current.Error($"{e.ToString()}{Environment.NewLine}");
				return Constants.FAILURE_EXIT_CODE;
			}

			return Constants.SUCCESS_EXIT_CODE;
		}
	}
}
