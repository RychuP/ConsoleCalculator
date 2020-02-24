using System;

namespace Calculator
{
    class Output : WindowSection
    {
        public Output() : base("Output")
        {
            Result = "Rychu's Calculator v0.5. Type 'help' for available commands.";
        }

        public string Result { get; set; }

        public void Print()
        {
            PrintHeader();
            Console.WriteLine(Result + "\n\n");
        }
    }
}
