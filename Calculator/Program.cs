using System;

namespace Calculator
{
    class Program
    {
        public static readonly string Title = $"Rychu's Console Calculator v0.59";
        static void Main(string[] args)
        {
            Console.WindowHeight = 30;
            Console.BufferWidth = Console.WindowWidth = 90;
            Console.Title = Title;
            Input input = new Input();

            while (!input.Exit)
            {
                input.Print();
            }
        }
    }
}
