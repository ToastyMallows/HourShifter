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
		private static string dllFile = "HourShifter.dll";
		private static string sampleJpg = "./SampleImages/sample.jpg";
		private static string samplePng = "./SampleImages/sample.png";
		private static string sampleJpgInvalidTime = "./SampleImages/sampleInvalidTime.jpg";

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
					yield return new Options { Hours = hour, CurrentDirectoryOnly = false };
					yield return new Options { Hours = hour, CurrentDirectoryOnly = true };
				}
			}
		}

		[Test]
		[TestCaseSource(nameof(Options_TestCases))]
		public void HourShifter_Constructor_DoesNotThrow(Options options)
		{
			Assert.That(() =>
			{
				new HourShifter.HourShifter(options, Mock.Of<IFileLoader>());
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
					new HourShifter.HourShifter(null, Mock.Of<IFileLoader>());
				}, Throws.ArgumentNullException);

				Assert.That(() =>
				{
					new HourShifter.HourShifter(options, null);
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

			HourShifter.HourShifter hourShifter = new HourShifter.HourShifter(options, mockFileLoader.Object);

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

			HourShifter.HourShifter hourShifter = new HourShifter.HourShifter(options, mockFileLoader.Object);

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

			HourShifter.HourShifter hourShifter = new HourShifter.HourShifter(options, mockFileLoader.Object);

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

			HourShifter.HourShifter hourShifter = new HourShifter.HourShifter(options, mockFileLoader.Object);

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

			HourShifter.HourShifter hourShifter = new HourShifter.HourShifter(options, mockFileLoader.Object);

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
	}
}
