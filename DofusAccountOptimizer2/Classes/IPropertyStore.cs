using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;

namespace DofusAccountOptimizer2.Classes
{
    /// <summary>
    /// Exposes methods for enumerating, getting, and setting property values.
    /// </summary>
    [ComImport, Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPropertyStore
    {
        void GetCount(out uint cProps);
        void GetAt([In] uint iProp, out PropertyKey pKey);
        void GetValue([In] ref PropertyKey key, [Out] PropVariant pv);
        void SetValue([In] ref PropertyKey key, [In] PropVariant propvar);
        void Commit();
    }
}
