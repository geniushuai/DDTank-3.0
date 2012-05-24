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
using Game.Server.Statics;
using Bussiness;
using log4net;
using System.Reflection;
using Game.Server.Enumerate;

namespace Game.Server.HotSpringRooms.TankHandle
{
    [HotSpringCommandAttribute((byte)HotSpringCmdType.CONTINUATION)]
    public class ContinuationCommand : IHotSpringCommandHandler
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public bool HandleCommand(TankHotSpringLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentHotSpringRoom == null)
            {
                return false;
            }

            if (player.PlayerCharacter.ID != player.CurrentHotSpringRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentHotSpringRoom.Info.BrideID)
            {
                return false;
            }

            int hour = packet.ReadInt();
            string[] money = GameProperties.PRICE_MARRY_ROOM.Split(',');
            if (money.Length < 3)
            {
                if (log.IsErrorEnabled)
                    log.Error("HotSpringRoomCreateMoney node in configuration file is wrong");

                return false;
            }

            int needMoney = 0;
            switch (hour)
            {
                case 2:
                    needMoney = int.Parse(money[0]);
                    break;
                case 3:
                    needMoney = int.Parse(money[1]);
                    break;
                case 4:
                    needMoney = int.Parse(money[2]);
                    break;
                default:
                    needMoney = int.Parse(money[2]);
                    hour = 4;
                    break;
            }

            if (player.PlayerCharacter.Money < needMoney)
            {
                player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg1"));
                return false;
            }

            //player.SetMoney(-needMoney, MoneyRemoveType.Marry);
            player.RemoveMoney(needMoney);
            LogMgr.LogMoneyAdd(LogMoneyType.Marry, LogMoneyType.Marry_RoomAdd, player.PlayerCharacter.ID, needMoney, player.PlayerCharacter.Money, 0, 0, 0, "", "", "");

            CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, needMoney, 0, (int)eConsumerType.Marry, (int)eSubConsumerType.Marry_HotSpringRoom);

            player.CurrentHotSpringRoom.RoomContinuation(hour);

            GSPacketIn pkg = player.Out.SendContinuation(player, player.CurrentHotSpringRoom.Info);

            int spouseID = 0;
            if (player.PlayerCharacter.ID == player.CurrentHotSpringRoom.Info.GroomID)
            {
                spouseID = player.CurrentHotSpringRoom.Info.BrideID;
            }
            else
            {
                spouseID = player.CurrentHotSpringRoom.Info.GroomID;
            }

            GamePlayer spouse = WorldMgr.GetPlayerById(spouseID);
            if (spouse != null)
            {
                spouse.Out.SendTCP(pkg);
            }
            
            player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ContinuationCommand.Successed"));
            
            return true;
        }
        
    }
}
