using System;
using HourShifter;
using Moq;
using NUnit.Framework;

namespace HourShifterTest
{
	[TestFixture]
	public class ProgramTest
	{
		[Test]
		public void Constructor_ThrowsForNull()
		{
			Assert.Multiple(() =>
			{
				Assert.That(() =>
				{
					new ProgramBootstrap(null, Mock.Of<ILogger>());
				}, Throws.ArgumentNullException);

				Assert.That(() =>
				{
					new ProgramBootstrap(Mock.Of<IHourShifter>(), null);
				}, Throws.ArgumentNullException);
			});
		}

		[Test]
		public void Constructor_DoesNotThrow()
		{
			Assert.That(() =>
			{
				new ProgramBootstrap(Mock.Of<IHourShifter>(), Mock.Of<ILogger>());
			}, Throws.Nothing);
		}

		[Test]
		public void Run_ReturnsSuccess()
		{
			Mock<IHourShifter> mock = new Mock<IHourShifter>();

			mock
				.Setup(m => m.Shift().Result)
					.Returns(0);

			ProgramBootstrap programBootstrap = new ProgramBootstrap(mock.Object, Mock.Of<ILogger>());

			Assert.Multiple(() =>
			{
				int? exitCode = null;

				Assert.That(async () =>
				{
					exitCode = await programBootstrap.Run();
				}, Throws.Nothing);

				Assert.That(exitCode.HasValue);
				Assert.That(exitCode.Value, Is.EqualTo(Constants.SUCCESS_EXIT_CODE));
				mock.Verify(m => m.Shift(), Times.Once);
			});
		}

		[Test]
		public void Run_ReturnsFailure()
		{
			Mock<IHourShifter> mock = new Mock<IHourShifter>();

			mock
				.Setup(m => m.Shift().Result)
					.Throws(new Exception());

			ProgramBootstrap programBootstrap = new ProgramBootstrap(mock.Object, Mock.Of<ILogger>());

			Assert.Multiple(() =>
			{
				int? exitCode = null;

				Assert.That(async () =>
				{
					exitCode = await programBootstrap.Run();
				}, Throws.Nothing);

				Assert.That(exitCode.HasValue);
				Assert.That(exitCode.Value, Is.EqualTo(Constants.FAILURE_EXIT_CODE));
				mock.Verify(m => m.Shift(), Times.Once);
			});
		}
	}
}
