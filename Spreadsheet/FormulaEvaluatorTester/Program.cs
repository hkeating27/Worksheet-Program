using FormulaEvaluator;

namespace FormulaEvaluatorTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(EvaluatorWorksSimpleEquation1());
            Console.WriteLine(EvaluatorWorksSimpleEquation2());
            Console.WriteLine(EvaluatorWorksSimpleEquationWithVar());
            Console.WriteLine(EvaluatorWorksComplexEquation());
            Console.WriteLine(EvaluatorWorksComplexEquationWithVar());
            Console.WriteLine(EvaluatorWorksOrderOfOpsSimple());
            Console.WriteLine(EvaluatorWorksOrderOfOpsComplex());
            Console.WriteLine(EvaluatorWorksComplex());
            Console.WriteLine(EvaluatorWorksWrongParenthesis());
            Console.WriteLine(EvaluatorWorksUnaryNegative());
            Console.WriteLine(EvaluatorWorksEmptyExpression());
            Console.WriteLine(EvaluatorWorksImpliedDivisionByZero());
            Console.WriteLine(EvaluatorWorksDivisionByZero());
            Console.WriteLine(EvaluatorWorksIllegalParenthesis());
            Console.WriteLine(EvaluatorWorksWithIllegalVar());
        }

        private static string EvaluatorWorksSimpleEquation1()
        {
            int value = Evaluator.Evaluate("5 + 5 - 2", Lookup);
            if (value == 8)
                return "correct";
            else
                return "wrong";
        }

        private static string EvaluatorWorksSimpleEquation2()
        {
            int value = Evaluator.Evaluate("(2 / 1) * 2", Lookup);
            if (value == 4)
                return "correct";
            else
                return "wrong";
        }

        private static string EvaluatorWorksSimpleEquationWithVar()
        {
            int value = Evaluator.Evaluate("7 * 2 - a1", Lookup);
            if (value == 12)
                return "correct";
            else
                return "wrong";
        }

        private static string EvaluatorWorksComplexEquation()
        {
            int value = Evaluator.Evaluate("8 / 4 + (7 * 7) - 9", Lookup);
            if (value == 42)
                return "correct";
            else
                return "wrong";
        }

        private static string EvaluatorWorksComplexEquationWithVar()
        {
            int value = Evaluator.Evaluate("8 / z2 + (7 * a1) - 9", Lookup);
            if (value == 13)
                return "correct";
            else
                return "wrong";
        }

        private static string EvaluatorWorksOrderOfOpsSimple()
        {
            int value = Evaluator.Evaluate("8 + 2 * 3", Lookup);
            if (value == 14)
                return "correct";
            else
                return "wrong";
        }

        private static string EvaluatorWorksOrderOfOpsComplex()
        {
            int value = Evaluator.Evaluate("8 - 24 / 8 * 2", Lookup);
            if (value == 2)
                return "correct";
            else
                return "wrong";
        }

        private static string EvaluatorWorksComplex()
        {
            int value = Evaluator.Evaluate("((24 * 3) / 3) + (3 + (1/1)) - (4 + (4*4))", Lookup);
            if (value == 8)
                return "correct";
            else
                return "wrong";
        }

        private static string EvaluatorWorksWrongParenthesis()
        {
            int value = Evaluator.Evaluate("8 - )2(", Lookup);
            if (value == 6)
                return "wrong";
            else
                return  value + "";
        }

        private static string EvaluatorWorksWithIllegalVar()
        {
            int value = Evaluator.Evaluate("8 * b2a - 2", Lookup);
            if (value == 6)
                return "wrong";
            else
                return value + "";
        }

        private static string EvaluatorWorksIllegalParenthesis()
        {
            int value = Evaluator.Evaluate("8 - 6 / (2", Lookup);
            if (value == 1)
                return "wrong";
            else
                return value + "";
        }

        private static string EvaluatorWorksDivisionByZero()
        {
            int value = Evaluator.Evaluate("8/0", Lookup);
            if (value == 8)
                return "wrong";
            else
                return value + "";
        }

        private static string EvaluatorWorksImpliedDivisionByZero()
        {
            int value = Evaluator.Evaluate("8 / (2-a1)", Lookup);
            if (value == 4)
                return "wrong";
            else
                return value + "";
        }

        private static string EvaluatorWorksEmptyExpression()
        {
            int value = Evaluator.Evaluate("", Lookup);
            if (value == 0)
                return "wrong";
            else
                return value + "";
        }

        private static string EvaluatorWorksUnaryNegative()
        {
            int value = Evaluator.Evaluate("-5 * 2", Lookup);
            if (value == -10)
                return "wrong";
            else
                return value + "";
        }

        private static int Lookup(string name)
        {
            if (name == "a1")
                return 2;
            else
                return 1;
        }
    }
}