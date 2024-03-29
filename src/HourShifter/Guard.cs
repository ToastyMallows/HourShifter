using System;

namespace HourShifter
{
	internal static class Guard
	{
		public static void AgainstNull(object value, string parameterName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		public static void AgainstNullOrWhitespace(string str, string parameterName)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				throw new ArgumentException($"{parameterName} is null or whitespace", parameterName);
			}
		}
	}
}
