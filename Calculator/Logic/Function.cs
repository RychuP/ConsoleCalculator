using System;
using System.Collections.Generic;

namespace Calculator
{
    class Function : Expression
    {
        // used in parsing
        public const char Symbol = 'ƒ';

        // degrees to radians
        const double exchangeRate = Math.PI / 180;

        // available trigonometric functions
        public static readonly Dictionary<string, Func<double, double>> Types = 
            new Dictionary<string, Func<double, double>>()
        { 
            // any function names that in the name contain other function names, have to go first (parsing)
            {"asin", x => Settings.AnglesInRadians ? Round(Math.Asin(x)) : Round(Math.Asin(x) / exchangeRate)},
            {"acos", x => Settings.AnglesInRadians ? Round(Math.Acos(x)) : Round(Math.Acos(x) / exchangeRate)},
            {"atan", x => Settings.AnglesInRadians ? Round(Math.Atan(x)) : Round(Math.Atan(x) / exchangeRate)},

            {"csc", x => 1 / Round(Math.Sin(x))},
            {"sec", x => 1 / Round(Math.Cos(x))},
            {"cot", x => 1 / Round(Math.Tan(x))},
            {"ctg", x => 1 / Round(Math.Tan(x))},

            {"sin", x => Round(Math.Sin(x))},
            {"cos", x => Round(Math.Cos(x))},
            {"tan", x => Round(Math.Tan(x))},
            { "tg", x => Round(Math.Tan(x))},

            {"log10", x => Math.Log10(x)},
            {"log7", x => Math.Log(x, 7)},
            {"log5", x => Math.Log(x, 5)},
            {"log3", x => Math.Log(x, 3)},
            {"log2", x => Math.Log(x, 2)},
            {"log", x => Math.Log(x)}
        };

        static double Round(double x)
        {
            int roundingPoint = 15;
            return Math.Round(x, roundingPoint);
        }

        public Function(string name, Parser parser) : base(parser)
        {
            if (Types.ContainsKey(name))
            {
                double input;

                // most functions work on current value without transformation
                if (name[0] == 'a' || name[0] == 'l')
                {
                    input = base.Value;
                }

                // change radians to degrees if needed for functions that need angle as input
                else
                {
                    input = Settings.AnglesInRadians ? base.Value : base.Value * exchangeRate;
                }

                Value = Types[name](input);
            }
            else
            {
                throw new Exception("Invalid function Types index");
            }
        }
    }
}
