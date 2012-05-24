using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.Messions
{
    public class CHM1272 : AMissionControl
    {
        private List<SimpleNpc> blueNpc = new List<SimpleNpc>();

        private int blueCount = 0;

        private int blueTotalCount = 0;

        private int dieBlueCount = 0;

        private int blueNpcID = 1202;

        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 1750)
            {
                return 3;
            }
            else if (score > 1675)
            {
                return 2;
            }
            else if (score > 1600)
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
            int[] resources = { blueNpcID };
            int[] gameOverResource = { blueNpcID, blueNpcID, blueNpcID, blueNpcID };
            Game.LoadResources(resources);
            Game.LoadNpcGameOverResources(gameOverResource);
            Game.SetMap(1072);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();
        }

        public override void OnPrepareNewGame()
        {
            base.OnPrepareNewGame();

            for (int i = 0; i < 4; i++)
            {
                blueTotalCount++;

                if (i < 1)
                {
                    blueNpc.Add(Game.CreateNpc(blueNpcID, 900 + (i + 1) * 100, 505, 1));
                }
                else if (i < 3)
                {
                    blueNpc.Add(Game.CreateNpc(blueNpcID, 920 + (i + 1) * 100, 505, 1));
                }
                else
                {
                    blueNpc.Add(Game.CreateNpc(blueNpcID, 1000 + (i + 1) * 100, 515, 1));
                }
            }

            blueTotalCount++;
            blueNpc.Add(Game.CreateNpc(blueNpcID, 1465, 494, 1));

            Game.BossCardCount = 1;
        }

        public override void OnNewTurnStarted()
        {
            blueCount = blueTotalCount - dieBlueCount;

            if (Game.GetLivedLivings().Count == 0)
            {
                Game.PveGameDelay = 0;
            }


            if (Game.TurnIndex > 1 && Game.CurrentPlayer.Delay > Game.PveGameDelay)
            {
                if (blueCount == 15)
                {
                    return;
                }

                if (blueTotalCount < 15)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        blueTotalCount++;

                        if (i < 1)
                        {
                            blueNpc.Add(Game.CreateNpc(blueNpcID, 900 + (i + 1) * 100, 505, 1));
                        }
                        else if (i < 3)
                        {
                            blueNpc.Add(Game.CreateNpc(blueNpcID, 920 + (i + 1) * 100, 505, 1));
                        }
                        else
                        {
                            blueNpc.Add(Game.CreateNpc(blueNpcID, 1000 + (i + 1) * 100, 515, 1));
                        }
                    }


                    blueTotalCount++;
                    blueNpc.Add(Game.CreateNpc(blueNpcID, 1465, 494, 1));
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (blueCount < 15 && blueTotalCount < Game.MissionInfo.TotalCount)
                        {
                            if (i < 1)
                            {
                                blueNpc.Add(Game.CreateNpc(blueNpcID, 900 + (i + 1) * 100, 505, 1));
                            }
                            else if (i < 3)
                            {
                                blueNpc.Add(Game.CreateNpc(blueNpcID, 920 + (i + 1) * 100, 505, 1));
                            }
                            else
                            {
                                blueNpc.Add(Game.CreateNpc(blueNpcID, 1000 + (i + 1) * 100, 515, 1));
                            }
                            blueCount++;
                            blueTotalCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (blueCount < 15 && blueTotalCount < Game.MissionInfo.TotalCount)
                    {
                        blueCount++;
                        blueTotalCount++;
                        blueNpc.Add(Game.CreateNpc(blueNpcID, 1465, 494, 1));
                    }
                }
            }

        }


        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
        }

        public override bool CanGameOver()
        {
            bool result = true;
            dieBlueCount = 0;


            foreach (SimpleNpc blueNpcSingle in blueNpc)
            {
                if (blueNpcSingle.IsLiving)
                {
                    result = false;
                }
                else
                {
                    dieBlueCount++;
                }
            }

            if (result && blueTotalCount == Game.MissionInfo.TotalCount)
            {
                Game.IsWin = true;
                return true;
            }
            return false;
        }

        public override int UpdateUIData()
        {
            base.UpdateUIData();
            return Game.TotalKillCount;
        }

        public override void OnPrepareGameOver()
        {
            base.OnPrepareGameOver();
        }

        public override void OnGameOver()
        {
            base.OnGameOver();
            if (Game.GetLivedLivings().Count == 0)
            {
                Game.IsWin = true;
            }
            else
            {
                Game.IsWin = false;
            }
            List<LoadingFileInfo> loadingFileInfos = new List<LoadingFileInfo>();
            loadingFileInfos.Add(new LoadingFileInfo(2, "image/map/3", ""));
            Game.SendLoadResource(loadingFileInfos);
        }
    }
}
