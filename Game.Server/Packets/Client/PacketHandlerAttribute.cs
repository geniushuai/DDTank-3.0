using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Packets.Client
{
    /// <summary>
    /// Denotes a class as a packet handler
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketHandlerAttribute : Attribute
    {
        /// <summary>
        /// Packet ID to handle
        /// </summary>
        protected int m_code;
        /// <summary>
        /// Description of the packet handler
        /// </summary>
        protected string m_desc;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of packet to handle</param>
        /// <param name="code">ID of the packet to handle</param>
        /// <param name="desc">Description of the packet handler</param>
        public PacketHandlerAttribute(int code, string desc)
        {
            m_code = code;
            m_desc = desc;
        }

        /// <summary>
        /// Gets the packet ID that is handled
        /// </summary>
        public int Code
        {
            get
            {
                return m_code;
            }
        }

        /// <summary>
        /// Gets the description of the packet handler
        /// </summary>
        public string Description
        {
            get
            {
                return m_desc;
            }
        }
    }
}
