// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
// (Clarified meaning of dependent and dependee.)
// (Clarified names in solution/project structure.)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings. Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a
    /// set, and the element is already in the set, the set remains unchanged.
    /// Given a DependencyGraph DG:
    /// (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    /// (The set of things that depend on s)       
    /// (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    /// (The set of things that s depends on) 
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    /// dependents("a") = {"b", "c"}
    /// dependents("b") = {"d"}
    /// dependents("c") = {}
    /// dependents("d") = {"d"}
    /// dependees("a") = {}
    /// dependees("b") = {"a"}
    /// dependees("c") = {"a"}
    /// dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        // Fields
        private Dictionary<string, List<string>> dependents;
        private Dictionary<string, List<string>> dependees;
        private int orderedPairs; // The total number of ordered pairs in the dependency graph

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, List<string>>();
            dependees = new Dictionary<string, List<string>>();
            orderedPairs = 0;
        }

        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get 
            {
                return orderedPairs; 
            }
        }

        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this: dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get 
            {
                if(dependees.TryGetValue(s, out List<string>? list))
                    return list.Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (!dependents.TryGetValue(s, out List<string>? list))
                return false;
            else if (list.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (!dependees.TryGetValue(s, out List<string>? list))
                return false;
            else if (list.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            dependents.TryGetValue(s, out List<string>? list);
            return list;
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            dependents.TryGetValue(s, out List<string>? list);
            return list;
        }

        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// <para>This should be thought of as:t depends on s</para>
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            dependents.TryGetValue(s, out List<string>? list);
            dependees.TryGetValue(t, out List<string>? list2);
            if (list == null && list2 == null)
            {
                dependents.Add(s, new List<string> { t });
                dependees.Add(t, new List<string> { s });
                orderedPairs++;
            }
            else if ((list != null && dependents.ContainsKey(s) && list.Contains(t)) &&
                      list2 != null && dependees.ContainsKey(t) && list.Contains(s))
            {
                list.Add(t);
                list2.Add(s);
                dependents.Add(s, list);
                dependees.Add(t, list2);
                orderedPairs++;
            }
        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            dependents.TryGetValue(s, out List<string>? list);
            dependees.TryGetValue(t, out List<string>? list2);
            if ((list != null && dependents.ContainsKey(s) && list.Contains(t)) &&
                      list2 != null && dependees.ContainsKey(t) && list.Contains(s))
            {
                dependents.Remove(s);
                dependees.Remove(t);
                list.Remove(t);
                list2.Remove(s);
                dependents.Add(s, list);
                dependees.Add(t, list2);
                orderedPairs--;
            }
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            dependents.Remove(s);
            dependents.Add(s, newDependents.ToList());
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            dependees.Remove(s);
            dependees.Add(s, newDependees.ToList());
        }
    }
}