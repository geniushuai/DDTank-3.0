using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using SqlDataProvider.Data;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.UPDATE_GP, "修改邮件的已读未读标志")]
    public class UserUpdateGPHandler : IPacketHandler
    {
        //修改:  TrieuLSL 
        //时间:  2009-11-4
        //描述:  修改邮件的已读未读标志<已测试>    
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //GSPacketIn pkg = packet.Clone();
            //pkg.ClearContext();

            //int id = packet.ReadInt();
            int gp = packet.ReadInt();
            client.Player.BeginChanges();
            client.Player.AddGP(gp);
            client.Player.CommitChanges();
            //client.Out.SendTCP(pkg);

            return 0;
        }
    }
}
