﻿using System;
using System.Text.RegularExpressions;

namespace Calculator
{
    class UserInput
    {
        // Fields
        Regex validCharacters = new Regex(@"^[a-zA-Z0-9_()*\/\+-.]+$");
        Regex digitsOnly = new Regex("^[0-9]+$");
        string input;
        char currentChar;
        int currentIndex = 0;
        // difference between the index in this instance and the original
        int indexAdjustement = 0;


        // Constructors
        public UserInput(string input = "")
        {
            input = input.Replace(" ", "");
            this.input = input;
            Reset();
        }

        public UserInput(int indexAdjustement)
        {
            input = "";
            this.indexAdjustement = indexAdjustement;
        }


        // Properties
        public bool ContainsOnlyValidChars
        {
            get
            {
                return validCharacters.IsMatch(input);
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
            UserInput output = StartOutput();
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

                        else if (currentChar == '(')
                        {
                            // string begins with the open bracket
                            if (output.IsEmpty)
                            {
                                openBracketCount++;
                                output.Add(currentChar);
                            }
                            // not expecting an open bracket
                            else
                            {
                                return GetError();
                            }
                        }

                        // 3. case: closing bracket
                        else if (currentChar == ')')
                        {
                            // not expecting a closing bracket
                            if (output.IsEmpty)
                            {
                                return GetError();
                            }
                        }

                        // 4. case: operator
                        else if (Operator.IsSupported(currentChar))
                        {
                            if (output.IsEmpty)
                            {
                                var op = new Operator(currentChar);
                                IncrementIndex();
                                return op;
                            }
                            // operator marks end of this operand
                            else
                            {
                                break;
                            }
                        }

                        // 5. case: some other unsupported char
                        else
                        {
                            return GetError();
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

        double ToDouble()
        {
            return Convert.ToDouble(input);
        }

        public UserInput StartOutput()
        {
            return new UserInput(currentIndex + indexAdjustement);
        }

        void TrimBrackets()
        {
            if (input.Length >= 2)
            {
                input = input[1..^1];
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

    class Error
    {
        public Error(int problemIndex)
        {
            Index = problemIndex;
        }

        public int Index { get; set; }
    }
}