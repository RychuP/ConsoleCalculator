using System;
using System.Collections.Generic;

namespace Calculator
{
    // holds raw values from the input string (namely ints and decimals)
    class Constant : Operand
    {
        // used in parsing
        public const char Symbol = '$';

        // new constant needs entry here, entry in symbol class and implementation in parser
        public static readonly Dictionary<string, double> Predefined = new Dictionary<string, double>()
        {
            {"pi", Math.PI}
        };

        public Constant(double value) : base()
        {
            Value = value;
        }

        public Constant(string name) : base()
        {
            if (Predefined.ContainsKey(name))
            {
                Value = Predefined[name];
            }
            else
            {
                throw new Exception("Call to new constant with a wrong name of a predefined number");
            }
        }
    }
}
