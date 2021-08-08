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
		public void ProgramBootstrap_Constructor_ThrowsForNull()
		{
			Assert.That(() =>
			{
				new ProgramBootstrap(null);
			}, Throws.ArgumentNullException);
		}

		[Test]
		public void ProgramBootstrap_Constructor_DoesNotThrow()
		{
			Assert.That(() =>
			{
				new ProgramBootstrap(Mock.Of<IHourShifter>());
			}, Throws.Nothing);
		}

		[Test]
		public void ProgramBootstrap_Run_ReturnsSuccess()
		{
			Mock<IHourShifter> mock = new Mock<IHourShifter>();

			mock
				.Setup(m => m.Shift().Result)
					.Returns(0)
						.Verifiable();

			ProgramBootstrap programBootstrap = new ProgramBootstrap(mock.Object);

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
		public void ProgramBootstrap_Run_ReturnsFailure()
		{
			Mock<IHourShifter> mock = new Mock<IHourShifter>();

			mock
				.Setup(m => m.Shift().Result)
					.Throws(new Exception())
						.Verifiable();

			ProgramBootstrap programBootstrap = new ProgramBootstrap(mock.Object);

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
