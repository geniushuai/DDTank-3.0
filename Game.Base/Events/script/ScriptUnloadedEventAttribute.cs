using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Base.Events
{
    /// <summary>
    /// This attribute can be applied to static methods to automatically
    /// register them with the GameServer's global script unloaded event
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScriptUnloadedEventAttribute : Attribute
    {
        /// <summary>
        /// Constructs a new ScriptUnloadedEventAttribute
        /// </summary>
        public ScriptUnloadedEventAttribute()
        {
        }
    }
}
