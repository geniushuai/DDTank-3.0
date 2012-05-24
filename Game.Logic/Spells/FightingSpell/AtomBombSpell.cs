using SqlDataProvider.Data;
using Game.Logic.Phy.Object;

namespace Game.Logic.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.ABOMB)]
    public class ABombSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            player.SetBall(4);
        }

    }
}
