using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TraidingBotI
{
    static class Program
    {
        private static Form _nextForm;
        private static bool _runningNextForm;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RunNewForm(new Form1());
            while (_runningNextForm)
            {
                _runningNextForm = false;
                Application.Run(_nextForm);
            }
        }


        internal static void RunNewForm(Form form)
        {
            _nextForm = form;
            _runningNextForm = true;
        }

    }
}
