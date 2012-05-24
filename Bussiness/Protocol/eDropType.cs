using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bussiness.Protocol
{
    /// <summary>
    /// 掉落类型
    /// </summary>
    public enum eDropType
    {
        /// <summary>
        /// 翻牌
        /// </summary>
        Cards=1,

        /// <summary>
        /// 宝箱掉落
        /// </summary>
        Box=2,

        /// <summary>
        /// NPC掉落
        /// </summary>
        NPC=3,

        /// <summary>
        /// Boss掉落
        /// </summary>
        Boss=4,

        /// <summary>
        /// 副本掉落
        /// </summary>
        Copy=5,

        /// <summary>
        /// 特殊关掉落
        /// </summary>
        Special=6,

        /// <summary>
        /// PVP任务掉落
        /// </summary>
        PvpQuests=7,

        /// <summary>
        /// 开炮掉落
        /// </summary>
        Fire=8,

        /// <summary>
        /// PVE任务掉落
        /// </summary>
        PveQuests=9,

        /// <summary>
        /// 答题掉落
        /// </summary>
        Answer=10,
    }

 
}
