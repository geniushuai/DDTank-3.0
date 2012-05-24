using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.Managers;
using Game.Base;

namespace Game.Base.Commands
{
    [CmdAttribute("&cs",
        ePrivLevel.Player,
        "Compile the C# scripts.",
        "/cs  <source file> <target> <importlib>",
        "eg: /cs ./scripts temp.dll game.base.dll,game.logic.dll")]
    public class BuildScriptCommand:AbstractCommandHandler,ICommandHandler
    {
        public bool OnCommand(BaseClient client, string[] args)
        {
            if (args.Length >= 4)
            {
                string path = args[1];
                string target = args[2];
                string libs = args[3];
                ScriptMgr.CompileScripts(false, path, target,libs.Split(','));
            }
            else
            {
                DisplaySyntax(client);
            }

            return true;
        }
    }
}
