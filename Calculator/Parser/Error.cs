namespace Calculator
{
    // holds index of a char in input string causing problems during parsing
    class Error
    {
        public Error(int problemIndex)
        {
            Index = problemIndex;
        }

        public int Index { get; set; }
    }
}
