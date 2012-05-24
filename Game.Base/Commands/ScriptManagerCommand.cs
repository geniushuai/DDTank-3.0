using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Game.Server.Managers;
using System.IO;

namespace Game.Base.Commands
{
    [CmdAttribute("&sm",
        ePrivLevel.Player,
        "Script Manager console commands.",
        "   /sm  <option>  [para1][para2]...",
        "eg: /sm -list              : List all assemblies in scripts array.",
        "    /sm -add <assembly>    : Add assembly into the scripts array.",
        "    /sm -remove <assembly> : Remove assembly from the scripts array.")]
    public class ScriptManagerCommand:AbstractCommandHandler,ICommandHandler
    {
        public bool OnCommand(BaseClient client, string[] args)
        {
            if (args.Length > 1)
            {
                switch (args[1])
                {
                    case "-list":
                        foreach (Assembly ass in ScriptMgr.Scripts)
                        {
                            DisplayMessage(client,ass.FullName);
                        }
                        return true;
                    case "-add":
                        if (args.Length > 2 && args[2] != null && File.Exists(args[2]))
                        {
                            try
                            {
                                Assembly ass = Assembly.LoadFile(args[2]);
                                if (ScriptMgr.InsertAssembly(ass))
                                {
                                    DisplayMessage(client, "Add assembly success!");
                                    return true;
                                }
                                else
                                {
                                    DisplayMessage(client, "Assembly already exists in the scripts array!");
                                    return false;
                                }
                            }
                            catch (Exception ex)
                            {
                                DisplayMessage(client, "Add assembly error:", ex.Message);
                                return false;
                            }
                        }
                        else
                        {
                            DisplayMessage(client,"Can't find add assembly!");
                            return false;
                        }
                    case "-remove":
                        if (args.Length > 2 && args[2] != null && File.Exists(args[2]))
                        {
                            try
                            {
                                Assembly ass = Assembly.LoadFile(args[2]);
                                if (ScriptMgr.RemoveAssembly(ass))
                                {
                                    DisplayMessage(client, "Remove assembly success!");
                                    return true;
                                }
                                else
                                {
                                    DisplayMessage(client, "Assembly didn't exist in the scripts array!");
                                    return false;
                                }
                            }
                            catch (Exception ex)
                            {
                                DisplayMessage(client, "Remove assembly error:", ex.Message);
                                return false;
                            }
                        }
                        else
                        {
                            DisplayMessage(client, "Can't find remove assembly!");
                            return false;
                        }
                    default:
                        DisplayMessage(client,"Can't fine option:{0}", args[1]);
                        return true;
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
