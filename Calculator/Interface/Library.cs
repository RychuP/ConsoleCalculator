using System;
using System.Collections.Generic;

namespace Calculator
{
    // holds all the variables
    class Library : WindowSection
    {
        /* -------------- Fields -------------- */
        List<Variable> variables = new List<Variable>();


        /* ------------ Constructor ----------- */
        public Library() : base("Variables")
        {
            // default var on startup
            variables.Add(new Variable('x'));
            variables[0].Comment = "Default var";
            CurrentVariable = variables[0];
        }

        /* ------------ Properties ------------ */
        public Variable CurrentVariable { get; private set; }


        /* ------------- Methods -------------- */
        public void Print()
        {
            PrintHeader();
            foreach (Variable v in variables)
            {
                v.Print();
            }
        }

        public string SetCurrentVariable(char name)
        {
            Variable existingVar = variables.Find(v => v.Name == name);
            if (existingVar != null)
            {
                CurrentVariable = existingVar;
                return CurrentVariable.Value.ToString();
            }
            // create new var
            else
            {
                Variable newVar = new Variable(name);
                newVar.LastOperation = "0";
                variables.Add(newVar);
                CurrentVariable = newVar;
                return $"New variable '{name}' created.";
            }
        }

        public string RemoveAll()
        {
            int count = variables.Count;
            variables.Clear();
            variables.Add(CurrentVariable);
            return $"Deleted {count - 1} variables.";
        }

        public string RemoveVar(char? name = null)
        {
            if (!name.HasValue)
            {
                if (variables.Count > 1)
                {
                    variables.Remove(CurrentVariable);
                    string msg = $"Variable '{CurrentVariable.Name}' has been removed.";
                    CurrentVariable = variables[0];
                    return msg;
                }
                else
                {
                    return $"Variable '{CurrentVariable.Name}' couldn't be removed.";
                }
            }
            else
            {
                Variable variable = variables.Find(x => x.Name == name.Value);
                if (variable != null)
                {
                    if (variables.Count > 1)
                    {
                        variables.Remove(variable);
                        if (CurrentVariable == variable)
                        {
                            CurrentVariable = variables[0];
                        }
                        return $"Variable '{variable.Name}' has been removed.";
                    }
                    else
                    {
                        return $"Variable '{CurrentVariable.Name}' couldn't be removed.";
                    }
                }
                else
                {
                    return $"Couldn't find variable {name.Value}.";
                }
            }
        }

        // library holds all the variables and it seems to be the most appropriate object
        // to coordinate calculation logic
        public string Evaluate(string inputTxt)
        {
            var parser = new Parser(inputTxt, this);
            if (parser.ContainsOnlyValidChars)
            {
                var expression = new Expression(parser);
                if (expression != null && expression.Error == null)
                {
                    double result = expression.Value;
                    if (Double.IsInfinity(result))
                    {
                        return "This operation couldn't compute. Result is infinity.";
                    }
                    else if (Double.IsNaN(result))
                    {
                        return "This operation couldn't compute. Result is not a number.";
                    }
                    else
                    {
                        CurrentVariable.Value = result;
                        CurrentVariable.LastOperation = inputTxt;
                        string errorsDuringRecalculation = "";

                        //autorecalculation of variables
                        if (Settings.AutomaticVariableRecalculation)
                        {
                            foreach (Variable x in variables)
                            {
                                if (x != CurrentVariable)
                                {
                                    parser = new Parser(x.LastOperation, this);
                                    expression = new Expression(parser);
                                    result = expression.Value;
                                    if (!Double.IsInfinity(result) && !Double.IsNaN(result))
                                    {
                                        x.Value = result;
                                    }
                                    else
                                    {
                                        errorsDuringRecalculation = $"Variable '{x.Name}' couldn't evaluate " +
                                            $"with the current value of '{CurrentVariable.Name}'";
                                    }
                                }
                            }
                        }

                        return String.IsNullOrEmpty(errorsDuringRecalculation) ? 
                            $"{result}" : errorsDuringRecalculation;
                    }
                }
                else
                {
                    return "This operation couldn't compute. Please check your input.";
                }
            }
            else
            {
                return "Invalid characters in the input.";
            }
        }

        public Variable Find(char name)
        {
            return variables.Find(x => x.Name == name);
        }

        public void MoveVariable()
        {
            if (variables.Count < 1) return;

            ConsoleKeyInfo info;
            do
            {
                Console.Clear();
                Print();
                Console.WriteLine("\nPress UP or DOWN to move variable, ENTER to save position...");
                info = Console.ReadKey();
                int index = variables.IndexOf(CurrentVariable);
                switch (info.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (index > 0)
                        {
                            variables.Remove(CurrentVariable);
                            variables.Insert(index - 1, CurrentVariable);
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (index < variables.Count - 1)
                        {
                            variables.Remove(CurrentVariable);
                            variables.Insert(index + 1, CurrentVariable);
                        }
                        break;
                }

            } while (info.Key != ConsoleKey.Enter);
        }
    }
}