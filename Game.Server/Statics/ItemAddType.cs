using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Statics
{
    public enum ItemAddType
    {
        /// <summary>
        /// 捡取
        /// </summary>
        MapDrop = 101,
        /// <summary>
        /// 购买
        /// </summary>
        Buy = 102,
        /// <summary>
        /// 赠送
        /// </summary>
        Gift = 103,
        /// <summary>
        /// 邮箱
        /// </summary>
        Mail = 104,
        /// <summary>
        /// 熔化
        /// </summary>
        Fusion = 105,
        /// <summary>
        /// 任务 
        /// </summary>
        Quest = 106,
        /// <summary>
        /// 其它
        /// </summary>
        Other = 107,

        /// <summary>
        /// 全能道具
        /// </summary>
        FullProp = 108,

        /// <summary>
        /// 临时道具
        /// </summary>
        TempProp = 109,

        /// <summary>
        /// 开箱子
        /// </summary>
        OpenArk = 110,

        /// <summary>
        /// 验证码
        /// </summary>
        CheckCode = 111,

        /// <summary>
        /// 结婚
        /// </summary>
        webing = 112,

        /// <summary>
        /// 日赠送
        /// </summary>
        DailyAward = 113,

        ///<summary>
        ///炼化
        ///</summary>
        Refinery = 114,

        /// <summary>
        /// 倾向转移
        /// </summary>
        RefineryTrend = 115,

        /// <summary>
        /// 强化
        /// </summary>
        Strengthen = 116,

        /// <summary>
        /// 上缴
        /// </summary>
        TurnProperty=117,

        /// <summary>
        /// 战斗中获得
        /// </summary>
        FightGet=118,

    }
}
