using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class MissionInfo
    {
        #region private attribute
        
        private int m_id;
        
        private string m_name;
        
        //本关卡需要击杀的npc数
        private int m_totalCount;
        
        //本关卡限制多少回合
        private int m_totalTurn;

        //递增delay值
        private int m_incrementDelay;

        private int m_delay;
        
        //关卡脚本
        private string m_script;
        
        private string m_failure;
        private string m_success;

        private string m_title;

        //预留字段
        private int m_param1;
        private int m_param2;

        //预留关卡中每轮更新字段
        private int m_param3;
        private int m_param4;
        
        private string m_description;
        #endregion

        #region constructor
        public MissionInfo()
        {
            m_param1 = -1;
            m_param2 = -1;
            m_param3 = -1;
            m_param4 = -1;
        }

        //TODO 加入初始Delay值,和递增delay值
        public MissionInfo(int id, string name, string key, string description, int totalCount, int totalTurn, int initDelay, int delay, string title, int param1, int param2)
        {
            m_id = id;
            m_name = name;
            m_description = description;
            m_failure = key;
            m_totalCount = totalCount;
            m_totalTurn = totalTurn;
            m_incrementDelay = initDelay;
            m_delay = delay;
            m_title = title;
            m_param1 = param1;
            m_param2 = param2;
            m_param3 = -1;
            m_param4 = -1;
        }
        #endregion

        #region properties
        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
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

        public int IncrementDelay
        {
            get { return m_incrementDelay; }
            set { m_incrementDelay = value; }
        }

        public int Delay
        {
            get { return m_delay; }
            set { m_delay = value; }
        }

        public string Script
        {
            get { return m_script; }
            set { m_script = value; }
        }

        public string Success
        {
            get { return m_success; }
            set { m_success = value; }
        }

        public string Failure
        {
            get { return m_failure; }
            set { m_failure = value; }
        }

        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        public string Title
        {
            get { return m_title; }
            set { m_title = value; }
        }

        public int Param1
        {
            get { return m_param1; }
            set { m_param1 = value; }
        }

        public int Param2
        {
            get { return m_param2; }
            set { m_param2 = value; }
        }

        public int Param3
        {
            get { return m_param3; }
            set { m_param3 = value; }
        }

        public int Param4
        {
            get { return m_param4; }
            set { m_param4 = value; }
        }

        #endregion
    }
}
