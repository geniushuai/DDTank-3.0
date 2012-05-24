using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Server.Spells;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Game.Server.Statics;
using Game.Server.Games.Cmd;
using Game.Server.Managers;

namespace Game.Server.Games.Cmd
{
    /// <summary>
    /// 使用道具协议
    /// </summary>
    [GameCommand((byte)TankCmdType.PROP,"使用道具")]
    public class PropUseCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, global::Phy.Object.Player player, GSPacketIn packet)
        {
            if (game.GameState != eGameState.Playing)
                return;

            if (player.IsAttacking || (player.IsLiving == false && player.Team == game.CurrentPlayer.Team))
            {
                int type = packet.ReadByte();
                int place = packet.ReadInt();
                int templateID = packet.ReadInt();

                ItemTemplateInfo template = PropItemMgr.FindPropBag(templateID);
                if(template == null)
                    return;

                //背包中的道具
                if (type == 1)
                {
                    if(player.IsLiving == false)
                        return;

                    if( place == -1 && player.PlayerDetail.BuffInventory.PropBag())
                    {
                        //无限道具buffer
                        player.UseItem(template);

                    }
                    else
                    {
                        ItemInfo item = player.PlayerDetail.PropBag.GetItemAt(place);
                        if(item != null && item.IsValidItem() && item.Count >= 0)
                        {
                            if(player.UseItem(PropItemMgr.FindPropBag(templateID)))
                            {
                                if(item.TemplateID == 10200)
                                {
                                     player.PlayerDetail.PropBag.UseItem(item);
                                     player.PlayerDetail.PropBag.RefreshItem(item);
                                }
                                else
                                {
                                    item.Count--;
                                    if (item.Count <= 0)
                                    {
                                        player.PlayerDetail.RemoveAllItem(item, false, ItemRemoveType.Use, 1);
                                    }
                                    else
                                    {
                                        StatMgr.LogItemRemove(item.TemplateID, ItemRemoveType.Use, 1);
                                        player.PlayerDetail.PropBag.RefreshItem(item);
                                    }
                                    game.AfterUseItem(item);
                                }
                                
                            }
                        }
                    }               
                }
                //道具栏的道具
                else
                {
                    ItemInfo item = player.PlayerDetail.PropInventory.GetItemAt(place);
                    if (item != null && player.UseItem(item.Template))
                    {
                        player.PlayerDetail.PropInventory.RemoveItemAt(place);
                    }
                }
            }
        }
    }
}
