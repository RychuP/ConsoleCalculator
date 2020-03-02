using System;

namespace Calculator
{
    class Settings : WindowSection
    {
        public static bool AnglesInRadians = false;
        public static bool DisplayDecimalDigitsRounded = false;
        public static bool AutomaticVariableRecalculation = false;
        public static int DecimalPlacesToShow = 2;

        public Settings() : base("Settings")
        {

        }

        public void Print()
        {
            ConsoleKeyInfo info;
            do
            {
                PrintOptions();
                info = Console.ReadKey();
                switch (info.Key)
                {
                    case ConsoleKey.D1:
                        AnglesInRadians = !AnglesInRadians;
                        break;

                    case ConsoleKey.D2:
                        DisplayDecimalDigitsRounded = !DisplayDecimalDigitsRounded;
                        break;

                    case ConsoleKey.D3:
                        AutomaticVariableRecalculation = !AutomaticVariableRecalculation;
                        break;

                    default:
                        return;
                }

            } while (true);
        }

        void PrintOptions()
        {
            Console.Clear();
            PrintHeader();
            string angleTypeStatus = AnglesInRadians ? "RADIANS" : "DEGREES";
            string roundingStatus = DisplayDecimalDigitsRounded ? "ON" : "OFF";
            string recalculationStatus = AutomaticVariableRecalculation ? "ON" : "OFF";
            Console.WriteLine("1. Trigonometric functions take argument in: {0}", angleTypeStatus);
            PrintInGray("This applies to arguments of sin, cos, tan, csc, sec, cot", 
                "and results of asin, acos and atan.");
            Console.WriteLine("2. Decimal places rounded in 'Variables' window: {0}", roundingStatus);
            PrintInGray("With this option turned on, variables will be displayed,",
                "for brevity, rounded to 2 decimal places.",
                "Example: x ≈ 2.35, instead of x = 2.34567325675421.",
                "This will NOT change their underlying value.");
            Console.WriteLine("3. Automatic recalculation of connected variables: {0}", recalculationStatus);
            PrintInGray("Changing value of one variable doesn't by default affect others.",
                "Example: x = 3; y = x * 4 (Value of y is calculated only once).",
                "With this option turned on, value of y will automatically",
                "recalculate every time x changes.");
            Console.WriteLine("\nSelect an option...");
        }

        void PrintInGray(params string[] comments)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            foreach (string c in comments)
            {
                Console.WriteLine("\t" + c);
            }
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
