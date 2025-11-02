using Three_Musketeers.Models;

namespace Three_Musketeers.Utils
{
    public class VariableResolver
    {
        private readonly Dictionary<string, Variable> variables;
        private readonly Func<string?> getCurrentFunctionName;

        public VariableResolver(
            Dictionary<string, Variable> variables,
            Func<string?> getCurrentFunctionName)
        {
            this.variables = variables;
            this.getCurrentFunctionName = getCurrentFunctionName;
        }

        public Variable? FindVariable(string varName)
        {
            // Try function scope first
            string? currentFunction = getCurrentFunctionName();

            if (currentFunction != null)
            {
                string scopedName = $"@{currentFunction}.{varName}";
                if (variables.ContainsKey(scopedName))
                {
                    return variables[scopedName];
                }
            }

            // Try global scope
            if (variables.ContainsKey(varName))
            {
                return variables[varName];
            }

            return null;
        }

        public Variable? GetVariable(string varName)
        {
            string? currentFunc = getCurrentFunctionName();

            // Try local scope first (if inside a function)
            if (currentFunc != null)
            {
                string scopedName = $"@{currentFunc}.{varName}";
                if (variables.ContainsKey(scopedName))
                {
                    return variables[scopedName];
                }
            }

            // Try global scope
            if (variables.ContainsKey(varName))
            {
                return variables[varName];
            }

            // Variable not found
            throw new Exception($"Variable '{varName}' not found in current scope");
        }
    }
}