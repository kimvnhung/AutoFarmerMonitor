
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.IO.Compression;

namespace AutoFarmerMonitor
{
    class UpdateManager
    {
        private static int RemoteVersion()
        {
            HttpClient client = new HttpClient();
            var response = client.GetStringAsync("https://drive.google.com/u/1/uc?export=download&confirm=-knS&id=1zQe84d_F3Hs_UaeJUnMSsmeI00hYHnVp");
            if (response != null)
            {
                try
                {
                    var rs = response.Result;
                    int rt = int.Parse(rs.ToString());
                    return rt;
                }
                catch (Exception e)
                {
                    Log.d(e.Message);
                }
            }

            return 0;
        }

        private static void UpdateNewVersion()
        {
            string downloadPath = "https://www.googleapis.com/drive/v2/files/1czlabezQZ8vF2kCTOSA0sns-LxkOaAE3?key=AIzaSyDe88A0ZgrGlYLIFqeW9KbJwFsN-2Jt4_8&alt=media&source=downloadUrl";
            string downloadedFile = Log.AssemblyDirectory + "/RegCloneApp.zip";
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(downloadPath, downloadedFile);
                if (File.Exists(downloadedFile))
                {
                    string tempPath = Log.AssemblyDirectory + "/temp";
                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }
                    ZipFile.ExtractToDirectory(downloadedFile, tempPath);
                    CopyFilesRecursively(tempPath, Log.AssemblyDirectory);
                    Directory.Delete(tempPath, true);
                    File.Delete(downloadedFile);
                    File.WriteAllText(Log.AssemblyDirectory + "/version.txt", "" + RemoteVersion());
                }
            }
            catch (Exception e)
            {
                Log.d(e.Message);
            }
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                try
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
                }
                catch(Exception e)
                {
                    Log.d(e.Message);
                }
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
                }
                catch(Exception e)
                {
                    Log.d(e.Message);
                }
            }
        }

        public static void CheckForUpdate()
        {
            string localVersionPath = Log.AssemblyDirectory + "/version.txt";
            int localVersion = 0;
            if (File.Exists(localVersionPath))
            {
                try
                {
                    localVersion = int.Parse(File.ReadAllText(localVersionPath));
                }catch(Exception e)
                {
                    Log.d(e.Message);
                }
            }

            int remoteVersion = RemoteVersion();
            if(localVersion < remoteVersion)
            {
                Log.d("Updating...");
                UpdateNewVersion();
                Log.d("Update done to version " + remoteVersion);
            }
        }
    }
}
