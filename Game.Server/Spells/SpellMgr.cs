using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using Game.Base.Events;

namespace Game.Server.Spells
{
    public class SpellMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<int,ISpellHandler> handles = new Dictionary<int,ISpellHandler>();

        public static ISpellHandler LoadSpellHandler(int code)
        {
            return handles[code];
        }

        [ScriptLoadedEvent]
        public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
        {
            handles.Clear();
            int count = SearchSpellHandlers(Assembly.GetAssembly(typeof(GameServer)));
            if (log.IsInfoEnabled)
                log.Info("SpellMgr: Loaded " + count + " spell handlers from GameServer Assembly!");
        }

        protected static int SearchSpellHandlers(Assembly assembly)
        {
            int count = 0;

            // Walk through each type in the assembly
            foreach (Type type in assembly.GetTypes())
            {
                // Pick up a class
                if (type.IsClass != true)
                    continue;
                if (type.GetInterface("Game.Server.Spells.ISpellHandler") == null)
                    continue;

                SpellAttibute[] attr = (SpellAttibute[])type.GetCustomAttributes(typeof(SpellAttibute),true);

                if (attr.Length > 0)
                {
                    count++;
                    RegisterSpellHandler(attr[0].Type, Activator.CreateInstance(type) as ISpellHandler);
                }
            }
            return count;
        }

        protected static void RegisterSpellHandler(int type, ISpellHandler handle)
        {
            handles.Add(type, handle);
        }
    }
}
