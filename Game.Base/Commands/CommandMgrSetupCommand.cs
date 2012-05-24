using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Base.Commands
{
    [Cmd("&cmd",ePrivLevel.Admin,"Config the command system.",
        "/cmd [option] <para1> <para2>      ",
        "eg: /cmd -reload           :Reload the command system.",
        "    /cmd -list             :Display all commands.")]
    public class CommandMgrSetupCommand:AbstractCommandHandler,ICommandHandler
    {
        public bool OnCommand(BaseClient client, string[] args)
        {
            if (args.Length > 1)
            {
                switch (args[1])
                {
                    case "-reload":
                        CommandMgr.LoadCommands();
                        break;
                    case "-list":
                        CommandMgr.DisplaySyntax(client);
                        break;
                    default:
                        DisplaySyntax(client);
                        break;
                }
            }
            else
            {
                DisplaySyntax(client);
            }
            return true;
        }
    }
}
