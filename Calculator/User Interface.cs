using System;
using System.Collections.Generic;

namespace Calculator
{
    abstract class WindowSection
    {
        string header;
        const int HEADER_LENGTH = 30;

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
            int leftSideHyphenAmount = (int) Math.Round(halfHyphenAmount);

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

            Console.WriteLine(hyphenatedHeader);
        }
    }

    class Output : WindowSection
    {
        public Output() : base("Output")
        {
            Result = "Rychu's Calculator v0.2. Type 'help' for available commands.";
        }

        public string Result { get; set; }

        public void Print()
        {
            PrintHeader();
            Console.WriteLine(Result);
        }
    }

    class Input : WindowSection
    {
        Output output = new Output();
        Library library = new Library();

        public Input() : base("Input")
        {

        }

        public bool Exit
        {
            get
            {
                return false;
            }
        }

        public void Print()
        {
            Operation operation;
            ConsoleKeyInfo key;

            Console.Clear();
            output.Print();
            library.Print();
            PrintHeader();

            String userInput = Console.ReadLine();
            Evaluate(userInput);
        }

        public void Evaluate(string userInput)
        {
            if (userInput.Length > 0)
            {
                switch (userInput)
                {
                    case "help":
                        output.Result =
                            "- calculator accepts brackets, decimals, single letter variables and four basic operators + - * /\n" +
                            "- type a letter and press enter to introduce a new or switch to an existing variable\n" +
                            "- type 'del' to delete current variable or type 'del' followed by a lettter to delete another variable\n" +
                            "- type 'help' to read this section again";
                        break;

                    default:
                        var parser = new UserInput(userInput);
                        if (parser.ContainsOnlyValidChars)
                        {
                            var expression = new Expression(parser);
                            if (expression != null && expression.Error == null)
                            {
                                library.CurrentVariable.Value = expression.Value;
                                library.CurrentVariable.LastOperation = userInput;
                                output.Result = expression.Value.ToString();
                            }
                            else
                            {
                                output.Result = "This operation couldn't compute. Please check your input.";
                            }
                        }
                        break;
                }
            }
        }
    }


    // holds all the variables
    class Library : WindowSection
    {
        /* -------------- Fields -------------- */
        List<Variable> variables = new List<Variable>();
        int currentVariableIndex = 0;


        /* ------------ Constructor ----------- */
        public Library() : base("Variables")
        {
            variables.Add(new Variable('x', false));
            variables[0].Comment = "Default, unremovable variable. Use it to hold final result.";

            variables.Add(new Variable('y'));
            variables[1].Comment = "Example, removable variable for intermediary calculations.";
        }

        /* ------------ Properties ------------ */
        public Variable CurrentVariable
        {
            get
            {
                return variables[currentVariableIndex];
            }
        }


        /* ------------- Methods -------------- */
        public void Print()
        {
            PrintHeader();
            foreach (Variable v in variables)
            {
                v.Print();
            }
        }

        public void SetCurrentVariable(char name)
        {
            
        }
    }
}