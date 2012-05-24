using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Spells
{
    public enum eSpellType
    {
        _Count = 0,
        ADD_LIFE = 1,
        FROST = 2,
        HIDE = 3,
        INVINCIBLY = 4,
        CARRY = 5,
        BECKON = 6,
        VANE = 7,

        /// <summary>
        /// 穿甲
        /// </summary>
        BREACHDEFENCE = 8,
        /// <summary>
        /// 免坑
        /// </summary>
        NOHOLE = 9,
        /// <summary>
        /// 原子弹
        /// </summary>
        ABOMB = 10,


        ATTACKUP = 11,
        SHOOTSTRAIGHT = 12,
        ADDWOUND = 13,
        ADDATTACK = 14,
        ADDBALL = 15,

        STUNT = 20,
    }
}
