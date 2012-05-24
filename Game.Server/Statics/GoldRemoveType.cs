using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Statics
{
    public enum GoldRemoveType
    {
        /// <summary>
        /// 购买道具
        /// </summary>
        BuyProp,
        /// <summary>
        /// 购买物品
        /// </summary>
        BuyItem,
        /// <summary>
        /// 赠送物品
        /// </summary>
        Present,
        /// <summary>
        /// 强化
        /// </summary>
        Strengthen,
        /// <summary>
        /// 合成
        /// </summary>
        Compose,
        /// <summary>
        /// 熔化
        /// </summary>
        Fusion,
        /// <summary>
        /// 续费
        /// </summary>
        CoutinueItem,
        /// <summary>
        /// 邮箱
        /// </summary>
        Mail,
        /// <summary>
        /// 其它
        /// </summary>
        Other,
        /// <summary>
        /// 拍卖
        /// </summary>
        Auction,
        /// <summary>
        /// 
        /// </summary>
        Consortia,

        /// <summary>
        /// 物品转移
        /// </summary>
        ItemTransfer,

        /// <summary>
        /// 婚姻登记
        /// </summary>
        MarryInfo,
        /// <summary>
        /// 结婚
        /// </summary>
        Marry,
        /// <summary>
        /// 烟花
        /// </summary>
        Firecrackers,

        ///<summary>
        ///炼化
        ///</summary>
        Refinery,
    }
}
