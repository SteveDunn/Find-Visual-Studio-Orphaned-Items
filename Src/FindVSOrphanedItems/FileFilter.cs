using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Tools.VisualStudioOrphanedFiles
{
    /// <summary>
    /// Represents a file filter, with filters specifying what to include and what to exclude.
    /// </summary>
    public class FileFilter
    {
        readonly List<string> _input;

        readonly List<string> _excludeExpressions;
        readonly List<string> _includeExpressions;
        string _patternsToInclude;
        string _patternsToExclude;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileFilter"/> class.
        /// </summary>
        public FileFilter()
        {
            _includeExpressions = new List<string>();
            _excludeExpressions = new List<string>();
            _input = new List<string>();

            updateExpressions();
        }

        /// <summary>
        /// Gets or sets the include filter.  Files matching this filter will be returned.
        /// </summary>
        /// <value>The include.</value>
        public string Include
        {
            get
            {
                return _patternsToInclude;
            }
            
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(@"value", @"Cannot set include expression as it is null.");
                }

                _patternsToInclude = value;

                updateExpressions();
            }
        }

        /// <summary>
        /// Gets or sets the input.
        /// </summary>
        /// <value>The input.</value>
        public IEnumerable<string> Input
        {
            get
            {
                return _input;
            }
            
            set
            {
                _input.Clear();
                _input.AddRange(value);
            }
        }

        /// <summary>
        /// Gets the output.
        /// </summary>
        /// <value>The output.</value>
        public IEnumerable<string> Output
        {
            get
            {
                if (_input.Count == 0)
                {
                    throw new InvalidOperationException(@"Cannot get output as no input has been specified.");
                }

                return doFilter();
            }
        }

        /// <summary>
        /// Gets or sets items to exclude.  Files matching this will not be returned.
        /// </summary>
        /// <value>The exclude.</value>
        public string Exclude
        {
            get
            {
                return _patternsToExclude;
            }
            
            set
            {
                _patternsToExclude = value;
                updateExpressions();
            }
        }

        static string wildcardStringToRegexString(string theString)
        {
            if( string.IsNullOrEmpty( theString ))
            {
                throw new ArgumentNullException(@"theString");
            }

            // escape any dots
            theString = theString.Replace(@".", @"\.");
            theString = theString.Replace(@"*", @".*");
            theString = theString.Replace(@"?", @".?");
            theString = theString + @"$";

            return theString;
        }

        IEnumerable<string> doFilter()
        {
            foreach (string s in _input)
            {
                if (shouldInclude(s) && !shouldExclude(s))
                {
                    yield return s;
                }
            }
        }

        bool shouldInclude(string theString)
        {
            foreach (string includedExpression in _includeExpressions)
            {
                var regex = new Regex(includedExpression);
                if (regex.IsMatch(theString))
                {
                    return true;
                }
            }

            return false;
        }

        void updateExpressions()
        {
            _includeExpressions.Clear();
            _excludeExpressions.Clear();

            string includes = _patternsToInclude;
            if (string.IsNullOrEmpty(includes))
            {
                includes = @"*.*";
            }

            string[] patternsToInclude = includes.Split(';');

            foreach (string patternToInclude in patternsToInclude)
            {                       
                _includeExpressions.Add(wildcardStringToRegexString(patternToInclude));
            }

            string excludes = _patternsToExclude;
            if ( string.IsNullOrEmpty( excludes ) )
            {
                return ;
            }
            
            string[] excludesArray = excludes.Split(';');

            foreach (string s in excludesArray)
            {
                _excludeExpressions.Add(wildcardStringToRegexString(s));
            }
        }

        bool shouldExclude(string theString)
        {
            foreach (string excludeExpression in _excludeExpressions)
            {
                var regex = new Regex(excludeExpression);

                if (regex.IsMatch(theString))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}