**Author:** Nathaniel Taylor
**Partner:** None
**Date Created:** 01/14/2023
**Course:** CS 3500, University of Utah, School of Computing
**GitHub ID:** NSwimer1321
**Repo:** https://github.com/uofu-cs3500-spring23/spreadsheet-NSwimer1321
**Date of Last Submission:** 02/02/2023 Time: 11:48 pm
**Solution:** Spreadsheet
**Copyright:** CS 3500 and Nathaniel Taylor - This work may not be copied for use in Academic Coursework


**Overview of Spreadsheet Functionality:**
This Spreadsheet program is currently capable of creating a Formula object that takes in a string (the
expression to be evaluated) as well as two delegates called normalize and isValid. The Formula object can
then split the given expression up into its individual tokens. During this process if at any point one of the
rules specified in the constructor XML comments is broken a FormulaFormatError is thrown. If the formula is
valid, then the user of the program can evaluate the formula, get a list of all the variables in the formula,
convert a Formula to a String, determine if two formulas are equal or not, and get a HashCode. A valid 
expression is defined above the Evaluate method in the FormulaEvaluator. The Spreadsheet program is also capable 
of identifying dependent and dependee relationships. This means that the Spreadsheet knows which "cells" must 
be calculated first and which ones can be calculated later ie. which "cells" depend on other "cells" to be 
calculated. NOTE: The Spreadsheet is not capable of identifying cycles, so the "cell" A1 could have the formula 
A1 + A2 in this iteration of the Spreadsheet.


**Assignment 01 Estimated Hours Worked:** 10
**Assignment 01 Hours Worked:** 10

**Assignment 02 Estimated Hours Worked:** 14
**Assignment 02 Hours Worked:** 11

**Assignment 03 Estimated Hours Worked:** 10
**Assignment 03 Hours Worked:** 11

**Total Hours Worked:** 32






