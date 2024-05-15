namespace _6_soft_3_all_calc
{
	public partial class MainWindow : Form
	{
		List<Button> buttons = new List<Button>();
		Controller controller;

		public MainWindow()
		{
			int number;

			InitializeComponent();

			buttons.AddRange([
				button0, button1, button2,
				button3, button4, button5,
				button6, button7, button8,
				button9, buttonA, buttonB,
				buttonC, buttonD, buttonE,
				buttonF, buttonDelimeter, buttonClear,
				buttonClearEdit, buttonBackSpace, buttonEqual,
				buttonPlus, buttonMinus, buttonMultiply,
				buttonDivision, buttonPI, buttonChangeSign,
				buttonSquare, buttonReverse, buttonSqrt,
				buttonM, buttonMC, buttonMR,
				buttonMS, buttonMPlus,

				button0_2, button1_2, button2_2,
				button3_2, button4_2, button5_2,
				button6_2, button7_2, button8_2,
				button9_2, buttonChangeSign_2, buttonEqual_2,
				buttonPlus_2, buttonMinus_2, buttonMultiply_2,
				buttonDivision_2, buttonDivisionDelimeter_2,
				buttonSquare_2, buttonReverse_2,
				buttonClear_2, buttonClearEdit_2, buttonBackSpace_2,
				buttonHalf_2, buttonThird_2, buttonQuarter_2,
				buttonM_2, buttonMC_2, buttonMR_2,
				buttonMS_2, buttonMPlus_2,

				button0_3, button1_3, button2_3,
				button3_3, button4_3, button5_3,
				button6_3, button7_3, button8_3,
				button9_3, buttonChangeSign_3, buttonEqual_3,
				buttonPlus_3, buttonMinus_3, buttonMultiply_3,
				buttonDivision_3, buttonDelimeter_3, buttonI_3,
				buttonClear_3, buttonClearEdit_3, buttonBackSpace_3,
				buttonModZ_3, buttonArgRad_3, buttonArgDeg_3,
				buttonPower_3, buttonRoot_3,
				buttonSin_3, buttonCos_3, buttonSquare_3, buttonTg_3, buttonReverse_3,
				buttonM_3, buttonMC_3, buttonMR_3,
				buttonMS_3, buttonMPlus_3,
			]);

			for (int i = 0; i < buttons.Count; i++)
				buttons[i].Click += ButtonClick;

			trackBarNotation.Scroll += TrackBarNotation_Scroll;

			number = trackBarNotation.Value;
			labelNotationValue.Text = number.ToString();
			DisableHigherNotationButtons(number);

			ClearLabels();

			DisableUselessMemoryButtons();

			controller = new Controller(number);

			tabControl.Selecting += TabControl_Selecting;
			tabControlChangeTypeOfNumber.Selecting += TabControlChangeTypeOfNumber_Selecting;

			buttonClearHistory.Click += ButtonClearHistory_Click;
			DisableClearHistoryButton();

			buttonChangeCurrentCalculationMode.Click += ButtonChangeCurrentCalculationMode_Click;

			numericUpDownPower.ValueChanged += NumericUpDownPower_ValueChanged;
		}

		private void ButtonClick(object? sender, EventArgs e)
		{
			if (sender == null)
			{
				MessageBox.Show("Ошибка при нажитии на кнопку.");
				return;
			}

			Button button = (Button)sender;
			DoCommandInController(Convert.ToInt32(button.Tag.ToString()));
		}

		private void DoCommandInController(int tag)
		{
			if (tag < 100)
				DoPNumberCommandInController(tag);
			else if (tag < 200)
				DoFractionCommandInController(tag - 100);
			else if (tag < 300)
				DoComplexCommandInController(tag - 200);
		}

		private void DoPNumberCommandInController(int tag)
		{
			try
			{
				string result = controller.PNumberButtonClicked(tag);

				//всё кроме памяти
				if (tag <= 29)
				{
					labelResult.Text = result;

					//"Clear"
					if (tag == 18)
					{
						controller.ClearEdit();
						ClearLabels();
					}

					textBoxFormula.Text = controller.GetFormulaFromEditor();
				}

				//M
				else if (tag == 30)
					MessageBox.Show(result);

				//MC
				else if (tag == 31)
				{
					DisableUselessMemoryButtons();
				}

				//MR
				else if (tag == 32)
				{
					labelResult.Text = result;
				}

				//MS || M+
				else if (tag == 33 || tag == 34)
				{
					if (result == Constants.zero)
						DisableUselessMemoryButtons();
					else
						EnableMemoryButtons();
				}
			}
			catch (CalculatorException e)
			{
				MessageBox.Show(e.Message);
				controller.ClearController();

				ClearLabels();
			}
		}

		private void DoFractionCommandInController(int tag)
		{
			try
			{
				string result = controller.FractionButtonClicked(tag);

				//всё кроме памяти
				if (tag <= 29)
				{
					labelResult.Text = result;

					//"Clear"
					if (tag == 18)
					{
						controller.ClearEdit();
						ClearLabels();
					}

					textBoxFormula.Text = controller.GetFormulaFromEditor();
				}

				//M
				else if (tag == 30)
					MessageBox.Show(result);

				//MC
				else if (tag == 31)
				{
					DisableUselessMemoryButtons();
				}

				//MR
				else if (tag == 32)
				{
					labelResult.Text = result;
				}

				//MS || M+
				else if (tag == 33 || tag == 34)
				{
					if (result == Constants.zero)
						DisableUselessMemoryButtons();
					else
						EnableMemoryButtons();
				}
			}
			catch (CalculatorException e)
			{
				MessageBox.Show(e.Message);
				controller.ClearController();

				ClearLabels();
			}
		}

		private void DoComplexCommandInController(int tag)
		{
			try
			{
				string result = controller.ComplexButtonClicked(tag);

				//всё кроме памяти, x^n, x^(1/n)
				if ((tag != 21 && tag <= 28) || tag == 35)
				{
					//"Clear"
					if (tag == 18)
					{
						controller.ClearEdit();
						ClearLabels();
					}

					if (tag >= 10 && tag <= 15 || tag == 21 || tag == 29)
						textBoxComplexFunctions.Text = result;
					else
					{
						labelResult.Text = result;
						textBoxFormula.Text = controller.GetFormulaFromEditor();
					}
				}

				//M
				else if (tag == 30)
					MessageBox.Show(result);

				//MC
				else if (tag == 31)
				{
					DisableUselessMemoryButtons();
				}

				//MR
				else if (tag == 32)
				{
					labelResult.Text = result;
				}

				//MS || M+
				else if (tag == 33 || tag == 34)
				{
					if (result == Constants.zero)
						DisableUselessMemoryButtons();
					else
						EnableMemoryButtons();
				}

				//x^n
				else if (tag == 21)
				{
					textBoxComplexFunctions.Text = controller.CalculateComplexPower(Convert.ToInt32(numericUpDownPower.Value));
				}

				//x^(1/n)
				else if (tag == 29)
				{
					textBoxComplexFunctions.Text = controller.CalculateComplexRoot(Convert.ToInt32(numericUpDownPower.Value), Convert.ToInt32(numericUpDownIndex.Value));
				}
			}
			catch (CalculatorException e)
			{
				MessageBox.Show(e.Message);
				controller.ClearController();

				ClearLabels();
			}
		}

		private void TrackBarNotation_Scroll(object? sender, EventArgs e)
		{
			int number = trackBarNotation.Value;

			labelNotationValue.Text = number.ToString();
			DisableHigherNotationButtons(number);

			ClearLabels();

			labelResult.Text = controller.ChangeNotation(number);
		}

		private void DisableHigherNotationButtons(int notation)
		{
			for (int i = 2; i < notation; i++)
				buttons[i].Enabled = true;

			for (int i = notation; i < 16; i++)
				buttons[i].Enabled = false;
		}

		private void EnableMemoryButtons()
		{
			buttonMC.Enabled = true;
			buttonMR.Enabled = true;
			buttonMC_2.Enabled = true;
			buttonMR_2.Enabled = true;
			buttonMC_3.Enabled = true;
			buttonMR_3.Enabled = true;
		}

		private void DisableUselessMemoryButtons()
		{
			buttonMC.Enabled = false;
			buttonMR.Enabled = false;
			buttonMC_2.Enabled = false;
			buttonMR_2.Enabled = false;
			buttonMC_3.Enabled = false;
			buttonMR_3.Enabled = false;
		}

		private void ClearLabels()
		{
			labelResult.Text = Constants.zero;
			textBoxFormula.Text = "";
		}

		private void ButtonClearHistory_Click(object? sender, EventArgs e)
		{
			controller.ClearRecordsFromHistory();

			listBoxHistoryRecords.Items.Clear();

			DisableClearHistoryButton();
		}

		private void EnableClearHistoryButton()
		{
			buttonClearHistory.Enabled = true;
			buttonClearHistory.Text = "Очистить историю";
		}

		private void DisableClearHistoryButton()
		{
			buttonClearHistory.Enabled = false;
			buttonClearHistory.Text = "История пуста";
		}

		private void TabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
			List<string> records = new List<string>();

			//history
			if (tabControl.SelectedIndex == 1)
			{
				records.AddRange(controller.GetRecordsFromHistory());
				if (records.Count != 0)
				{
					EnableClearHistoryButton();

					for (int i = records.Count - 1; i >= 0; i--)
						listBoxHistoryRecords.Items.Add(records[i]);
				}
				else
					DisableClearHistoryButton();
			}
			else
				listBoxHistoryRecords.Items.Clear();
		}

		private void TabControlChangeTypeOfNumber_Selecting(object? sender, TabControlCancelEventArgs e)
		{
			ClearLabels();
			DisableUselessMemoryButtons();

			if (tabControlChangeTypeOfNumber.SelectedIndex == 0)
			{
				controller.ChangeTypeOfNumber(TypeOfNumber.PNumber);
				trackBarNotation.Value = 2;
				labelNotationValue.Text = trackBarNotation.Value.ToString();
				DisableHigherNotationButtons(trackBarNotation.Value);
			}
			else if (tabControlChangeTypeOfNumber.SelectedIndex == 1)
			{
				controller.ChangeTypeOfNumber(TypeOfNumber.Fraction);
			}
			else if (tabControlChangeTypeOfNumber.SelectedIndex == 2)
			{
				controller.ChangeTypeOfNumber(TypeOfNumber.Complex);
				textBoxComplexFunctions.Text = "0";
			}
		}

		private void ButtonChangeCurrentCalculationMode_Click(object? sender, EventArgs e)
		{
			string result;

			if (controller.GetCalculationMode() == CalculationMode.Standard)
			{
				result = controller.ChangeCalculationMode(CalculationMode.Alternative);

				ClearLabels();
				labelResult.Text = result;

				buttonChangeCurrentCalculationMode.Text = "Поменять на стандартный режим";
				labelCurrentCalculationModeValue.Text = "Альтернативный режим";

				buttonDelimeter.Enabled = false;
				buttonReverse.Enabled = false;
			}
			else
			{
				result = controller.ChangeCalculationMode(CalculationMode.Standard);

				ClearLabels();
				labelResult.Text = result;

				buttonChangeCurrentCalculationMode.Text = "Поменять на альтернативный режим";
				labelCurrentCalculationModeValue.Text = "Стандартный режим";

				buttonDelimeter.Enabled = true;
				buttonReverse.Enabled = true;
			}
		}

		private void NumericUpDownPower_ValueChanged(object? sender, EventArgs e)
		{
			numericUpDownIndex.Maximum = numericUpDownPower.Value;
		}
	}
}
