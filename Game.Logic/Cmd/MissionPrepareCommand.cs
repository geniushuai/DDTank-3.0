using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Game.Base.Packets;
using Bussiness;

namespace Game.Logic.Cmd
{
    [GameCommand((int)eTankCmdType.GAME_MISSION_PREPARE, "关卡准备")]
    public class MissionPrepareCommand : ICommandHandler
    {
        public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
        {
            if (game.GameState == eGameState.SessionPrepared || game.GameState == eGameState.GameOver)
            {
                //if (player.Weapon == null)
                //{
                //    player.PlayerDetail.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
                //    return;
                //}
                bool isReady = packet.ReadBoolean();
                if (player.Ready != isReady)
                {
                    player.Ready = isReady;
                    game.SendToAll(packet);
                }
            }
        }
    }
}
