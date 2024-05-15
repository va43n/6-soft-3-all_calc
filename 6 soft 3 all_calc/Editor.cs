using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace _6_soft_3_all_calc
{
	public abstract class Editor
	{
		public abstract string AddDigit(int index);
		public abstract string AddDelimeter();
		public abstract string ChangeSign();
		public abstract string ClearInput();
		public abstract string ClearEdit();
		public abstract string DeleteRightmostSymbol();
		public abstract string PrepareNumberToCalculate();
		public abstract void ClearEditor();
		public abstract void ChangeCurrentNumber(string newNumber);
		public abstract string GetCurrentNumber();
		public abstract void UpdateFormula(string part);
		public abstract string GetFormula();
	}

	public class PNumberEditor : Editor
	{
		private string currentNumber;
		private string formula;

		public PNumberEditor()
		{
			currentNumber = Constants.zero;
			formula = "";
		}

		public override string AddDigit(int index)
		{
			currentNumber = currentNumber == Constants.zero ? PNumber.alphabet[index].ToString() : currentNumber + PNumber.alphabet[index];

			return currentNumber;
		}

		public override string AddDelimeter()
		{
			if (currentNumber.IndexOf(Constants.standardDelimeter) == -1)
				currentNumber += currentNumber == "" ? Constants.zero + Constants.standardDelimeter : Constants.standardDelimeter;

			return currentNumber;
		}

		public override string ChangeSign()
		{
			if (currentNumber[0] == Constants.stringSign[0])
				currentNumber = currentNumber.Remove(0, 1);
			else
				currentNumber = currentNumber.Insert(0, Constants.stringSign);

			return currentNumber;
		}

		public override string ClearInput()
		{
			currentNumber = Constants.zero;
			formula = "";

			return currentNumber;
		}

		public override string ClearEdit()
		{
			currentNumber = Constants.zero;

			return currentNumber;
		}

		public override string DeleteRightmostSymbol()
		{
			if (currentNumber.Length == 1)
			{
				currentNumber = Constants.zero;
				return currentNumber;
			}
			currentNumber = currentNumber.Remove(currentNumber.Length - 1, 1);

			return currentNumber;
		}

		public override string PrepareNumberToCalculate()
		{
			TransformToNormalNumber();

			return currentNumber;
		}

		public override void ChangeCurrentNumber(string newNumber)
		{
			currentNumber = newNumber;
		}

		public override string GetCurrentNumber()
		{
			return currentNumber;
		}

		public override void ClearEditor()
		{
			formula = "";
			currentNumber = Constants.zero;
		}

		public override string GetFormula()
		{
			return formula;
		}

		public override void UpdateFormula(string part)
		{
			formula += part;
		}

		public void ChangeSignInFormula(string newSign)
		{
			formula = formula.Remove(formula.Length - 1, 1);
			formula += newSign;
		}

		private void TransformToNormalNumber()
		{
			for (int i = currentNumber.Length - 1; i >= 0; i--)
			{
				if (currentNumber[i] != Constants.zero[0] && currentNumber[i] != Constants.standardDelimeter[0] || currentNumber.IndexOf(Constants.standardDelimeter) == -1)
					break;
				currentNumber = currentNumber.Remove(i, 1);
			}

			if (currentNumber == Constants.stringSign + Constants.zero)
				currentNumber = Constants.zero;
		}
	}

	public class FractionEditor : Editor
	{
		private string currentNumber;
		private string formula;

		public FractionEditor()
		{
			currentNumber = Constants.zero;
			formula = "";
		}

		public override string AddDigit(int index)
		{
			if (currentNumber == Constants.zero)
				currentNumber = Fraction.alphabet[index].ToString();
			else if (currentNumber.IndexOf(Constants.fractionDelimeter) == currentNumber.Length - 1 && index == 0)
				return currentNumber;
			else
				currentNumber += Fraction.alphabet[index].ToString();

			return currentNumber;
		}

		public override string AddDelimeter()
		{
			if (currentNumber.IndexOf(Constants.fractionDelimeter) == -1)
				currentNumber += currentNumber == "" ? Constants.zero + Constants.fractionDelimeter : Constants.fractionDelimeter;

			return currentNumber;
		}

		public override string ChangeSign()
		{
			if (currentNumber[0] == Constants.stringSign[0])
				currentNumber = currentNumber.Remove(0, 1);
			else
				currentNumber = currentNumber.Insert(0, Constants.stringSign);

			return currentNumber;
		}

		public override string ClearInput()
		{
			currentNumber = Constants.zero;
			formula = "";

			return currentNumber;
		}

		public override string ClearEdit()
		{
			currentNumber = Constants.zero;

			return currentNumber;
		}

		public override string DeleteRightmostSymbol()
		{
			if (currentNumber.Length == 1)
			{
				currentNumber = Constants.zero;
				return currentNumber;
			}
			currentNumber = currentNumber.Remove(currentNumber.Length - 1, 1);

			return currentNumber;
		}

		public override string PrepareNumberToCalculate()
		{
			TransformToNormalNumber();

			return currentNumber;
		}

		public override void ChangeCurrentNumber(string newNumber)
		{
			currentNumber = newNumber;
		}

		public override string GetCurrentNumber()
		{
			return currentNumber;
		}

		public override void ClearEditor()
		{
			formula = "";
			currentNumber = Constants.zero;
		}

		public override string GetFormula()
		{
			return formula;
		}

		public override void UpdateFormula(string part)
		{
			formula += part;
		}

		public void ChangeSignInFormula(string newSign)
		{
			formula = formula.Remove(formula.Length - 1, 1);
			formula += newSign;
		}

		private void TransformToNormalNumber()
		{
			if (currentNumber.IndexOf(Constants.fractionDelimeter) == -1)
				currentNumber += Constants.fractionDelimeter + "1";
		}
	}

	public class ComplexEditor : Editor
	{
		private string currentNumber;
		private string formula;

		public ComplexEditor()
		{
			currentNumber = Constants.zero;
			formula = "";
		}

		public override string AddDigit(int index)
		{
			if (currentNumber == Constants.zero)
				currentNumber = Complex.alphabet[index].ToString();
			else
				currentNumber += Complex.alphabet[index];

			return currentNumber;
		}

		public override string AddDelimeter()
		{
			long numberOfDelimeters = 0;

			foreach (char letter in currentNumber)
				if (letter == Constants.standardDelimeter[0])
					numberOfDelimeters++;

			if (numberOfDelimeters < 1 || (numberOfDelimeters < 2 && currentNumber.IndexOf(Constants.complexDelimeter) != -1))
			{
				if (currentNumber[currentNumber.Length - 1] == Constants.complexDelimeter[0])
					currentNumber += Constants.zero + Constants.standardDelimeter;
				else
					currentNumber += Constants.standardDelimeter;
			}

			return currentNumber;
		}

		public string AddSignAndComplexDelimeter(string sign)
		{
			currentNumber += sign + Constants.complexDelimeter;

			return currentNumber;
		}

		public string AddComplexDelimeter()
		{
			if (currentNumber.IndexOf(Constants.complexDelimeter) != -1)
				return currentNumber;

			if (currentNumber == Constants.zero)
				currentNumber = Constants.complexDelimeter;
			else
				currentNumber += Constants.formulaSymbols[1] + Constants.complexDelimeter;

			return currentNumber;
		}

		public override string ChangeSign()
		{
			int iPosition = currentNumber.IndexOf(Constants.complexDelimeter);

			if (iPosition == -1)
			{
				if (currentNumber[0] == Constants.stringSign[0])
					currentNumber = currentNumber.Remove(0, 1);
				else
					currentNumber = currentNumber.Insert(0, Constants.stringSign);
			}
			else
			{
				if (currentNumber[iPosition - 1] == Constants.stringSign[0])
				{
					currentNumber = currentNumber.Remove(iPosition - 1, 1);
					currentNumber = currentNumber.Insert(iPosition - 1, "+");
				}
				else
				{
					currentNumber = currentNumber.Remove(iPosition - 1, 1);
					currentNumber = currentNumber.Insert(iPosition - 1, Constants.stringSign);
				}
			}

			return currentNumber;
		}

		public override string ClearInput()
		{
			currentNumber = Constants.zero;
			formula = "";

			return currentNumber;
		}

		public override string ClearEdit()
		{
			currentNumber = Constants.zero;

			return currentNumber;
		}

		public override string DeleteRightmostSymbol()
		{
			if (currentNumber.Length == 1)
			{
				currentNumber = Constants.zero;
				return currentNumber;
			}

			currentNumber = currentNumber.Remove(currentNumber.Length - 1, 1);

			return currentNumber;
		}

		public override string PrepareNumberToCalculate()
		{
			TransformToNormalNumber();

			return currentNumber;
		}

		public override void ChangeCurrentNumber(string newNumber)
		{
			currentNumber = newNumber;
		}

		public override string GetCurrentNumber()
		{
			return currentNumber;
		}

		public override void ClearEditor()
		{
			formula = "";
			currentNumber = Constants.zero;
		}

		public override string GetFormula()
		{
			return formula;
		}

		public override void UpdateFormula(string part)
		{
			formula += part;
		}

		public void ChangeSignInFormula(string newSign)
		{
			formula = formula.Remove(formula.Length - 1, 1);
			formula += newSign;
		}

		private void TransformToNormalNumber()
		{
			int firstDelimeterPosition = -1, secondDelimeterPosition = -1;

			if (currentNumber[currentNumber.Length - 1] == Constants.formulaSymbols[1][0] || currentNumber[currentNumber.Length - 1] == Constants.formulaSymbols[2][0])
				currentNumber = currentNumber.Substring(0, currentNumber.Length - 1);

			for (int i = 0; i < currentNumber.Length; i++)
			{
				if (currentNumber[i] == Constants.standardDelimeter[0])
				{
					if (firstDelimeterPosition == -1)
						firstDelimeterPosition = i;
					else
						secondDelimeterPosition = i;
				}
			}

			if (secondDelimeterPosition != -1)
			{
				for (int pos = currentNumber.IndexOf(Constants.complexDelimeter) - 1; pos >= firstDelimeterPosition; pos--)
				{
					if (currentNumber[pos] == Constants.zero[0] || currentNumber[pos] == Constants.standardDelimeter[0])
						currentNumber = currentNumber.Substring(0, pos) + currentNumber.Substring(pos + 1);
					else
						break;
				}

				for (int pos = currentNumber.Length - 1; pos >= secondDelimeterPosition; pos--)
				{
					if (currentNumber[pos] == Constants.zero[0] || currentNumber[pos] == Constants.standardDelimeter[0])
						currentNumber = currentNumber.Remove(pos, 1);
					else
						break;
				}
			}

			else if (firstDelimeterPosition != -1)
			{
				if (firstDelimeterPosition < currentNumber.IndexOf(Constants.complexDelimeter))
				{
					for (int pos = currentNumber.IndexOf(Constants.complexDelimeter) - 1; pos >= firstDelimeterPosition; pos--)
					{
						if (currentNumber[pos] == Constants.zero[0] || currentNumber[pos] == Constants.standardDelimeter[0])
							currentNumber = currentNumber.Substring(0, pos) + currentNumber.Substring(pos + 1);
						else
							break;
					}
				}

				else
				{
					for (int pos = currentNumber.Length - 1; pos >= firstDelimeterPosition; pos--)
					{
						if (currentNumber[pos] == Constants.zero[0] || currentNumber[pos] == Constants.standardDelimeter[0])
							currentNumber = currentNumber.Remove(pos, 1);
						else
							break;
					}
				}
			}

			if (currentNumber.IndexOf(Constants.complexDelimeter) == -1)
				currentNumber += Constants.formulaSymbols[1] + Constants.complexDelimeter + Constants.zero;
		}
	}
}
