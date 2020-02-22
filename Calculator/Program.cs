using System;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Input input = new Input();

            while (!input.Exit)
            {
                input.Print();
            }
        }
    }
}
