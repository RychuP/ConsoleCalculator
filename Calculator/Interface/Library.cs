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
            variables.Add(new Variable('x', false));
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
            Console.WriteLine();
        }

        public void SetCurrentVariable(char name)
        {
            Variable existingVar = variables.Find(v => v.Name == name);
            if (existingVar != null)
            {
                CurrentVariable = existingVar;
            }
            // create new var
            else
            {
                Variable newVar = new Variable(name);
                variables.Add(newVar);
                CurrentVariable = newVar;
            }
        }

        public string RemoveAll()
        {
            Variable x = variables[0];
            int count = variables.Count;
            variables.Clear();
            variables.Add(x);
            if (CurrentVariable != variables[0])
            {
                variables.Add(CurrentVariable);
                return $"Deleted {count - 2} variables.";
            }
            else
            {
                return $"Deleted {count - 1} variables.";
            }
        }

        public string RemoveVar(char? name = null)
        {
            if (!name.HasValue)
            {
                if (CurrentVariable.IsRemovable)
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
                    if (variable.IsRemovable)
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
                    return $"Couldn't remove variable {name.Value}. It has not been created.";
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
                        result = Math.Round(result, 2);
                        return $"{result}";
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
    }
}