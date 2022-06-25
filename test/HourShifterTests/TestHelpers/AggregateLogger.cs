using System.Collections.Generic;
using HourShifter;

namespace HourShifterTest
{
	public class AggregateLogger : ILogger
	{
		public List<string> DebugLogs = new List<string>();
		public List<string> ErrorLogs = new List<string>();
		public List<string> InfoLogs = new List<string>();
		public List<string> WarnLogs = new List<string>();

		public void Debug(string logMessage)
		{
			DebugLogs.Add(logMessage);
		}

		public void Error(string logMessage)
		{
			ErrorLogs.Add(logMessage);
		}

		public void Info(string logMessage)
		{
			InfoLogs.Add(logMessage);
		}

		public void SetLogLevel(LogLevel newLogLevel)
		{
			// no-op, accept all logs
		}

		public void Warn(string logMessage)
		{
			WarnLogs.Add(logMessage);
		}
	}
}
