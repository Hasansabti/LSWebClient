using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LSWebClient1
{

    public class ConfigUtils
    {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public ConfigUtils(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? "Config.txt").FullName;


            if (!KeyExists("serverlink"))
            {
                Write("serverlink", "http://172.16.1.3:8080/BC140/?page=99008880");
            }

            if (!KeyExists("username"))
            {
                Write("username", "POS01");
            }
            if (!KeyExists("password"))
            {
                Write("password", "36r(1ldm*92n");
            }
            if (!KeyExists("fullscreen"))
            {
                Write("fullscreen", "true");
            }
            //dualscreennum
            if (!KeyExists("dualscreen"))
            {
                Write("dualscreen", "true");
            }
            if (!KeyExists("dualscreennum"))
            {
                Write("dualscreennum", "1");
            }
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
    }
}
