using System;

namespace Calculator
{
    // single letter variable that holds results of valid user inputs
    class Variable
    {
        /* -------------- Fields -------------- */
        char name;
        bool removable;


        /* ------------ Constructor ----------- */
        public Variable(char name, bool removable = true)
        {
            this.name = name;
            this.removable = removable;
            Comment = "";
            Value = 0;
        }


        /* ------------ Properties ------------ */
        public string LastOperation { get; set; }
        public string Comment { get; set; }
        public double Value { get; set; }
        public bool IsRemovable
        {
            get
            {
                return removable;
            }
        }
        public char Name
        {
            get
            {
                return name;
            }
        }


        /* ------------- Methods -------------- */
        public void Print()
        {
            if (Comment.Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("# " + Comment);
                Console.ResetColor();
            }
            Console.WriteLine("{0} = {1} ({2})\n", name, Math.Round(Value, 2), LastOperation);
        }
    }
}
