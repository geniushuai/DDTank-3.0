using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic
{
    public class MissionInfo
    {
        private int m_totalCount;
        private int m_totalTurn;
        private int m_missionIndex;
        private string m_description;
        private string m_name;

        //死掉的NPC经验基础之和
        private int m_totalNpcExperience;
        //总NPC等级
        private int m_totalNpcLevel;

        public MissionInfo()
        {
        }

        public MissionInfo(int totalCount, int totalTurn, int missionIndex, string description, string name)
        {
            m_totalNpcLevel = totalCount;
            m_totalTurn = totalTurn;
            m_missionIndex = missionIndex;
            m_description = description;
            m_name = name;
        }

        public int TotalCount
        {
            get { return m_totalCount; }
            set { m_totalCount = value; }
        }

        public int TotalTurn
        {
            get { return m_totalTurn; }
            set { m_totalTurn = value; }
        }

        public int MissionIndex
        {
            get { return m_missionIndex; }
            set { m_missionIndex = value; }
        }

        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public int TotalNpcExperience
        {
            get { return m_totalNpcExperience; }
            set { m_totalNpcExperience = value; }
        }

        public int TotalNpcLevel
        {
            get { return m_totalNpcLevel; }
            set { m_totalNpcLevel = value; }
        }

        public void AddNpcExperience(int npcExperience)
        {
            TotalNpcExperience += npcExperience;
        }

        public void AddTotalNpcLevel(int npcLevel)
        {
            TotalNpcLevel += npcLevel;
        }

    }
}
