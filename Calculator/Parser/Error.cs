namespace Calculator
{
    class Error
    {
        public Error(int problemIndex)
        {
            Index = problemIndex;
        }

        public int Index { get; set; }
    }
}
