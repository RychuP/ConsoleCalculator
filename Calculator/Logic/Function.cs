using System;
using System.Collections.Generic;

namespace Calculator
{
    class Function : Expression
    {
        public static readonly Dictionary<string, char> Types = new Dictionary<string, char>()
        {
            {"sin", Symbol.Sin},
            {"cos", Symbol.Cos},
            {"tan", Symbol.Tan},
            {"tg", Symbol.Tan},

            {"csc", Symbol.Csc},
            {"sec", Symbol.Sec},
            {"cot", Symbol.Cot},
            {"ctg", Symbol.Cot}
        };

        public Function(char functionType, Parser input) : base(input)
        {
            double angleInDegrees = base.Value * (Math.PI / 180);

            switch (functionType)
            {
                case Symbol.Sin:
                    Value = Math.Sin(angleInDegrees);
                    break;

                case Symbol.Cos:
                    Value = Math.Cos(angleInDegrees);
                    break;

                case Symbol.Tan:
                    Value = Math.Tan(angleInDegrees);
                    break;


                case Symbol.Csc:
                    Value = 1 / Math.Sin(angleInDegrees);
                    break;

                case Symbol.Sec:
                    Value = 1 / Math.Cos(angleInDegrees);
                    break;

                case Symbol.Cot:
                    Value = 1 / Math.Tan(angleInDegrees);
                    break;


                default:
                    throw new Exception("Unknown symbol in the function constructor");
            }
        }
    }
}
