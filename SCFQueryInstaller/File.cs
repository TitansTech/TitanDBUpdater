using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;


namespace SCFQueryInstaller
{
    public class File
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static string GetString(string Section, string Key, string DefaultValue, string Path)
        {
            StringBuilder temp = new StringBuilder(2048);
            GetPrivateProfileString(Section, Key, DefaultValue, temp, 255, Path);
            return (temp.ToString());
        }

        public static void SetString(string Section, string Key, string Value, string Path)
        {
            WritePrivateProfileString(Section, Key, Value, Path);
        }

        public static void WriteLine(string Path, string Text, bool Append)
        {
            try
            {
                StreamWriter w = new StreamWriter(Path, Append);
                w.WriteLine(Text);
                w.Close();
            }
            catch (Exception) { }
        }

        public static void Write(string Path, string Text, bool Append)
        {
            try
            {
                StreamWriter w = new StreamWriter(Path, Append);
                w.Write(Text);
                w.Close();
            }
            catch (Exception) { }
        }

        public static string Read(string FilePath)
        {
            try
            {
                StreamReader r = new StreamReader(FilePath);
                string str = r.ReadToEnd();
                r.Close();
                return str;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
