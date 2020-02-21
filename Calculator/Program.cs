using System;
using System.Text.RegularExpressions;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            String userInput;
            Operation operation;
            ConsoleKeyInfo key;

            do
            {
                Console.Clear();
                Console.WriteLine("Enter calculation to compute:");
                userInput = Console.ReadLine();
                operation = new Operation(userInput);
                Console.WriteLine("= " + operation.Result);
                Console.WriteLine();
                Console.Write("Another operation? Y/N ");
                key = Console.ReadKey();
            }
            while (key.KeyChar == 'y');
        }
    }


    class Operation
    {
        UserInput userInput;
        Expression expression;

        public Operation(string txt)
        {
            userInput = new UserInput(txt);

            if (userInput.ContainsOnlyValidChars)
            {
                // an expression to evaluate
                if (!userInput.IsEmpty)
                {
                    expression = new Expression(userInput);
                }
            }
        }

        public string Result
        {
            get
            {
                if (expression != null && expression.Error == null)
                {
                    return expression.Value.ToString();
                }
                else
                {
                    return "This operation couldn't compute. Please check your input.";
                }
            }
        }
    }

    abstract class Operand
    {
        // Constructor
        public Operand()
        {
            IsPositive = true;
        }
        // Properties
        public abstract double Value { get; set; }
        public bool IsPositive { get; set; }
        public Operand NextOperand { get; set; }
        public Operator Operator { get; set; }
        public Error Error { get; protected set; }

        // Methods
        public void Evaluate()
        {
            if (Operator != null)
            {
                while (NextOperand != null)
                {
                    if (!Operator.IsHighPriority && NextOperand.Operator != null)
                    {
                        NextOperand.Evaluate();
                    }

                    if (Operator.IsAddition) Value += NextOperand.Value;
                    else if (Operator.IsSubstraction) Value -= NextOperand.Value;
                    else if (Operator.IsMultiplication) Value *= NextOperand.Value;
                    else if (Operator.IsDivision) Value /= NextOperand.Value;
                    else throw new Exception("Attempt to evaluate operands with an unimplemented operator.");

                    Operator = NextOperand.Operator;
                    NextOperand = NextOperand.NextOperand;
                }
            }
        }
    }

    class Constant : Operand
    {
        double myValue;

        public Constant(double value) : base()
        {
            Value = value;
        }

        // constant holds its own value
        public override double Value
        {
            get
            {
                if (IsPositive) return myValue;
                else return -myValue;
            }

            set
            {
                myValue = value;
            }
        }
    }

    // self contained stream of operands: either the whole user input or whatever is in brackets
    class Expression : Operand
    {
        // First operand in the que of operands
        Operand myOperand;
        // Remembers last operand added to the que
        Operand lastOperand;

        // Constructor
        public Expression(UserInput input) : base()
        {
            Object expressionPart = input.GetNextPart();
            while (expressionPart != null)
            {
                if (expressionPart is Error)
                {
                    Error = expressionPart as Error;
                    break;
                }

                bool isPositive = true;

                // 1. get an operator (if present for signed operands)
                if (expressionPart is Operator)
                {
                    Operator op = expressionPart as Operator;

                    // I. can only be positive or negative
                    if (op.IsAddition || op.IsSubstraction)
                    {
                        if (op.IsSubstraction)
                        {
                            isPositive = false;
                        }
                    }
                    else
                    {
                        Error = input.GetError();
                        break;
                    }

                    // get part for the next if statement
                    expressionPart = input.GetNextPart();
                    if (expressionPart is null || expressionPart is Operator)
                    {
                        Error = input.GetError();
                        break;
                    }
                }

                // 2. get the operand
                Operand operand = expressionPart as Operand;
                if (operand != null)
                {
                    if (operand.Error == null)
                    {
                        if (!isPositive)
                        {
                            operand.IsPositive = false;
                        }
                        AddOperand(operand);
                    }
                    else
                    {
                        Error = operand.Error;
                        break;
                    }
                }
                else if (myOperand == null || lastOperand.Operator != null)
                {
                    Error = operand.Error;
                    break;
                }

                // 3. get an operator
                expressionPart = input.GetNextPart();
                if (expressionPart != null)
                {
                    if (expressionPart is Operator)
                    {
                        AddOperator(expressionPart as Operator);
                    }
                    else
                    {
                        Error = operand.Error;
                        break;
                    }
                }
                else
                {
                    break;
                }

                // 4. get next part
                expressionPart = input.GetNextPart();
            }
        }

        // Properties

        // expression's value is held in its operand, which should somewhere down the line be a constant
        public override double Value
        {
            get
            {
                myOperand.Evaluate();
                if (IsPositive)
                {
                    return myOperand.Value;
                }
                else
                {
                    return -myOperand.Value;
                }
            }

            set
            {
                myOperand.Evaluate();
                myOperand.Value = value;
            }
        }

        // Methods
        void AddOperand(Operand operand)
        {
            if (lastOperand != null)
            {
                lastOperand.NextOperand = operand;
            }
            else
            {
                myOperand = operand;
            }
            lastOperand = operand;
        }

        void AddOperator(Operator op)
        {
            if (lastOperand != null)
            {
                lastOperand.Operator = op;
            }
            else if (myOperand != null)
            {
                myOperand.Operator = op;
            }
            else throw new Exception("Invalid use of add operator method");
        }
    }

    class Operator
    {
        // Fields
        char type;
        static char[] supportedTypes = { '+', '-', '*', '/' };

        // Constructor
        public Operator(char type)
        {
            if (IsSupported(type))
            {
                this.type = type;
            }
            else throw new Exception("Attempt to use an unsupported operator");
        }

        // Properties
        public static char[] SupportedTypes
        {
            get
            {
                return supportedTypes;
            }
        }

        public bool IsHighPriority
        {
            get
            {
                return (IsMultiplication || IsDivision) ? true : false;
            }
        }

        public bool IsAddition
        {
            get
            {
                return (type == '+') ? true : false;
            }
        }

        public bool IsSubstraction
        {
            get
            {
                return (type == '-') ? true : false;
            }
        }

        public bool IsMultiplication
        {
            get
            {
                return (type == '*') ? true : false;
            }
        }

        public bool IsDivision
        {
            get
            {
                return (type == '/') ? true : false;
            }
        }

        // Methods
        public static bool IsSupported(char op)
        {
            bool result = Array.Exists(supportedTypes, ch => ch == op);
            return result;
        }
    }

    class UserInput
    {
        // Fields
        Regex validCharacters = new Regex(@"^[a-zA-Z0-9_()*\/\+-.]+$");
        Regex digitsOnly = new Regex("^[0-9]+$");
        string input;
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
        char CurrentChar { get; set; }

        public bool ContainsOperators
        {
            get
            {
                if (input.IndexOfAny(Operator.SupportedTypes) != -1) return true;
                else return false;
            }
        }

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

        public bool IsFinished
        {
            get
            {
                if (currentIndex == input.Length) return true;
                else return false;
            }
        }

        public bool IsDigitsOnly
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

            while (currentIndex < input.Length)
            {
                // I. when there is no open brackets
                if (openBracketCount == 0)
                {
                    // 1. case: digit
                    if (Char.IsDigit(CurrentChar))
                    {
                        output.Add(CurrentChar);
                    }

                    // 2. case: open bracket
                    else if (CurrentChar == '(')
                    {
                        // string begins with the open bracket
                        if (output.IsEmpty)
                        {
                            openBracketCount++;
                            output.Add(CurrentChar);
                        }
                        // not expecting an open bracket
                        else
                        {
                            return new Error(currentIndex + indexAdjustement);
                        }
                    }

                    // 3. case: closing bracket
                    else if (CurrentChar == ')')
                    {
                        // not expecting a closing bracket
                        if (output.IsEmpty)
                        {
                            return new Error(currentIndex + indexAdjustement);
                        }
                    }

                    // 4. case: operator
                    else if (Operator.IsSupported(CurrentChar))
                    {
                        if (output.IsEmpty)
                        {
                            var op = new Operator(CurrentChar);
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
                        return new Error(currentIndex + indexAdjustement);
                    }
                }

                // II. when there are open brackets
                else
                {
                    // 1. case: digit
                    if (Char.IsDigit(CurrentChar))
                    {
                        output.Add(CurrentChar);
                    }

                    // 2. case: open bracket
                    else if (CurrentChar == '(')
                    {
                        openBracketCount++;
                        output.Add(CurrentChar);
                    }

                    // 3. case: closing bracket
                    else if (CurrentChar == ')')
                    {
                        openBracketCount--;
                        output.Add(CurrentChar);
                        // closing brackets match
                        if (openBracketCount == 0)
                        {
                            IncrementIndex();
                            output.TrimBrackets();
                            output.Reset();
                            break;
                        }
                    }

                    // 4. case: operator
                    else if (Operator.IsSupported(CurrentChar))
                    {
                        output.Add(CurrentChar);
                    }

                    // 5. case: some other unsupported char
                    else
                    {
                        return new Error(currentIndex + indexAdjustement);
                    }
                }

                IncrementIndex();
            }

            // finalize
            if (!output.IsEmpty)
            {
                if (output.IsDigitsOnly)
                {
                    return new Constant(output.ToDouble());
                }

                if (openBracketCount == 0)
                {
                    return new Expression(output);
                }

                return new Error(currentIndex + indexAdjustement);
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
                CurrentChar = input[currentIndex];
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

        public double ToDouble()
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
            CurrentChar = new char();
            if (input.Length > 0)
            {
                // remove all white spaces
                CurrentChar = input[0];
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
