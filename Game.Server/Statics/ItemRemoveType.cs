using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Statics
{
    public enum ItemRemoveType
    {
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 1,
        /// <summary>
        /// 使用
        /// </summary>
        Use = 2,
        /// <summary>
        /// 邮箱
        /// </summary>
        Mail = 3,
        /// <summary>
        /// 强化失败
        /// </summary>
        StrengthFailed = 4,
        /// <summary>
        /// 强化
        /// </summary>
        Strengthen = 5,
        /// <summary>
        /// 合成
        /// </summary>
        Compose = 6,
        /// <summary>
        /// 熔化
        /// </summary>
        Fusion = 7,
        /// <summary>
        /// 拍卖
        /// </summary>
        Auction = 8,
        /// <summary>
        /// 其它 
        /// </summary>
        Other = 9,

        /// <summary>
        /// 叠加
        /// </summary>
        Fold = 10,

        /// <summary>
        /// 邮件删除
        /// </summary>
        MailDelete = 11,

        /// <summary>
        /// <summary>
        /// 结婚
        /// </summary>
        wedding = 12,
        
        /// <summary>
        /// 开火异常
        /// </summary>
        FireError = 21,
        /// <summary>
        /// 移动错误
        /// </summary>
        MoveError = 22,

        /// <summary>
        /// 移动错误
        /// </summary>
        FastError = 24,

        ItemTransfer = 23,

        ///<summary>
        ///炼化
        ///</summary>
        Refinery = 25,

    }
}
