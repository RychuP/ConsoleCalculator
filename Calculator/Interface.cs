using System;
using System.Collections.Generic;

namespace Calculator
{
    abstract class WindowSection
    {
        string header;
        const int HEADER_LENGTH = 100;

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
            Console.WriteLine(Result + "\n\n");
        }
    }

    class Input : WindowSection
    {
        Output output = new Output();
        Library library = new Library();

        public Input() : base("Input")
        {
            Exit = false;
        }

        public bool Exit { get; private set; }

        public void Print()
        {
            Console.Clear();
            output.Print();
            library.Print();
            PrintHeader();

            Console.Write("[{0}]> ", library.CurrentVariable.Name);
            String Parser = Console.ReadLine();
            Evaluate(Parser);
        }

        public void Evaluate(string inputTxt)
        {
            if (inputTxt.Length > 0)
            {
                switch (inputTxt)
                {
                    case "help":
                        output.Result =
                            "- calculator accepts brackets, decimals, single letter variables and four basic operators '+ - * /'\n" +
                            "- type a letter and press enter to introduce a new or switch to an existing variable\n" +
                            "- type '#' followed by text to put a comment on top of the current variable\n" +
                            "- type 'del' to delete current variable or type 'del' followed by a lettter to delete another variable\n" +
                            "- type 'exit' to exit from the application\n" +
                            "- type 'help' to read this section again";
                        break;

                    case "exit":
                        Exit = true;
                        break;

                    case "del":
                        output.Result = library.RemoveVar();
                        break;

                    default:
                        if (inputTxt.Length == 1)
                        {
                            if (Char.IsLetter(inputTxt[0]))
                            {
                                library.SetCurrentVariable(inputTxt[0]);
                            }
                            else if (inputTxt[0] == '#')
                            {
                                library.CurrentVariable.Comment = "";
                            }
                        }
                        // calculations 
                        else
                        {
                            // comment
                            if (inputTxt[0] == '#')
                            {
                                inputTxt = inputTxt.Substring(1);
                                inputTxt = inputTxt.Trim();
                                library.CurrentVariable.Comment = inputTxt;
                            }
                            // deletion
                            else if (inputTxt.StartsWith("del"))
                            {
                                // skip 'del'
                                inputTxt = inputTxt.Substring(3);
                                inputTxt = inputTxt.Trim();
                                if (inputTxt.Length == 1 && Char.IsLetter(inputTxt[0]))
                                {
                                    output.Result = library.RemoveVar(inputTxt[0]);
                                }
                                else
                                {
                                    output.Result = $"Couldn't delete '{inputTxt}'.";
                                }
                            }
                            // computation
                            else
                            {
                                var parser = new Parser(inputTxt);
                                if (parser.ContainsOnlyValidChars)
                                {
                                    var expression = new Expression(parser);
                                    if (expression != null && expression.Error == null)
                                    {
                                        library.CurrentVariable.Value = expression.Value;
                                        library.CurrentVariable.LastOperation = inputTxt;
                                        output.Result = $"{expression.Value}";
                                    }
                                    else
                                    {
                                        output.Result = "This operation couldn't compute. Please check your input.";
                                    }
                                }
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


        /* ------------ Constructor ----------- */
        public Library() : base("Variables")
        {
            variables.Add(new Variable('x', false));
            variables[0].Comment = "Default, unremovable variable. Use it to hold final result.";

            variables.Add(new Variable('y'));
            variables[1].Comment = "Example, removable variable for intermediary calculations.";

            CurrentVariable = variables[0];
        }

        /* ------------ Properties ------------ */
        public Variable CurrentVariable { get; private set; }


        /* ------------- Methods -------------- */
        public void Print()
        {
            PrintHeader();
            foreach (Variable v in variables)
            {
                v.Print();
            }
            Console.WriteLine();
        }

        public void SetCurrentVariable(char name)
        {
            Variable existingVar = variables.Find(v => v.Name == name);
            if (existingVar != null)
            {
                CurrentVariable = existingVar;
            }
            // create new var
            else
            {
                Variable newVar = new Variable(name);
                variables.Add(newVar);
                CurrentVariable = newVar;
            }
        }

        public string RemoveVar(char? name = null)
        {
            if (!name.HasValue)
            {
                if (CurrentVariable.IsRemovable)
                {
                    variables.Remove(CurrentVariable);
                    string msg = $"Variable '{CurrentVariable.Name}' has been removed.";
                    CurrentVariable = variables[0];
                    return msg;
                }
                else
                {
                    return $"Variable '{CurrentVariable.Name}' couldn't be removed.";
                }
            }
            else
            {
                Variable variable = variables.Find(x => x.Name == name.Value);
                if (variable != null)
                {
                    if (variable.IsRemovable)
                    {
                        variables.Remove(variable);
                        if (CurrentVariable == variable)
                        {
                            CurrentVariable = variables[0];
                        }
                        return $"Variable '{variable}' has been removed.";
                    }
                    else
                    {
                        return $"Variable '{CurrentVariable.Name}' couldn't be removed.";
                    }
                }
                else
                {
                    return $"Couldn't remove variable {name.Value}. It has not been created.";
                }
            }
        }
    }
}