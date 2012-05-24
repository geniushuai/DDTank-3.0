using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System.Drawing;
using Game.Base.Packets;
using System.Collections;
using Game.Logic.Phy.Maps;
using Game.Logic.Actions;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using System.Configuration;


namespace Game.Logic
{
    public class PVPGame : BaseGame
    {
        private static readonly new ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //TrieuLSL
        private static readonly int Xu_Rate = Int32.Parse(ConfigurationSettings.AppSettings["Xu_Rate"]);
        private static readonly int Gold_Rate = Int32.Parse(ConfigurationSettings.AppSettings["Gold_Rate"]);
        private static readonly int GP_Rate = Int32.Parse(ConfigurationSettings.AppSettings["GP_Rate"]);
        private static readonly int Gift_Rate = Int32.Parse(ConfigurationSettings.AppSettings["Gift_Rate"]);

        private List<Player> m_redTeam;

        private float m_redAvgLevel;

        private List<Player> m_blueTeam;

        private float m_blueAvgLevel;

        private int BeginPlayerCount;

        private string teamAStr;
        private string teamBStr;
        private DateTime beginTime;

        public PVPGame(int id, int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, Map map, eRoomType roomType, eGameType gameType, int timeType)
            : base(id, roomId, map, roomType, gameType, timeType)
        {
            m_redTeam = new List<Player>();
            m_blueTeam = new List<Player>();

            StringBuilder sbTeampA = new StringBuilder();
            m_redAvgLevel = 0;
            foreach (IGamePlayer player in red)
            {
                Player fp = new Player(player, PhysicalId++, this, 1);
                sbTeampA.Append(player.PlayerCharacter.ID).Append(",");
                fp.Reset();
                fp.Direction = m_random.Next(0, 1) == 0 ? 1 : -1;

                AddPlayer(player, fp);

                m_redTeam.Add(fp);
                m_redAvgLevel += player.PlayerCharacter.Grade;
            }
            m_redAvgLevel = m_redAvgLevel / m_redTeam.Count;
            teamAStr = sbTeampA.ToString();

            StringBuilder sbTeampB = new StringBuilder();
            m_blueAvgLevel = 0;
            foreach (IGamePlayer player in blue)
            {
                Player fp = new Player(player, PhysicalId++, this, 2);
                sbTeampB.Append(player.PlayerCharacter.ID).Append(",");
                fp.Reset();
                fp.Direction = m_random.Next(0, 1) == 0 ? 1 : -1;

                AddPlayer(player, fp);

                m_blueTeam.Add(fp);
                m_blueAvgLevel += player.PlayerCharacter.Grade;
            }
            m_blueAvgLevel = m_blueAvgLevel / blue.Count;
            teamBStr = sbTeampB.ToString();

            BeginPlayerCount = m_redTeam.Count + m_blueTeam.Count;
            beginTime = DateTime.Now;
        }

        public Player CurrentPlayer
        {
            get { return m_currentLiving as Player; }
        }

        public void Prepare()
        {
            if (GameState == eGameState.Inited)
            {
                SendCreateGame();
                m_gameState = eGameState.Prepared;
                CheckState(0);
            }
        }

        public void StartLoading()
        {
            if (GameState == eGameState.Prepared)
            {
                ClearWaitTimer();
                SendStartLoading(60);
                AddAction(new WaitPlayerLoadingAction(this, 61 * 1000));
                m_gameState = eGameState.Loading;
            }
        }

        public void StartGame()
        {
            if (GameState == eGameState.Loading)
            {

                m_gameState = eGameState.Playing;
                ClearWaitTimer();

                //同步时间
                SendSyncLifeTime();

                List<Player> list = GetAllFightPlayers();

                MapPoint mapPos = MapMgr.GetMapRandomPos(m_map.Info.ID);
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
                pkg.WriteByte((byte)eTankCmdType.START_GAME);
                pkg.WriteInt(list.Count);
                foreach (Player p in list)
                {
                    p.Reset();

                    Point pos = GetPlayerPoint(mapPos, p.Team);
                    p.SetXY(pos);
                    m_map.AddPhysical(p);
                    p.StartMoving();

                    p.StartGame();

                    pkg.WriteInt(p.Id);
                    pkg.WriteInt(p.X);
                    pkg.WriteInt(p.Y);
                    pkg.WriteInt(p.Direction);
                    pkg.WriteInt(p.Blood);
                    pkg.WriteInt(2);
                    //ratio
                    pkg.WriteInt(34);
                    //dander
                    pkg.WriteInt(p.Dander);
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
                }

                SendToAll(pkg);

                WaitTime(list.Count * 1000);

                OnGameStarted();
            }
        }

        public void NextTurn()
        {
            if (GameState == eGameState.Playing)
            {
                ClearWaitTimer();
                ClearDiedPhysicals();
                CheckBox();

                m_turnIndex++;
                UpdateWind(GetNextWind(), false);

                List<Box> newBoxes = CreateBox();

                List<Physics> list = m_map.GetAllPhysicalSafe();
                foreach (Physics p in list)
                {
                    p.PrepareNewTurn();
                }

                m_currentLiving = FindNextTurnedLiving();
                MinusDelays(m_currentLiving.Delay);
                //Console.WriteLine("本轮坐标Y：{0}", CurrentLiving.Y);
                m_currentLiving.PrepareSelfTurn();
                if (CurrentLiving.IsFrost == false)
                {
                    m_currentLiving.StartAttacking();
                    SendGameNextTurn(m_currentLiving, this, newBoxes);

                    if (m_currentLiving.IsAttacking)
                    {
                        AddAction(new WaitLivingAttackingAction(m_currentLiving, m_turnIndex, (m_timeType + 20) * 1000));
                    }

                }
                OnBeginNewTurn();
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
            //if (player.CanTakeOut == 0)
            //    return false;

            if (player.IsActive == false || index < 0 || index > Cards.Length || player.FinishTakeCard || Cards[index] > 0)
                return false;

            int gold = 0;
            int money = 0;
            int giftToken = 0;
            int templateID = 0;
            List<ItemInfo> infos = null;
            if (DropInventory.CardDrop(RoomType, ref  infos))
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
                }
                else
                {
                    gold = 100;
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

        public void GameOver()
        {
            if (GameState == eGameState.Playing)
            {
                m_gameState = eGameState.GameOver;
                ClearWaitTimer();

                CurrentTurnTotalDamage = 0;

                List<Player> players = GetAllFightPlayers();

                int winTeam = -1;
                foreach (Player p in players)
                {
                    if (p.IsLiving)
                    {
                        winTeam = p.Team;
                        break;
                    }
                }

                if (winTeam == -1 && CurrentPlayer != null)
                    winTeam = CurrentPlayer.Team;



                // int riches = 0;
                int losebaseoffer = 0;
                int winbaseoffer = 0;
                int riches = CalculateGuildMatchResult(players, winTeam);
                if (RoomType == eRoomType.Match)
                {
                    if (GameType == eGameType.Guild)
                    {
                        winbaseoffer = 10;
                        losebaseoffer = -10;
                        winbaseoffer += players.Count / 2;
                        losebaseoffer += (int)Math.Round(players.Count / 2 * 0.5);
                    }
                    else
                    {
                        winbaseoffer = 3;
                        losebaseoffer = -3;
                    }


                }
                int canBlueTakeOut = 0;
                int canRedTakeOut = 0;

                foreach (Player p in players)
                {
                    if (p.TotalHurt > 0)
                    {
                        if (p.Team == 1)
                            canRedTakeOut = 1;
                        else
                            canBlueTakeOut = 1;
                    }
                }

                ////P赢=[2+伤害的血量*0.1%+击杀数*0.5+（命中次数/玩家总的回合次数）*2]*对方平均等级*[0.9+(游戏开始时对方玩家人数-1)*30%]
                ////P输=[伤害的血量*0.1%+击杀数*0.5+（命中次数/玩家总的回合次数）*2]*对方平均等级*[0.9+(游戏开始时对方玩家人数-1)*30%]

                GSPacketIn pkg = new GSPacketIn((short)ePackageType.GAME_CMD);
                pkg.WriteByte((byte)eTankCmdType.GAME_OVER);
                pkg.WriteInt(PlayerCount);


                foreach (Player p in players)
                {
                  
                        float againstTeamLevel = p.Team == 1 ? m_blueAvgLevel : m_redAvgLevel;
                        float againstTeamCount = p.Team == 1 ? m_blueTeam.Count : m_redTeam.Count;
                        float disLevel = Math.Abs(againstTeamLevel - p.PlayerDetail.PlayerCharacter.Grade);
                        float winPlus = p.Team == winTeam ? 2 : 0;
                        int gp = 0;
                        int totalShoot = p.TotalShootCount == 0 ? 1 : p.TotalShootCount;

                        if (m_roomType == eRoomType.Match || disLevel < 5)
                        {
                            gp = (int)Math.Ceiling((winPlus + p.TotalHurt * 0.001 + p.TotalKill * 0.5 + (p.TotalHitTargetCount / totalShoot) * 2) * againstTeamLevel * (0.9 + (againstTeamCount - 1) * 0.3));
                        }
                        gp = gp == 0 ? 1 : gp;

                        //TrieuLSL
                        //RoomType=eRoomType.
                        if (RoomType != eRoomType.Freedom)
                        {
                            p.PlayerDetail.AddMoney((int)Math.Round((double)Xu_Rate * gp));
                            p.PlayerDetail.AddGold((int)Math.Round((double)Gold_Rate * gp));
                            p.PlayerDetail.AddGiftToken((int)Math.Round((double)Gift_Rate * gp));
                        }
                        p.GainGP = p.PlayerDetail.AddGP((int)GP_Rate * gp);

                        var msg = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg1", (int)Math.Round((double)Xu_Rate * gp), (int)Math.Round((double)Xu_Rate * gp));
                        p.PlayerDetail.SendMessage(msg);
                        p.CanTakeOut = p.Team == 1 ? canRedTakeOut : canBlueTakeOut;
                        riches += p.GainOffer;

             
                        pkg.WriteInt(p.Id);
                        pkg.WriteBoolean(p.Team==winTeam);
                        pkg.WriteInt(p.Grade);
                        pkg.WriteInt(p.PlayerDetail.PlayerCharacter.GP);
                        pkg.WriteInt(p.TotalKill);
                        pkg.WriteInt(p.TotalHurt);
                //    _loc_10.fightGP = _loc_2.readInt();
                        pkg.WriteInt(p.TotalShootCount);
                //    _loc_10.ghostGP = _loc_2.readInt();
                        pkg.WriteInt(p.TotalCure);
                //    _loc_10.gpForVIP = _loc_2.readInt();
                        pkg.WriteInt(1);
                //    _loc_10.gpForConsortia = _loc_2.readInt();
                        pkg.WriteInt(1);
                //    _loc_10.gpForSpouse = _loc_2.readInt();
                        pkg.WriteInt(100);
                //    _loc_10.gpForServer = _loc_2.readInt();
                        pkg.WriteInt(100);
                //    _loc_10.gpForApprenticeOnline = _loc_2.readInt();
                        pkg.WriteInt(0);
                //    _loc_10.gpForApprenticeTeam = _loc_2.readInt();
                        pkg.WriteInt(0);
                //    _loc_10.gpForDoubleCard = _loc_2.readInt();
                        pkg.WriteInt(0);
                //    _loc_10.gpForPower = _loc_2.readInt();
                        pkg.WriteInt(1);
                //    _loc_10.gainGP = _loc_2.readInt();
                        pkg.WriteInt(p.GainGP);
                //    _loc_10.offerFight = _loc_2.readInt();
                        pkg.WriteInt(0);
                //    _loc_10.offerDoubleCard = _loc_2.readInt();
                        pkg.WriteInt(0);
                //    _loc_10.offerVIP = _loc_2.readInt();
                        pkg.WriteInt(0);
                //    _loc_10.offerService = _loc_2.readInt();
                        pkg.WriteInt(0);
                //    _loc_10.offerBuff = _loc_2.readInt();
                        pkg.WriteInt(0);
                //    _loc_10.offerConsortia = _loc_2.readInt();
                        pkg.WriteInt(0);
                //    _loc_10.gainOffer = _loc_2.readInt();
                        pkg.WriteInt(p.GainOffer);
                //    _loc_10.canTakeOut = _loc_2.readInt();
                        pkg.WriteInt(p.CanTakeOut);
                    }
                pkg.WriteInt(riches);
                SendToAll(pkg);


                //GSPacketIn pkg2 = new GSPacketIn((byte)eTankCmdType.);
                //pkg2.WriteInt(1);
                //pkg2.WriteString("Bạn đang sử dụng gunny phiên bản do smallwind1912 phát triển mọi thắc mắc xin liên hệ smallwind1912@gmail.com hoặc truy cập wohzoo.com ");
                //client.Out.SendTCP(pkg2);
                StringBuilder sb = new StringBuilder();
                foreach (Player p in players)
                {
                    p.PlayerDetail.OnGameOver(this, p.Team == winTeam, p.GainGP);

                }

                string templateIdsStr = "";
                OnGameOverLog(RoomId, RoomType, GameType, 0, beginTime, DateTime.Now, BeginPlayerCount, Map.Info.ID, teamAStr, teamBStr, templateIdsStr, winTeam, BossWarField);
                WaitTime(15 * 1000);
                OnGameOverred();

            }
        }

        public override void Stop()
        {
            if (GameState == eGameState.GameOver)
            {
                m_gameState = eGameState.Stopped;

                List<Player> players = GetAllFightPlayers();
                foreach (Player p in players)
                {
                    if (p.IsActive && p.FinishTakeCard == false && p.CanTakeOut > 0)
                    {
                        TakeCard(p);
                    }
                }

                lock (m_players)
                {
                    m_players.Clear();
                }

                base.Stop();
            }

        }

        private int CalculateGuildMatchResult(List<Player> players, int winTeam)
        {
            if (RoomType == eRoomType.Match)
            {
                StringBuilder winStr = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5"));
                StringBuilder loseStr = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5"));

                IGamePlayer winPlayer = null;
                IGamePlayer losePlayer = null;
                int count = 0;

                foreach (Player p in players)
                {
                    if (p.Team == winTeam)
                    {
                        winStr.Append(string.Format("[{0}]", p.PlayerDetail.PlayerCharacter.NickName));
                        winPlayer = p.PlayerDetail;
                    }
                    else
                    {
                        loseStr.Append(string.Format("{0}", p.PlayerDetail.PlayerCharacter.NickName));
                        losePlayer = p.PlayerDetail;
                        count++;
                    }
                }
                if (losePlayer != null)
                {
                    winStr.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg1") + losePlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg2"));
                    loseStr.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg3") + winPlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg4"));

                    int riches = 0;
                    if (GameType == eGameType.Guild)
                    {
                        riches = count + TotalHurt / 2000;
                    }
                    winPlayer.ConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID, losePlayer.PlayerCharacter.ConsortiaID, Players, RoomType, GameType, TotalHurt, players.Count);
                    if (winPlayer.ServerID != losePlayer.ServerID)
                        losePlayer.ConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID, losePlayer.PlayerCharacter.ConsortiaID, Players, RoomType, GameType, TotalHurt, players.Count);
                    if (GameType == eGameType.Guild)
                    {
                        winPlayer.SendConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID, riches, winStr.ToString());
                        //losePlayer.SendConsortiaFight(losePlayer.PlayerCharacter.ConsortiaID, -riches, loseStr.ToString());
                    }
                    return riches;
                }

            }
            return 0;
        }

        public bool CanGameOver()
        {
            bool red = true;
            bool blue = true;
            foreach (Player p in m_redTeam)
            {
                if (p.IsLiving)
                {
                    red = false;
                    break;
                }
            }

            foreach (Player p in m_blueTeam)
            {
                if (p.IsLiving)
                {
                    blue = false;
                    break;
                }
            }
            return red || blue;
        }

        public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
        {
            Player player = base.RemovePlayer(gp, IsKick);
            if (player != null && player.IsLiving && GameState != eGameState.Loading)
            {

                gp.RemoveGP(gp.PlayerCharacter.Grade * 12);
                string msg = null;
                string msg1 = null;
                if (RoomType == eRoomType.Match)
                {
                    if (GameType == eGameType.Guild)
                    {
                        msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", gp.PlayerCharacter.Grade * 12, 15);
                        gp.RemoveOffer(15);
                        msg1 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12, 15);

                    }
                    else if (GameType == eGameType.Free)
                    {
                        msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", gp.PlayerCharacter.Grade * 12, 5);
                        gp.RemoveOffer(5);
                        msg1 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12, 5);
                    }
                }
                else
                {
                    msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg4", gp.PlayerCharacter.Grade * 12);
                    msg1 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg5", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12);
                }
                SendMessage(gp, msg, msg1, 3);

                if (GetSameTeam() == true)
                {
                    CurrentLiving.StopAttacking();
                    CheckState(0);
                }
            }
            return player;

        }

        public override void CheckState(int delay)
        {
            AddAction(new CheckPVPGameStateAction(delay));
        }


    }
}
