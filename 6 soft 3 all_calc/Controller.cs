using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6_soft_3_all_calc
{
	public enum TypeOfNumber { PNumber, Fraction, Complex }

	public class Controller
	{
		private Editor editor;

		private Processor processor;
		private Memory memory;
		private History history;

		private int p;
		private int lastInput;

		private CalculationMode calculationMode;


		public Controller(int p)
		{
			this.p = p;

			editor = new PNumberEditor();

			processor = new Processor();
			memory = new Memory();
			history = new History();

			lastInput = 0;

			calculationMode = CalculationMode.Standard;
		}

		~Controller()
		{
			ClearController();
		}

		public void ChangeTypeOfNumber(TypeOfNumber typeOfNumber)
		{
			if (typeOfNumber == TypeOfNumber.PNumber)
			{
				processor.ChangeTypeOfNumber(typeOfNumber);
				editor = new PNumberEditor();
				memory.ChangeTypeOfNumber(typeOfNumber);
			}
			else if (typeOfNumber == TypeOfNumber.Fraction)
			{
				processor.ChangeTypeOfNumber(typeOfNumber);
				editor = new FractionEditor();
				memory.ChangeTypeOfNumber(typeOfNumber);
			}
			else if (typeOfNumber == TypeOfNumber.Complex)
			{
				processor.ChangeTypeOfNumber(typeOfNumber);
				editor = new ComplexEditor();
				memory.ChangeTypeOfNumber(typeOfNumber);
			}
		}

		public string PNumberButtonClicked(int tag)
		{
			string result = "";

			//всё кроме памяти и PI
			if (tag <= 20 || (tag >= 22 && tag <= 29))
			{
				//tag: 0123456789ABCDEF
				//lastInput: PI || MR
				if (tag <= 15 && (lastInput == 21 || lastInput == 32))
					editor.ChangeCurrentNumber(Constants.zero);

				//tag: 0123456789ABCDEF
				//lastInput: "="
				else if (tag <= 15 && lastInput == 22)
					ClearEdit();

				if (tag <= 15)
					result = editor.AddDigit(tag);
				else if (tag == 16)
					result = editor.AddDelimeter();
				else if (tag == 17)
					result = editor.ChangeSign();
				else if (tag == 18)
					result = editor.ClearInput();
				else if (tag == 19)
					result = editor.ClearEdit();
				else if (tag == 20)
					result = editor.DeleteRightmostSymbol();
				else if (tag >= 22 && tag <= 29)
				{
					result = editor.PrepareNumberToCalculate();
					editor.ClearEdit();
				}

				//"="
				if (tag == 22)
				{
					if (processor.CheckOperation() == Operation.None)
						throw new CalculatorException("Чтобы узнать результат необходимо поставить хотя бы один оператор.");

					//"="
					if (lastInput == 22)
					{
						result = processor.CalculateOperation(Operation.Equal);

						editor.UpdateFormula(Constants.formulaSymbols[tag - 22] + result);
					}

					//"+", "-", "*", "/"
					else if (lastInput >= 23 && lastInput <= 26)
					{
						result = processor.SetSameOperand();
						editor.UpdateFormula(result);

						result = processor.CalculateOperation(Operation.Equal);
						editor.UpdateFormula(Constants.formulaSymbols[tag - 22] + result);
					}

					else
					{
						PNumber number = new PNumber(result, p);
						if (calculationMode == CalculationMode.Alternative)
							number.ChangeCalculationMode(CalculationMode.Alternative);

						processor.SetOperand(number);

						//"^2", "^(-1)", "^(1/2)"
						if (lastInput >= 27 && lastInput <= 29)
							editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
						else
							editor.UpdateFormula(result + Constants.formulaSymbols[tag - 22]);

						result = processor.CalculateOperation(Operation.Equal);

						editor.UpdateFormula(result);
					}

					editor.ChangeCurrentNumber(result);

					history.Add(string.Format("{0} ({1}, {2}, {3})", editor.GetFormula(), "PNumber", p, calculationMode));
				}

				//"+", "-", "*", "/"
				else if (tag >= 23 && tag <= 26)
				{
					//"+", "-", "*", "/"
					if (lastInput >= 23 && lastInput <= 26)
					{
						processor.SetOperation(tag);
						((PNumberEditor)editor).ChangeSignInFormula(Constants.formulaSymbols[tag - 22]);
					}

					//"="
					else if (lastInput == 22)
					{
						processor.SetOperation(tag);
						editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
					}

					else
					{
						PNumber number = new PNumber(result, p);
						if (calculationMode == CalculationMode.Alternative)
							number.ChangeCalculationMode(CalculationMode.Alternative);

						processor.SetOperand(number);

						//"^2", "^(-1)", "^(1/2)" || "="
						if (lastInput >= 27 && lastInput <= 29 || lastInput == 22)
							editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
						else
							editor.UpdateFormula(result + Constants.formulaSymbols[tag - 22]);

						if (tag == 23)
							result = processor.CalculateOperation(Operation.Addition);
						else if (tag == 24)
							result = processor.CalculateOperation(Operation.Subtraction);
						else if (tag == 25)
							result = processor.CalculateOperation(Operation.Multiplication);
						else if (tag == 26)
							result = processor.CalculateOperation(Operation.Division);
					}
				}

				//"^2", "^(-1)", "^(1/2)"
				else if (tag >= 27 && tag <= 29)
				{
					//"^2", "^(-1)", "^(1/2)"
					if (lastInput >= 27 && lastInput <= 29)
					{
						editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
					}

					//"+", "-", "*", "/"
					else if (lastInput >= 23 && lastInput <= 26)
					{
						result = processor.SetSameOperand();

						editor.UpdateFormula(result + Constants.formulaSymbols[tag - 22]);
					}

					else
					{
						PNumber number = new PNumber(result, p);
						if (calculationMode == CalculationMode.Alternative)
							number.ChangeCalculationMode(CalculationMode.Alternative);

						processor.SetOperand(number);


						//"="
						if (lastInput == 22)
						{
							editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
							processor.SetOperation(-1);
						}
						else
							editor.UpdateFormula(result + Constants.formulaSymbols[tag - 22]);
					}

					if (tag == 27)
						result = processor.CalculateFunction(Function.Square);
					else if (tag == 28)
						result = processor.CalculateFunction(Function.Reverse);
					else if (tag == 29)
						result = processor.CalculateFunction(Function.SquareRoot);

					editor.ChangeCurrentNumber(result);
				}
			}

			//PI
			else if (tag == 21)
			{
				PNumber number = new PNumber();
				if (calculationMode == CalculationMode.Alternative)
					number.ChangeCalculationMode(CalculationMode.Alternative);

				number.ChangeP(p);
				number.CreatePI();

				result = number.ToString();
				editor.ChangeCurrentNumber(result);
			}

			//M
			else if (tag == 30)
			{
				result = string.Format("Значение в памяти: {0}", memory.CheckMemoryValue());
			}

			//MC
			else if (tag == 31)
			{
				memory.ClearMemory();
				result = memory.CheckMemoryValue();
			}

			//MR
			else if (tag == 32)
			{
				if (memory.CheckMemoryState() == Memory.MemoryState.Off)
					throw new CalculatorException("В памяти лежит нулевое значение.");

				result = memory.CheckMemoryValue();
				editor.ChangeCurrentNumber(result);
			}

			//MS
			else if (tag == 33)
			{
				PNumber number;

				result = editor.GetCurrentNumber();
				number = new PNumber(result, p);

				memory.SaveValueInMemory(number);
			}

			//M+
			else if (tag == 34)
			{
				PNumber number;

				result = editor.GetCurrentNumber();
				number = new PNumber(result, p);

				result = memory.AddValueToMemory(number);
			}

			lastInput = tag;

			return result;
		}

		public string FractionButtonClicked(int tag)
		{
			string result = "";

			//всё кроме памяти
			if (tag <= 29)
			{
				//tag: 0123456789 || "\"
				//lastInput: 1/2 || 1/3 || 1/4 || MR
				if ((tag <= 9 || tag == 16) && (lastInput == 12 || lastInput == 13 || lastInput == 14 || lastInput == 32))
					editor.ChangeCurrentNumber(Constants.zero);

				//tag: 0123456789 || "\"
				//lastInput: "="
				else if ((tag <= 9 || tag == 16) && lastInput == 22)
					ClearEdit();

				if (tag <= 9)
					result = editor.AddDigit(tag);
				else if (tag == 12)
				{
					Fraction number = new Fraction();

					number.SetHalf();

					result = number.ToString();
					editor.ChangeCurrentNumber(result);
				}
				else if (tag == 13)
				{
					Fraction number = new Fraction();

					number.SetThird();

					result = number.ToString();
					editor.ChangeCurrentNumber(result);
				}
				else if (tag == 14)
				{
					Fraction number = new Fraction();

					number.SetQuarter();

					result = number.ToString();
					editor.ChangeCurrentNumber(result);
				}
				else if (tag == 16)
					result = editor.AddDelimeter();
				else if (tag == 17)
					result = editor.ChangeSign();
				else if (tag == 18)
					result = editor.ClearInput();
				else if (tag == 19)
					result = editor.ClearEdit();
				else if (tag == 20)
					result = editor.DeleteRightmostSymbol();
				else if (tag >= 22 && tag <= 28)
				{
					result = editor.PrepareNumberToCalculate();
					editor.ClearEdit();
				}

				//"="
				if (tag == 22)
				{
					if (processor.CheckOperation() == Operation.None)
						throw new CalculatorException("Чтобы узнать результат необходимо поставить хотя бы один оператор.");

					//"="
					if (lastInput == 22)
					{
						result = processor.CalculateOperation(Operation.Equal);

						editor.UpdateFormula(Constants.formulaSymbols[tag - 22] + result);
					}

					//"+", "-", "*", "/"
					else if (lastInput >= 23 && lastInput <= 26)
					{
						result = processor.SetSameOperand();
						editor.UpdateFormula(result);

						result = processor.CalculateOperation(Operation.Equal);
						editor.UpdateFormula(Constants.formulaSymbols[tag - 22] + result);
					}

					else
					{
						Fraction number = new Fraction(result);
						if (calculationMode == CalculationMode.Alternative)
							number.ChangeCalculationMode(CalculationMode.Alternative);

						processor.SetOperand(number);

						//"^2", "^(-1)"
						if (lastInput >= 27 && lastInput <= 28)
							editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
						else
							editor.UpdateFormula(result + Constants.formulaSymbols[tag - 22]);

						result = processor.CalculateOperation(Operation.Equal);

						editor.UpdateFormula(result);
					}

					editor.ChangeCurrentNumber(result);

					history.Add(string.Format("{0} ({1}, {2})", editor.GetFormula(), "Fraction", calculationMode));
				}

				//"+", "-", "*", "/"
				else if (tag >= 23 && tag <= 26)
				{
					//"+", "-", "*", "/"
					if (lastInput >= 23 && lastInput <= 26)
					{
						processor.SetOperation(tag);
						((FractionEditor)editor).ChangeSignInFormula(Constants.formulaSymbols[tag - 22]);
					}

					//"="
					else if (lastInput == 22)
					{
						processor.SetOperation(tag);
						editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
					}

					else
					{
						Fraction number = new Fraction(result);
						if (calculationMode == CalculationMode.Alternative)
							number.ChangeCalculationMode(CalculationMode.Alternative);

						processor.SetOperand(number);

						//"^2", "^(-1)" || "="
						if (lastInput >= 27 && lastInput <= 28 || lastInput == 22)
							editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
						else
							editor.UpdateFormula(result + Constants.formulaSymbols[tag - 22]);

						if (tag == 23)
							result = processor.CalculateOperation(Operation.Addition);
						else if (tag == 24)
							result = processor.CalculateOperation(Operation.Subtraction);
						else if (tag == 25)
							result = processor.CalculateOperation(Operation.Multiplication);
						else if (tag == 26)
							result = processor.CalculateOperation(Operation.Division);
					}
				}

				//"^2", "^(-1)"
				else if (tag >= 27 && tag <= 28)
				{
					//"+", "-", "*", "/"
					if (lastInput >= 23 && lastInput <= 26)
					{
						result = processor.SetSameOperand();

						editor.UpdateFormula(result + Constants.formulaSymbols[tag - 22]);
					}

					else
					{
						Fraction number = new Fraction(result);
						if (calculationMode == CalculationMode.Alternative)
							number.ChangeCalculationMode(CalculationMode.Alternative);

						processor.SetOperand(number);

						//"="
						if (lastInput == 22)
						{
							editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
							processor.SetOperation(-1);
						}
						else
							editor.UpdateFormula(result + Constants.formulaSymbols[tag - 22]);
					}

					if (tag == 27)
						result = processor.CalculateFunction(Function.Square);
					else if (tag == 28)
						result = processor.CalculateFunction(Function.Reverse);

					editor.ChangeCurrentNumber(result);
				}
			}

			//M
			else if (tag == 30)
			{
				result = string.Format("Значение в памяти: {0}", memory.CheckMemoryValue());
			}

			//MC
			else if (tag == 31)
			{
				memory.ClearMemory();
				result = memory.CheckMemoryValue();
			}

			//MR
			else if (tag == 32)
			{
				if (memory.CheckMemoryState() == Memory.MemoryState.Off)
					throw new CalculatorException("В памяти лежит нулевое значение.");

				result = memory.CheckMemoryValue();
				editor.ChangeCurrentNumber(result);
			}

			//MS
			else if (tag == 33)
			{
				Fraction number;

				result = editor.GetCurrentNumber();
				number = new Fraction(result);

				memory.SaveValueInMemory(number);
			}

			//M+
			else if (tag == 34)
			{
				Fraction number;

				result = editor.GetCurrentNumber();
				number = new Fraction(result);

				result = memory.AddValueToMemory(number);
			}

			lastInput = tag;

			return result;
		}

		public string ComplexButtonClicked(int tag)
		{
			string result = "";

			//всё кроме памяти
			if (tag <= 29 || tag == 35)
			{
				//tag: 0123456789 || "." || "i"
				//lastInput: MR
				if ((tag <= 9 || tag == 16 || tag == 35) && lastInput == 32)
					editor.ChangeCurrentNumber(Constants.zero);

				//tag: 0123456789 || "." || "i"
				//lastInput: "="
				else if ((tag <= 9 || tag == 16 || tag == 35) && lastInput == 22)
					ClearEdit();

				//tag: 0123456789 || "."
				//lastInput: "+" || "-"
				else if ((tag <= 9 || tag == 16 || tag == 35) && lastInput == 22)
					ClearEdit();

				if (tag <= 9)
					result = editor.AddDigit(tag);
				else if (tag >= 10 && tag <= 15 || tag == 21 || tag == 29)
				{
					result = editor.PrepareNumberToCalculate();
					Complex number = new Complex(result);

					if (tag == 10)
						result = number.FindModulus().ToString();
					else if (tag == 11)
						result = number.FindArgumentInDegrees().ToString();
					else if (tag == 12)
						result = number.FindArgumentInRadians().ToString();
					else if (tag == 13)
						result = number.Sin().ToString();
					else if (tag == 14)
						result = number.Cos().ToString();
					else if (tag == 15)
						result = number.Tan().ToString();
				}
				else if (tag == 16)
					result = editor.AddDelimeter();
				else if (tag == 35)
					result = ((ComplexEditor)editor).AddComplexDelimeter();
				else if (tag == 17)
					result = editor.ChangeSign();
				else if (tag == 18)
					result = editor.ClearInput();
				else if (tag == 19)
					result = editor.ClearEdit();
				else if (tag == 20)
					result = editor.DeleteRightmostSymbol();
				else if ((tag == 23 || tag == 24) && editor.GetCurrentNumber().IndexOf(Constants.complexDelimeter) == -1)
				{
					if (tag == 23)
						result = ((ComplexEditor)editor).AddSignAndComplexDelimeter(Constants.formulaSymbols[1]);
					else if (tag == 24)
						result = ((ComplexEditor)editor).AddSignAndComplexDelimeter(Constants.formulaSymbols[2]);
				}
				else if (tag >= 22 && tag <= 28)
				{
					result = editor.PrepareNumberToCalculate();
					editor.ClearEdit();

					//"="
					if (tag == 22)
					{
						if (processor.CheckOperation() == Operation.None)
							throw new CalculatorException("Чтобы узнать результат необходимо поставить хотя бы один оператор.");

						//"="
						if (lastInput == 22)
						{
							result = processor.CalculateOperation(Operation.Equal);

							editor.UpdateFormula(Constants.formulaSymbols[tag - 22] + "(" + result + ")");
						}

						//"+", "-", "*", "/"
						else if (lastInput >= 23 && lastInput <= 26)
						{
							result = processor.SetSameOperand();
							editor.UpdateFormula(result);

							result = processor.CalculateOperation(Operation.Equal);
							editor.UpdateFormula(Constants.formulaSymbols[tag - 22] + "(" + result + ")");
						}

						else
						{
							Complex number = new Complex(result);
							if (calculationMode == CalculationMode.Alternative)
								number.ChangeCalculationMode(CalculationMode.Alternative);

							processor.SetOperand(number);

							//"^2", "^(-1)"
							if (lastInput >= 27 && lastInput <= 28)
								editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
							else
								editor.UpdateFormula("(" + result + ")" + Constants.formulaSymbols[tag - 22]);

							result = processor.CalculateOperation(Operation.Equal);

							editor.UpdateFormula("(" + result + ")");
						}

						editor.ChangeCurrentNumber(result);

						history.Add(string.Format("{0} ({1}, {2})", editor.GetFormula(), "Complex", calculationMode));
					}

					//"+", "-", "*", "/"
					else if (tag >= 23 && tag <= 26)
					{
						//"+", "-", "*", "/"
						if (lastInput >= 23 && lastInput <= 26)
						{
							processor.SetOperation(tag);
							((ComplexEditor)editor).ChangeSignInFormula(Constants.formulaSymbols[tag - 22]);
						}

						//"="
						else if (lastInput == 22)
						{
							processor.SetOperation(tag);
							editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
						}

						else
						{
							Complex number = new Complex(result);
							if (calculationMode == CalculationMode.Alternative)
								number.ChangeCalculationMode(CalculationMode.Alternative);

							processor.SetOperand(number);

							//"^2", "^(-1)" || "="
							if (lastInput >= 27 && lastInput <= 28 || lastInput == 22)
								editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
							else
								editor.UpdateFormula("(" + result + ")" + Constants.formulaSymbols[tag - 22]);

							if (tag == 23)
								result = processor.CalculateOperation(Operation.Addition);
							else if (tag == 24)
								result = processor.CalculateOperation(Operation.Subtraction);
							else if (tag == 25)
								result = processor.CalculateOperation(Operation.Multiplication);
							else if (tag == 26)
								result = processor.CalculateOperation(Operation.Division);
						}
					}

					//"^2", "^(-1)"
					else if (tag >= 27 && tag <= 28)
					{
						//"^2", "^(-1)", +/-
						if (lastInput >= 27 && lastInput <= 28 || lastInput == 17)
						{
							editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
						}

						//"+", "-", "*", "/"
						else if (lastInput >= 23 && lastInput <= 26)
						{
							result = processor.SetSameOperand();

							editor.UpdateFormula("(" + result + ")" + Constants.formulaSymbols[tag - 22]);
						}

						else
						{
							Complex number = new Complex(result);
							if (calculationMode == CalculationMode.Alternative)
								number.ChangeCalculationMode(CalculationMode.Alternative);

							processor.SetOperand(number);

							//"="
							if (lastInput == 22)
							{
								editor.UpdateFormula(Constants.formulaSymbols[tag - 22]);
								processor.SetOperation(-1);
							}
							else
								editor.UpdateFormula("(" + result + ")" + Constants.formulaSymbols[tag - 22]);
						}

						if (tag == 27)
							result = processor.CalculateFunction(Function.Square);
						else if (tag == 28)
							result = processor.CalculateFunction(Function.Reverse);

						editor.ChangeCurrentNumber(result);
					}
				}
			}

			//M
			else if (tag == 30)
			{
				result = string.Format("Значение в памяти: {0}", memory.CheckMemoryValue());
			}

			//MC
			else if (tag == 31)
			{
				memory.ClearMemory();
				result = memory.CheckMemoryValue();
			}

			//MR
			else if (tag == 32)
			{
				if (memory.CheckMemoryState() == Memory.MemoryState.Off)
					throw new CalculatorException("В памяти лежит нулевое значение.");

				result = memory.CheckMemoryValue();
				editor.ChangeCurrentNumber(result);
			}

			//MS
			else if (tag == 33)
			{
				Complex number;

				result = editor.GetCurrentNumber();
				number = new Complex(result);

				memory.SaveValueInMemory(number);
			}

			//M+
			else if (tag == 34)
			{
				Complex number;

				result = editor.GetCurrentNumber();
				number = new Complex(result);

				result = memory.AddValueToMemory(number);
			}

			lastInput = tag;

			return result;
		}

		public string CalculateComplexPower(int n)
		{
			string result = "";

			result = editor.PrepareNumberToCalculate();
			Complex number = new Complex(result);

			result = number.Power(n);

			return result;
		}

		public string CalculateComplexRoot(int n, int i)
		{
			string result = "";

			result = editor.PrepareNumberToCalculate();
			Complex number = new Complex(result);

			result = number.Root(n, i);

			return result;
		}

		public string ChangeNotation(int newP)
		{
			string result = processor.SetNotation(newP);

			p = newP;

			memory.SetNotation(p);

			editor.ClearEditor();
			editor.ChangeCurrentNumber(result);
			if (result != Constants.zero)
				editor.UpdateFormula(result);

			return result;
		}

		public string GetFormulaFromEditor()
		{
			return editor.GetFormula();
		}

		public void ClearController()
		{
			processor.ClearProcessor();
			editor.ClearEditor();
			history.ClearHistory();
		}

		public void ClearEdit()
		{
			processor.ClearProcessor();
			editor.ClearEditor();
		}

		public void ClearRecordsFromHistory()
		{
			history.ClearHistory();
		}

		public List<string> GetRecordsFromHistory()
		{
			return history.GetHistory();
		}

		public string ChangeCalculationMode(CalculationMode newMode)
		{
			string result;

			calculationMode = newMode;

			result = processor.ChangeCalculationModeInOperands(newMode);
			editor.ClearEditor();
			editor.ChangeCurrentNumber(result);
			editor.UpdateFormula(result);

			memory.ChangeCalculationModeInMemory(newMode);

			return result;
		}

		public CalculationMode GetCalculationMode()
		{
			return calculationMode;
		}
	}
}
