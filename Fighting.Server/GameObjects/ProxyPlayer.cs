using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Game.Logic.Phy.Object;
using Game.Logic.Protocol;

namespace Fighting.Server.GameObjects
{
    public class ProxyPlayer : IGamePlayer
    {
        private ServerClient m_client;

        private PlayerInfo m_character;

        private ItemTemplateInfo m_currentWeapon;

        private ItemInfo m_secondWeapon;

        private bool m_canUseProp;

        private int m_gamePlayerId;

        private double GPRate;

        private double OfferRate;

        public double Rate;

        public List<BufferInfo> Buffers;

        public int m_serverid;

        public int ServerID
        {
            get { return m_serverid; }
            set { m_serverid = value; }
        }


        public int GamePlayerId
        {
            get { return m_gamePlayerId; }
            set
            {
                m_gamePlayerId = value;
                m_client.SendGamePlayerId(this);
            }
        }

        public ProxyPlayer(ServerClient client, PlayerInfo character, ItemTemplateInfo currentWeapon, ItemInfo secondweapon, double baseAttack, double baseDefence, double baseAglilty, double baseBoold, double gprate, double offerrate, double rate, List<BufferInfo> infos, int serverid)
        {
            m_client = client;
            m_character = character;
            m_baseAttack = baseAttack;
            m_baseDefence = baseDefence;
            m_baseAglilty = baseAglilty;
            m_baseBlood = baseBoold;
            m_currentWeapon = currentWeapon;
            m_secondWeapon = secondweapon;
            m_equipEffect = new List<int>();
            GPRate = gprate;
            OfferRate = offerrate;
            Rate = rate;
            Buffers = infos;
            m_serverid = serverid;

        }

        #region  Room/Game Properties/Events

        public PlayerInfo PlayerCharacter
        {
            get { return m_character; }
        }

        public ItemTemplateInfo MainWeapon
        {
            get { return m_currentWeapon; }
        }

        public ItemInfo SecondWeapon
        {
            get { return m_secondWeapon; }
        }
        private double m_baseAglilty;

        public double GetBaseAgility()
        {
            return m_baseAglilty;
        }

        private double m_baseAttack;

        public double GetBaseAttack()
        {
            return m_baseAttack;
        }

        private double m_baseDefence;

        public double GetBaseDefence()
        {
            return m_baseDefence;
        }

        private double m_baseBlood;

        public double GetBaseBlood()
        {
            return m_baseBlood;
        }

        public bool CanUseProp
        {
            get { return m_canUseProp; }
            set { m_canUseProp = value; }
        }

        private List<int> m_equipEffect;
        public List<int> EquipEffect
        {
            get { return m_equipEffect; }
            set { m_equipEffect = value; }
        }


        #endregion

        public int AddGP(int gp)
        {
            if (gp > 0) m_client.SendPlayerAddGP(PlayerCharacter.ID, gp);
            return (int)(GPRate * gp);
        }
        public int RemoveGP(int gp)
        {
            m_client.SendPlayerRemoveGP(PlayerCharacter.ID, gp);
            return gp;
        }


        public int AddGold(int value)
        {
            if(value>0)m_client.SendPlayerAddGold(PlayerCharacter.ID, value);
            return value;
        }

        public int RemoveGold(int value)
        {
          m_client.SendPlayerRemoveGold(m_character.ID, value);
            return 0;

        }

        public int AddMoney(int value)
        {
            if(value>0)
            m_client.SendPlayerAddMoney(m_character.ID, value);
            return value;
        }

        public int RemoveMoney(int value)
        {
            m_client.SendPlayerRemoveMoney(m_character.ID, value);
            return 0;
        }

        public int AddGiftToken(int value)
        {
            if (value > 0) m_client.SendPlayerAddGiftToken(m_character.ID, value);
            return value;
        }

        public int RemoveGiftToken(int value)
        {
            return 0;
        }


        public int AddOffer(int baseoffer)
        {
            if (baseoffer < 0)
            {
                return baseoffer;
            }
            else
            {
                return (int)(baseoffer * OfferRate * Rate);
            }
        }

        public int RemoveOffer(int value)
        {
            m_client.SendPlayerRemoveOffer(m_character.ID, value);
            return value;
        }

        public void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int SpareMoney)
        {

        }

        public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving)
        {
            m_client.SendPlayerUsePropInGame(PlayerCharacter.ID, bag, place, templateId, isLiving);
            //等待服务器处理
            game.Pause(500);

            return false;
        }

        public void OnGameOver(AbstractGame game, bool isWin, int gainXp)
        {
            Console.WriteLine("游戏结束:玩家编号【{0}】 房间编号【{1}】 是否胜利【{2}】  伤害【{3}】", PlayerCharacter.ID, game.Id, isWin, gainXp);
            m_client.SendPlayerOnGameOver(PlayerCharacter.ID, game.Id, isWin, gainXp);
        }

        public void Disconnect()
        {
            m_client.SendDisconnectPlayer(m_character.ID);
        }

        public void SendTCP(GSPacketIn pkg)
        {
            m_client.SendPacketToPlayer(m_character.ID, pkg);
        }

        public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage)
        {
            Console.WriteLine("游戏结束:玩家编号【{0}】 目标类型【{1}】 目标编号【{2}】  是否活着【{3}】 伤害【{4}】", m_character.ID, type, id, isLiving, demage);
            m_client.SendPlayerOnKillingLiving(m_character.ID, game, type, id, isLiving, demage);

        }

        public void OnMissionOver(AbstractGame game, bool isWin, int MissionID, int turnNum)
        {
            m_client.SendPlayerOnMissionOver(m_character.ID, game, isWin, MissionID, turnNum);

        }

        public int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
        {
            m_client.SendPlayerConsortiaFight(m_character.ID, consortiaWin, consortiaLose, players, roomType, gameClass, totalKillHealth);

            return 0;
        }

        public void SendConsortiaFight(int consortiaID, int riches, string msg)
        {
            m_client.SendPlayerSendConsortiaFight(m_character.ID, consortiaID, riches, msg);

        }



        public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count)
        {
            m_client.SendPlayerAddTemplate(m_character.ID, cloneItem, bagType, count);
            return true;
        }




        public void SendMessage(string msg)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CHAT);
            pkg.WriteInt(3);
            pkg.WriteString(msg);

            SendTCP(pkg);
        }

        public bool IsPvePermission(int missionId, eHardLevel hardLevel)
        {
            return true;
        }

        public bool SetPvePermission(int missionId, eHardLevel hardLevel)
        {
            return true;
        }

        public void SendInsufficientMoney(int type)
        {
            //TODO 实现SendInsufficientMoney logic
        }

        public bool ClearTempBag()
        {
            //TODO: 实现战斗服务器的清除临时背包
            return true;
        }

        public bool ClearFightBag()
        {
            //TODO: 清除战斗背包
            return true;
        }
    }
}
