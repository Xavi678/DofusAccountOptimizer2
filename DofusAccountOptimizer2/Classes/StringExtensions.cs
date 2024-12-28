using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DofusAccountOptimizer2.Classes
{
    public static class StringExtensions
    {
        public static bool ContainsCharName(this string windowTitle,string Name)
        {
            var match=Regex.Match(windowTitle, $"{Name}\\s-");
            return match.Success;
        }
    }
}
