using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.STUNT, "使用必杀技能")]
    public class StuntCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (player.IsAttacking)
            {

                player.UseSpecialSkill();
                player.CurrentShootMinus *= (float)player.CurrentBall.Power;
            }
        }
    }
}
