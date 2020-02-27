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
            foreach (KeyValuePair<string, char> kvp in Function.Types)
            {
                input = input.Replace(kvp.Key, $"{kvp.Value}");
            }
            // replace constant names with symbols (easier to read by parser)
            foreach (KeyValuePair<string, char> kvp in Constant.Predefined)
            {
                input = input.Replace(kvp.Key, $"{kvp.Value}");
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
            bool assemblingFunction = false;
            // some random char to throw exception if function symbol doesn't get assigned when it should
            char functionType = '?';

            while (currentIndex < input.Length)
            {
                // when there is no open brackets
                if (openBracketCount == 0)
                {
                    // after the function symbol the only expected char is open bracket
                    if (assemblingFunction)
                    {
                        if (currentChar == '(')
                        {
                            openBracketCount++;
                            output.Add(currentChar);
                        }
                        else
                        {
                            return GetError();
                        }
                    }

                    else
                    {
                        if (Char.IsDigit(currentChar))
                        {
                            output.Add(currentChar);
                        }

                        else if (!assemblingDecimal)
                        {
                            if (output.IsEmpty)
                            {
                                // open bracket
                                if (currentChar == '(')
                                {
                                    openBracketCount++;
                                    output.Add(currentChar);
                                }

                                // operators
                                else if (Operator.IsSupported(currentChar))
                                {
                                    var op = new Operator(currentChar);
                                    IncrementIndex();
                                    return op;
                                }

                                // variable letters
                                else if (letterOnly.IsMatch($"{currentChar}"))
                                {
                                    Variable variable = library.Find(currentChar);
                                    bool nextCharNotLetter = currentIndex + 1 == input.Length || !Char.IsLetter(input[currentIndex + 1]);
                                    if (variable != null && nextCharNotLetter)
                                    {
                                        IncrementIndex();
                                        return new Constant(variable.Value);
                                    }
                                    else
                                    {
                                        return GetError();
                                    }
                                }

                                // pi
                                else if (currentChar == Constant.Predefined["pi"])
                                {
                                    IncrementIndex();
                                    return new Constant(Math.PI);
                                }

                                // function symbols
                                else if (currentChar == Function.Types["sin"] ||
                                         currentChar == Function.Types["cos"])
                                {
                                    assemblingFunction = true;
                                    functionType = currentChar;
                                }

                                // everything else
                                else
                                {
                                    return GetError();
                                }
                            }

                            // output not empty
                            else
                            {
                                // the only thing expected other than a digit is a dot or an operator
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
                            // exit the while loop
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
                if (assemblingFunction)
                {
                    if (openBracketCount == 0)
                    {
                        return new Function(functionType, output);
                    }

                    else
                    {
                        return GetError();
                    }
                }

                else if (output.IsDigitsOnly || assemblingDecimal)
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
