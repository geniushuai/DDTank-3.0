using SqlDataProvider.Data;
using Game.Logic.Phy.Object;

namespace Game.Logic.Spells.FightingSpell
{
    [SpellAttibute((int)eSpellType.ADDWOUND)]
    public class AddWoudSpell:ISpellHandler
    {
        public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
        {
            player.CurrentDamagePlus += (float)item.Property2 / 100;
        }
    }
}
