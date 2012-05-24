using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.ITEM_EQUIP,"获取用户装备")]
    public class UserEquipListHandler:IPacketHandler 
    {
        //修改:  Xiaov 
        //时间:  2009-11-7
        //描述:  获取用户装备<已测试>   
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            GamePlayer player = Managers.WorldMgr.GetPlayerById(id);
            PlayerInfo info;
            List<ItemInfo> items;
            if (player != null)
            {
                info = player.PlayerCharacter;
                items = player.MainBag.GetItems(0, 31);
            }
            else
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    info = pb.GetUserSingleByUserID(id);
                    items = pb.GetUserEuqip(id);
                }
            }

            if (info != null && items != null)
                client.Out.SendUserEquip(info, items);
            return 0;
        }
    }
}
