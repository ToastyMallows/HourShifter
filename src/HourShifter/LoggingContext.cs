using System;
namespace HourShifter
{
	public enum LogLevel
	{
		Debug,
		Info,
		Warn,
		Error,
		Silent
	}

	public interface ILogger
	{
		void Debug(string logMessage);

		void Info(string logMessage);

		void Warn(string logMessage);

		void Error(string logMessage);

		void SetLogLevel(LogLevel newLogLevel);
	}

	public sealed class Logger : ILogger
	{
		private LogLevel _logLevel;

		public Logger(LogLevel logLevel = LogLevel.Info)
		{
			_logLevel = logLevel;
		}

		public void Debug(string logMessage)
		{
			if (_logLevel == LogLevel.Silent) return;
			if (_logLevel != LogLevel.Debug) return;

			Console.WriteLine("DEBUG: " + logMessage);
		}

		public void Info(string logMessage)
		{
			if (_logLevel == LogLevel.Silent) return;
			if (_logLevel > LogLevel.Info) return;

			Console.WriteLine("INFO: " + logMessage);
		}

		public void Warn(string logMessage)
		{
			if (_logLevel == LogLevel.Silent) return;
			if (_logLevel > LogLevel.Warn) return;

			Console.WriteLine("WARNING: " + logMessage);
		}

		public void Error(string logMessage)
		{
			if (_logLevel == LogLevel.Silent) return;

			Console.WriteLine("ERROR: " + logMessage);
		}

		public void SetLogLevel(LogLevel newLogLevel)
		{
			this.Debug($"Setting log level to {newLogLevel.ToString()} from {_logLevel.ToString()}");
			_logLevel = newLogLevel;
		}
	}

	internal static class LoggingContext
	{
		private static ILogger _current;

		static LoggingContext()
		{
			ResetToDefault();
		}

		public static ILogger Current
		{
			get
			{
				return _current;
			}
			set
			{
				_ = value ?? throw new ArgumentNullException(nameof(value));
				_current = value;
				Current.Debug($"LoggingContext changed.");
			}
		}

		public static void ResetToDefault()
		{
			_current = new Logger();
			Current.Debug($"LoggingContext reset to default.");
		}
	}
}
