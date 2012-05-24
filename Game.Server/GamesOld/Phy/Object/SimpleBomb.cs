using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phy.Maps;
using Phy.Object;
using Phy.Actions;
using System.Drawing;
using Phy.Maths;
using Game.Server.Effects;
using Game.Server.Games;
using SqlDataProvider.Data;

namespace Game.Server.Phy.Object
{
    public class SimpleBomb:BombObject
    {
        private Player m_owner;

        private BaseGame m_game;

        protected Tile m_shape;

        protected int m_radius;

        protected double m_power;

        protected List<BombAction> m_actions;

        protected BombType m_type;

        protected bool m_controled;

        private float m_lifeTime;

        private BallInfo m_info;
        

        public SimpleBomb(int id,BombType type, Player owner,BaseGame game,BallInfo info, Tile shape,bool controled)
            :base(id,info.Mass,info.Weight,info.Wind,info.DragIndex)
        {
            m_owner = owner;
            m_game = game;
            m_info = info;

            m_shape = shape;
            m_type = type;
            m_power = info.Power;
            m_radius = info.Radii;
            m_controled = controled;

            m_lifeTime = 0;
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
            m_actions = new List<BombAction>();

            while (m_isLiving)
            {
                m_lifeTime += 0.04F;
                Point pos = CompleteNextMovePoint(0.04F);
              
                MoveTo(pos.X,pos.Y);

                if (m_isLiving)
                {
                    if (Math.Round(m_lifeTime * 100) % 40 == 0 && pos.Y > 0)
                        m_game.AddTempPoint(pos.X, pos.Y);

                    if (m_controled && vY > 0)
                    {
                        Player player = m_map.FindNearestEnemy(m_x, m_y, 300, m_owner);

                        if (player != null)
                        {
                            Point v = new Point(player.X - m_x, player.Y - m_y);
                            v.Normalize(1000);
                            setSpeedXY(v.X, v.Y);
                            m_actions.Add(new BombAction(m_lifeTime, ActionType.CHANGE_SPEED, v.X, v.Y, 0,0));
                            //使炮弹不受重力和风力和空气阻力的影响
                            UpdateForceFactor(0, 0, 0);
                            m_controled = false;
                        }
                    }
                }
            }
        }

        protected override void CollideObjects(Physics[] list)
        {
            bool flag = false;
            foreach (Physics phy in list)
            {
                if (phy is Box)
                {
                    m_actions.Add(new BombAction(m_lifeTime, ActionType.PICK, phy.Id, 0,0,0));
                    m_owner.PickBox(phy as Box,false);
                }
                else
                {
                    flag = true;
                }
            }
            if (flag)
            {
                Bomb();
            }
        }

        protected override void CollideGround()
        {
            base.CollideGround();
            Bomb();
        }

        public void Bomb()
        {
            bool digMap = true;
            Player[] playersAround = m_map.FindPlayers(m_x, m_y, m_radius);
            foreach (Player p in playersAround)
            {
                if (p.IsNoHole || p.NoHoleTurn)
                {
                    p.NoHoleTurn = true;
                    digMap = false;
                }
            }
            if (digMap)
            {
                m_map.Dig(m_x, m_y, m_shape, null);
            }
            m_actions.Add(new BombAction(m_lifeTime, ActionType.BOMB, m_x, m_y, digMap ? 1 : 0, 0));

            switch (m_type)
            {
                case BombType.FORZEN:
                    foreach (Player p in playersAround)
                    {
                        p.IsFrost = true;
                        m_actions.Add(new BombAction(m_lifeTime, ActionType.FORZEN, p.Id, 0, 0,0));
                    }
                    break;

                case BombType.TRANFORM:
                    m_owner.SetXY(m_x, m_y);
                    m_owner.StartMoving();
                    m_actions.Add(new BombAction(m_lifeTime, ActionType.TRANSLATE, m_x, m_y, 0,0));
                    m_actions.Add(new BombAction(m_lifeTime, ActionType.START_MOVE, m_owner.Id, m_owner.X, m_owner.Y, m_owner.IsLiving ? 1 : 0));
                    break;

                default:
                    foreach (Player p in playersAround)
                    {
                        if (p.IsFrost)
                        {
                            m_actions.Add(new BombAction(m_lifeTime, ActionType.UNFORZEN, p.Id, 0, 0,0));
                        }
                        else
                        {
                            int damage = MakeDamage(p);
                            int critical = MakeCriticalDamage(p,damage);
                            if (p.TakeDamage(m_owner, damage, critical,false))
                            {
                                m_owner.TotalHurt += (damage + critical);
                                m_actions.Add(new BombAction(m_lifeTime, ActionType.KILL_PLAYER, p.Id,damage + critical , critical !=0 ? 2 : 1, 0));
                                m_actions.Add(new BombAction(m_lifeTime, ActionType.DANDER, p.Id, p.Dander, 0, 0));
                            }
                        }
                        p.EffectList.StopEffect(typeof(IceFronzeEffect));
                        p.EffectList.StopEffect(typeof(HideEffect));
                        p.EffectList.StopEffect(typeof(NoHoleEffect));
                        //被炸死后不需要移动
                        if (p.IsLiving)
                        {
                            p.StartMoving();
                            m_actions.Add(new BombAction(m_lifeTime, ActionType.START_MOVE, p.Id, p.X, p.Y, p.IsLiving ? 1 : 0));
                        }

                        m_owner.CurrentIsHitTarget = true;
                    }
                    break;
            }

            this.Die();
        }

        protected int MakeDamage(Player target)
        {
            int baseDamage = m_owner.PlayerDetail.BaseAttack;
            int baseGuard = target.PlayerDetail.BaseDefence;

            int defence = target.PlayerDetail.PlayerCharacter.Defence;
            int attack = m_owner.PlayerDetail.PlayerCharacter.Attack;
            if (m_owner.IgnoreArmor)
            {
                baseGuard = 0;
                defence = 0;
            }

            float damagePlus = m_owner.CurrentDamagePlus;
            float shootMinus = m_owner.CurrentShootMinus;

            //伤害={ 基础伤害*（1+攻击*0.1%）*[1-（基础护甲/200+防御*0.03%）] }*(1+道具攻击加成）*炸弹威力*连击系数
            double damage = (baseDamage * ( 1 + attack * 0.001) * (1 - (baseGuard / 200 + defence * 0.003))) * (1 + damagePlus) * shootMinus ;

            double distance = Math.Sqrt((target.X - X) * (target.X - X) + (target.Y - Y) * (target.Y - Y));

            damage = damage * (1 - distance / m_radius / 4);

            return (int)damage;
        }

        protected int MakeCriticalDamage(Player target,int baseDamage)
        {
            int lucky = m_owner.PlayerDetail.PlayerCharacter.Luck;
            Random rd = new Random();
            bool canHit = lucky * 45 > rd.Next(100000);
            if (canHit)
            {
                return (int)((0.5 + lucky * 0.0005) * baseDamage);
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
