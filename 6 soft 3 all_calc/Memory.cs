﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6_soft_3_all_calc
{
	public class Memory
	{
		public enum MemoryState { On, Off }

		private MemoryState state;
		private Number number;

		public Memory()
		{
			state = MemoryState.Off;
			number = new PNumber();
		}

		public string CheckMemoryValue()
		{
			return number.ToString();
		}

		public void ClearMemory()
		{
			number.ClearNumber();
			state = MemoryState.Off;
		}

		public string SaveValueInMemory(PNumber newNumber)
		{
			state = MemoryState.On;

			number.Copy(newNumber);

			return number.ToString();
		}

		public string AddValueToMemory(PNumber newNumber)
		{
			state = MemoryState.On;

			number.Addition(newNumber);

			return number.ToString();
		}

		public MemoryState CheckMemoryState()
		{
			return state;
		}

		public void SetNotation(int newP)
		{
			((PNumber)number).ChangeP(newP);
		}

		public void ChangeCalculationModeInMemory(CalculationMode newMode)
		{
			number.ChangeCalculationMode(newMode);
		}
	}
}
