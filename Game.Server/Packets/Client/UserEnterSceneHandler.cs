using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Rooms;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.SCENE_LOGIN, "Player enter scene.")]
    public class UserEnterSceneHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //时间:  2009-11-4
            //描述:  进入游戏大厅<将当前用户加入等待房间列表中>
           // RoomMgr.EnterWaitingRoom(client.Player);
            var typeScene = packet.ReadInt();
            switch (typeScene)
            {   
                    //RoomList
                case 501:
                case 2:
                    //RoomMgr.EnterWaitingRoom(client.Player);
                    break;
                   
                default:
                    break;
            }
            //GSPacketIn pkg = packet.Clone();

            //if (WorldMgr.WaitingScene.AddPlayer(client.Player))
            //{
            //    pkg.WriteBoolean(true);
            //}
            //else
            //{
            //    pkg.WriteBoolean(false);
            //}
            //client.Out.SendTCP(pkg);

            //if (client.Player.CurrentScene != null)
            //{
            //    BaseSceneGame[] list = GameMgr.GetAllGame();
            //    foreach (BaseSceneGame g in list)
            //    {
            //        //if (g.Count != 0 && g.GameState == eGameState.FREE)
            //        if (g.Count > 0)
            //        {
            //            client.Out.SendTCP(client.Out.SendRoomInfo(g.Player, g));
            //        }
            //    }

            //    GSPacketIn pkgMsg = null;
            //    GamePlayer[] players = client.Player.CurrentScene.GetAllPlayer();
            //    foreach (GamePlayer p in players)
            //    {
            //        if (p != client.Player && p.CurrentGame == null)
            //        {
            //            if (pkgMsg == null)
            //            {
            //                pkgMsg = p.Out.SendSceneAddPlayer(client.Player);

            //            }
            //            else
            //            {
            //                p.Out.SendTCP(pkgMsg);
            //            }

            //            client.Out.SendSceneAddPlayer(p);
            //        }
            //    }

            //    client.Out.SendPingTime();
            //}

            return 1;
        }
    }
}
