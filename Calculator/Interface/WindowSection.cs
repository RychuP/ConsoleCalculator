using System;

namespace Calculator
{
    abstract class WindowSection
    {
        string header;
        const int HEADER_LENGTH = 70;

        public WindowSection(string header)
        {
            this.header = header;
        }

        protected void PrintHeader()
        {
            string hyphenatedHeader = "";
            int emptySpaceCount = 2;
            int hyphenAmount = HEADER_LENGTH - header.Length - emptySpaceCount;
            double halfHyphenAmount = hyphenAmount / 2;
            int leftSideHyphenAmount = (int)Math.Round(halfHyphenAmount);

            for (int i = 0; i < leftSideHyphenAmount; i++)
            {
                hyphenatedHeader += "-";
            }

            int rightSideHyphenAmount = hyphenAmount - leftSideHyphenAmount;
            hyphenatedHeader += $" {header} ";

            for (int i = 0; i < rightSideHyphenAmount; i++)
            {
                hyphenatedHeader += "-";
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(hyphenatedHeader);
            Console.ResetColor();
        }
    }
}
