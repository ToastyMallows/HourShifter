using HourShifter;
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
			Assert.That(LoggingContext.Current, Is.Not.Null);
		}

		[Test]
		public void LoggingContext_CannotSetNull()
		{
			ILogger logger = null;
			Assert.That(() => { LoggingContext.Current = logger; }, Throws.ArgumentNullException);
		}

		[Test]
		public void LoggingContext_CanSetNewLogger()
		{
			ILogger logger = Mock.Of<ILogger>();
			Assert.That(() => { LoggingContext.Current = logger; }, Throws.Nothing);
		}
	}
}
