// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a mathematical expression:");
            string input = Console.ReadLine();

            try
            {
                double result = EvaluateExpression(input);
                Console.WriteLine($"The result is: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static double EvaluateExpression(string expression)
        {
            var tokens = Tokenize(expression);
            var rpn = ConvertToRPN(tokens);
            return EvaluateRPN(rpn);
        }

        static List<string> Tokenize(string expression)
        {
            var tokens = new List<string>();
            var number = "";

            for (int i = 0; i < expression.Length; i++)
            {
                var ch = expression[i];

                if (char.IsDigit(ch) || ch == '.')
                {
                    number += ch;
                }
                else
                {
                    if (!string.IsNullOrEmpty(number))
                    {
                        tokens.Add(number);
                        number = "";
                    }

                    if (IsOperator(ch.ToString()) || ch == '(' || ch == ')')
                    {
                        tokens.Add(ch.ToString());
                    }
                }
            }

            if (!string.IsNullOrEmpty(number))
            {
                tokens.Add(number);
            }

            return tokens;
        }

        static bool IsOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/" || token == "^";
        }

        static int GetPrecedence(string op)
        {
            switch (op)
            {
                case "+": case "-": return 1;
                case "*": case "/": return 2;
                case "^": return 3;
                default: return 0;
            }
        }

        static bool IsLeftAssociative(string op)
        {
            switch (op)
            {
                case "^": return false;
                default: return true;
            }
        }

        static List<string> ConvertToRPN(List<string> tokens)
        {
            var output = new List<string>();
            var operators = new Stack<string>();

            foreach (var token in tokens)
            {
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    output.Add(token);
                }
                else if (IsOperator(token))
                {
                    while (operators.Count > 0 && IsOperator(operators.Peek()))
                    {
                        var op1 = token;
                        var op2 = operators.Peek();

                        if ((IsLeftAssociative(op1) && GetPrecedence(op1) <= GetPrecedence(op2)) ||
                            (!IsLeftAssociative(op1) && GetPrecedence(op1) < GetPrecedence(op2)))
                        {
                            output.Add(operators.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }
                    operators.Push(token);
                }
                else if (token == "(")
                {
                    operators.Push(token);
                }
                else if (token == ")")
                {
                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        output.Add(operators.Pop());
                    }
                    operators.Pop(); // Remove '('
                }
            }

            while (operators.Count > 0)
            {
                output.Add(operators.Pop());
            }

            return output;
        }

        static double EvaluateRPN(List<string> rpn)
        {
            var stack = new Stack<double>();

            foreach (var token in rpn)
            {
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                {
                    stack.Push(number);
                }
                else if (IsOperator(token))
                {
                    var right = stack.Pop();
                    var left = stack.Pop();
                    stack.Push(EvaluateOperator(token, left, right));
                }
            }

            return stack.Pop();
        }

        static double EvaluateOperator(string op, double left, double right)
        {
            switch (op)
            {
                case "+": return left + right;
                case "-": return left - right;
                case "*": return left * right;
                case "/": return left / right;
                case "^": return Math.Pow(left, right);
                default: throw new ArgumentException("Invalid operator");
            }
        }
    }
}
