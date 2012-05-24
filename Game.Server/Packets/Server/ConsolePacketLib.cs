using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;

namespace Game.Server.Packets.Server
{
    public class ConsolePacketLib : IPacketLib
    {
        public GSPacketIn SendMessage(eMessageType type, string message)
        {
            Console.WriteLine(message);
            return null;
        }

        #region IPacketLib Members

        public void SendTCP(GSPacketIn packet)
        {
            throw new NotImplementedException();
        }

        public void SendLoginSuccess()
        {
            throw new NotImplementedException();
        }
        public void SendLoginSuccess2()
        {
            throw new NotImplementedException();
        }

        public void SendCheckCode()
        {
            throw new NotImplementedException();
        }

        public void SendLoginFailed(string msg)
        {
            throw new NotImplementedException();
        }

        public void SendKitoff(string msg)
        {
            throw new NotImplementedException();
        }

        public void SendEditionError(string msg)
        {
            throw new NotImplementedException();
        }

        public void SendDateTime()
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendDailyAward(Game.Server.GameObjects.GamePlayer player)
        {
            throw new NotImplementedException();
        }

        public void SendPingTime(Game.Server.GameObjects.GamePlayer player)
        {
            throw new NotImplementedException();
        }

        public void SendUpdatePrivateInfo(SqlDataProvider.Data.PlayerInfo info)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendUpdatePublicPlayer(SqlDataProvider.Data.PlayerInfo info)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendNetWork(int id, long delay)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendUserEquip(SqlDataProvider.Data.PlayerInfo info, List<SqlDataProvider.Data.ItemInfo> items)
        {
            throw new NotImplementedException();
        }

        
        public void SendWaitingRoom(bool result)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendUpdateRoomList(Game.Server.Rooms.BaseRoom room)
        {
            throw new NotImplementedException();
        }
        public GSPacketIn SendUpdateRoomList(List<Game.Server.Rooms.BaseRoom> room)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendSceneAddPlayer(Game.Server.GameObjects.GamePlayer player)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendSceneRemovePlayer(Game.Server.GameObjects.GamePlayer player)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendRoomCreate(Game.Server.Rooms.BaseRoom room)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendRoomLoginResult(bool result)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendRoomPlayerAdd(Game.Server.GameObjects.GamePlayer player)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendRoomPlayerRemove(Game.Server.GameObjects.GamePlayer player)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendRoomUpdatePlayerStates(byte[] states)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendRoomUpdatePlacesStates(int[] states)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendRoomPlayerChangedTeam(Game.Server.GameObjects.GamePlayer player)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendRoomPairUpStart(Game.Server.Rooms.BaseRoom room)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendRoomPairUpCancel(Game.Server.Rooms.BaseRoom room)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendEquipChange(Game.Server.GameObjects.GamePlayer player, int place, int goodsID, string style)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendRoomChange(Game.Server.Rooms.BaseRoom room)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendFusionPreview(Game.Server.GameObjects.GamePlayer player, Dictionary<int, double> previewItemList, bool isBind, int MinValid)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendFusionResult(Game.Server.GameObjects.GamePlayer player, bool result)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendRefineryPreview(Game.Server.GameObjects.GamePlayer player, int templateid, bool isbind, SqlDataProvider.Data.ItemInfo item)
        {
            throw new NotImplementedException();
        }

        public void SendUpdateInventorySlot(Game.Server.GameUtils.PlayerInventory bag, int[] updatedSlots)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendFriendRemove(int FriendID)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendFriendState(int playerID, bool state)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendUpdateBuffer(Game.Server.GameObjects.GamePlayer player, SqlDataProvider.Data.BufferInfo[] infos)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendBufferList(Game.Server.GameObjects.GamePlayer player, List<Game.Server.Buffer.AbstractBuffer> infos)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendUpdateQuests(Game.Server.GameObjects.GamePlayer player, byte[] states, Game.Server.Quests.BaseQuest[] quests)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendMailResponse(int playerID, eMailRespose type)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendAuctionRefresh(SqlDataProvider.Data.AuctionInfo info, int auctionID, bool isExist, SqlDataProvider.Data.ItemInfo item)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendIDNumberCheck(bool result)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendAASState(bool result)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendAASInfoSet(bool result)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendAASControl(bool result, bool IsAASInfo, bool IsMinor)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendGameRoomInfo(Game.Server.GameObjects.GamePlayer player, Game.Server.Rooms.BaseRoom game)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendMarryInfoRefresh(SqlDataProvider.Data.MarryInfo info, int ID, bool isExist)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendMarryRoomInfo(Game.Server.GameObjects.GamePlayer player, Game.Server.SceneMarryRooms.MarryRoom room)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendPlayerEnterMarryRoom(Game.Server.GameObjects.GamePlayer player)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendPlayerMarryStatus(Game.Server.GameObjects.GamePlayer player, int userID, bool isMarried)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendPlayerMarryApply(Game.Server.GameObjects.GamePlayer player, int userID, string userName, string loveProclamation, int ID)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendPlayerDivorceApply(Game.Server.GameObjects.GamePlayer player, bool result, bool isProposer)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendMarryApplyReply(Game.Server.GameObjects.GamePlayer player, int UserID, string UserName, bool result, bool isApplicant, int ID)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendBigSpeakerMsg(Game.Server.GameObjects.GamePlayer player, string msg)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendPlayerLeaveMarryRoom(Game.Server.GameObjects.GamePlayer player)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendMarryRoomLogin(Game.Server.GameObjects.GamePlayer player, bool result)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendMarryRoomInfoToPlayer(Game.Server.GameObjects.GamePlayer player, bool state, SqlDataProvider.Data.MarryRoomInfo info)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendMarryInfo(Game.Server.GameObjects.GamePlayer player, SqlDataProvider.Data.MarryInfo info)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendContinuation(Game.Server.GameObjects.GamePlayer player, SqlDataProvider.Data.MarryRoomInfo info)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendMarryProp(Game.Server.GameObjects.GamePlayer player, SqlDataProvider.Data.MarryProp info)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendRoomType(Game.Server.GameObjects.GamePlayer player, Game.Server.Rooms.BaseRoom game)
        {
            throw new NotImplementedException();
        }

        public GSPacketIn SendInsufficientMoney(Game.Server.GameObjects.GamePlayer player, int type)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
