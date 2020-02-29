using System;
using System.Collections.Generic;

namespace Calculator
{
    class Help : WindowSection
    {
        Dictionary<string, string[]> topics = new Dictionary<string, string[]>()
        {
            {"general", new string[] {
                "$Calculator accepts:",
                "- brackets: ()",
                "- decimals: 123.123",
                "- single letter variables: a-z A-Z",
                "- five operators +, -, *, /, ^",
                "- constants: pi",
                "- functions: sin, cos, tan (tg), csc, sec, cot (ctg), asin, acos, atan",
                "",
                "$Commands:",
                "- '# <description>' add comment to current variable (no description: remove comment)",
                "- 'de' delete current variable ('de <letter>' remove specific var)",
                "- 'da' delete all but current variable",
                "- 'fu' full precision (shows current variable with all fractional digits)",
                "- 'se' settings",
                "- 'he' help",
                "- 'hf' help on functions",
                "- 'hv' help on variables",
                "- 'hp' help on priorities of operations",
                "- 'ex' exit application"
            } },

            {"functions", new string[] {
                "$Functions:",
                "  Trigonometric functions accept degrees or radians (change in settings 'se').",
                "  Arguments need to go in brackets, for example: sin(1.5 * pi).",
                "  Use lower case characters for the name of the functions.",
                "  Currently implemented ones are listed below.",
                "",
                "$Types:",
                "- 'sin'  returns sine of the specified number (opposite over hypotenuse)",
                "- 'cos'  returns cosine (adjacent over hypotenuse)",
                "- 'tan'  returns tangent (opposite over adjacent)",
                "- 'csc'  returns cosecant (hypotenuse over opposite)",
                "- 'sec'  returns secant (hypotenuse over adjacent)",
                "- 'cot'  returns cotangent (adjacent over opposite)",
                "- 'asin' returns the angle whose sine is the specified number",
                "- 'acos' returns the angle whose cosine is the specified number",
                "- 'atan' returns the angle whose tangent is the specified number",
                "- 'log'  returns the logarithm of a specified number."
            } },

            {"variables", new string[] {
                "$Variables:",
                "  All results get assigned to the currently selected [in square brackets] variable.",
                "  Type a letter to introduce a new variable or switch to an existing one.",
                "",
                "  [x]> 2 <Enter> -> will assign 2 to the default variable x.",
                "  [x]> y <Enter> -> will create a new variable y and set it as current.",
                "  [y]> x * 3 -> will assign a value of x multiplied by 3 to the variable y (6)",
                "",
                "$Results:",
                "  Decimal numbers are by default displayed with full precision.",
                "  For brevity, in settings 'se', you can turn on displaying variables",
                "  rounded up to 2 decimal places. Calling the variable with its letter or",
                "  command 'fu' (for full precision) will show all stored fractional digits."
            } },

            {"priorities", new string[] {
                "$Priorities:",
                "  Expressions evaluate from left to right with following priorities:",
                "",
                "1. functions",
                "2. powers",
                "3. multiplication and division",
                "4. addition and substraction",
                "",
                "$Examples:",
                "  sin(20 + x) ^ (2 / 3) -> will evaluate 20 + x first, than sine of that, than 2 / 3",
                "                           and finally, power. Brackets force priorities.",
                "",
                "  4 / cos(45) ^ 2 * y   -> cosine of 45 degrees first, raised to power of 2 second,",
                "                           4 devided by the result of that third and finally,",
                "                           multiplication by value of y.",
                "",
                "$Tip:",
                "  For absolute certainty over priorities, remember to use brackets."
            } }
        };

        public Help() : base("Help")
        {

        }

        public void Print(string title = "general")
        {
            if (topics.ContainsKey(title))
            {
                Console.Clear();
                PrintHeader();

                foreach (string line in topics[title])
                {
                    if (line.Length > 0 && line[0] == '$')
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(line.Substring(1));
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(line);
                    }
                }

                Console.WriteLine("\nPress any key...");
                Console.ReadKey();
            }
        }

    }
}
