using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Collections;
using Game;
using Game.Server.Managers;
using Game.Base.Events;

namespace Game.Base
{
    public enum ePrivLevel : uint
    {
        Player = 1,
        GM = 2,
        Admin = 3,
    }

    public class GameCommand
    {
        public string[] m_usage;
        public string m_cmd;
        public uint m_lvl;
        public string m_desc;
        public ICommandHandler m_cmdHandler;
    }

    public class CommandMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Hashtable m_cmds = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

        private static string[] m_disabledarray = new string[0];

        public static string[] DisableCommands
        {
            get { return m_disabledarray; }
            set
            {
                m_disabledarray = value == null ? new string[0] : value;
            }
        }

        public static GameCommand GetCommand(string cmd)
        {
            return m_cmds[cmd] as GameCommand;
        }

        public static GameCommand GuessCommand(string cmd)
        {
            GameCommand myCommand = GetCommand(cmd);
            if (myCommand != null) return myCommand;

            string compareCmdStr = cmd.ToLower();
            IDictionaryEnumerator iter = m_cmds.GetEnumerator();

            while (iter.MoveNext())
            {
                GameCommand currentCommand = iter.Value as GameCommand;
                string currentCommandStr = iter.Key as string;

                if (currentCommand == null) continue;

                if (currentCommandStr.ToLower().StartsWith(compareCmdStr))
                {
                    myCommand = currentCommand;
                    break;
                }
            }
            return myCommand;
        }

        public static string[] GetCommandList(ePrivLevel plvl, bool addDesc)
        {
            IDictionaryEnumerator iter = m_cmds.GetEnumerator();

            ArrayList list = new ArrayList();

            while (iter.MoveNext())
            {
                GameCommand cmd = iter.Value as GameCommand;
                string cmdString = iter.Key as string;

                if (cmd == null || cmdString == null)
                {
                    continue;
                }

                if (cmdString[0] == '&')
                    cmdString = '/' + cmdString.Remove(0, 1);
                if ((uint)plvl >= cmd.m_lvl)
                {
                    if (addDesc)
                    {
                        list.Add(cmdString + " - " + cmd.m_desc);
                    }
                    else
                    {
                        list.Add(cmd.m_cmd);
                    }
                }
            }

            return (string[])list.ToArray(typeof(string));
        }

        [ScriptLoadedEvent]
        public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
        {
            LoadCommands();
        }

        public static bool LoadCommands()
        {
            m_cmds.Clear();

            ArrayList asms = new ArrayList(ScriptMgr.Scripts);

            foreach (Assembly script in asms)
            {
                if (log.IsDebugEnabled)
                    log.Debug("ScriptMgr: Searching for commands in " + script.GetName());
                // Walk through each type in the assembly
                foreach (Type type in script.GetTypes())
                {
                    // Pick up a class
                    if (type.IsClass != true) continue;
                    if (type.GetInterface("Game.Base.ICommandHandler") == null) continue;

                    try
                    {
                        object[] objs = type.GetCustomAttributes(typeof(CmdAttribute), false);
                        foreach (CmdAttribute attrib in objs)
                        {
                            bool disabled = false;
                            foreach (string str in m_disabledarray)
                            {
                                if (attrib.Cmd.Replace('&', '/') == str)
                                {
                                    disabled = true;
                                    log.Info("Will not load command " + attrib.Cmd + " as it is disabled in game properties");
                                    break;
                                }
                            }
                            if (disabled)
                                continue;
                            if (m_cmds.ContainsKey(attrib.Cmd))
                            {
                                log.Info(attrib.Cmd + " from " + script.GetName() + " has been suppressed, a command of that type already exists!");
                                continue;
                            }
                            if (log.IsDebugEnabled)
                                log.Debug("Load: " + attrib.Cmd + "," + attrib.Description);

                            GameCommand cmd = new GameCommand();
                            cmd.m_usage = attrib.Usage;
                            cmd.m_cmd = attrib.Cmd;
                            cmd.m_lvl = attrib.Level;
                            cmd.m_desc = attrib.Description;
                            cmd.m_cmdHandler = (ICommandHandler)Activator.CreateInstance(type);
                            m_cmds.Add(attrib.Cmd, cmd);
                            if (attrib.Aliases != null)
                            {
                                foreach (string alias in attrib.Aliases)
                                {
                                    m_cmds.Add(alias, cmd);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (log.IsErrorEnabled)
                            log.Error("LoadCommands", e);
                    }
                }
            }
            log.Info("Loaded " + m_cmds.Count + " commands!");
            return true;
        }

        public static void DisplaySyntax(BaseClient client)
        {
            client.DisplayMessage("Commands list:");
            foreach (string str in GetCommandList(ePrivLevel.Admin, true))
            {
                client.DisplayMessage("         "+str);
            }
        }

        public static bool HandleCommandNoPlvl(BaseClient client, string cmdLine)
        {
            try
            {
                string[] pars = ParseCmdLine(cmdLine);
                GameCommand myCommand = GuessCommand(pars[0]);

                if (myCommand == null) return false;

                ExecuteCommand(client, myCommand, pars);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("HandleCommandNoPlvl", e);
            }
            return true;
        }

        private static bool ExecuteCommand(BaseClient client, GameCommand myCommand, string[] pars)
        {
            pars[0] = myCommand.m_cmd;
            return myCommand.m_cmdHandler.OnCommand(client, pars);
        }

        private static string[] ParseCmdLine(string cmdLine)
        {
            if (cmdLine == null)
            {
                throw new ArgumentNullException("cmdLine");
            }

            List<string> args = new List<string>();
            int state = 0;
            StringBuilder arg = new StringBuilder(cmdLine.Length >> 1);

            for (int i = 0; i < cmdLine.Length; i++)
            {
                char c = cmdLine[i];
                switch (state)
                {
                    case 0: // waiting for first arg char
                        if (c == ' ') continue;
                        arg.Length = 0;
                        if (c == '"') state = 2;
                        else
                        {
                            state = 1;
                            i--;
                        }
                        break;
                    case 1: // reading arg
                        if (c == ' ')
                        {
                            args.Add(arg.ToString());
                            state = 0;
                        }
                        arg.Append(c);
                        break;
                    case 2: // reading string
                        if (c == '"')
                        {
                            args.Add(arg.ToString());
                            state = 0;
                        }
                        arg.Append(c);
                        break;
                }
            }
            if (state != 0) args.Add(arg.ToString());

            string[] pars = new string[args.Count];
            args.CopyTo(pars);

            return pars;
        }


    }
}
