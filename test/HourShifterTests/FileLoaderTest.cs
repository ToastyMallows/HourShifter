using HourShifter;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;

namespace HourShifterTest
{
	[TestFixture]
	public class FileLoaderTest
	{
		private static IEnumerable FileLoader_InvalidCurrentDirectories
		{
			get
			{
				yield return new TestCaseData(null);
				yield return new TestCaseData(string.Empty);
				yield return new TestCaseData(" ");
				yield return new TestCaseData(string.Empty + "\n");
				yield return new TestCaseData(string.Empty + "\r\n");
				yield return new TestCaseData('\t' + " ");
			}
		}

		[Test]
		[TestCaseSource(nameof(FileLoader_InvalidCurrentDirectories))]
		public void Constructor_ThrowsNullOrWhitespaceCurrentDirectory(string currentDirectory)
		{
			Assert.That(() =>
			{
				new FileLoader(new Options(), currentDirectory, Mock.Of<ILogger>());
			}, Throws.InstanceOf<ArgumentException>().Or.InstanceOf<ArgumentNullException>());
		}

		[Test]
		public void Constructor_ThrowsForNull()
		{
			Assert.Multiple(() =>
			{
				Assert.That(() =>
				{
					new FileLoader(null, Directory.GetCurrentDirectory(), Mock.Of<ILogger>());
				}, Throws.ArgumentNullException);

				Assert.That(() =>
				{
					new FileLoader(new Options(), Directory.GetCurrentDirectory(), null);
				}, Throws.ArgumentNullException);
			});
		}

		[Test]
		public void Constructor_DoesNotThrow()
		{
			Assert.Multiple(() =>
			{
				var directory = Directory.GetCurrentDirectory();
				var logger = new AggregateLogger();

				Assert.That(() =>
				{
					new FileLoader(new Options(), directory, logger);
				}, Throws.Nothing);

				Assert.That(logger.DebugLogs.Contains($"FileLoader created with current directory of {directory}, search current directories only: False"));
			});
		}

		[Test]
		public void AllPaths_NotNull([Values(true, false)] bool currentDirectoryOnly)
		{
			Options options = new Options
			{
				CurrentDirectoryOnly = currentDirectoryOnly
			};

			FileLoader fileLoader = new FileLoader(options, Directory.GetCurrentDirectory(), Mock.Of<ILogger>());
			Assert.That(fileLoader.FindAllPaths(), Is.Not.Null.Or.Empty);
		}

		[Test]
		[TestCaseSource(nameof(FileLoader_InvalidCurrentDirectories))]
		public void LoadImage_ThrowsForNullOrWhitespace(string dir)
		{
			Options options = new Options
			{
				CurrentDirectoryOnly = true
			};

			FileLoader fileLoader = new FileLoader(options, Directory.GetCurrentDirectory(), Mock.Of<ILogger>());

			Assert.That(async () =>
			{
				await fileLoader.LoadImage(dir);
			}, Throws.ArgumentException);
		}

		[Test]
		public void LoadImage_ReturnsBytes()
		{
			Options options = new Options
			{
				CurrentDirectoryOnly = true
			};

			FileLoader fileLoader = new FileLoader(options, Directory.GetCurrentDirectory(), Mock.Of<ILogger>());
			Assert.Multiple(() =>
			{
				byte[] bytes = null;

				Assert.That(async () =>
				{
					bytes = await fileLoader.LoadImage(Path.GetFullPath("./SampleImages/sample.jpg"));
				}, Throws.Nothing);

				Assert.That(bytes, Is.Not.Null);
			});
		}
	}
}
