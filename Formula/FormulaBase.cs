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
namespace SpreadsheetUtilities
{
    public class FormulaBase
    {

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was 
        ///passed tothe constructor.) For example, if L("x") is 2, L("X") is 4, and N is a 
        /// method that converts all the letters in a string to upper case:
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11 new Formula("x+7").Evaluate(L) is 9
        /// Given a variable symbol as its parameter, lookup returns the variable's value
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// If no undefined variables or divisions by zero are encountered when evaluating
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.
        /// The Reason property of the FormulaError should have a meaningful explanation.
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {

        }
    }
}