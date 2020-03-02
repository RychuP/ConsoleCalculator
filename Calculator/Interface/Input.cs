using System;
using System.Text.RegularExpressions;

namespace Calculator
{
    // main object coordinating all the others
    class Input : WindowSection
    {
        Output output = new Output();
        Library library = new Library();
        Help help = new Help();
        Settings settings = new Settings();
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

        // this evaluation step is mainly for commands
        // calculation logic is passed to library which holds all variables
        public void Evaluate(string inputTxt)
        {
            inputTxt = inputTxt.Trim();
            if (inputTxt.Length > 0)
            {
                switch (inputTxt)
                {
                    case "help":
                    case "he":
                        help.Print();
                        Print();
                        break;

                    case "hf":
                        help.Print("functions");
                        Print();
                        break;

                    case "hv":
                        help.Print("variables");
                        Print();
                        break;

                    case "hp":
                        help.Print("priorities");
                        Print();
                        break;

                    case "full":
                    case "fu":
                        output.Result = library.CurrentVariable.Value.ToString();
                        break;

                    case "exit":
                    case "ex":
                        Exit = true;
                        break;

                    case "de":
                        output.Result = library.RemoveVar();
                        break;

                    case "da":
                        output.Result = library.RemoveAll();
                        break;

                    case "settings":
                    case "se":
                        settings.Print();
                        break;

                    case "mo":
                        output.Result = library.MoveVariable();
                        Print();
                        break;

                    default:
                        // variable change or clear comment
                        if (inputTxt.Length == 1 && !Char.IsDigit(inputTxt[0]))
                        {
                            if (asciiLettersOnly.IsMatch(inputTxt))
                            {
                                output.Result = library.SetCurrentVariable(inputTxt[0]);
                            }
                            else if (inputTxt[0] == '#')
                            {
                                library.CurrentVariable.Comment = "";
                                output.Result = "Comment removed.";
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
                            else if (inputTxt.StartsWith("de"))
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
