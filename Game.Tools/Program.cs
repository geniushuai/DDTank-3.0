using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness.Managers;
using Game.Logic;
using SqlDataProvider.Data;
using System.Diagnostics;

namespace Game.Tools
{
    class Program
    {
         public static string m_pvepermissions;
        static void Main(string[] args)
        {
            //GC.Collect();
            //DropMgr.Init();
            //System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start(); //  开始监视代码运行时间            
            //List<ItemInfo> tempItem=null;
            //for (int i = 0; i < 100000; i++)
            //{
            //    DropInventory.PvEQuestsDrop(1, ref tempItem);                
            //    DropInventory.NPCDrop(3, ref tempItem);                
            //    DropInventory.CopyDrop(1071,1, ref tempItem);                
            //}
            //stopwatch.Stop(); //  停止监视
            //TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间          
            //double seconds = timespan.TotalSeconds;  //  总秒数
            //double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数 
            //Console.WriteLine("完毕" + seconds.ToString()+"秒");
            m_pvepermissions = "11222";
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Simple, m_pvepermissions, SetPvePermission(1, eHardLevel.Simple));
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Normal, m_pvepermissions, SetPvePermission(1, eHardLevel.Normal));
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Hard, m_pvepermissions, SetPvePermission(1, eHardLevel.Hard));
            Console.WriteLine();
            m_pvepermissions = "3F222";
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Simple, m_pvepermissions, SetPvePermission(1, eHardLevel.Simple));
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Normal, m_pvepermissions, SetPvePermission(1, eHardLevel.Normal));
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Hard, m_pvepermissions, SetPvePermission(1, eHardLevel.Hard));
            Console.WriteLine();
            m_pvepermissions = "7F222";
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Simple, m_pvepermissions, SetPvePermission(1, eHardLevel.Simple));
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Normal, m_pvepermissions, SetPvePermission(1, eHardLevel.Normal));
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Hard, m_pvepermissions, SetPvePermission(1, eHardLevel.Hard));

            //m_pvepermissions = "11122";
            //Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Simple, m_pvepermissions, SetPvePermission(2, eHardLevel.Simple));
            //Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Normal, m_pvepermissions, SetPvePermission(2, eHardLevel.Normal));
            //Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Hard, m_pvepermissions, SetPvePermission(1, eHardLevel.Hard));
            //Console.WriteLine();
            //m_pvepermissions = "33322";
            //Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Simple, m_pvepermissions, SetPvePermission(2, eHardLevel.Simple));
            //Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Normal, m_pvepermissions, SetPvePermission(2, eHardLevel.Normal));
            //Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Hard, m_pvepermissions, SetPvePermission(2, eHardLevel.Hard));
            //Console.WriteLine();
            //m_pvepermissions = "7F722";
            //Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Simple, m_pvepermissions, SetPvePermission(2, eHardLevel.Simple));
            //Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Normal, m_pvepermissions, SetPvePermission(2, eHardLevel.Normal));
            //Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Hard, m_pvepermissions, SetPvePermission(2, eHardLevel.Hard));
            m_pvepermissions = "11111";
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Simple, m_pvepermissions, SetPvePermission(5, eHardLevel.Simple));
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Normal, m_pvepermissions, SetPvePermission(5, eHardLevel.Normal));
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Hard, m_pvepermissions, SetPvePermission(5, eHardLevel.Hard));
            Console.WriteLine();
            m_pvepermissions = "33323";
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Simple, m_pvepermissions, SetPvePermission(5, eHardLevel.Simple));
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Normal, m_pvepermissions, SetPvePermission(5, eHardLevel.Normal));
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Hard, m_pvepermissions, SetPvePermission(5, eHardLevel.Hard));
            Console.WriteLine();
            m_pvepermissions = "7F72F";
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Simple, m_pvepermissions, SetPvePermission(5, eHardLevel.Simple));
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Normal, m_pvepermissions, SetPvePermission(5, eHardLevel.Normal));
            Console.WriteLine("11 : level {0} source {1}result={2}", eHardLevel.Hard, m_pvepermissions, SetPvePermission(5, eHardLevel.Hard));
            Console.Read();


             
        }
     
        public static string SetPvePermission(int missionId, eHardLevel hardLevel)
        {
          
            if (hardLevel == eHardLevel.Terror)
                return "FF";
            var setPvePermision = string.Empty;
            string right = m_pvepermissions.Substring(missionId - 1, 1);
            if (hardLevel == eHardLevel.Simple && right == "1")
            {

                setPvePermision = "3";
            }
            else if (hardLevel == eHardLevel.Normal && right == "3")
            {

                setPvePermision = "7";
            }
            else if (hardLevel == eHardLevel.Hard && right == "7")
            {
                setPvePermision = "F";
            }
            else
            {
                return m_pvepermissions;
            }
            var strPvePermision = m_pvepermissions;
            var length = strPvePermision.Length;
            strPvePermision = strPvePermision.Substring(0, missionId - 1) + setPvePermision + strPvePermision.Substring(missionId , length - missionId);
            return strPvePermision;
            //return true;
        }
 

    }
}
