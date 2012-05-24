using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using Game.Base.Events;

namespace Game.Server.SceneGames
{
    public class SceneGameMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        #region Static Methods

        /// <summary>
        /// Stores all packet handlers found when searching the gameserver assembly
        /// </summary>
        protected static readonly IGameProcessor[] _gameProcessors = new IGameProcessor[256];

        public static IGameProcessor LoadGameProcessor(byte code)
        {
            return _gameProcessors[code];
        }

        /// <summary>
        /// Callback function called when the scripts assembly has been compiled
        /// </summary>
        [ScriptLoadedEvent]
        public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
        {
            Array.Clear(_gameProcessors, 0, _gameProcessors.Length);

            int count = SearchGameProcessors(typeof(GameServer).Assembly);

            if (log.IsInfoEnabled)
                log.Info(string.Format("GameProcessor: Loaded {0} processors!",count));
        }

        public static void RegisterProcessor(int code, IGameProcessor processor)
        {
            _gameProcessors[code] = processor;
        }

        protected static int SearchGameProcessors(Assembly assembly)
        {
            int count = 0;

            // Walk through each type in the assembly
            foreach (Type type in assembly.GetTypes())
            {
                // Pick up a class
                if (type.IsClass != true)
                    continue;
                if (type.GetInterface("Game.Server.SceneGames.IGameProcessor") == null)
                    continue;
                GameProcessorAttribute[] attr = (GameProcessorAttribute[])type.GetCustomAttributes(typeof(GameProcessorAttribute), true);
                
                if (attr.Length > 0)
                {
                    count++;
                    RegisterProcessor(attr[0].Code, (IGameProcessor)Activator.CreateInstance(type));
                }
            }
            return count;
        }
        #endregion
    }

    
}
