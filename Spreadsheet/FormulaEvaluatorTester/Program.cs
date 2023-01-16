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

        private static int Lookup(string name)
        {
            if (name == "a1")
                return 2;
            else
                return 1;
        }
    }
}