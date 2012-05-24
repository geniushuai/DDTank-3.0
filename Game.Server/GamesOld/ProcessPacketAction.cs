using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phy.Object;
using Game.Base.Packets;
using log4net;
using System.Reflection;
using Game.Server.Games.Cmd;

namespace Game.Server.Games
{
    public class ProcessPacketAction:IAction
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Player m_player;

        private GSPacketIn m_packet;

        public ProcessPacketAction(Player player, GSPacketIn pkg)
        {
            m_player = player;
            m_packet = pkg;
        }

        public void Execute(BaseGame game, long tick)
        {
            if (m_player.IsActive)
            {
                TankCmdType type = (TankCmdType)m_packet.ReadByte();
                try
                {
                    ICommandHandler handleCommand = CommandMgr.LoadCommandHandler((int)type);
                    if (handleCommand != null)
                    {
                        handleCommand.HandleCommand(game, m_player, m_packet);
                    }
                    else
                    {
                        log.Error(string.Format("IP: {0}", m_player.PlayerDetail.Client.TcpEndpoint));
                    }
                }
                catch (Exception e)
                {
                    log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", e.ToString(), m_player.PlayerDetail.Client.TcpEndpoint));
                }
            }
        }

        public bool IsFinish()
        {
            return true;
        }
    }
}
