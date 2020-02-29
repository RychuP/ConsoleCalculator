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
            {"asin", x => Settings.AnglesInRadians ? Math.Asin(x) : Math.Asin(x) / exchangeRate},
            {"acos", x => Settings.AnglesInRadians ? Math.Acos(x) : Math.Acos(x) / exchangeRate},
            {"atan", x => Settings.AnglesInRadians ? Math.Atan(x) : Math.Atan(x) / exchangeRate},

            {"csc", x => 1 / Math.Sin(x)},
            {"sec", x => 1 / Math.Cos(x)},
            {"cot", x => 1 / Math.Tan(x)},
            {"ctg", x => 1 / Math.Tan(x)},

            {"sin", x => Math.Sin(x)},
            {"cos", x => Math.Cos(x)},
            {"tan", x => Math.Tan(x)},
            { "tg", x => Math.Tan(x)},

            {"log", x => Math.Log(x)}
        };

        /*
            double dtr = Math.PI / 180;
            double rtd = 180 / Math.PI;
            double angleInDegrees = 100;
            double angle = angleInDegrees * dtr;
            double x = Math.Sin(angle);
            double angleofx = Math.Asin(x);
            string name = "sin";
            Console.WriteLine(
                $"angle = angleInDegrees\n" +
                $"angle in radians = {angle}\n\n" +
                $"{name} of angle = {x}\n\n" +
                $"a{name} of {name} in radians = {angleofx}\n" +
                $"a{name} of {name} = {angleofx * rtd}");
            Console.ReadKey();
            return;
        */

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
