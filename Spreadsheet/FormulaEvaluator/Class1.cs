using System.Numerics;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// This class is used in the Spreadsheet to evaluate any infix expressions that
    /// may be located in the Spreadsheet. The class solves expressions in accordance with the
    /// order of operations.
    /// </summary>
    public static class Evaluator
    {
        // Field
        public delegate int Lookup(String variable_name); //A delegate that allows the Evaluate method to assign values to variables

        /// <summary>
        /// Takes a given mathematical infix expression and determines the value of that expression.
        /// The given expression can only consist of +, -, /, *, (, ) symbols as well as non-negative
        /// integers and variables that contain one or more letters followed by one or more numbers. The
        /// letters can be upper case or lower case.
        /// </summary>
        /// <param name="expression"></param> The given mathematical infix expression to be evaluated
        /// <param name="variableEvaluator"></param> The delegate given to allow the Evaluate method to evaluate variables in given expressions
        /// <returns></returns>
        public static int Evaluate (String expression, Lookup variableEvaluator)
        {
            string curToken; //The token currently being evaluated
            string[] tokens = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)"); //Splits the tokens in the given expression into an array
            Stack<String> operators = new Stack<String>(); //Creates a stack that will hold all of the operators in the given expression
            Stack<int> values = new Stack<int>(); //Creates a stack that will hold all of the variable and number values in the given expression

            for (int pos = 0; pos < tokens.Length; pos++)
            {
                curToken = tokens[pos].Trim();

                if (curToken.StartsWith("a-zA-Z") && curToken.Contains("0-9"))
                {
                    if (operators.Peek() == "*")
                    {
                        int leftHandSide = values.Pop();
                        operators.Pop();
                        values.Push(leftHandSide * variableEvaluator(curToken));
                    }
                    else if (operators.Peek() == "/")
                    {
                        int leftHandSide = values.Pop();
                        operators.Pop();
                        values.Push(leftHandSide / variableEvaluator(curToken));
                    }
                    else
                    {
                        values.Push(variableEvaluator(curToken));
                    }
                }
                else if (curToken.Contains("0-9"))
                {
                    int rightHandSide = int.Parse(curToken);
                    if (operators.Peek() == "*")
                    {
                        int leftHandSide = values.Pop();
                        operators.Pop();
                        values.Push(leftHandSide * rightHandSide);
                    }
                    else if (operators.Peek() == "/")
                    {
                        int leftHandSide = values.Pop();
                        operators.Pop();
                        values.Push(leftHandSide / rightHandSide);
                    }
                    else
                    {
                        values.Push(rightHandSide);
                    }
                }
                else if (curToken == "+" || curToken == "-")
                {
                    if (operators.Peek() == "+")
                    {
                        int rightHandSide = values.Pop();
                        int leftHandSide = values.Pop();
                        operators.Pop();
                        values.Push(leftHandSide + rightHandSide);
                    }
                    else if (operators.Peek() == "-")
                    {
                        int rightHandSide = values.Pop();
                        int leftHandSide = values.Pop();
                        operators.Pop();
                        values.Push(leftHandSide - rightHandSide);
                    }
                    operators.Push(curToken);
                }
                else if (curToken == "*" || curToken == "/" || curToken == "(")
                {
                    operators.Push(curToken);
                }
                else if (curToken == ")")
                {

                }
            }

            return 0;
        }
    }
}