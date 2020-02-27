﻿using System;

namespace Calculator
{
    // single letter variable that holds results of valid user inputs
    class Variable
    {
        /* -------------- Fields -------------- */
        char name;
        bool removable;


        /* ------------ Constructor ----------- */
        public Variable(char name, bool removable = true)
        {
            this.name = name;
            this.removable = removable;
            Comment = "";
            Value = 0;
        }


        /* ------------ Properties ------------ */
        public string LastOperation { get; set; }
        public string Comment { get; set; }
        public double Value { get; set; }
        public bool IsRemovable
        {
            get
            {
                return removable;
            }
        }
        public char Name
        {
            get
            {
                return name;
            }
        }


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
                if (decimalDigitCount > 2)
                {
                    valueToDisplay = String.Format("{0:0.00}", Value) + "..";
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
            Console.WriteLine("{0} {1} {2} [{3}]\n", name, equalitySign, valueToDisplay, LastOperation);
        }
    }
}
