using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Games
{
    public interface IAction
    {
        void Execute(BaseGame game,long tick);
        bool IsFinish();
    }
}
