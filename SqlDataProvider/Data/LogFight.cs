using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    /// <summary>
    /// 战斗记录
    /// </summary>
    public class LogFight
    {
        public int ApplicationId { set; get; }
        public int SubId { set; get; }
        public int LineId { set; get; }
        public int RoomType { set; get; }
        public int FightType { set; get; }
        public DateTime PlayBegin { set; get; }
        public DateTime PlayEnd { set; get; }
        public int UserCount { set; get; }
        public int MapId { set; get; }
        public string Users { set; get; }
        public string PlayResult { set; get; }
    }
}
