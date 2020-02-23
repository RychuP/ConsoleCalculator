using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
            Result = "Rychu's Calculator v0.3. Type 'help' for available commands.";
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
        Regex asciiLettersOnly = new Regex(@"^[a-zA-Z]+$");

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
            String inputTxt = Console.ReadLine();
            Evaluate(inputTxt);
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
                            "- type '#' followed by text to comment current variable or just '#' to remove existing comment\n" +
                            "- type 'del' to delete current variable or 'del' followed by a lettter to delete another variable\n" +
                            "- type 'exit' to leave the application\n" +
                            "- type 'help' to read these instructions again";
                        break;

                    case "exit":
                        Exit = true;
                        break;

                    case "del":
                        output.Result = library.RemoveVar();
                        break;

                    default:
                        if (inputTxt.Length == 1 && !Char.IsDigit(inputTxt[0]))
                        {
                            if (asciiLettersOnly.IsMatch(inputTxt))
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
                                output.Result = library.Evaluate(inputTxt);
                                
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
            variables[0].Comment = "Default, unremovable variable. Put final result in here.";

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
                        return $"Variable '{variable.Name}' has been removed.";
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

        public string Evaluate(string inputTxt)
        {
            var parser = new Parser(inputTxt, this);
            if (parser.ContainsOnlyValidChars)
            {
                var expression = new Expression(parser);
                if (expression != null && expression.Error == null)
                {
                    CurrentVariable.Value = expression.Value;
                    CurrentVariable.LastOperation = inputTxt;
                    return $"{expression.Value}";
                }
                else
                {
                    return "This operation couldn't compute. Please check your input.";
                }
            }
            else
            {
                return "Invalid characters in the input. Please use standard ASCII letters and numbers.";
            }
        }

        public Variable Find(char name)
        {
            return variables.Find(x => x.Name == name);
        }
    }
}