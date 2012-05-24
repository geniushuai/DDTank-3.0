using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Base
{
    /// <summary>
    /// This class is used to hold statistics about DOL usage
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// The total mob count
        /// </summary>
        public static int MemMobCount = 0;
        /// <summary>
        /// The total player count
        /// </summary>
        public static int MemPlayerCount = 0;
        /// <summary>
        /// The total account count
        /// </summary>
        public static int MemAccCount = 0;
        /// <summary>
        /// The total character count
        /// </summary>
        public static int MemCharCount = 0;
        /// <summary>
        /// The total incoming packet objects count
        /// </summary>
        public static int MemPacketInObj = 0;
        /// <summary>
        /// The total outgoing packet objects count
        /// </summary>
        public static int MemPacketOutObj = 0;
        /// <summary>
        /// The total spellhandler objects
        /// </summary>
        public static int MemSpellHandlerObj = 0;
        /// <summary>
        /// The total bytes sent
        /// </summary>
        public static long BytesOut = 0;
        /// <summary>
        /// The total bytes received
        /// </summary>
        public static long BytesIn = 0;
        /// <summary>
        /// The total outgoing packets
        /// </summary>
        public static long PacketsOut = 0;
        /// <summary>
        /// The total incoming packets
        /// </summary>
        public static long PacketsIn = 0;
    }
}
