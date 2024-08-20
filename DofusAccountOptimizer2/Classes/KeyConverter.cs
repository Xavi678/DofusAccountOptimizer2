using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace DofusAccountOptimizer2.Classes
{
    [ValueConversion(typeof(ObservableCollection<int>), typeof(String))]
    public class KeyConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            var list=(ObservableCollection<int>)value;
           var resultList= String.Join(" + ", list.Select(x=> $"{(Keys)x}"));
            return resultList;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            throw new NotImplementedException();
        
        }
    }
}
