using Game.Logic.Phy.Object;
using Game.Base.Packets;
using System;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.TAKE_CARD, "翻牌")]
    public class TakeCardCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            //if (player.FinishTakeCard == false && player.CanTakeOut > 0)
            //{
                int index = packet.ReadByte();
                if (index < 0 || index > game.Cards.Length)
                {
                    game.TakeCard(player);
                }
                else
                {
                    game.TakeCard(player,index);
                }
            //}
        }
    }
}
