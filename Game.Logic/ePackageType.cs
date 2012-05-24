using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic
{
    public enum ePackageType
    {
        GAME_CMD = 0x5b,
        GAME_CHAT = 0x03,
        GAME_PLAYER_EXIT = 0x53,
        /// <summary>
        /// 获取全部BUFFER
        /// </summary>
        BUFF_OBTAIN = 0xba,
    }
}
