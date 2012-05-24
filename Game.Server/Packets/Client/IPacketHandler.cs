using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    /// <summary>
    /// The interface for all received packets
    /// </summary>
    public interface IPacketHandler
    {
        /// <summary>
        /// Handles every received packet
        /// </summary>
        /// <param name="client">The client that sent the packet</param>
        /// <param name="packet">The received packet data</param>
        /// <returns></returns>
        int HandlePacket(GameClient client, GSPacketIn packet);
    }
}
