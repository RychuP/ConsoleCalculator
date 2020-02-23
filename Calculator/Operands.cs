using System;

namespace Calculator
{
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
        public Expression(Parser input) : base()
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
                        Error = input.GetError();
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

            // check for missing operand at the end
            if (myOperand != null && lastOperand.Operator != null)
            {
                Error = input.GetError();
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
}