using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Rooms;
using Game.Logic;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.SCENE_REMOVE_USER, "场景用户离开")]
    public class UserLeaveSceneHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {


            RoomMgr.ExitWaitingRoom(client.Player);
            //修改:  Xiaov 
            //时间:  2009-11-4
            //描述:  用户离开<房间分成两类：一种是房间里，一种是等待房间。为了分享聊天信息，所有等待房间均可收到聊天信息>                  
            return 0;
        }
    }
}
