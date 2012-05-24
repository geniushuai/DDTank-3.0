using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.SceneMarryRooms;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.MARRY_SCENE_LOGIN, "Player enter marry scene.")]
    public class UserEnterMarrySceneHandler : IPacketHandler
    {

        //修改:  Xiaov 
        //时间:  2009-11-7
        //描述:  进入结婚场景<已测试>   
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn pkg = packet.Clone();
            
            if (WorldMgr.MarryScene.AddPlayer(client.Player))
            {
                pkg.WriteBoolean(true);
            }
            else
            {
                pkg.WriteBoolean(false);
            }
            client.Out.SendTCP(pkg);
            
            if (client.Player.CurrentMarryRoom==null)
            {
                MarryRoom[] list = MarryRoomMgr.GetAllMarryRoom();
                
                foreach (MarryRoom g in list)
                {
                    client.Player.Out.SendMarryRoomInfo(client.Player, g);
                }                
            }
            return 0;

            //if (client.Player.CurrentScene == WorldMgr.MarryScene)
            //{
            //    MarryRoom[] list = MarryRoomMgr.GetAllMarryRoom();
            //    foreach (MarryRoom g in list)
            //    {
            //        client.Player.Out.SendMarryRoomInfo(client.Player, g);
            //    }

            //    client.Out.SendPingTime();
            //}
        }
    }
}
