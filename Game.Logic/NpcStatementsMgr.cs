using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using log4net;
using System.Reflection;

namespace Game.Logic
{
    public class NpcStatementsMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private static List<string> m_npcstatement = new List<string>();

        private static string filePath;

        private static Random random;

        public static bool Init()
        {
            filePath = Directory.GetCurrentDirectory() + @"\ai\npc\npc_statements.txt";
            random = new Random();
            return ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                string line = string.Empty;
                StreamReader streamReader = new StreamReader(filePath, System.Text.Encoding.Default);
                while (!string.IsNullOrEmpty(line = streamReader.ReadLine()))
                {
                    m_npcstatement.Add(line);
                }
                return true;
            }
            catch(Exception e)
            {
                log.Error("NpcStatementsMgr.Reload()", e);
                return false;
            }
        }

        public static int[] RandomStatementIndexs(int count)
        {
            int[] rands = new int[count];
            for (int i = 0; i < count; )
            {
                int next = random.Next(0, m_npcstatement.Count);
                if (!rands.Contains(next))
                {
                    rands[i] = next;
                    i++;
                }
            }

            return rands;
        }

        public static string[] RandomStatement(int count)
        {
            string[] temp = new string[count];
            int[] rands = RandomStatementIndexs(count);

            for(int i = 0; i < count; i++)
            {
                int k = rands[i];
                temp[i] = m_npcstatement[k];
            }

            return temp;
        }

        public static string GetStatement(int index)
        {
            if (index < 0 || index > m_npcstatement.Count)
                return null;
            return m_npcstatement[index];
        }

        public static string GetRandomStatement()
        {
            int index = random.Next(0, m_npcstatement.Count);
            return m_npcstatement[index];
        }
    }
}
