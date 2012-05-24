using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using log4net;
using System.Reflection;
using System.IO;
using SqlDataProvider.Data;
using Game.Logic.Actions;
using Bussiness.Managers;
using Game.Logic.Phy.Maps;
using System.Drawing;
using Game.Logic.AI;
using Game.Server.Managers;
using Game.Logic.AI.Npc;

namespace Game.Logic.Phy.Object
{
    public class SimpleBoss : TurnedLiving
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private NpcInfo m_npcInfo;

        private ABrain m_ai;

        private List<SimpleNpc> m_child = new List<SimpleNpc>();

        private Dictionary<Player, int> m_mostHateful;

        public SimpleBoss(int id, BaseGame game, NpcInfo npcInfo, int direction, int type)
            : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
        {
            if (type == 0)
            {
                Type = eLivingType.SimpleBoss;
            }
            else
            {
                Type = eLivingType.SimpleBoss1;
            }

            m_mostHateful = new Dictionary<Player, int>();
            m_npcInfo = npcInfo;
            m_ai = ScriptMgr.CreateInstance(npcInfo.Script) as ABrain;
            if (m_ai == null)
            {
                log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
                m_ai = SimpleBrain.Simple;
            }
            m_ai.Game = m_game;
            m_ai.Body = this;
            try
            {

                m_ai.OnCreated();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleBoss Created error:{1}", ex);
            }
        }

        public override void Reset()
        {
            m_maxBlood = m_npcInfo.Blood;
            BaseDamage = m_npcInfo.BaseDamage;
            BaseGuard = m_npcInfo.BaseGuard;

            Attack = m_npcInfo.Attack;
            Defence = m_npcInfo.Defence;
            Agility = m_npcInfo.Agility;
            Lucky = m_npcInfo.Lucky;

            Grade = m_npcInfo.Level;
            Experience = m_npcInfo.Experience;

            SetRect(m_npcInfo.X, m_npcInfo.Y, m_npcInfo.Width, m_npcInfo.Height);

            base.Reset();
        }

        public override void Die()
        {
            base.Die();
        }

        public override void Die(int delay)
        {
            base.Die(delay);
        }

        public NpcInfo NpcInfo
        {
            get { return m_npcInfo; }
        }

        public List<SimpleNpc> Child
        {
            get { return m_child; }
        }

        public int CurrentLivingNpcNum
        {
            get
            {
                int count = 0;
                foreach (SimpleNpc child in Child)
                {
                    if (child.IsLiving == false)
                    {
                        count++;
                    }
                }
                return Child.Count - count;
            }
        }
        public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
        {
            bool result = false;
            result = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
            if (source is Player)
            {
                Player p = source as Player;
                int damage = damageAmount + criticalAmount;
                if (m_mostHateful.ContainsKey(p))
                {
                    m_mostHateful[p] = m_mostHateful[p] + damage;
                }
                else
                {
                    m_mostHateful.Add(p, damage);
                }
            }

            return result;
        }

        public Player FindMostHatefulPlayer()
        {
            if (m_mostHateful.Count > 0)
            {
                KeyValuePair<Player, int> k = m_mostHateful.ElementAt(0);
                foreach (KeyValuePair<Player, int> kvp in m_mostHateful)
                {
                    if (k.Value < kvp.Value)
                    {
                        k = kvp;
                    }
                }
                return k.Key;
            }
            return null;
        }

        public void CreateChild(int id, int x, int y, int disToSecond, int maxCount)
        {
            if (CurrentLivingNpcNum < maxCount)
            {
                if (maxCount - CurrentLivingNpcNum >= 2)
                {
                    Child.Add(((PVEGame)Game).CreateNpc(id, x + disToSecond, y, 1));
                    Child.Add(((PVEGame)Game).CreateNpc(id, x, y, 1));
                }
                else if (maxCount - CurrentLivingNpcNum == 1)
                {
                    Child.Add(((PVEGame)Game).CreateNpc(id, x, y, 1));
                }
            }
        }
        public void CreateChild(int id, Point[] brithPoint, int maxCount, int maxCountForOnce, int type)
        {

            var index = 0;
            var length = Game.Random.Next(0, maxCountForOnce);
            for (int i = 0; i < length; i++)
            {
                index = Game.Random.Next(0, brithPoint.Length);
                CreateChild(id, brithPoint[index].X, brithPoint[index].Y, 4, maxCount);
            }
          
        }

        public void RandomSay(string[] msg, int type, int delay, int finishTime)
        {
            string[] content = msg;
            string text = null;
            int IsSay = Game.Random.Next(0, 2);
            if (IsSay == 1)
            {
                int index = Game.Random.Next(0, content.Count());
                text = content[index];
                m_game.AddAction(new LivingSayAction(this, text, type, delay, finishTime));
            }
        }
        #region AI Rule
        public override void PrepareNewTurn()
        {
            base.PrepareNewTurn();
            try
            {

                m_ai.OnBeginNewTurn();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleBoss BeginNewTurn error:{1}", ex);
            }
        }

        public override void PrepareSelfTurn()
        {
            base.PrepareSelfTurn();
            AddDelay(m_npcInfo.Delay);

            try
            {
                m_ai.OnBeginSelfTurn();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleBoss BeginSelfTurn error:{1}", ex);
            }
        }

        public override void StartAttacking()
        {
            base.StartAttacking();
            try
            {
                m_ai.OnStartAttacking();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleBoss StartAttacking error:{1}", ex);
            }
            if (IsAttacking)
            {
                StopAttacking();
            }
        }

        public override void StopAttacking()
        {
            base.StopAttacking();

            try
            {
                m_ai.OnStopAttacking();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleBoss StopAttacking error:{1}", ex);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            try
            {

                m_ai.Dispose();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleBoss Dispose error:{1}", ex);
            }
        }
        #endregion
    }
}
