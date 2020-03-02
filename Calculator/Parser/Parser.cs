using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Calculator
{
    // there are many ways to improve this parser, but it works, so... might as well leave it as it is ;)
    class Parser
    {
        // Fields
        Regex validCharacters = new Regex(@"^[a-zA-Z0-9_()\^\/\+*-. ]+$");
        Regex digitsOnly = new Regex("^[0-9]+$");
        Regex letterOnly = new Regex("^[a-zA-Z]+$");
        List<string> functionNames = new List<string>();
        List<string> constantNames = new List<string>();
        Library library;
        string input;
        string originalInput;
        // difference between the index in this instance and the original
        int indexAdjustement = 0;
        int currentIndex = 0;
        char currentChar;


        // Constructors
        public Parser(string input, Library library)
        {
            originalInput = input;
            input = input.Replace(" ", "");

            // replace function names with a symbol and index in Function.Types (easier to read by parser)
            foreach (string name in Function.Types.Keys)
            {
                functionNames.Add(name);
                input = input.Replace(name, $"{Function.Symbol}" + (char)(functionNames.Count - 1));
            }

            // replace constant names with a symbol and index in Constant.Predefined (easier to read by parser)
            foreach (string name in Constant.Predefined.Keys)
            {
                constantNames.Add(name);
                input = input.Replace(name, $"{Constant.Symbol}" + (char)(constantNames.Count - 1));
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
            string functionName = "";

            while (currentIndex < input.Length)
            {
                // when there is no open brackets
                if (openBracketCount == 0)
                {
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
                                if (Operator.IsSupported(currentChar))
                                {
                                    var op = new Operator(currentChar);
                                    IncrementIndex();
                                    return op;
                                }

                                // variable letters
                                else if (letterOnly.IsMatch($"{currentChar}"))
                                {
                                    Variable variable = library.Find(currentChar);
                                    bool nextCharNotLetter = currentIndex + 1 == input.Length || !letterOnly.IsMatch($"{input[currentIndex + 1]}");
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

                                // everything else
                                else switch (currentChar)
                                {
                                    case '(':
                                        openBracketCount++;
                                        output.Add(currentChar);
                                        break;

                                    case Constant.Symbol:
                                        IncrementIndex();
                                        string constantName = GetObjNameByIndex(currentChar, constantNames);
                                        IncrementIndex();
                                        return new Constant(constantName);

                                    case Function.Symbol:
                                        IncrementIndex();
                                        assemblingFunction = true;
                                        functionName = GetObjNameByIndex(currentChar, functionNames);
                                        break;

                                    default:
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
                        return new Function(functionName, output);
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

        string GetObjNameByIndex(char i, List<string> collection)
        {
            int index = (int) i;

            if (index < collection.Count)
            {
                return collection[index];
            }
            else
            {
                throw new Exception("Parser engine error. Index larger than collection count.");
            }
        }
    }
}
