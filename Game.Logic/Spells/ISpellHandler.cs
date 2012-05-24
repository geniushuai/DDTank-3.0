using SqlDataProvider.Data;
using Game.Logic.Phy.Object;

namespace Game.Logic.Spells
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISpellHandler
    {
        void Execute(BaseGame game, Player player,ItemTemplateInfo item);
    }
}
