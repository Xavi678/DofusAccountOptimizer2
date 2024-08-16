using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DofusAccountOptimizer2.Models
{
    public partial class Configuracio
    {
        public bool GetUpdateIcons()
        {
            return this.UpdateIcons == 1;
        }
        public bool GetOrderWindows()
        {
            return this.OrderWindows == 1;
        }
        public void SetUpdateIcons(bool updateIcons)
        {
            this.UpdateIcons = updateIcons ? 1 : 0;
        }
        public void SetOrderWindows(bool orderWindows)
        {
            this.OrderWindows = orderWindows ? 1 : 0;
        }
    }
}
