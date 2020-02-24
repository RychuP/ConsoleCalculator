using System;
using System.Text.RegularExpressions;

namespace Calculator
{
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
            inputTxt = inputTxt.Trim();
            if (inputTxt.Length > 0)
            {
                switch (inputTxt)
                {
                    case "help":
                        output.Result =
                            "- calculator accepts brackets, decimals, single letter variables and five operators '+, -, *, /, ^'\n" +
                            "- fractional exponents work in this form: ^ (2 / 3), or this: ^ 0.5\n" +
                            "- type a letter to introduce a new variable or switch to an existing variable\n" +
                            "- type '#' followed by text to comment current variable\n" +
                            "- type '#' on its own to remove existing comment\n" +
                            "- type 'del' to delete current variable\n" +
                            "- type 'del' followed by a lettter to delete specific variable\n" +
                            "- type 'del all' to delete all variables but current and x\n" +
                            "- type 'full' to see the current variable with full precision (all fractional digits)\n" +
                            "- type 'help' to read these instructions again" +
                            "- type 'exit' to leave the application\n";
                        break;

                    case "full":
                        output.Result = library.CurrentVariable.Value.ToString();
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
                                library.CurrentVariable.Comment = inputTxt;
                            }
                            // deletion
                            else if (inputTxt.StartsWith("del"))
                            {
                                // skip 'del'
                                inputTxt = inputTxt.Substring(3);
                                // trim for the second time to get rid of spaces between del and word
                                inputTxt = inputTxt.Trim();
                                // remove one var
                                if (inputTxt.Length == 1 && Char.IsLetter(inputTxt[0]))
                                {
                                    output.Result = library.RemoveVar(inputTxt[0]);
                                }
                                // remove all vars but current and x
                                else if (inputTxt == "all")
                                {
                                    output.Result = library.RemoveAll();
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
}
