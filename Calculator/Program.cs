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
