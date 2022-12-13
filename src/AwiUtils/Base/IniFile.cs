using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace AwiUtils
{

    /// <summary> Makes handling of traditional ini-files from C# easier.  </summary>
    /// <remarks>
    /// Internally uses the WinAPI functions WritePrivateProfileString 
    /// and GetPrivateProfileString.  See there also. 
    /// </remarks>
    public class IniFile
    {
        static class NativeMethods
        {
            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool WritePrivateProfileString(string section,
                string key, string val, string filePath);
            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            [return: MarshalAs(UnmanagedType.U4)]
            internal static extern uint GetPrivateProfileString(string section,
                     string key, string defVal, [Out] StringBuilder retVal,
                uint size, string filePath);
        }


        /// <summary> Creates an IniFile object. Creates the directory to the file 
        /// if it does not exist. </summary>
        public IniFile(string directory, string fileName)
        {
            Directory.CreateDirectory(directory);
            FullPath = Path.Combine(directory, fileName);
        }


        /// <summary> Write one value to the ini-file. </summary>
        public void WriteValue(string section, string key, string value)
        {
            NativeMethods.WritePrivateProfileString(section, key, value, FullPath);
        }

        /// <summary> Write one value to the ini-file. Puts key and subkey together with a dot. </summary>
        public void WriteValue(string section, string key, string subkey, string value)
        {
            WriteValue(section, key + "." + subkey, value);
        }


        /// <summary> Read one value from the ini-file. </summary>
        public string ReadValue(string section, string key, string defaultValue)
        {
            // Take some starting value for the size of the buffer, into which
            // most values will fit. Which value we take here exactly doesn't matter. 
            int bufLength = 64; 
            uint u = 0;
            System.Text.StringBuilder temp = new System.Text.StringBuilder();
            do
            {
                bufLength *= 2;
                temp.Capacity = bufLength;
                u = NativeMethods.GetPrivateProfileString(section, key, defaultValue, temp,
                                            (uint)bufLength, FullPath);
            } while(u == bufLength - 1);
            return temp.ToString();
        }

        /// <summary> Read one value from the ini-file. Puts key and subkey together with a dot. </summary>
        public string ReadValue(string section, string key, string subkey, string defaultValue)
        {
            return ReadValue(section, key + "." + subkey, defaultValue);
        }


        /// <summary> Flushes the ini-file cache of the OS. </summary>
        public void Flush()
        {
            NativeMethods.WritePrivateProfileString(null, null, null, null);
        }

        // The full path to the real inifile. Only for debugging/Logging Don't use otherwise. 
        protected string FullPath { get; private set; }

    } // class IniFile
}


