using SpreadsheetUtilities;
using static SpreadsheetUtilities.Formula;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorEmptyFormula()
        {
            Formula exp = new Formula("");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorOnlyOneOperator()
        {
            Formula exp = new Formula("*");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorInvalidFirstToken()
        {
            Formula exp = new Formula("+ 98");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorInvalidLastToken()
        {
            Formula exp = new Formula("(87 - A1) + 9(");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorIllegalParenthesisUse()
        {
            Formula exp = new Formula("(9))"); 
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorIllegalFollowingOperator()
        {
            Formula exp1 = new Formula("(+8)");
        }

        /// <summary>
        /// Determines whether or not the constructor correctly identifies incorrect
        /// usage of numbers (doubles and ints) as well as variables
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorIllegalFollowingNumOrVar()
        {
            Formula exp1 = new Formula("8.2 7 - 9");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorMissingParenthesis()
        {
            Formula exp = new Formula("((9)");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SecondConstructorIllegalVar()
        {
            Formula exp1 = new Formula("_A1", s => "1", s => true);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SecondConstructorInvalidVar()
        {
            Formula exp1 = new Formula("C5", s => s, s => s.Contains('B'));
        }

        /// <summary>
        /// Determines whether the constructor correctly identifies a 
        /// unary negative
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SecondConstructorUnaryNegative()
        {
            Formula exp1 = new Formula("(-5)", s => s.ToLower(), s => true);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SecondConstructorInvalidToken()
        {
            Formula exp1 = new Formula("5 % 7", s => s.ToLower(), s => true);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetVariablesWorksAsExpected()
        {
            Formula exp = new Formula("(A1 - _B2 + 3) / 2");
            List<string> list = exp.GetVariables().ToList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("A1", list[0]);
            Assert.AreEqual("_B2", list[1]);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void ToStringWorksAsExpected()
        {
            Formula exp = new Formula("2 + x / (3 - 1)");
            Formula exp2 = new Formula("x + Y * 7", s => s.ToUpper(), s => true);
            Assert.AreEqual("2+x/(3-1)", exp.ToString());
            Assert.AreEqual("X+Y*7", exp2.ToString());
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void EqualsWorksAsExpectedSimple()
        {
            Formula exp = new Formula("2 + 9 - A1");
            Formula exp2 = new Formula("2.0+9.0-A1");
            Formula exp3 = new Formula("2.01 + 9 - A1");
            Assert.IsTrue(exp.Equals(exp2));
            Assert.IsFalse(exp.Equals(exp3));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void EqualsWorksAsExpectedComplex()
        {
            Formula exp = new Formula("2e2");
            Formula exp2 = new Formula("200");
            Formula exp3 = new Formula("2e3");
            Assert.IsTrue(exp.Equals(exp2));
            Assert.IsFalse(exp.Equals(exp3));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void EqualsWorksAsExpectedNullAndDifferentTypes()
        {
            Formula exp = new Formula("(2 + 8)/5");
            Assert.IsFalse(exp.Equals(null));
            Assert.IsFalse(exp.Equals(2));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void EqualsWorksAsExpectedDifferentSizes()
        {
            Formula exp = new Formula("(2 + 8)/5");
            Formula exp2 = new Formula("2 - 3");
            Assert.IsFalse(exp.Equals(exp2));
        }

        /// <summary>
        /// Determines whether or not the Equals method works as expected
        /// when the two formulas are identical except for one token
        /// </summary>
        [TestMethod]
        public void EqualsWorksAsExpectedOnEqualExceptForOneToken()
        {
            Formula exp = new Formula("(2 + 8)/5");
            Formula exp2 = new Formula("(2 + 8)*5");
            Assert.IsFalse(exp.Equals(exp2));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void DoubleEqualsWorksAsExpectedSimple()
        {
            Formula exp = new Formula("2 + 9 - A1");
            Formula exp2 = new Formula("2.0+9.0-A1");
            Formula exp3 = new Formula("2.01 + 9 - A1");
            Assert.IsTrue(exp == exp2);
            Assert.IsFalse(exp == exp3);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void DoubleEqualsWorksAsExpectedDifferentSizes()
        {
            Formula exp = new Formula("2 + 9 - A1");
            Formula exp2 = new Formula("5/(2 - 2)");
            Assert.IsFalse(exp == exp2);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void DoubleEqualsWorksAsExpectedOnEqualExceptForOneToken()
        {
            Formula exp = new Formula("(2 + 8)/5");
            Formula exp2 = new Formula("(2 - 8)/5");
            Assert.IsFalse(exp == exp2);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void NotEqualsWorksAsExpectedSimple()
        {
            Formula exp = new Formula("2 + 9 - A1");
            Formula exp2 = new Formula("2.0+9.0-A1");
            Formula exp3 = new Formula("3 + 9 - A1");
            Assert.IsTrue(exp != exp3);
            Assert.IsFalse(exp != exp2);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void EvaluateWorksAsExpectedDivisionByZero()
        {
            Formula exp = new Formula("2 + 9/0");
            Formula exp2 = new Formula("(7 * 2) /(10 - 10)");
            Assert.IsInstanceOfType(exp.Evaluate(s => 0), typeof(FormulaError));
            Assert.IsInstanceOfType(exp2.Evaluate(s => 0), typeof(FormulaError));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void EvaluateWorksAsExpectedSimpleFormula()
        {
            Formula exp = new Formula("7 - 9 * 2");
            Assert.IsTrue((double)exp.Evaluate(s => -1) == -11.0);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void EvaluateWorksAsExpectedComplexFormula()
        {
            Formula exp = new Formula("(44 * 6) / 2 - (_A2 * 8)");
            Assert.IsTrue((double)exp.Evaluate(s => 5.5) == 88.0);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void EvaluateWorksAsExpectedDivisionByZeroVar()
        {
            Formula exp = new Formula("20/A1");
            Formula exp2 = new Formula("(2 + 0)/0");
            Assert.AreEqual(exp.Evaluate(s => 0).GetType(), exp2.Evaluate(s => 0).GetType());
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod()]
        public void TestOperatorAfterParentheses()
        {
            Formula exp = new Formula("(1*1)-2/2");
            Assert.AreEqual(0.0, exp.Evaluate(s => 0));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod()]
        public void TestComplexAndParentheses()
        {
            Formula exp = new Formula("2+3*5+(3+4*8)*5+2");
            Assert.AreEqual(194.0, exp.Evaluate(s => 0));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod()]
        public void TestComplexNestedParensLeft()
        {
            Formula exp = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12.0, exp.Evaluate(s => 2));
        }

        /// <summary>
        /// See title
        /// </summary>
        /// <exception cref="ArgumentException"></exception> The only exception Lookup should be throwing
        [TestMethod]
        public void EvaluatorWorksAsExpectedInvalidLookup()
        {
            Formula exp = new Formula("A1 + A2");
            FormulaError error = new FormulaError("");
            Assert.IsInstanceOfType(error, exp.Evaluate(s => throw new ArgumentException()).GetType());
        }
    }
}