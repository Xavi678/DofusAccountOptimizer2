using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusAccountOptimizer2.Models
{
    public partial class Personatge
    {
        public bool GetActive()
        {
            return this.IsActive == 1;
        }
        public void SetActive(bool isActive)
        {
            this.IsActive = isActive ? 1 : 0;
        }
    }
}
