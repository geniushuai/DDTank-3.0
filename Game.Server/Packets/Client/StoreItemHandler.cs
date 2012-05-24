using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;
using Game.Server.GameUtils;
using Game.Server.Managers;


namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.ITEM_STORE, "储存物品")]
    public class StoreItemHandler:IPacketHandler
    {
     
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {

           
            //TODO: 储存物品
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 1;

            int bagType = packet.ReadByte();
            int bagPlace = packet.ReadInt();
            int storePlace = packet.ReadInt();

            if (bagType == 0 && bagPlace < 31)
                return 1;

            ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
            if (info != null)
            {
                PlayerInventory storeBag = client.Player.StoreBag;
                PlayerInventory toBag = client.Player.GetInventory((eBageType)bagType);
               // client.Player.StoreBag.MoveToStore(storeBag, storePlace, bagPlace, toBag, info.StoreLevel * 10);
            }

            return 0;
        }

        //public void MoveToStore(int fromSolt, int toSolt, int bagType)
        //{
        //    IBag bag = _player.GetBag(bagType);
        //    if (bag == null)
        //        return;

        //    ItemInfo bagItem = bag.GetItemAt(toSolt);
        //    ItemInfo storeItem = _player.StoreBag.GetItemAt(fromSolt);

        //    if (bagItem == null && storeItem == null)
        //        return;

        //    if (storeItem != null && storeItem.GetBagType() != bagType)
        //        return;

        //    if (storeItem == null && !IsSolt(fromSolt))
        //    {
        //        fromSolt = FindFirstEmptySlot(_bagStart);
        //    }

        //    if (bagItem == null && !bag.IsSolt(toSolt))
        //    {
        //        toSolt = bag.FindFirstEmptySlot(_bagStart);
        //    }

        //    if (fromSolt == -1 || toSolt == -1)
        //    {
        //        if (bagItem != null)
        //            _player.Out.SendUpdateInventorySlot(bagItem.Place, true, bagItem, _bagType);

        //        if (storeItem != null)
        //            _player.Out.SendUpdateInventorySlot(storeItem.Place, true, storeItem, _bagType);
        //    }
        //    else
        //    {
        //        if (storeItem != null)
        //        {
        //            RemoveItem(storeItem);
        //            bag.AddItemByPlace(bagItem, toSolt);
        //        }

        //        if(bagItem!=null)
        //        {
        //            bag.RemoveItem(bagItem);
        //            AddItemByPlace(storeItem,fromSolt);
        //        }


        //    }

        //}
    }
}
