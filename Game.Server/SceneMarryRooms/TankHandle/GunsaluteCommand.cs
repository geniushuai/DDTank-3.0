using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;
using Game.Server.SceneMarryRooms.TankHandle;
using Bussiness.Managers;
using Game.Server.Statics;
using Bussiness;
using Game.Server.Enumerate;

namespace Game.Server.SceneMarryRooms.TankHandle
{
    [MarryCommandAttbute((byte)MarryCmdType.GUNSALUTE)]
    public class GunsaluteCommand : IMarryCommandHandler
    {
        public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentMarryRoom != null/* && player.CurrentMarryRoom.RoomState == eRoomState.FREE*/)
            {
                int userID = packet.ReadInt();
                int templateID = packet.ReadInt();


                ItemTemplateInfo template = ItemMgr.FindItemTemplate(templateID);

                if(template != null)
                {
                    if (player.CurrentMarryRoom.Info.IsGunsaluteUsed == false && (player.CurrentMarryRoom.Info.GroomID == player.PlayerCharacter.ID
                        || player.CurrentMarryRoom.Info.BrideID == player.PlayerCharacter.ID))
                    {
                        player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
                        player.CurrentMarryRoom.Info.IsGunsaluteUsed = true;
                        GSPacketIn msg = player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("GunsaluteCommand.Successed1", player.PlayerCharacter.NickName));
                        player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(msg,player);
                        GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(player.CurrentMarryRoom.Info.GroomID, true, player.CurrentMarryRoom.Info);
                        GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(player.CurrentMarryRoom.Info.BrideID, true, player.CurrentMarryRoom.Info);

                        using (PlayerBussiness db = new PlayerBussiness())
                        {
                            db.UpdateMarryRoomInfo(player.CurrentMarryRoom.Info);
                        }

                        return true;
                    }
                    //未开始
                    //if(template.PayType == 0)
                    //{
                    //    if(player.PlayerCharacter.Gold >= template.Price1)
                    //    {
                    //        player.RemoveGold(template.Price1, GoldRemoveType.Firecrackers);
                    //        CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, 0, template.Price1, (int)eConsumerType.Marry, (int)eSubConsumerType.Marry_Gunsalute);
                    //        //0 player.CurrentMarryRoom.ReturnPacket(player, packet);
                    //        player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
                    //        GSPacketIn msg = player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("GunsaluteCommand.Successed1", player.PlayerCharacter.NickName));
                    //        player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(msg, player);
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
                    //        //0 player.CurrentMarryRoom.ReturnPacket(player, packet);
                    //        player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
                    //        GSPacketIn msg = player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("GunsaluteCommand.Successed1", player.PlayerCharacter.NickName));
                    //        player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(msg, player);
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
