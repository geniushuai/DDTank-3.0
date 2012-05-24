using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Base.Packets;
using Game.Server;
using Game.Server.Statics;
using Game.Server.Games;
using Game.Server.Effects;
using Game.Server.Managers;
using Game.Server.Phy.Object;
using System.Collections;
using Phy.Actions;
using Phy.Maps;
using Game.Server.Packets;
using Bussiness.Managers;
using Bussiness;
using Game.Server.Spells;
using Phy.Maths;

namespace Phy.Object
{
    public class Player : Physics
    {
        private GamePlayer m_player;
        private BaseGame m_game;
        private int m_team;
        private ItemInfo m_weapon;
        private int m_mainBallId;
        private int m_spBallId;

        private int m_maxBlood;
        private int m_blood;
        private bool m_isCaptain;

        private int m_direction;
        private int m_delay;
        private int m_energy;
        private int m_dander;

        private bool m_isAttacking;
        private bool m_isActive;

        private EffectList m_effectList;

        private bool m_isFrost;
        private bool m_isHide;
        private bool m_isNoHole;
        private int m_shootCount;
        private int m_ballCount;

        private BallInfo m_currentBall;
                
        public bool NoHoleTurn;
        

        public int TurnNum;
        public int TotalHurt;
        public int TotalKill;
        public int TotalHitTargetCount;
        public int TotalShootCount;

        public int GainGP;
        public int GainOffer;

        public bool CanTakeOut;


        public int LoadingProcess;
        public bool WannaLeader;

        public float CurrentDamagePlus;
        public float CurrentShootMinus;
        public bool IgnoreArmor;
        public bool ControlBall;
        public Point TargetPoint;

        public bool CurrentIsHitTarget;
        public bool HasTakeCard;

        

        public Player(GamePlayer player,BaseGame game,int team)
            : base(player.PlayerId)
        {
            m_rect = new Rectangle(-15, -20, 30, 30);
            m_player = player;
            m_game = game;
            m_team = team;
            m_isCaptain = false;
            m_effectList = new EffectList(this);
            m_isActive = true;

            m_weapon = m_player.CurrentWeapon();
            m_mainBallId = m_weapon.Template.Property1;
            m_spBallId = m_weapon.Template.Property2;
            m_direction = 1;

            LoadingProcess = 0;

            Reset();
        }

        public void Reset()
        {
            m_maxBlood = (int)((950 + m_player.PlayerCharacter.Grade * 50) * m_player.BaseBlood);
            if (m_isCaptain)
                m_maxBlood *= 2;
            m_blood = m_maxBlood;
            
           
            
            m_dander = 0;
            m_delay = (int)(1000 * m_player.BaseAgility);
            m_energy = 240;
            m_isCaptain = false;

            m_isFrost = false;
            m_isHide = false;
            m_isNoHole = false;

            m_isLiving = true;
            m_currentBall = BallMgr.FindBall(m_mainBallId);

            m_shootCount = 1;
            m_ballCount = 1;

            TurnNum = 0;
            TotalHurt = 0;
            TotalKill = 0;
            TotalShootCount = 0;
            TotalHitTargetCount = 0;

            GainGP = 0;
            GainOffer = 0;
        }

        public int Team
        {
            get { return m_team; }
        }

        public GamePlayer PlayerDetail
        {
            get {   return m_player; }
        }

        public BaseGame Game
        {
            get { return m_game; }
        }

        public int Direction
        {
            get { return m_direction; }
            set { m_direction = value; }
        }

        public ItemInfo Weapon
        {
            get { return m_weapon; }
        }

        public bool IsAttacking
        {
            get { return m_isAttacking; }
        }

        public bool IsActive
        {
            get { return m_isActive; }
        }


        #region Captain/SetCaptain

        public bool IsCaptain
        {
            get {   return m_isCaptain; }
        }

        public void SetCaptain(bool isCaptain)
        {
            if(m_isCaptain == isCaptain) return;
            m_isCaptain = isCaptain;

            Reset();
        }

        #endregion

        #region Blood/Dander/Delay/Energy   TakeDamage
        public int Blood
        {
            get { return m_blood; }
        }

        public int Dander
        {
            get { return m_dander; }
        }

        public int Delay
        {
            get { return m_delay; }
            set { m_delay += value; }
        }

        public int Energy
        {
            get { return m_energy; }
            set { m_energy = value; }
        }

        public void AddDelay(int value)
        {
            m_delay += value;
        }


        public void AddBlood(int value,bool sendToClient)
        {
            m_blood += value;
            if (m_blood > m_maxBlood)
            {
                m_blood = m_maxBlood;
            }
            if (sendToClient)
            {
                GSPacketIn pkg = m_player.Out.SendGameUpdateHealth(this, 0);
                m_game.SendToAll(pkg, m_player);
            }
        }

        public void AddDander(int value,bool sendToClient)
        {
            if (IsLiving)
            {
                SetDander(m_dander + value, sendToClient);
            }
        }

        public void SetDander(int value, bool sendToClient)
        {
            m_dander = Math.Min(value, 200);
            if (sendToClient)
            {
                GSPacketIn pkg = m_player.Out.SendGameUpdateDander(this);
                m_game.SendToAll(pkg, m_player);
            }
        }

        
        public bool ReduceEnergy(int value)
        {
            if (value > m_energy)
                return false;

            m_energy -= value;
            return true;
        }

        public bool TakeDamage(Player source, int damageAmount, int criticalAmount, bool sendToClient)
        {
            m_blood -= (damageAmount + criticalAmount);
            m_game.CurrentTurnTotalDamage += (damageAmount + criticalAmount);

            if (sendToClient)
            {
                GSPacketIn pkg = m_player.Out.SendGameUpdateHealth(this, 1);
                m_game.SendToAll(pkg, m_player);
            }

            if (m_blood < 0)
            {
                Die();
            }
            if (IsLiving)
            {
                AddDander((damageAmount * 2 / 5 + 5) / 2, sendToClient);
            }

            return true;
        }
        #endregion

        #region States/Effects/Fight Properties

        public BallInfo CurrentBall
        {
            get { return m_currentBall; }
        }

        public bool IsFrost
        {
            get {   return m_isFrost;   }
            set
            {
                if (m_isFrost != value)
                {
                    m_isFrost = value;
                    GSPacketIn pkg = m_player.Out.SendGameUpdateFrozenState(this);
                    m_game.SendToAll(pkg, m_player);
                }
            }
        }

        public bool IsNoHole
        {
            get {   return m_isNoHole;  }
            set
            {
                if (m_isNoHole != value)
                {
                    m_isNoHole = value;
                    GSPacketIn pkg = m_player.Out.SendGameUpdateNoHoleState(this);
                    m_game.SendToAll(pkg, m_player);
                }
            }
        }

        public bool IsHide
        {
            get{    return m_isHide;    }
            set
            {
                if (m_isHide != value)
                {
                    m_isHide = value;
                    GSPacketIn pkg = m_player.Out.SendGameUpdateHideState(this);
                    m_game.SendToAll(pkg, m_player);
                }
            }
        }

        public EffectList EffectList
        {
            get { return m_effectList; }
        }

        public int ShootCount
        {
            get { return m_shootCount; }
            set
            {
                if (m_shootCount != value)
                {
                    m_shootCount = value;
                    GSPacketIn pkg = m_player.Out.SendGameUpdateShootCount(this);
                    m_game.SendToAll(pkg, m_player);
                }
            }
        }

        public int BallCount
        {
            get { return m_ballCount; }
            set
            {
                if (m_ballCount != value)
                {
                    m_ballCount = value;
                    GSPacketIn pkg = m_player.Out.SendGameUpdateShootCount(this);
                    m_game.SendToAll(pkg, m_player);
                }
            }
        }

        public bool IsSpecialSkill
        {
            get { return m_currentBall.ID == m_spBallId; }
        }

        public void UseSpecialSkill()
        {
            if (m_dander >= 200)
            {
                SetBall(m_spBallId);
                SetDander(0, true);
            }
        }

        
        public void SetBall(int ballId)
        {
            if (ballId != m_currentBall.ID)
            {
                m_currentBall = BallMgr.FindBall(ballId);
                GSPacketIn pkg = m_player.Out.SendGameUpdateBall(this);
                m_game.SendToAll(pkg, m_player);

                BallCount = m_currentBall.Amount;
            }
        }

        #endregion

        #region Override Method[Moving/Die/SetXY]

        public override void StartMoving()
        {
            if (m_map != null)
            {
                Point p = m_map.FindYLineNotEmptyPoint(m_x, m_y); ;
                if (p.IsEmpty)
                {
                    m_y = m_map.Ground.Height;
                }
                else
                {
                    m_x = p.X;
                    m_y = p.Y;
                }

                if (p.IsEmpty)
                {
                    Die();
                }
            }
        }

        public void StartGhostMoving()
        {
            if (TargetPoint.IsEmpty)
                return;

            Point pv = new Point(TargetPoint.X - X, TargetPoint.Y - Y);
            Point target = TargetPoint;
            if (pv.Length() > 160)
            {
                pv.Normalize(160);
            }

            m_game.AddAction(new GhostMoveAction(this,new Point(X+pv.X, Y + pv.Y)));
        }

        public override void SetXY(int x, int y)
        {
            if (m_x == x && m_y == y)
                return;
            
            m_x = x;
            m_y = y;

            if (IsLiving)
            {
                m_energy -= Math.Abs(m_x - x);
                if (m_energy < -20)
                {
                    StatMgr.LogErrorPlayer(m_player.PlayerCharacter.ID, m_player.PlayerCharacter.UserName, m_player.PlayerCharacter.NickName, ItemRemoveType.MoveError, m_x.ToString() + " to " + x.ToString() + ",MapID:" + m_map.Info.ID);
                    GameServer.log.Error("move is error,player state: move energy:" + m_energy);
                }
            }
            else
            {
                Rectangle rect = m_rect;
                rect.Offset(m_x, m_y);

                Physics[] phys = m_map.FindPhysicalObjects(rect, this);
                foreach (Physics p in phys)
                {
                    if (p is Box)
                    {
                        Box b = p as Box;
                        PickBox(b, true);
                        //立刻开箱子给客户端
                        OpenBox(b.Id);
                    }
                }
            }
        }

        public override void Die()
        {
            if (IsLiving)
            {
                m_y -= 70;
                m_isLiving = false;

                if (m_isAttacking)
                {
                    StopAttacking();
                    m_game.CheckState(0);
                }
                OnDied();
            }
        }
        #endregion

        #region Boxes/OpenBox

        private ArrayList m_tempBoxes = new ArrayList();

        public void PickBox(Box box,bool sendToClient)
        {
            box.UserID = Id;
            box.Die();

            m_tempBoxes.Add(box);

            if (sendToClient)
            {
                GSPacketIn pkg = m_player.Out.SendGamePickBox(this, box.Id, box.Items.Type, "");
                m_game.SendToAll(pkg);
            }
        }

        public void OpenBox(int boxId)
        {
            Box box = null;
            foreach (Box temp in m_tempBoxes)
            {
                if (temp.Id == boxId && temp.Items != null)
                {
                    box = temp;
                    break;
                }
            }

            if (box != null)
            {
                ItemTemplateInfo temp = ItemMgr.GetSingleGoods(box.Items.GoodsID);
                if (temp != null)
                {
                    if (temp.CategoryID == 10) //如果是道具
                    {
                        if (m_player.PropInventory.AddItemTemplate(temp) == null)
                        {
                            m_player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("Game.Server.SceneGames.TankHandle.PropFull"));
                        }
                    }
                    else
                    {
                        m_player.TempInventory.AddItemTemplate(temp, box.Items);
                    }
                }
                m_tempBoxes.Remove(box);
            }
        }

        #endregion

        #region StartGame/NextTurn/Shoot/UseItem/Skip/DeadLink

        public void StartGame()
        {

        }

        public void BeginNextTurn()
        {
            if (CurrentIsHitTarget == true)
                TotalHitTargetCount++;
            CurrentIsHitTarget = false;

            m_energy = 240;
            m_shootCount = 1;
            m_ballCount = 1;
            m_flyCoolDown--;
            
            CurrentDamagePlus = 1;
            CurrentShootMinus = 1;
            if (m_currentBall.ID != m_mainBallId)
            {
                m_currentBall = BallMgr.FindBall(m_mainBallId);
            }

            if (IsLiving == false)
            {
                StartGhostMoving();
                TargetPoint = Point.Empty;
            }
        }

        public void StartAttacking()
        {
            if (m_isAttacking == false)
            {
                m_isAttacking = true;

                AddDelay(1600 - m_player.PlayerCharacter.Agility / 2);
                OnStartAttacking();
            }
        }

        public void PrepareShoot(byte speedTime)
        {
            int turnWaitTime = m_game.GetTurnWaitTime();
            int time = speedTime > turnWaitTime ? turnWaitTime : speedTime;
            AddDelay(time * 20);
            TotalShootCount++;
        }

        public void Shoot(int x, int y, int force, int angle)
        {
            if (m_shootCount > 0)
            {
                m_shootCount--;

                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, Id);

                pkg.WriteByte((byte)TankCmdType.FIRE);
                pkg.WriteInt(BallCount);

                Tile shape = BallMgr.FindTile(m_currentBall.ID);
                BombType ballType = BallMgr.GetBallType(m_currentBall.ID);

                float lifeTime = 0;
                for (int i = 0; i < BallCount; i++)
                {
                    double reforce = 1;
                    int reangle = 0;
                    if (i == 1)
                    {
                        reforce = 0.9;
                        reangle = -5;
                    }
                    else if (i == 2)
                    {
                        reforce = 1.1;
                        reangle = 5;
                    }

                    int vx = (int)(force * reforce * Math.Cos((double)(angle + reangle) / 180 * Math.PI));
                    int vy = (int)(force * reforce * Math.Sin((double)(angle + reangle) / 180 * Math.PI));

                    SimpleBomb bomb = new SimpleBomb(m_game.PhysicalId++, ballType, this, m_game, m_currentBall, shape, ControlBall);

                    bomb.SetXY(x, y);
                    bomb.setSpeedXY(vx, vy);
                    m_game.Map.AddPhysical(bomb);
                    bomb.StartMoving();

                    pkg.WriteInt(bomb.Id);
                    pkg.WriteInt(x);
                    pkg.WriteInt(y);
                    pkg.WriteInt(vx);
                    pkg.WriteInt(vy);
                    pkg.WriteInt(bomb.Actions.Count);
                    foreach (BombAction action in bomb.Actions)
                    {
                        pkg.WriteInt(action.TimeInt);
                        pkg.WriteInt(action.Type);
                        pkg.WriteInt(action.Param1);
                        pkg.WriteInt(action.Param2);
                        pkg.WriteInt(action.Param3);
                        pkg.WriteInt(action.Param4);
                    }

                    lifeTime = Math.Max(lifeTime, bomb.LifeTime);
                }

                m_game.SendToAll(pkg);

                if (m_shootCount <= 0 || IsLiving == false)
                {
                    StopAttacking();

                    AddDelay(m_currentBall.Delay);
                    AddDander(20, true);

                    m_game.WaitTime((int)(lifeTime * 1000));
                }
            }
        }

        public void StopAttacking()
        {
            if (m_isAttacking == true)
            {
                m_isAttacking = false;
                OnStopAttacking();
            }
        }

        public void Skip(int spendTime)
        {
            StopAttacking();

            AddDelay(100);
            AddDander(40, true);

            m_game.CheckState(0);
        }

        public bool UseItem(ItemTemplateInfo item)
        {
            if (m_energy > item.Property4)
            {
                m_energy -= item.Property4;

                m_delay += item.Property5;

                GSPacketIn pkg = m_player.Out.SendPropUseRespone(this, -2, -2, item.TemplateID);
                m_game.SendToAll(pkg, m_player);

                SpellMgr.ExecuteSpell(m_game, m_game.CurrentPlayer, item);

                m_player.QuestInventory.CheckUseItem(item.TemplateID);

                if (item.Property6 == 1 && IsAttacking)
                {
                    StopAttacking();

                    m_game.CheckState(0);
                }

                return true;
            }
            return false;
        }

        private static readonly int FLY_COOLDOWN = 2;

        private static readonly int CARRY_TEMPLATE_ID = 10016;

        private int m_flyCoolDown = 0;

        public void UseFlySkill()
        {
            if (m_flyCoolDown <= 0)
            {
                m_flyCoolDown = FLY_COOLDOWN;
                GSPacketIn pkg = m_player.Out.SendPropUseRespone(this, -2, -2, CARRY_TEMPLATE_ID);
                m_game.SendToAll(pkg, m_player);

                SetBall(3);
            }
        }

        public void DeadLink()
        {
            m_isActive = false;
            if (IsLiving)
            {
                Die();
            }
        }

        public bool CheckShootPoint(int x, int y)
        {
            if (Math.Abs(X - x) > 100)
            {
                string username = m_player.PlayerCharacter.UserName;
                string nickname = m_player.PlayerCharacter.NickName;

                StatMgr.LogErrorPlayer(Id, username, nickname, ItemRemoveType.FireError, string.Format("error shoot pos: {0} -> {1} mapId:{2}", X, x, m_game.Map.Info.ID));
                m_player.Client.Disconnect();
                return false;
            }
            return true;
        }

        #endregion
        
        #region Events

        public event PlayerEventHandle BeginAttacking;
        public event PlayerEventHandle EndAttacking;
        public event PlayerEventHandle Died;

        private void OnStartAttacking()
        {
            if (BeginAttacking != null) BeginAttacking(this);
        }

        private void OnStopAttacking()
        {
            if (EndAttacking != null)   EndAttacking(this);
        }

        private void OnDied()
        {
            if (Died != null) Died(this);
        }

        #endregion
    }

    public delegate void PlayerEventHandle(Player player);
}
