using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    /// <summary>
    /// This is a test class designed to hold all of the
    /// unit tests for the Spreadsheet class.
    /// </summary>
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsDoubleWorksAsExceptedIllegalName()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("_1A", "98");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsWorksStringAsExceptedIllegalName()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("%7", "invalid");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsFormulaWorksAsExceptedIllegalName()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("*1_", "=A1");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsDoubleInvalidName()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => false, s=> s, "default");
            ss.SetContentsOfCell("B2", "23");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsStringInvalidName()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => false, s => s, "default");
            ss.SetContentsOfCell("J6", "Hello");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContentsFormulaInvalidName()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => false, s => s, "default");
            ss.SetContentsOfCell("O0", "=8 + 9");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void SetCellContentsWorksAsExceptedEmptySS()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.AreEqual("", ss.GetCellContents("A1"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void SetCellContentsWorksAsExceptedSimple()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "text");
            ss.SetContentsOfCell("B1", "32");
            ss.SetContentsOfCell("A3", "=3+2");
            Assert.AreEqual("text", ss.GetCellContents("A1"));
            Assert.AreEqual(32.0, ss.GetCellContents("B1"));
            Assert.IsTrue(ss.GetCellContents("A3").Equals(new Formula("3 + 2")));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void SetCellContentsWorksAsExceptedWhenResettingExistingCell()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "text");
            ss.SetContentsOfCell("B1", "32");
            ss.SetContentsOfCell("A1", "= 3 + 2");
            ss.SetContentsOfCell("B1", "A1");
            Assert.IsTrue(ss.GetCellContents("A1").Equals(new Formula("3 + 2")));
            Assert.AreEqual("A1", ss.GetCellContents("B1"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void SetCellContentsWorksAsExceptedChangingFormulaToDouble()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B2 - (J5 * 7)");
            ss.SetContentsOfCell("B2", "97");
            HashSet<string> dependents = ss.SetContentsOfCell("J5", "=7").ToHashSet();
            Assert.IsTrue(dependents.Count == 2);
            Assert.IsTrue(dependents.Contains("A1"));

            ss.SetContentsOfCell("A1", "74");
            HashSet<string> newDependents = ss.SetContentsOfCell("J5", "=7").ToHashSet();
            Assert.IsTrue(newDependents.Count == 1);
            Assert.IsFalse(newDependents.Contains("A1"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void SetCellContentsWorksAsExceptedChangingFormulaToString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B2 - (J5 * 7)");
            ss.SetContentsOfCell("B2", "=J5 / 2");
            HashSet<string> dependents = ss.SetContentsOfCell("J5", "=7").ToHashSet();
            Assert.IsTrue(dependents.Count == 3);
            Assert.IsTrue(dependents.Contains("A1"));
            Assert.IsTrue(dependents.Contains("B2"));

            ss.SetContentsOfCell("A1", "new");
            ss.SetContentsOfCell("B2", " also new");
            HashSet<string> newDependents = ss.SetContentsOfCell("J5", "=7").ToHashSet();
            Assert.IsTrue(newDependents.Count == 1);
            Assert.IsFalse(newDependents.Contains("A1"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsWorksAsExceptedCircularDependencySimple()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("Z1", "=Z1 + 9");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsWorksAsExceptedCircularDependencyComplex()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("Z1", "=A2 + 7");
            ss.SetContentsOfCell("A2", "=H87");
            ss.SetContentsOfCell("H87", "=XYZ9 / 2");
            ss.SetContentsOfCell("XYZ9", "=Z1 * (8 - 3)");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContentsWorksAsExceptedCircularDependencyDoesNotReplaceContents()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("Z1", "88");
            ss.SetContentsOfCell("Z1", "=Z1 + 9");
            Assert.AreEqual(88, ss.GetCellContents("Z1"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContentsWorksAsExceptedInvalidName()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("Z1", "valid");
            ss.GetCellContents("1Z");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetCellContentsWorksAsExceptedEmpty()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.AreEqual("", ss.GetCellContents("Z33"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetCellContentsWorksAsExceptedValidName()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("Z1", "1");
            ss.SetContentsOfCell("H7", "2");
            ss.SetContentsOfCell("AA62", "=3");
            Assert.AreEqual(1.0, ss.GetCellContents("Z1"));
            Assert.AreEqual(2.0, ss.GetCellContents("H7"));
            Assert.IsTrue(ss.GetCellContents("AA62").Equals(new Formula("3")));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetNamesOfAllNonemptyCellsWorksAsExpectedSimple()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("AA1", "35");
            ss.SetContentsOfCell("AA2", "90");
            ss.SetContentsOfCell("BB1", "full");
            List<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells().ToList();
            Assert.AreEqual("AA1", nonEmptyCells[0]);
            Assert.AreEqual("AA2", nonEmptyCells[1]);
            Assert.AreEqual("BB1", nonEmptyCells[2]);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetNamesOfAllNonemptyCellsWorksAsExpectedEmpty()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            List<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells().ToList();
            Assert.IsTrue(nonEmptyCells.Count == 0);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetNamesOfAllNonemptyCellsWorksAsExpectedTechnicallyNotEmpty()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("AA1", "0");
            ss.SetContentsOfCell("AA2", "=0");
            List<string> nonEmptyCells = ss.GetNamesOfAllNonemptyCells().ToList();
            Assert.AreEqual("AA1", nonEmptyCells[0]);
            Assert.AreEqual("AA2", nonEmptyCells[1]);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetCellValueOfTypeDouble()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("B2", "23");
            Assert.AreEqual(23.0, ss.GetCellValue("B2"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetCellValueOfTypeString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("J2", "hello.txt");
            Assert.AreEqual("hello.txt", ss.GetCellValue("J2"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetCellValueOfTypeFormula()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "10");
            ss.SetContentsOfCell("I8", "=A1 - 2");
            Assert.AreEqual(8.0, ss.GetCellValue("I8"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void GetCellValueOfTypeFormulaError()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A3", "=B2/0");
            Assert.AreEqual(typeof(FormulaError), ss.GetCellValue("A3").GetType());
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void SavedVersionWorksAsExpectedSimpleSpreadsheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "v1");
            ss.SetContentsOfCell("A1", "87");
            ss.SetContentsOfCell("A2", "Hello");
            ss.Save("test.xml");
            Assert.AreEqual("v1", ss.GetSavedVersion("test.xml") );
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void FourthConstructorWorksAsExpectedSimpleSpreadsheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "v1");
            ss.SetContentsOfCell("A1", "87");
            ss.SetContentsOfCell("A2", "Hello");
            ss.Save("test2.xml");

            AbstractSpreadsheet ss2 = new Spreadsheet("test2.xml", s => true, s => s, "v1");
            Assert.AreEqual(87.0, ss2.GetCellContents("A1"));
            Assert.AreEqual("Hello", ss2.GetCellContents("A2"));
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void SetCellContentsSetsChanged()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "v1");
            ss.SetContentsOfCell("A2", "=A1 / 3");
            Assert.IsTrue(ss.Changed);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void ChangedWorksAsexpectedWithFormulas()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A2", "=A1 / 3");
            Assert.IsTrue(ss.Changed);
            ss.Save("XML.xml");
            Assert.IsFalse(ss.Changed);
            ss.SetContentsOfCell("B1", "=A3 / 3");
            Assert.IsTrue(ss.Changed);
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionWorksAsExpectedInvalidFileFormat()
        {
            AbstractSpreadsheet ss = new Spreadsheet("invalid.xml", s => true, s => s, "v1");
            ss.GetSavedVersion("invalid.xml");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionWorksAsExpectedNullVersion()
        {
            AbstractSpreadsheet ss = new Spreadsheet("invalid2.xml", s => true, s => s, "v1");
            ss.GetSavedVersion("invalid2.xml");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersionWorksAsExpectedInvalidFile()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "v1");
            ss.GetSavedVersion("doesNotExist.xml");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveThrowsAsExpected()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "v1");
            ss.Save("/utter/nonsense.xml");
        }

        /// <summary>
        /// See title
        /// </summary>
        [TestMethod]
        public void SetContentsRecalculatesAsExpected()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "90");
            ss.SetContentsOfCell("B1", "=A1 - 2");
            ss.SetContentsOfCell("C1", "=B1 / 4");

            List<string> answers = new List<string>() { "A1", "B1", "C1" };
            List<string> results = ss.SetContentsOfCell("A1", "85").ToList();
            Assert.AreEqual(answers.Count, results.Count);
            for(int i = 0; i < answers.Count; i++) 
            {
                Assert.AreEqual(answers[i], results[i]);
            }
        }
    }
}