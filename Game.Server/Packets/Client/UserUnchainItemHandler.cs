using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using SqlDataProvider.Data;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.UNCHAIN_ITEM, "解除物品")]
    public class UserUnchainItemHandler : IPacketHandler
    {
        //修改:  Xiaov 
        //时间:  2009-11-4
        //描述:  解除物品<未用到>    
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //游戏中不能换装
            if (client.Player.CurrentRoom != null && client.Player.CurrentRoom.IsPlaying == true)
                return 0;

            int start = packet.ReadInt();
            int place = client.Player.MainBag.FindFirstEmptySlot(31);
            client.Player.MainBag.MoveItem(start, place,0);

            return 0;
        }
    }
}
