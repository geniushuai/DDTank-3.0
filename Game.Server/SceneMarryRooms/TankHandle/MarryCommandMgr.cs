using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Game.Server.SceneMarryRooms.TankHandle
{
    public class MarryCommandMgr
    {
        private Dictionary<int, IMarryCommandHandler> handles = new Dictionary<int, IMarryCommandHandler>();

        public IMarryCommandHandler LoadCommandHandler(int code)
        {
            return handles[code];
        }

        public MarryCommandMgr()
        {
            handles.Clear();
            SearchCommandHandlers(Assembly.GetAssembly(typeof(GameServer)));
        }

        protected int SearchCommandHandlers(Assembly assembly)
        {
            int count = 0;

            // Walk through each type in the assembly
            foreach (Type type in assembly.GetTypes())
            {
                // Pick up a class
                if (type.IsClass != true)
                    continue;
                if (type.GetInterface("Game.Server.SceneMarryRooms.TankHandle.IMarryCommandHandler") == null)
                    continue;

                MarryCommandAttbute[] attr = (MarryCommandAttbute[])type.GetCustomAttributes(typeof(MarryCommandAttbute), true);

                if (attr.Length > 0)
                {
                    count++;
                    RegisterCommandHandler(attr[0].Code, Activator.CreateInstance(type) as IMarryCommandHandler);
                }
            }
            return count;
        }

        protected  void RegisterCommandHandler(int code, IMarryCommandHandler handle)
        {
            handles.Add(code, handle);
        }
    }
}
