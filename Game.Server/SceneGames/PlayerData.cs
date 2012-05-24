using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;

namespace Game.Server.SceneGames
{
    public enum TankGameState
    {
        NONE = 1,
        FRIST = 2,
        WAIT = 3,
        FINISH = 4,
        DEAD = 5,
        LOSE = 6,
    }
}
