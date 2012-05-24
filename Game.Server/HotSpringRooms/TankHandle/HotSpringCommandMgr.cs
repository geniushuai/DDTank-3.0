using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Game.Server.HotSpringRooms.TankHandle
{
    public class HotSpringCommandMgr
    {
        private Dictionary<int, IHotSpringCommandHandler> handles = new Dictionary<int, IHotSpringCommandHandler>();

        public IHotSpringCommandHandler LoadCommandHandler(int code)
        {
            return handles[code];
        }

        public HotSpringCommandMgr()
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
                if (type.GetInterface("Game.Server.HotSpringRooms.TankHandle.IHotSpringCommandHandler") == null)
                    continue;

                HotSpringCommandAttribute[] attr = (HotSpringCommandAttribute[])type.GetCustomAttributes(typeof(HotSpringCommandAttribute), true);

                if (attr.Length > 0)
                {
                    count++;
                    RegisterCommandHandler(attr[0].Code, Activator.CreateInstance(type) as IHotSpringCommandHandler);
                }
            }
            return count;
        }

        protected  void RegisterCommandHandler(int code, IHotSpringCommandHandler handle)
        {
            handles.Add(code, handle);
        }
    }
}
