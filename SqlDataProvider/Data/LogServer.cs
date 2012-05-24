using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    /// <summary>
    /// 游戏服务器状态
    /// </summary>
    public class LogServer
    {
        public int ApplicationId { set; get; }
        public int SubId { set; get; }
        public DateTime EnterTime { set; get; }
        public int Online { set; get; }
        public int Reg { set; get; }
        public int PayMan { set; get; }

    }
}
