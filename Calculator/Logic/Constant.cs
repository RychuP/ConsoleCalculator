using System;

namespace Calculator
{
    class Constant : Operand
    {
        double myValue;

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
