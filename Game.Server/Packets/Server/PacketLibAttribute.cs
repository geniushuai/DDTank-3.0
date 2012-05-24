using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Base.Packets
{
    /// <summary>
    /// Denotes a class as a packet lib.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class PacketLibAttribute : Attribute
    {
        /// <summary>
        /// Stores version Id sent by the client.
        /// </summary>
        int m_rawVersion;

        /// <summary>
        /// Constructs a new PacketLibAttribute.
        /// </summary>
        /// <param name="rawVersion">The version Id sent by the client.</param>
        /// <param name="clientVersion">PacketLib client version.</param>
        public PacketLibAttribute(int rawVersion)
        {
            m_rawVersion = rawVersion;
        }

        /// <summary>
        /// Gets version Id sent by the client.
        /// </summary>
        public int RawVersion
        {
            get { return m_rawVersion; }
        }
    }
}
