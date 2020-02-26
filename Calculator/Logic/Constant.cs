using System.Collections.Generic;

namespace Calculator
{
    class Constant : Operand
    {
        double myValue;

        public static readonly Dictionary<string, string> Predefined = new Dictionary<string, string>()
        {
            {"pi", "π"}
        };

        public Constant(double value) : base()
        {
            Value = value;
        }

        // constant holds its own value
        public override double Value
        {
            get
            {
                if (IsPositive) return myValue;
                else return -myValue;
            }

            set
            {
                myValue = value;
            }
        }
    }
}
