using Microsoft.VisualBasic;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace SS
{
    /// <summary>
    /// <para>
    ///     An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    ///     spreadsheet consists of an infinite number of named cells.
    /// </para>
    /// <para>
    ///     A string is a valid cell name if and only if:
    /// </para>
    /// <list type="number">
    ///      <item> it starts with one or more letters</item>
    ///      <item> it ends with one or more numbers (digits)</item>
    /// </list>   
    /// 
    /// <para>
    ///     For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    ///     satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    ///     regardless of IsValid.
    /// </para>
    ///
    /// <para>
    ///     Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    ///     must be normalized with the Normalize method before it is used by or saved in 
    ///     this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    ///     the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// </para>
    /// 
    /// <para>
    ///     A spreadsheet contains a cell corresponding to every possible cell name.  (This
    ///     means that a spreadsheet contains an infinite number of cells.)  In addition to 
    ///     a name, each cell has a contents and a value.  The distinction is important.
    /// </para>
    /// 
    /// <para>
    ///     The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    ///     contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    ///     of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// </para>
    /// 
    /// <para>
    ///     In a new spreadsheet, the contents of every cell is the empty string. Note: 
    ///     this is by definition (it is IMPLIED, not stored).
    /// </para>
    /// 
    /// <para>
    ///     The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    ///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
    ///     in the grid.)
    /// </para>
    /// 
    /// <list type="number">
    ///   <item>If a cell's contents is a string, its value is that string.</item>
    /// 
    ///   <item>If a cell's contents is a double, its value is that double.</item>
    /// 
    ///   <item>
    ///      If a cell's contents is a Formula, its value is either a double or a FormulaError,
    ///      as reported by the Evaluate method of the Formula class.  The value of a Formula,
    ///      of course, can depend on the values of variables.  The value of a variable is the 
    ///      value of the spreadsheet cell it names (if that cell's value is a double) or 
    ///      is undefined (otherwise).
    ///   </item>
    /// 
    /// </list>
    /// 
    /// <para>
    ///     Spreadsheets are never allowed to contain a combination of Formulas that establish
    ///     a circular dependency.  A circular dependency exists when a cell depends on itself.
    ///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    ///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    ///     dependency.
    /// </para>
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        //Fields
        private Dictionary<string, Cell> cells;
        private DependencyGraph spreadsheet;
        private Func<string, bool> isValid;
        private Func<string, string> normalize;
        private string version;
        private bool actualValue;

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed 
        {
            get => actualValue;
            protected set => actualValue = value;
        }

        /// <summary>
        /// Creates a new spreadsheet.
        /// </summary>
        public Spreadsheet() : this(s => true, s => s, "default")
        {
            //See Second Constructor
        }

        /// <summary>
        /// Constructs an abstract spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  
        /// </summary>
        /// 
        /// <remarks>
        ///   The variable validity test is used throughout to determine whether a string that consists of 
        ///   one or more letters followed by one or more digits is a valid cell name.  The variable
        ///   equality test should be used throughout to determine whether two variables are equal.
        /// </remarks>
        /// 
        /// <param name="isValid">   defines what valid variables look like for the application</param>
        /// <param name="normalize"> defines a normalization procedure to be applied to all valid variable strings</param>
        /// <param name="version">   defines the version of the spreadsheet (should it be saved)</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            this.cells       = new Dictionary<string, Cell>();
            this.spreadsheet = new DependencyGraph();
            this.isValid     = isValid;
            this.normalize   = normalize;
            this.version     = version;
            Changed          = true;
        }

        /// <summary>
        /// Constructs a spreadsheet from a previously existing XML file. It also
        /// records the spreadsheet's variable validity test, the variable normalizer method,
        /// and the spreadsheet's version information.
        /// </summary>
        /// <param name="fileName">     the name of the previously existing XML file</param>
        /// <param name="isValid">      the variable validity test</param>
        /// <param name="normalize">    the normalizer method</param>
        /// <param name="version">      the version information</param>
        public Spreadsheet(string fileName, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            this.cells       = new Dictionary<string, Cell>();
            this.spreadsheet = new DependencyGraph();
            this.isValid     = isValid;
            this.normalize   = normalize;
            this.version     = version;
            Changed = true;

            constructFromFile(fileName);
        }

        /// <summary>
        ///   Returns the contents (as opposed to the value) of the named cell.
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   Thrown if the name is null or invalid
        /// </exception>
        /// 
        /// <param name="name">The name of the spreadsheet cell to query</param>
        /// 
        /// <returns>
        ///   The return value should be either a string, a double, or a Formula.
        ///   See the class header summary 
        /// </returns>
        public override object GetCellContents(string name)
        {
            isNameValid(name);

            if (cells.TryGetValue(normalize(name), out Cell? cell))
                return cell.getContents();
            else
                return "";
        }

        /// <summary>
        ///   Returns the names of all non-empty cells.
        /// </summary>
        /// 
        /// <returns>
        ///     Returns an Enumerable that can be used to enumerate
        ///     the names of all the non-empty cells in the spreadsheet.  If 
        ///     all cells are empty then an IEnumerable with zero values will be returned.
        /// </returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        /// <summary>
        ///  Set the contents of the named cell to the given number.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid: non-empty/not ""
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="number"> The new contents/value </param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            isNameValid(name);

            List<string> toRecalculate = new List<string>();
            if (!cells.TryGetValue(normalize(name), out Cell? cell))
            {
                cells.Add(normalize(name), new Cell(normalize(name), number));
            }
            else
            {
                settingCellInSpreadsheet(normalize(name), number, cell);
            }
            spreadsheet.ReplaceDependees(normalize(name), new List<string>());
            return findCellsToRecalculate(toRecalculate, normalize(name));
        }

        /// <summary>
        /// The contents of the named cell becomes the text.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid/non-empty ""
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>       
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="text"> The new content/value of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            isNameValid(name);

            List<string> toRecalculate = new List<string>();
            if (!cells.TryGetValue(normalize(name), out Cell? cell))
            {
                cells.Add(normalize(name), new Cell(normalize(name), text));
            }
            else
            {
                settingCellInSpreadsheet(normalize(name), text, cell);
            }
            spreadsheet.ReplaceDependees(normalize(name), new List<string>());
            return findCellsToRecalculate(toRecalculate, normalize(name));
        }

        /// <summary>
        /// Set the contents of the named cell to the formula.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid/non empty
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name</param>
        /// <param name="formula"> The content of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            isNameValid(name);
            bool previouslyUnchanged;
            if (Changed == false)
                previouslyUnchanged = true;
            else
                previouslyUnchanged = false;

            object? oldContents = null;
            DependencyGraph oldSS = spreadsheet;
            List<string> toRecalculate = new List<string>();
            if (!cells.TryGetValue(normalize(name), out Cell? cell))
            {
                cells.Add(normalize(name), new Cell(normalize(name), formula, calculateCellvalue(formula)));
            }
            else
            {
                oldContents = cell.getContents();
                settingCellInSpreadsheet(normalize(name), formula, cell);
            }

            try
            {
                spreadsheet.ReplaceDependees(normalize(name), formula.GetVariables());
                return findCellsToRecalculate(toRecalculate, normalize(name));
            }
            catch
            {
                spreadsheet = oldSS;
                if (oldContents == null)
                    cells.Remove(name);
                else
                    if (cell != null) //won't ever be null but need to get rid of a warning
                        settingCellInSpreadsheet(normalize(name), oldContents, cell);

                if (previouslyUnchanged == true)
                    Changed = false;
            }
            throw new CircularException();
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell. 
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"></param>
        /// <returns>
        ///   Returns an enumeration, without duplicates, of the names of all cells that contain
        ///   formulas containing name.
        /// 
        ///   <para>For example, suppose that: </para>
        ///   <list type="bullet">
        ///      <item>A1 contains 3</item>
        ///      <item>B1 contains the formula A1 * A1</item>
        ///      <item>C1 contains the formula B1 + A1</item>
        ///      <item>D1 contains the formula B1 - C1</item>
        ///   </list>
        /// 
        ///   <para>The direct dependents of A1 are B1 and C1</para>
        /// 
        /// </returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            isNameValid(name);

            List<string> dependents = new List<string>();
            if (spreadsheet.HasDependents(normalize(name)))
                dependents = spreadsheet.GetDependents(normalize(name)).ToList();

            return dependents;
        }

        /// <summary>
        /// Determines if the given string is a variable
        /// </summary>
        /// <param name="token"></param> the given string
        /// <returns></returns> true or false
        private bool isVar(string token)
        {
            if (Regex.IsMatch(token, @"^[a-zA-Z](?: [a-zA-Z]|\d)*"))
                return true;
            return false;
        }

        /// <summary>
        /// Determines if the given name is valid (determined by isVar).
        /// </summary>
        /// <param name="checkName"></param> the name being checked
        /// <exception cref="InvalidNameException"></exception> If the name is not of a valid form, then 
        /// throw this exception
        private void isNameValid(string checkName)
        {
            if (!isVar(checkName))
                throw new InvalidNameException();
            if (!isValid(normalize(checkName)))
                throw new InvalidNameException();
        }

        /// <summary>
        /// Sets a cell's contents in the spreadsheet
        /// </summary>
        /// <param name="name"></param> the name of the cell whose contents are being changed
        /// <param name="contents"></param> the contents being changed to
        /// <param name="cell"></param> a reference to the cell being changed
        private void settingCellInSpreadsheet(string name, object contents, Cell cell)
        {

            cells.Remove(name);
            cell.setContents(contents);
            cells.Add(name, cell);
        }

        /// <summary>
        /// Uses the GetCellsToRecalculate method to find the cells to recalculate
        /// </summary>
        /// <param name="toRecalculate"></param> a HashSet that holds the names of all the cells to recalculate
        /// <param name="name"></param> the name of the cell to start the search of cells to recalculate from
        /// <returns></returns> toRecalculate (or throws a CircularException if a cycle is found)
        private List<string> findCellsToRecalculate(List<string> toRecalculate, string name)
        {
            toRecalculate = GetCellsToRecalculate(GetDirectDependents(name).ToHashSet()).ToList();
            toRecalculate.Add(name);
            return toRecalculate;
        }

        /// <summary>
        ///   <para>Sets the contents of the named cell to the appropriate value. </para>
        ///   <para>
        ///       First, if the content parses as a double, the contents of the named
        ///       cell becomes that double.
        ///   </para>
        ///
        ///   <para>
        ///       Otherwise, if content begins with the character '=', an attempt is made
        ///       to parse the remainder of content into a Formula.  
        ///       There are then three possible outcomes:
        ///   </para>
        ///
        ///   <list type="number">
        ///       <item>
        ///           If the remainder of content cannot be parsed into a Formula, a 
        ///           SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       </item>
        /// 
        ///       <item>
        ///           If changing the contents of the named cell to be f
        ///           would cause a circular dependency, a CircularException is thrown,
        ///           and no change is made to the spreadsheet.
        ///       </item>
        ///
        ///       <item>
        ///           Otherwise, the contents of the named cell becomes f.
        ///       </item>
        ///   </list>
        ///
        ///   <para>
        ///       Finally, if the content is a string that is not a double and does not
        ///       begin with an "=" (equal sign), save the content as a string.
        ///   </para>
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name parameter is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="SpreadsheetUtilities.FormulaFormatException"> 
        ///   If the content is "=XYZ" where XYZ is an invalid formula, throw a FormulaFormatException.
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name that is being changed</param>
        /// <param name="content"> The new content of the cell</param>
        /// 
        /// <returns>
        ///       <para>
        ///           This method returns a list consisting of the passed in cell name,
        ///           followed by the names of all other cells whose value depends, directly
        ///           or indirectly, on the named cell. The order of the list MUST BE any
        ///           order such that if cells are re-evaluated in that order, their dependencies 
        ///           are satisfied by the time they are evaluated.
        ///       </para>
        ///
        ///       <para>
        ///           For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///           list {A1, B1, C1} is returned.  If the cells are then evaluate din the order:
        ///           A1, then B1, then C1, the integrity of the Spreadsheet is maintained.
        ///       </para>
        /// </returns>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            Changed = true;
            if (Double.TryParse(content, out double number))
                return SetCellContents(name, number);
            else if (content[0] == '=')
                return SetCellContents(name, new Formula(content.Substring(1), normalize, isValid));
            else
                return SetCellContents(name, content);
        }

        /// <summary>
        ///   Look up the version information in the given file. If there are any problems opening, reading, 
        ///   or closing the file, the method should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        /// 
        /// <remarks>
        ///   In an ideal world, this method would be marked static as it does not rely on an existing SpreadSheet
        ///   object to work; indeed it should simply open a file, lookup the version, and return it.  Because
        ///   C# does not support this syntax, we abused the system and simply create a "regular" method to
        ///   be implemented by the base class.
        /// </remarks>
        /// 
        /// <exception cref="SpreadsheetReadWriteException"> 
        ///   Thrown if any problem occurs while reading the file or looking up the version information.
        /// </exception>
        /// 
        /// <param name="filename"> The name of the file (including path, if necessary)</param>
        /// <returns>Returns the version information of the spreadsheet saved in the named file.</returns>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) && reader.Name == "spreadsheet")
                        {
                            string? attribute = reader.GetAttribute("version");
                            if (attribute != null)
                                return attribute;
                            else
                                throw new ArgumentException();
                        }
                    }
                }
            }
            catch
            {
                // The XmlReader can throw many different errors while trying to read a file.
                // The try-catch block just needs to catch all of those possible errors so that the
                // SpreadsheetReadWriteExcepion can be thrown instead.
            }
            throw new SpreadsheetReadWriteException("The given spreadsheet could not be read properly. This could be because the " +
                "given file does not exist or the file is not in the proper form.");
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            bool errorThrown = false;
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";

                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", version);

                    foreach (Cell cell in cells.Values)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", cell.getName());
                        writer.WriteElementString("contents", cell.getContents().ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch
            {
                errorThrown = true;
            }
            Changed = false;
            if (errorThrown)
                throw new SpreadsheetReadWriteException("The given spreadsheet could not be written properly. This could be because the " +
                "given file does not exist or the file is not in the proper form.");
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell that we want the value of (will be normalized)</param>
        /// 
        /// <returns>
        ///   Returns the value (as opposed to the contents) of the named cell.  The return
        ///   value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </returns>
        public override object GetCellValue(string name)
        {
            isNameValid(name);

            if (cells.TryGetValue(normalize(name), out Cell? cell))
                return cell.getValue();
            return "";
        }

        /// <summary>
        /// Constructs a spreadsheet by reading from a file
        /// </summary>
        /// <param name="fileName"> the name of the file being read from</param>
        private void constructFromFile(string fileName)
        {
            bool errorThrown = false;
            try
            {
                string? name     = null;
                string? contents = null;

                using (XmlReader reader = XmlReader.Create(fileName))
                {
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) && reader.Name == "name")
                            name = reader.ReadElementContentAsString();
                        else if ((reader.NodeType == XmlNodeType.Element) && reader.Name == "contents")
                            contents = reader.ReadElementContentAsString();

                        if ((name != null) && (contents != null))
                            SetContentsOfCell(name, contents);
                    }
                }
            }
            catch
            {
                errorThrown = true;
            }

            if(errorThrown)
                throw new SpreadsheetReadWriteException("The given spreadsheet could not be read properly. This could be because the " +
                    "given file does not exist or the file is not in the proper form.");
        }

        /// <summary>
        /// Looks up the value of the cell with the given cell name
        /// </summary>
        /// <param name="name"> the name of a cell in the spreadsheet</param>
        /// <returns> the value of the cell with the given cell name</returns>
        /// <exception cref=""> Throws an argument exception if the value of the cell name is not a double</exception>
        private double lookup(string name)
        {
            object cellValue = GetCellValue(name);
            if (cellValue.GetType() == typeof(double))
                return (double)cellValue;
            else
                throw new ArgumentException();
        }

        /// <summary>
        /// Calculates the value of a cell
        /// </summary>
        /// <param name="contents">the contents of a cell (the value will be calculated from this)</param>
        /// <returns>the value of a cell</returns>
        private object calculateCellvalue(Formula contents)
        {
            object newValue = contents.Evaluate(lookup);
            return newValue;
        }

        /// <summary>
        /// This class is meant to represent a cell in a spreadsheet. A cell contains
        /// a name and the contents of the cell. The contents of a cell can be a string, a double,
        /// or a formula.
        /// </summary>
        internal class Cell
        {
            //Fields
            private string name;
            private object contents;
            private object value;

            /// <summary>
            /// Creates a new cell with a string as the contents.
            /// </summary>
            /// <param name="name"></param> the name of the cell
            /// <param name="contents"></param> the contents contained in the cell
            public Cell(string name, string contents)
            {
                this.name     = name;
                this.contents = contents;
                this.value    = contents;
            }

            /// <summary>
            /// Creates a new cell with a double as the contents
            /// </summary>
            /// <param name="name"></param> the name of the cell
            /// <param name="contents"></param> the contents contained in the cell
            public Cell(string name, double contents)
            {
                this.name     = name;
                this.contents = contents;
                this.value    = contents;
            }

            /// <summary>
            /// Creates a new cell with a formula as the contents
            /// </summary>
            /// <param name="name"></param> the name of the cell
            /// <param name="contents"></param> the contents contained in the cell
            public Cell(string name, Formula contents, object value)
            {
                this.name = name;
                this.contents = contents;
                this.value = value;
            }

            /// <summary>
            /// Sets the contents of the cell
            /// </summary>
            /// <param name="contents"></param> the new contents of the cell
            public void setContents(object contents)
            {
                this.contents = contents;
            }

            /// <summary>
            /// Retrieves the contents of the cell
            /// </summary>
            /// <returns>the contents of the cell</returns>
            public object getContents()
            {
                return contents;
            }

            /// <summary>
            /// Retrieves the value of the cell
            /// </summary>
            /// <returns> the value of the cell</returns>
            public object getValue()
            {
                return value;
            }

            /// <summary>
            /// Retrieves the name of the cell
            /// </summary>
            /// <returns>the name of the cell</returns>
            public string getName()
            {
                return name;
            }
        }
    }
}
