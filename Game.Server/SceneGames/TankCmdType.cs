using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.SceneGames
{
    public enum TankCmdType
    {
        MOVE = 0x01,
        FIRE = 0x02,
        BLAST = 0x03,
        TURN = 0x06,
        DIRECTION = 0x07,
        GUN_ROTATION = 0x08,
        MOVESTART = 0x09,
        MOVESTOP = 0x0a,
        HEALTH = 0x0b,
        SKIPNEXT = 0x0c,
        DELAY = 0x0d,
        DANDER = 0x0e,
        STUNT = 0x0f,

        LOAD = 0x10,
        SUICIDE = 0x11,
        ENERGY = 0x12,
        CHANGEBALL = 0x13,
        CURRENTBALL = 0x14,
        KILLSELF = 0x15,
        BEAT = 0x16,
        
        //道具
        PROP = 0x20,
        FROST = 0x21,
        INVINCIBLY = 0x22,
        HIDE = 0x23,
        CARRY = 0x24,
        BECKON = 0x25,
        VANE = 0x26,
        RELIEVESPELLS = 0x27,
        FLY = 0x28,


        //战斗道具
        USEFIGHTPROP = 0x2a,
        ATTACKUP = 0x2b,
        SHOOTSTRAIGHT = 0x2c,
        ADDWOUND = 0x2d,
        ADDATTACK = 0x2e,
        ADDBALL = 0x2f,


        //地图物品
        ARK = 0x30,
        PICK = 0x31,
        ARKPOINT = 0x32,
        TAKEOUT = 0x33,
        FIGHTPROP = 0x34,
        DISAPPEAR = 0x35,

        //添加战利品
        TEMP_INVENTORY  = 0x40,

                /// <summary>
        /// 穿甲
        /// </summary>
        BREACHDEFENCE = 0x51,
        /// <summary>
        /// 免坑
        /// </summary>
        NOHOLE = 0x52,
        /// <summary>
        /// 原子弹
        /// </summary>
        ABOMB = 0x53,


        FIRE_TAG = 0x60,
        PLAYFINISH = 0x70,
    }
}
