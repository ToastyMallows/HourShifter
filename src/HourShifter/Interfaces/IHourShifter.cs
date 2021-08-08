using System.Threading.Tasks;

namespace HourShifter
{
	public interface IHourShifter
	{
		Task<int> Shift();
	}
}
