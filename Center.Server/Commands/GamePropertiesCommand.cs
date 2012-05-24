using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;

namespace Center.Server.Commands
{
    [Cmd("&gp",ePrivLevel.Admin,
        "Manage game properties at runtime.",
        "   /gp <option> [para1][para2] ...",
        "eg:    /gp -view   :List all game properties.",
        "       /gp -load   :Load game properties from database.",
        "       /gp -save   :Save game properties into database.")]
    public class GamePropertiesCommand:AbstractCommandHandler,ICommandHandler
    {
        public bool OnCommand(BaseClient client, string[] args)
        {
            //TODO,完成gp命令的内容

            return true;
        }
    }
}
