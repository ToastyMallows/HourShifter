using CommandLine;

namespace HourShifter
{
	public sealed class Options
	{
		[Option('h', "hours", Required = false, HelpText = "Integer that represents the number of hours to shift the DateTaken EXIF value of the file(s).  Negative numbers will shift backward.  The default value is 12.")]
		public int? Hours { get; set; }

		[Option('c', "currentDirectoryOnly", Required = false, HelpText = "Flag to control whether or not the utility will search subdirectories for images.")]
		public bool CurrentDirectoryOnly { get; set; }

		[Option('l', "logLevel", Required = false, HelpText = "Flag to control the log level of the application.  Default: Info.  Acceptable values:  Debug, Info, Warn, Error")]
		public string LogLevel { get; set; }

		[Option('q', "quiet", Required = false, HelpText = "Flag for showing any output or waiting to press any key to exit.  Program will run and exit immediately without any output. Log level is ignored.")]
		public bool Quiet { get; set; }
	}
}
