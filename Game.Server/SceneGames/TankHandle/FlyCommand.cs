using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets.Client;
using Game.Server.Spells;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Bussiness;
using Game.Server.Statics;

namespace Game.Server.SceneGames.TankHandle
{
    /// <summary>
    /// 使用飞行功能协议
    /// </summary>
    [CommandAttbute((byte)TankCmdType.FLY)]
    public class FlyCommand : ICommandHandler
    {
        private static readonly int CARRY_TEMPLATE_ID = 10016;
        public bool HandleCommand(TankGameLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            //if (player.CurrentGame.Data.CurrentFire != null && player.CurrentGame.Data.CurrentIndex != player && !(player.CurrentGame.Data.Players[player].State == TankGameState.DEAD 
            //    && player.CurrentTeamIndex == player.CurrentGame.Data.CurrentIndex.CurrentTeamIndex) && player.CanFly == false)
            //    return false;

            if (player.CurrentGame.Data.CurrentFire != null )
                return false;
            if (player.CurrentGame.Data.CurrentIndex != player|| !player.CanFly)
                return false;

            player.CanFly = false;
            GSPacketIn pkg = player.Out.SendPropUseRespone(player, -2, -2, CARRY_TEMPLATE_ID);
            player.CurrentGame.SendToPlayerExceptSelf(pkg, player);

            //构造一个CARRY Item
            //ItemInfo CarryItem;
            //using (ProduceBussiness db = new ProduceBussiness())
            //{
            //    ItemTemplateInfo[] CarryTemplate = db.GetSingleCategory(10);

            //    foreach (ItemTemplateInfo tmp in CarryTemplate)
            //    {
            //        if (tmp.TemplateID == CARRY_TEMPLATE_ID)
            //        {
            //            CarryItem = ItemInfo.CreateFromTemplate(tmp, 1);

            //            if (CarryItem == null)
            //            {
            //                return false;
            //            }

            //            //if (player.CurrentGame.Data.CurrentIndex == player)
            //            //    player.CurrentGame.Data.TotalDelay += CarryItem.Template.Property5;

            //            ISpellHandler spellHandler = SpellMgr.LoadSpellHandler(CarryItem.Template.Property1);
            //            spellHandler.Execute(player.CurrentGame.Data.CurrentIndex, CarryItem);



            //            return true;
            //        }
            //    }
            //}

            ItemTemplateInfo templateInfo = Managers.PropItemMgr.FindAllProp(CARRY_TEMPLATE_ID);

            if (templateInfo != null)
            {
                ItemInfo CarryItem = ItemInfo.CreateFromTemplate(templateInfo, 1, (int)ItemAddType.TempProp);
                ISpellHandler spellHandler = SpellMgr.LoadSpellHandler(CarryItem.Template.Property1);
                spellHandler.Execute(player.CurrentGame.Data.CurrentIndex, CarryItem);
                return true;
            }

            return false;

        }
    }
}
