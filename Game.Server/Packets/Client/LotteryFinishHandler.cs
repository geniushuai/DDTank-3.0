using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Bussiness.Managers;
using SqlDataProvider.Data;
using Game.Server.GameUtils;
using Bussiness;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.LOTTERY_FINISH, "打开物品")]
    public class LotteryFinishBoxHandler : IPacketHandler
    {


        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int bagType = packet.ReadByte();
            int place = packet.ReadInt();
            PlayerInventory arkBag = client.Player.CaddyBag;
            PlayerInventory propBag = client.Player.PropBag;
            for (int i = 0; i < arkBag.Capalility; i++)
            {
                var item = arkBag.GetItemAt(i);
                if (item != null)
                {
                    arkBag.MoveToStore(arkBag, i, propBag.FindFirstEmptySlot(0), propBag, 999);
                }
            }
            return 1;
        }
    }
}
