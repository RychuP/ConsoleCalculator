using System;
using System.Collections.Generic;

namespace Calculator
{
    class Function : Operand
    {
        double value;

        public static readonly Dictionary<string, string> Types = new Dictionary<string, string>()
        {
            {"sin", "$"},
            {"cos", "@"},
            {"pi", "π"}
        };

        public Function() : base()
        {

        }

        public override double Value
        {
            get {
                return value;
            }

            set
            {
                this.value = value;
            }
        }
    }
}
