using CommandLine;
using System;
using System.IO;

namespace HourShifter
{
    internal class Program
    {
        static int Main( string[] args )
        {
            int exitCode = Constants.SUCCESS_EXIT_CODE;

            Parser.Default.ParseArguments<Options>( args )
                .WithParsed( ( options ) =>
                    {
                        int? hoursToShift = options.Hours;

                        if( hoursToShift == null || hoursToShift == 0 )
                        {
                            LoggingContext.Current.Debug( $"Using the default number of hours ({Constants.DEFAULT_HOURS})" );
                            hoursToShift = Constants.DEFAULT_HOURS;
                        }

                        // Composition root
                        string currentDirectory = Directory.GetCurrentDirectory();
                        LoggingContext.Current.Debug( $"The current working directory is {currentDirectory}." );

                        FileLoader fileLoader = new FileLoader( currentDirectory, options.CurrentDirectoryOnly );
                        HourShifter hourShifter = new HourShifter( hoursToShift.Value, fileLoader );
                        ProgramBootstrap bootstrap = new ProgramBootstrap( hourShifter );

                        exitCode = bootstrap.Run();
                        LoggingContext.Current.Debug( $"Returning exit code {exitCode}." );

                        Console.Write( "Press any key to exit..." );
                        Console.ReadKey();
                    } )
                .WithNotParsed( ( errors ) =>
                    {
                        LoggingContext.Current.Debug( $"One or more errors occurred when parsing the command line parameters." );
                        LoggingContext.Current.Debug( string.Empty );

                        exitCode = Constants.FAILURE_EXIT_CODE;
                    } );

            return exitCode;
        }
    }

    internal class ProgramBootstrap
    {
        private readonly HourShifter _hourShifter;

        public ProgramBootstrap( HourShifter hourShifter )
        {
            Guard.AgainstNull( hourShifter, nameof( hourShifter ) );

            _hourShifter = hourShifter;
        }

        public int Run()
        {
            try
            {
                _hourShifter.Shift();
            }
            catch( Exception e )
            {
                LoggingContext.Current.Error( e.ToString() );
                LoggingContext.Current.Error( string.Empty );
                return Constants.FAILURE_EXIT_CODE;
            }

            return Constants.SUCCESS_EXIT_CODE;
        }
    }

    internal static class Constants
    {
        public static int SUCCESS_EXIT_CODE = 0;
        public static int FAILURE_EXIT_CODE = 1;
        public static int DEFAULT_HOURS = 12;
    }
}