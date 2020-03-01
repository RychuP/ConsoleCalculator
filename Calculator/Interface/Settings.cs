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
                }

            } while (info.Key != ConsoleKey.D4);
        }

        void PrintOptions()
        {
            Console.Clear();
            PrintHeader();
            string angleTypeStatus = AnglesInRadians ? "RADIANS" : "DEGREES";
            string roundingStatus = DisplayDecimalDigitsRounded ? "ON" : "OFF";
            string recalculationStatus = AutomaticVariableRecalculation ? "ON" : "OFF";
            Console.WriteLine("1. Trigonometric functions take argument in: {0}", angleTypeStatus);
            Console.WriteLine("2. Decimal places rounded in 'Variables' window: {0}", roundingStatus);
            Console.WriteLine("3. Automatic recalculation of connected variables: {0}", recalculationStatus);
            Console.WriteLine("4. Exit settings");
            Console.WriteLine("\nSelect an option...");
        }
    }
}
