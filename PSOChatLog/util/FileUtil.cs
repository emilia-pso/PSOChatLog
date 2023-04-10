using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace PSOChatLog
{
    public static class FileUtil
    {
        //public static readonly string strEnvironment = "PSOChatLog";
        public static readonly string strEnvironment = Value.strEnvironment;
        public static readonly string strIniFileName = ".\\" + strEnvironment + ".ini";

        // DLL関数の定義です。
        [DllImport("KERNEL32.DLL")]
        private static extern uint GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            uint nSize,
            string lpFileName);

        // DLL関数の定義です。
        [DllImport("KERNEL32.DLL")]
        private static extern uint GetPrivateProfileInt(
            string lpAppName,
            string lpKeyName,
            int nDefault,
            string lpFileName);

        // DLL関数の定義です。
        [DllImport("KERNEL32.DLL")]
        private static extern int WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName
        );

        // DLL関数をラップしたメソッドです。
        public static string CallApiGetValueString(string section, string key, string fileName)
        {
            var sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, "", sb, Convert.ToUInt32(sb.Capacity), fileName);
            return sb.ToString();
        }

        public static int CallApiGetPrivateProfileInt(string section, string key, string fileName)
        {
            var sb = new StringBuilder(1024);
            return (int)GetPrivateProfileInt(section, key, 0, fileName);
        }
        // DLL関数をラップしたメソッドです。
        public static void SetValue(string section, string key, string value, string fileName)
        {
            WritePrivateProfileString(section, key, value, fileName);
        }
    }
}
