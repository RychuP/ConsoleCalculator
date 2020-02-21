using System;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            String userInput;
            Operation operation;
            ConsoleKeyInfo key;

            do
            {
                Console.Clear();
                Console.WriteLine("Enter calculation to compute:");
                userInput = Console.ReadLine();
                operation = new Operation(userInput);
                Console.WriteLine("= " + operation.Result);
                Console.WriteLine();
                Console.Write("Another operation? Y/N ");
                key = Console.ReadKey();
            }
            while (key.KeyChar == 'y');
        }
    }


    class Operation
    {
        UserInput userInput;
        Expression expression;

        public Operation(string txt)
        {
            userInput = new UserInput(txt);

            if (userInput.ContainsOnlyValidChars)
            {
                // an expression to evaluate
                if (!userInput.IsEmpty)
                {
                    expression = new Expression(userInput);
                }
            }
        }

        public string Result
        {
            get
            {
                if (expression != null && expression.Error == null)
                {
                    return expression.Value.ToString();
                }
                else
                {
                    return "This operation couldn't compute. Please check your input.";
                }
            }
        }
    }
}
