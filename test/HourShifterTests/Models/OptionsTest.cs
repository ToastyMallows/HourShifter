using HourShifter;
using NUnit.Framework;

namespace HourShifterTest
{
	[TestFixture]
	public class OptionsTests
	{
		[Test]
		public void Options_Constructor_DoesNotThrow()
		{
			Assert.That(() =>
			{
				new Options();
			}, Throws.Nothing);
		}

		[Test]
		public void Options_GettersGet_SettersSet()
		{
			Options options = new Options
			{
				Hours = 1,
				CurrentDirectoryOnly = true,
				LogLevel = nameof(LogLevel.Silent),
				Quiet = true,
			};

			Assert.Multiple(() =>
			{
				Assert.That(options.Hours, Is.EqualTo(1));
				Assert.That(options.CurrentDirectoryOnly, Is.True);
				Assert.That(options.LogLevel, Is.EqualTo(nameof(LogLevel.Silent)));
				Assert.That(options.Quiet, Is.True);
			});
		}
	}
}
