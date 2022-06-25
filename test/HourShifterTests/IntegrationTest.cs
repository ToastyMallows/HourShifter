using System;
using System.IO;
using HourShifter;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using System.Globalization;

namespace HourShifterTest.IntegrationTests
{
	[TestFixture]
	public class IntegrationTests
	{
		private static string sampleJpgFile = Path.GetFullPath("./SampleImages/sample.jpg");

		[Test]
		public async Task HourShifter_ShiftsDateTakenExifValue()
		{
			Options options = new Options
			{
				Hours = 1,
				CurrentDirectoryOnly = false,
				Quiet = true
			};

			var aggregateLogger = new AggregateLogger();

			IFileLoader fileLoader = new FileLoader(options, Directory.GetCurrentDirectory(), aggregateLogger);
			IHourShifter hourShifter = new HourShifter.HourShifter(options, fileLoader, aggregateLogger);
			ProgramBootstrap programBootstrap = new ProgramBootstrap(hourShifter, aggregateLogger);

			DateTime originalDateTime = await getJpgDateTaken(sampleJpgFile);

			Assert.Multiple(async () =>
			{
				int? exitCode = null;

				Assert.That(async () =>
				{
					exitCode = await programBootstrap.Run();
				}, Throws.Nothing);

				DateTime newDateTime = await getJpgDateTaken(sampleJpgFile);

				Assert.That(originalDateTime != newDateTime);
				Assert.That(originalDateTime.AddHours(1) == newDateTime);

				Assert.That(exitCode.HasValue);
				Assert.That(exitCode.Value, Is.Zero);
			});
		}

		private static async Task<DateTime> getJpgDateTaken(string path)
		{
			const string DATE_TAKEN_DATETIME_FORMAT = "yyyy:MM:dd HH:mm:ss";
			byte[] originalImageBytes = await File.ReadAllBytesAsync(path);
			using (MemoryStream memoryStream = new MemoryStream(originalImageBytes))
			using (Image image = await Image.LoadAsync(memoryStream))
			{
				IExifValue<string> dateCreated = image.Metadata.ExifProfile.GetValue<string>(ExifTag.DateTimeOriginal);

				if (!DateTime.TryParseExact(dateCreated.Value, DATE_TAKEN_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTaken))
				{
					Assert.Fail("sample.jpg is missing a DateTaken or it's invalid.");
				}

				return dateTaken;
			}
		}
	}
}
