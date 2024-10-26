using DofusAccountOptimizer2.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusAccountOptimizer2.Models
{
    public partial class Personatge
    {
        public bool GetActive
        {
            get { return this.IsActive == 1; }
        }
        public void SetActive(bool isActive)
        {
            this.IsActive = isActive ? 1 : 0;
        }
        public IEnumerable<int>? GetKeyCodes()
        {
           return KeyCodesExtensions.ConvertKeys(this.KeyCodes);
            //return this.KeyCodes?.Split("|").Select(x => Convert.ToInt32(x));
        }
    }
}
