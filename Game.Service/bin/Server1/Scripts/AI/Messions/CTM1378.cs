using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using Game.Logic;

namespace GameServerScript.AI.Messions
{
    public class CTM1378 : AMissionControl
    {
        private SimpleBoss m_king = null;

        private int m_kill = 0;

        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 900)
            {
                return 3;
            }
            else if (score > 825)
            {
                return 2;
            }
            else if (score > 725)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            
            NpcInfo npcInfo28 = NPCInfoMgr.GetNpcInfoById(28);
            Game.GameOverResources[0] = npcInfo28.ModelID;
            Game.AddLoadingFile(2, npcInfo28.ResourcesPath, npcInfo28.ModelID);

            NpcInfo npcInfo32 = NPCInfoMgr.GetNpcInfoById(32);
            Game.GameOverResources[1] = npcInfo32.ModelID;
            Game.AddLoadingFile(2, npcInfo32.ResourcesPath, npcInfo32.ModelID);

            Game.SetMap(1076);
            Game.IsBossWar = "真炸弹人王";
        }

        public override void OnStartGame()
        {
            base.OnStartGame();
            m_king = Game.CreateBoss(32, 750, 510, -1, 0);

            m_king.SetRelateDemagemRect(-41, -187, 83, 140);
            //m_king.Say("你们知道的太多了，我不能让你们继续活着！", 3000);
            m_king.AddDelay(16);
        }

        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
        }

        public override bool CanGameOver()
        {


            if (m_king.IsLiving == false)
            {
                m_kill++;
                return true;
            }

            if (Game.TurnIndex > Game.MissionInfo.TotalTurn - 1)
            {
                return true;
            }

            return false;

        }

        public override int UpdateUIData()
        {
            base.UpdateUIData();
            return m_kill;
        }

        public override void OnGameOver()
        {
            base.OnGameOver();
            bool IsAllPlayerDie = true;
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving == true)
                {
                    IsAllPlayerDie = false;
                }
            }
            if (m_king.IsLiving == false && IsAllPlayerDie == false)
            {
                Game.IsWin = true;
            }
            else
            {
                Game.IsWin = false;
            }
        }
    }
}
