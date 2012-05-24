using System;
using Game.Base.Packets;
using log4net;
using System.Reflection;
using Game.Logic.Phy.Object;
using Game.Logic.Cmd;

namespace Game.Logic.Actions
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
                eTankCmdType type = (eTankCmdType)m_packet.ReadByte();
                try
                {
                    ICommandHandler handleCommand = CommandMgr.LoadCommandHandler((int)type);
                    if (handleCommand != null)
                    {
                        handleCommand.HandleCommand(game, m_player, m_packet);
                    }
                    else
                    {
                        log.Error(string.Format("Player Id: {0}", m_player.Id));
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Player Id: {0}  cmd:0x{1:X2}", m_player.Id,(byte)type),ex);
                }
            }
        }

        public bool IsFinished(long tick)
        {
            return true;
        }
    }
}
