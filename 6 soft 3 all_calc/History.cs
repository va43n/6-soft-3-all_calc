using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6_soft_3_all_calc
{
	public class History
	{
		private List<string> records;

		public History()
		{
			records = new List<string>();
		}

		public void ClearHistory()
		{
			records.Clear();
		}

		public void Add(string record)
		{
			records.Add(record);
		}

		public List<string> GetHistory()
		{
			return records;
		}
	}
}
