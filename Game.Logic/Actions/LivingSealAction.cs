using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Game.Logic.Effects;
namespace Game.Logic.Actions
{

    public class LivingSealAction:BaseAction
    {
        private Living m_Living;

        private Player m_Target;

        private int m_Type;

        public LivingSealAction(Living Living, Player target, int type, int delay):base(delay, 2000)
        {
            m_Living = Living;
            m_Target = target;
            m_Type = type;
        }

        protected override void ExecuteImp(BaseGame game, long tick)
        {
            m_Target.AddEffect(new SealEffect(2, m_Type), 0);
            Finish(tick);
        }
    }
}
