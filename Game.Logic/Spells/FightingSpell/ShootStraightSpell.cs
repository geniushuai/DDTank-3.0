using SqlDataProvider.Data;
using Game.Logic.Phy.Object;

namespace Game.Logic.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.SHOOTSTRAIGHT)]
    public class ShootStraightSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            player.ControlBall = true;
            player.CurrentShootMinus *= 0.5f;
        }
    }
}
