using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Game.Server.Rooms;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.SCENE_SMILE, "用户场景表情")]
    public class SceneSmileHandler : IPacketHandler
    {
        //修改:  Xiaov 
        //时间:  2009-11-7
        //描述:  用户发送表情动作<已测试>    
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            packet.ClientID = (client.Player.PlayerCharacter.ID);

            if (client.Player.CurrentRoom != null)
            {
                client.Player.CurrentRoom.SendToAll(packet);
            }
            else
            {
                if (client.Player.CurrentMarryRoom != null)
                {
                    //0 client.Player.CurrentMarryRoom.SendToAll(packet);
                    client.Player.CurrentMarryRoom.SendToAllForScene(packet, client.Player.MarryMap);
                }
                else
                {
                    RoomMgr.WaitingRoom.SendToALL(packet);
                }
            }
            return 1;
        }
    }
}
