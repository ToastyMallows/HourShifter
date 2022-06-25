using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace HourShifter
{
	internal class FileLoader : IFileLoader
	{
		private const string ALL_FILES_WILDCARD = "*";
		private readonly string _currentDirectory;
		private readonly ILogger _logger;
		private readonly Options _options;

		public FileLoader(Options options, string currentDirectory, ILogger logger)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
			_currentDirectory = !string.IsNullOrWhiteSpace(currentDirectory) ? currentDirectory : throw new ArgumentException(nameof(currentDirectory));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			_logger.Debug($"{nameof(FileLoader)} created with current directory of {_currentDirectory}, search current directories only: {_options.CurrentDirectoryOnly}");
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
			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException(nameof(path));
			}

			return await File.ReadAllBytesAsync(path);
		}
	}
}
