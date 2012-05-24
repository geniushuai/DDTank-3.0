using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Bussiness
{
    public class IniReader
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
             int size, string filePath);

        private string FilePath;

        /// <summary>
        /// 构造函数重载版本。
        /// </summary>
        /// <param name="_FilePath">ini文件地址。</param>
        public IniReader(string _FilePath)
        {
            this.FilePath = _FilePath;
        }

        //public string GetIniString(string section, string key)
        //{
        //    StringBuilder retVal = new StringBuilder(255);
        //    if (FilePath.Length <= 0)
        //    {
        //        throw new Exception("没有指定ini文件路径。");
        //    }
        //    try
        //    {
        //        GetPrivateProfileString(section, key, "", retVal, 255, this.FilePath);
        //        return retVal.ToString();
        //    }
        //    catch (Exception err)
        //    {
        //        throw err;
        //    }
        //}

        public string GetIniString(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(2550);

            GetPrivateProfileString(Section, Key, "", temp, 2550, this.FilePath);
            return temp.ToString();
        }
    }
}
