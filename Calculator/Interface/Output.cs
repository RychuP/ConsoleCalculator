using System;

namespace Calculator
{
    class Output : WindowSection
    {
        public Output() : base("Output")
        {
            Result = $"{Program.Title}. Type 'he' for help, 'se' for settings.";
        }

        public string Result { get; set; }

        public void Print()
        {
            PrintHeader();
            Console.WriteLine(Result + "\n");
        }
    }
}
