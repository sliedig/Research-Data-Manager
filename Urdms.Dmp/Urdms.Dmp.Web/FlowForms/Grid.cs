using System.Collections.Generic;

namespace Urdms.Dmp.Web.FlowForms
{
    public class Grid<T> : Dictionary<string, Dictionary<string, T>>
    {
        public T Cell(string promptKey, string choiceKey)
        {
            if (ContainsKey(promptKey) && this[promptKey].ContainsKey(choiceKey))
                return this[promptKey][choiceKey];
            return default(T);
        }
    };
}
