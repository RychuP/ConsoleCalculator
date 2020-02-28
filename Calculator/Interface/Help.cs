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
                "- constants: pi, dtr (degrees to radians: pi / 180)",
                "- functions: sin, cos, tan (tg), csc, sec, cot (ctg)",
                "",
                "$Commands:",
                "- '# <description>' add comment to current variable (no description: remove comment)",
                "- 'de' delete current variable ('de <letter>' remove specific var)",
                "- 'da' delete all but current variable",
                "- 'fu' full precision (shows current variable with all fractional digits)",
                "- 'se' settings",
                "- 'ex' exit application",
                "",
                "$Variables:",
                "  Type a letter to introduce a new variable or switch to an existing one.",
                "  For brevity, results with decimal digits in 'Variables' window are displayed",
                "  rounded to 2 decimal places. Underlying value is stored with full precision.",
                "  You can change this behaviour in settings.",
                "",
                "$Functions:",
                "  Built in C# math trigonometric functions are not perfectly accurate.",
                "  Some results are a tiny fraction off."
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
