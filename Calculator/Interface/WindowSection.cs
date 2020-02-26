using System;

namespace Calculator
{
    // common functionality for input, ouput and library 
    abstract class WindowSection
    {
        string header;

        public WindowSection(string header)
        {
            this.header = header;
        }

        protected void PrintHeader()
        {
            string hyphenatedHeader = "";
            int emptySpaceCount = 2;
            int hyphenAmount = Console.WindowWidth - header.Length - emptySpaceCount;
            
            double halfHyphenAmount = hyphenAmount / 2;
            int leftSideHyphenAmount = (int) Math.Round(halfHyphenAmount);
            hyphenatedHeader += new String('-', leftSideHyphenAmount);

            int rightSideHyphenAmount = hyphenAmount - leftSideHyphenAmount;
            hyphenatedHeader += $" {header} {new String('-', rightSideHyphenAmount)}";

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(hyphenatedHeader);
            Console.ResetColor();
        }
    }
}
