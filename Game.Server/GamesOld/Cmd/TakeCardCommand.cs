using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phy.Object;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Bussiness.Managers;
using Game.Base.Packets;

namespace Game.Server.Games.Cmd
{
    [GameCommand((byte)TankCmdType.TAKE_CARD, "翻牌")]
    public class TakeCardCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, Game.Base.Packets.GSPacketIn packet)
        {
            if (player.HasTakeCard == false && player.CanTakeOut == true)
            {
                int index = packet.ReadByte();
                if (index < 0 || index > 7)
                {
                    game.TakeCard(player);
                }
                else
                {
                    game.TakeCard(player,index);
                }
            }
        }
    }
}
