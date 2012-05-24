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

namespace Game.Server.SceneGames.TankHandle
{
    /// <summary>
    /// 使用道具协议
    /// </summary>
    [CommandAttbute((byte)TankCmdType.PROP)]
    public class PropUseCommand : ICommandHandler
    {
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            //if (player.CurrentGame.Data.CurrentFire != null)
            //    return false;
            //if (player.CurrentGame.Data.CurrentIndex != player && !(player.CurrentGame.Data.Players[player].State == TankGameState.DEAD && player.CurrentTeamIndex == player.CurrentGame.Data.CurrentIndex.CurrentTeamIndex))
            //    return false;

            //int type = packet.ReadByte();
            ////int place = packet.ReadByte();
            //int place = packet.ReadInt();
            //int templateID = packet.ReadInt();
            
            ////背包中的道具
            //if (type == 1)
            //{
            //    if (player.CurrentGame.Data.Players[player].State == TankGameState.DEAD)
            //        return false;

            //    ItemInfo item = player.PropBag.GetItemAt(place);
            //    bool isInfinity = false;

            //    if (item == null && place != -1)
            //        return false;
                
            //    //如果是全能道具
            //    if (place == -1 || item.TemplateID == 10200)
            //    {
            //        if (!player.BuffInventory.PropBag() && (item == null || !item.IsValidItem() || item.TemplateID != 10200))
            //        {
            //            return false;
            //        }

            //        isInfinity = true;
            //        if (item != null)
            //        {
            //            player.PropBag.UseItem(item);
            //            player.PropBag.RefreshItem(item);
            //        }

            //        //ItemTemplateInfo template = Managers.PropItemMgr.FindAllProp(templateID);
            //        ItemTemplateInfo template = Managers.PropItemMgr.FindPropBag(templateID);
            //        item = ItemInfo.CreateFromTemplate(template, 1, (int)ItemAddType.FullProp);
            //    }

            //    if (item != null && item.Count > 0 && item.IsValidItem() && player.CurrentGame.Data.Players[player].ReduceEnergy(item.Template.Property4))
            //    {
            //        player.CurrentGame.ReturnPacket(player, packet);

            //        if (player.CurrentGame.Data.CurrentIndex == player)
            //            player.CurrentGame.Data.TotalDelay += item.Template.Property5;

            //        ISpellHandler spellHandler = SpellMgr.LoadSpellHandler(item.Template.Property1);
            //        spellHandler.Execute(player.CurrentGame.Data.CurrentIndex, item);

            //        if (!isInfinity)
            //        {
            //            player.PropBag.UseItem(item);
            //            item.Count--;
            //            if (item.Count <= 0)
            //            {
            //                player.RemoveAllItem(item, false, Game.Server.Statics.ItemRemoveType.Use,1);
            //            }
            //            else
            //            {
            //                Statics.StatMgr.LogItemRemove(item.TemplateID, Game.Server.Statics.ItemRemoveType.Use, 1);
            //                player.PropBag.RefreshItem(item);
            //            }
            //        }
            //        player.QuestInventory.CheckUseItem(item.TemplateID);

            //        if (item.Template.Property6 == 1 && player == player.CurrentGame.Data.CurrentIndex)
            //            process.SendPlayFinish(player.CurrentGame, player.CurrentGame.Data.CurrentIndex);

            //        if (!isInfinity)
            //            player.CurrentGame.Data.CostInfo(item);
            //        return true;

            //    }

            //}
            ////道具栏的道具
            //else
            //{
            //    ItemInfo item = player.PropInventory.GetItemAt(place);               
            //    if (item != null && player.CurrentGame.Data.Players[player].ReduceEnergy(item.Template.Property4))
            //    {
            //        player.CurrentGame.ReturnPacket(player, packet);

            //        if (player.CurrentGame.Data.CurrentIndex == player)
            //            player.CurrentGame.Data.TotalDelay += item.Template.Property5;

            //        ISpellHandler spellHandler = SpellMgr.LoadSpellHandler(item.Template.Property1);
            //        spellHandler.Execute(player.CurrentGame.Data.CurrentIndex, item);

            //        player.PropInventory.RemoveItemAt(place);
            //        player.QuestInventory.CheckUseItem(item.TemplateID);

            //        if (item.Template.Property6 == 1 && player == player.CurrentGame.Data.CurrentIndex)
            //            process.SendPlayFinish(player.CurrentGame, player.CurrentGame.Data.CurrentIndex);
            //        return true;
            //    }
            //}
            return false;
        }
    }
}
