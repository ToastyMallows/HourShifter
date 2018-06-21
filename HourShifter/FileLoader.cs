using System.Collections.Generic;
using System.IO;

namespace HourShifter
{
    internal class FileLoader
    {
        private const string ALL_FILES_WILDCARD = "*";
        private readonly string _currentDirectory;
        private readonly bool _currentDirectoryOnly;

        public FileLoader( string currentDirectory, bool currentDirectoryOnly )
        {
            Guard.AgainstNullOrWhitespace( currentDirectory, nameof( currentDirectory ) );

            _currentDirectory = currentDirectory;
            _currentDirectoryOnly = currentDirectoryOnly;

            LoggingContext.Current.Debug( $"{nameof( FileLoader )} created with current directory of {_currentDirectory}, search current directories only: {currentDirectoryOnly}" );
        }

        public IEnumerable<string> AllPaths
        {
            get
            {
                SearchOption searchOption = SearchOption.AllDirectories;

                if( _currentDirectoryOnly )
                {
                    searchOption = SearchOption.TopDirectoryOnly;
                }

                return Directory.EnumerateFiles( _currentDirectory, ALL_FILES_WILDCARD, searchOption );
            }
        }
    }
}
