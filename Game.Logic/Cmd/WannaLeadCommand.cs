using Game.Logic.Phy.Object;
using Game.Base.Packets;

namespace Game.Logic.Cmd
{
    [GameCommand((int)eTankCmdType.WANNA_LEADER,"希望成为队长")]
    public class WannaLeadCommand:ICommandHandler
    {
        public void HandleCommand(BaseGame game,Player player, GSPacketIn packet)
        {
            //game.SendToAll(packet);
        }
    }
}
