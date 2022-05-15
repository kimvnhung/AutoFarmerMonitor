using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoFarmerMonitor
{
    class Log
    {
        public static void d(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss :") + message);
            File.AppendAllText(AssemblyDirectory + "/log_monitor.txt", DateTime.Now.ToString("hh:mm:ss :") + message + "\n");
        }

        public static void DirSearch_ex3(string sDir)
        {
            //Console.WriteLine("DirSearch..(" + sDir + ")");
            try
            {
                Console.WriteLine(sDir);

                foreach (string f in Directory.GetFiles(sDir))
                {
                    Console.WriteLine(f);
                }

                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirSearch_ex3(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
