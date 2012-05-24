using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.SceneMarryRooms;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.PLAYER_EXIT_MARRY_ROOM, "玩家退出礼堂")]
    public class UserLeaveMarryRoom : IPacketHandler
    {
        //修改:  Xiaov 
        //时间:  2009-11-7
        //描述:  玩家退出礼堂<已测试>
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.IsInMarryRoom )
            {
                 //client.Player.CurrentMarryRoom.ReturnPacket(client.Player, packet);
                 client.Player.CurrentMarryRoom.RemovePlayer(client.Player);
                //client.Player.CurrentMarryRoom.SendToPlayerExceptSelf(packet,client.Player);
            }

            return 0;
        }
    }
}

