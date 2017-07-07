using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Management;
//using static System.OperatingSystem;

namespace MinecraftBackup
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ///Application.Run(new Form1());

            /* Variablen */
            string strMCBDir;
            string strMCWDirP1;
            string strMCWDirP2;
            string strMCWDir;

            /// Backup directory
            strMCBDir = Environment.GetEnvironmentVariable("USERPROFILE") + "\\Documents\\MCBackup";

            /// Minecraft 10 worlds
            strMCWDirP1 = Environment.GetEnvironmentVariable("LOCALAPPDATA");
            strMCWDirP2 = "\\Packages\\Microsoft.MinecraftUWP_8wekyb3d8bbwe\\LocalState\\games\\com.mojang\\minecraftWorlds";
            strMCWDir = strMCWDirP1 + strMCWDirP2;

            // helper class
            helper h = new helper();

        /// Check OS
            /// Get OS' major version number as int
            int result = h.WinVerInt(h.GetOSVersionAndCaption().Value);
            /// Quit if not running Windows 10
            if (!(result == 10))
            {
                h.NoWin10(result);
            } 

        /// Check folders
            /// CheckDir class
            CheckDir cd = new CheckDir();

            /// Check world folder
            /// Quit if it not exists
            if (cd.isMCBWThere(strMCWDir) == 1)
            {
                if (System.Windows.Forms.Application.MessageLoop)
                {
                    /// WinForms app
                    Application.Exit();
                }
                else
                {
                    /// Console app
                    System.Environment.Exit(1);
                }
            }
            
            /// Check backup folder
            /// Create if it not exists
            cd.CheckMCBfolder(strMCBDir);

            /// count your worlds
            int directoryCount = Directory.GetDirectories(strMCWDir).Length;

            /// build FQ file name
            string filename = h.getFilename();
            string MCBFile = strMCBDir + "\\" + filename;
            /// zip it
            h.ZipIt(strMCWDir, MCBFile);
            string caption = "Minecraft Backup";
            string message = directoryCount + " worlds saved\r\n"
                + ":-)";
            MessageBox.Show(message, caption);

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CheckDir
    {
        /// <summary>
        ///  Check Minecraft Backup Directory
        ///  Create one if nesseary
        /// </summary>
        /// <param name="strDir">Folder Name</param>
        /// <returns>Allways 0</returns>
        public int CheckMCBfolder(string strDir)
        {
            if (Directory.Exists(strDir))
            {
                /// all good
                return 0;
            }
            else
            {
                /// Message Box
                const string caption = "Minecraft Backup";
                const string message = "Minecraft backup directory not found\r\n"
                    + ":-(\r\n"
                    + "But no worries. I will create one for you.\r\n"
                    + ":-)";
                MessageBox.Show(message, caption);
                /// Create directory
                Directory.CreateDirectory(strDir);
                return 0;
            }
        }

        // Message if Minecraft world is empty
        public int isMCBWThere(string strDir)
        {
            if (!Directory.Exists(strDir))
            {
                const string caption = "Minecraft Backup";
                const string message = "No Minecraft worlds found\r\n"
                    + ":-(\r\n"
                    + "Do you have Minecraft Windows 10 Edition installed?\r\n"
                    + ";-)";
                MessageBox.Show(message, caption);
                return 1;
            }
            else
            {
                return 0;
            }
        }

    }

    public class mcb
    {

    }

    public class helper
    {
        public KeyValuePair<string, string> GetOSVersionAndCaption()
        {
            KeyValuePair<string, string> kvpOSSpecs = new KeyValuePair<string, string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption, Version FROM Win32_OperatingSystem");
            try
            {
                foreach (var os in searcher.Get())
                {
                    var version = os["Version"].ToString();
                    var productName = os["Caption"].ToString();
                    kvpOSSpecs = new KeyValuePair<string, string>(productName, version);
                }
            }
            catch { }

            return kvpOSSpecs;
        }

        public int WinVerInt(string strVersion)
        {
            int intVersion = 0;
            int.TryParse(strVersion.Substring(0,strVersion.IndexOf(".")), out intVersion);
            

            // Debug.WriteLine("WinVerInt :: " + strVersion);
            // int @int = strVersion.IndexOf(".");
            // string str = strVersion.Substring(0, @int);
            // Debug.WriteLine("Major :: " + str);

            return intVersion;
        }

/*        public int WinVer()
        {
            var os = Environment.OSVersion;
            int major = os.Version.Major;
            int minor = os.Version.Minor;
            Debug.WriteLine("major :: " + major);
            Debug.WriteLine("minor :: " + minor);
            Debug.WriteLine("OSVersion: " + Environment.OSVersion.ToString());
            return major;
        } */

        public int NoWin10(int major)
        {
            /// Message Box
            const string caption = "Minecraft Backup";
            const string message = "You are not running Windows 10\r\n"
                + ":-(\r\n"
                + "I quit!";
            MessageBox.Show(message, caption);
            if (System.Windows.Forms.Application.MessageLoop)
            {
                /// WinForms app
                Application.Exit();
            }
            else
            {
                /// Console app
                System.Environment.Exit(1);
            }
            return 0;
        }

        public string getFilename()
        {
            string dt = DateTime.Now.ToString("yyyyMMdd-HHMMss");
            string filename = "MCBackup_" + dt + ".zip";
            return filename;
        }

        public int ZipIt(string SourceDir, string DestinationFile)
        {
            ZipFile.CreateFromDirectory(SourceDir, DestinationFile);
            return 0;
        }

    }
}