using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Games
{
    public enum eMapType
    {
         /// <summary>
        /// 普通战地图
        /// </summary>
        Normal = 0x01,
        /// <summary>
        /// 撮合战地图
        /// </summary>
        PairUp = 0x02,
        /// <summary>
        /// 竞技场地图
        /// </summary>
        Arena = 0x04,
        /// <summary>
        /// 副本地图
        /// </summary>
        Duplicate = 0x08,
    }
}
