using System;
namespace HourShifter
{
    public interface ILogger
    {
        void Debug(string logMessage);

        void Error(string logMessage);

        void Info(string logMessage);

        void Warn(string logMessage);
    }

    public sealed class Logger : ILogger
    {
        public void Debug(string logMessage)
        {
            Console.WriteLine("DEBUG: " + logMessage);
        }

        public void Error(string logMessage)
        {
            Console.WriteLine("ERROR: " + logMessage);
        }

        public void Info(string logMessage)
        {
            Console.WriteLine("INFO: " + logMessage);
        }

        public void Warn(string logMessage)
        {
            Console.WriteLine("WARNING: " + logMessage);
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
                Guard.AgainstNull( value, nameof( ILogger ) );
                _current = value;
                Current.Debug( $"LoggingContext changed." );
            }
        }

        public static void ResetToDefault()
        {
            _current = new Logger();
            Current.Debug( $"LoggingContext reset to default." );
        }
    }
}