using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic.Actions
{
    public interface IAction
    {
        void Execute(BaseGame game,long tick);
        bool IsFinished(long tick);
    }
}
