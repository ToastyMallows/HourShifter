using log4net;

namespace HourShifter
{
    internal static class LoggingContext
    {
        private static ILog _current;

        static LoggingContext()
        {
            ResetToDefault();
        }

        public static ILog Current
        {
            get
            {
                return _current;
            }
            set
            {
                Guard.AgainstNull( value, nameof( ILog ) );
                _current = value;
                Current.Debug( $"LoggingContext changed." );
            }
        }

        public static void ResetToDefault()
        {
            _current = LogManager.GetLogger( nameof( HourShifter ) );
            Current.Debug( $"LoggingContext reset to default." );
        }
    }
}
