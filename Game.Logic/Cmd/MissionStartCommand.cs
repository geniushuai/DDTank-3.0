using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Logic.Phy.Object;
using Game.Base.Packets;
using Bussiness;

namespace Game.Logic.Cmd
{
    [GameCommand((int)eTankCmdType.GAME_MISSION_START, "关卡开始")]
    public class MissionStartCommand : ICommandHandler
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
                if (isReady == true)
                {
                    player.Ready = true;
                    game.CheckState(0);
                }
            }
        }
    }
}
