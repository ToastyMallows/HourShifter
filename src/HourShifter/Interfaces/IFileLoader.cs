using System.Collections.Generic;
using System.Threading.Tasks;

namespace HourShifter
{
	public interface IFileLoader
	{
		IEnumerable<string> FindAllPaths();
		Task<byte[]> LoadImage(string path);
	}
}
