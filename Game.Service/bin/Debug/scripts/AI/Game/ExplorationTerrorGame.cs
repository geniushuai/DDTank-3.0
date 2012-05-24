using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;

namespace GameServerScript.AI.Game
{
    public class ExplorationTerrorGame :ExplorationGame
    {
        public override void OnCreated()
        {
            totalMissionCount = 5;
            missionIds = "1001,1002,1003,1004,1005";
            base.OnCreated();
        }
    }
}
