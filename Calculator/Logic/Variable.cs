using System;
using System.Collections.Generic;

namespace Calculator
{
    // single letter variable that holds results of valid user inputs
    class Variable
    {
        /* ------------ Constructor ----------- */
        public Variable(char name)
        {
            Name = name;
            Comment = "";
            Value = 0;
        }


        /* ------------ Properties ------------ */
        public string LastOperation { get; set; }
        public string Comment { get; set; }
        public double Value { get; set; }
        public char Name { get; private set; }


        /* ------------- Methods -------------- */
        public void Print()
        {
            // display comment
            if (Comment.Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("# " + Comment);
                Console.ResetColor();
            }

            // format value
            string valueToDisplay;
            bool approximation = false;
            if ((Value % 1) != 0)
            {
                decimal valueAsDecimal = (decimal) Value;
                int decimalDigitCount = BitConverter.GetBytes(decimal.GetBits(valueAsDecimal)[3])[2];
                if (Settings.DisplayDecimalDigitsRounded && decimalDigitCount > 2)
                {
                    string format = "{0:0." + new String('0', Settings.DecimalPlacesToShow) + "}";
                    valueToDisplay = String.Format(format, Value); // + "..";
                    approximation = true;
                }
                else
                {
                    valueToDisplay = Value.ToString();
                }
                    
            }
            else
            {
                valueToDisplay = Value.ToString();
            }

            // display value
            string equalitySign = approximation ? "≈" : "=";
            Console.WriteLine("{0} {1} {2} [{3}]\n", Name, equalitySign, valueToDisplay, LastOperation);
        }
    }
}
