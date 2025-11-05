using System;
using System.Collections.Generic;

namespace Three_Musketeers.Utils
{
    public class LibraryDependencyTracker
    {
        private readonly HashSet<string> includedLibraries;
        private readonly Dictionary<string, string> functionToLibrary;
        private readonly Action<int, string> reportError;

        public LibraryDependencyTracker(Action<int, string> reportError)
        {
            this.reportError = reportError;
            this.includedLibraries = new HashSet<string>();
            
            // Map functions to their required libraries
            this.functionToLibrary = new Dictionary<string, string>
            {
                // stdio.tm functions
                { "printf", "stdio.tm" },
                { "scanf", "stdio.tm" },
                { "gets", "stdio.tm" },
                { "puts", "stdio.tm" },
                
                // stdlib.tm functions
                { "malloc", "stdlib.tm" },
                { "free", "stdlib.tm" },
                { "atoi", "stdlib.tm" },
                { "atod", "stdlib.tm" },
                { "itoa", "stdlib.tm" },
                { "dtoa", "stdlib.tm" }
            };
        }

        public void RegisterInclude(string libraryName)
        {
            includedLibraries.Add(libraryName);
        }

        public bool CheckFunctionDependency(string functionName, int line)
        {
            if (!functionToLibrary.ContainsKey(functionName))
            {
                // Not a library function, no check needed
                return true;
            }

            string requiredLibrary = functionToLibrary[functionName];
            
            if (!includedLibraries.Contains(requiredLibrary))
            {
                reportError(line, 
                    $"Function '{functionName}' requires '#include <{requiredLibrary}>' at the beginning of the file");
                return false;
            }

            return true;
        }
    }
}