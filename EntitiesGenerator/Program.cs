using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EntitiesGenerator
{
    static class Program
    {
        /// <summary>
        /// 应用程序主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormMain formMain;
            if ((args != null) && (args.Length > 0))
            {
                formMain = new FormMain(args[0]);
            }
            else
            {
                formMain = new FormMain();
            }
            Application.Run(formMain);
        }
    }
}
