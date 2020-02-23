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
}
