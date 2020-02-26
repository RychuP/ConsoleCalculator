using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Calculator
{
    class Parser
    {
        // Fields
        Regex validCharacters = new Regex(@"^[a-zA-Z0-9_()\^\/\+*-. ]+$");
        Regex digitsOnly = new Regex("^[0-9]+$");
        Regex letterOnly = new Regex("^[a-zA-Z]+$");
        string input;
        string originalInput;
        char currentChar;
        int currentIndex = 0;
        // difference between the index in this instance and the original
        int indexAdjustement = 0;
        Library library;


        // Constructors
        public Parser(string input, Library library)
        {
            originalInput = input;
            input = input.Replace(" ", "");
            // replace function names with symbols (easier to read by parser)
            foreach (KeyValuePair<string, string> kvp in Function.Types)
            {
                input = input.Replace(kvp.Key, kvp.Value);
            }
            // replace constant names with symbols (easier to read by parser)
            foreach (KeyValuePair<string, string> kvp in Constant.Predefined)
            {
                input = input.Replace(kvp.Key, kvp.Value);
            }
            this.library = library;
            this.input = input;
            Reset();
        }

        public Parser(int indexAdjustement, Library library)
        {
            input = "";
            this.library = library;
            this.indexAdjustement = indexAdjustement;
        }


        // Properties
        public bool ContainsOnlyValidChars
        {
            get
            {
                return validCharacters.IsMatch(originalInput);
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (input.Length == 0) return true;
                else return false;
            }
        }

        bool IsDigitsOnly
        {
            get
            {
                return digitsOnly.IsMatch(input);
            }
        }


        // Methods
        public Object GetNextPart()
        {
            Parser output = StartOutput();
            int openBracketCount = 0;
            bool assemblingDecimal = false;

            while (currentIndex < input.Length)
            {
                // when there is no open brackets
                if (openBracketCount == 0)
                {
                    if (Char.IsDigit(currentChar))
                    {
                        output.Add(currentChar);
                    }

                    else if (!assemblingDecimal)
                    {
                        if (output.IsEmpty)
                        {
                            if (currentChar == '(')
                            {
                                openBracketCount++;
                                output.Add(currentChar);
                            }

                            // not expecting a closing bracket
                            else if (currentChar == ')')
                            {
                                return GetError();
                            }

                            else if (Operator.IsSupported(currentChar))
                            {
                                var op = new Operator(currentChar);
                                IncrementIndex();
                                return op;
                            }

                            else if (letterOnly.IsMatch($"{currentChar}"))
                            {
                                Variable variable = library.Find(currentChar);
                                if (variable != null &&
                                    // end of input string or next char is not a letter
                                    (currentIndex + 1 == input.Length || !Char.IsLetter(input[currentIndex + 1])))
                                {
                                    IncrementIndex();
                                    return new Constant(variable.Value);
                                }
                                else
                                {
                                    return GetError();
                                }
                            }

                            else if (currentChar == 'π')
                            {
                                IncrementIndex();
                                return new Constant(Math.PI);
                            }

                            // anything else
                            else
                            {
                                return GetError();
                            }
                        }

                        else
                        {
                            // when the output is not empty and there are no open brackets, the only thing expected other than a digit is a dot or an operator
                            if (currentChar == '.')
                            {
                                if (output.IsDigitsOnly)
                                {
                                    assemblingDecimal = true;
                                    output.Add(currentChar);
                                }
                                else
                                {
                                    return GetError();
                                }
                            }

                            // operator marks end of this operand
                            else if (Operator.IsSupported(currentChar))
                            {
                                break;
                            }

                            // anything else other than a dot or operator means error
                            else
                            {
                                return GetError();
                            }
                        }
                    }
                    else
                    {
                        // only operator or end of input can terminate decimal assembly
                        if (Operator.IsSupported(currentChar))
                        {
                            return new Constant(output.ToDouble());
                        }
                        else
                        {
                            return GetError();
                        }
                    }
                }

                // when there are open brackets
                else
                {
                    if (currentChar == '(')
                    {
                        openBracketCount++;
                        output.Add(currentChar);
                    }

                    else if (currentChar == ')')
                    {
                        openBracketCount--;
                        output.Add(currentChar);
                        // closing brackets match
                        if (openBracketCount == 0)
                        {
                            IncrementIndex();
                            output.TrimBrackets();
                            output.Reset();
                            // this is why we are using if and not switch here
                            break;
                        }
                    }

                    // anything else just add in and let the next expression worry about it
                    else
                    {
                        output.Add(currentChar);
                    }
                }

                IncrementIndex();
            }

            // case: end of input -> finalize
            if (!output.IsEmpty)
            {
                if (output.IsDigitsOnly || assemblingDecimal)
                {
                    return new Constant(output.ToDouble());
                }

                else if (openBracketCount == 0)
                {
                    return new Expression(output);
                }

                else
                {
                    return GetError();
                }
            }
            else
            {
                return null;
            }
        }

        public void IncrementIndex()
        {
            if (input.Length > ++currentIndex)
            {
                currentChar = input[currentIndex];
            }
        }

        public Error GetError()
        {
            return new Error(currentIndex + indexAdjustement);
        }

        public void Add(char x)
        {
            input += x;
        }

        public Parser StartOutput()
        {
            return new Parser(currentIndex + indexAdjustement, library);
        }

        double ToDouble()
        {
            return Convert.ToDouble(input);
        }

        void TrimBrackets()
        {
            if (input.Length >= 2)
            {
                input = input.Substring(1, input.Length - 2);
                indexAdjustement++;
            }
        }

        void Reset()
        {
            currentChar = new char();
            if (input.Length > 0)
            {
                // remove all white spaces
                currentChar = input[0];
            }
        }
    }
}
