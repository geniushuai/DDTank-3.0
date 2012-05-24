using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.SceneMarryRooms
{
    public enum MarryCmdType
    {
        MOVE = 0x01,
        HYMENEAL_BEGIN = 0x02,
        CONTINUATION = 0x03,
        INVITE = 0x04,
        LARGESS = 0x05,
        USEFIRECRACKERS = 0x06,
        KICK = 0x07,
        FORBID = 0x08,
        HYMENEAL_STOP = 0x09,
        POSITION = 0x0a,
        GUNSALUTE = 0x0b,
    }
}
