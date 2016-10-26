using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Calculator
{
    public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button0_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '0';
		}

		private void Button1_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '1';
		}

		private void Button2_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '2';
		}

		private void Button3_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '3';
		}

		private void Button4_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '4';
		}

		private void Button5_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '5';
		}

		private void Button6_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '6';
		}

		private void Button7_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '7';
		}

		private void Button8_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '8';
		}

		private void Button9_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '9';
		}

		private void ButtonDot_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '.';
		}

		private void ButtonAdd_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '+';
		}

		private void ButtonSub_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '-';
		}

		private void ButtonMult_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '*';
		}

		private void ButtonDiv_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '/';
		}

		private void ButtonMod_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '%';
		}

		private void ButtonExp_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '^';
		}

		private void ButtonEquals_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var exp = PostfixExpression.ParseInfix(expression.Text);
                expression.Text = exp.Evaluate().ToString();
				expression.CaretIndex = expression.Text.Length;
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ButtonDel_Click(object sender, RoutedEventArgs e)
		{
			string txt = expression.Text;
			expression.Text = txt.Remove(txt.Length - 1);
		}

		private void ButtonClear_Click(object sender, RoutedEventArgs e)
		{
			expression.Text = "";
		}

		private void ButtonLeftParen_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += '(';
		}

		private void ButtonRightParen_Click(object sender, RoutedEventArgs e)
		{
			expression.Text += ')';
		}

		private void ExpressionPreviewInput(object sender, TextCompositionEventArgs e)
		{
			// input will not be sent to the TextBox if we say we handled the event
			e.Handled = !IsMathTextOnly(e.Text);
		}

		private void ExpressionPaste(object sender, DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(typeof(string)))
			{
				string input = (string)e.DataObject.GetData(typeof(string));

				// if math text only, return and do not cancel the paste
				if (!IsMathTextOnly(input))
					return;
			}

			// cancel for non-string data and input that isn't all math text
			e.CancelCommand();
		}

		private void Expression_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
				ButtonEquals_Click(sender, e);
		}

		private static readonly Regex IsMathText = new Regex("[0-9+\\-*\\/%\\^\\(\\)\\.]+");
		private static bool IsMathTextOnly(string input)
		{
			return IsMathText.IsMatch(input);
		}
	}
}
