// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        //Fields
        private Func<string, string> normalizer; //Sets all variables to a common form
        private Func<string, bool> validator; //Determines if variables are of an accepted form
        private List<string> tokens; //A list of the individual tokens in the given formula
        private int sumOfNums; //The sum of all numbers(rounded down) in the formula (used for HashCode)

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) : this(formula, s => s, s => true)
        {
            //See Second Constructor
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            normalizer = normalize;
            validator = isValid;
            sumOfNums = 0;
            int rightParenthesis = 0;
            int leftParenthesis = 0;
            tokens = GetTokens(formula).ToList();

            //If the formula is empty, does not start with a number, variable, or opening parenthesis, and
            //does not end with a number, variable, or closing parenthesis then the formula is invalid
            if (tokens.Count == 0)
                throw new FormulaFormatException("The given formula is invalid because formulas must contain" +
                    " at least one valid token.");
            else if (!double.TryParse(tokens[0], out double result) && !isVar(tokens[0]) && tokens[0] != "(")
                throw new FormulaFormatException("The given formula is invalid because formulas must start" +
                    " with a number, a valid variable, or opening parentheis.");
            else if (!double.TryParse(tokens[tokens.Count - 1], out double result2) &&
                     !isVar(tokens[tokens.Count - 1]) && tokens[tokens.Count - 1] != ")")
                throw new FormulaFormatException("The given formula is invalid because formulas must end" +
                   " with a number, a valid variable, or opening parentheis.");

            //Determines whether or not every token in the formula is valid
            for (int i = 0; i < tokens.Count; i++)
            {
                if (isVar(tokens[i]) && !isVar(normalize(tokens[i])))
                {
                    throw new FormulaFormatException("The given formula is invalid because there is a" +
                        " variable that is illegal when normalized.");
                }
                else if (isVar(tokens[i]) && !isValid(normalize(tokens[i])))
                {
                    throw new FormulaFormatException("The given formula is invalid because there is a" +
                        " variable that is invalid when normalized.");
                }
                else if (tokens[i] == ")")
                {
                    rightParenthesis++;
                    if (rightParenthesis > leftParenthesis)
                        throw new FormulaFormatException("The given formula is invalid because formulas must" +
                            " contain the same amount of opening and closing parenthesis.");
                }
                else if (tokens[i] == "(" || tokens[i] == "+" || tokens[i] == "-" || tokens[i] == "*" ||
                    tokens[i] == "/")
                {
                    if (tokens[i] == "(")
                        leftParenthesis++;

                    if (!double.TryParse(tokens[i + 1], out double result) && !isVar(tokens[i + 1])
                        && tokens[i + 1] != "(")
                    {
                        throw new FormulaFormatException("The given formula is invalid because all opening" +
                            " parenthesis and operators must be followed by a number, variable, or opening" +
                            " parenthesis.");
                    }
                }
                else if (double.TryParse(tokens[i], out double result) || tokens[i] == ")")
                {
                    sumOfNums += (int)result;
                    if (i + 1 < tokens.Count && tokens[i + 1] != "+" && tokens[i + 1] != "-" &&
                        tokens[i + 1] != "/" && tokens[i + 1] != "*" && tokens[i + 1] != ")")
                        throw new FormulaFormatException("The given formula is invalid because all numbers," +
                            " variables, and closing parenthesis must be followed by an operator or closing" +
                            " parenthesis.");
                }
            }
            if (leftParenthesis != rightParenthesis)
                throw new FormulaFormatException("The given formula is invalid because the formula does" +
                    " not have an equal number of opening and closing parenthesis.");
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            try
            {
                Stack<double> values = new Stack<double>();
                Stack<string> operators = new Stack<string>();
                string curToken; //The token currently being evaluated

                //Evaluates the given expression, token by token
                for (int pos = 0; pos < tokens.Count; pos++)
                {
                    curToken = tokens[pos].Trim(); //Ignores any whitespace in the current token

                    //Determines what kind of token the current token is and evaluates it
                    if (isVar(curToken))
                    {
                        values.Push(lookup(normalizer(curToken)));
                        multiplyOrDivide(values, operators);
                    }
                    else if (double.TryParse(curToken, out double result))
                    {
                        values.Push(result);
                        multiplyOrDivide(values, operators);
                    }
                    else if (curToken == "+" || curToken == "-")
                    {
                        addOrSubtract(values, operators);
                        operators.Push(curToken);
                    }
                    else if (curToken == "*" || curToken == "/" || curToken == "(")
                    {
                        operators.Push(curToken);
                    }
                    else if (curToken == ")")
                    {
                        addOrSubtract(values, operators);
                        operators.Pop();
                        multiplyOrDivide(values, operators);
                    }
                }

                //Determines if one of the two possible endstates have been achieved.
                //If so then a value is returned. If not then an exception is thrown.
                if (operators.Count == 0 && values.Count == 1)
                {
                    return values.Pop();
                }
                else
                {
                    if (operators.Peek() == "+")
                    {
                        double rightHandSide = values.Pop();
                        operators.Pop();
                        double leftHandSide = values.Pop();
                        return (leftHandSide + rightHandSide);
                    }
                    else
                    {
                        double rightHandSide = values.Pop();
                        operators.Pop();
                        double leftHandSide = values.Pop();
                        return (leftHandSide - rightHandSide);
                    }
                }
            }
            catch
            {
                //If at any point during the calculation of the formula an error is thrown then
                // a new FormulaError is returned.
                return new FormulaError("The given formula was invalid. The reason could be due to a divison" +
                                        " by 0 or an invalid Lookup.");
            }
        }


        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<string> variables = new HashSet<string>();

            //Looks through every token in the formula and adds every variable to a HashSet
            foreach (string token in tokens)
            {
                if (isVar(token))
                    if (!variables.Contains(normalizer(token)))
                        variables.Add(normalizer(token));
            }
            //Returns a Hashset containing every variable in the formula (a variable can only be added once)
            return variables;
        }


        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            string returnString = "";

            //Looks through every token in the formula and concatonates each on into a string
            foreach (string token in tokens)
            {
                if (isVar(token))
                    returnString += normalizer(token);
                else
                    returnString += token;
            }
            return returnString;
        }


        /// <summary>
        ///  <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            //If obj is null or is not of the type Formula, then this.Equals(obj) is false
            if (obj == null || obj.GetType() != this.GetType())
                return false;

            Formula exp2 = (Formula)obj;
            List<string> exp2Tokens = exp2.retrieveTokens();
            if (exp2Tokens.Count != this.tokens.Count)
                return false;

            //Looks through every token in both Formulas and if at any point the two tokens
            //are not equal to each other then false will be returned. Otherwise true will be returned.
            for (int i = 0; i < tokens.Count; i++)
            {
                if (double.TryParse(tokens[i], out double result) && double.TryParse(exp2Tokens[i],
                                                                                     out double result2))
                {
                    if (result.ToString() != result2.ToString())
                        return false;
                }
                else if (tokens[i] != exp2Tokens[i])
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// 
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            List<string> tokens = f1.retrieveTokens();
            List<string> exp2Tokens = f2.retrieveTokens();
            if (tokens.Count != exp2Tokens.Count)
                return false;

            //Looks through every token in both Formulas and if at any point the two tokens
            //are not equal to each other then false will be returned. Otherwise true will be returned.
            for (int i = 0; i < tokens.Count; i++)
            {
                if (double.TryParse(tokens[i], out double result) && double.TryParse(exp2Tokens[i],
                                                                                        out double result2))
                {
                    if (result.ToString() != result2.ToString())
                        return false;
                }
                else if (tokens[i] != exp2Tokens[i])
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
        ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            //If the two formulas have the same HashCode then they are (usually)
            //equivalent. If they aren't then the two formulas are not equivalent.
            if (f1.GetHashCode() == f2.GetHashCode())
                return false;
            return true;
        }


        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            //Takes the total number of tokens in the formula and adds it to the total sum
            //of all the numbers in the formula to create a HashCode. 
            return tokens.Count + sumOfNums;
        }


        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
        /// <summary>
        /// Determines if the given string is a variable using the rules
        /// from assignment 01
        /// </summary>
        /// <param name="token"></param> the given string
        /// <returns></returns> true or false
        private bool isVar(string token)
        {
            if (Regex.IsMatch(token, "^[a-zA-Z]+[0-9]$+") && !Char.IsLetter(token[token.Length - 1]))
                return true;
            return false;
        }

        /// <summary>
        /// A helper function that allows access to a formula's
        /// list of tokens.
        /// </summary>
        /// <returns></returns a list of tokens
        private List<string> retrieveTokens()
        {
            return tokens;
        }

        /// <summary>
        /// Determines if the next operator to be evaluated is addition or
        /// subtraction. It the uses the values stack to evaluate that operator.
        /// </summary>
        /// <param name="values"></param> the values stack
        /// <param name="ops"></param> the operators stack
        private static void addOrSubtract(Stack<double> values, Stack<string> ops)
        {
            if (ops.Count != 0 && ops.Peek() == "+")
            {
                double rightHandSide = values.Pop();
                double leftHandSide = values.Pop();
                ops.Pop();
                values.Push(leftHandSide + rightHandSide);
            }
            else if (ops.Count != 0 && ops.Peek() == "-")
            {
                double rightHandSide = values.Pop();
                double leftHandSide = values.Pop();
                ops.Pop();
                values.Push(leftHandSide - rightHandSide);
            }
        }

        /// <summary>
        /// Determines if the next operator to be evaluated is multiplication
        /// or division. It then uses the values stack to evaluate that operator.
        /// </summary>
        /// <param name="values"></param> the values stack
        /// <param name="ops"></param> the operators stack
        /// <returns></returns> return false if there is a divison by 0 and true otherwise
        private static void multiplyOrDivide(Stack<double> values, Stack<string> ops)
        {
            if (ops.Count != 0 && ops.Peek() == "*")
            {
                double rightHandSide = values.Pop();
                double leftHandSide = values.Pop();
                ops.Pop();
                values.Push(leftHandSide * rightHandSide);
            }
            else if (ops.Count != 0 && ops.Peek() == "/")
            {
                double rightHandSide = values.Pop();
                double leftHandSide = values.Pop();
                ops.Pop();
                values.Push(leftHandSide / rightHandSide);
            }
        }
    }
}

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

}



// <change>
//   If you are using Extension methods to deal with common stack operations (e.g., checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
//   as long as it doesn't allow nulls!
// </change>
