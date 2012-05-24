using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.LINKREQUEST_GOODS, "物品比较")]
    public class ItemCompareHandler : IPacketHandler
    {
      
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {

            int Type = packet.ReadInt();
            if (Type == 2)
            {
                int ItemID = packet.ReadInt();

                using (PlayerBussiness db = new PlayerBussiness())
                {
                    ItemInfo Item = db.GetUserItemSingle(ItemID);
                    if (Item != null)
                    {
                        GSPacketIn pkg = new GSPacketIn((byte)ePackageType.LINKREQUEST_GOODS, client.Player.PlayerCharacter.ID);
                        pkg.WriteInt(Item.TemplateID);
                        pkg.WriteInt(Item.ItemID);

                        pkg.WriteInt(Item.StrengthenLevel);

                        pkg.WriteInt(Item.AttackCompose);

                        pkg.WriteInt(Item.AgilityCompose);

                        pkg.WriteInt(Item.LuckCompose);

                        pkg.WriteInt(Item.DefendCompose);
                        pkg.WriteInt(Item.ValidDate);
                        pkg.WriteBoolean(Item.IsBinds);
                        pkg.WriteBoolean(Item.IsJudge);
                        pkg.WriteBoolean(Item.IsUsed);
                        if (Item.IsUsed)
                        {
                            pkg.WriteString(Item.BeginDate.ToString());
                        }
                        pkg.WriteInt(Item.Hole1);
                        pkg.WriteInt(Item.Hole2);
                        pkg.WriteInt(Item.Hole3);
                        pkg.WriteInt(Item.Hole4);
                        pkg.WriteInt(Item.Hole5);
                        pkg.WriteInt(Item.Hole6);

                        pkg.WriteString(Item.Template.Hole);

                        client.Out.SendTCP(pkg);
                    }
                    return 1;
                }

            }
            return 0;
        }
    }
}
