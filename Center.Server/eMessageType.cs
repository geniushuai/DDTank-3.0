using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Center.Server
{
    public enum eMessageType
    {
        /// <summary>
        /// 普通消息
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 错误消息
        /// </summary>
        ERROR = 1,

        /// <summary>
        /// 普通消息
        /// </summary>
        ChatNormal = 2,

        /// <summary>
        /// 错误消息
        /// </summary>
        ChatERROR = 3,

        ALERT = 4,

    }
}
