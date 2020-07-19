using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoClicker
{
    class AppSettings
    {
        public bool IsSimulateHumanClick { set; get; }
        public string FocusedWindowName { set; get; }
        public int Min { set; get; }
        public int Max { set; get; }
        public Keys bindedKey { set; get; }
    }
}
