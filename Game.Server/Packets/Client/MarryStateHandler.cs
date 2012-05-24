using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Game.Server.Statics;
using Bussiness;
using Game.Server.Rooms;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.SCENE_STATE, "当前场景状态")]
    class MarryStateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int stateID = packet.ReadInt();

            switch (stateID)
            {
                //客户端进入结婚房间已经准备完成
                case 0:
                    if (client.Player.IsInMarryRoom)
                    {
                        //GSPacketIn msgEnter = client.Out.SendPlayerEnterMarryRoom(client.Player);
                        if (client.Player.MarryMap == 1)
                        {
                            client.Player.X = 646;
                            client.Player.Y = 1241;
                        }
                        else if (client.Player.MarryMap == 2)
                        {
                            client.Player.X = 800;
                            client.Player.Y = 763;
                        }

                        GamePlayer[] list = client.Player.CurrentMarryRoom.GetAllPlayers();
                        foreach (GamePlayer p in list)
                        {
                            if (p != client.Player && p.MarryMap == client.Player.MarryMap)
                            {
                                p.Out.SendPlayerEnterMarryRoom(client.Player);
                                client.Player.Out.SendPlayerEnterMarryRoom(p);
                            }
                        }
                    }
                    break;
                //客户端进入大厅
                case 1:
                    RoomMgr.EnterWaitingRoom(client.Player);
                    break;
                     

            }

            return 0;
        }
    }
}
