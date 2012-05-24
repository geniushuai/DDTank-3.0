using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Game.Base.Packets;

namespace Game.Logic.Cmd
{
    [GameCommand((byte)eTankCmdType.GAME_MISSION_TRY_AGAIN, "关卡失败再试一次")]
    public class TryAgainCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if(game is PVEGame)
            {
                PVEGame pve = game as PVEGame;
                bool tryAgain = packet.ReadBoolean();
                bool isHost = packet.ReadBoolean();
                if (isHost == true)
                {
                    if (tryAgain == true)
                    {
                        if (player.PlayerDetail.RemoveMoney(100) > 0)
                        {
                            //退回关卡结算
                            pve.WantTryAgain = 1;
                            game.SendToAll(packet);
                            player.PlayerDetail.LogAddMoney(AddMoneyType.Game, AddMoneyType.Game_TryAgain, player.PlayerDetail.PlayerCharacter.ID, 100, player.PlayerDetail.PlayerCharacter.Money);
                        }
                        else
                        {
                            player.PlayerDetail.SendInsufficientMoney((int)eBattleRemoveMoneyType.TryAgain);
                        }
                    }
                    else
                    {
                        //退回房间
                        pve.WantTryAgain = 0;
                        game.SendToAll(packet);
                    }
                    pve.CheckState(0);
                }
            }
        }
    }
}
