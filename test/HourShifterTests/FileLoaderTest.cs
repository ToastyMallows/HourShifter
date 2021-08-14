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
				yield return new TestCaseData(string.Empty + Environment.NewLine);
				yield return new TestCaseData('\t' + " ");
			}
		}

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
		[TestCaseSource(nameof(FileLoader_InvalidCurrentDirectories))]
		public void FileLoader_Constructor_ThrowsNullOrWhitespaceCurrentDirectory(string currentDirectory)
		{
			Assert.That(() =>
			{
				new FileLoader(new Options(), currentDirectory);
			}, Throws.InstanceOf<ArgumentException>().Or.InstanceOf<ArgumentNullException>());
		}

		[Test]
		public void FileLoader_Constructor_ThrowsForNull()
		{
			Assert.That(() =>
			{
				new FileLoader(null, Directory.GetCurrentDirectory());
			}, Throws.ArgumentNullException);
		}

		[Test]
		public void FileLoader_AllPaths_NotNull([Values(true, false)] bool currentDirectoryOnly)
		{
			Options options = new Options
			{
				CurrentDirectoryOnly = currentDirectoryOnly
			};

			FileLoader fileLoader = new FileLoader(options, Directory.GetCurrentDirectory());
			Assert.That(fileLoader.FindAllPaths(), Is.Not.Null.Or.Empty);
		}

		[Test]
		[TestCaseSource(nameof(FileLoader_InvalidCurrentDirectories))]
		public void FileLoader_LoadImage_ThrowsForNullOrWhitespace(string dir)
		{
			Options options = new Options
			{
				CurrentDirectoryOnly = true
			};

			FileLoader fileLoader = new FileLoader(options, Directory.GetCurrentDirectory());
			Assert.That(async () =>
			{
				await fileLoader.LoadImage(dir);
			}, Throws.ArgumentException);
		}

		[Test]
		public void FileLoader_LoadImage_ReturnsBytes()
		{
			Options options = new Options
			{
				CurrentDirectoryOnly = true
			};

			FileLoader fileLoader = new FileLoader(options, Directory.GetCurrentDirectory());
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
