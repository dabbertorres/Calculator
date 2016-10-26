using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
	public class PostfixExpression
	{
		#region Class Instance Definition
		private Queue<Token> evalQueue;

		private PostfixExpression(Queue<Token> tokens)
		{
			evalQueue = tokens;
		}

		public double Evaluate()
		{
			var args = new Stack<double>();

			while(evalQueue.Count != 0)
			{
				Token tok = evalQueue.Dequeue();

				if(tok.type == Token.Type.Number)
					args.Push(double.Parse(tok.value));
				// Operator
				else if(args.Count >= 2)
				{
					double a0 = args.Pop();
					double a1 = args.Pop();

					char op = tok.value[0];
					switch(op)
					{
						case '+':
							args.Push(a1 + a0);
							break;

						case '-':
							args.Push(a1 - a0);
							break;

						case '*':
							args.Push(a1 * a0);
							break;

						case '/':
							args.Push(a1 / a0);
							break;

						case '%':
							args.Push(a1 % a0);
							break;

						case '^':
							args.Push(Math.Pow(a1, a0));
							break;

						default:
							throw new EvaluationException("Unkown operator: " + op);
					}
				}
				else
					throw new EvaluationException("Not enough arguments");
			}

			return args.Pop();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			foreach(var tok in evalQueue)
			{
				sb.AppendFormat("{0} ", tok.value);
			}

			return sb.ToString();
		}
		#endregion

		#region Exceptions
		public class ParsingException : Exception
		{
			public ParsingException() : base()
			{ }

			public ParsingException(string message) : base(message)
			{ }

			public ParsingException(string message, Exception innerException) : base(message, innerException)
			{ }
		}

		public class EvaluationException : Exception
		{
			public EvaluationException() : base()
			{ }

			public EvaluationException(string message) : base(message)
			{ }

			public EvaluationException(string message, Exception innerException) : base(message, innerException)
			{ }
		}
		#endregion

		#region Parsing
		private struct Token
		{
			public enum Type
			{
				Number,
				Operator,
			}

			public Type type;
			public string value;
		}

		public static PostfixExpression ParseInfix(string infix)
		{
			Queue<Token> output = new Queue<Token>();
			Stack<Token> operators = new Stack<Token>();

			int idx = 0;

			try
			{
				bool lastTokenWasOperator = false;

				while(idx < infix.Length)
				{
					char ch = infix[idx];

					switch(ch)
					{
						case ' ':
							// ignore spaces
							break;

						case '+':
						case '-':
							// if true, then ch is a sign for a number
							if(lastTokenWasOperator)
								goto default;
							else
								goto case '^';

						case '*':
						case '/':
						case '%':
						case '^':
							while(operators.Count != 0)
							{
								char op2 = operators.Peek().value[0];

								if(op2 == '(')
								{
									break;
								}
								else if((IsLeftAssoc(ch) && OpLessOrEq(ch, op2)) ||
								   (IsRightAssoc(ch) && OpLess(ch, op2)))
								{
									output.Enqueue(operators.Pop());
								}
							}

							operators.Push(new Token { type = Token.Type.Operator, value = ch + "" });
							lastTokenWasOperator = true;
							break;

						case '(':
							operators.Push(new Token { type = Token.Type.Operator, value = "(" });
							lastTokenWasOperator = false;
							break;

						case ')':
							while(operators.Count != 0 && operators.Peek().value[0] != '(')
								output.Enqueue(operators.Pop());

							if(operators.Count == 0)
								throw new ParsingException("Mismatched parentheses");

							operators.Pop();
							lastTokenWasOperator = false;
							break;

						default:
							string number = "";

							int len = TryExtractNumber(infix, idx, ref number);

							if(len > 0)
								output.Enqueue(new Token { type = Token.Type.Number, value = number });
							else
								throw new ParsingException("Unknown character at: " + (Math.Abs(len) + idx));

							// we increment idx by 1 after the switch already
							idx += len - 1;
							lastTokenWasOperator = false;
							break;
					}

					++idx;
				}

				while(operators.Count != 0)
				{
					if(operators.Peek().value[0] == '(')
						throw new ParsingException("Mismatched parentheses");

					output.Enqueue(operators.Pop());
				}
			}
			catch(ParsingException ex)
			{
				throw new ParsingException("Parsing error starting at index: " + idx, ex);
			}

			return new PostfixExpression(output);
		}
		#endregion

		#region Parsing Utilities
		// assigns value to the found number in str starting at start, and returns the length of the number in str
		// returns a negative if there was parsing error, with the absolute value being the offset from start
		private static int TryExtractNumber(string str, int start, ref string value)
		{
			bool hasDot = false;

			int idx = start;

			// signs are valid, but only as the first character
			// waste of time to check for them every loop iteration
			if(str[idx] == '+' || str[idx] == '-')
				++idx;

			for(; idx < str.Length; ++idx)
			{
				char ch = str[idx];

				if(ch == '.')
				{
					if(!hasDot)
						hasDot = true;
					else
						return start - idx;
				}
				else if(IsDigit(ch))
					continue;
				else
					break;
			}

			value = str.Substring(start, idx - start);
			return value.Length;
		}

		private static bool IsDigit(char ch)
		{
			return '0' <= ch && ch <= '9';
		}

		private static bool IsLeftAssoc(char op)
		{
			return op == '+' || op == '-' || op == '*' || op == '/' || op == '%';
		}

		private static bool IsRightAssoc(char op)
		{
			return op == '^';
		}

		// true if op1 <= op2
		private static bool OpLessOrEq(char op1, char op2)
		{
			switch(op1)
			{
				case '+':
				case '-':
					return true;

				case '*':
				case '/':
				case '%':
					return op2 != '+' && op2 != '-';

				case '^':
					return op2 == '^';

				default:
					throw new ParsingException("'" + op1 + " is not a valid operator");
			}
		}

		// true if op1 < op2
		private static bool OpLess(char op1, char op2)
		{
			switch(op1)
			{
				case '+':
				case '-':
					return op2 != '+' && op2 != '-';

				case '*':
				case '/':
				case '%':
					return op2 == '+' || op2 == '-';

				case '^':
					return op2 != '^';

				default:
					throw new ParsingException("'" + op1 + " is not a valid operator");
			}
		}
		#endregion
	}
}
