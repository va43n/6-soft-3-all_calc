using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _6_soft_3_all_calc
{
	public enum Operation { None, Addition, Subtraction, Multiplication, Division, Equal }

	public enum Function { Square, SquareRoot, Reverse }

	public enum CalculationMode { Standard, Alternative }

	public abstract class Number
	{
		public abstract void Addition(Number number);
		public abstract void Subtraction(Number number);
		public abstract void Multiplication(Number number);
		public abstract void Division(Number number);
		public abstract void Square();
		public abstract void Reverse();
		public abstract override string ToString();
		public abstract void ClearNumber();
		public abstract void Copy(Number number);
		public abstract void ChangeCalculationMode(CalculationMode newCalculationMode);
	}

	public class PNumber : Number
	{
		public static string alphabet = "0123456789ABCDEF";

		private double number;
		private int p;
		private int c;
		private CalculationMode mode;

		public PNumber()
		{
			number = 0;
			p = 2;
			c = 0;
			//Standard - real numbers
			//Alternative - only integers
			mode = CalculationMode.Standard;
		}

		public PNumber(string n0, int p0)
		{
			int delimeterPosition, sign;

			if (n0[0] == Constants.stringSign[0])
			{
				sign = -1;
				n0 = n0.Remove(0, 1);
			}
			else
				sign = 1;

			number = ConvertTo10(n0, p0) * sign;
			p = p0;

			delimeterPosition = n0.IndexOf(Constants.standardDelimeter);
			if (delimeterPosition == -1)
				delimeterPosition = n0.IndexOf(Constants.differentDelimeter);

			c = delimeterPosition != -1 ? n0.Length - delimeterPosition - 1 : 0;
			c = (int)Math.Floor(c * Math.Log(10) / Math.Log(p) + 0.5);

			mode = CalculationMode.Standard;
		}

		private double ConvertTo10(string n0, int p0)
		{
			int weightPower, delimeterPosition = n0.IndexOf(Constants.standardDelimeter);
			string numberWithoutDelimeter = n0;
			double result = 0;

			if (delimeterPosition != -1)
			{
				weightPower = delimeterPosition - 1;
				numberWithoutDelimeter = numberWithoutDelimeter.Remove(delimeterPosition, 1);
			}
			else
				weightPower = n0.Length - 1;

			try
			{
				weightPower = Convert.ToInt32(Math.Pow(p0, weightPower));

				for (int i = 0; i < numberWithoutDelimeter.Length; i++)
					result += weightPower * alphabet.IndexOf(numberWithoutDelimeter[i]) / Math.Pow(p0, i);
			}
			catch (OverflowException)
			{
				throw new CalculatorException("Вы ввели слишком большое число.");
			}

			return result;
		}

		public override string ToString()
		{
			string result;
			double absNumber = Math.Abs(number);

			try
			{
				if (number == Math.Floor(number))
					result = TenToInt(Convert.ToInt32(absNumber));
				else
					result = TenToInt(Convert.ToInt32(Math.Floor(absNumber))) + Constants.standardDelimeter + TenToDouble(absNumber - Math.Floor(absNumber));
			}
			catch (OverflowException)
			{
				throw new CalculatorException("Вы ввели слишком большое число.");
			}

			result = number >= 0 ? result : Constants.stringSign + result;

			return result;
		}

		private string TenToInt(int number)
		{
			string s = "", temp;

			if (number == 0)
				return Constants.zero;

			while (number >= 1)
			{
				temp = alphabet[number % p].ToString();

				s = s.Insert(0, temp);
				number /= p;
			}

			return s;
		}

		private string TenToDouble(double number)
		{
			string s = "";

			for (int i = 0; i < c && number != 0.0; i++)
			{
				number *= p;
				s += alphabet[Convert.ToInt32(Math.Floor(number))];
				number -= Math.Floor(number);
			}

			return s;
		}

		public override void Addition(Number num2)
		{
			PNumber number2 = (PNumber)num2;

			string strResult;
			int accuracy;

			if (this.p != number2.p)
				this.p = number2.p;

			this.number += number2.number;

			if (this.c > number2.c && this.c <= 15)
				this.number = Math.Round(this.number, this.c);
			else if (number2.c >= this.c && number2.c <= 15)
			{
				this.c = number2.c;
				this.number = Math.Round(this.number, this.c);
			}
			else
				this.c = 16;

			strResult = Math.Abs(this.number).ToString();
			accuracy = strResult.IndexOf(Constants.standardDelimeter);
			if (accuracy == -1)
				accuracy = strResult.IndexOf(Constants.differentDelimeter);
			accuracy = accuracy != -1 ? strResult.Length - accuracy - 1 : 0;

			this.c = (int)Math.Floor(accuracy * Math.Log(10) / Math.Log(p) + 0.5);
		}

		public override void Subtraction(Number num2)
		{
			PNumber number2 = (PNumber)num2;

			string strResult;
			int accuracy;

			if (this.p != number2.p)
				this.p = number2.p;

			this.number -= number2.number;

			if (this.c > number2.c && this.c <= 15)
				this.number = Math.Round(this.number, this.c);
			else if (number2.c >= this.c && number2.c <= 15)
			{
				this.c = number2.c;
				this.number = Math.Round(this.number, this.c);
			}
			else
				this.c = 16;

			strResult = Math.Abs(this.number).ToString();
			accuracy = strResult.IndexOf(Constants.standardDelimeter);
			if (accuracy == -1)
				accuracy = strResult.IndexOf(Constants.differentDelimeter);
			accuracy = accuracy != -1 ? strResult.Length - accuracy - 1 : 0;

			this.c = (int)Math.Floor(accuracy * Math.Log(10) / Math.Log(p) + 0.5);
		}

		public override void Multiplication(Number num2)
		{
			PNumber number2 = (PNumber)num2;

			string strResult;
			int accuracy;

			if (this.p != number2.p)
				this.p = number2.p;

			this.number *= number2.number;
			this.c = this.c + number2.c;

			if (this.c < 16)
				this.number = Math.Round(this.number, this.c);
			else this.c = 16;

			strResult = Math.Abs(this.number).ToString();
			accuracy = strResult.IndexOf(Constants.standardDelimeter);
			if (accuracy == -1)
				accuracy = strResult.IndexOf(Constants.differentDelimeter);
			accuracy = accuracy != -1 ? strResult.Length - accuracy - 1 : 0;

			this.c = (int)Math.Floor(accuracy * Math.Log(10) / Math.Log(p) + 0.5);
		}

		public override void Division(Number num2)
		{
			PNumber number2 = (PNumber)num2;

			string strResult;
			int accuracy;

			if (this.p != number2.p)
				this.p = number2.p;

			if (number2.number == 0)
				throw new CalculatorException("Деление на 0 запрещено.");

			this.number = mode == CalculationMode.Standard ? this.number / number2.number : Math.Floor(this.number / number2.number);

			strResult = Math.Abs(this.number).ToString();
			accuracy = strResult.IndexOf(Constants.standardDelimeter);
			if (accuracy == -1)
				accuracy = strResult.IndexOf(Constants.differentDelimeter);
			accuracy = accuracy != -1 ? strResult.Length - accuracy - 1 : 0;

			this.c = (int)Math.Floor(accuracy * Math.Log(10) / Math.Log(p) + 0.5);
		}

		public override void Square()
		{
			int accuracy;
			string strResult;

			number *= number;

			strResult = Math.Abs(number).ToString();
			accuracy = strResult.IndexOf(Constants.standardDelimeter);
			if (accuracy == -1)
				accuracy = strResult.IndexOf(Constants.differentDelimeter);
			accuracy = accuracy != -1 ? strResult.Length - accuracy - 1 : 0;

			this.c = (int)Math.Floor(accuracy * Math.Log(10) / Math.Log(p) + 0.5);
		}

		public override void Reverse()
		{
			int accuracy;
			string strResult;

			if (number == 0)
				throw new CalculatorException("Деление на 0 запрещено.");

			number = mode == CalculationMode.Standard ? 1 / number : Math.Floor(1 / number);

			strResult = Math.Abs(number).ToString();

			accuracy = strResult.IndexOf(Constants.standardDelimeter);
			if (accuracy == -1)
				accuracy = strResult.IndexOf(Constants.differentDelimeter);
			accuracy = accuracy != -1 ? strResult.Length - accuracy - 1 : 0;

			this.c = (int)Math.Floor(accuracy * Math.Log(10) / Math.Log(p) + 0.5);
		}

		public void SquareRoot()
		{
			int accuracy;
			string strResult;

			if (number < 0)
				throw new CalculatorException("Отрицательное число под корнем недопустимо.");

			number = mode == CalculationMode.Standard ? Math.Sqrt(number) : Math.Floor(Math.Sqrt(number));

			strResult = Math.Abs(number).ToString();

			accuracy = strResult.IndexOf(Constants.standardDelimeter);
			if (accuracy == -1)
				accuracy = strResult.IndexOf(Constants.differentDelimeter);
			accuracy = accuracy != -1 ? strResult.Length - accuracy - 1 : 0;

			this.c = (int)Math.Floor(accuracy * Math.Log(10) / Math.Log(p) + 0.5);
		}

		public void ChangeP(int newP)
		{
			int accuracy;
			string strResult = number.ToString();

			accuracy = strResult.IndexOf(Constants.standardDelimeter);
			if (accuracy == -1)
				accuracy = strResult.IndexOf(Constants.differentDelimeter);
			accuracy = accuracy != -1 ? strResult.Length - accuracy - 1 : 0;

			p = newP;
			c = (int)Math.Floor(accuracy * Math.Log(10) / Math.Log(p) + 0.5);
		}

		public override void ClearNumber()
		{
			c = 0;
			number = 0;
		}

		public override void Copy(Number num)
		{
			PNumber number = (PNumber)num;

			this.number = number.number;
			this.p = number.p;
			this.c = number.c;
			this.mode = number.mode;
		}

		public void CreatePI()
		{
			string strResult;
			int delimeterPosition;

			number = Math.PI;
			if (mode == CalculationMode.Alternative)
				number = Math.Floor(number);

			strResult = number.ToString();
			delimeterPosition = strResult.IndexOf(Constants.standardDelimeter);
			if (delimeterPosition == -1)
				delimeterPosition = strResult.IndexOf(Constants.differentDelimeter);

			c = delimeterPosition != -1 ? strResult.Length - delimeterPosition - 1 : 0;
		}

		public int GetP()
		{
			return p;
		}

		public override void ChangeCalculationMode(CalculationMode newMode)
		{
			int delimeterPosition;
			string strResult = number.ToString();

			mode = newMode;
			if (mode == CalculationMode.Alternative)
			{
				delimeterPosition = strResult.IndexOf(Constants.standardDelimeter);
				if (delimeterPosition == -1)
					delimeterPosition = strResult.IndexOf(Constants.differentDelimeter);

				if (delimeterPosition != -1)
					number = Convert.ToDouble(strResult.Substring(0, delimeterPosition));

				c = 0;
			}
		}
	}

	public class Fraction : Number
	{
		public static string alphabet = "0123456789";

		private long numerator, denominator;
		private CalculationMode mode;
		
		public Fraction()
		{
			numerator = 0;
			denominator = 1;
			//Standard - only fraction
			//Alternative - may be non fraction number if denom. = 1
			mode = CalculationMode.Standard;
		}

		public Fraction(string number)
		{
			int delimeterPosition;
			string stringNumerator, stringDenominator;

			try
			{
				delimeterPosition = number.IndexOf(Constants.fractionDelimeter);

				stringNumerator = number.Substring(0, delimeterPosition);
				stringDenominator = number.Substring(delimeterPosition + 1);

				numerator = Convert.ToInt64(stringNumerator);
				denominator = Convert.ToInt64(stringDenominator);
			}
			catch
			{
				throw new CalculatorException("Дробь введена некорректно.");
			}

			if (denominator == 0)
				throw new CalculatorException("В знаменателе не может быть 0.");

			Reduce();
		}

		private void Reduce()
		{
			long sign;
			long gcd;

			sign = 1;
			if (numerator < 0) sign *= -1;
			if (denominator < 0) sign *= -1;

			numerator = Math.Abs(numerator);
			denominator = Math.Abs(denominator);

			if (numerator != 0)
			{
				gcd = GCD(numerator, denominator);

				numerator /= gcd;
				denominator /= gcd;

				numerator *= sign;
			}
		}

		private long GCD(long m, long n)
		{
			m = Math.Abs(m);
			n = Math.Abs(n);

			while (m != n)
			{
				if (m > n)
					m -= n;
				else
					n -= m;
			}

			return n;
		}

		public override void Copy(Number num)
		{
			Fraction number = (Fraction)num;

			this.numerator = number.numerator;
			this.denominator = number.denominator;
		}

		public override string ToString()
		{
			Reduce();

			if (mode == CalculationMode.Alternative && denominator == 1)
				return string.Format("{0}", numerator);

			return string.Format("{0}\\{1}", numerator, denominator);
		}

		public override void Addition(Number num2)
		{
			Fraction number2 = (Fraction)num2;

			try
			{
				this.numerator = (this.numerator * number2.denominator + number2.numerator * this.denominator);
				this.denominator = this.denominator * number2.denominator;
			}
			catch
			{
				throw new CalculatorException("Не удалось сложить две дроби.");
			}

			Reduce();
		}

		public override void Subtraction(Number num2)
		{
			Fraction number2 = (Fraction)num2;

			try
			{
				this.numerator = (this.numerator * number2.denominator - number2.numerator * this.denominator);
				this.denominator = this.denominator * number2.denominator;
			}
			catch
			{
				throw new CalculatorException("Не удалось найти разность дробей.");
			}
			
			Reduce();
		}

		public override void Multiplication(Number num2)
		{
			Fraction number2 = (Fraction)num2;

			try
			{
				this.numerator = this.numerator * number2.numerator;
				this.denominator = this.denominator * number2.denominator;
			}
			catch
			{
				throw new CalculatorException("Не удалось перемножить две дроби.");
			}

			Reduce();
		}

		public override void Division(Number num2)
		{
			Fraction number2 = (Fraction)num2;

			try
			{
				this.numerator = this.numerator * number2.denominator;
				this.denominator = this.denominator * number2.numerator;
			}
			catch
			{
				throw new CalculatorException("Не удалось найти частное двух дробей.");
			}
			
			Reduce();
		}

		public override void Square()
		{
			try
			{
				numerator *= numerator;
				denominator *= denominator;
			}
			catch
			{
				throw new CalculatorException("Не удалось возвести дробь в квадрат.");
			}
		}

		public override void Reverse()
		{
			if (numerator == 0)
				throw new CalculatorException("Нельзя найти дробь, обратную нулевой.");

			long sign = 1, num = numerator;

			if (numerator < 0) sign *= -1;
			if (denominator < 0) sign *= -1;

			numerator = Math.Abs(denominator) * sign;
			denominator = Math.Abs(num);
		}

		public override void ClearNumber()
		{
			numerator = 0;
			denominator = 1;
		}

		public void SetNumerator(long number)
		{
			numerator = number;
		}

		public void SetDenominator(long number)
		{
			if (number == 0)
				throw new CalculatorException("В знаменателе не может быть 0.");

			denominator = number;

			Reduce();
		}

		public void SetHalf()
		{
			numerator = 1;
			denominator = 2;
		}

		public void SetThird()
		{
			numerator = 1;
			denominator = 3;
		}

		public void SetQuarter()
		{
			numerator = 1;
			denominator = 4;
		}

		public override void ChangeCalculationMode(CalculationMode newMode)
		{
			mode = newMode;
		}
	}

	public class Complex : Number
	{
		public static string alphabet = "0123456789";

		private double re, im;
		private int reAccuracy, imAccuracy;
		private CalculationMode mode;

		public Complex()
		{
			re = 0;
			im = 0;
			reAccuracy = 0;
			imAccuracy = 0;
			//Standard - only complex
			//Alternative - may be real if im = 0
			mode = CalculationMode.Standard;
		}

		public Complex(string number)
		{
			int iPosition;
			string stringRe, stringIm;

			iPosition = number.IndexOf(Constants.differentDelimeter);
			if (iPosition != -1)
			{
				number = number.Replace(Constants.differentDelimeter[0], Constants.standardDelimeter[0]);

				iPosition = number.IndexOf(Constants.differentDelimeter);
				if (iPosition != -1)
					number = number.Replace(Constants.differentDelimeter[0], Constants.standardDelimeter[0]);
			}

			try
			{
				iPosition = number.IndexOf(Constants.complexDelimeter);

				if (iPosition == -1)
				{
					stringRe = number;
					re = double.Parse(stringRe, System.Globalization.CultureInfo.InvariantCulture);
				}
				else
				{
					if (iPosition == 0)
					{
						stringIm = number.Substring(1);
						im = double.Parse(stringIm, System.Globalization.CultureInfo.InvariantCulture);
					}
					else
					{
						stringRe = number.Substring(0, iPosition - 1);
						
						stringIm = number.Substring(iPosition - 1);
						stringIm = stringIm.Substring(0, 1) + stringIm.Substring(2);

						re = double.Parse(stringRe, System.Globalization.CultureInfo.InvariantCulture);
						im = double.Parse(stringIm, System.Globalization.CultureInfo.InvariantCulture);
					}
				}

				reAccuracy = CalculateAccuracy(re);
				imAccuracy = CalculateAccuracy(im);
			}
			catch
			{
				throw new CalculatorException("Комплексное число введено некорректно.");
			}
		}

		private int CalculateAccuracy(double number)
		{
			int delimeterPosition;
			string strNumber = number.ToString();

			delimeterPosition = strNumber.IndexOf(Constants.standardDelimeter);
			if (delimeterPosition == -1)
				delimeterPosition = strNumber.IndexOf(Constants.differentDelimeter);

			if (delimeterPosition != -1)
				return strNumber.Length - 1 - delimeterPosition;

			return 0;
		}

		public override string ToString()
		{
			if (im == 0 && mode == CalculationMode.Alternative)
				return string.Format("{0}", re);

			if (im >= 0)
				return string.Format("{0}+i{1}", re, im);

			return string.Format("{0}-i{1}", re, Math.Abs(im));
		}

		public override void Copy(Number num)
		{
			Complex number = (Complex)num;

			re = number.re;
			im = number.im;
			reAccuracy = number.reAccuracy;
			imAccuracy = number.imAccuracy;
		}

		public override void Addition(Number num2)
		{
			Complex number2 = (Complex)num2;

			try
			{
				this.re += number2.re;
				if (this.reAccuracy >= number2.reAccuracy)
					this.re = Math.Round(this.re, this.reAccuracy);
				else
				{
					this.reAccuracy = number2.reAccuracy;
					this.re = Math.Round(this.re, this.reAccuracy);
				}

				this.im += number2.im;
				if (this.imAccuracy >= number2.imAccuracy)
					this.im = Math.Round(this.im, this.imAccuracy);
				else
				{
					this.imAccuracy = number2.imAccuracy;
					this.im = Math.Round(this.im, this.imAccuracy);
				}
			}
			catch
			{
				throw new CalculatorException("Не удалось сложить два комплексных числа.");
			}
		}

		public override void Subtraction(Number num2)
		{
			Complex number2 = (Complex)num2;

			try
			{
				this.re -= number2.re;
				if (this.reAccuracy >= number2.reAccuracy)
					this.re = Math.Round(this.re, this.reAccuracy);
				else
				{
					this.reAccuracy = number2.reAccuracy;
					this.re = Math.Round(this.re, this.reAccuracy);
				}

				this.im -= number2.im;
				if (this.imAccuracy >= number2.imAccuracy)
					this.im = Math.Round(this.im, this.imAccuracy);
				else
				{
					this.imAccuracy = number2.imAccuracy;
					this.im = Math.Round(this.im, this.imAccuracy);
				}
			}
			catch
			{
				throw new CalculatorException("Не найти разность двух комплексных чисел.");
			}
		}

		public override void Multiplication(Number num2)
		{
			Complex number2 = (Complex)num2;
			double re1 = this.re, re2 = number2.re;
			double im1 = this.im, im2 = number2.im;
			int firstAccuracy, secondAccuracy, newReAccuracy, newImAccuracy;

			try
			{
				this.re = re1 * re2 - im1 * im2;
				firstAccuracy = this.reAccuracy + number2.reAccuracy;
				if (firstAccuracy >= 16)
					firstAccuracy = 15;
				secondAccuracy = this.imAccuracy + number2.imAccuracy;
				if (secondAccuracy >= 16)
					secondAccuracy = 15;

				if (firstAccuracy >= secondAccuracy)
				{
					newReAccuracy = firstAccuracy;
					this.re = Math.Round(this.re, newReAccuracy);
				}
				else
				{
					newReAccuracy = secondAccuracy;
					this.re = Math.Round(this.re, newReAccuracy);
				}

				this.im = re1 * im2 + re2 * im1;
				firstAccuracy = this.reAccuracy + number2.imAccuracy;
				if (firstAccuracy >= 16)
					firstAccuracy = 15;
				secondAccuracy = number2.reAccuracy + this.imAccuracy;
				if (secondAccuracy >= 16)
					secondAccuracy = 15;

				if (firstAccuracy >= secondAccuracy)
				{
					newImAccuracy = firstAccuracy;
					this.im = Math.Round(this.im, newImAccuracy);
				}
				else
				{
					newImAccuracy = secondAccuracy;
					this.im = Math.Round(this.im, newImAccuracy);
				}

				this.reAccuracy = newReAccuracy;
				this.imAccuracy = newImAccuracy;
			}
			catch
			{
				throw new CalculatorException("Не удалось перемножить два комплексных числа.");
			}
		}

		public override void Division(Number num2)
		{
			Complex number2 = (Complex)num2;
			double re1 = this.re, re2 = number2.re;
			double im1 = this.im, im2 = number2.im;

			if (re2 == 0 && im2 == 0)
				throw new CalculatorException("Деление на 0.");

			try
			{
				this.re = (re1 * re2 + im1 * im2) / (re2 * re2 + im2 * im2);
				this.im = (re2 * im1 - re1 * im2) / (re2 * re2 + im2 * im2);

				this.reAccuracy = CalculateAccuracy(this.re);
				if (this.reAccuracy > 10)
				{
					this.reAccuracy = 10;
					this.re = Math.Round(this.re, this.reAccuracy);
				}

				this.imAccuracy = CalculateAccuracy(im);
				if (this.imAccuracy > 10)
				{
					this.imAccuracy = 10;
					this.im = Math.Round(this.im, this.imAccuracy);
				}
			}
			catch
			{
				throw new CalculatorException("Не удалось найти частное двух комплексных чисел.");
			}
		}

		public override void Square()
		{
			double re1 = re, im1 = im;
			int firstAccuracy, secondAccuracy, newReAccuracy, newImAccuracy;

			try
			{
				re = re1 * re1 - im1 * im1;
				firstAccuracy = reAccuracy + reAccuracy;
				if (firstAccuracy >= 16)
					firstAccuracy = 15;
				secondAccuracy = imAccuracy + imAccuracy;
				if (secondAccuracy >= 16)
					firstAccuracy = 15;

				if (firstAccuracy >= secondAccuracy)
				{
					newReAccuracy = firstAccuracy;
					re = Math.Round(re, newReAccuracy);
				}
				else
				{
					newReAccuracy = secondAccuracy;
					re = Math.Round(re, newReAccuracy);
				}

				im = 2 * re1 * im1;
				firstAccuracy = reAccuracy + imAccuracy;
				if (firstAccuracy >= 16)
					firstAccuracy = 15;

				newImAccuracy = firstAccuracy;
				this.im = Math.Round(this.im, newImAccuracy);

				this.reAccuracy = newReAccuracy;
				this.imAccuracy = newImAccuracy;
			}
			catch
			{
				throw new CalculatorException("Не удалось возвести комплексное число в квадрат.");
			}
		}

		public override void Reverse()
		{
			double re1 = re, im1 = im;

			try
			{
				if (re1 == 0 && im1 == 0)
					throw new CalculatorException("Деление на 0.");

				re = re1 / (re1 * re1 + im1 * im1);
				im = -im1 / (re1 * re1 + im1 * im1);

				reAccuracy = CalculateAccuracy(re);
				if (reAccuracy > 10)
				{
					reAccuracy = 10;
					re = Math.Round(re, reAccuracy);
				}

				imAccuracy = CalculateAccuracy(im);
				if (imAccuracy > 10)
				{
					imAccuracy = 10;
					im = Math.Round(im, imAccuracy);
				}
			}
			catch
			{
				throw new CalculatorException("Не удалось найти обратное к комплексному числу.");
			}
		}

		public double FindModulus()
		{
			double number;

			try
			{
				number = Math.Sqrt(re * re + im * im);
				if (CalculateAccuracy(number) > 10)
					number = Math.Round(number, 10);
			}
			catch
			{
				throw new CalculatorException("Не удалось найти модуль комплексного числа.");
			}

			return number;
		}

		public double FindArgumentInRadians()
		{
			double number;

			if (re == 0 && im == 0)
				return 0.0;

			if (re == 0)
			{
				number = im > 0 ? Math.PI / 2 : -Math.PI / 2;
				number = Math.Round(number, 10);
				return number;
			}

			if (re > 0)
			{
				number = Math.Atan(im / re);
				if (CalculateAccuracy(number) > 10)
					number = Math.Round(number, 10);
				return number;
			}

			if (im < 0)
			{
				number = Math.Atan(im / re) - Math.PI;
				if (CalculateAccuracy(number) > 10)
					number = Math.Round(number, 10);
				return number;
			}

			number = Math.Atan(im / re) + Math.PI;
			if (CalculateAccuracy(number) > 10)
				number = Math.Round(number, 10);
			return number;
		}

		public double FindArgumentInDegrees()
		{
			return Math.Round(FindArgumentInRadians() * 180 / Math.PI, 10);
		}

		public string Power(long n)
		{
			double mod = FindModulus();
			double arg = FindArgumentInRadians();

			try
			{
				mod = Math.Pow(mod, n);
				arg = Math.Round(arg * n / Math.PI, 10);
			}
			catch
			{
				throw new CalculatorException(string.Format("Не удалось возвести комплексное число в {0} степень.", n));
			}

			return string.Format("{0}(cos({1}pi) + i*sin({2}pi))", mod, arg, arg);
		}

		public string Root(long n, long i)
		{
			double arg, mod, constant;

			if (i > n)
				throw new CalculatorException("Индекс i должен находиться в промежутке [1, n-1].");

			try
			{
				mod = FindModulus();
				arg = FindArgumentInRadians();

				mod = Math.Round(Math.Pow(mod, 1.0 / n), 10);
				arg = Math.Round(arg / Math.PI, 10);

				constant = 2 * (i - 1);
			}
			catch
			{
				throw new CalculatorException("Не удалось найти корень комплексного числа.");
			}

			if (constant == 0)
				return string.Format("{0}*(sin({1}pi)/{2}+i*sin({1}pi)/{2})", mod, arg, n);
			return string.Format("{0}*(sin({1}pi+{3}pi)/{2}+i*sin({1}pi+{3}pi)/{2})", mod, arg, n, constant);
		}

		public double Sin()
		{
			return Math.Sin(FindArgumentInRadians());
		}

		public double Cos()
		{
			return Math.Cos(FindArgumentInRadians());
		}

		public double Tan()
		{
			double number = Cos();

			if (Math.Abs(number) < Math.Pow(10, -10))
				throw new CalculatorException("Тангенс угла данного числа не определен.");

			return Math.Tan(FindArgumentInRadians());
		}

		public void SetRe(double re)
		{
			this.re = re;
		}

		public void SetIm(double im)
		{
			this.im = im;
		}

		public override void ClearNumber()
		{
			re = 0;
			im = 0;
		}

		public override void ChangeCalculationMode(CalculationMode newMode)
		{
			mode = newMode;
		}
	}
}
