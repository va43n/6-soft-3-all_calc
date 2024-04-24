using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6_soft_3_all_calc
{
	public class CalculatorException : Exception
	{
		public CalculatorException(string message) : base(message) { }
	}
}
