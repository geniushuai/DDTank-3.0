using System;
using System.Collections.Generic;
using System.Drawing;
using SqlDataProvider.Data;
using Game.Logic.Effects;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maths;
using Bussiness;

namespace Game.Logic.Phy.Object
{
    public class SimpleBomb : BombObject
    {
        private Living m_owner;

        private BaseGame m_game;

        protected Tile m_shape;

        protected int m_radius;

        protected double m_power;

        protected List<BombAction> m_actions;

        protected BombType m_type;

        protected bool m_controled;

        private float m_lifeTime;

        private BallInfo m_info;

        private bool m_bombed;

        private bool digMap;

        public bool DigMap
        {
            get
            {
                return digMap;
            }
        }

        public BallInfo BallInfo
        {
            get
            {
                return m_info;
            }
        }

        public SimpleBomb(int id, BombType type, Living owner, BaseGame game, BallInfo info, Tile shape, bool controled)
            : base(id, info.Mass, info.Weight, info.Wind, info.DragIndex)
        {
            m_owner = owner;
            m_game = game;
            m_info = info;

            m_shape = shape;
            m_type = type;
            m_power = info.Power;
            m_radius = info.Radii;
            m_controled = controled;
            m_bombed = false;

            m_lifeTime = 0;
            digMap = true;
        }

        public Living Owner
        {
            get { return m_owner; }
        }

        public List<BombAction> Actions
        {
            get { return m_actions; }
        }

        public float LifeTime
        {
            get { return m_lifeTime; }
        }

        public override void StartMoving()
        {
            base.StartMoving();
            m_actions = new List<BombAction>();
            int oldLifeTime = m_game.LifeTime;

            while (m_isMoving && m_isLiving)
            {
                m_lifeTime += 0.04F;
                // Console.WriteLine(string.Format("af:{0},wf:{1},gf:{2} vx: {3} vy:{4} x:{5},y:{6}", Arf, Wf, Gf, vX, vY, X, Y));
                Point pos = CompleteNextMovePoint(0.04F);
                MoveTo(pos.X, pos.Y);

                if (m_isLiving)
                {
                    if (Math.Round(m_lifeTime * 100) % 40 == 0 && pos.Y > 0)
                        m_game.AddTempPoint(pos.X, pos.Y);

                    if (m_controled && vY > 0)//瞄准
                    {
                        Living player = m_map.FindNearestEnemy(m_x, m_y, 150, m_owner);
                        Point v;
                        if (player != null)
                        {
                            if (player is SimpleBoss)
                            {
                                Rectangle dis = player.GetDirectDemageRect();
                                v = new Point(dis.X - m_x, dis.Y - m_y);
                            }
                            else
                            {
                                v = new Point(player.X - m_x, player.Y - m_y);
                            }
                            v = v.Normalize(1000);

                            setSpeedXY(v.X, v.Y);
                            //使炮弹不受重力和风力和空气阻力的影响
                            UpdateForceFactor(0, 0, 0);
                            m_controled = false;
                            m_actions.Add(new BombAction(m_lifeTime, ActionType.CHANGE_SPEED, v.X, v.Y, 0, 0));
                        }
                    }
                }

                if (m_bombed)
                {
                    m_bombed = false;
                    BombImp();
                }
            }
        }

        protected override void CollideObjects(Physics[] list)
        {
            foreach (Physics phy in list)
            {
                phy.CollidedByObject(this);
                m_actions.Add(new BombAction(m_lifeTime, ActionType.PICK, phy.Id, 0, 0, 0));
            }
        }

        protected override void CollideGround()
        {
            base.CollideGround();
            Bomb();
        }

        public void Bomb()
        {
            StopMoving();
            m_isLiving = false;
            m_bombed = true;
        }

        private void BombImp()
        {
            List<Living> playersAround = m_map.FindHitByHitPiont(GetCollidePoint(), m_radius);
            foreach (Living p in playersAround)
            {
                if (p.IsNoHole || p.NoHoleTurn)
                {
                    p.NoHoleTurn = true;
                    digMap = false;
                }
                p.SyncAtTime = false;
            }
            m_owner.SyncAtTime = false;
            try
            {

                //TrieuLSL DIG DIG DIG
                if (digMap)
                {
                    m_map.Dig(m_x, m_y, m_shape, null);
                }
                m_actions.Add(new BombAction(m_lifeTime, ActionType.BOMB, m_x, m_y, digMap ? 1 : 0, 0));

                switch (m_type)
                {
                    case BombType.FORZEN:
                        foreach (Living p in playersAround)
                        {
                            if (m_owner is SimpleBoss && new IceFronzeEffect(100).Start(p))
                            {
                                m_actions.Add(new BombAction(m_lifeTime, ActionType.FORZEN, p.Id, 0, 0, 0));
                            }
                            else
                            {
                                if (new IceFronzeEffect(2).Start(p))
                                {
                                    m_actions.Add(new BombAction(m_lifeTime, ActionType.FORZEN, p.Id, 0, 0, 0));
                                }
                                else
                                {
                                    m_actions.Add(new BombAction(m_lifeTime, ActionType.FORZEN, -1, 0, 0, 0));
                                    m_actions.Add(new BombAction(m_lifeTime, ActionType.UNANGLE, p.Id, 0, 0, 0));
                                }
                            }
                        }
                        break;
                    case BombType.TRANFORM:
                        //炸弹的飞行时间必须超过1
                        if (m_y > 10 && m_lifeTime > 0.04f)
                        {
                            //炸弹最后的落脚地点必须为空,否则向后退5像素
                            if (m_map.IsEmpty(m_x, m_y) == false)
                            {
                                PointF v = new PointF(-vX, -vY);
                                v = v.Normalize(5);
                                m_x -= (int)v.X;
                                m_y -= (int)v.Y;
                            }
                            m_owner.SetXY(m_x, m_y);
                            m_owner.StartMoving();
                            m_actions.Add(new BombAction(m_lifeTime, ActionType.TRANSLATE, m_x, m_y, 0, 0));
                            m_actions.Add(new BombAction(m_lifeTime, ActionType.START_MOVE, m_owner.Id, m_owner.X, m_owner.Y, m_owner.IsLiving ? 1 : 0));
                        }
                        break;
                    case BombType.CURE:
                        foreach (Living p in playersAround)
                        {
                            double plus = 0;
                            if (playersAround.Count > 1)
                                plus = 0.4;
                            else
                                plus = 1;
                            int blood = (int)(((Player)m_owner).PlayerDetail.SecondWeapon.Template.Property7 * Math.Pow(1.1, ((Player)m_owner).PlayerDetail.SecondWeapon.StrengthenLevel) * plus);
                            p.AddBlood(blood);
                            if (p is Player)
                            {
                                ((Player)p).TotalCure += blood;
                            }
                            m_actions.Add(new BombAction(m_lifeTime, ActionType.CURE, p.Id, p.Blood, blood, 0));
                            //Console.WriteLine("治疗{0},血量{1}", blood, p.Blood);
                        }
                        break;
                    default:
                        foreach (Living p in playersAround)
                        {
                            //Console.WriteLine("炸弹ID{0}", m_info.Name);
                            //判断npc之间的阵营
                            if (m_owner.IsFriendly(p) == true)
                            {
                                continue;
                            }

                            int damage = MakeDamage(p);
                            int critical = 0;
                            if (damage != 0)
                            {
                                critical = MakeCriticalDamage(p, damage);//暴击
                                m_owner.OnTakedDamage(m_owner, ref damage, ref damage);
                                if (p.TakeDamage(m_owner, ref damage, ref critical, "爆炸"))
                                {
                                    m_actions.Add(new BombAction(m_lifeTime, ActionType.KILL_PLAYER, p.Id, damage + critical, critical != 0 ? 2 : 1, p.Blood));
                                }
                                else
                                {
                                    m_actions.Add(new BombAction(m_lifeTime, ActionType.UNFORZEN, p.Id, 0, 0, 0));
                                }

                                if (p is Player)
                                {
                                    m_actions.Add(new BombAction(m_lifeTime, ActionType.DANDER, p.Id, ((Player)p).Dander, 0, 0));
                                    //(p as Player).StartMoving();//((int)((m_lifeTime + 1) * 1000), 12);
                                }
                                if (p is SimpleBoss)
                                {
                                    ((PVEGame)m_game).OnShooted();
                                }
                            }
                            else if (p is SimpleBoss)
                            {

                                m_actions.Add(new BombAction(m_lifeTime, ActionType.PLAYMOVIE, p.Id, 0, 0, 2));
                            }
                            m_owner.OnAfterKillingLiving(p, damage, critical);

                            if (p.IsLiving)
                            {
                               // Console.WriteLine("爆炸之前Y：{0}", p.Y);

                                p.StartMoving((int)((m_lifeTime + 1) * 1000), 12);
                              //  Console.WriteLine("爆炸之后Y：{0}", p.Y);
                                Console.WriteLine("爆炸还活着？"+p.IsLiving.ToString());
                                m_actions.Add(new BombAction(m_lifeTime, ActionType.START_MOVE, p.Id, p.X, p.Y, p.IsLiving ? 1 : 0));
                            }

                        }
                        break;
                }
                this.Die();
            }
            finally
            {
                m_owner.SyncAtTime = true;
                foreach (Living p in playersAround)
                {
                    p.SyncAtTime = true;
                }
            }
        }

        protected int MakeDamage(Living target)
        {
            double baseDamage = m_owner.BaseDamage;
            double baseGuard = target.BaseGuard;

            double defence = target.Defence;
            double attack = m_owner.Attack;
            if (m_owner.IgnoreArmor)
            {
                baseGuard = 0;
                defence = 0;
            }

            float damagePlus = m_owner.CurrentDamagePlus;
            float shootMinus = m_owner.CurrentShootMinus;

            //伤害={ 基础伤害*（1+攻击*0.1%）*[1-（基础护甲/200+防御*0.03%）] }*(1+道具攻击加成）*炸弹威力*连击系数
            // double damage = (baseDamage * ( 1 + attack * 0.001) * (1 - (baseGuard / 200 + defence * 0.003))) * (1 + damagePlus) * shootMinus ;

            double DR1 = 0.95 * (target.BaseGuard - 3 * m_owner.Grade) / (500 + target.BaseGuard - 3 * m_owner.Grade);//护甲提供伤害减免
            double DR2 = 0;
            if ((target.Defence - m_owner.Lucky) < 0)
            {
                DR2 = 0;
            }
            else
            {

                DR2 = 0.95 * (target.Defence - m_owner.Lucky) / (600 + target.Defence - m_owner.Lucky); //防御提供的伤害减免
            }
            //DR2 = DR2 < 0 ? 0 : DR2;

            double damage = (baseDamage * (1 + attack * 0.001) * (1 - (DR1 + DR2 - DR1 * DR2))) * damagePlus * shootMinus;

            Point p = new Point(X, Y);
            double distance = target.Distance(p);
            if (distance < m_radius)
            {
                damage = damage * (1 - distance / m_radius / 4);
                if (damage < 0)
                {
                    return 1;
                }
            }
            else
            {
                return 0;
            }
            return (int)damage;
        }

        protected int MakeCriticalDamage(Living target, int baseDamage)
        {
            double lucky = m_owner.Lucky;

            //Random rd = new Random();
            bool canHit = lucky * 75 / (800 + lucky) > ThreadSafeRandom.NextStatic(100);
            if (canHit)
            {
                return (int)((0.5 + lucky * 0.0003) * baseDamage);
            }
            else
            {
                return 0;
            }
        }

        protected override void FlyoutMap()
        {
            m_actions.Add(new BombAction(m_lifeTime, ActionType.FLY_OUT, 0, 0, 0, 0));
            base.FlyoutMap();
        }
    }
}
