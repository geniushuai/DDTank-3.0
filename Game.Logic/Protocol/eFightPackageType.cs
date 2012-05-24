using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Protocol
{
    public enum eFightPackageType
    {
        /**************************************系统*********************************************
         * */
        /// <summary>
        /// 服务器Ready
        /// </summary>
        RSAKey = 0x00,
        /* 
        * 登陆 
        */
        LOGIN = 0x01,

        SEND_TO_GAME = 0x02,

        CHAT = 0x13,

        DISCONNECT = 0x53,

        SYS_NOTICE = 0x03,
        /*************************************用户**********************************************
         **/

        SEND_TO_USER = 0x20,

        SEND_GAME_PLAYER_ID = 0x21,

        DISCONNECT_PLAYER = 0x22,

        PLAYER_ON_GAME_OVER = 0x23,

        PLAYER_USE_PROP_INGAME = 0x24,

        PLAYER_ADD_ITEM = 0x25,

        PLAYER_ADD_GOLD = 0x26,

        PLAYER_ADD_GP = 0x27,

        PLAYER_ONKILLING_LIVING = 0x28,

        PLAYER_ONMISSION_OVER = 0x29,

        PLAYER_CONSORTIAFIGHT = 0x2A,

        PLAYER_SEND_CONSORTIAFIGHT = 0x2B,

        PLAYER_REMOVE_GOLD = 0x2C,

        PLAYER_REMOVE_MONEY = 0x2D,

        PLAYER_ADD_TEMPLATE = 0x2E,

        PLAYER_ADD_TEMPLATE1 = 0x30,

        PLAYER_REMOVE_GP = 0x31,

        PLAYER_REMOVE_OFFER = 0x32,

        /*************************************房间**********************************************
        **/

        ROOM_CREATE = 0x40,

        ROOM_REMOVE = 0x41,

        ROOM_START_GAME = 0x42,

        SEND_TO_ROOM = 0x43,

        ROOM_STOP_GAME = 0x44,

        FIND_CONSORTIA_ALLY =0x45,



        PLAYER_ADD_MONEY = 0x46,

        PLAYER_ADD_GIFTTOKEN = 0x47,
    }
}
