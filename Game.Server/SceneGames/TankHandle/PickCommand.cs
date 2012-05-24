using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Server.Spells;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Base.Packets;

namespace Game.Server.SceneGames.TankHandle
{
    /// <summary>
    /// 战斗中拾取宝箱
    /// </summary>
    [CommandAttbute((byte)TankCmdType.PICK)]
    public class PickCommand:ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            //if (player.CurrentGame.Data.CurrentIndex != player && player.CurrentGame.Data.Players[player].State != TankGameState.DEAD)
            //    return false;

            player.CurrentGame.ReturnPacket(player, packet);
            MapGoodsInfo goods = null;
            if (player.CurrentGame.Data.Players[player].State == TankGameState.DEAD)
            {
                int arkID = packet.ReadInt();
                goods = player.CurrentGame.Data.GetFallItemsID(arkID, player);
            }
            else
            {
                int arkID = packet.ReadInt();
                int bombID = packet.ReadInt();
                int time = packet.ReadInt();
                goods = player.CurrentGame.Data.GetFallItemsID(arkID, player.PlayerCharacter.ID);

            }
            if (goods != null)
            {
                ItemTemplateInfo temp = Bussiness.Managers.ItemMgr.GetSingleGoods(goods.GoodsID);
                if (temp != null)
                {
                    //如果是道具
                    if (temp.CategoryID == 10)
                    {
                        if (player.PropInventory.AddItemTemplate(temp) == null)
                        {
                            player.Out.SendMessage(Game.Server.Packets.eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.TankHandle.PropFull"));
                        }
                    }
                    else
                    {
                        player.TempInventory.AddItemTemplate(temp, goods);
                    }
                }
            }

            return true;
        }
    }
}
