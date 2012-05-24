using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Game.Base.Packets;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.BOSS_TAKE_CARD, "战胜关卡中Boss翻牌")]
    public class BossTakeCardCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (game is PVEGame)
            {
                PVEGame pve = game as PVEGame;
                if (pve.BossCardCount + 1 > 0)
                {
                    int index = packet.ReadByte();
                    if (index < 0 || index > pve.BossCards.Length)
                    {
                        if (pve.IsBossWar != "")
                        {
                            pve.TakeBossCard(player);
                        }
                        else pve.TakeCard(player);
                    }
                    else
                    {
                        if (pve.IsBossWar != "")
                        {
                            pve.TakeBossCard(player, index);
                        }
                        else pve.TakeCard(player, index);
                    }
                }
            }
        }
    }
}
