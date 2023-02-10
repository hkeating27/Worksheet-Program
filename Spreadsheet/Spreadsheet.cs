using Microsoft.VisualBasic;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SS
{
    /// <summary>
    /// This class represents a spreadsheet using the API
    /// specified in the AbstractSpreadsheet class. The class
    /// allows the user to retrieve the contents of any cell in the
    /// spreadsheet, set the contents of any cell in the spreadsheet,
    /// get a list of all of the nonempty cells in the spreadsheet, and
    /// get the direct dependents of a cell.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        //Fields
        private Dictionary<string, Cell> cells;
        private DependencyGraph spreadsheet;

        /// <summary>
        /// Creates a new spreadsheet.
        /// </summary>
        public Spreadsheet()
        {
            cells = new Dictionary<string, Cell>();
            spreadsheet = new DependencyGraph();
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

            if (cells.TryGetValue(name, out Cell? cell))
                return cell.getContents();
            else
                return "";
        }

        /// <summary>
        /// Returns an Enumerable that can be used to enumerates 
        /// the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        /// <summary>
        ///  Set the contents of the named cell to the given number.  
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="number"> The new contents/value </param>
        /// 
        /// <returns>
        ///   <para>
        ///      The method returns a set consisting of name plus the names of all other cells whose value depends, 
        ///      directly or indirectly, on the named cell.
        ///   </para>
        /// 
        ///   <para>
        ///      For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///      set {A1, B1, C1} is returned.
        ///   </para>
        /// </returns>
        public override ISet<string> SetCellContents(string name, double number)
        {
            isNameValid(name);

            HashSet<string> toRecalculate = new HashSet<string>();
            if (!cells.TryGetValue(name, out Cell? cell))
            {
                cells.Add(name, new Cell(name, number));
            }
            else
            {
                settingCellInSpreadsheet(name, number, cell);
            }
            spreadsheet.ReplaceDependees(name, new List<string>());
            return findCellsToRecalculate(toRecalculate, name);
        }

        /// <summary>
        /// The contents of the named cell becomes the text.  
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If text is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="text"> The new content/value of the cell</param>
        /// 
        /// <returns>
        ///   The method returns a set consisting of name plus the names of all 
        ///   other cells whose value depends, directly or indirectly, on the 
        ///   named cell.
        /// 
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned.
        ///   </para>
        /// </returns>
        public override ISet<string> SetCellContents(string name, string text)
        {
            hasExceptions(text, name);

            HashSet<string> toRecalculate = new HashSet<string>();
            if (!cells.TryGetValue(name, out Cell? cell))
            {
                cells.Add(name, new Cell(name, text));
            }
            else
            {
                settingCellInSpreadsheet(name, text, cell);
            }
            spreadsheet.ReplaceDependees(name, new List<string>());
            return findCellsToRecalculate(toRecalculate, name);
        }

        /// <summary>
        /// Set the contents of the named cell to the formula.  
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If formula parameter is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
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
        ///     The method returns a Set consisting of name plus the names of all other 
        ///     cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///   <para> 
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned.
        ///   </para>
        /// 
        /// </returns>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            hasExceptions(formula, name);

            object? oldContents = null;
            DependencyGraph tempSS = spreadsheet;
            HashSet<string> toRecalculate = new HashSet<string>();
            if (!cells.TryGetValue(name, out Cell? cell))
            {
                cells.Add(name, new Cell(name, formula));
            }
            else
            {
                oldContents = cell.getContents();
                settingCellInSpreadsheet(name, formula, cell);
            }

            try
            {
                spreadsheet.ReplaceDependees(name, formula.GetVariables());
                return findCellsToRecalculate(toRecalculate, name);
            }
            catch
            {
                spreadsheet = tempSS;
                if (oldContents == null)
                    cells.Remove(name);
                else
                    if (cell != null) //won't ever be null but need to get rid of a warning
                        settingCellInSpreadsheet(name, oldContents, cell);
            }
            throw new CircularException();
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell. 
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If the name is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
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
            hasExceptions(name, name);

            HashSet<string> dependents = new HashSet<string>();
            if (spreadsheet.HasDependents(name))
                dependents = spreadsheet.GetDependents(name).ToHashSet();

            return dependents;
        }

        /// <summary>
        /// Determines if the given string is a variable
        /// </summary>
        /// <param name="token"></param> the given string
        /// <returns></returns> true or false
        private bool isVar(string token)
        {
            if (Regex.IsMatch(token, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
                return true;
            return false;
        }

        /// <summary>
        /// Determines if the object is null and if the given name
        /// is valid (determined by isVar).
        /// </summary>
        /// <param name="checkNull"></param> the object being checked
        /// <param name="checkName"></param> the name being checked
        /// <exception cref="ArgumentNullException"></exception> if the object is null, then throw this exception
        /// <exception cref="InvalidNameException"></exception> if the name is not of a valid form, then
        /// throw this exception
        private void hasExceptions(object checkNull, string checkName)
        {
            if (checkNull is null)
                throw new ArgumentNullException();
            if (!isVar(checkName))
                throw new InvalidNameException();
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
        private HashSet<string> findCellsToRecalculate(HashSet<string> toRecalculate, string name)
        {
            toRecalculate = GetCellsToRecalculate(GetDirectDependents(name).ToHashSet()).ToHashSet();
            toRecalculate.Add(name);
            return toRecalculate;
        }
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

        /// <summary>
        /// Creates a new cell with a string as the contents.
        /// </summary>
        /// <param name="name"></param> the name of the cell
        /// <param name="contents"></param> the contents contained in the cell
        public Cell(string name, string contents)
        {
            this.name = name;
            this.contents = contents;
        }

        /// <summary>
        /// Creates a new cell with a double as the contents
        /// </summary>
        /// <param name="name"></param> the name of the cell
        /// <param name="contents"></param> the contents contained in the cell
        public Cell(string name, double contents)
        {
            this.name = name;
            this.contents = contents;
        }

        /// <summary>
        /// Creates a new cell with a formula as the contents
        /// </summary>
        /// <param name="name"></param> the name of the cell
        /// <param name="contents"></param> the contents contained in the cell
        public Cell(string name, Formula contents)
        {
            this.name = name;
            this.contents = contents;
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
    }
}
