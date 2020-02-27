using System;
using System.Collections.Generic;

namespace Calculator
{
    class Function : Expression
    {
        public static readonly Dictionary<string, char> Types = new Dictionary<string, char>()
        {
            {"sin", '$'},
            {"cos", '@'}
        };

        public Function(char functionType, Parser input) : base(input)
        {
            switch (functionType)
            {
                case '$':
                    Value = Math.Sin(base.Value * (Math.PI / 180));
                    break;

                case '@':
                    Value = Math.Cos(base.Value * (Math.PI / 180));
                    break;

                default:
                    throw new Exception("Unknown function symbol in the constructor");
            }
        }
    }
}
