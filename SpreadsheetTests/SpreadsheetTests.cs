using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsDoubleWorksAsExceptedInvaidName()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("9_A", 98);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsStringWorksAsExceptedInvalidName()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("%7", "invalid");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsFormulaWorksAsExceptedInvalidName()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("*1_", new Formula("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentsStringWorksAsExceptedNullArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            string? str = null;
            ss.SetCellContents("A2", str);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContentsFormulaWorksAsExceptedNullArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formula? formula = null;
            ss.SetCellContents("A2", formula);
        }

        [TestMethod]
        public void SetCellContentsWorksAsExceptedEmptySS()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.AreEqual("", ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void SetCellContentsWorksAsExceptedSimple()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("A1", "text");
            ss.SetCellContents("B1", 32);
            ss.SetCellContents("A3", new Formula("3 + 2"));
            Assert.AreEqual("text", ss.GetCellContents("A1"));
            Assert.AreEqual(32.0, ss.GetCellContents("B1"));
            Assert.IsTrue(ss.GetCellContents("A3").Equals(new Formula("3 + 2")));
        }

        [TestMethod]
        public void SetCellContentsWorksAsExceptedWhenResettingExistingCell()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("A1", "text");
            ss.SetCellContents("B1", 32);
            ss.SetCellContents("A1", new Formula("3 + 2"));
            ss.SetCellContents("B1", "A1");
            Assert.IsTrue(ss.GetCellContents("A1").Equals(new Formula("3 + 2")));
            Assert.AreEqual("A1", ss.GetCellContents("B1"));
        }

        [TestMethod]
        public void SetCellContentsWorksAsExceptedChangingFormulaToDouble()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("A1", new Formula("B2 - (J5 * 7)"));
            ss.SetCellContents("B2", 97);
            HashSet<string> dependents = ss.SetCellContents("J5", new Formula("7")).ToHashSet();
            Assert.IsTrue(dependents.Count == 2);
            Assert.IsTrue(dependents.Contains("A1"));

            ss.SetCellContents("A1", 74);
            HashSet<string> newDependents = ss.SetCellContents("J5", new Formula("7")).ToHashSet();
            Assert.IsTrue(newDependents.Count == 1);
            Assert.IsFalse(newDependents.Contains("A1"));
        }

        [TestMethod]
        public void SetCellContentsWorksAsExceptedChangingFormulaToString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("A1", new Formula("B2 - (J5 * 7)"));
            ss.SetCellContents("B2", new Formula("J5 / 2"));
            HashSet<string> dependents = ss.SetCellContents("J5", new Formula("7")).ToHashSet();
            Assert.IsTrue(dependents.Count == 3);
            Assert.IsTrue(dependents.Contains("A1"));
            Assert.IsTrue(dependents.Contains("B2"));

            ss.SetCellContents("A1", "new");
            ss.SetCellContents("B2", " also new");
            HashSet<string> newDependents = ss.SetCellContents("J5", new Formula("7")).ToHashSet();
            Assert.IsTrue(newDependents.Count == 1);
            Assert.IsFalse(newDependents.Contains("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsWorksAsExceptedCircularDependencySimple()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("Z1", new Formula("Z1 + 9"));
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsWorksAsExceptedCircularDependencyComplex()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("Z1", new Formula("A2 + 7"));
            ss.SetCellContents("A2", new Formula("H87"));
            ss.SetCellContents("H87", new Formula("XYZ9 / 2"));
            ss.SetCellContents("XYZ9", new Formula("Z1 * (8 - 3)"));
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsWorksAsExceptedCircularDependencyDoesNotReplaceContents()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("Z1", 88);
            ss.SetCellContents("Z1", new Formula("Z1 + 9"));
            Assert.AreEqual(88, ss.GetCellContents("Z1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsWorksAsExceptedInvalidName()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("Z1", "valid");
            ss.GetCellContents("1Z");
        }

        [TestMethod]
        public void GetCellContentsWorksAsExceptedEmpty()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.AreEqual("", ss.GetCellContents("Z33"));
        }

        [TestMethod]
        public void GetCellContentsWorksAsExceptedValidName()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("Z1", 1);
            ss.SetCellContents("H7", "2");
            ss.SetCellContents("AA62", new Formula("3"));
            Assert.AreEqual(1.0, ss.GetCellContents("Z1"));
            Assert.AreEqual("2", ss.GetCellContents("H7"));
            Assert.IsTrue(ss.GetCellContents("AA62").Equals(new Formula("3")));
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCellsWorksAsExpectedSimple()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("AA1", 35);
            ss.SetCellContents("AA2", 90);
            ss.SetCellContents("BB1", "full");
            List<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells().ToList();
            Assert.AreEqual("AA1", nonEmptyCells[0]);
            Assert.AreEqual("AA2", nonEmptyCells[1]);
            Assert.AreEqual("BB1", nonEmptyCells[2]);
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCellsWorksAsExpectedEmpty()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            List<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells().ToList();
            Assert.IsTrue(nonEmptyCells.Count == 0);
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCellsWorksAsExpectedTechnicallyNotEmpty()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetCellContents("AA1", 0);
            ss.SetCellContents("AA2", new Formula("0"));
            ss.SetCellContents("BB1", "");
            List<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells().ToList();
            Assert.AreEqual("AA1", nonEmptyCells[0]);
            Assert.AreEqual("AA2", nonEmptyCells[1]);
            Assert.AreEqual("BB1", nonEmptyCells[2]);
        }
    }
}