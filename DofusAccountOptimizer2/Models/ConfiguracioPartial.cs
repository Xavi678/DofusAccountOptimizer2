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

        public void SetMouseEnabled(bool isMouseEnabled)
        {
            this.EnableMouse = isMouseEnabled ? 1 : 0;
        }

        public void SetKeyboardEnabled(bool isKeyboardEnabled)
        {
            this.EnableKeyboard = isKeyboardEnabled ? 1 : 0;
        }
        public bool GetMouseEnabled()
        {
          return  this.EnableMouse==1;
        }
        public bool GetKeyboardEnabled()
        {
            return this.EnableKeyboard == 1;
        }

        public bool GetOrderWindowsOnChangeComp()
        {
            return this.OrderWindowsOnChangeComp == 1;
        }
        public void SetOrderWindowsOnChangeComp(bool isOrderWindowsOnChangeComp)
        {
            this.OrderWindowsOnChangeComp = isOrderWindowsOnChangeComp ? 1 : 0;
        }
    }
}
