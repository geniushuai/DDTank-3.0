using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd
{
    /// <summary>
    /// 战斗中拾取宝箱
    /// </summary>
    [GameCommand((byte)eTankCmdType.PICK,"战斗中拾取箱子")]
    public class PickCommand:ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            player.OpenBox(packet.ReadInt());
        }
    }
}
