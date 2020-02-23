﻿using System;
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
            variables[0].Comment = "Default, unremovable variable. Put final result in here.";

            variables.Add(new Variable('y'));
            variables[1].Comment = "Example, removable variable for intermediary calculations.";

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

        public string Evaluate(string inputTxt)
        {
            var parser = new Parser(inputTxt, this);
            if (parser.ContainsOnlyValidChars)
            {
                var expression = new Expression(parser);
                if (expression != null && expression.Error == null)
                {
                    CurrentVariable.Value = expression.Value;
                    CurrentVariable.LastOperation = inputTxt;
                    return $"{expression.Value}";
                }
                else
                {
                    return "This operation couldn't compute. Please check your input.";
                }
            }
            else
            {
                return "Invalid characters in the input. Please use standard ASCII letters and numbers.";
            }
        }

        public Variable Find(char name)
        {
            return variables.Find(x => x.Name == name);
        }
    }
}