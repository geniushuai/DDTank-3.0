using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.ITEM_HIDE,"隐藏装备")]
    public class UserHideItemHandler:IPacketHandler
    {
        //修改:  Xiaov 
        //时间:  2009-11-7
        //描述:  隐藏装备<已测试>     
        public int HandlePacket(GameClient client,GSPacketIn packet)
        {
            bool hide = packet.ReadBoolean();
            int categroyID = packet.ReadInt();
            switch (categroyID)
            {
                case 13:
                    categroyID = 3;
                    break;
            }
            client.Player.HideEquip(categroyID, hide);
            return 0;
        }
    }
}
