using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestCheckCN
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("请将带检查文件拖拽至窗口内。");
                while (true)
                {
                    string path = Console.ReadLine();
                    if ("" != path)
                    {
                        CheckFile(path);
                        Console.WriteLine("finish");
                        //Console.ReadKey();
                        WriteLogFile("finish");
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error :{0}",e.ToString());
                WriteLogFile(e.ToString());
                Console.ReadKey();
            }

        }

        private static void CheckFile(string FilePath)
        {
            //using (FileStream fs = File.OpenRead("D:\\技术练习集中营\\技术练习\\文件中判断中文字符的方法\\TestCheckCN\\testnote.txt"))
            WriteLogFile(FilePath);
            
            using (FileStream fs = File.OpenRead(FilePath))
            {
                Encoding ec = Encoding.GetEncoding("gb2312");

                using (StreamReader Reader = new StreamReader(fs, ec))
                {
                    string rd = null;
                    int i = 0;
                    while ((rd = Reader.ReadLine()) != null)
                    {
                        i++;
                        bool rlt = CheckCNFromString(rd);   
                        if (rlt)
                        {
                            string content = string.Format("Line:{0}  {1}", i, rd);
                            Console.WriteLine(content);
                            WriteLogFile(content);
                        }
                    }
                }
            }
            return;
        }

        //private static bool CheckCNFromString(string CString)
        //{
        //    bool BoolValue = false;
        //    for (int i = 0; i < CString.Length; i++)
        //    {
        //        if (Convert.ToInt32(Convert.ToChar(CString.Substring(i, 1))) < Convert.ToInt32(Convert.ToChar(128)))
        //        {
        //            BoolValue = false;
        //        }
        //        else
        //        {
        //            return BoolValue = true;
        //        }
        //    }
        //    return BoolValue;
        //}

        private static bool CheckCNFromString(string CString)
        {
            int code = 0;
            int chfrom = Convert.ToInt32("4e00", 16);    //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            int chend = Convert.ToInt32("9fff", 16);

            bool BoolValue = false;
            for (int i = 0; i < CString.Length; i++)
            {
                code = Char.ConvertToUtf32(CString, i);    //获得字符串CString中指定索引index处字符unicode编码

                if (code >= chfrom && code <= chend)
                {
                    return true;     //当code在中文范围内返回true
                }
                else
                {
                    BoolValue = false;
                }
            }
            return BoolValue;
        }

        public static void WriteLogFile(string content)
        {
            using (FileStream fs = File.Open(".//中文字符检查结果.txt", FileMode.Append))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine("{0}", content);
                }
            }
            return;
        }
    }
}
