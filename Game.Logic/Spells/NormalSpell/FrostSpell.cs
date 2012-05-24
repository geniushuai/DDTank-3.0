using SqlDataProvider.Data;
using Game.Logic.Phy.Object;

namespace Game.Logic.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.FROST)]
    public class FrostSpell : ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            player.SetBall(1);
        }
    }
}
