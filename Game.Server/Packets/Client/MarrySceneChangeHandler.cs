using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.MARRY_SCENE_CHANGE, "结婚场景切换")]
    class MarrySceneChangeHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        { 
            if(client.Player.CurrentMarryRoom == null || client.Player.MarryMap == 0)
            {
                return 1;
            }

            int sceneID = packet.ReadInt();

            if(sceneID == client.Player.MarryMap)
            {
                return 1;
            }

            //GSPacketIn pkg_leave = client.Player.Out.SendPlayerLeaveMarryRoom(client.Player);
            GSPacketIn pkg_leave = new GSPacketIn((byte)ePackageType.PLAYER_EXIT_MARRY_ROOM, client.Player.PlayerCharacter.ID);
            client.Player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(pkg_leave, client.Player);

            client.Player.MarryMap = sceneID;

            if (sceneID == 1)
            {
                client.Player.X = 514;
                client.Player.Y = 637;
            }
            else if(sceneID == 2)
            { 
                client.Player.X = 800;
                client.Player.Y = 763;
            }

            //GSPacketIn pkg_enter = client.Player.Out.SendMarryRoomLogin(client.Player, true);
            //client.Player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(pkg_enter, client.Player);

            GamePlayer[] list = client.Player.CurrentMarryRoom.GetAllPlayers();
            foreach (GamePlayer p in list)
            {
                if (p != client.Player && p.MarryMap == client.Player.MarryMap)
                {
                    p.Out.SendPlayerEnterMarryRoom(client.Player);
                    client.Player.Out.SendPlayerEnterMarryRoom(p);
                }
            }

            return 0;
        }

    }
}
