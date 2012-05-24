using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.FIRE, "用户开炮")]
    public class FireCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsAttacking)
            {
                int x = packet.ReadInt();
                int y = packet.ReadInt();

                //检查开炮点的距离有效性
                if (player.CheckShootPoint(x, y) == false)
                    return;

                int force = packet.ReadInt();
                int angle = packet.ReadInt();
                player.Shoot(x, y, force, angle);
            }
        }
    }
}
