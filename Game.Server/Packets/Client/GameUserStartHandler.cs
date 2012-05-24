using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using Game.Server.Rooms;
using Game.Logic;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.GAME_START,"游戏开始")]
    public class GameUserStartHandler:IPacketHandler
    {
        
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            BaseRoom room = client.Player.CurrentRoom;

          

            if (room != null &&　room.Host == client.Player)
            {
                if (client.Player.MainWeapon == null)
                {
                    client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
                    return 0;
                }

                if (room.RoomType == eRoomType.Treasure)
                {
                    if (!client.Player.IsPvePermission(room.MapId, room.HardLevel))
                    {
                        //TODO 写入语言包中，以便多语言转换
                        client.Player.SendMessage("您选择的难度等级不匹配，请重新选择");
                        return 0;
                    }
                }
                else if (room.RoomType == eRoomType.Boss)
                {
                    if (client.Player.RemoveMoney(100) <= 0)
                    {
                        client.Player.SendInsufficientMoney((int)eBattleRemoveMoneyType.Boss);
                        return 0;
                    }
                    else
                    {
                        LogMgr.LogMoneyAdd(LogMoneyType.Game, LogMoneyType.Game_Boos, client.Player.PlayerCharacter.ID, 100, client.Player.PlayerCharacter.Money, 0, 0, 0, "", "", "");
                    }
                }
 
                RoomMgr.StartGame(client.Player.CurrentRoom);
            }
            return 0;
        }
    }
}
