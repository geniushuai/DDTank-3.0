using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Logic;
using Game.Server.GameUtils;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.SCENE_READY,"Client scene ready1")]
    public class UserSceneReadyHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {

            //if (client.Player.CurrentScene != null)
            //{
            //    GSPacketIn pkgMsg = null;
            //    GamePlayer[] players = client.Player.CurrentScene.GetAllPlayer();
            //    foreach (GamePlayer p in players)
            //    {
            //        //if (p != client.Player && p.CurrentGame == null)
            //        if (p != client.Player)
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
            //}

            //修改:  Xiaov 
            //时间:  2009-11-4
            //描述:  场景准备<已通过>
            if (client.Player.CurrentRoom != null)
            {
                GSPacketIn pkgMsg = null;
                List<GamePlayer> players = client.Player.CurrentRoom.GetPlayers();
                foreach (GamePlayer p in players)
                {
                    if (p != client.Player)
                    {
                        if (pkgMsg == null)
                        {
                            pkgMsg = p.Out.SendSceneAddPlayer(client.Player);
                        }
                        else
                        {
                            p.Out.SendTCP(pkgMsg);
                        }
                        client.Out.SendSceneRemovePlayer(p);
                    }
                }
            }
            
            

            return 1;
        }
    }
}
