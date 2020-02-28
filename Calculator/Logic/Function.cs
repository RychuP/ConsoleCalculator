using System;
using System.Collections.Generic;

namespace Calculator
{
    class Function : Expression
    {
        public static readonly Dictionary<string, char> Types = new Dictionary<string, char>()
        {
            {"sin", Symbol.Sin},
            {"cos", Symbol.Cos}
        };

        public Function(char functionType, Parser input) : base(input)
        {
            switch (functionType)
            {
                case Symbol.Sin:
                    // solve the floating point inaccuracy
                    if (base.Value == 180) Value = 0;
                    else Value = Math.Sin(base.Value * (Math.PI / 180));
                    break;

                case Symbol.Cos:
                    Value = Math.Cos(base.Value * (Math.PI / 180));
                    break;

                default:
                    throw new Exception("Unknown function symbol in the constructor");
            }
        }
    }
}
