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
					ILogger logger = new Logger();
					if (!options.Quiet)
					{
						if (Enum.TryParse(options.LogLevel, out LogLevel logLevel))
						{
							logger.SetLogLevel(logLevel);
						}
					}
					else
					{
						logger.SetLogLevel(LogLevel.Silent);
					}

					if (!options.Hours.HasValue || options.Hours == 0)
					{
						logger.Debug($"Using the default number of hours ({Constants.DEFAULT_HOURS})");
						options.Hours = Constants.DEFAULT_HOURS;
					}

					// Composition root
					string currentDirectory = Directory.GetCurrentDirectory();
					logger.Debug($"The current working directory is {currentDirectory}.");

					IFileLoader fileLoader = new FileLoader(options, currentDirectory, logger);
					IHourShifter hourShifter = new HourShifter(options, fileLoader, logger);
					ProgramBootstrap bootstrap = new ProgramBootstrap(hourShifter, logger);

					exitCode = await bootstrap.Run();
					logger.Debug($"Returning exit code {exitCode}.");
				});

			return exitCode;
		}
	}

	internal class ProgramBootstrap
	{
		private readonly IHourShifter _hourShifter;
		private readonly ILogger _logger;

		public ProgramBootstrap(IHourShifter hourShifter, ILogger logger)
		{
			_hourShifter = hourShifter ?? throw new ArgumentNullException(nameof(hourShifter));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<int> Run()
		{
			try
			{
				int filesShifted = await _hourShifter.Shift();

				_logger.Info($"Shifted {filesShifted} files.{Environment.NewLine}");
			}
			catch (Exception e)
			{
				_logger.Error($"{e.ToString()}{Environment.NewLine}");
				return Constants.FAILURE_EXIT_CODE;
			}

			return Constants.SUCCESS_EXIT_CODE;
		}
	}
}
