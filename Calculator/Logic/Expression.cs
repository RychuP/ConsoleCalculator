using System;

namespace Calculator
{
    // self contained stream of operands: either the whole user input or whatever is in brackets
    class Expression : Operand
    {
        // First operand in the que of operands
        Operand myOperand;
        // Last operand added to the que
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

            // evaluate contents
            myOperand.Evaluate();
            Value = myOperand.Value;
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
}
