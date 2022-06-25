using System.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace HourShifter
{
	internal class HourShifter : IHourShifter
	{
		private const string DATE_TAKEN_DATETIME_FORMAT = "yyyy:MM:dd HH:mm:ss";

		private readonly bool _shiftBackwards;
		private readonly Options _options;
		private readonly TimeSpan _hoursToShift;
		private readonly IFileLoader _fileLoader;
		private readonly ILogger _logger;

		public HourShifter(Options options, IFileLoader fileLoader, ILogger logger)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
			_fileLoader = fileLoader ?? throw new ArgumentNullException(nameof(fileLoader));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			int hoursToShift = options.Hours.Value;

			if (_options.Hours < 0)
			{
				_shiftBackwards = true;
				hoursToShift *= -1; // make positive again for TimeSpan
			}

			_hoursToShift = TimeSpan.FromHours(hoursToShift);
		}

		public async Task<int> Shift()
		{
			_logger.Info($"Beginning the hour shift...");
			_logger.Info($"Shifting all images {(_shiftBackwards ? "backwards" : "forwards")} {_hoursToShift.Hours} hour(s).{Environment.NewLine}");

			IEnumerable<string> allPaths = _fileLoader.FindAllPaths();

			int pathsCount = allPaths.Count();

			_logger.Debug($"Found {pathsCount} files.");

			if (allPaths.Count() == 0)
			{
				_logger.Info($"Didn't find any files to shift.");
				return 0;
			}

			int filesShifted = 0;

			foreach (string path in allPaths)
			{
				// See if the file is an image
				try
				{
					using (Image image = Image.Load(path)) { }
				}
				catch
				{
					_logger.Debug($"This file doesn't seem to be an image: {path}");
					continue;
				}

				try
				{
					byte[] originalImageBytes = await _fileLoader.LoadImage(path);
					using (MemoryStream memoryStream = new MemoryStream(originalImageBytes))
					using (Image image = await Image.LoadAsync(memoryStream))
					{
						IExifValue<string> dateCreated = image.Metadata.ExifProfile.GetValue<string>(ExifTag.DateTimeOriginal);

						if (!DateTime.TryParseExact(dateCreated.Value, DATE_TAKEN_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTaken))
						{
							_logger.Warn($"Could not parse the Date/Time {dateCreated.Value} for image located at {path}.");
							continue;
						}

						_logger.Info($"Shifting Date/Time taken for image {path}...");

						long shiftedTicks;
						if (_shiftBackwards)
						{
							shiftedTicks = dateTaken.Ticks - _hoursToShift.Ticks;
						}
						else
						{
							shiftedTicks = dateTaken.Ticks + _hoursToShift.Ticks;
						}

						DateTime shiftedDateTaken = new DateTime(shiftedTicks);
						image.Metadata.ExifProfile.SetValue<string>(ExifTag.DateTimeOriginal, shiftedDateTaken.ToString(DATE_TAKEN_DATETIME_FORMAT));

						await image.SaveAsJpegAsync(path);

						_logger.Info($"The original Date/Time {dateTaken} was shifted to {shiftedDateTaken}.{Environment.NewLine}");
						filesShifted++;
					}
				}
				catch (Exception e)
				{
					_logger.Error($"File located at '{path}' has issues. Skipping this file.");
					_logger.Error($"{e.ToString()}{Environment.NewLine}");
				}
			}

			if (filesShifted == 0)
			{
				_logger.Info(@$"No images were modified! Either: 
\t1. No images were found in the current directory{(!_options.CurrentDirectoryOnly ? " (including subfolders)" : string.Empty)}
  or
\t2. Images were found but errors occurred.
Run this utility with Debug logging turned on for more information");
			}

			return filesShifted;
		}
	}
}
