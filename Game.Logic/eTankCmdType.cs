using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic
{
    public enum eTankCmdType
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
        USING_PROP = 0x20,
        FROST = 0x21,
        INVINCIBLY = 0x22,
        HIDE = 0x23,
        CARRY = 0x24,
        BECKON = 0x25,
        VANE = 0x26,
        RELIEVESPELLS = 0x27,
        FLY = 0x28,
        SEAL = 0x29,

        //战斗道具
        USEFIGHTPROP = 0x2a,
        ATTACKUP = 0x2b,
        SHOOTSTRAIGHT = 0x2c,
        ADDWOUND = 0x2d,
        ADDATTACK = 0x2e,
        ADDBALL = 0x2f,
        SECONDWEAPON = 0x54,

        //地图物品
        ADD_BOX = 0x30,
        PICK = 0x31,
        ARKPOINT = 0x32,
        TAKEOUT = 0x33,
        FIGHTPROP = 0x34,
        DISAPPEAR = 0x35,
        GHOST_TATGET = 0x36,
        LIVING_MOVETO = 0x37,
        LIVING_FALLING = 0x38,
        LIVING_JUMP = 0x39,
        LIVING_BEAT = 0x3a,
        LIVING_SAY = 0x3b,
        LIVING_PLAYMOVIE = 0x3c,
        LIVING_RANGEATTACKING = 0x3d,
        FOCUS_ON_OBJECT = 0x3e,
        PLAY_SOUND = 0x3f,
        ADD_LIVING = 0x40,
        ADD_BOARD = 0x41,
        UPDATE_BOARD_STATE = 0x42,
        LOAD_RESOURCE = 0x43,
        ADD_TIP = 0x44,

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


        SHOW_CARDS = 0x59,

        FIRE_TAG = 0x60,

        WANNA_LEADER = 0x61,

        TAKE_CARD = 0x62,

        START_GAME = 0x63,

        GAME_OVER = 0x64,

        GAME_CREATE = 0x65,

        GAME_CAPTAIN_AFFIRM = 0x66,

        GAME_LOAD = 0x67,

        GAME_UI_DATA = 0x68,

        GAME_MISSION_OVER = 0x70,

        GAME_MISSION_INFO = 0x71,

        PAYMENT_TAKE_CARD = 0x72,

        GAME_ALL_MISSION_OVER = 0x73,

        GAME_MISSION_PREPARE = 0x74,

        GAME_MISSION_START = 0x75,

        LIVING_STATE = 0x76,

        GAME_MISSION_TRY_AGAIN = 0x77,

        PLAY_INFO_IN_GAME = 0x78,

        GAME_ROOM_INFO = 0x79,

        SEND_PICTURE = 0x80,

        ATTACKEFFECT = 0x81,

        BOSS_TAKE_CARD = 0x82,

        GAME_TIME=0x83,
    }
}
