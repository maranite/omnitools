using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Text;
using BwsPresetTool.VST;
using BwsPresetTool.Bitwig;
using System.Threading;
using System.Text.RegularExpressions;

namespace BwsPresetTool
{
    static partial class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
#if Debug
                if (!Debugger.IsAttached)
                    Debugger.Launch();
#endif
                Application.Run(new VstPluginHost(args[1]));
            }
            else
                Application.Run(new VstPluginsForm());
        }
    }
}
