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
}
