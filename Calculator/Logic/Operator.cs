using System;

namespace Calculator
{
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