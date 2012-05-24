using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.FIRE_TAG,"准备开炮")]
    public class FireTagCommand:ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsAttacking)
            {
                packet.Parameter1 = player.Id;
                game.SendToAll(packet);

                //同客户端同步LifeTime
                game.SendSyncLifeTime();
                bool tag = packet.ReadBoolean();

                //开炮所需时间
                byte speedTime = packet.ReadByte();
                player.PrepareShoot(speedTime);
            }
        }
    }
}
