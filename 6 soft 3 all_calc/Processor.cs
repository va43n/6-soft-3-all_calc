using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace _6_soft_3_all_calc
{
	public class Processor
	{
		private Operation operation;
		private Number leftOperand;
		private Number rightOperand;

		public Processor()
		{
			operation = Operation.None;

			leftOperand = new PNumber();
			rightOperand = new PNumber();
		}

		public void ChangeTypeOfNumber(TypeOfNumber typeOfNumber, CalculationMode calculationMode)
		{
			operation = Operation.None;

			if (typeOfNumber == TypeOfNumber.PNumber)
			{
				leftOperand = new PNumber();
				rightOperand = new PNumber();
			}
			else if (typeOfNumber == TypeOfNumber.Fraction)
			{
				leftOperand = new Fraction();
				rightOperand = new Fraction();
			}
			else if (typeOfNumber == TypeOfNumber.Complex)
			{
				leftOperand = new Complex();
				rightOperand = new Complex();
			}

			leftOperand.ChangeCalculationMode(calculationMode);
			rightOperand.ChangeCalculationMode(calculationMode);
		}

		public string CalculateOperation(Operation operation)
		{
			if (operation != Operation.None)
			{
				if (this.operation == Operation.Addition)
					leftOperand.Addition(rightOperand);
				else if (this.operation == Operation.Subtraction)
					leftOperand.Subtraction(rightOperand);
				else if (this.operation == Operation.Multiplication)
					leftOperand.Multiplication(rightOperand);
				else if (this.operation == Operation.Division)
					leftOperand.Division(rightOperand);
			}

			if (operation == Operation.Addition)
				this.operation = Operation.Addition;
			else if (operation == Operation.Subtraction)
				this.operation = Operation.Subtraction;
			else if (operation == Operation.Multiplication)
				this.operation = Operation.Multiplication;
			else if (operation == Operation.Division)
				this.operation = Operation.Division;

			return leftOperand.ToString();
		}

		public string CalculateFunction(Function function)
		{
			if (operation == Operation.None)
			{
				if (function == Function.Square)
					leftOperand.Square();
				else if (function == Function.Reverse)
					leftOperand.Reverse();
				else if (function == Function.SquareRoot)
					((PNumber)leftOperand).SquareRoot();

				return leftOperand.ToString();
			}

			if (function == Function.Square)
				rightOperand.Square();
			else if (function == Function.Reverse)
				rightOperand.Reverse();
			else if (function == Function.SquareRoot)
				((PNumber)rightOperand).SquareRoot();

			return rightOperand.ToString();
		}

		public void ClearProcessor()
		{
			operation = Operation.None;

			leftOperand.ClearNumber();
			rightOperand.ClearNumber();
		}

		public void SetOperand(Number number)
		{
			if (operation == Operation.None)
				leftOperand.Copy(number);
			else
				rightOperand.Copy(number);
		}

		public void SetOperation(int tag)
		{
			if (tag == -1)
				operation = Operation.None;
			else if (tag == 23)
				operation = Operation.Addition;
			else if (tag == 24)
				operation = Operation.Subtraction;
			else if (tag == 25)
				operation = Operation.Multiplication;
			else if (tag == 26)
				operation = Operation.Division;
		}

		public string SetNotation(int newP)
		{
			((PNumber)leftOperand).ChangeP(newP);

			rightOperand.ClearNumber();
			((PNumber)rightOperand).ChangeP(newP);

			return leftOperand.ToString();
		}

		public string SetSameOperand()
		{
			rightOperand.Copy(leftOperand);

			return rightOperand.ToString();
		}

		public Operation CheckOperation()
		{
			return operation;
		}

		public string ChangeCalculationModeInOperands(CalculationMode newMode)
		{
			leftOperand.ChangeCalculationMode(newMode);
			rightOperand.ChangeCalculationMode(newMode);

			return leftOperand.ToString();
		}
	}
}
