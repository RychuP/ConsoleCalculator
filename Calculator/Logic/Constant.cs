using System.Collections.Generic;

namespace Calculator
{
    class Constant : Operand
    {
        public static readonly Dictionary<string, char> Predefined = new Dictionary<string, char>()
        {
            {"pi", 'π'}
        };

        public Constant(double value) : base()
        {
            Value = value;
        }
    }
}
