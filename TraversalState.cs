using System;
using System.Collections.Generic;

namespace QuickOpenFile
{
    public class TraversalState
    {
        public string CurrentSolutionName { get; set; }
        public string CurrentProjectName { get; set; }
        public List<String> CurrentPathComponents { get; set; }
        public string CurrentPath { get; set; }
        public int LastRecursionLevel { get; set; }

        public TraversalState()
        {
            CurrentPathComponents = new List<string>();
            Clear();
        }

        public void Clear()
        {
            CurrentSolutionName = "";
            CurrentProjectName = "";
            CurrentPathComponents.Clear();
            CurrentPath = "";
            LastRecursionLevel = 0;
        }
    }
}
