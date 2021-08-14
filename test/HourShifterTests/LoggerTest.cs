using System.Collections;
using System;
using HourShifter;
using NUnit.Framework;

namespace HourShifterTest
{
	[TestFixture]
	public class LoggerTest
	{
		[Test]
		public void Logger_Constructor_DoesNotThrow()
		{
			Assert.Multiple(() =>
			{
				Assert.That(() =>
				{
					new Logger();
				}, Throws.Nothing);

				foreach (LogLevel logLevel in Enum.GetValues<LogLevel>())
				{
					Assert.That(() =>
					{
						new Logger(logLevel);
					}, Throws.Nothing);
				}
			});
		}

		public static IEnumerable LogLevel_TestCases
		{
			get
			{
				foreach (LogLevel logLevel in Enum.GetValues<LogLevel>())
				{
					yield return logLevel;
				}
			}
		}

		[Test]
		[TestCaseSource(nameof(LogLevel_TestCases))]
		public void Logger_Debug_DoesNotThrow(LogLevel logLevel)
		{
			Assert.That(() =>
			{
				new Logger(logLevel).Debug(string.Empty);
			}, Throws.Nothing);
		}

		[Test]
		[TestCaseSource(nameof(LogLevel_TestCases))]
		public void Logger_Info_DoesNotThrow(LogLevel logLevel)
		{
			Assert.That(() =>
			{
				new Logger(logLevel).Info(string.Empty);
			}, Throws.Nothing);
		}

		[Test]
		[TestCaseSource(nameof(LogLevel_TestCases))]
		public void Logger_Warn_DoesNotThrow(LogLevel logLevel)
		{
			Assert.That(() =>
			{
				new Logger(logLevel).Warn(string.Empty);
			}, Throws.Nothing);
		}

		[Test]
		[TestCaseSource(nameof(LogLevel_TestCases))]
		public void Logger_Error_DoesNotThrow(LogLevel logLevel)
		{
			Assert.That(() =>
			{
				new Logger(logLevel).Error(string.Empty);
			}, Throws.Nothing);
		}

		[Test]
		[TestCaseSource(nameof(LogLevel_TestCases))]
		public void Logger_SetLogLevel_DoesNotThrow(LogLevel logLevel)
		{
			Assert.That(() =>
			{
				new Logger(logLevel).SetLogLevel(logLevel);
			}, Throws.Nothing);
		}
	}
}
