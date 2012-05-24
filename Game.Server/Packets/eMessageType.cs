using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Packets
{
    public enum eMessageType
    {
        /// <summary>
        /// 普通消息黄色，有屏幕提示
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 错误消息红色，有屏幕提示
        /// </summary>
        ERROR = 1,

        /// <summary>
        /// 普通消息黄色，无屏幕提示
        /// </summary>
        ChatNormal = 2,

        /// <summary>
        /// 错误消息红色，无屏幕提示
        /// </summary>
        ChatERROR = 3,


        /// <summary>
        /// 弹出框
        /// </summary>
        ALERT = 4,


        /// <summary>
        /// 日常奖励
        /// </summary>
        DailyAward = 5,


        /// <summary>
        /// 防御绿色
        /// </summary>
        Defence = 6,

    }
}
