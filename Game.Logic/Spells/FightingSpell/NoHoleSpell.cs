using SqlDataProvider.Data;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Logic.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.NOHOLE)]
    public class NoHoleSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            new NoHoleEffect(item.Property3).Start(player);
        }
    }
}
