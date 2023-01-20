using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// This class is used in the Spreadsheet to evaluate any infix expressions that
    /// may be located in the Spreadsheet. The class solves expressions in by following the
    /// order of operations. Expressions that have a unary negative are invalid and 
    /// can not be evaluated.
    /// </summary>
    public static class Evaluator
    {
        // Field
        public delegate int Lookup(String variableName); //A delegate that allows the Evaluate method to assign values to variables
        private static Stack<String> operators = new Stack<String>(); //Creates a stack that will hold all of the operators in the given expression
        private static Stack<int> values = new Stack<int>(); //Creates a stack that will hold all of the number values to be evaluated

        /// <summary>
        /// Takes a given mathematical infix expression and determines the value of that expression.
        /// The given expression can only consist of +, -, /, *, (, ) symbols as well as non-negative
        /// integers and variables that contain one or more letters followed by one or more numbers. The
        /// letters can be upper case or lower case.
        /// </summary>
        /// <param name="expression"></param> The given mathematical infix expression to be evaluated
        /// <param name="variableEvaluator"></param> The delegate given to allow the Evaluate method to evaluate variables in given expressions
        /// <returns></returns> Returns the total value of the given expression
        public static int Evaluate (String expression, Lookup variableEvaluator)
        {
            nullExpression(expression);
            string curToken; //The token currently being evaluated
            string[] tokens = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)"); //Splits the tokens in the given expression into an array

            //Evaluates the given expression, token by token
            for (int pos = 0; pos < tokens.Length; pos++)
            {
                curToken = tokens[pos].Trim(); //Ignores any whitespace in the current token

                //Determines what kind of token the current token is and evaluates it
                if (Regex.IsMatch(curToken, "^[a-zA-Z]+[0-9]$+") && !Char.IsLetter(curToken[curToken.Length - 1]))
                {
                    values.Push(variableEvaluator(curToken));
                    multiplyOrDivide();
                }
                else if (int.TryParse(curToken, out int result))
                {
                    values.Push(result);
                    multiplyOrDivide();
                }
                else if (curToken == "+" || curToken == "-")
                {
                    addOrSubtract();
                    operators.Push(curToken);
                }
                else if (curToken == "*" || curToken == "/" || curToken == "(")
                {
                    operators.Push(curToken);
                }
                else if (curToken == ")")
                {
                    addOrSubtract();
                    operators.Pop();
                    multiplyOrDivide();
                }
            }

            //Determines if one of the two possible endstates have been achieved.
            //If so then a value is returned. If not then an exception is thrown.
            if (operators.Count == 0 && values.Count == 1)
                return values.Pop();
            else
            {
                toManyOperators();
                notEnoughValues();
                if (operators.Peek() == "+")
                {
                    int rightHandSide = values.Pop();
                    operators.Pop();
                    int leftHandSide = values.Pop();
                    return (leftHandSide + rightHandSide);
                }
                else
                {
                    int rightHandSide = values.Pop();
                    operators.Pop();
                    int leftHandSide = values.Pop();
                    return (leftHandSide - rightHandSide);
                }
            }
        }
        
        /// <summary>
        /// Determines if the next operator to be evaluated is addition or
        /// subtraction. If it is then the values stack is used to evaluate that operator.
        /// </summary>
        private static void addOrSubtract()
        {
            if (operators.Count != 0 && operators.Peek() == "+")
            {
                notEnoughValues();
                int rightHandSide = values.Pop();
                int leftHandSide = values.Pop();
                operators.Pop();
                values.Push(leftHandSide + rightHandSide);
            }
            else if (operators.Count != 0 && operators.Peek() == "-")
            {
                notEnoughValues();
                int rightHandSide = values.Pop();
                int leftHandSide = values.Pop();
                operators.Pop();
                values.Push(leftHandSide - rightHandSide);
            }
        }

        /// <summary>
        /// Determines if the next operator to be evaluated is multiplication or
        /// division. If it is then the values stack is used to evaluate that operator.
        /// </summary>
        private static void multiplyOrDivide()
        {
            if (operators.Count != 0 && operators.Peek() == "*")
            {
                notEnoughValues();
                int rightHandSide = values.Pop();
                int leftHandSide = values.Pop();
                operators.Pop();
                values.Push(leftHandSide * rightHandSide);
            }
            else if (operators.Count != 0 && operators.Peek() == "/")
            {
                notEnoughValues();
                int rightHandSide = values.Pop();
                divisionByZero(rightHandSide);
                int leftHandSide = values.Pop();
                operators.Pop();
                values.Push(leftHandSide / rightHandSide);
            }
        }

        /// <summary>
        /// Determines if a division by 0 will occur
        /// </summary>
        private static void divisionByZero(int value)
        {
            try
            {
                if (value == 0)
                    throw new ArgumentException();
            }
            catch (ArgumentException)
            {
                Console.WriteLine("A division by 0 has occured. This is not allowed");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Determines if there are enough values in the given expression
        /// </summary>
        private static void notEnoughValues()
        {
            try
            {
                if (values.Count <= 1)
                    throw new ArgumentException();
            } 
            catch (ArgumentException)
            {
                Console.WriteLine("The given expression is invalid.");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Determines if there are to many operators in the given expression
        /// </summary>
        private static void toManyOperators()
        {
            try
            {
                if (operators.Count > 1)
                    throw new ArgumentException();
            }
            catch (ArgumentException)
            {
                Console.WriteLine("The given expression is invalid.");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Determines whether or not the given expression is null
        /// </summary>
        /// <param name="expression"></param> The expression to be evaluated
        private static void nullExpression(string expression)
        {
            try
            {
                if (expression == null)
                    throw new ArgumentException();
            }
            catch (ArgumentException)
            {
                Console.WriteLine("The given expression can not be null");
                Environment.Exit(1);
            }
        }
    }
}