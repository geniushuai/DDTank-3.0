using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness.Managers;
using Game.Base;
using Bussiness;

namespace Game.Server.Commands.Admin
{
    [CmdAttribute("&load",
        ePrivLevel.Player,
        "Load the metedata.",
        "   /load  [option]...  ",
        "Option:    /config     :Application config file.",
        "           /shop       :ShopMgr.ReLoad().",
        "           /item       :ItemMgr.Reload().",
        "           /property   :Game properties.")]
    public class ReloadCommand:AbstractCommandHandler,ICommandHandler
    {
        public bool OnCommand(BaseClient client, string[] args)
        {
            if (args.Length > 1)
            {
                StringBuilder success = new StringBuilder();
                StringBuilder failed = new StringBuilder();
                if (args.Contains<string>("/cmd"))
                {
                    CommandMgr.LoadCommands();
                    DisplayMessage(client, "Command load success!");
                    success.Append("/cmd,");
                }
                
                if (args.Contains<string>("/config"))
                {
                    GameServer.Instance.Configuration.Refresh();
                    DisplayMessage(client, "Application config file load success!");
                    success.Append("/config,");
                }

                if (args.Contains<string>("/property"))
                {
                    GameProperties.Refresh();
                    DisplayMessage(client, "Game properties load success!");
                    success.Append("/property,");
                }

                if (args.Contains<string>("/item"))
                {
                    if (ItemMgr.ReLoad())
                    {
                        DisplayMessage(client,"Items load success!");
                        success.Append("/item,");
                    }
                    else
                    {
                        DisplayMessage(client,"Items load failed!");
                        failed.Append("/item,");
                    }
                }
                if (args.Contains<string>("/shop"))
                {
                    if (ItemMgr.ReLoad())
                    {
                        DisplayMessage(client, "Shops load success!");
                        success.Append("/shop,");
                    }
                    else
                    {
                        DisplayMessage(client,"Shops load failed!");
                        failed.Append("/shop,");
                    }
                }
                if (success.Length == 0 && failed.Length == 0)
                {
                    DisplayMessage(client,"Nothing executed!");
                    DisplaySyntax(client);
                }
                else
                {
                    DisplayMessage(client,"Success Options:    " + success.ToString());

                    if (failed.Length > 0)
                    {
                        DisplayMessage(client, "Faile Options:      " + failed.ToString());
                        return false;
                    }
                }
                return true;
            }
            else
            {
                DisplaySyntax(client);
            }
            return true;
        }
    }
}
