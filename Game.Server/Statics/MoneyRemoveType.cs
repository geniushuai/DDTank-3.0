using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Statics
{
    public enum MoneyRemoveType
    {
        /// <summary>
        /// 购买物品
        /// </summary>
        BuyItem,
        /// <summary>
        /// 购买道具
        /// </summary>
        BuyProp,
        /// <summary>
        /// 赠送物品
        /// </summary>
        Present,
        /// <summary>
        /// 续费
        /// </summary>
        CoutinueItem,
        /// <summary>
        /// 购买武器
        /// </summary>
        BuyWeapon,
        /// <summary>
        /// 其它
        /// </summary>
        Other,
        /// <summary>
        /// 
        /// </summary>
        Auction,
        /// <summary>
        /// 转移
        /// </summary>
        ItemTransfer,
        /// <summary>
        /// 离婚
        /// </summary>
        Divorce,
        /// <summary>
        /// 结婚
        /// </summary>
        Marry,
        /// <summary>
        /// 烟花
        /// </summary>
        Firecrackers,
        /// <summary>
        /// 公会捐献
        /// </summary>
        ConsortiaRichesOffer,
        /// <summary>
        /// Boss战
        /// </summary>
        Boss,
        /// <summary>
        /// 付费翻牌
        /// </summary>
        PaymentTakeCard,
        /// <summary>
        /// 关卡失败再试一次
        /// </summary>
        TryAgain,
    }
}
