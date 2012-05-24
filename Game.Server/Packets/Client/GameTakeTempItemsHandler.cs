using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.GameUtils;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.GAME_TAKE_TEMP,"选取")]
    public class GameTakeTempItemsHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client,GSPacketIn packet)
        {
            string message = string.Empty;
            int place = packet.ReadInt();
            if (place != -1)             //单选战利品
            {
                ItemInfo item = client.Player.TempBag.GetItemAt(place);
                GetItem(client.Player,item,ref message);
            }
            else                        //全选战利品
            {
                List<ItemInfo> items = client.Player.TempBag.GetItems();
                foreach (ItemInfo item in items)
                {
                    if (GetItem(client.Player, item, ref message) == false)
                    {
                        break;
                    }
                }
            }
            client.Player.SaveIntoDatabase();
            if (!string.IsNullOrEmpty(message))
                client.Out.SendMessage(eMessageType.ERROR, message);

            return 0;
        }
        /// <summary>
        /// 从临时背包中的数据移到背包中
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="item">物品</param>
        /// <param name="message">返回消息</param>
        private bool GetItem(GamePlayer player, ItemInfo item,ref string message)
        {
            if (item == null) return false;

            PlayerInventory bag = player.GetItemInventory(item.Template);
            if (bag.AddItem(item))
            {
                player.TempBag.RemoveItem(item);
                item.IsExist = true;
                return true;
            }
            else
            {
                bag.UpdateChangedPlaces();
                message = LanguageMgr.GetTranslation(item.GetBagName()) + LanguageMgr.GetTranslation("GameTakeTempItemsHandler.Msg");
            }
            return false;
        }
    }
}
