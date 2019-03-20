using CommandLine;

namespace HourShifter
{
    internal sealed class Options
    {
        [Option( 'h', "hours", Required = false, HelpText = "Integer that represents the number of hours to shift the DateTaken EXIF value of the file(s).  Negative numbers will shift backward.  The default value is 12." )]
        public int? Hours { get; set; }

        [Option('c', "currentDirectoryOnly", Required = false, HelpText = "Flag to control whether or not the utility will search subdirectories for images.")]
        public bool CurrentDirectoryOnly { get; set; }
    }
}