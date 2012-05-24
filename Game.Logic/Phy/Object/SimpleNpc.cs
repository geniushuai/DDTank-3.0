using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using log4net;
using System.Reflection;
using System.IO;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Game.Logic.AI;
using Game.Server.Managers;
using Game.Logic.AI.Npc;

namespace Game.Logic.Phy.Object
{
    public class SimpleNpc : Living
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private NpcInfo m_npcInfo;
        private ABrain m_ai;

        public SimpleNpc(int id, BaseGame game, NpcInfo npcInfo, int type)
            : base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, -1)
        {
            if (type == 0)
            {
                Type = eLivingType.SimpleNpc;
            }
            else
            {
                Type = eLivingType.SimpleNpc1;
            }
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
                log.ErrorFormat("SimpleNpc Created error:{1}", ex);
            }

        }

        public override void Reset()
        {
            Agility = m_npcInfo.Agility;
            Attack = m_npcInfo.Attack;
            BaseDamage = m_npcInfo.BaseDamage;
            BaseGuard = m_npcInfo.BaseGuard;
            Lucky = m_npcInfo.Lucky;
            Grade = m_npcInfo.Level;
            Experience = m_npcInfo.Experience;
            SetRect(m_npcInfo.X, m_npcInfo.Y, m_npcInfo.Width, m_npcInfo.Height);
            base.Reset();
        }

        public void GetDropItemInfo()
        {            
            //NPC物品掉落表
            if (m_game.CurrentLiving is Player)
            {
                Player p = m_game.CurrentLiving as Player;
                List<ItemInfo> infos = null;
                int gold = 0;
                int money = 0;
                int gifttoken = 0;
                DropInventory.NPCDrop(m_npcInfo.DropId, ref infos);
                if (infos != null)
                {
                    foreach (ItemInfo info in infos)
                    {                        
                        ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref gifttoken);
                        if (info != null)
                        {
                            if (info.Template.CategoryID == 10)
                            {
                                //添加物品到道具栏
                                p.PlayerDetail.AddTemplate(info, eBageType.FightBag, info.Count);
                            }
                            else
                            {
                                p.PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count);
                            }
                        }

                    }
                    p.PlayerDetail.AddGold(gold);
                    p.PlayerDetail.AddMoney(money);
                    p.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_Drop, p.PlayerDetail.PlayerCharacter.ID, money, p.PlayerDetail.PlayerCharacter.Money);
                    p.PlayerDetail.AddGiftToken(gifttoken);
                }
            }
        }

        public override void Die()
        {
            GetDropItemInfo();
            base.Die();
        }

        public override void Die(int delay)
        {
            GetDropItemInfo();
            base.Die(delay);
        }

        public NpcInfo NpcInfo
        {
            get { return m_npcInfo; }
        }

        public override void PrepareNewTurn()
        {
            base.PrepareNewTurn();
            try
            {

                m_ai.OnBeginNewTurn();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SimpleNpc BeginNewTurn error:{1}", ex);
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
                log.ErrorFormat("SimpleNpc StartAttacking error:{1}", ex);
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
                log.ErrorFormat("SimpleNpc Dispose error:{1}", ex);
            }
        }
    }
}
