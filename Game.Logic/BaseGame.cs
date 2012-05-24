using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Bussiness.Managers;
using Game.Base.Packets;
using log4net;
using SqlDataProvider.Data;
using Game.Logic.Phy.Object;
using Game.Logic.Phy.Maps;
using Game.Logic.Actions;
using System.Text;
using Bussiness;
using System.Configuration;

namespace Game.Logic
{
    public class  BaseGame : AbstractGame
    {
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected int m_turnIndex
        {
            get { return turnIndex; }
            set { turnIndex = value; }
        }

        protected int turnIndex;

        protected int m_nextWind;

        protected eGameState m_gameState;

        protected Map m_map;

        protected Dictionary<int, Player> m_players;

        protected List<Living> m_livings;

        protected Random m_random;

        protected TurnedLiving m_currentLiving;

        public int PhysicalId;

        public int CurrentTurnTotalDamage;
        public int TotalHurt;

        public int ConsortiaAlly;

        public int RichesRate;

        public string BossWarField;

        private ArrayList m_actions;

        private List<TurnedLiving> m_turnQueue;

        private int m_roomId;

        public BaseGame(int id, int roomId, Map map, eRoomType roomType, eGameType gameType, int timeType)
            : base(id, roomType, gameType, timeType)
        {
            m_roomId = roomId;
            m_players = new Dictionary<int, Player>();
            m_turnQueue = new List<TurnedLiving>();
            m_livings = new List<Living>();

            m_random = new Random();

            m_map = map;
            m_actions = new ArrayList();
            PhysicalId = 0;
            BossWarField = "";

            m_tempBox = new List<Box>();
            m_tempPoints = new List<Point>();

            if (roomType == eRoomType.Treasure || roomType == eRoomType.Boss)
            {
                Cards = new int[21];
            }
            else
            {
                Cards = new int[8];
            }

            m_gameState = eGameState.Inited;
        }

        public int RoomId
        {
            get { return m_roomId; }
        }

        public Dictionary<int, Player> Players
        {
            get { return m_players; }
        }

        public int PlayerCount
        {
            get
            {
                lock (m_players)
                {
                    return m_players.Count;
                }
            }
        }

        public int TurnIndex
        {
            get { return m_turnIndex; }
        }

        public eGameState GameState
        {
            get { return m_gameState; }
        }

        public float Wind
        {
            get { return m_map.wind; }
        }

        public void SetWind(int wind)
        {
            m_map.wind = wind;
        }

        public Map Map
        {
            get { return m_map; }
        }

        public bool SetMap(int mapId)
        {
            if (GameState == eGameState.Playing)
                return false;
            Map map = MapMgr.CloneMap(mapId);
            if (map != null)
            {
                m_map = map;
                return true;
            }
            return false;
        }

        public int GetTurnWaitTime()
        {
            return m_timeType;
        }

        public List<TurnedLiving> TurnQueue
        {
            get { return m_turnQueue; }
        }

        protected void AddPlayer(IGamePlayer gp, Player fp)
        {
            lock (m_players)
            {
                m_players.Add(fp.Id, fp);
                //玩家没有武器，则不加入到m_turnQueue中去
                if (fp.Weapon == null)
                {
                    return;
                }
                m_turnQueue.Add(fp);
            }
        }

        public virtual void AddLiving(Living living)
        {
            m_map.AddPhysical(living);

            //玩家没有武器，则不加入到m_turnQueue中去
            if (living is Player)
            {
                Player p = living as Player;
                if (p.Weapon == null)
                {
                    return;
                }
            }

            if (living is TurnedLiving)
            {
                m_turnQueue.Add(living as TurnedLiving);
            }
            else
            {
                m_livings.Add(living);
            }

            SendAddLiving(living);
        }

        public virtual void AddPhysicalObj(PhysicalObj phy, bool sendToClient)
        {
            m_map.AddPhysical(phy);
            phy.SetGame(this);
            if (sendToClient)
            {
                SendAddPhysicalObj(phy);
            }
        }

        public virtual void AddPhysicalTip(PhysicalObj phy, bool sendToClient)
        {
            m_map.AddPhysical(phy);
            phy.SetGame(this);
            if (sendToClient)
            {
                SendAddPhysicalTip(phy);
            }
        }


        public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
        {
            Player player = null;
            lock (m_players)
            {
                foreach (Player p in m_players.Values)
                {
                    if (p.PlayerDetail == gp)
                    {
                        player = p;
                        m_players.Remove(p.Id);
                        break;
                    }
                }
            }
            if (player != null)
            {
                AddAction(new RemovePlayerAction(player));
            }
            return player;
        }

        public void RemovePhysicalObj(PhysicalObj phy, bool sendToClient)
        {
            m_map.RemovePhysical(phy);
            phy.SetGame(null);
            if (sendToClient)
            {
                SendRemovePhysicalObj(phy);
            }
        }

        public void RemoveLiving(int id)
        {
            SendRemoveLiving(id);
        }

        public List<Living> GetLivedLivings()
        {
            List<Living> temp = new List<Living>();
            foreach (Living living in m_livings)
            {
                if (living.IsLiving)
                {
                    temp.Add(living);
                }
            }
            return temp;
        }

        public void ClearDiedPhysicals()
        {
            List<Living> temp = new List<Living>();
            foreach (Living living in m_livings)
            {
                if (living.IsLiving == false)
                    temp.Add(living);
            }

            foreach (Living living in temp)
            {
                m_livings.Remove(living);
                living.Dispose();
            }

            List<Living> turnedTemp = new List<Living>();
            foreach (TurnedLiving turnedLiving in m_turnQueue)
            {
                if (turnedLiving.IsLiving == false)
                    turnedTemp.Add(turnedLiving);
            }

            foreach (TurnedLiving turnedLiving in turnedTemp)
            {
                m_turnQueue.Remove(turnedLiving);
            }

            List<Physics> phys = m_map.GetAllPhysicalSafe();
            foreach (Physics phy in phys)
            {
                if (phy.IsLiving == false && !(phy is Player))
                {
                    m_map.RemovePhysical(phy);
                }
            }
        }

        public bool HasPlayer
        {
            get { return m_players.Count > 0; }
        }

        public Random Random
        {
            get { return m_random; }
        }

        public TurnedLiving CurrentLiving
        {
            get { return m_currentLiving; }
        }

        public bool IsAllComplete()
        {
            List<Player> list = GetAllFightPlayers();
            foreach (Player p in list)
            {
                if (p.LoadingProcess < 100)
                {
                    return false;
                }
            }
            return true;
        }

        public Player FindPlayer(int id)
        {
            lock (m_players)
            {
                if (m_players.ContainsKey(id))
                {
                    return m_players[id];
                }
            }
            return null;
        }

        public TurnedLiving FindNextTurnedLiving()
        {
            if (m_turnQueue.Count == 0)
            {
                return null;
            }
            int start = m_random.Next(m_turnQueue.Count - 1);

            TurnedLiving player = m_turnQueue[start];
            int delay = player.Delay;
            for (int i = 0; i < m_turnQueue.Count; i++)
            {
                if (m_turnQueue[i].Delay < delay && m_turnQueue[i].IsLiving)
                {
                    delay = m_turnQueue[i].Delay;
                    player = m_turnQueue[i];
                }
            }

            player.TurnNum++;
            return player;
        }

        public virtual void MinusDelays(int lowestDelay)
        {
            foreach (TurnedLiving trunedLiving in m_turnQueue)
            {
                trunedLiving.Delay -= lowestDelay;
            }
        }

        public SimpleBoss[] FindAllBoss()
        {
            List<SimpleBoss> list = new List<SimpleBoss>();
            foreach (Living boss in m_livings)
            {
                if (boss is SimpleBoss)
                {
                    list.Add(boss as SimpleBoss);
                }
            }
            return list.ToArray();
        }

        public SimpleNpc[] FindAllNpc()
        {
            List<SimpleNpc> list = new List<SimpleNpc>();
            foreach (Living npc in m_livings)
            {
                if (npc is SimpleNpc)
                {
                    list.Add(npc as SimpleNpc);
                    return list.ToArray();
                }
            }
            return null;
        }

        public float GetNextWind()
        {
            int currentWind = (int)(Wind * 10);
            int wind = 0;
            if (currentWind > m_nextWind)
            {
                wind = currentWind - m_random.Next(11);
                if (currentWind <= m_nextWind)
                {
                    m_nextWind = m_random.Next(-40, 40);
                }
            }
            else
            {
                wind = currentWind + m_random.Next(11);
                if (currentWind >= m_nextWind)
                {
                    m_nextWind = m_random.Next(-40, 40);
                }
            }
            return ((float)wind / 10);
        }

        public void UpdateWind(float wind, bool sendToClient)
        {
            if (m_map.wind != wind)
            {
                m_map.wind = wind;

                if (sendToClient)
                {
                    SendGameUpdateWind(wind);
                }
            }
        }

        public int GetDiedPlayerCount()
        {
            int count = 0;
            foreach (Player p in m_players.Values)
            {
                if (p.IsActive && p.IsLiving == false)
                {
                    count++;
                }
            }
            return count;
        }

        protected Point GetPlayerPoint(MapPoint mapPos, int team)
        {
            List<Point> list = team == 1 ? mapPos.PosX : mapPos.PosX1;
            int rand = m_random.Next(list.Count);
            Point pos = list[rand];
            list.Remove(pos);
            return pos;
        }

        public virtual void CheckState(int delay) { }

        public override void ProcessData(GSPacketIn packet)
        {
            if (m_players.ContainsKey(packet.Parameter1))
            {
                Player player = m_players[packet.Parameter1];
                AddAction(new ProcessPacketAction(player, packet));
            }
        }

        #region FindNearestPlayer FindRandomPlayer FindPhsicalObjByName GetPlayerByIndex

        public Player GetPlayerByIndex(int index)
        {
            return m_players.ElementAt(index).Value;
        }

        public Player FindNearestPlayer(int x, int y)
        {
            double min = double.MaxValue;
            Player player = null;
            foreach (Player p in m_players.Values)
            {
                if (p.IsLiving)
                {
                    double dis = p.Distance(x, y);
                    if (dis < min)
                    {
                        min = dis;
                        player = p;
                    }
                }
            }

            return player;
        }

        public Player FindRandomPlayer()
        {
            List<Player> list = new List<Player>();
            foreach (Player player in m_players.Values)
            {
                if (player.IsLiving)
                {
                    list.Add(player);
                }
            }

            int next = Random.Next(0, list.Count);

            return list.ElementAt(next);
        }

        public int FindlivingbyDir(Living npc)
        {
            int left = 0;
            int right = 0;
            foreach (Player p in m_players.Values)
            {
                if (p.IsLiving)
                {
                    if (p.X > npc.X)
                    {
                        right++;
                    }
                    else
                    {
                        left++;
                    }
                }
            }
            if (right > left)
            {
                return 1;
            }
            else if (right < left)
            {
                return -1;
            }
            else
            {
                return -npc.Direction;
            }
        }

        public PhysicalObj[] FindPhysicalObjByName(string name)
        {
            List<PhysicalObj> phys = new List<PhysicalObj>();
            foreach (PhysicalObj phy in m_map.GetAllPhysicalObjSafe())
            {
                if (phy.Name == name)
                {
                    phys.Add(phy);
                }
            }
            return phys.ToArray();
        }

        public PhysicalObj[] FindPhysicalObjByName(string name, bool CanPenetrate)
        {
            List<PhysicalObj> phys = new List<PhysicalObj>();
            foreach (PhysicalObj phy in m_map.GetAllPhysicalObjSafe())
            {
                if (phy.Name == name && phy.CanPenetrate == CanPenetrate)
                {
                    phys.Add(phy);
                }
            }
            return phys.ToArray();
        }

        public Player GetFrostPlayerRadom()
        {
            List<Player> players = GetAllFightPlayers();
            List<Player> list = new List<Player>();
            foreach (Player player in players)
            {
                if (player.IsFrost == true)
                {
                    list.Add(player);
                }
            }
            if (list.Count > 0)
            {
                int next = Random.Next(0, list.Count);
                return list.ElementAt(next);
            }
            else
            {
                return null;
            }
        }


        #endregion

        #region Cards/TakeCard

        public int[] Cards;

        public virtual bool TakeCard(Player player)
        {
            return false;
        }

        public virtual bool TakeCard(Player player, int index)
        {
            return false;
        }


        #endregion

        #region Actions/Update

        private int m_lifeTime = 0;

        public int LifeTime
        {
            get { return m_lifeTime; }
        }

        public override void Pause(int time)
        {
            m_passTick = Math.Max(m_passTick, TickHelper.GetTickCount() + time);
        }

        public override void Resume()
        {
            m_passTick = 0;
        }

        public void AddAction(IAction action)
        {
            log.Info(action);
            lock (m_actions)
            {
                m_actions.Add(action);
            }
        }

        public void AddAction(ArrayList actions)
        {
            lock (m_actions)
            {
                m_actions.AddRange(actions);
            }
        }

        private long m_waitTimer = 0;
        private long m_passTick = 0;
        public void ClearWaitTimer()
        {
            m_waitTimer = 0;
        }
        public void WaitTime(int delay)
        {
            m_waitTimer = Math.Max(m_waitTimer, TickHelper.GetTickCount() + delay);
        }

        public long GetWaitTimer()
        {
            return m_waitTimer;
        }

        public int CurrentActionCount = 0;

        public void Update(long tick)
        {
            if (m_passTick < tick)
            {
                m_lifeTime++;
                ArrayList temp;

                lock (m_actions)
                {
                    temp = (ArrayList)m_actions.Clone();
                    m_actions.Clear();
                }

                if (temp != null && GameState != eGameState.Stopped)
                {
                    CurrentActionCount = temp.Count;
                    if (temp.Count > 0)
                    {
                        ArrayList left = new ArrayList();
                        foreach (IAction action in temp)
                        {
                            try
                            {
                                action.Execute(this, tick);
                                if (action.IsFinished(tick) == false)
                                {
                                    left.Add(action);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("Map update error:", ex);
                            }
                        }
                        AddAction(left);
                    }
                    else if (m_waitTimer < tick)
                    {
                        CheckState(0);
                    }
                }
            }
        }

        #endregion

        #region SendToAll/SendToTeam GetAllPlayer

        public List<Player> GetAllFightPlayers()
        {
            List<Player> list = new List<Player>();
            lock (m_players)
            {
                list.AddRange(m_players.Values);
            }

            return list;
        }

        public List<Player> GetAllLivingPlayers()
        {
            List<Player> list = new List<Player>();
            lock (m_players)
            {
                foreach (Player livedPlayer in m_players.Values)
                {
                    if (livedPlayer.IsLiving)
                        list.Add(livedPlayer);
                }
            }
            return list;
        }
        public bool GetSameTeam()
        {
            bool result = false;
            Player[] players = GetAllPlayers();
            foreach (Player p in players)
            {
                if (p.Team == players[0].Team)
                {
                    result = true;
                }
                else
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public Player[] GetAllPlayers()
        {
            return GetAllFightPlayers().ToArray();
        }

        public Player GetPlayer(IGamePlayer gp)
        {
            Player player = null;
            lock (m_players)
            {
                foreach (Player p in m_players.Values)
                {
                    if (p.PlayerDetail == gp)
                    {
                        player = p;
                        // m_players.Remove(p.Id);
                        break;
                    }
                }
            }
            return player;
        }

        public int GetPlayerCount()
        {
            return GetAllFightPlayers().Count;
        }

        public virtual void SendToAll(GSPacketIn pkg)
        {
            SendToAll(pkg, null);
        }

        public virtual void SendToAll(GSPacketIn pkg, IGamePlayer except)
        {
            if (pkg.Parameter2 == 0)
            {
                pkg.Parameter2 = LifeTime;
            }

            List<Player> temp = GetAllFightPlayers();
            foreach (Player p in temp)
            {
                if (p.IsActive && p.PlayerDetail != except)
                {
                    p.PlayerDetail.SendTCP(pkg);
                }
            }
        }

        public virtual void SendToTeam(GSPacketIn pkg, int team)
        {
            SendToTeam(pkg, team, null);
        }

        public virtual void SendToTeam(GSPacketIn pkg, int team, IGamePlayer except)
        {
            List<Player> temp = GetAllFightPlayers();
            foreach (Player p in temp)
            {
                if (p.IsActive && p.PlayerDetail != except && p.Team == team)
                {
                    p.PlayerDetail.SendTCP(pkg);
                }
            }
        }
        #endregion

        #region Box/Temp Point

        private List<Box> m_tempBox;
        private List<Point> m_tempPoints;

        public void AddTempPoint(int x, int y)
        {
            m_tempPoints.Add(new Point(x, y));
        }

        public Box AddBox(ItemInfo item, Point pos, bool sendToClient)
        {
            Box box = new Box(PhysicalId++, "1", item);
            box.SetXY(pos);
            AddPhysicalObj(box, sendToClient);
            return AddBox(box, sendToClient);
        }

        public Box AddBox(Box box, bool sendToClient)
        {
            m_tempBox.Add(box);
            AddPhysicalObj(box, sendToClient);
            return box;
        }

        public void CheckBox()
        {
            List<Box> temp = new List<Box>();
            foreach (Box b in m_tempBox)
            {
                if (b.IsLiving == false)
                {
                    temp.Add(b);
                }
            }

            foreach (Box b in temp)
            {
                m_tempBox.Remove(b);
                RemovePhysicalObj(b, true);
            }
        }

        public List<Box> CreateBox()
        {
            int total = m_players.Count + 2;
            int itemCount = 0;

            List<ItemInfo> infos = null;
            if (CurrentTurnTotalDamage > 0)
            {
                itemCount = m_random.Next(1, 3);
                if (m_tempBox.Count + itemCount > total)
                {
                    itemCount = total - m_tempBox.Count;
                }
                if (itemCount > 0)
                {
                    DropInventory.BoxDrop(m_roomType, ref infos);
                }
            }

            int ghostCount = GetDiedPlayerCount();
            int propCount = 0;
            if (ghostCount > 0)
            {
                propCount = m_random.Next(ghostCount);
            }
            if (m_tempBox.Count + itemCount + propCount > total)
            {
                propCount = total - m_tempBox.Count - itemCount;
            }
            if (propCount > 0)
            {
                //add:未开始，战斗掉落
                //MapMgr.GetRandomFightPropByCount(m_map.Info.ID, propCount, list);
            }

            List<Box> box = new List<Box>();
            if (infos != null)
            {
                for (int i = 0; i < m_tempPoints.Count; i++)
                {
                    int index = m_random.Next(m_tempPoints.Count);
                    Point p = m_tempPoints[index];
                    m_tempPoints[index] = m_tempPoints[i];
                    m_tempPoints[i] = p;
                }
                int count = Math.Min(infos.Count, m_tempPoints.Count);
                for (int i = 0; i < count; i++)
                {
                    //在BeginNextTurn中一起发给客户端
                    box.Add(AddBox(infos[i], m_tempPoints[i], false));
                }
            }

            m_tempPoints.Clear();
            return box;
        }

        #endregion

        #region AddLoadingFile/ClearLoadingFiles

        private List<LoadingFileInfo> m_loadingFiles = new List<LoadingFileInfo>();

        /// <summary>
        /// 添加加载的资源
        /// </summary>
        /// <param name="type">文件的类型:1 程序库 2 资源</param>
        /// <param name="file">文件的地址</param>
        /// <param name="className">检测类</param>
        public void AddLoadingFile(int type, string file, string className)
        {
            if (file == null || className == null)
                return;
            m_loadingFiles.Add(new LoadingFileInfo(type, file, className));
        }

        public void ClearLoadingFiles()
        {
            m_loadingFiles.Clear();
        }

        #endregion

        #region Statics Number/Method

        public int TotalCostMoney;
        public int TotalCostGold;


        public void AfterUseItem(ItemInfo item)
        {
            //TotalCostGold += item.Template.Gold;
            //TotalCostMoney += item.Template.Money;
        }

        #endregion

        #region Send/Protocal

        internal void SendCreateGame()
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);

            pkg.WriteByte((byte)eTankCmdType.GAME_CREATE);
            //pkg.WriteInt(m_map.Info.ID);
            pkg.WriteInt((byte)m_roomType);
            pkg.WriteInt((byte)m_gameType);
            pkg.WriteInt(m_timeType);

            List<Player> players = GetAllFightPlayers();
            pkg.WriteInt(players.Count);
            foreach (Player p in players)
            {
                IGamePlayer gp = p.PlayerDetail;
                pkg.WriteInt(4);
                pkg.WriteString("zonename");
                pkg.WriteInt(gp.PlayerCharacter.ID);
                pkg.WriteString(gp.PlayerCharacter.NickName);
                //isvip
                pkg.WriteBoolean(true);
                //viplevel
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
                }

                pkg.WriteInt(12);
                pkg.WriteString(" ");
                pkg.WriteDateTime(DateTime.Now);
                pkg.WriteInt(0);
                pkg.WriteInt(gp.PlayerCharacter.Nimbus);

                pkg.WriteInt(gp.PlayerCharacter.ConsortiaID);// pkg.WriteInt(0);
                pkg.WriteString(gp.PlayerCharacter.ConsortiaName);
                pkg.WriteInt(gp.PlayerCharacter.ConsortiaLevel);
                pkg.WriteInt(gp.PlayerCharacter.ConsortiaRepute);
                pkg.WriteInt(gp.PlayerCharacter.Win);
                pkg.WriteInt(gp.PlayerCharacter.Total);
                pkg.WriteInt(gp.PlayerCharacter.FightPower);
                pkg.WriteInt(5);
                pkg.WriteInt(0);
                pkg.WriteString("Master");
                //pkg.WriteInt(5);
                pkg.WriteInt(5);
                pkg.WriteString("honor");


                pkg.WriteBoolean(gp.PlayerCharacter.IsMarried);
                if (gp.PlayerCharacter.IsMarried)
                {
                    pkg.WriteInt(gp.PlayerCharacter.SpouseID);
                    pkg.WriteString(gp.PlayerCharacter.SpouseName);
                }
                pkg.WriteInt(5); pkg.WriteInt(5); pkg.WriteInt(5); pkg.WriteInt(5); pkg.WriteInt(5); 
                pkg.WriteInt(5);

                pkg.WriteInt(p.Team);
                pkg.WriteInt(p.Id);
                pkg.WriteInt(p.MaxBlood);                            

            }

            SendToAll(pkg);
        }

        internal void SendOpenSelectLeaderWindow(int maxTime)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);

            pkg.WriteByte((byte)eTankCmdType.GAME_CAPTAIN_AFFIRM);

            pkg.WriteInt(maxTime);
            SendToAll(pkg);
        }

        internal void SendStartLoading(int maxTime)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);

            pkg.WriteByte((byte)eTankCmdType.GAME_LOAD);
            pkg.WriteInt(maxTime);
            pkg.WriteInt(m_map.Info.ID);

            pkg.WriteInt(m_loadingFiles.Count);
            foreach (LoadingFileInfo file in m_loadingFiles)
            {
                pkg.WriteInt(file.Type);
                pkg.WriteString(file.Path);
                pkg.WriteString(file.ClassName);
            }

            SendToAll(pkg);
        }

        internal void SendAddPhysicalObj(PhysicalObj obj)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.ADD_BOX);

            pkg.WriteInt(obj.Id);
            pkg.WriteInt(obj.Type);
            pkg.WriteInt(obj.X);
            pkg.WriteInt(obj.Y);
            pkg.WriteString(obj.Model);
            pkg.WriteString(obj.CurrentAction);
            pkg.WriteInt(obj.Scale);
            pkg.WriteInt(obj.Scale);
            pkg.WriteInt(obj.Rotation);
            pkg.WriteInt(0);
            pkg.WriteInt(0);
            
            //var id:int = evt.pkg.readInt();
            //var type:int =  evt.pkg.readInt();
            //var px:int = evt.pkg.readInt();
            //var py:int = evt.pkg.readInt();
            //var model:String = evt.pkg.readUTF();
            //var action:String = evt.pkg.readUTF();
            //var pscaleX:int = evt.pkg.readInt();
            //var pscaleY:int = evt.pkg.readInt();
            //var protation:int = evt.pkg.readInt();
            //var layer:int = evt.pkg.readInt();

            SendToAll(pkg);
        }

        internal void SendAddPhysicalTip(PhysicalObj obj)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.ADD_TIP);

            pkg.WriteInt(obj.Id);
            pkg.WriteInt(obj.Type);
            pkg.WriteInt(obj.X);
            pkg.WriteInt(obj.Y);
            pkg.WriteString(obj.Model);
            pkg.WriteString(obj.CurrentAction);
            pkg.WriteInt(obj.Scale);
            pkg.WriteInt(obj.Rotation);

            SendToAll(pkg);
        }
        internal void SendPhysicalObjFocus(Physics obj, int type)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.FOCUS_ON_OBJECT);
            pkg.WriteInt(type);
            pkg.WriteInt(obj.X);
            pkg.WriteInt(obj.Y);
            SendToAll(pkg);
        }

        internal void SendPhysicalObjPlayAction(PhysicalObj obj)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.UPDATE_BOARD_STATE);
            pkg.WriteInt(obj.Id);
            pkg.WriteString(obj.CurrentAction);
            SendToAll(pkg);
        }

        internal void SendRemovePhysicalObj(PhysicalObj obj)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.DISAPPEAR);
            pkg.WriteInt(obj.Id);
            SendToAll(pkg);
            //TODO 完成删除物品
        }

        internal void SendRemoveLiving(int id)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.DISAPPEAR);
            pkg.WriteInt(id);
            SendToAll(pkg);
            //TODO 完成删除物品
        }

        internal void SendAddLiving(Living living)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);

            pkg.Parameter1 = living.Id;

            pkg.WriteByte((byte)eTankCmdType.ADD_LIVING);
            pkg.WriteByte((byte)living.Type); //Type;
            pkg.WriteInt(living.Id);
            pkg.WriteString(living.Name);
            pkg.WriteString(living.ModelId);
            pkg.WriteString("");
            pkg.WriteInt(living.X);
            pkg.WriteInt(living.Y);
            pkg.WriteInt(living.Blood);
            pkg.WriteInt(living.MaxBlood);
            pkg.WriteInt(living.Team);
            pkg.WriteByte((byte)living.Direction);
            SendToAll(pkg);
        }

        internal void SendPlayerMove(Player player, int type, int x, int y, byte dir, bool isLiving, string action)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);
            pkg.Parameter1 = player.Id;

            pkg.WriteByte((byte)eTankCmdType.MOVESTART);
            pkg.WriteByte((byte)type);
            pkg.WriteInt(x);
            pkg.WriteInt(y);
            pkg.WriteByte(dir);
            pkg.WriteBoolean(isLiving);
            pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "move");//怪物资源的动作

            SendToAll(pkg);
        }

        internal void SendLivingMoveTo(Living living, int fromX, int fromY, int toX, int toY, string action)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, living.Id);

            pkg.Parameter1 = living.Id;

            pkg.WriteByte((byte)eTankCmdType.LIVING_MOVETO);
            pkg.WriteInt(fromX);
            pkg.WriteInt(fromY);
            pkg.WriteInt(toX);
            pkg.WriteInt(toY);
            //TrieuLSL writespeed
            pkg.WriteInt(5);
            pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "");//怪物资源的动作
            pkg.WriteString("");
            SendToAll(pkg);
        }


        internal void SendLivingSay(Living living, string msg, int type)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, living.Id);

            pkg.Parameter1 = living.Id;

            pkg.WriteByte((byte)eTankCmdType.LIVING_SAY);
            pkg.WriteString(msg);
            pkg.WriteInt(type);
            SendToAll(pkg);

        }

        internal void SendLivingFall(Living living, int toX, int toY, int speed, string action, int type)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, living.Id);

            pkg.Parameter1 = living.Id;

            pkg.WriteByte((byte)eTankCmdType.LIVING_FALLING);
            pkg.WriteInt(toX);
            pkg.WriteInt(toY);
            pkg.WriteInt(speed);
            pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "");//怪物资源的动作
            pkg.WriteInt(type);

            SendToAll(pkg);
        }

        internal void SendLivingJump(Living living, int toX, int toY, int speed, string action, int type)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, living.Id);

            pkg.Parameter1 = living.Id;

            pkg.WriteByte((byte)eTankCmdType.LIVING_JUMP);
            pkg.WriteInt(toX);
            pkg.WriteInt(toY);
            pkg.WriteInt(speed);
            pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "");//怪物资源的动作
            pkg.WriteInt(type);

            SendToAll(pkg);
        }

        internal void SendLivingBeat(Living living, Living target, int totalDemageAmount, string action)
        {
            int dander = 0;
            if (target is Player)
            {
                Player p = target as Player;
                dander = p.Dander;
            }
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, living.Id);

            pkg.Parameter1 = living.Id;

            pkg.WriteByte((byte)eTankCmdType.LIVING_BEAT);
            pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "");//怪物资源的动作
            pkg.WriteInt(1);
            pkg.WriteInt(target.Id);
            pkg.WriteInt(totalDemageAmount);
            pkg.WriteInt(target.Blood);
            pkg.WriteInt(dander);
            pkg.WriteInt(1);
            SendToAll(pkg);//, (target as Player).PlayerDetail);
        }

        internal void SendLivingRangeAttacking(Living living, Living target, int totalDemageAmount, string action)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, living.Id);
            pkg.Parameter1 = living.Id;

            pkg.WriteByte((byte)eTankCmdType.LIVING_RANGEATTACKING);
            pkg.WriteInt(living.Id);
            pkg.WriteInt(totalDemageAmount);
            pkg.WriteInt(target.Blood);
            //pkg.WriteInt(0);
            //pkg
            //pkg.WriteString(!string.IsNullOrEmpty(action) ? action : "");
            
                //var livingID:int = e.pkg.readInt();
                //var demage:int = e.pkg.readInt();
                //var blood : int = e.pkg.readInt();
                //var dander:int = e.pkg.readInt();
            SendToAll(pkg);
        }

        internal void SendLivingPlayMovie(Living living, string action)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, living.Id);

            pkg.Parameter1 = living.Id;

            pkg.WriteByte((byte)eTankCmdType.LIVING_PLAYMOVIE);
            pkg.WriteString(action);

            SendToAll(pkg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="type">0:加血 1:减血 5:不显示 6:死亡,不播放动画</param>
        internal void SendGameUpdateHealth(Living player, int type, int value)
        {
            //Console.WriteLine("SendGameUpdateHealth {0} : {1} : {2}", player.Name, player.Blood, type);
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);

            pkg.Parameter1 = player.Id;

            pkg.WriteByte((byte)eTankCmdType.HEALTH);
            pkg.WriteByte((byte)type);
            pkg.WriteInt(player.Blood);
            pkg.WriteInt(value);
            pkg.WriteInt(0);

            SendToAll(pkg);
        }

        internal void SendGameUpdateDander(TurnedLiving player)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);
            pkg.Parameter1 = player.Id;
            pkg.WriteByte((byte)eTankCmdType.DANDER);
            pkg.WriteInt(player.Dander);
            SendToAll(pkg);
        }

        internal void SendGameUpdateFrozenState(Living player)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);
            pkg.Parameter1 = player.Id;
            pkg.WriteByte((byte)eTankCmdType.FROST);
            pkg.WriteBoolean(player.IsFrost);
            SendToAll(pkg);
        }

        internal void SendGameUpdateNoHoleState(Living player)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);
            pkg.Parameter1 = player.Id;
            pkg.WriteByte((byte)eTankCmdType.NOHOLE);
            pkg.WriteBoolean(player.IsNoHole);
            SendToAll(pkg);
        }

        internal void SendGameUpdateHideState(Living player)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);
            pkg.Parameter1 = player.Id;
            pkg.WriteByte((byte)eTankCmdType.HIDE);
            pkg.WriteBoolean(player.IsHide);
            SendToAll(pkg);
        }

        internal void SendGameUpdateSealState(Living player, int type)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);
            pkg.Parameter1 = player.Id;
            pkg.WriteByte((byte)eTankCmdType.SEAL);
            pkg.WriteByte((byte)type);
            pkg.WriteBoolean(player.GetSealState());
            SendToAll(pkg);
        }

        internal void SendGameUpdateShootCount(Player player)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);
            pkg.WriteByte((byte)eTankCmdType.ADDATTACK);
            pkg.WriteByte((byte)player.ShootCount);
            SendToAll(pkg);
        }

        internal void SendGameUpdateBall(Player player, bool Special)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);
            pkg.Parameter1 = player.Id;
            pkg.WriteByte((byte)eTankCmdType.CURRENTBALL);
            pkg.WriteBoolean(Special);
            pkg.WriteInt(player.CurrentBall.ID);
            //pkg.WriteByte((byte)player.BallCount);
            SendToAll(pkg);
        }

        internal void SendGamePickBox(Living player, int index, int arkType, string goods)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);
            pkg.WriteByte((byte)eTankCmdType.PICK);
            pkg.WriteByte((byte)index);
            pkg.WriteByte((byte)arkType);
            pkg.WriteString(goods);
            SendToAll(pkg);
        }

        internal void SendGameUpdateWind(float wind)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.WriteByte((byte)eTankCmdType.VANE);
            pkg.WriteInt((int)(wind * 10));
            SendToAll(pkg);
        }

        internal void SendPlayerUseProp(Player player, int type, int place, int templateID)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);
            pkg.Parameter1 = player.Id;
            pkg.WriteByte((byte)eTankCmdType.USING_PROP);
            pkg.WriteByte((byte)type);
            pkg.WriteInt(place);
            pkg.WriteInt(templateID);
            SendToAll(pkg);
        }

        internal void SendGamePlayerTakeCard(Player player, int index, int templateID, int gold, int money, int giftToken)
        {
            //Console.WriteLine("player name: {0}, templateId : {1}", player.PlayerDetail.PlayerCharacter.NickName, templateID);

            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, player.Id);
            pkg.Parameter1 = player.Id;
            pkg.WriteByte((byte)eTankCmdType.TAKE_CARD);
            pkg.WriteBoolean(false);
            pkg.WriteByte((byte)index);
            pkg.WriteInt(templateID);
            //pkg.WriteInt(1);
            pkg.WriteInt(gold);
            pkg.WriteInt(money);
            pkg.WriteInt(giftToken);
            SendToAll(pkg);
        }
        public static int getTurnTime(int timeType)
        {
           
            switch (timeType)
            {
                case 1:
                       return 6;
                case 2:
                        return 8;
                case 3:
                        return 11;
                case 4:
                        return 16;
                case 5:
                        return 21;
                case 6:
                        return 31;
                default:
                        return -1;
            }

        }
        internal void SendGameNextTurn(Living living, BaseGame game, List<Box> newBoxes)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, living.Id);
            pkg.Parameter1 = living.Id;
            //pkg.Parameter2 = -1;
            pkg.WriteByte((byte)eTankCmdType.TURN);
            pkg.WriteInt((int)(game.Wind * 10));
            pkg.WriteBoolean(false);
            pkg.WriteByte(0);
            pkg.WriteByte(0);
            pkg.WriteByte(0);
            pkg.WriteBoolean(living.IsHide);

            //turnTime
           // TimeType
            pkg.WriteInt(getTurnTime(TimeType));
            pkg.WriteInt(newBoxes.Count);
            foreach (Box box in newBoxes)
            {
                pkg.WriteInt(box.Id);
                pkg.WriteInt(box.X);
                pkg.WriteInt(box.Y);
                pkg.WriteInt(9);
               // pkg.WriteBoolean(false);
            }

            List<Player> list = game.GetAllFightPlayers();
            pkg.WriteInt(list.Count);
            foreach (Player p in list)
            {
                pkg.WriteInt(p.Id);
                pkg.WriteBoolean(p.IsLiving);
                pkg.WriteInt(p.X);
                pkg.WriteInt(p.Y);
                pkg.WriteInt(p.Blood);
                pkg.WriteBoolean(p.IsNoHole);
                pkg.WriteInt(p.Energy);
                pkg.WriteInt(p.Dander);
                pkg.WriteInt(p.ShootCount);
            }
            pkg.WriteInt(game.TurnIndex);

            SendToAll(pkg);
        }

        internal void SendLivingUpdateDirection(Living living)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.Parameter1 = living.Id;
            pkg.WriteByte((byte)eTankCmdType.DIRECTION);
            pkg.WriteInt(living.Direction);

            SendToAll(pkg);
        }

        internal void SendLivingUpdateAngryState(Living living)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.Parameter1 = living.Id;
            pkg.WriteByte((byte)eTankCmdType.LIVING_STATE);
            pkg.WriteInt(living.State);

            SendToAll(pkg);
        }

        internal void SendEquipEffect(Living player, string buffer)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CHAT);//, player.PlayerDetail.PlayerCharacter.ID);
            // pkg.ClientID = (player.PlayerDetail.PlayerCharacter.ID);
            pkg.WriteInt(0);
            //pkg.WriteBoolean(false);
            //pkg.WriteString(player.PlayerDetail.PlayerCharacter.NickName);
            pkg.WriteString(buffer);

            SendToAll(pkg);
        }

        internal void SendMessage(IGamePlayer player, string msg, string msg1, int type)
        {
            if (msg != null)
            {
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CHAT);
                pkg.WriteInt(type);
                pkg.WriteString(msg);
                player.SendTCP(pkg);
            }
            if (msg1 != null)
            {
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CHAT);
                pkg.WriteInt(type);
                pkg.WriteString(msg1);
                SendToAll(pkg, player);
            }
        }

        internal void SendPlayerPicture(Living living, int type, bool state)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.Parameter1 = living.Id;
            pkg.WriteByte((byte)eTankCmdType.SEND_PICTURE);
            pkg.WriteInt(type);
            pkg.WriteBoolean(state);
            SendToAll(pkg);
            //(living as Player).PlayerDetail.SendTCP(pkg);

        }

        //internal void SendInsufficientMoney(Player player, int type)
        //{
        //    GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
        //    pkg.WriteByte((byte)eTankCmdType.PAYMENT_TAKE_CARD);
        //    pkg.WriteByte((byte)type);
        //    pkg.WriteBoolean(false);
        //    player.PlayerDetail.SendTCP(pkg);
        //}
        public static int serverId=int.Parse(ConfigurationSettings.AppSettings["ServerID"]);
        internal void SendPlayerRemove(Player player)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_PLAYER_EXIT, player.PlayerDetail.PlayerCharacter.ID);
            pkg.WriteInt(serverId);
            pkg.WriteInt(4);
            SendToAll(pkg, player.PlayerDetail);

        }


        internal void SendAttackEffect(Living player, int type)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.Parameter1 = player.Id;
            pkg.WriteByte((byte)eTankCmdType.ATTACKEFFECT);
            pkg.WriteBoolean(true);
            pkg.WriteInt(type);
            SendToAll(pkg);

        }

        internal void SendSyncLifeTime()
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD);
            pkg.Parameter2 = -1;
            pkg.WriteByte((byte)eTankCmdType.GAME_TIME);
            pkg.WriteInt(m_lifeTime);
            SendToAll(pkg);

        }

        #endregion

        #region Events

        public event GameEventHandle GameOverred;
        public event GameEventHandle BeginNewTurn;

        protected void OnGameOverred()
        {
            if (GameOverred != null) GameOverred(this);
        }

        protected void OnBeginNewTurn()
        {
            if (BeginNewTurn != null) BeginNewTurn(this);
        }

        public delegate void GameOverLogEventHandle(int roomId, eRoomType roomType, eGameType fightType, int changeTeam, DateTime playBegin, DateTime playEnd, int userCount, int mapId, string teamA, string teamB, string playResult, int winTeam, string BossWar);
        public event GameOverLogEventHandle GameOverLog;
        public void OnGameOverLog(int _roomId, eRoomType _roomType, eGameType _fightType, int _changeTeam, DateTime _playBegin, DateTime _playEnd, int _userCount, int _mapId, string _teamA, string _teamB, string _playResult, int _winTeam, string BossWar)
        {
            if (GameOverLog != null)
            {
                GameOverLog(_roomId, _roomType, _fightType, _changeTeam, _playBegin, _playEnd, _userCount, _mapId, _teamA, _teamB, _playResult, _winTeam, BossWarField);
            }
        }

        public delegate void GameNpcDieEventHandle(int NpcId);
        /// <summary>
        /// NPC被打死
        /// </summary>
        public event GameNpcDieEventHandle GameNpcDie;
        public void OnGameNpcDie(int Id)
        {
            if (GameNpcDie != null)
            {
                GameNpcDie(Id);
            }
        }
        #endregion
        public override string ToString()
        {
            return string.Format("Id:{0},player:{1},state:{2},current:{3},turnIndex:{4},actions:{5}", Id, PlayerCount, GameState, CurrentLiving, m_turnIndex, m_actions.Count);
        }
    }
}
