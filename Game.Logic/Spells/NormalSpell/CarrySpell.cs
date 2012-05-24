using SqlDataProvider.Data;
using Game.Logic.Phy.Object;

namespace Game.Logic.Spells.NormalSpell
{
    [SpellAttibute((int)eSpellType.CARRY)]
    public class CarrySpell:ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            player.SetBall(3);
        }
    }
}
