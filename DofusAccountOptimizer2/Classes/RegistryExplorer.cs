using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace DofusAccountOptimizer2.Classes
{
    public static class RegistryExplorer
    {
        public const string TASKBAR_GLOOM_LEVEL = "TaskbarGlomLevel";
        public static RegistryKey? GetTaskbarRegistryPropertie()
        {
            return Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", true);
        }
    }
}
