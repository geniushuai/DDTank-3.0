using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Game.Server.SceneGames.TankHandle
{
    public class CommandMgr
    {
        private static Dictionary<int, ICommandHandler> handles = new Dictionary<int, ICommandHandler>();

        public static ICommandHandler LoadCommandHandler(int code)
        {
            return handles[code];
        }

        [ScriptLoadedEvent]
        public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
        {
            handles.Clear();
            SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
        }

        protected static int SearchCommandHandlers(Assembly assembly)
        {
            int count = 0;

            // Walk through each type in the assembly
            foreach (Type type in assembly.GetTypes())
            {
                // Pick up a class
                if (type.IsClass != true)
                    continue;
                if (type.GetInterface("Game.Server.SceneGames.TankHandle.ICommandHandler") == null)
                    continue;

                CommandAttbute[] attr = (CommandAttbute[])type.GetCustomAttributes(typeof(CommandAttbute), true);

                if (attr.Length > 0)
                {
                    count++;
                    RegisterCommandHandler(attr[0].Code, Activator.CreateInstance(type) as ICommandHandler);
                }
            }
            return count;
        }

        protected  void RegisterCommandHandler(int code, ICommandHandler handle)
        {
            handles.Add(code, handle);
        }
    }
}
