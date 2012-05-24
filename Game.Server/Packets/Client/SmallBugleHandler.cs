using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.S_BUGLE, "小喇叭")]
    public class SmallBugleHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
 
            //修改:  Xiaov 
            //时间:  2009-11-4
            //描述:  小喇叭<未测试>
            
            ItemInfo item = client.Player.PropBag.GetItemByCategoryID(0,11, 4);
            if (item != null)
            {
                //item.Count--;
                //if (item.Count <= 0)
                //{
                //    client.Player.RemoveItem(item);
                //}
                //else
                //{                    
                    client.Player.PropBag.RemoveCountFromStack(item, 1);                   
              //  }
                int senderID = packet.ReadInt();
                string senderName = packet.ReadString();
                string msg = packet.ReadString();
                GSPacketIn pkg = packet.Clone();
                pkg.ClearContext();
                pkg.ClientID = (client.Player.PlayerCharacter.ID);
                pkg.WriteInt(client.Player.PlayerCharacter.ID);
                pkg.WriteString(client.Player.PlayerCharacter.NickName);
                pkg.WriteString(msg);


                GamePlayer[] players = Managers.WorldMgr.GetAllPlayers();

                foreach (GamePlayer p in players)
                {
                    p.Out.SendTCP(pkg);
                }

            }

            return 0;
        }
    }
}
