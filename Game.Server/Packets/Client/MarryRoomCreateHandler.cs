using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.SceneMarryRooms;
using Game.Server.Statics;
using log4net;
using System.Reflection;
using Game.Server.Enumerate;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.MARRY_ROOM_CREATE, "礼堂创建")]
    public class MarryRoomCreateHandler : IPacketHandler
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {                        
            if (!client.Player.PlayerCharacter.IsMarried)
            {
                return 1;
            }

            if(client.Player.PlayerCharacter.IsCreatedMarryRoom)
            {
                //client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoomCreateHandler.Msg1"));
                return 1;
            }

            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {

                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 0;
            }
            
            if (client.Player.CurrentRoom != null)
            {
                client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
            }

            if (client.Player.CurrentMarryRoom != null)
            {
                client.Player.CurrentMarryRoom.RemovePlayer(client.Player);
            }

            //GamePlayer tempPlayer = WorldMgr.GetPlayerById(client.Player.PlayerCharacter.SpouseID);
            //if(tempPlayer != null)
            //{
            //    if (tempPlayer.IsMarryRommCreating)
            //    {
            //        client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoomCreateHandler.IsCreating"));
            //        return 1;
            //    }
            //}

            //client.Player.IsMarryRommCreating = true;

            MarryRoomInfo info = new MarryRoomInfo();
            info.Name = packet.ReadString().Replace(";", "");
            info.Pwd = packet.ReadString();
            info.MapIndex = packet.ReadInt();
            info.AvailTime = packet.ReadInt();
            info.MaxCount = packet.ReadInt();
            info.GuestInvite = packet.ReadBoolean();
            info.RoomIntroduction = packet.ReadString();
            info.ServerID = GameServer.Instance.Configuration.ServerID;
            info.IsHymeneal = false;

            string[] money = GameProperties.PRICE_MARRY_ROOM.Split(',');
            if(money.Length < 3)
            {
                if (log.IsErrorEnabled)
                    log.Error("MarryRoomCreateMoney node in configuration file is wrong");

                return 1;
            }

            int needMoney = 0;
            switch (info.AvailTime)
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
                    info.AvailTime = 4;
                    break;
            }

            if (client.Player.PlayerCharacter.Money >= needMoney)
            {
                MarryRoom room = MarryRoomMgr.CreateMarryRoom(client.Player, info);
                               
                if (room != null)
                {
                    client.Player.RemoveMoney(needMoney);
                    LogMgr.LogMoneyAdd(LogMoneyType.Marry, LogMoneyType.Marry_Room, client.Player.PlayerCharacter.ID, needMoney, client.Player.PlayerCharacter.Money, 0, 0, 0, "", "", "");
                    GSPacketIn pkg = client.Player.Out.SendMarryRoomInfo(client.Player, room);
                    client.Player.Out.SendMarryRoomLogin(client.Player, true);
                    room.SendToScenePlayer(pkg);

                    CountBussiness.InsertSystemPayCount(client.Player.PlayerCharacter.ID, needMoney, 0, (int)eConsumerType.Marry,(int)eSubConsumerType.Marry_MarryRoom);
                }

                return 0;
            }
            else
            {
                client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough"));
            }

            return 1;

        }

    }
}
