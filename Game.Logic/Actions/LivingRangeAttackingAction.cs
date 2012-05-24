using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Game.Base.Packets;
using System.Drawing;

namespace Game.Logic.Actions
{
    public class LivingRangeAttackingAction : BaseAction
    {

        private Living m_living;

        private List<Player> m_players;

        private int m_fx;

        private int m_tx;

        private String m_action;



        public LivingRangeAttackingAction(Living living, int fx, int tx, string action, int delay, List<Player>players)
            : base(delay, 1000)
        {
            m_living = living;
            m_players = players;
            m_fx = fx;
            m_tx = tx;
            m_action = action;
        }

        private int MakeDamage(Living p)
        {
            double baseDamage = m_living.BaseDamage;
            double baseGuard = p.BaseGuard;

            double defence = p.Defence;
            double attack = m_living.Attack;

            if (m_living.IgnoreArmor)
            {
                baseGuard = 0;
                defence = 0;
            }

            float damagePlus = m_living.CurrentDamagePlus;
            float shootMinus = m_living.CurrentShootMinus;


            double DR1 = 0.95 * (p.BaseGuard - 3 * m_living.Grade) / (500 + p.BaseGuard - 3 * m_living.Grade);//护甲提供伤害减免
            double DR2 = 0;
            if ((p.Defence - m_living.Lucky) < 0)
            {
                DR2 = 0;
            }
            else
            {
                DR2 = 0.95 * (p.Defence - m_living.Lucky) / (600 + p.Defence - m_living.Lucky); //防御提供的伤害减免
            }
            //DR2 = DR2 < 0 ? 0 : DR2;

            double damage = (baseDamage * (1 + attack * 0.001) * (1 - (DR1 + DR2 - DR1 * DR2))) * damagePlus * shootMinus;

            Rectangle rect = p.GetDirectDemageRect();
            double distance = Math.Sqrt((rect.X - m_living.X) * (rect.X - m_living.X) + (rect.Y - m_living.Y) * (rect.Y - m_living.Y));
            damage = damage * (1 - distance /Math.Abs(m_tx - m_fx) / 4);
            
            if (damage < 0)
            {
                return 1;
            }
            else
            {
                return (int)damage;
            }
        }

        private int MakeCriticalDamage(Living p, int baseDamage)
        {
            double lucky = m_living.Lucky;

            Random rd = new Random();
            bool canHit = 75000 * lucky / (lucky + 800) > rd.Next(100000);
            if (canHit)
            {
                return (int)((0.5 + lucky * 0.0003) * baseDamage);
            }
            else
            {
                return 0;
            }
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_CMD, m_living.Id);

            pkg.Parameter1 = m_living.Id;
            pkg.WriteByte((byte)eTankCmdType.LIVING_RANGEATTACKING);

            List<Living> playersAround = game.Map.FindPlayers(m_fx, m_tx, m_players);
            int count = playersAround.Count;
            foreach (Living p in playersAround)
            {
                if (m_living.IsFriendly(p) == true)
                    count--;
            }
            pkg.WriteInt(count);
            m_living.SyncAtTime = false;
            try
            {
                foreach (Living p in playersAround)
                {
                    p.SyncAtTime = false;

                    if (m_living.IsFriendly(p) == true)
                        continue;
                    int dander = 0;
                    p.IsFrost = false;
                    game.SendGameUpdateFrozenState(p);
                    int damage = MakeDamage(p);
                    int critical = MakeCriticalDamage(p, damage);
                    int totalDemageAmount = 0;
                    if (p.TakeDamage(m_living, ref damage, ref critical, "范围攻击"))
                    {
                        totalDemageAmount = damage + critical;
                        if (p is Player)
                        {
                            Player player = p as Player;
                            dander = player.Dander;
                        }
                    }
                    pkg.WriteInt(p.Id);
                    pkg.WriteInt(totalDemageAmount);
                    pkg.WriteInt(p.Blood);
                    //同步怒气值
                    pkg.WriteInt(dander);
                    //attackEffect
                    pkg.WriteInt(new Random().Next(2));
                }
                game.SendToAll(pkg);

                Finish(tick);
            }
            finally
            {
                m_living.SyncAtTime = true;
                foreach (Living p in playersAround)
                {
                    p.SyncAtTime = true;
                }
            }
        }
    }
}
