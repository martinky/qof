using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.Shell.Interop;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading.Tasks;

namespace QuickOpenFile
{
    public sealed class SearchEngine : IDisposable
    {
        private Settings settings;

        public SearchEngine(QuickOpenFileControl notifyControl)
        {
            this.notifyControl = notifyControl;
            solutionFiles = new List<SolutionFile>();
            solutionReader = new SolutionReader();
        }

        public void SetPackage(QuickOpenFilePackage package)
        {
            this.settings = package.Settings;
        }

        /// <summary>
        /// Orders a new search of the solution. The UI is notified
        /// asynchronously when the search is finished.
        /// </summary>
        /// <param name="query">Search expression.</param>
        /// <param name="options">Search options.</param>
        /// <param name="index">If true, the solution is indexed first.</param>
        public void Search(string query)
        {
            if (query == null)
                return;

            int seq = GetSequence(1);
            task = Task.Factory.StartNew(() => SearchInternal(query, seq));
            
        }

        public void Index(Action andThen)
        {
            Task.Factory.StartNew(() => IndexSolution()).ContinueWith((t) => andThen());
        }

        private void IndexSolution()
        {
            NotifyStatusText("Indexing...");
            Debug.Print("QOF.SearchEngine: Indexing...");
            var stopwatch = Stopwatch.StartNew();
            solutionFiles = solutionReader.GetSolutionFiles(notifyControl.GetSolution(), settings);
            initialIndexingComplete.Set();
            Debug.Print("QOF.SearchEngine: Indexed " + solutionFiles.Count + " solution files in " + stopwatch.Elapsed + ".");
            NotifyStatusText("Ready");
        }

        private void SearchInternal(string query, int sequence)
        {
            initialIndexingComplete.WaitOne();

            if (GetSequence() != sequence)
            {
                Debug.Print("QOF.SearchEngine: Canceling outdated search " + sequence);
                return;
            }

            IEnumerable<SolutionFile> result = null;

            var stopwatch = Stopwatch.StartNew();
            try
            {
                if (String.IsNullOrWhiteSpace(query))
                {
                    throw new ArgumentNullException("Empty search expression.");
                }

                var positiveTerms = MakeRegexes(query);
                var negativeTerms = settings.IgnorePatterns.SelectMany(nq => MakeRegexes(nq));

                Debug.Print("QOF.SearchEngine: Search " + sequence + " for: '" + String.Join(" ", positiveTerms) +
                    "' " + String.Join(" ", negativeTerms.Select(re => "NOT '" + re.ToString() + "'")));
                result = solutionFiles
                    .Where(sr => positiveTerms.Any(r => r.IsMatch(sr.Name)) && !negativeTerms.Any(r => r.IsMatch(sr.Name)))
                    .Distinct(new DistinctByFilePath())
                    //TODO: show open files first
                    .OrderBy(item => item.Name).ThenBy(i => i.Project)
                    // force to execute the query now (in try block)
                    .ToArray();

                if (result.Count() > 0)
                {
                    NotifyStatusText("Ready");
                }
                else
                {
                    NotifyStatusText("No files match the expression.");
                }
            }
            catch (ArgumentNullException)
            {
                NotifyStatusText("Ready");
            }
            catch (ArgumentException)
            {
                NotifyStatusText("Invalid search expression.");
            }

            if (GetSequence() != sequence)
            {
                Debug.Print("QOF.SearchEngine: Canceling outdated search " + sequence);
                return;
            }

            Debug.Print("QOF.SearchEngine: Search " + sequence + " Found " + (result == null ? 0 : result.Count()) + " matching files in " + stopwatch.Elapsed + ".");

            NotifyResults(result);
        }

        private IEnumerable<Regex> MakeRegexes(string query)
        {
            // replace internal whitespace with single * wildcards
            if (settings.SpaceAsWildcard)
            {
                query = ReplaceWhitespaceWithWildcard(query);
            }

            // escape control characters (search 'as is' for all other characters than ?, *)
            query = EscapeRegularExpression(query);

            // replace ? and * wildcards in the string with proper regex syntax
            query = query.Trim().Replace('?', '.').Replace("*", ".*");

            // match pattern at the begining
            if (!settings.SearchInTheMiddle)
            {
                query = "^" + query;
            }

            var regexOptions = RegexOptions.IgnoreCase;

            // search using camel case
            if (settings.UseCamelCase)
            {
                yield return new Regex(MakeCamelCaseExpression(query));

                if (char.IsUpper(query.FirstOrDefault()))
                {
                    regexOptions = RegexOptions.None;
                }
            }

            yield return new Regex(query, regexOptions);
        }

        private string MakeCamelCaseExpression(string query)
        {
            // makes an expression that matches any number of lower case letters
            // before the second (and every further) capital letter
            string s = "";
            bool wasUpperChar = false;
            for (int i = 0; i < query.Length; i++)
            {
                char c = query[i];
                if (Char.IsUpper(c))
                {
                    if (wasUpperChar)
                        s += "[a-z]*";
                    wasUpperChar = true;
                }
                else if (Char.IsLower(c))
                { }
                else
                    wasUpperChar = false;
                s += c;
            }
            return s;
        }

        private static readonly string charsToEscape = @"/+|(){}[].^$#";
        private string EscapeRegularExpression(string query)
        {
            string s = "";
            for (int i = 0; i < query.Length; i++)
            {
                char c = query[i];
                if (charsToEscape.Contains(c) || Char.IsWhiteSpace(c))
                    s += "\\" + c;
                else
                    s += c;
            }
            return s;
        }

        // Changes internal whitespace blocks for * wildard character.
        private string ReplaceWhitespaceWithWildcard(string query)
        {
            string s = "";
            bool wasWhitespace = false;
            for (int i = 0; i < query.Length; i++)
            {
                if (Char.IsWhiteSpace(query[i]))
                {
                    if (!wasWhitespace)
                        s += "*";
                    wasWhitespace = true;
                }
                else
                {
                    s += query[i];
                    wasWhitespace = false;
                }
            }
            return s;
        }

        private void NotifyStatusText(string text)
        {
            notifyControl.BeginInvoke((Action)(() => notifyControl.SetStatusText(text)));
        }

        private void NotifyResults(IEnumerable<SolutionFile> results)
        {
            notifyControl.BeginInvoke((Action)(() => notifyControl.ShowResults(results)));
        }

        private int GetSequence(int increment = 0)
        {
            Monitor.Enter(this);
            sequence = sequence + increment;
            int seq = sequence;
            Monitor.Exit(this);
            return seq;
        }

        class DistinctByFilePath : IEqualityComparer<SolutionFile>
        {
            public bool Equals(SolutionFile x, SolutionFile y)
            {
                if (x == null ^ y == null)
                {
                    return false;
                }

                return x.FilePath.Equals(y.FilePath, StringComparison.InvariantCultureIgnoreCase);
            }

            public int GetHashCode(SolutionFile file)
            {
                return file.FilePath.GetHashCode();
            }
        }

        private QuickOpenFileControl notifyControl;
        private List<SolutionFile> solutionFiles;
        private SolutionReader solutionReader;
        private Task task;
        private ManualResetEvent initialIndexingComplete = new ManualResetEvent(false);
        private int sequence = 0;

        #region IDisposable Members

        public void Dispose()
        {
            initialIndexingComplete.Close();
        }

        #endregion
    }
}
