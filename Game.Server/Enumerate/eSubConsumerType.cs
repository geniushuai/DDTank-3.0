using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Enumerate
{
    public enum eSubConsumerType
    {
        /// <summary>
        /// 创建结婚礼堂
        /// </summary>
        Marry_MarryRoom = 0,
        /// <summary>
        /// 结婚典礼
        /// </summary>
        Marry_Hymeneal = 1,
        /// <summary>
        /// 礼炮
        /// </summary>
        Marry_Gunsalute = 2,
        /// <summary>
        /// 离婚
        /// </summary>
        Marry_Divorce = 3,
        /// <summary>
        /// 转移
        /// </summary>
        Smithy_Transfer = 4,
        /// <summary>
        /// 公会捐献
        /// </summary>
        Consortia_Riches_Offer = 5,
    }
}
