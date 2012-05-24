using System.Collections.Generic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Server.Packets;
using Game.Server.Rooms;
using Game.Server.GameUtils;
using Game.Server.SceneMarryRooms;
using Game.Server.Quests;
using Game.Server.Buffer;

namespace Game.Base.Packets
{
    public interface IPacketLib
    {
        void SendTCP(GSPacketIn packet);
        void SendLoginSuccess();
        void SendLoginSuccess2();
        void SendCheckCode();
        void SendLoginFailed(string msg);
        void SendKitoff(string msg);
        void SendEditionError(string msg);


        void SendDateTime();
        //发送当前用户是否可以接收每日赠送
        GSPacketIn SendDailyAward(GamePlayer player);
        void SendPingTime(GamePlayer player);
        void SendUpdatePrivateInfo(PlayerInfo info);
        GSPacketIn SendUpdatePublicPlayer(PlayerInfo info);
        GSPacketIn SendNetWork(int id, long delay);
        GSPacketIn SendUserEquip(PlayerInfo info, List<ItemInfo> items);
        GSPacketIn SendMessage(eMessageType type, string message);

        //房间列表
        void SendWaitingRoom(bool result);
        GSPacketIn SendUpdateRoomList(BaseRoom room);
        GSPacketIn SendUpdateRoomList(List<BaseRoom> room);
        GSPacketIn SendSceneAddPlayer(GamePlayer player);
        GSPacketIn SendSceneRemovePlayer(GamePlayer player);

        //房间
        GSPacketIn SendRoomCreate(BaseRoom room);
        GSPacketIn SendRoomLoginResult(bool result);
        GSPacketIn SendRoomPlayerAdd(GamePlayer player);
        GSPacketIn SendRoomPlayerRemove(GamePlayer player);
        GSPacketIn SendRoomUpdatePlayerStates(byte[] states);
        GSPacketIn SendRoomUpdatePlacesStates(int[] states);
        GSPacketIn SendRoomPlayerChangedTeam(GamePlayer player);
        GSPacketIn SendRoomPairUpStart(BaseRoom room);
        GSPacketIn SendRoomPairUpCancel(BaseRoom room);
        GSPacketIn SendEquipChange(GamePlayer player, int place, int goodsID, string style);
        GSPacketIn SendRoomChange(BaseRoom room);
        //熔炼
        GSPacketIn SendFusionPreview(GamePlayer player, Dictionary<int, double> previewItemList, bool isBind, int MinValid);
        GSPacketIn SendFusionResult(GamePlayer player, bool result);
        //炼化
        GSPacketIn SendRefineryPreview(GamePlayer player, int templateid, bool isbind,ItemInfo item);

        //背包
        void SendUpdateInventorySlot(PlayerInventory bag, int[] updatedSlots);


        GSPacketIn SendFriendRemove(int FriendID);
        GSPacketIn SendFriendState(int playerID, bool state);

        //Buffer
        GSPacketIn SendUpdateBuffer(GamePlayer player, BufferInfo[] infos);
        GSPacketIn SendBufferList(GamePlayer player, List<AbstractBuffer> infos);

        //任务
        GSPacketIn SendUpdateQuests(GamePlayer player, byte[] states, BaseQuest[] quests);
        GSPacketIn SendMailResponse(int playerID, eMailRespose type);
        GSPacketIn SendAuctionRefresh(AuctionInfo info, int auctionID, bool isExist, ItemInfo item);
        GSPacketIn SendIDNumberCheck(bool result);

        //防沉迷
        GSPacketIn SendAASState(bool result);
        GSPacketIn SendAASInfoSet(bool result);
        GSPacketIn SendAASControl(bool result, bool IsAASInfo, bool IsMinor);

        GSPacketIn SendGameRoomInfo(GamePlayer player, BaseRoom game);

        GSPacketIn SendMarryInfoRefresh(MarryInfo info, int ID, bool isExist);

        GSPacketIn SendMarryRoomInfo(GamePlayer player, MarryRoom room);

        GSPacketIn SendPlayerEnterMarryRoom(Game.Server.GameObjects.GamePlayer player);

        GSPacketIn SendPlayerMarryStatus(GamePlayer player, int userID, bool isMarried);

        GSPacketIn SendPlayerMarryApply(GamePlayer player, int userID, string userName, string loveProclamation,int ID);

        GSPacketIn SendPlayerDivorceApply(GamePlayer player, bool result, bool isProposer);

        GSPacketIn SendMarryApplyReply(GamePlayer player, int UserID, string UserName, bool result, bool isApplicant,int ID);

        GSPacketIn SendBigSpeakerMsg(GamePlayer player, string msg);

        GSPacketIn SendPlayerLeaveMarryRoom(GamePlayer player);

        GSPacketIn SendMarryRoomLogin(GamePlayer player, bool result);

        GSPacketIn SendMarryRoomInfoToPlayer(GamePlayer player, bool state, MarryRoomInfo info);

        GSPacketIn SendMarryInfo(GamePlayer player, MarryInfo info);

        GSPacketIn SendContinuation(GamePlayer player, MarryRoomInfo info);

        GSPacketIn SendMarryProp(GamePlayer player, MarryProp info);

        GSPacketIn SendRoomType(GamePlayer player, BaseRoom game);
    }
}
