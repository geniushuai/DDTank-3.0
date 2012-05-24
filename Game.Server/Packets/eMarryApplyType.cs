using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Packets
{
    public enum eMarryApplyType
    {
        /// <summary>
        /// 默认
        /// </summary>  
        Default = 0,
        /// <summary>
        /// 求婚
        /// </summary>
        Courtship = 1,
        /// <summary>
        /// 接受求婚
        /// </summary>
        Accept = 2,
        /// <summary>
        /// 离婚
        /// </summary>
        Divorce = 3,
    }
}
