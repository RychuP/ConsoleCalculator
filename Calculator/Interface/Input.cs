using System;
using System.Text.RegularExpressions;

namespace Calculator
{
    // main object coordinating all the others
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
                        output.Result =
                            "calculator accepts:\n" +
                            "- brackets: ()\n" +
                            "- decimals: 123.123\n" +
                            "- single letter variables: a-z A-Z\n" +
                            "- five operators +, -, *, /, ^\n" +
                            "- constants: pi\n" +
                            "- functions: work in progres...\n\n" +
                            "commands:\n" +
                            "- '#' followed by text adds comment to current variable (no text: removes comment)\n" +
                            "- 'del' deletes current variable (also: 'del <letter>' removes specific var) \n" +
                            "- 'del all' (or 'da') deletes all variables but current and x\n" +
                            "- 'full' (or 'fu') shows current variable with full precision (all fractional digits)\n" +
                            "- 'help' (or 'he') shows these instructions\n" +
                            "- 'exit' (or 'ex') leaves application\n\n" +
                            "tips:\n" +
                            "- type a letter to introduce a new variable or switch to an existing one\n" +
                            "- result of your input automaticaly gets assigned to the variable letter\n" +
                            "  in square brackets -> no need to use '='\n" +
                            "- fractional exponents work in this form: ^ (2 / 3), or this: ^ 0.5\n";
                            
                        break;

                    case "full":
                    case "fu":
                        output.Result = library.CurrentVariable.Value.ToString();
                        break;

                    case "exit":
                    case "ex":
                        Exit = true;
                        break;

                    case "del":
                        output.Result = library.RemoveVar();
                        break;

                    case "da":
                        output.Result = library.RemoveAll();
                        break;

                    default:
                        // variable change or clear comment
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
