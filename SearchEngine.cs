using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.Shell.Interop;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace QuickOpenFile
{
    public class SearchEngine
    {
        public SearchEngine(QuickOpenFileControl notifyControl, bool async = false)
        {
            isAsync = async;
            this.notifyControl = notifyControl;
            work = null;
            cancel = false;
            solutionFiles = new List<SolutionFile>();
            solutionReader = new SolutionReader();
            if (isAsync)
            {
                mutex = new Mutex();
                semaphore = new Semaphore(0, 999999);
                Debug.Print("QOF.SearchEngine: Starting search thread.");
                worker = new Thread(this.WorkerProc);
                worker.Priority = ThreadPriority.Highest;
                worker.Name = "QuickOpenFile.SearchEngine";
                worker.Start();
            }
        }

        /// <summary>
        /// Orders a new search of the solution. The UI is notified
        /// asynchronously when the search is finished.
        /// </summary>
        /// <param name="query">Search expression.</param>
        /// <param name="options">Search options.</param>
        /// <param name="index">If true, the solution is indexed first.</param>
        public void Search(string query, Options options, bool index)
        {
            if (query == null && !index)
                return;

            if (isAsync)
            {
                mutex.WaitOne();
                work = new WorkItem(options, query, index);
                mutex.ReleaseMutex();
                semaphore.Release();
            }
            else
            {
                if (index)
                {
                    DoIndexSolution(options);
                }
                if (query != null)
                {
                    DoSearch(query, options);
                }
            }
        }

        /// <summary>
        /// Signals the worker thread to stop.
        /// </summary>
        public void Stop()
        {
            if (isAsync)
            {
                mutex.WaitOne();
                cancel = true;
                work = null;
                mutex.ReleaseMutex();
                semaphore.Release();
                Debug.Print("QOF.SearchEngine: Stopping search thread.");
            }
        }

        private void WorkerProc()
        {
            Debug.Print("QOF.SearchEngine: Search thread started.");
            bool breakNow = false;
            while (!breakNow)
            {
                semaphore.WaitOne();
                mutex.WaitOne();
                breakNow = cancel;
                WorkItem w = work;
                work = null;
                mutex.ReleaseMutex();

                if (w == null)
                    continue;
                if (w.doIndex)
                    DoIndexSolution(w.options);
                if (w.query != null)
                {
                    DoSearch(w.query, w.options);
                }
            }
            Debug.Print("QOF.SearchEngine: Search thread stopped.");
        }

        private void DoIndexSolution(Options options)
        {
            NotifyStatusText("Indexing...");
            DateTime time1 = DateTime.Now;
            solutionFiles = solutionReader.GetSolutionFiles(notifyControl.GetSolution(), options);
            DateTime time2 = DateTime.Now;
            Debug.Print("QOF.SearchEngine: Indexed " + solutionFiles.Count + " solution files in " + (time2 - time1) + ".");
            NotifyStatusText("Ready.");
        }

        private void DoSearch(string query, Options options)
        {
            IEnumerable<SolutionFile> result = null;

            var stopwatch = Stopwatch.StartNew();
            try
            {
                if (String.IsNullOrWhiteSpace(query))
                {
                    throw new ArgumentNullException("Empty search expression.");
                }

                var positiveTerms = MakeRegexes(query, options);
                var negativeTerms = options.IgnorePatterns.SelectMany(nq => MakeRegexes(nq, options));

                Debug.Print("QOF.SearchEngine: Searching for: '" + String.Join(" ", positiveTerms) + 
                    "' " + String.Join(" ", negativeTerms.Select(re => "NOT '" + re.ToString() + "'")));
                result = solutionFiles
                    .Where(sr => positiveTerms.Any(r => r.IsMatch(sr.Name)) && !negativeTerms.Any(r => r.IsMatch(sr.Name)))
                    .Distinct(new DistinctByFilePath())
                    //TODO: show open files first
                    //TODO: for now, just sorts alphabetically (by Name, then by ProjectName, then by FilePath)
                    .OrderBy(item => item.LastWriteTime)
                    // force to execute the query now (in try block)
                    .ToArray();

                if (result.Count() > 0)
                {
                    NotifyStatusText("Ready.");
                }
                else
                {
                    NotifyStatusText("No files match the expression.");
                }
            }
            catch (ArgumentNullException)
            {
                NotifyStatusText("Ready.");
            }
            catch (ArgumentException)
            {
                NotifyStatusText("Invalid search expression.");
            }

            Debug.Print("QOF.SearchEngine: Found " + (result == null ? 0 : result.Count()) + " solution files in " + stopwatch.Elapsed + ".");

            NotifyResults(result);
        }

        private IEnumerable<Regex> MakeRegexes(string query, Options options)
        {
            // replace internal whitespace with single * wildcards
            if (options.SpaceAsWildcard)
                query = ReplaceWhitespaceWithWildcard(query);
            
            // escape control characters (search 'as is' for all other characters than ?, *)
            query = EscapeRegularExpression(query);
            
            // replace ? and * wildcards in the string with proper regex syntax
            query = query.Trim().Replace('?', '.').Replace("*", ".*");
            
            // match patter at the begining
            if (!options.SearchInTheMiddle)
                query = "^" + query;
            
            // search using camel case
            if (options.UseCamelCase)
            {
                yield return new Regex(MakeCamelCaseExpression(query));
            }

            yield return new Regex(query, RegexOptions.IgnoreCase);
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
            if (isAsync)
            {
                notifyControl.BeginInvoke(new SetStatusTextDelegate(notifyControl.SetStatusText), new[] { text });
            }
            else
            {
                notifyControl.SetStatusText(text);
            }
        }

        private void NotifyResults(IEnumerable<SolutionFile> results)
        {
            if (isAsync)
            {
                notifyControl.BeginInvoke(new ShowResultsDelegate(notifyControl.ShowResults), new[] { results });
            }
            else
            {
                notifyControl.ShowResults(results);
            }
        }

        class WorkItem
        {
            public Options options;
            public String query;
            public bool doIndex;

            public WorkItem() { }
            public WorkItem(Options options, String query, bool doIndex)
            {
                this.options = options;
                this.query = query;
                this.doIndex = doIndex;
            }
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

        private delegate void SetStatusTextDelegate(string text);
        private delegate void ShowResultsDelegate(IEnumerable<SolutionFile> results);

        bool isAsync;
        Thread worker;
        Mutex mutex;
        Semaphore semaphore;
        volatile bool cancel;
        volatile WorkItem work;
        QuickOpenFileControl notifyControl;
        List<SolutionFile> solutionFiles;
        SolutionReader solutionReader;
    }
}
