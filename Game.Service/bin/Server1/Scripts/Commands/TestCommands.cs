using System;
using System.Collections.Generic;
using System.Text;
using Game.Server;
using Game.Base;

namespace GameServerScript.Commands
{
    [CmdAttribute(
        "&version",
        ePrivLevel.Player,
        "   显示版本信息",
        "       /version :  显示版本信息",
        "                   ")]
    public class VersionCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        public bool OnCommand(BaseClient client, string[] args)
        {
            DisplayMessage(client,"Servion Id:{0},Version:{1}",GameServer.Instance.Configuration.ServerID,GameServer.Edition);

            return true;
        }
    }
}
