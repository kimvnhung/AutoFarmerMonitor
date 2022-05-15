using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoFarmerMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread thread;
        private string _path = "";

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                pathTb.Text = _path;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Path = Log.AssemblyDirectory + "/RegClone.exe";
            thread = new Thread(() =>
            {
                RunMonitor();
            });
            thread.Start();
        }

        private void selectBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Executed files (*.exe)|*.exe";
            fileDialog.InitialDirectory = _path;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Path = fileDialog.FileName;
            }
        }

        private void RunMonitor()
        {
            
            while (true)
            {
                Process[] processCollection = Process.GetProcesses();
                for (int i = 0; i < processCollection.Length; i++)
                {
                    Process p = processCollection[i];
                    if (p.ProcessName.Equals("RegClone"))
                    {
                        Log.d(ProcessExtensions.Parent(p).ProcessName);
                        Log.d(p.ProcessName + ": still running...");
                        p.Refresh();
                        Log.d($"{p} -");
                        Log.d("-------------------------------------");

                        Log.d($"  Physical memory usage     : {p.WorkingSet64/(1024*1024)}");
                        Log.d($"  Base priority             : {p.BasePriority}");
                        Log.d($"  Priority class            : {p.PriorityClass}");
                        Log.d($"  User processor time       : {p.UserProcessorTime}");
                        Log.d($"  Privileged processor time : {p.PrivilegedProcessorTime}");
                        Log.d($"  Total processor time      : {p.TotalProcessorTime}");
                        Log.d($"  Paged system memory size  : {p.PagedSystemMemorySize64 / (1024 * 1024)}");
                        Log.d($"  Paged memory size         : {p.PagedMemorySize64 / (1024 * 1024)}");
                        break;
                    }
                    if (i == processCollection.Length - 1)
                    {
                        Log.d("Restart");
                        try
                        {
                            UpdateManager.CheckForUpdate();
                            //Log.DirSearch_ex3(Log.AssemblyDirectory);

                            if (File.Exists(Path))
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    Process.Start(Path);
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            Log.d(e.Message);
                        }
                    }
                }
                Thread.Sleep(2 * 60 * 1000);

            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            thread.Abort();
        }
    }
}
