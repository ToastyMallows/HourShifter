using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace HourShifter
{
	internal class FileLoader : IFileLoader
	{
		private const string ALL_FILES_WILDCARD = "*";
		private readonly string _currentDirectory;
		private readonly Options _options;

		public FileLoader(Options options, string currentDirectory)
		{
			Guard.AgainstNull(options, nameof(options));
			Guard.AgainstNullOrWhitespace(currentDirectory, nameof(currentDirectory));

			_options = options;
			_currentDirectory = currentDirectory;

			LoggingContext.Current.Debug($"{nameof(FileLoader)} created with current directory of {_currentDirectory}, search current directories only: {_options.CurrentDirectoryOnly}");
		}

		public IEnumerable<string> FindAllPaths()
		{
			SearchOption searchOption = SearchOption.AllDirectories;

			if (_options.CurrentDirectoryOnly)
			{
				searchOption = SearchOption.TopDirectoryOnly;
			}

			return Directory.EnumerateFiles(_currentDirectory, ALL_FILES_WILDCARD, searchOption);
		}

		public async Task<byte[]> LoadImage(string path)
		{
			Guard.AgainstNullOrWhitespace(path, nameof(path));

			return await File.ReadAllBytesAsync(path);
		}
	}
}
