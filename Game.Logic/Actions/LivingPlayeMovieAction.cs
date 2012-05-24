using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions
{
    public class LivingPlayeMovieAction:BaseAction
    {
        private Living m_living;
        private string m_action;
        
        public LivingPlayeMovieAction(Living living, string action, int delay, int movieTime)
            : base(delay, movieTime)
        {
            m_living = living;

            m_action = action;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            game.SendLivingPlayMovie(m_living, m_action);
            Finish(tick);
        }
    }
}
