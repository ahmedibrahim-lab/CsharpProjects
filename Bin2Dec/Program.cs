// See https://aka.ms/new-console-template for more information
using System;
using System.Numerics;

namespace Bin2Dec
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter up to 256 binary digits (0 or 1):");
            string input = Console.ReadLine();

            // Validate the input
            if (string.IsNullOrEmpty(input) || input.Length > 256 || !IsBinary(input))
            {
                Console.WriteLine("Invalid input. Please enter up to 256 binary digits (0 or 1) only.");
                return;
            }

            // Convert binary to decimal
            BigInteger decimalValue = BinaryToDecimal(input);
            Console.WriteLine($"The decimal equivalent of {input} is {decimalValue}");
        }

        static bool IsBinary(string input)
        {
            foreach (char c in input)
            {
                if (c != '0' && c != '1')
                {
                    return false;
                }
            }
            return true;
        }

        static BigInteger BinaryToDecimal(string binary)
        {
            BigInteger decimalValue = 0;
            int length = binary.Length;

            for (int i = 0; i < length; i++)
            {
                // Calculate the decimal value using the position of the binary digit
                if (binary[i] == '1') decimalValue += BigInteger.Pow(2, length - 1 - i);
            }

            return decimalValue;
        }
    }
}


