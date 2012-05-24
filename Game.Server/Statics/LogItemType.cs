using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Statics
{
    public enum LogItemType
    {
        /// <summary>
        /// 强化
        /// </summary>
        Strengthen = 1,

        /// <summary>
        /// 合成
        /// </summary>
        Compose=2,

        /// <summary>
        /// 熔炼
        /// </summary>
        Fusion=3,

        /// <summary>
        /// 炼化
        /// </summary>
        Refinery=4,

        /// <summary>
        /// 镶嵌
        /// </summary>
        Insert=5,

        /// <summary>
        /// 转移
        /// </summary>
        Move=6,
    }
}
