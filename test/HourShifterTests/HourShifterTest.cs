using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using HourShifter;
using Moq;
using NUnit.Framework;
using System.Collections;
using System;

namespace HourShifterTest
{
	[TestFixture]
	public class HourShifterTest
	{
		private static string dllFile = Path.GetFullPath("./HourShifter.dll");
		private static string sampleJpg = Path.GetFullPath("./SampleImages/sample.jpg");
		private static string samplePng = Path.GetFullPath("./SampleImages/sample.png");
		private static string sampleJpgInvalidTime = Path.GetFullPath("./SampleImages/sampleInvalidTime.jpg");

		private static IEnumerable<string> sampleDllList = new List<string> { dllFile };
		private static IEnumerable<string> sampleJpgList = new List<string> { sampleJpg };
		private static IEnumerable<string> samplePngList = new List<string> { samplePng };
		private static IEnumerable<string> sampleJpgInvalidTimeList = new List<string> { sampleJpgInvalidTime };

		private async Task<Mock<IFileLoader>> createMockFileLoader(
			IEnumerable<string> list = null,
			string file = null
		)
		{
			list ??= sampleJpgList;
			file ??= sampleJpg;

			byte[] bytes = await File.ReadAllBytesAsync(file);

			Mock<IFileLoader> mock = new Mock<IFileLoader>();

			mock
				.Setup(m => m.FindAllPaths())
					.Returns(list)
						.Verifiable();

			mock
				.Setup(m => m.LoadImage(It.IsAny<string>()).Result)
					.Returns(bytes)
						.Verifiable();

			return mock;
		}

		public static IEnumerable Options_TestCases
		{
			get
			{
				List<int> hours = new List<int> { -1, 0, 1 };

				foreach (int hour in hours)
				{
					yield return new Options { Hours = hour, CurrentDirectoryOnly = false, Quiet = true };
					yield return new Options { Hours = hour, CurrentDirectoryOnly = true, Quiet = true };
				}
			}
		}

		[Test]
		[TestCaseSource(nameof(Options_TestCases))]
		public void HourShifter_Constructor_DoesNotThrow(Options options)
		{
			Assert.That(() =>
			{
				new HourShifter.HourShifter(options, Mock.Of<IFileLoader>(), Mock.Of<ILogger>());
			}, Throws.Nothing);
		}

		[Test]
		[TestCaseSource(nameof(Options_TestCases))]
		public void HourShifter_Constructor_ThrowsForNull(Options options)
		{
			Assert.Multiple(() =>
			{
				Assert.That(() =>
				{
					new HourShifter.HourShifter(null, Mock.Of<IFileLoader>(), Mock.Of<ILogger>());
				}, Throws.ArgumentNullException);

				Assert.That(() =>
				{
					new HourShifter.HourShifter(options, null, Mock.Of<ILogger>());
				}, Throws.ArgumentNullException);

				Assert.That(() =>
				{
					new HourShifter.HourShifter(options, Mock.Of<IFileLoader>(), null);
				}, Throws.ArgumentNullException);
			});
		}

		[Test]
		[TestCaseSource(nameof(Options_TestCases))]
		public void HourShifter_Shift_DoesntShiftAnything(Options options)
		{
			Mock<IFileLoader> mockFileLoader = new Mock<IFileLoader>();

			mockFileLoader
				.Setup(m => m.FindAllPaths())
					.Returns(new List<string>())
						.Verifiable();

			mockFileLoader
				.Setup(m => m.LoadImage(It.IsAny<string>()))
					.ThrowsAsync(new Exception());

			HourShifter.HourShifter hourShifter = new HourShifter.HourShifter(options, mockFileLoader.Object, Mock.Of<ILogger>());

			Assert.Multiple(() =>
			{
				int? shiftedFiles = null;

				Assert.That(async () =>
				{
					shiftedFiles = await hourShifter.Shift();
				}, Throws.Nothing);

				Assert.That(shiftedFiles.HasValue);
				Assert.That(shiftedFiles.Value, Is.Zero);
				mockFileLoader.Verify(m => m.FindAllPaths(), Times.Once);
				mockFileLoader.Verify(m => m.LoadImage(It.IsAny<string>()), Times.Never);
			});
		}

		[Test]
		[TestCaseSource(nameof(Options_TestCases))]
		public async Task HourShifter_Shift_IgnoresNonImages(Options options)
		{
			Mock<IFileLoader> mockFileLoader = await createMockFileLoader(sampleDllList);

			HourShifter.HourShifter hourShifter = new HourShifter.HourShifter(options, mockFileLoader.Object, Mock.Of<ILogger>());

			Assert.Multiple(() =>
			{
				int? shiftedFiles = null;

				Assert.That(async () =>
				{
					shiftedFiles = await hourShifter.Shift();
				}, Throws.Nothing);

				Assert.That(shiftedFiles.HasValue);
				Assert.That(shiftedFiles.Value, Is.Zero);
				mockFileLoader.Verify(m => m.FindAllPaths(), Times.Once);
				mockFileLoader.Verify(m => m.LoadImage(It.IsAny<string>()), Times.Never);
			});
		}

		[Test]
		[TestCaseSource(nameof(Options_TestCases))]
		public async Task HourShifter_Shift_IgnoresNonJpg(Options options)
		{
			Mock<IFileLoader> mockFileLoader = await createMockFileLoader(samplePngList, samplePng);

			HourShifter.HourShifter hourShifter = new HourShifter.HourShifter(options, mockFileLoader.Object, Mock.Of<ILogger>());

			Assert.Multiple(() =>
			{
				int? shiftedFiles = null;

				Assert.That(async () =>
				{
					shiftedFiles = await hourShifter.Shift();
				}, Throws.Nothing);

				Assert.That(shiftedFiles.HasValue);
				Assert.That(shiftedFiles.Value, Is.Zero);
				mockFileLoader.Verify(m => m.FindAllPaths(), Times.Once);
				mockFileLoader.Verify(m => m.LoadImage(It.IsAny<string>()), Times.Once);
			});
		}

		[Test]
		[TestCaseSource(nameof(Options_TestCases))]
		public async Task HourShifter_Shift_IgnoresJpgWithInvalidExif(Options options)
		{
			Mock<IFileLoader> mockFileLoader =
				await createMockFileLoader(sampleJpgInvalidTimeList, sampleJpgInvalidTime);

			HourShifter.HourShifter hourShifter = new HourShifter.HourShifter(options, mockFileLoader.Object, Mock.Of<ILogger>());

			Assert.Multiple(() =>
			{
				int? shiftedFiles = null;

				Assert.That(async () =>
				{
					shiftedFiles = await hourShifter.Shift();
				}, Throws.Nothing);

				Assert.That(shiftedFiles.HasValue);
				Assert.That(shiftedFiles.Value, Is.Zero);
				mockFileLoader.Verify(m => m.FindAllPaths(), Times.Once);
				mockFileLoader.Verify(m => m.LoadImage(It.IsAny<string>()), Times.Once);
			});
		}

		[Test]
		[TestCaseSource(nameof(Options_TestCases))]
		public async Task HourShifter_Shift_ShiftsJpgFile(Options options)
		{
			Mock<IFileLoader> mockFileLoader = await createMockFileLoader();

			HourShifter.HourShifter hourShifter = new HourShifter.HourShifter(options, mockFileLoader.Object, Mock.Of<ILogger>());

			Assert.Multiple(() =>
			{
				int? shiftedFiles = null;

				Assert.That(async () =>
				{
					shiftedFiles = await hourShifter.Shift();
				}, Throws.Nothing);

				Assert.That(shiftedFiles.HasValue);
				Assert.That(shiftedFiles.Value, Is.EqualTo(1));
				mockFileLoader.Verify(m => m.FindAllPaths(), Times.Once);
				mockFileLoader.Verify(m => m.LoadImage(It.IsAny<string>()), Times.Once);
			});
		}
	}
}
