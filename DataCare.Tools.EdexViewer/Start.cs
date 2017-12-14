using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCare.Tools.EdexViewer
{
    public static class Start
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var application = new System.Windows.Application();
            application.Run(new MainWindow());
        }
    }
}
