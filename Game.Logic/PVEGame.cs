using System;
using System.Collections.Generic;
using System.Linq;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using LuaInterface;
using Game.Logic.Actions;
using Game.Base.Packets;
using log4net;
using System.Reflection;
using System.Drawing;
using SqlDataProvider.Data;
using Bussiness.Managers;
using System.Text;
using Bussiness;
using Game.Logic.AI;
using Game.Server.Managers;
using Game.Logic.AI.Game;
using Game.Logic.AI.Mission;

namespace Game.Logic
{
    public class PVEGame : BaseGame
    {
        private static readonly new ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private APVEGameControl m_gameAI = null;
        private AMissionControl m_missionAI = null;

        public int SessionId;

        public bool IsWin;

        public int TotalMissionCount;
        public int TotalCount;
        public int TotalTurn;
        public int Param1;
        public int Param2;
        public int Param3;
        public int Param4;

        public int TotalKillCount;

        public double TotalNpcExperience;

        public double TotalNpcGrade;

        private int BeginPlayersCount;

        private PveInfo m_info;

        private List<string> m_gameOverResources;

        public Dictionary<int, MissionInfo> Misssions;

        private MapPoint mapPos;

        public int WantTryAgain;

        private eHardLevel m_hardLevel;

        private DateTime beginTime;

        private string m_IsBossType;

        public PVEGame(int id, int roomId, PveInfo info, List<IGamePlayer> players, Map map, eRoomType roomType, eGameType gameType, int timeType, eHardLevel hardLevel)
            : base(id, roomId, map, roomType, gameType, timeType)
        {
            foreach (IGamePlayer player in players)
            {
                Player fp = new Player(player, PhysicalId++, this, 1);
                //fp.Reset();
                fp.Direction = m_random.Next(0, 1) == 0 ? 1 : -1;
                AddPlayer(player, fp);
            }

            m_info = info;
            BeginPlayersCount = players.Count;
            TotalKillCount = 0;
            TotalNpcGrade = 0;
            TotalNpcExperience = 0;
            TotalHurt = 0;

            m_IsBossType = "";

            WantTryAgain = 0;
            SessionId = 0;
            m_gameOverResources = new List<string>();
            Misssions = new Dictionary<int, MissionInfo>();
            m_mapHistoryIds = new List<int>();
            m_hardLevel = hardLevel;

            string script = GetScript(info, hardLevel);

            m_gameAI = ScriptMgr.CreateInstance(script) as APVEGameControl;
            if (m_gameAI == null)
            {
                log.ErrorFormat("Can't create game ai :{0}", script);
                m_gameAI = SimplePVEGameControl.Simple;
            }
            m_gameAI.Game = this;
            m_gameAI.OnCreated();

            m_missionAI = SimpleMissionControl.Simple;
            beginTime = DateTime.Now;
            m_bossCardCount = 0;
        }

        #region MissionSetting
        private string GetScript(PveInfo pveInfo, eHardLevel hardLevel)
        {
            string script = string.Empty;

            switch (hardLevel)
            {
                case eHardLevel.Simple:
                    script = pveInfo.SimpleGameScript;
                    break;
                case eHardLevel.Normal:
                    script = pveInfo.NormalGameScript;
                    break;
                case eHardLevel.Hard:
                    script = pveInfo.HardGameScript;
                    break;
                case eHardLevel.Terror:
                    script = pveInfo.TerrorGameScript;
                    break;
                default:
                    script = pveInfo.SimpleGameScript;
                    break;
            }
            return script;
        }

        public string GetMissionIdStr(string missionIds, int randomCount)
        {
            if (string.IsNullOrEmpty(missionIds))
            {
                return "";
            }
            string[] ids = missionIds.Split(',');
            if (ids.Length < randomCount)
            {
                return "";
            }
            List<string> idList = new List<string>();
            int seed = ids.Length;
            int i = 0;
            while (i < randomCount)
            {
                int rand = Random.Next(seed);
                string id = ids[rand];
                if (!idList.Contains(id))
                {
                    idList.Add(id);
                    i++;
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (string s in idList)
            {
                sb.Append(s).Append(",");
            }
            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        public void SetupMissions(string missionIds)
        {
            if (string.IsNullOrEmpty(missionIds))
            {
                return;
            }
            else
            {
                int i = 0;
                string[] ids = missionIds.Split(',');
                foreach (string id in ids)
                {
                    i++;
                    MissionInfo mi = MissionInfoMgr.GetMissionInfo(int.Parse(id));
                    Misssions.Add(i, mi);
                }
            }
        }

        private MissionInfo m_missionInfo;
        public MissionInfo MissionInfo
        {
            get { return m_missionInfo; }
            set { m_missionInfo = value; }
        }
        public Player CurrentPlayer
        {
            get { return m_currentLiving as Player; }
        }

        public TurnedLiving CurrentTurnLiving
        {
            get { return m_currentLiving; }
        }

        #endregion

        #region CreateNpc/CreateBoss/CreateBox/CreatePhysicalObj/ClearSimpNpc

        public SimpleNpc CreateNpc(int npcId, int x, int y, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleNpc npc = new SimpleNpc(PhysicalId++, this, npcInfo, type);
            npc.Reset();
            npc.SetXY(x, y);

            AddLiving(npc);

            npc.StartMoving();

            return npc;
        }

        public SimpleNpc CreateNpc(int npcId, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleNpc npc = new SimpleNpc(PhysicalId++, this, npcInfo, type);
            Point pos = GetPlayerPoint(mapPos, npcInfo.Camp);
            npc.Reset();

            npc.SetXY(pos);

            AddLiving(npc);

            npc.StartMoving();

            return npc;
        }

        public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type)
        {
            NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
            SimpleBoss boss = new SimpleBoss(PhysicalId++, this, npcInfo, direction, type);
            boss.Reset();
            boss.SetXY(x, y);

            AddLiving(boss);

            boss.StartMoving();

            return boss;
        }

        public Box CreateBox(int x, int y, string model, ItemInfo item)
        {
            Box box = new Box(PhysicalId++, model, item);
            box.SetXY(x, y);

            m_map.AddPhysical(box);
            AddBox(box, true);

            return box;
        }

        public PhysicalObj CreatePhysicalObj(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
        {
            PhysicalObj obj = new PhysicalObj(PhysicalId++, name, model, defaultAction, scale, rotation);
            obj.SetXY(x, y);
            AddPhysicalObj(obj, true);
            return obj;
        }

        public Layer Createlayer(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
        {
            Layer obj = new Layer(PhysicalId++, name, model, defaultAction, scale, rotation);
            obj.SetXY(x, y);
            AddPhysicalObj(obj, true);
            return obj;
        }


        public Layer CreateTip(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
        {
            Layer obj = new Layer(PhysicalId++, name, model, defaultAction, scale, rotation);
            obj.SetXY(x, y);
            AddPhysicalTip(obj, true);
            return obj;
        }
        public void ClearMissionData()
        {
            foreach (Living living in m_livings)
            {
                living.Dispose();
            }
            m_livings.Clear();

            List<TurnedLiving> temp = new List<TurnedLiving>();
            foreach (TurnedLiving tl in TurnQueue)
            {
                if (tl is Player)
                {
                    if (tl.IsLiving)
                    {
                        temp.Add(tl);
                    }
                }
                else
                {
                    tl.Dispose();
                }
            }
            TurnQueue.Clear();
            foreach (TurnedLiving tl in temp)
            {
                TurnQueue.Add(tl);
            }
            if (m_map != null)
            {
                foreach (PhysicalObj obj in m_map.GetAllPhysicalObjSafe())
                {
                    obj.Dispose();
                }
            }
        }

        public void AddAllPlayerToTurn()
        {
            foreach (Player player in Players.Values)
            {
                TurnQueue.Add(player);
            }
        }
        #endregion

        #region Override AddLiving/RemovePlayer

        public override void AddLiving(Living living)
        {
            base.AddLiving(living);
            living.Died += new LivingEventHandle(living_Died);
        }

        private void living_Died(Living living)
        {
            if (CurrentLiving != null && CurrentLiving is Player)
            {
                if (!(living is Player) && living != CurrentLiving)
                {
                    TotalKillCount++;
                    TotalNpcExperience += living.Experience;
                    TotalNpcGrade += living.Grade;
                }
            }
        }

        public override bool CanAddPlayer()
        {
            lock (m_players)
            {
                return GameState == eGameState.SessionPrepared && m_players.Count < 4;
            }
        }

        public override Player AddPlayer(IGamePlayer gp)
        {
            if (CanAddPlayer())
            {
                Player fp = new Player(gp, PhysicalId++, this, 1);
                //fp.Reset();
                fp.Direction = m_random.Next(0, 1) == 0 ? 1 : -1;
                AddPlayer(gp, fp);
                SendCreateGameToSingle(this, gp);
                SendPlayerInfoInGame(this, gp, fp);
                return fp;
            }
            else
            {
                return null;
            }
        }

        public override Player RemovePlayer(IGamePlayer gp, bool isKick)
        {
            Player player = GetPlayer(gp);

            if (player != null)
            {
                player.PlayerDetail.RemoveGP(gp.PlayerCharacter.Grade * 12);
                string msg = null;
                string msg1 = null;
                if (player.IsLiving && GameState == eGameState.Playing)
                {

                    msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg4", gp.PlayerCharacter.Grade * 12);
                    msg1 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg5", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12);
                    SendMessage(gp, msg, msg1, 3);
                }
                else
                {
                    msg1 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg1", gp.PlayerCharacter.NickName);
                    SendMessage(gp, msg, msg1, 3);
                }
                base.RemovePlayer(gp, isKick);
            }


            return player;
        }


        #endregion

        #region Prepare/PrepareNewSession/StartLoading/StartGame/NextTurn/CanGameOver/GameOver/GameOverAllSession/Stop

        /// <summary>
        /// 加载客户端资源
        /// </summary>
        /// <param name="arrays">需要加载npc数据列表</param>
        public void LoadResources(int[] npcIds)
        {
            if (npcIds == null || npcIds.Length == 0)
                return;
            foreach (int npcId in npcIds)
            {
                NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
                if (npcInfo == null)
                {
                    log.Error("LoadResources npcInfo resoure is not exits");
                    continue;
                }
                AddLoadingFile(2, npcInfo.ResourcesPath, npcInfo.ModelID);
            }
        }

        public void LoadNpcGameOverResources(int[] npcIds)
        {
            if (npcIds == null || npcIds.Length == 0)
                return;
            foreach (int npcId in npcIds)
            {
                NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
                if (npcInfo == null)
                {
                    log.Error("LoadGameOverResources npcInfo resoure is not exits");
                    continue;
                }
                m_gameOverResources.Add(npcInfo.ModelID);
            }
        }

        public void Prepare()
        {
            if (GameState == eGameState.Inited)
            {
                m_gameState = eGameState.Prepared;

                SendCreateGame();
                CheckState(0);
                try
                {
                    m_gameAI.OnPrepated();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("game ai script {0} error:{1}", GameState, ex);
                }
            }
        }

        public void PrepareNewSession()
        {
            if (GameState == eGameState.Prepared || GameState == eGameState.GameOver || GameState == eGameState.ALLSessionStopped)
            {
                m_gameState = eGameState.SessionPrepared;
                SessionId++;
                ClearLoadingFiles();
                ClearMissionData();
                m_gameOverResources.Clear();
                WantTryAgain = 0;

                m_missionInfo = Misssions[SessionId];
                m_pveGameDelay = m_missionInfo.Delay;
                TotalCount = m_missionInfo.TotalCount;
                TotalTurn = m_missionInfo.TotalTurn;
                Param1 = m_missionInfo.Param1;
                Param2 = m_missionInfo.Param2;
                Param3 = -1;
                Param4 = -1;

                m_missionAI = ScriptMgr.CreateInstance(m_missionInfo.Script) as AMissionControl;

                if (m_missionAI == null)
                {
                    log.ErrorFormat("Can't create game mission ai :{0}", m_missionInfo.Script);
                    m_missionAI = SimpleMissionControl.Simple;
                }
                IsBossWar = "";
                m_missionAI.Game = this;

                try
                {
                    m_missionAI.OnPrepareNewSession();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("game ai script {0} error:{1}", GameState, ex);
                }
            }
        }

        public bool CanStartNewSession()
        {
            return m_turnIndex == 0 || IsAllReady();
        }

        public bool IsAllReady()
        {
            foreach (Player p in Players.Values)
            {
                if (p.Ready == false)
                {
                    return false;
                }
            }
            return true;
        }

        public void StartLoading()
        {
            if (GameState == eGameState.SessionPrepared)
            {
                m_gameState = eGameState.Loading;
                m_turnIndex = 0;
                SendMissionInfo();
                SendStartLoading(60);
                AddAction(new WaitPlayerLoadingAction(this, 61 * 1000));
            }
        }
        public void StartGameMovie()
        {
            if (GameState == eGameState.Loading)
            {
                try
                {
                    m_missionAI.OnStartMovie();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("game ai script {0} error:{1}", GameState, ex);
                }
            }
        }
        public void StartGame()
        {
            if (GameState == eGameState.Loading)
            {
                m_gameState = eGameState.GameStart;

                //同步时间
                SendSyncLifeTime();

                TotalKillCount = 0;
                TotalNpcGrade = 0;
                TotalNpcExperience = 0;
                TotalHurt = 0;

                m_bossCardCount = 0;
                BossCards = null;

                List<Player> list = GetAllFightPlayers();
                mapPos = MapMgr.GetPVEMapRandomPos(m_map.Info.ID);
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
                pkg.WriteByte((byte)eTankCmdType.START_GAME);
                pkg.WriteInt(list.Count);
                foreach (Player p in list)
                {
                    if (!p.IsLiving)
                    {
                        AddLiving(p);
                    }
                    p.Reset();

                    Point pos = GetPlayerPoint(mapPos, p.Team);
                    p.SetXY(pos);
                    m_map.AddPhysical(p);
                    p.StartMoving();
                    p.StartGame();

                    pkg.WriteInt(p.Id);
                    pkg.WriteInt(p.X);
                    pkg.WriteInt(p.Y);

                    if (pos.X < 600)
                    {
                        p.Direction = 1;

                    }
                    else
                    {
                        p.Direction = -1;
                    }
                    pkg.WriteInt(p.Direction);

         
                    pkg.WriteInt(p.Blood);
                    //Weapon refineLevel
                    pkg.WriteInt(2);
                    //ratio
                    pkg.WriteInt(34);
                    //dander
                    pkg.WriteInt(p.Dander);
                    //pkg.WriteInt(0);
                    pkg.WriteInt(p.PlayerDetail.EquipEffect.Count);
                    foreach (var templateID in p.PlayerDetail.EquipEffect)
                    {
                        ItemTemplateInfo item = ItemMgr.FindItemTemplate(templateID);
                        if (item.Property3 < 27)
                        {
                            pkg.WriteInt(item.Property3);
                            pkg.WriteInt(item.Property4);
                        }
                        else
                        {
                            pkg.WriteInt(0);
                            pkg.WriteInt(0);

                        }
                    }
                   // pkg.WriteInt(0);
                
                }

                SendToAll(pkg);
                SendUpdateUiData();
                WaitTime(PlayerCount * 2500 + 1000);
                OnGameStarted();
            }
        }

        public void PrepareNewGame()
        {
            if (GameState == eGameState.GameStart)
            {
                m_gameState = eGameState.Playing;
                WaitTime(PlayerCount * 1000);
                try
                {
                    m_missionAI.OnStartGame();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("game ai script {0} error:{1}", GameState, ex);
                }
            }
        }

        public void NextTurn()
        {
            if (GameState == eGameState.Playing)
            {
                ClearWaitTimer();
                ClearDiedPhysicals();
                CheckBox();

                LivingRandSay();

                List<Physics> list = m_map.GetAllPhysicalSafe();

                foreach (Physics p in list)
                {
                    p.PrepareNewTurn();
                }

                List<Box> newBoxes = CreateBox();
                m_currentLiving = FindNextTurnedLiving();

                try
                {
                    m_missionAI.OnNewTurnStarted();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("game ai script {0} error:{1}", GameState, ex);
                }

                if (m_currentLiving != null)
                {
                    m_turnIndex++;

                    SendUpdateUiData();

                    List<Living> livings = GetLivedLivings();

                    if (livings.Count > 0 && m_currentLiving.Delay >= m_pveGameDelay)
                    {
                        m_currentLiving = null;
                        MinusDelays(m_pveGameDelay);
                        foreach (Living living in m_livings)
                        {
                            living.PrepareSelfTurn();
                            if (living.IsFrost == false)
                            {
                                SendGameNextTurn(livings[0], this, newBoxes);
                                living.StartAttacking();
                            }
                        }

                        foreach (Living living in m_livings)
                        {
                            if (living.IsAttacking)
                            {
                                living.StopAttacking();
                            }
                        }
                        m_pveGameDelay += MissionInfo.IncrementDelay;
                        CheckState(0);
                    }
                    else
                    {
                        MinusDelays(m_currentLiving.Delay);

                        UpdateWind(GetNextWind(), false);

                        CurrentTurnTotalDamage = 0;

                        m_currentLiving.PrepareSelfTurn();

                        if (m_currentLiving.IsFrost == false)
                        {
                            m_currentLiving.StartAttacking();

                            SendGameNextTurn(m_currentLiving, this, newBoxes);

                            if (m_currentLiving.IsAttacking)
                            {
                                AddAction(new WaitLivingAttackingAction(m_currentLiving, m_turnIndex, (m_timeType + 20) * 1000));
                            }
                        }
                    }
                }

                OnBeginNewTurn();
                try
                {
                    m_missionAI.OnBeginNewTurn();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("game ai script {0} error:{1}", GameState, ex);
                }
            }
        }


        public void LivingRandSay()
        {
            if (m_livings == null || m_livings.Count == 0)
                return;
            int sayCount = 0;
            int livCount = m_livings.Count;
            foreach (Living living in m_livings)
            {
                living.IsSay = false;
            }
            if (TurnIndex % 2 == 0)
            {
                return;
            }
            if (livCount <= 5)
            {
                sayCount = Random.Next(0, 2);
            }
            else if (livCount > 5 && livCount <= 10)
            {
                sayCount = Random.Next(1, 3);
            }
            else
            {
                sayCount = Random.Next(1, 4);
            }

            if (sayCount > 0)
            {
                int[] sayIndexs = new int[sayCount];
                for (int i = 0; i < sayCount; )
                {
                    int index = Random.Next(0, livCount);
                    if (m_livings[index].IsSay == false)
                    {
                        m_livings[index].IsSay = true;
                        i++;
                    }
                }
            }
        }


        public override bool TakeCard(Player player)
        {
            int index = 0;

            for (int i = 0; i < Cards.Length; i++)
            {
                if (Cards[i] == 0)
                {
                    index = i;
                    break;
                }
            }

            return TakeCard(player, index);
        }

        public override bool TakeCard(Player player, int index)
        {
            if (player.CanTakeOut == 0)

                return false;

            if (player.IsActive == false || index < 0 || index > Cards.Length || player.FinishTakeCard || Cards[index] > 0)
                return false;

            int gold = 0;
            int money = 0;
            int giftToken = 0;
            int templateID = 0;
            List<ItemInfo> infos = null;
            if (DropInventory.CopyDrop(m_missionInfo.Id, 1, ref  infos))
            {
                if (infos != null)
                {
                    foreach (ItemInfo info in infos)
                    {
                        ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken);
                        if (info != null)
                        {
                            templateID = info.TemplateID;
                            player.PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count);
                        }
                    }
                    player.PlayerDetail.AddGold(gold);
                    player.PlayerDetail.AddMoney(money);
                    player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_TakeCard, player.PlayerDetail.PlayerCharacter.ID, money, player.PlayerDetail.PlayerCharacter.Money);
                    player.PlayerDetail.AddGiftToken(giftToken);
                }
            }

            if (RoomType == eRoomType.Treasure || RoomType == eRoomType.Boss)
            {
                player.CanTakeOut--;
                if (player.CanTakeOut == 0)
                {
                    player.FinishTakeCard = true;
                }
            }
            else
            {
                player.FinishTakeCard = true;
            }
            Cards[index] = 1;

            SendGamePlayerTakeCard(player, index, templateID, gold, money, giftToken);
            return true;
        }

        public bool CanGameOver()
        {
            if (PlayerCount == 0)
                return true;
            if (GetDiedPlayerCount() == PlayerCount)
            {
                IsWin = false;
                return true;
            }

            try
            {
                return m_missionAI.CanGameOver();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("game ai script {0} error:{1}", GameState, ex);
            }
            return true;
        }
        //public static void LogFightAdd(int roomId, eRoomType roomType, eGameType fightType, int changeTeam, DateTime playBegin, DateTime playEnd, int userCount, int mapId, string teamA,string teamB, string playResult,int winTeam)
        //TODO
        /*
        public void GameOverMovie()
        {
            if(GameState == eGameState.GameOverMovie)
        }
        */
        public void GameOver()
        {
            if (GameState == eGameState.Playing)
            {
                m_gameState = eGameState.GameOver;
                SendUpdateUiData();
                try
                {
                    m_missionAI.OnGameOver();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("game ai script {0} error:{1}", GameState, ex);
                }

                List<Player> players = GetAllFightPlayers();
                BossCardCount = 5;
                CurrentTurnTotalDamage = 0;
                bool canEnterFinall = true;
                bool hasNextSess = HasNextSession();
                if (IsWin == false || hasNextSess == false)
                {
                    m_bossCardCount = 0;
                }
                var listPacket = new List<GSPacketIn>();

                GSPacketIn pkg = new GSPacketIn((short)ePackageType.GAME_CMD);
                pkg.WriteByte((byte)eTankCmdType.GAME_MISSION_OVER);
                //TrieuLSL
                if (hasNextSess == false)
                {
                    pkg.WriteInt(2);
                }
                else
                {
                    pkg.WriteInt(1);
                }
                pkg.WriteBoolean(hasNextSess);
                //misioninfoPic
                pkg.WriteString("samll_map.jpg");
                pkg.WriteBoolean(canEnterFinall);

                //pkg.WriteInt(0);
                pkg.WriteInt(PlayerCount);
                foreach (Player p in players)
                {
                    int experience = CalculateExperience(p);
                    int score = CalculateScore(p);
                    int rate = m_missionAI.CalculateScoreGrade(p.TotalAllScore);
                    p.CanTakeOut = 1;
                    if (p.CurrentIsHitTarget == true)
                    {
                        p.TotalHitTargetCount += 1;
                    }

                    int hitRate = CalculateHitRate(p.TotalHitTargetCount, p.TotalShootCount);
                    p.TotalAllHurt += p.TotalHurt;
                    p.TotalAllCure += p.TotalCure;
                    p.TotalAllHitTargetCount += p.TotalHitTargetCount;
                    p.TotalAllShootCount += p.TotalShootCount;
                    p.GainGP = p.PlayerDetail.AddGP(experience);
                    p.TotalAllExperience += p.GainGP;
                    p.TotalAllScore += score;
                    p.BossCardCount = BossCardCount;

                    pkg.WriteInt(p.PlayerDetail.PlayerCharacter.ID);

                    pkg.WriteInt(p.PlayerDetail.PlayerCharacter.Grade);
                    pkg.WriteInt(500);
                    pkg.WriteInt(p.GainGP);
                    ////VIpGP
                    //pkg.WriteInt(500);
                    //pkg.WriteInt(p.TotalHurt);
                    //pkg.WriteInt(p.TotalKill);
                    //pkg.WriteInt(p.TotalHitTargetCount);
                    //pkg.WriteInt(score);

                    //pkg.WriteInt(p.TotalAllExperience);
                    pkg.WriteBoolean(IsWin);
                    pkg.WriteInt(p.BossCardCount + 1);
                }
    
                pkg.WriteInt(m_gameOverResources.Count);
                foreach (string res in m_gameOverResources)
                {
                    pkg.WriteString(res);
                }
  
                SendToAll(pkg);

                StringBuilder sb = new StringBuilder();
                foreach (Player p in players)
                {
                    sb.Append(p.PlayerDetail.PlayerCharacter.ID).Append(",");
                    p.Ready = false;
                    p.PlayerDetail.OnMissionOver(p.Game, IsWin, MissionInfo.Id, p.TurnNum);
                }
                int winTeam = IsWin ? 1 : 2;
                string teamAStr = sb.ToString();
                string teamBStr = "";
                string dropTemplateIdsStr = "";

                if (!IsWin && SessionId < 2)
                {
                    OnGameStopped();
                }

                StringBuilder BossWarRecord = new StringBuilder();

                if (IsWin && IsBossWar != "")
                {
                    BossWarRecord.Append(IsBossWar).Append(",");

                    foreach (Player p in players)
                    {
                        BossWarRecord.Append("玩家ID:").Append(p.PlayerDetail.PlayerCharacter.ID).Append(",");
                        BossWarRecord.Append("等级:").Append(p.PlayerDetail.PlayerCharacter.Grade).Append(",");
                        BossWarRecord.Append("攻击回合数:").Append(p.TurnNum).Append(",");
                        BossWarRecord.Append("攻击:").Append(p.PlayerDetail.PlayerCharacter.Attack).Append(",");
                        BossWarRecord.Append("防御:").Append(p.PlayerDetail.PlayerCharacter.Defence).Append(",");
                        BossWarRecord.Append("敏捷:").Append(p.PlayerDetail.PlayerCharacter.Agility).Append(",");
                        BossWarRecord.Append("幸运:").Append(p.PlayerDetail.PlayerCharacter.Luck).Append(",");
                        BossWarRecord.Append("伤害:").Append(p.PlayerDetail.GetBaseAttack()).Append(",");
                        BossWarRecord.Append("总血量:").Append(p.MaxBlood).Append(",");
                        BossWarRecord.Append("护甲:").Append(p.PlayerDetail.GetBaseDefence()).Append(",");
                        if (p.PlayerDetail.SecondWeapon != null)
                        {
                            BossWarRecord.Append("副武器:").Append(p.PlayerDetail.SecondWeapon.TemplateID).Append(",");
                            BossWarRecord.Append("副武器强化等级:").Append(p.PlayerDetail.SecondWeapon.StrengthenLevel).Append(".");
                        }
                    }
                }

                BossWarField = BossWarRecord.ToString();

                OnGameOverLog(RoomId, RoomType, GameType, 0, beginTime, DateTime.Now, BeginPlayersCount, MissionInfo.Id, teamAStr, teamBStr, dropTemplateIdsStr, winTeam, BossWarField);
                OnGameOverred();
            }
        }

        public bool HasNextSession()
        {
            if (PlayerCount == 0 || IsWin == false)
                return false;
            int nextSessionId = SessionId + 1;
            if (Misssions.ContainsKey(nextSessionId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void GameOverAllSession()
        {
            // SendCreateGame();
            //SendMissionInfo();
            if (GameState == eGameState.GameOver)
            {
                //Console.WriteLine("GameOverAllSession SessionId : " + SessionId);
                m_gameState = eGameState.ALLSessionStopped;

                try
                {
                    m_gameAI.OnGameOverAllSession();
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("game ai script {0} error:{1}", GameState, ex);
                }

                List<Player> players = GetAllFightPlayers();

                GSPacketIn pkg = new GSPacketIn((short)ePackageType.GAME_CMD);
                pkg.WriteByte((byte)eTankCmdType.GAME_ALL_MISSION_OVER);

                int canTakeCards = 1;
                if (IsWin == false)
                {
                    canTakeCards = 0;
                }
                else
                {
                    if (m_roomType == eRoomType.Treasure || m_roomType == eRoomType.Boss)
                    {
                        canTakeCards = 2;
                    }
                }


                //}

                pkg.WriteInt(PlayerCount);
                foreach (Player p in players)
                {

                    p.CanTakeOut = canTakeCards;
                    p.PlayerDetail.OnGameOver(this, IsWin, p.GainGP);
                    int hitRate = CalculateHitRate(p.TotalAllHitTargetCount, p.TotalAllShootCount);
                    int rate = m_gameAI.CalculateScoreGrade(p.TotalAllScore);

                    pkg.WriteInt(p.PlayerDetail.PlayerCharacter.ID);

                    //_loc_7.killGP = _loc_2.readInt();
                    pkg.WriteInt(p.TotalAllKill);
                    //_loc_7.hertGP = _loc_2.readInt();
                    pkg.WriteInt(p.TotalAllHurt);
                    //_loc_7.fightGP = _loc_2.readInt();
                    pkg.WriteInt(p.TotalAllScore);
                    //_loc_7.ghostGP = _loc_2.readInt();
                    pkg.WriteInt(p.TotalAllCure);

                    //_loc_7.gpForVIP = _loc_2.readInt();
                    pkg.WriteInt(0);
                    //_loc_7.gpForSpouse = _loc_2.readInt();
                    pkg.WriteInt(0);
                    //_loc_7.gpForServer = _loc_2.readInt();
                    pkg.WriteInt(0);
                    //_loc_7.gpForApprenticeOnline = _loc_2.readInt();
                    pkg.WriteInt(0);
                    //_loc_7.gpForApprenticeTeam = _loc_2.readInt();
                    pkg.WriteInt(0);
                    //_loc_7.gpForDoubleCard = _loc_2.readInt();
                    pkg.WriteInt(0);
                    //_loc_7.gainGP = _loc_2.readInt()
                    pkg.WriteInt(p.TotalAllExperience);
                    pkg.WriteBoolean(IsWin);

                    //是否可以翻牌,每人可以翻几张牌
                    //pkg.WriteInt(p.CanTakeOut);

                    //pkg.WriteInt(0);
                }
                //加载客户端胜利页面显示的怪物图片
                pkg.WriteInt(m_gameOverResources.Count);
                foreach (string res in m_gameOverResources)
                {
                    pkg.WriteString(res);
                }
                //p.PlayerDetail.SendTCP(pkg);


                SendToAll(pkg);
                WaitTime(21 * 1000);
                CanStopGame();
            }
        }
        #region backup GamOver
        //public void GameOverAllSession()
        //{
        //    // SendCreateGame();
        //    //SendMissionInfo();
        //    if (GameState == eGameState.GameOver)
        //    {
        //        //Console.WriteLine("GameOverAllSession SessionId : " + SessionId);
        //        m_gameState = eGameState.ALLSessionStopped;

        //        try
        //        {
        //            m_gameAI.OnGameOverAllSession();
        //        }
        //        catch (Exception ex)
        //        {
        //            log.ErrorFormat("game ai script {0} error:{1}", GameState, ex);
        //        }

        //        List<Player> players = GetAllFightPlayers();

        //        GSPacketIn pkg = new GSPacketIn((short)ePackageType.GAME_CMD);
        //        pkg.WriteByte((byte)eTankCmdType.GAME_ALL_MISSION_OVER);

        //        int canTakeCards = 1;
        //        if (IsWin == false)
        //        {
        //            canTakeCards = 0;
        //        }
        //        else
        //        {
        //            if (m_roomType == eRoomType.Treasure || m_roomType == eRoomType.Boss)
        //            {
        //                canTakeCards = 2;
        //            }
        //        }

        //        foreach (Player p in players)
        //        {
        //            p.CanTakeOut = canTakeCards;
        //            p.PlayerDetail.OnGameOver(this, IsWin, p.GainGP);
        //        }

        //        pkg.WriteInt(PlayerCount);
        //        foreach (Player p in players)
        //        {
        //            int hitRate = CalculateHitRate(p.TotalAllHitTargetCount, p.TotalAllShootCount);
        //            int rate = m_gameAI.CalculateScoreGrade(p.TotalAllScore);
        //            pkg.WriteInt(p.PlayerDetail.PlayerCharacter.ID);

        //            pkg.WriteInt(p.PlayerDetail.PlayerCharacter.Grade);
        //            pkg.WriteInt(p.TotalAllExperience);
        //            pkg.WriteInt(p.TotalAllHurt);
        //            pkg.WriteInt(p.TotalAllCure);
        //            pkg.WriteInt(hitRate);
        //            pkg.WriteInt(rate);

        //            pkg.WriteBoolean(IsWin);

        //            //是否可以翻牌,每人可以翻几张牌
        //            //pkg.WriteInt(p.CanTakeOut);
        //        }
        //        //pkg.WriteInt(0);

        //        //加载客户端胜利页面显示的怪物图片
        //        pkg.WriteInt(m_gameOverResources.Count);
        //        foreach (string res in m_gameOverResources)
        //        {
        //            pkg.WriteString(res);
        //        }

        //        SendToAll(pkg);
        //        WaitTime(21 * 1000);
        //        CanStopGame();
        //    }
        //}
        #endregion
        public void CanStopGame()
        {
            if (IsWin == false)
            {
                //夺宝,大于2关的boss战,当当前关卡大于1时,可以付费再试一次
                if ((GameType == eGameType.Treasure || GameType != eGameType.Boss) && SessionId > 1)
                {
                    WantTryAgain = 2;
                    ClearWaitTimer();
                }
            }
        }

        public override void Stop()
        {
            if (GameState == eGameState.ALLSessionStopped)
            {
                m_gameState = eGameState.Stopped;

                if (IsWin == true)
                {
                    List<Player> players = GetAllFightPlayers();
                    //系统自动给玩家翻牌
                    foreach (Player p in players)
                    {
                        if (p.IsActive && p.CanTakeOut > 0)
                        {
                            p.HasPaymentTakeCard = true;
                            int left = p.CanTakeOut;
                            for (int i = 0; i < left; i++)
                            {
                                TakeCard(p);
                            }
                        }
                    }

                    //展示未翻开的牌
                    if (RoomType == eRoomType.Treasure || RoomType == eRoomType.Boss)
                    {
                        SendShowCards();
                    }

                    //设置该玩家已经玩过该难度等级
                    if (GameType == eGameType.Treasure)
                    {
                        foreach (Player p in players)
                        {
                            p.PlayerDetail.SetPvePermission(m_info.ID, m_hardLevel);
                        }
                    }
                }

                lock (m_players)
                {
                    m_players.Clear();
                }

                OnGameStopped();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            foreach (Living living in m_livings)
            {
                living.Dispose();
            }
            try
            {
                m_missionAI.Dispose();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("game ai script m_missionAI.Dispose() error:{1}", ex);
            }
            try
            {
                m_gameAI.Dispose();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("game ai script m_gameAI.Dispose() error:{1}", ex);
            }
        }

        public void DoOther()
        {
            try
            {
                m_missionAI.DoOther();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("game ai script m_gameAI.DoOther() error:{1}", ex);
            }
        }

        internal void OnShooted()
        {
            try
            {
                m_missionAI.OnShooted();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("game ai script m_gameAI.OnShooted() error:{1}", ex);
            }
        }

        #endregion

        #region GameCalculate
        private int CalculateExperience(Player p)
        {
            //当(个人等级-NPC平均等级)>=7时，获得经验值固定为1

            if (TotalKillCount == 0)
            {
                return 1;
            }

            double gradeGap = Math.Abs(p.Grade - (double)TotalNpcGrade / TotalKillCount);
            if (gradeGap >= 7)
            {
                return 1;
            }
            //战斗表现修正
            //(个人击杀数/队伍总击杀数)(队伍总击杀数为0时此值为0) * 0.4
            //(个人伤害量/队伍总伤害量)(队伍总伤害量为0时此值为0) * 0.4
            //生存奖励（玩家生存此值为1，玩家死亡此值为0） * 0.4
            double behaveRevisal = 0;
            if (TotalKillCount > 0)
            {
                behaveRevisal += ((double)p.TotalKill / TotalKillCount) * 0.4;
            }
            if (TotalHurt > 0)
            {
                behaveRevisal += ((double)p.TotalHurt / TotalHurt) * 0.4;
            }
            if (p.IsLiving)
            {
                behaveRevisal += 0.4;
            }

            //等级差修正
            //当5<=|个人等级-NPC平均等级|<=6时，等级差修正=0.4
            //当3<=|个人等级-NPC平均等级|<=4时，等级差修正=0.7
            //当0<=|个人等级-NPC平均等级|<=2时，等级差修正=1
            double gradeGapRevisal = 1;
            if (gradeGap >= 3 && gradeGap <= 4)
            {
                gradeGapRevisal = 0.7;
            }
            else if (gradeGap >= 5 && gradeGap <= 6)
            {
                gradeGapRevisal = 0.4;
            }

            //战斗人数修正
            //人数修正=[0.9+（游戏开始队伍人数-1）*0.4]/玩家人数
            double playerCountRevisal = (0.9 + (BeginPlayersCount - 1) * 0.4) / PlayerCount;

            //战斗获得经验=被击杀NPC基础经验之和*战斗表现修正*等级差修正*人数修正
            double experience = 0;
            experience = TotalNpcExperience * behaveRevisal * gradeGapRevisal * playerCountRevisal;
            experience = experience == 0 ? 1 : experience;
            return (int)experience;
        }

        private int CalculateScore(Player p)
        {
            //（200-回合数）*5 + 个人击杀数*5 + （个人剩余HP/个人最大HP）*10
            int score = (200 - TurnIndex) * 5 + (p.TotalKill * 5) + (int)((double)p.Blood / p.MaxBlood) * 10;
            //加入战斗胜负修正：战斗失败时，以上评分-400，战斗胜利时，以上评分不变
            if (IsWin == false)
            {
                score -= 400;
            }
            return score;
        }

        private int CalculateHitRate(int hitTargetCount, int shootCount)
        {
            double toHit = 0;
            if (shootCount > 0)
            {
                toHit = (double)hitTargetCount / shootCount;
            }
            int hitRate = (int)(toHit * 100);
            return hitRate;
        }
        #endregion

        #region CheckState
        public override void CheckState(int delay)
        {
            AddAction(new CheckPVEGameStateAction(delay));
        }

        #endregion

        #region Gameinfo

        private List<int> m_mapHistoryIds;
        public List<int> MapHistoryIds
        {
            get { return m_mapHistoryIds; }
            set
            {
                m_mapHistoryIds = value;
            }
        }

        public eHardLevel HandLevel
        {
            get { return m_hardLevel; }
        }

        public MapPoint MapPos
        {
            get { return mapPos; }
        }

        public string IsBossWar
        {
            get { return m_IsBossType; }
            set { m_IsBossType = value; }
        }

        public List<string> GameOverResources
        {
            get { return m_gameOverResources; }
        }

        //关卡中boss死亡的翻牌
        public int[] BossCards;
        private int m_bossCardCount;
        public int BossCardCount
        {
            get { return m_bossCardCount; }
            set
            {
                if (value > 0)
                {
                    BossCards = new int[8];
                    m_bossCardCount = value;
                }
            }
        }

        public bool TakeBossCard(Player player)
        {
            int index = 0;

            for (int i = 0; i < BossCards.Length; i++)
            {
                if (Cards[i] == 0)
                {
                    index = i;
                    break;
                }
            }

            return TakeCard(player, index);
        }

        public bool TakeBossCard(Player player, int index)
        {
            if (player.IsActive == false || player.BossCardCount <= 0 || index < 0 || index > BossCards.Length || BossCards[index] > 0)
                return false;

            List<ItemInfo> infos = null;
            int templateId = 0;
            int gold = 0;
            int money = 0;
            int giftToken = 0;

            DropInventory.BossDrop(Map.Info.ID, ref infos);
            if (infos != null)
            {
                foreach (ItemInfo info in infos)
                {
                    ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken);
                    if (info != null)
                    {
                        player.PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count);
                        templateId = info.TemplateID;
                    }
                }
                player.PlayerDetail.AddGold(gold);
                player.PlayerDetail.AddMoney(money);
                player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_BossDrop, player.PlayerDetail.PlayerCharacter.ID, money, player.PlayerDetail.PlayerCharacter.Money);
                player.PlayerDetail.AddGiftToken(giftToken);
            }
            player.BossCardCount--;
            BossCards[index] = 1;
            SendGamePlayerTakeCard(player, index, templateId, gold, money, giftToken);
            return true;
        }

        public void SendMissionInfo()
        {
            if (m_missionInfo == null)
                return;

            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.GAME_MISSION_INFO);
            pkg.WriteString(m_missionInfo.Name);
            pkg.WriteString(m_missionInfo.Success);
            pkg.WriteString(m_missionInfo.Failure);
            pkg.WriteString(m_missionInfo.Description);
            pkg.WriteString(m_missionInfo.Title);
            pkg.WriteInt(TotalMissionCount);
            pkg.WriteInt(SessionId);
            pkg.WriteInt(TotalTurn);
            pkg.WriteInt(TotalCount);
            pkg.WriteInt(Param1);
            pkg.WriteInt(Param2);
            SendToAll(pkg);
        }

        public void SendUpdateUiData()
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.GAME_UI_DATA);

            //在第四关结束时，使用TotalKillCount来计数木板
            int count = 0;
            try
            {
                count = m_missionAI.UpdateUIData();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("game ai script {0} error:{1}", string.Format("m_missionAI.UpdateUIData()"), ex);
            }

            pkg.WriteInt(TurnIndex);
            pkg.WriteInt(count);
            pkg.WriteInt(Param3);
            pkg.WriteInt(Param4);
            SendToAll(pkg);
        }

        /// <summary>
        /// 系统翻牌
        /// </summary>
        internal void SendShowCards()
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.SHOW_CARDS);
            int count = 0;
            List<int> cardIndexs = new List<int>();
            for (int i = 0; i < Cards.Length; i++)
            {
                if (Cards[i] == 0)
                {
                    cardIndexs.Add(i);
                    count++;
                }
            }
            pkg.WriteInt(count);
            int gold = 0;
            int money = 0;
            int giftToken = 0;
            int templateID = 0;
            foreach (int index in cardIndexs)
            {
                List<ItemInfo> infos = null;
                DropInventory.CopyDrop(m_missionInfo.Id, 2, ref infos);
                if (infos != null)
                {
                    foreach (ItemInfo info in infos)
                    {
                        templateID = info.TemplateID;
                    }
                }
                else
                {
                    gold = 100;
                }
                pkg.WriteByte((byte)index);
                pkg.WriteInt(templateID);
                pkg.WriteInt(1);
            }
            SendToAll(pkg);
        }

        public void SendGameObjectFocus(int type, string name, int delay, int finishTime)
        {

            Physics[] physics = null;

            physics = FindPhysicalObjByName(name);

            foreach (Physics p in physics)
            {
                AddAction(new FocusAction(p, type, delay, finishTime));
            }
        }

        private void SendCreateGameToSingle(PVEGame game, IGamePlayer gamePlayer)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.GAME_ROOM_INFO);
            pkg.WriteInt(game.Map.Info.ID);
            pkg.WriteInt((byte)game.RoomType);
            pkg.WriteInt((byte)game.GameType);
            pkg.WriteInt(game.TimeType);
            List<Player> players = game.GetAllFightPlayers();
            pkg.WriteInt(players.Count);
            foreach (Player p in players)
            {
                IGamePlayer gp = p.PlayerDetail;
                pkg.WriteInt(gp.PlayerCharacter.ID);
                //zoneid
                //pkg.WriteInt(4);
                pkg.WriteString(gp.PlayerCharacter.NickName);
                //isvip
                pkg.WriteBoolean(true);
                pkg.WriteInt(5);
                pkg.WriteBoolean(gp.PlayerCharacter.Sex);
                pkg.WriteInt(gp.PlayerCharacter.Hide);
                pkg.WriteString(gp.PlayerCharacter.Style);
                pkg.WriteString(gp.PlayerCharacter.Colors);
                pkg.WriteString(gp.PlayerCharacter.Skin);
                pkg.WriteInt(gp.PlayerCharacter.Grade);
                pkg.WriteInt(gp.PlayerCharacter.Repute);
                if (gp.MainWeapon == null)
                {
                    pkg.WriteInt(0);
                }
                else
                {
                    pkg.WriteInt(gp.MainWeapon.TemplateID);
                    pkg.WriteInt(0);
                    pkg.WriteString("");
                    pkg.WriteDateTime(DateTime.MinValue);
                }
                pkg.WriteInt(0);
                pkg.WriteInt(gp.PlayerCharacter.ConsortiaID);
                pkg.WriteString(gp.PlayerCharacter.ConsortiaName);
                pkg.WriteInt(gp.PlayerCharacter.ConsortiaLevel);
                pkg.WriteInt(gp.PlayerCharacter.ConsortiaRepute);

                pkg.WriteInt(p.Team);
                pkg.WriteInt(p.Id);
                pkg.WriteInt(p.MaxBlood);
                pkg.WriteBoolean(p.Ready);
            }

            MissionInfo missionInfo = game.Misssions[game.SessionId - 1];
            pkg.WriteString(missionInfo.Name);
            pkg.WriteString(missionInfo.Name);
            pkg.WriteString(missionInfo.Success);
            pkg.WriteString(missionInfo.Failure);
            pkg.WriteString(missionInfo.Description);
            pkg.WriteInt(game.TotalMissionCount);
            pkg.WriteInt(game.SessionId - 1);
            gamePlayer.SendTCP(pkg);
        }

        public void SendPlayerInfoInGame(PVEGame game, IGamePlayer gp, Player p)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.PLAY_INFO_IN_GAME);
            pkg.WriteInt(gp.PlayerCharacter.ID);
            //zoneid
            pkg.WriteInt(4);
            pkg.WriteInt(p.Team);
            pkg.WriteInt(p.Id);
            pkg.WriteInt(p.MaxBlood);
            pkg.WriteBoolean(p.Ready);
            game.SendToAll(pkg);
        }

        public void SendPlaySound(string playStr)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.PLAY_SOUND);
            pkg.WriteString(playStr);
            SendToAll(pkg);
        }

        public void SendLoadResource(List<LoadingFileInfo> loadingFileInfos)
        {
            if (loadingFileInfos != null && loadingFileInfos.Count > 0)
            {
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
                pkg.WriteByte((byte)eTankCmdType.LOAD_RESOURCE);
                pkg.WriteInt(loadingFileInfos.Count);
                foreach (LoadingFileInfo file in loadingFileInfos)
                {
                    pkg.WriteInt(file.Type);
                    pkg.WriteString(file.Path);
                    pkg.WriteString(file.ClassName);
                }
                SendToAll(pkg);
            }
        }

        private int m_pveGameDelay;
        public int PveGameDelay
        {
            get { return m_pveGameDelay; }
            set { m_pveGameDelay = value; }
        }

        public override void MinusDelays(int lowestDelay)
        {
            m_pveGameDelay = m_pveGameDelay - lowestDelay;
            base.MinusDelays(lowestDelay);
        }

        #endregion

        public void Print(string str)
        {
            Console.WriteLine(str);
        }


    }
}
