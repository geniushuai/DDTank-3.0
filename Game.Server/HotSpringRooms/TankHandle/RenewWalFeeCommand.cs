using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Server.HotSpringRooms;
using Game.Server.HotSpringRooms.TankHandle;
using Bussiness.Managers;
using Game.Server.Statics;
using Bussiness;
using Game.Server.Enumerate;

namespace Game.Server.HotSpringRooms.TankHandle
{
    [HotSpringCommandAttribute((byte)HotSpringCmdType.HOTSPRING_ROOM_RENEWAL_FEE)]
    public class RenewWalFeeCommand : IHotSpringCommandHandler
    {
        public bool HandleCommand(TankHotSpringLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentHotSpringRoom != null/* && player.CurrentHotSpringRoom.RoomState == eRoomState.FREE*/)
            {
                int userID = packet.ReadInt();
                int templateID = packet.ReadInt();


                ItemTemplateInfo template = ItemMgr.FindItemTemplate(templateID);

                if(template != null)
                {
                    //if (player.CurrentHotSpringRoom.Info.IsGunsaluteUsed == false && (player.CurrentHotSpringRoom.Info.GroomID == player.PlayerCharacter.ID
                    //    || player.CurrentHotSpringRoom.Info.BrideID == player.PlayerCharacter.ID))
                    //{
                    //    player.CurrentHotSpringRoom.ReturnPacketForScene(player, packet);
                    //    player.CurrentHotSpringRoom.Info.IsGunsaluteUsed = true;
                    //    GSPacketIn msg = player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("GunsaluteCommand.Successed1", player.PlayerCharacter.NickName));
                    //    player.CurrentHotSpringRoom.SendToPlayerExceptSelfForScene(msg,player);
                    //    GameServer.Instance.LoginServer.SendHotSpringRoomInfoToPlayer(player.CurrentHotSpringRoom.Info.GroomID, true, player.CurrentHotSpringRoom.Info);
                    //    GameServer.Instance.LoginServer.SendHotSpringRoomInfoToPlayer(player.CurrentHotSpringRoom.Info.BrideID, true, player.CurrentHotSpringRoom.Info);

                    //    using (PlayerBussiness db = new PlayerBussiness())
                    //    {
                    //        db.UpdateHotSpringRoomInfo(player.CurrentHotSpringRoom.Info);
                    //    }

                    //    return true;
                    //}
                    //未开始
                    //if(template.PayType == 0)
                    //{
                    //    if(player.PlayerCharacter.Gold >= template.Price1)
                    //    {
                    //        player.RemoveGold(template.Price1, GoldRemoveType.Firecrackers);
                    //        CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, 0, template.Price1, (int)eConsumerType.Marry, (int)eSubConsumerType.Marry_Gunsalute);
                    //        //0 player.CurrentHotSpringRoom.ReturnPacket(player, packet);
                    //        player.CurrentHotSpringRoom.ReturnPacketForScene(player, packet);
                    //        GSPacketIn msg = player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("GunsaluteCommand.Successed1", player.PlayerCharacter.NickName));
                    //        player.CurrentHotSpringRoom.SendToPlayerExceptSelfForScene(msg, player);
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("GunsaluteCommand.GoldNotEnough"));
                    //    }
                    //}
                    //else
                    //{
                    //    if(player.PlayerCharacter.Money >= template.Price1)
                    //    {
                    //        player.RemoveMoney(template.Price1, MoneyRemoveType.Firecrackers);
                    //        CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, template.Price1, 0, (int)eConsumerType.Marry, (int)eSubConsumerType.Marry_Gunsalute);
                    //        //0 player.CurrentHotSpringRoom.ReturnPacket(player, packet);
                    //        player.CurrentHotSpringRoom.ReturnPacketForScene(player, packet);
                    //        GSPacketIn msg = player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("GunsaluteCommand.Successed1", player.PlayerCharacter.NickName));
                    //        player.CurrentHotSpringRoom.SendToPlayerExceptSelfForScene(msg, player);
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("GunsaluteCommand.MoneyNotEnough"));
                    //    }
                    //}
                }
            }
            return false;
        }
        
    }
}
