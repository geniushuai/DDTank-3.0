using System;
using System.Drawing;
using SqlDataProvider.Data;
using Game.Base.Packets;
using System.Collections;
using Bussiness.Managers;
using Game.Logic.Effects;
using Game.Logic.Actions;
using Game.Logic.Phy.Maths;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Actions;
using Game.Logic.Spells;
using System.Collections.Generic;
using Bussiness;


namespace Game.Logic.Phy.Object
{
    public class Player : TurnedLiving
    {
        private IGamePlayer m_player;

        private ItemTemplateInfo m_weapon;
        private int m_mainBallId;
        private int m_spBallId;
        private BallInfo m_currentBall;
        private int m_energy;
        //private int m_dander;
        private bool m_isActive;

        public Point TargetPoint;

        public int GainGP;
        public int GainOffer;

        //public double GPRate;
        //public double OfferRate;

        public bool LockDirection = false;
        public int TotalCure;

        private bool m_canGetProp;

        //副本总伤害
        public int TotalAllHurt;
        public int TotalAllHitTargetCount;
        public int TotalAllShootCount;
        public int TotalAllKill;
        public int TotalAllExperience;
        public int TotalAllScore;
        public int TotalAllCure;

        public int CanTakeOut;
        public bool FinishTakeCard;
        public bool HasPaymentTakeCard;
        public int BossCardCount;

        public bool Ready;

        public Player(IGamePlayer player, int id, BaseGame game, int team)
            : base(id, game, team, "", "", 1000, 0, 1)                   //TODO   lastPatemer    direction
        {
            m_rect = new Rectangle(-15, -20, 30, 30);
            m_player = player;
            m_player.GamePlayerId = id;
            m_isActive = true;
            m_canGetProp = true;
            Grade = player.PlayerCharacter.Grade;

            TotalAllHurt = 0;
            TotalAllHitTargetCount = 0;
            TotalAllShootCount = 0;
            TotalAllKill = 0;
            TotalAllExperience = 0;
            TotalAllScore = 0;
            TotalAllCure = 0;
            m_weapon = m_player.MainWeapon;
            if (m_weapon != null)
            {
                var ballConfig = BallConfigMgr.FindBall(m_weapon.TemplateID);
                m_mainBallId = ballConfig.Common;
                m_spBallId = ballConfig.Special;
                //m_mainBallId = m_weapon.Property1;
                //m_spBallId = m_weapon.Property2;
            }
            m_loadingProcess = 0;

            InitBuffer(m_player.EquipEffect);
            m_energy = (m_player.PlayerCharacter.Agility / 30 + 240);
            m_maxBlood = (int)((950 + m_player.PlayerCharacter.Grade * 50 + LevelPlusBlood + m_player.PlayerCharacter.Defence / 10) * m_player.GetBaseBlood());
        }

        public override void Reset()
        {
            m_maxBlood = (int)((950 + m_player.PlayerCharacter.Grade * 50 + LevelPlusBlood + m_player.PlayerCharacter.Defence / 10) * m_player.GetBaseBlood());
            if (m_game.RoomType == eRoomType.Treasure || m_game.RoomType == eRoomType.Boss)
            {
                m_game.Cards = new int[21];
            }
            else
            {
                m_game.Cards = new int[8];
            }

            Dander = 0;
            m_energy = (m_player.PlayerCharacter.Agility / 30 + 240);
            IsLiving = true;
            FinishTakeCard = false;
            m_weapon = m_player.MainWeapon;
            //m_mainBallId = m_weapon.Property1;
            //m_spBallId = m_weapon.Property2;
            var ballConfig = BallConfigMgr.FindBall(m_weapon.TemplateID);
            m_mainBallId = ballConfig.Common;
            m_spBallId = ballConfig.Special;
            BaseDamage = m_player.GetBaseAttack();
            BaseGuard = m_player.GetBaseDefence();

            Attack = m_player.PlayerCharacter.Attack;
            Defence = m_player.PlayerCharacter.Defence;
            Agility = m_player.PlayerCharacter.Agility;
            Lucky = m_player.PlayerCharacter.Luck;

            m_currentBall = BallMgr.FindBall(m_mainBallId);
            m_shootCount = 1;
            m_ballCount = 1;

            CurrentIsHitTarget = false;

            TotalCure = 0;
            TotalHitTargetCount = 0;
            TotalHurt = 0;
            TotalKill = 0;
            TotalShootCount = 0;
            LockDirection = false;
            GainGP = 0;
            GainOffer = 0;
            Ready = false;
            PlayerDetail.ClearTempBag();

            LoadingProcess = 0;

            base.Reset();
        }

        public IGamePlayer PlayerDetail
        {
            get { return m_player; }
        }

        public ItemTemplateInfo Weapon
        {
            get { return m_weapon; }
        }

        public bool IsActive
        {
            get { return m_isActive; }
        }

        public bool CanGetProp
        {
            get { return m_canGetProp; }
            set
            {
                if (m_canGetProp != value)
                {
                    m_canGetProp = value;
                }
            }
        }
        #region Init Buffers

        public void InitBuffer(List<int> equpedEffect)
        {
            for (int i = 0; i < equpedEffect.Count; i++)
            {

                ItemTemplateInfo item = ItemMgr.FindItemTemplate(equpedEffect[i]);

                switch (item.Property3)
                {

                    case 1:
                        new AddAttackEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 2:
                        new AddDefenceEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 3:
                        new AddAgilityEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 4:
                        new AddLuckyEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 5:
                        new AddDamageEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 6:
                        new ReduceDamageEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 7:
                        new AddBloodEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 8:
                        new FatalEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 9:
                        new IceFronzeEquipEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 10:
                        new NoHoleEquipEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 11:
                        new AtomBombEquipEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 12:
                        new ArmorPiercerEquipEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 13:
                        new AvoidDamageEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 14:
                        new MakeCriticalEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 15:
                        new AssimilateDamageEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 16:
                        new AssimilateBloodEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 17:
                        new SealEquipEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 18:
                        new AddTurnEquipEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 19:
                        new AddDanderEquipEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 20:
                        new ReflexDamageEquipEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 21:
                        new ReduceStrengthEquipEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 22:
                        new ContinueReduceBloodEquipEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 23:
                        new LockDirectionEquipEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 24:
                        new AddBombEquipEffect(item.Property4, item.Property5).Start(this);
                        break;
                    case 25:
                        new ContinueReduceDamageEquipEffect(item.Property4, item.Property5).Start(this);
                        break;


                }
            }
        }

        #endregion

        #region LoadingProcess

        private int m_loadingProcess;

        public int LoadingProcess
        {
            get { return m_loadingProcess; }
            set
            {
                if (m_loadingProcess != value)
                {
                    m_loadingProcess = value;
                    if (m_loadingProcess >= 100)
                        OnLoadingCompleted();
                }
            }
        }

        #endregion

        #region Blood/Dander/Delay/Energy   TakeDamage

        public int LevelPlusBlood
        {
            get
            {
                int plusblood = 0;
                for (int i = 10; i <= 60; )
                {
                    if ((PlayerDetail.PlayerCharacter.Grade - i) > 0)
                    {
                        plusblood += (PlayerDetail.PlayerCharacter.Grade - i) * (i + 20);
                    }
                    i += 10;
                }
                return plusblood;
            }
        }

        public int Energy
        {
            get { return m_energy; }
            set { m_energy = value; }
        }

        public bool ReduceEnergy(int value)
        {
            if (value > m_energy)
                return false;

            m_energy -= value;
            return true;
        }

        public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
        {
            bool result = false;

            //玩家炸自己炸不死
            if ((source == this || source.Team == this.Team) && damageAmount + criticalAmount >= m_blood)
            {
                damageAmount = m_blood - 1;
                criticalAmount = 0;
            }

            result = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);

            if (IsLiving)
            {
                AddDander((damageAmount * 2 / 5 + 5) / 2);
            }
            //Console.WriteLine("最后伤血{0}", damageAmount);
            return result;
        }
        #endregion

        #region States/Effects/Fight Properties/Buffers

        private int m_shootCount;
        private int m_ballCount;

        public BallInfo CurrentBall
        {
            get { return m_currentBall; }
        }

        public bool IsSpecialSkill
        {
            get { return m_currentBall.ID == m_spBallId; }
        }

        public void UseSpecialSkill()
        {
            if (Dander >= 200)
            {
                SetBall(m_spBallId, true);
                SetDander(0);
            }
        }

        public int ShootCount
        {
            get { return m_shootCount; }
            set
            {
                if (m_shootCount != value)
                {
                    m_shootCount = value;
                    m_game.SendGameUpdateShootCount(this);
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
                }
            }
        }

        public void SetBall(int ballId)
        {
            SetBall(ballId, false);
        }

        public void SetBall(int ballId, bool special)
        {
            if (ballId != m_currentBall.ID)
            {
               if(BallMgr.FindBall(ballId)!=null) m_currentBall = BallMgr.FindBall(ballId);
                BallCount = m_currentBall.Amount;
                if (!special)
                    ShootCount = 1;
                m_game.SendGameUpdateBall(this, special);
            }
        }

        public void SetCurrentWeapon(ItemTemplateInfo item)
        {
            m_weapon = item;
            //m_mainBallId = m_weapon.Property1;
            //m_spBallId = m_weapon.Property2;
            var ballConfig = BallConfigMgr.FindBall(m_weapon.TemplateID);
            m_mainBallId = ballConfig.Common;
            m_spBallId = ballConfig.Special;

            SetBall(m_mainBallId);
        }

        #endregion

        #region StartMoving/StartGhostMoving/SetXY/Die

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
                //Console.WriteLine("p.x : {0}, p.y : {1}, playerId {2}", p.X, p.Y, Id);
                if (p.IsEmpty)
                {
                    //Console.WriteLine("p is empty, playerId : {0}", Id);
                    m_syncAtTime = false;
                    Die();
                }
            }

        }

        public override void StartMoving(int delay, int speed)
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
                base.StartMoving(delay, speed);
                if (p.IsEmpty)
                {
                    m_syncAtTime = false;
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

            m_game.AddAction(new GhostMoveAction(this, new Point(X + pv.X, Y + pv.Y)));
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
                        PickBox(b);
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

                base.Die();
            }
        }
        #endregion

        #region Boxes/OpenBox

        private ArrayList m_tempBoxes = new ArrayList();

        override public void PickBox(Box box)
        {
            m_tempBoxes.Add(box);

            base.PickBox(box);
        }

        public void OpenBox(int boxId)
        {
            Box box = null;
            foreach (Box temp in m_tempBoxes)
            {
                if (temp.Id == boxId)
                {
                    box = temp;
                    break;
                }
            }
            if (box != null && box.Item != null)
            {
                ItemInfo item = box.Item;
                switch (item.TemplateID)
                {
                    case -100:
                        m_player.AddGold(item.Count);
                        break;
                    case -200:
                        m_player.AddMoney(item.Count);
                        m_player.LogAddMoney(AddMoneyType.Box, AddMoneyType.Box_Open, m_player.PlayerCharacter.ID, item.Count, m_player.PlayerCharacter.Money);
                        break;
                    case -300:
                        m_player.AddGiftToken(item.Count);
                        break;
                    default:
                        if (item.Template.CategoryID == 10)
                        {
                            //添加物品到道具栏
                            m_player.AddTemplate(item, eBageType.FightBag, item.Count);
                        }
                        else
                        {
                            m_player.AddTemplate(item, eBageType.TempBag, item.Count);
                        }
                        break;
                }

                m_tempBoxes.Remove(box);
            }
        }

        #endregion

        #region StartGame/NextTurn/Shoot/UseItem/Skip/DeadLink

        public override void PrepareNewTurn()
        {
            if (CurrentIsHitTarget == true)
            {
                TotalHitTargetCount++;
                //Console.WriteLine("TotalHitTargetCount + 1 ------>>>> p.TotalHitTargetCount : {0}", TotalHitTargetCount);
            }

            //Game.SendUpdateUiData(this, game);
            // BufferMgr.BufferList.Clear();
            m_energy = m_player.PlayerCharacter.Agility / 30 + 240;
            m_shootCount = 1;
            m_ballCount = 1;
            m_flyCoolDown--;
            m_secondWeapon--;
            //if(m_currentBall.ID!=PlayerDetail.MainWeapon)
            //SetCurrentWeapon(PlayerDetail.MainWeapon);
            SetCurrentWeapon(PlayerDetail.MainWeapon);
            if (m_currentBall.ID != m_mainBallId)
            {
                m_currentBall = BallMgr.FindBall(m_mainBallId);
            }

            if (IsLiving == false)
            {
                StartGhostMoving();
                TargetPoint = Point.Empty;
            }

            base.PrepareNewTurn();
        }

        public override void StartAttacking()
        {
            if (!IsAttacking)
            {
                AddDelay(1600 - 1200 * PlayerDetail.PlayerCharacter.Agility / (PlayerDetail.PlayerCharacter.Agility + 1200) + PlayerDetail.PlayerCharacter.Attack / 10);
                base.StartAttacking();
            }
        }

        public override void Skip(int spendTime)
        {
            if (IsAttacking)
            {
                base.Skip(spendTime);

                AddDelay(100);
                AddDander(40);
            }
        }

        public void PrepareShoot(byte speedTime)
        {
            int turnWaitTime = m_game.GetTurnWaitTime();
            int time = speedTime > turnWaitTime ? turnWaitTime : speedTime;
            AddDelay(time * 20);
            TotalShootCount++;
        }

        public bool Shoot(int x, int y, int force, int angle)
        {
            if (m_shootCount > 0)
            {
                EffectTrigger = false;
                OnPlayerShoot();
                if (EffectTrigger)
                {
                    
                    Game.SendMessage(PlayerDetail, LanguageMgr.GetTranslation("PlayerEquipEffect.Success"), LanguageMgr.GetTranslation("PlayerEquipEffect.Success1", PlayerDetail.PlayerCharacter.NickName), 3);
                }

                if (ShootImp(m_currentBall.ID, x, y, force, angle, m_ballCount))
                {
                    m_shootCount--;

                    if (m_shootCount <= 0 || IsLiving == false)
                    {
                        StopAttacking();

                        AddDelay(m_currentBall.Delay + m_weapon.Property8);
                        AddDander(20);
                        if (CanGetProp)
                        {
                            int gold = 0;
                            int money = 0;
                            int giftToken = 0;
                            int templateID = 0;
                            List<ItemInfo> infos = null;
                            if (DropInventory.FireDrop(m_game.RoomType, ref  infos))
                            {
                                if (infos != null)
                                {
                                    foreach (ItemInfo info in infos)
                                    {
                                        ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken);
                                        if (info != null)
                                        {
                                            templateID = info.TemplateID;
                                            PlayerDetail.AddTemplate(info, eBageType.FightBag, info.Count);
                                        }
                                    }
                                    PlayerDetail.AddGold(gold);
                                    PlayerDetail.AddMoney(money);
                                    PlayerDetail.LogAddMoney(AddMoneyType.Game, AddMoneyType.Game_Shoot, PlayerDetail.PlayerCharacter.ID, money, PlayerDetail.PlayerCharacter.Money);
                                    PlayerDetail.AddGiftToken(giftToken);
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public bool CanUseItem(ItemTemplateInfo item)
        {
            return m_energy >= item.Property4 && (IsAttacking || (IsLiving == false && Team == m_game.CurrentLiving.Team));
        }

        public bool UseItem(ItemTemplateInfo item)
        {
            if (CanUseItem(item))
            {
                m_energy -= item.Property4;

                m_delay += item.Property5;


                m_game.SendPlayerUseProp(this, -2, -2, item.TemplateID);

                SpellMgr.ExecuteSpell(m_game, m_game.CurrentLiving as Player, item);

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
                m_game.SendPlayerUseProp(this, -2, -2, CARRY_TEMPLATE_ID);

                SetBall(3);
            }
        }


        private static readonly int SECOND_WEAPON = 3;

        private int m_secondWeapon = 0;

        public void UseSecondWeapon()
        {
            if (m_secondWeapon <= 0)
            {
                if (PlayerDetail.SecondWeapon.Template.TemplateID == 17003 || PlayerDetail.SecondWeapon.Template.TemplateID == 17004)
                {
                    new AddDefenceEffect(PlayerDetail.SecondWeapon.Template.Property4 + PlayerDetail.SecondWeapon.StrengthenLevel * 2, 100).Start(this) ;
                }
                else
                {
                    m_secondWeapon = SECOND_WEAPON;
                    SetCurrentWeapon(PlayerDetail.SecondWeapon.Template);
                }
               // UseItem(PlayerDetail.SecondWeapon.Template);
                m_game.SendPlayerUseProp(this, -2, -2, m_weapon.TemplateID);
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

                m_player.Disconnect();
                return false;
            }
            return true;
        }

        #endregion

        #region Events

        public event PlayerEventHandle LoadingCompleted;

        protected void OnLoadingCompleted()
        {
            if (LoadingCompleted != null) LoadingCompleted(this);
        }

        public event PlayerEventHandle PlayerShoot;
        public void OnPlayerShoot()
        {
            if (PlayerShoot != null) PlayerShoot(this);
        }

        public override void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount)
        {
            base.OnAfterKillingLiving(target, damageAmount, criticalAmount);
            if (target is Player)//目标为玩家还是NPC
            {
                m_player.OnKillingLiving(m_game, 1, target.Id, target.IsLiving, damageAmount + criticalAmount);

            }
            else
            {
                int targetId = 0;
                if (target is SimpleBoss)
                {
                    SimpleBoss tempBoss = target as SimpleBoss;
                    targetId = tempBoss.NpcInfo.ID;
                }
                if (target is SimpleNpc)
                {
                    SimpleNpc tempNpc = target as SimpleNpc;
                    targetId = tempNpc.NpcInfo.ID;
                }
                m_player.OnKillingLiving(m_game, 2, targetId, target.IsLiving, damageAmount + criticalAmount);
            }


        }

        #endregion
    }

    public delegate void PlayerEventHandle(Player player);
}
