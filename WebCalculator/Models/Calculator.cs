using System;
using System.Collections.Generic;
using System.Globalization;

namespace WebCalculator.Models
{
    public class Calculator
    {
        public static double EvaluateExpression(string expression)
        {
            var tokens = Tokenize(expression);
            var rpn = ConvertToRPN(tokens);
            return EvaluateRPN(rpn);
        }

        private static List<string> Tokenize(string expression)
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

        private static bool IsOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/" || token == "^";
        }

        private static int GetPrecedence(string op)
        {
            return op switch
            {
                "+" or "-" => 1,
                "*" or "/" => 2,
                "^" => 3,
                _ => 0,
            };
        }

        private static bool IsLeftAssociative(string op)
        {
            return op switch
            {
                "^" => false,
                _ => true,
            };
        }

        private static List<string> ConvertToRPN(List<string> tokens)
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

        private static double EvaluateRPN(List<string> rpn)
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

        private static double EvaluateOperator(string op, double left, double right)
        {
            return op switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => left / right,
                "^" => Math.Pow(left, right),
                _ => throw new ArgumentException("Invalid operator"),
            };
        }
    }
}