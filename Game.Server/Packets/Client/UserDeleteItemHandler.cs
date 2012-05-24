using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.DELETE_ITEM,"删除物品")]
    public class UserDeleteItemHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {

            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 0;
            }


            int bagType = packet.ReadByte();
            int place = packet.ReadInt();

            //IBag bag = client.Player.GetBag(bagType);
            //ItemInfo goods = bag.GetItemAt(place);
            //if (goods != null && goods.Template.CanDelete)
            //{
            //    client.Player.RemoveAllItem(goods,false, Game.Server.Statics.ItemRemoveType.Delete,bagType);
            //}

            return 0;
        }
    }
}
