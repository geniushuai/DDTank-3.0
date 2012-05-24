using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Collections;
using Game.Base;
using System.Timers;
using System.Net.Sockets;
using System.Threading;
using Game.Server.Packets.Client;
using Game.Base.Events;
using Game.Server;

namespace Game.Base.Packets
{
    /// <summary>
    /// This class handles the packets, receiving and sending
    /// </summary>
    public class PacketProcessor
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// currently active packet handler
        /// </summary>
        protected IPacketHandler m_activePacketHandler;

        /// <summary>
        /// thread id of running packet handler
        /// </summary>
        protected int m_handlerThreadID = 0;

        protected GameClient m_client;

        /// <summary>
        /// Constructs a new PacketProcessor
        /// </summary>
        /// <param name="client">The processor client</param>
        public PacketProcessor(GameClient client)
        {
            m_client = client;
        }


        #region Handle Package
       
        public void HandlePacket(GSPacketIn packet)
        {

            int code = packet.Code;

            Statistics.BytesIn += packet.Length;
            Statistics.PacketsIn++;

            IPacketHandler packetHandler = null;
            if (code < m_packetHandlers.Length)
                packetHandler = m_packetHandlers[code];
            else if (log.IsErrorEnabled)
            {
                log.ErrorFormat("Received packet code is outside of m_packetHandlers array bounds! " + m_client.ToString());
                log.Error(Marshal.ToHexDump(
                    String.Format("===> <{2}> Packet 0x{0:X2} (0x{1:X2}) length: {3} (ThreadId={4})", code, code ^ 168, m_client.TcpEndpoint, packet.Length, Thread.CurrentThread.ManagedThreadId),
                    packet.Buffer));
            }

            if (packetHandler != null)
            {

                long start = Environment.TickCount;
                try
                {
                    packetHandler.HandlePacket(m_client, packet);
                }
                catch (Exception e)
                {
                    if (log.IsErrorEnabled)
                    {
                        string client = m_client.TcpEndpoint;
                        log.Error("Error while processing packet (handler=" + packetHandler.GetType().FullName + "  client: " + client + ")", e);
                        log.Error(Marshal.ToHexDump("Package Buffer:", packet.Buffer, 0, packet.Length));
                    }
                }

                long timeUsed = Environment.TickCount - start;

                m_activePacketHandler = null;
                if (log.IsDebugEnabled)
                {
                    log.Debug("Package process Time:" + timeUsed + "ms!");
                }
                if (timeUsed > 1000)
                {
                    string source = m_client.TcpEndpoint;
                    if (log.IsWarnEnabled)
                        log.Warn("(" + source + ") Handle packet Thread " + Thread.CurrentThread.ManagedThreadId + " " + packetHandler + " took " + timeUsed + "ms!");
                }
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Stores all packet handlers found when searching the gameserver assembly
        /// </summary>
        protected static readonly IPacketHandler[] m_packetHandlers = new IPacketHandler[256];

        /// <summary>
        /// Callback function called when the scripts assembly has been compiled
        /// </summary>
        [ScriptLoadedEvent]
        public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
        {
            Array.Clear(m_packetHandlers, 0, m_packetHandlers.Length);
            int count = SearchPacketHandlers("v168", Assembly.GetAssembly(typeof(GameServer)));
            if (log.IsInfoEnabled)
                log.Info("PacketProcessor: Loaded " + count + " handlers from GameServer Assembly!");
        }

        /// <summary>
        /// Registers a packet handler
        /// </summary>
        /// <param name="handler">The packet handler to register</param>
        /// <param name="packetCode">The packet ID to register it with</param>
        public static void RegisterPacketHandler(int packetCode, IPacketHandler handler)
        {
            m_packetHandlers[packetCode] = handler;
        }

        /// <summary>
        /// Searches an assembly for packet handlers
        /// </summary>
        /// <param name="version">namespace of packethandlers to search eg. 'v167'</param>
        /// <param name="assembly">Assembly to search</param>
        /// <returns>The number of handlers loaded</returns>
        protected static int SearchPacketHandlers(string version, Assembly assembly)
        {
            int count = 0;

            // Walk through each type in the assembly
            foreach (Type type in assembly.GetTypes())
            {
                // Pick up a class
                if (type.IsClass != true)
                    continue;
                if (type.GetInterface("Game.Server.Packets.Client.IPacketHandler") == null)
                    continue;
                PacketHandlerAttribute[] packethandlerattribs = (PacketHandlerAttribute[])type.GetCustomAttributes(typeof(PacketHandlerAttribute), true);
                if (packethandlerattribs.Length > 0)
                {
                    count++;
                    RegisterPacketHandler(packethandlerattribs[0].Code, (IPacketHandler)Activator.CreateInstance(type));
                }
            }
            return count;
        }
        #endregion
    }
}
