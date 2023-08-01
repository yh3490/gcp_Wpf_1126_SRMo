using System;
using System.Text;
using System.Runtime.InteropServices;   //DllImport

namespace gcp_Wpf
{
    public static class cIniAccess
    {

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder retVal, int size, string filePath);

        public static void Write(string filePath, string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, filePath);
        }

        public static string Read(string filePath, string section, string key, string defaultValue = "0")
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", retVal, 255, filePath);
            if(string.IsNullOrEmpty(retVal.ToString()))
            {
                WritePrivateProfileString(section, key, defaultValue, filePath);
            }
            else
            {
                defaultValue = retVal.ToString();
            }

            return defaultValue;
        }
    }
}
