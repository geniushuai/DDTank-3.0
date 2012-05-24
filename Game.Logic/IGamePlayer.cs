using SqlDataProvider.Data;
using Game.Base.Packets;
using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic
{
    public interface IGamePlayer
    {
        PlayerInfo PlayerCharacter { get; }
        ItemTemplateInfo MainWeapon { get; }
        ItemInfo SecondWeapon { get; }

        bool CanUseProp { get; set; }
        int GamePlayerId { get; set; }
        int ServerID { get; set; }
        List<int> EquipEffect { get; set; }

        double GetBaseBlood();

        double GetBaseAttack();
        double GetBaseDefence();

        int AddGP(int gp);
        int RemoveGP(int gp);

        int AddGold(int value);
        int RemoveGold(int value);

        int AddMoney(int value);
        int RemoveMoney(int value);

        int AddGiftToken(int value);
        int RemoveGiftToken(int value);

        int AddOffer(int value);
        int RemoveOffer(int value);
        bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count);
        bool ClearTempBag();
        bool ClearFightBag();
       


        bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving);
        void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage);
        void OnGameOver(AbstractGame game, bool isWin, int gainXp);
        void OnMissionOver(AbstractGame game, bool isWin, int MissionID, int TurnNum);
        int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count);
        void SendConsortiaFight(int consortiaID, int riches, string msg);

        bool SetPvePermission(int missionId, eHardLevel hardLevel);
        bool IsPvePermission(int missionId, eHardLevel hardLevel);

        void Disconnect();

        void SendInsufficientMoney(int type);

        void SendMessage(string msg);
        void SendTCP(GSPacketIn pkg);

        void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int SpareMoney);
    }
}
