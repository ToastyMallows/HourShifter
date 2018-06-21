using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HourShifter
{
    internal class HourShifter
    {
        private const string DATE_TAKEN_DATETIME_FORMAT = "yyyy:MM:dd HH:mm:ss\0";
        private const int DATE_TAKEN_PROPERTY_ID = 36867;

        private readonly bool _shiftBackwards;
        private readonly TimeSpan _hoursToShift;
        private readonly FileLoader _fileLoader;
        private static Regex _regex = new Regex( ":" );

        public HourShifter( int hoursToShift, FileLoader fileLoader )
        {
            Guard.AgainstZero( hoursToShift, nameof( hoursToShift ) );
            Guard.AgainstNull( fileLoader, nameof( fileLoader ) );

            if ( hoursToShift < 0 )
            {
                _shiftBackwards = true;
                hoursToShift *= -1; // make positive again for TimeSpan
            }

            _hoursToShift = TimeSpan.FromHours( hoursToShift );
            _fileLoader = fileLoader;
        }

        public void Shift()
        {
            LoggingContext.Current.Info( string.Empty );
            LoggingContext.Current.Info( "Beginning the hour shift..." );
            LoggingContext.Current.Info( string.Empty );

            IEnumerable<string> allPaths = _fileLoader.AllPaths;

            int pathsCount = allPaths.Count();

            LoggingContext.Current.Debug( $"Found {pathsCount} files." );

            if( allPaths.Count() == 0 )
            {
                LoggingContext.Current.Info( $"Didn't find any files to shift." );
                return;
            }

            int filesShifted = 0;

            foreach( string path in allPaths )
            {
                try
                {
                    // See if the file is an image
                    try
                    {
                        using( Image image = Image.FromFile( path ) ) { }
                    }
                    catch
                    {
                        LoggingContext.Current.Debug( $"This file doesn't seem to be an image: {path}" );
                        continue;
                    }

                    byte[] originalImageBytes = File.ReadAllBytes( path );
                    using( MemoryStream memoryStream = new MemoryStream( originalImageBytes ) )
                    using( Image image = Image.FromStream( memoryStream, false, false ) )
                    {
                        ImageFormat imageFormat = image.RawFormat;
                        PropertyItem propertyItem = image.GetPropertyItem( DATE_TAKEN_PROPERTY_ID );
                        string dateTakenString = _regex.Replace( Encoding.UTF8.GetString( propertyItem.Value ), "-", 2 );

                        DateTime dateTaken;
                        if( !DateTime.TryParse( dateTakenString, out dateTaken ) )
                        {
                            LoggingContext.Current.Warn( $"Could not parse the Date/Time {dateTakenString} for image located at {path}." );
                            continue;
                        }

                        LoggingContext.Current.Info( $"Shifting Date/Time taken for image {path}..." );

                        long shiftedTicks;
                        if( _shiftBackwards )
                        {
                            shiftedTicks = dateTaken.Ticks - _hoursToShift.Ticks;
                        }
                        else
                        {
                            shiftedTicks = dateTaken.Ticks + _hoursToShift.Ticks;
                        }

                        DateTime shiftedDateTaken = new DateTime( shiftedTicks );
                        propertyItem.Value = Encoding.UTF8.GetBytes( shiftedDateTaken.ToString( DATE_TAKEN_DATETIME_FORMAT ) );
                        image.SetPropertyItem( propertyItem );

                        image.Save( path, imageFormat );

                        LoggingContext.Current.Info( $"The original Date/Time {dateTaken} was shifted to {shiftedDateTaken}." );
                        filesShifted++;
                    }
                }
                catch( Exception e )
                {
                    LoggingContext.Current.Error( $"File located at '{path}' has issues. Skipping this file." );
                    LoggingContext.Current.Error( e.ToString() );
                    LoggingContext.Current.Error( string.Empty );
                }

                LoggingContext.Current.Info( string.Empty );
            }

            if( filesShifted == 0 )
            {
                LoggingContext.Current.Info( $"No images were modified! Either: " );
                LoggingContext.Current.Info( $"\t1. No images were found in the current directory (including subfolders)" );
                LoggingContext.Current.Info( $"  or" );
                LoggingContext.Current.Info( $"\t2. Images were found but errors occurred." );
                LoggingContext.Current.Info( string.Empty );
                LoggingContext.Current.Info( $"Run this utility with Debug logging turned on for more information" );
                LoggingContext.Current.Info( string.Empty );
            }
        }
    }
}
