using System.Net.Mime;
using System;
using System.Linq;
using System.IO;
using HourShifter;
using NUnit.Framework;
using System.Drawing;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Text;

namespace HourShifterTest.IntegrationTests
{
	[TestFixture]
	public class IntegrationTests
	{
		private static string sampleJpgFile = "./SampleImages/sample.jpg";

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			LoggingContext.Current = new Logger(LogLevel.Silent);
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			LoggingContext.ResetToDefault();
		}

		[Test]
		public async Task HourShifter_ShiftsDateTakenExifValue()
		{
			Options options = new Options
			{
				Hours = 1,
				CurrentDirectoryOnly = false,
				Quiet = true
			};

			IFileLoader fileLoader = new FileLoader(options, Directory.GetCurrentDirectory());
			IHourShifter hourShifter = new HourShifter.HourShifter(options, fileLoader);
			ProgramBootstrap programBootstrap = new ProgramBootstrap(hourShifter);

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
			Regex _regex = new Regex(":");
			byte[] originalImageBytes = await File.ReadAllBytesAsync(path);
			using (MemoryStream memoryStream = new MemoryStream(originalImageBytes))
			using (Image image = Image.FromStream(memoryStream, false, false))
			{
				PropertyItem propertyItem = image.GetPropertyItem(36867);
				string dateTakenString = _regex.Replace(Encoding.UTF8.GetString(propertyItem.Value), "-", 2);

				DateTime dateTaken;
				if (!DateTime.TryParse(dateTakenString, out dateTaken))
				{
					Assert.Fail("sample.jpg is missing a DateTaken or it's invalid.");
				}

				return dateTaken;
			}
		}
	}
}
