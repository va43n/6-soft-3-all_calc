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

			buttonClearHistory.Click += ButtonClearHistory_Click;
			DisableClearHistoryButton();

			buttonChangeCurrentCalculationMode.Click += ButtonChangeCurrentCalculationMode_Click;
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
				string result = controller.ButtonClicked(tag, 0);

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

		}

		private void DoComplexCommandInController(int tag)
		{

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
		}

		private void DisableUselessMemoryButtons()
		{
			buttonMC.Enabled = false;
			buttonMR.Enabled = false;
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

		private void ButtonChangeCurrentCalculationMode_Click(object? sender, EventArgs e)
		{
			string result;

			if (controller.GetCalculationMode() == CalculationMode.Standard)
			{
				result = controller.ChangeCalculationMode(CalculationMode.Alternative);

				ClearLabels();
				labelResult.Text = result;

				buttonChangeCurrentCalculationMode.Text = "Поменять на действительные числа";
				labelCurrentCalculationModeValue.Text = "Целые числа";

				buttonDelimeter.Enabled = false;
				buttonReverse.Enabled = false;
			}
			else
			{
				result = controller.ChangeCalculationMode(CalculationMode.Standard);

				ClearLabels();
				labelResult.Text = result;

				buttonChangeCurrentCalculationMode.Text = "Поменять на целые числа";
				labelCurrentCalculationModeValue.Text = "Действительные числа";

				buttonDelimeter.Enabled = true;
				buttonReverse.Enabled = true;
			}
		}
	}
}
