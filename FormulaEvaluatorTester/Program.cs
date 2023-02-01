using FormulaEvaluator;

namespace FormulaEvaluatorTester
{
    /// <summary>
    /// This class tests various cases and edge cases to determine how
    /// well the Evaluate method works.
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Test1());
            Console.WriteLine(EvaluatorWorksSimpleEquation1());
            Console.WriteLine(EvaluatorWorksSimpleEquation2());
            Console.WriteLine(EvaluatorWorksSimpleEquationWithVar());
            Console.WriteLine(EvaluatorWorksComplexEquation());
            Console.WriteLine(EvaluatorWorksComplexEquationWithVar());
            Console.WriteLine(EvaluatorWorksOrderOfOpsSimple());
            Console.WriteLine(EvaluatorWorksOrderOfOpsComplex());
            Console.WriteLine(EvaluatorWorksWrongParenthesis());
            Console.WriteLine(EvaluatorWorksUnaryNegative());
            Console.WriteLine(EvaluatorWorksEmptyExpression());
            Console.WriteLine(EvaluatorWorksImpliedDivisionByZero());
            Console.WriteLine(EvaluatorWorksDivisionByZero());
            Console.WriteLine(EvaluatorWorksIllegalParenthesis());
            Console.WriteLine(EvaluatorWorksWithIllegalVar());
        }

        private static string Test1()
        {
            int value = Evaluator.Evaluate("5+7+(5)8", s => 0);
            return value + "";
        }
        /// <summary>
        /// This method tests the Evaluate method on a simple method
        /// </summary>
        /// <returns></returns> A string that tells the user whether or not the
        /// Evaluate method gave back the correct answer or the wrong answer
        private static string EvaluatorWorksSimpleEquation1()
        {
            int value = Evaluator.Evaluate("5 + 5 - 2", Lookup);
            if (value == 8)
                return "correct";
            else
                return "wrong";
        }

        /// <summary>
        /// This method tests the Evaluate method on a simple method (uses multiplication)
        /// </summary>
        /// <returns></returns> A string that tells the user whether or not the
        /// Evaluate method gave back the correct answer or the wrong answer
        private static string EvaluatorWorksSimpleEquation2()
        {
            int value = Evaluator.Evaluate("(2 / 1) * 2", Lookup);
            if (value == 4)
                return "correct";
            else
                return "wrong";
        }

        /// <summary>
        /// This method tests the Evaluate method on a simple method that includes a variable
        /// </summary>
        /// <returns></returns> A string that tells the user whether or not the Evaluate
        ///  method gave back the correct answer or the wrong answer.
        private static string EvaluatorWorksSimpleEquationWithVar()
        {
            int value = Evaluator.Evaluate("7 * 2 - a1", Lookup);
            if (value == 12)
                return "correct";
            else
                return "wrong";
        }

        /// <summary>
        /// This method tests the Evaluate method on a complex method
        /// </summary>
        /// <returns></returns> A string that tells the user whether or not the Evaluate
        /// method gave back the correct answer or the wrong answer.
        private static string EvaluatorWorksComplexEquation()
        {
            int value = Evaluator.Evaluate("8 / 4 + (7 * 7) - 9", Lookup);
            if (value == 42)
                return "correct";
            else
                return "wrong";
        }

        /// <summary>
        /// This method tests the Evaluate method on a complex method that includes a variable
        /// </summary>
        /// <returns></returns> A string that tells the user whether or not the Evaluate
        /// method gave back the correct answer or the wrong answer.
        private static string EvaluatorWorksComplexEquationWithVar()
        {
            int value = Evaluator.Evaluate("8 / z2 + (7 * a1) - 9", Lookup);
            if (value == 13)
                return "correct";
            else
                return "wrong";
        }

        /// <summary>
        /// This method tests the Evaluate method on a simple method, but takes 
        /// the order of operations into account
        /// </summary>
        /// <returns></returns> A string that tells the user whether or not the Evaluate
        /// method gave back the correct answer or the wrong answer.
        private static string EvaluatorWorksOrderOfOpsSimple()
        {
            int value = Evaluator.Evaluate("8 + 2 * 3", Lookup);
            if (value == 14)
                return "correct";
            else
                return "wrong";
        }

        /// <summary>
        /// This method tests the Evaluate method on a complex method, but takes
        /// the order of operations into account
        /// </summary>
        /// <returns></returns> A string that tells the user whether or not the Evaluate
        /// method gave back the correct answer or the wrong answer.
        private static string EvaluatorWorksOrderOfOpsComplex()
        {
            int value = Evaluator.Evaluate("8 - 24 / 8 * 2", Lookup);
            if (value == 2)
                return "correct";
            else
                return "wrong";
        }

        /// <summary>
        /// This method tests the Evaluate method on a simple method that
        /// should throw an exception due to an incorrect use of parenthesis.
        /// </summary>
        /// <returns></returns> A string that should only return if the Evaluate
        /// method did something incorrectly and did not throw an exception.
        private static string EvaluatorWorksWrongParenthesis()
        {
            int value = Evaluator.Evaluate("8 - )2(", Lookup);
            if (value == 6)
                return "wrong";
            else
                return  value + "";
        }

        /// <summary>
        /// This method tests the Evaluate method on a simple method that should
        /// throw an exception because it contains an illegal variable.
        /// </summary>
        /// <returns></returns> A string that should only return if the Evaluate
        /// method did something incorrectly and did not throw an exception.
        private static string EvaluatorWorksWithIllegalVar()
        {
            int value = Evaluator.Evaluate("8 * b2a - 2", Lookup);
            if (value == 6)
                return "wrong";
            else
                return value + "";
        }

        /// <summary>
        /// This method tests the Evaluate method on a simple method that should
        /// throw an exception because it of an incorrect use of parenthesis.
        /// </summary>
        /// <returns></returns> A string that should only return if the Evaluate
        /// method did something incorrectly and did not throw an exception.
        private static string EvaluatorWorksIllegalParenthesis()
        {
            int value = Evaluator.Evaluate("8 - 6 / (2", Lookup);
            if (value == 1)
                return "wrong";
            else
                return value + "";
        }

        /// <summary>
        /// This method tests the Evaluate method on a simple method that should
        /// throw an exception because it divides by zero.
        /// </summary>
        /// <returns></returns> A string that should only print out if the Evaluate
        /// method did something incorrectly and did not throw an exception.
        private static string EvaluatorWorksDivisionByZero()
        {
            int value = Evaluator.Evaluate("8/0", Lookup);
            if (value == 8)
                return "wrong";
            else
                return value + "";
        }

        /// <summary>
        /// This method tests the Evaluate method on a simple method that should
        /// throw an exception because it divides by zero as long as Evaluate follows
        /// the order of operations correctly,
        /// </summary>
        /// <returns></returns> A string that should only print out if the Evaluate
        /// method did something incorrectly and did not throw an exception.
        private static string EvaluatorWorksImpliedDivisionByZero()
        {
            int value = Evaluator.Evaluate("8 / (2-a1)", Lookup);
            if (value == 4)
                return "wrong";
            else
                return value + "";
        }

        /// <summary>
        /// This method tests the Evaluate method on an empty expression, which
        /// should throw an exception because an int must be able to be returned for
        /// an expression to be valid.
        /// </summary>
        /// <returns></returns> A string that should only print out if the Evaluate
        /// method did something incorrectly and did not throw an exception.
        private static string EvaluatorWorksEmptyExpression()
        {
            int value = Evaluator.Evaluate("", Lookup);
            if (value == 0)
                return "wrong";
            else
                return value + "";
        }

        /// <summary>
        /// This method tests the Evaluate method on a simple expression that should
        /// throw an exception because unary negatives invalidate an expression.
        /// </summary>
        /// <returns></returns> A string that should only print out if the Evaluate
        /// method did something incorrectly and did not throw an exception.
        private static string EvaluatorWorksUnaryNegative()
        {
            int value = Evaluator.Evaluate("-5 * 2", Lookup);
            if (value == -10)
                return "wrong";
            else
                return value + "";
        }

        /// <summary>
        /// Assigns a value to variables
        /// </summary>
        /// <param name="name"></param> the name of the variable
        /// <returns></returns> the int value associated with that variable name
        private static int Lookup(string name)
        {
            if (name == "a1")
                return 2;
            else
                return 1;
        }
    }
}