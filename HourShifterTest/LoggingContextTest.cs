using HourShifter;
using log4net;
using Moq;
using NUnit.Framework;

namespace HourShifterTest
{
    [TestFixture]
    public class LoggingContextTest
    {
        [SetUp]
        public void TestSetup()
        {
            LoggingContext.ResetToDefault();
        }

        [Test]
        [Description("Tests that the LoggingContext is not null by default")]
        public void LoggingContext_IsNotNull()
        {
            Assert.That( LoggingContext.Current, Is.Not.Null );
        }

        [Test]
        [Description("Tests that you cannot set null to the LoggingContext")]
        public void LoggingContext_CannotSetNull()
        {
            ILog logger = null;
            Assert.That( () => { LoggingContext.Current = logger; }, Throws.ArgumentNullException );
        }

        [Test]
        [Description("Tests that ResetToDefault resets to the default logger")]
        public void LoggingContext_ResetToDefault_ResetsToDefaultLogger()
        {
            Mock<ILog> mockLogger = new Mock<ILog>();
            mockLogger.SetupGet( m => m.IsInfoEnabled ).Returns( false );
            LoggingContext.Current = mockLogger.Object;

            bool isInfoEnabled = LoggingContext.Current.IsInfoEnabled;
            LoggingContext.ResetToDefault();
            isInfoEnabled = LoggingContext.Current.IsInfoEnabled;

            mockLogger.Verify( m => m.IsInfoEnabled, Times.Once );
        }
    }
}
