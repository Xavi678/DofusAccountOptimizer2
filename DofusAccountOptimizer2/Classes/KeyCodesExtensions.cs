using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusAccountOptimizer2.Classes
{
    public static class KeyCodesExtensions
    {
        public static IEnumerable<int> ConvertKeys(string keys)
        {
            return keys.Split("|").Select(x => Convert.ToInt32(x));
        }
        public static string ConvertToString(IEnumerable<int> keycodes)
        {
            return String.Join("|", keycodes.Select(x => $"{x}"));
        }

        public static string ConvertToTextBoxString(IEnumerable<int> keyCodes)
        {
            return string.Join(" + ", MainWindow.keyCodes.Select(x => (System.Windows.Forms.Keys)x));
        }
    }
}
