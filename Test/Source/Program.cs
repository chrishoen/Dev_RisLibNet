using System;
using System.Collections.Generic;
using System.Text;

using Ris;

namespace MainApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Init.initialize(args);
            MyCmdLineExec tCmdLineExec = new MyCmdLineExec();
            CmdLineConsole.execute(tCmdLineExec);
            Init.finalize();
        }
    }
}
