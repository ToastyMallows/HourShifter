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
		private const int DATE_TAKEN_PROPERTY_ID = 36867;

		private readonly bool _shiftBackwards;
		private readonly Options _options;
		private readonly TimeSpan _hoursToShift;
		private readonly IFileLoader _fileLoader;
		private readonly static Regex _regex = new Regex(":");

		public HourShifter(Options options, IFileLoader fileLoader)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
			_fileLoader = fileLoader ?? throw new ArgumentNullException(nameof(fileLoader));

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
			LoggingContext.Current.Info($"Beginning the hour shift...");
			LoggingContext.Current.Info($"Shifting all images {(_shiftBackwards ? "backwards" : "forwards")} {_hoursToShift.Hours} hour(s).{Environment.NewLine}");

			IEnumerable<string> allPaths = _fileLoader.FindAllPaths();

			int pathsCount = allPaths.Count();

			LoggingContext.Current.Debug($"Found {pathsCount} files.");

			if (allPaths.Count() == 0)
			{
				LoggingContext.Current.Info($"Didn't find any files to shift.");
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
					LoggingContext.Current.Debug($"This file doesn't seem to be an image: {path}");
					continue;
				}

				try
				{
					byte[] originalImageBytes = await _fileLoader.LoadImage(path);
					using (MemoryStream memoryStream = new MemoryStream(originalImageBytes))
					using (Image image = await Image.LoadAsync(memoryStream))
					{
						IExifValue<string> dateCreated = image.Metadata.ExifProfile.GetValue<string>(ExifTag.DateTimeOriginal);

						if (DateTime.TryParseExact(dateCreated.Value, DATE_TAKEN_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTaken))
						{
							LoggingContext.Current.Warn($"Could not parse the Date/Time {dateCreated.Value} for image located at {path}.");
							continue;
						}

						LoggingContext.Current.Info($"Shifting Date/Time taken for image {path}...");

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

						LoggingContext.Current.Info($"The original Date/Time {dateTaken} was shifted to {shiftedDateTaken}.{Environment.NewLine}");
						filesShifted++;
					}
				}
				catch (Exception e)
				{
					LoggingContext.Current.Error($"File located at '{path}' has issues. Skipping this file.");
					LoggingContext.Current.Error($"{e.ToString()}{Environment.NewLine}");
				}
			}

			if (filesShifted == 0)
			{
				LoggingContext.Current.Info($"No images were modified! Either: ");
				LoggingContext.Current.Info($"\t1. No images were found in the current directory{(!_options.CurrentDirectoryOnly ? " (including subfolders)" : string.Empty)}");
				LoggingContext.Current.Info($"  or");
				LoggingContext.Current.Info($"\t2. Images were found but errors occurred.{Environment.NewLine}");
				LoggingContext.Current.Info($"Run this utility with Debug logging turned on for more information{Environment.NewLine}");
			}

			return filesShifted;
		}
	}
}
