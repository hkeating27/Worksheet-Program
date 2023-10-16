**Author:** Hunter Keating
**Date Created:** 01/14/2023
**Course:** CS 3500, University of Utah, School of Computing
**GitHub ID:** NSwimer1321
**Repo:** https://github.com/uofu-cs3500-spring23/spreadsheet-NSwimer1321
**Date of Last Submission:** 02/17/2023 Time: 2:40 pm
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
calculated. The Spreadsheet is also capable of keeping track of all of the cells containing a string, double, or
formula. The Spreadsheet can also keep track of the direct relationships between cells and can find the indirect
relationships between cells. The Spreadsheet can also set the contents of a cell in the Spreadsheet to
a string, double, or formula. The Spreadsheet can also set the value of a cell to a string, double, or FormulaError.
Lastly, the Spreadsheet can create a spreadsheet from a file, it can Save a spreadsheet to a file, and it can find the
version information from a file (if that file is a spreadsheet).

**Examples of Good Software Practices(GSP):**
I am getting much better at good seperation of concerns. It's something that took me a bit to remember and start
doing again when the semester started, but now I think I am doing well at it. An example in my code would be in
the Spreadsheet assignment with my IsVar and HasExceptions helper methods. The only place IsVar is ever used is
in the HasExceptions method, but HasExceptions doesn't need to know what makes a valid name, so rather than
put the IsVar code into the HasExceptions method I just made a new helper method and put it in there.

I also have improved my testing strategies. This is most evident in my assignment four unit tests. I use several 
different inputs, my tests hit almost every line of code (there are a few lines that are impossible to hit by
design of the assignment), the names of my tests do a good job of describing the test, and every test tests
every possible way the thing it is testing could fail.

**Other Good Software Practices:**
-Regression Testing
-Abstraction


**Assignment 01 Estimated Hours Worked:** 10
**Assignment 01 Hours Worked:** 10

**Assignment 02 Estimated Hours Worked:** 14
**Assignment 02 Hours Worked:** 11

**Assignment 03 Estimated Hours Worked:** 10
**Assignment 03 Hours Worked:** 11

**Assignment 04 Estimated Hours Worked:** 10
**Assignment 04 Hours Worked:** 10.5

**Assignment 05 Estimated Hours Worked:** 12
**Assignment 05 Hours Worked:** 13

**Total Hours Worked:** 55.5

**Assign 05 Time Spent Towards Completion:** 6.5
**Assign 05 Time Spent Debugging:** 5
**Assign 05 Time Spent Learning Techniques:** 1.5




