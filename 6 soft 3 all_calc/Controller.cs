using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6_soft_3_all_calc
{
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

		public string ButtonClicked(int tag, int id)
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
					result = editor.PrepareNumberToCalculate();

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

					history.Add(string.Format("{0} ({1}, {2})", editor.GetFormula(), p, calculationMode));
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
