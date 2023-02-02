using SpreadsheetUtilities;
using static SpreadsheetUtilities.Formula;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorEmptyFormula()
        {
            Formula exp = new Formula("");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorInvalidFirstToken()
        {
            Formula exp = new Formula("+ 98");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorInvalidLastToken()
        {
            Formula exp = new Formula("(87 - A1) + 9(");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorIllegalParenthesisUse()
        {
            Formula exp = new Formula("(9))"); 
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorIllegalFollowingOperator()
        {
            Formula exp1 = new Formula("(+8)");
            Formula exp2 = new Formula("-*8)");
            Formula exp3 = new Formula("+/8)");
            Formula exp4 = new Formula("*)8)");
            Formula exp5 = new Formula("/+8)");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorIllegalFollowingNumOrVar()
        {
            Formula exp1 = new Formula("8 7 - 9");
            Formula exp2 = new Formula("A1(8 + 2)");
            Formula exp3 = new Formula("(9 - 8)2");
            Formula exp4 = new Formula("7 Z6 * 2");
            Formula exp5 = new Formula("(4 + 5) Z3 / 2");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FirstConstructorMissingParenthesis()
        {
            Formula exp = new Formula("((9)");
        }

        [TestMethod]
        public void GetVariablesWorksAsExpected()
        {
            Formula exp = new Formula("(A1 - _B2 + 3) / 2");
            List<string> list = exp.GetVariables().ToList();
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("A1", list[0]);
            Assert.AreEqual("_B2", list[1]);
        }

        [TestMethod]
        public void ToStringWorksAsExpected()
        {
            Formula exp = new Formula("2 + x / (3 - 1)");
            Formula exp2 = new Formula("x + Y * 7", s => s.ToUpper(), s => true);
            Assert.AreEqual("2+x/(3-1)", exp.ToString());
            Assert.AreEqual("X+Y*7", exp2.ToString());
        }

        [TestMethod]
        public void EqualsWorksAsExpectedSimple()
        {
            Formula exp = new Formula("2 + 9 - A1");
            Formula exp2 = new Formula("2.0+9.0-A1");
            Formula exp3 = new Formula("2.01 + 9 - A1");
            Assert.IsTrue(exp.Equals(exp2));
            Assert.IsFalse(exp.Equals(exp3));
        }

        [TestMethod]
        public void EqualsWorksAsExpectedComplex()
        {
            Formula exp = new Formula("2e2");
            Formula exp2 = new Formula("2.0+9.0-A1");
            Formula exp3 = new Formula("2.01 + 9 - A1");
            Assert.IsTrue(exp.Equals(exp2));
            Assert.IsFalse(exp.Equals(exp3));
        }

        [TestMethod]
        public void DoubleEqualsWorksAsExpectedSimple()
        {
            Formula exp = new Formula("2 + 9 - A1");
            Formula exp2 = new Formula("2.0+9.0-A1");
            Formula exp3 = new Formula("2.01 + 9 - A1");
            Assert.IsTrue(exp == exp2);
            Assert.IsFalse(exp == exp3);
        }

        [TestMethod]
        public void NotEqualsWorksAsExpectedSimple()
        {
            Formula exp = new Formula("2 + 9 - A1");
            Formula exp2 = new Formula("2.0+9.0-A1");
            Formula exp3 = new Formula("2.01 + 9 - A1");
            Assert.IsTrue(exp != exp3);
            Assert.IsFalse(exp != exp2);
        }
    }
}