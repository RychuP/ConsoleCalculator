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
    }
}
