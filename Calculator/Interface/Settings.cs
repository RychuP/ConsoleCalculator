using System;

namespace Calculator
{
    class Settings : WindowSection
    {
        public static bool AnglesInRadians = false;
        public static bool DisplayDecimalDigitsRounded = true;
        public static int DecimalPlacesToShow = 2;

        public Settings() : base("Settings")
        {

        }

        public void Print()
        {
            Console.Clear();
            PrintHeader();
            string angleType = AnglesInRadians ? "RADIANS" : "DEGREES";
            string rounding = DisplayDecimalDigitsRounded ? "ON" : "OFF";
            Console.WriteLine("1. Trigonometric functions take argument in: {0}", angleType);
            Console.WriteLine("2. Decimal places rounded in 'Variables' window: {0}", rounding);
            Console.WriteLine("3. Exit settings");
            Console.WriteLine("\nSelect an option...");
            ConsoleKeyInfo info = Console.ReadKey();
            switch (info.Key)
            {
                case ConsoleKey.D1:
                    AnglesInRadians = !AnglesInRadians;
                    break;

                case ConsoleKey.D2:
                    DisplayDecimalDigitsRounded = !DisplayDecimalDigitsRounded;
                    break;

                case ConsoleKey.D3:
                    return;
            }
            Print();
        }
    }
}
