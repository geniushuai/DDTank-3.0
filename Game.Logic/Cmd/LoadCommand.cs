using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.LOAD,"游戏加载进度")]
    public class LoadCommand:ICommandHandler
    {
        public void  HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (game.GameState == eGameState.Loading)
            {
                player.LoadingProcess = packet.ReadInt();
                if (player.LoadingProcess >= 100)
                {
                    game.CheckState(0);
                }
               var pkg = new GSPacketIn((short)ePackageType.GAME_CMD);
                pkg.WriteByte((byte)eTankCmdType.LOAD);
                //var pkg = packet.Clone();
              //  pkg.ClearContext();
                pkg.WriteInt(player.LoadingProcess);
                pkg.WriteInt(4);
                pkg.WriteInt(player.PlayerDetail.PlayerCharacter.ID);
                game.SendToAll(pkg);
            }
        }
    }
}
