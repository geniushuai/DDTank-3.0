using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using log4net;
using System.Reflection;

namespace Game.Base.Packets
{
    /// <summary>
    /// Game server specific packet
    /// </summary>
    public class GSPacketIn : PacketIn
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Header size including checksum at the end of the packet
        /// </summary>
        public const ushort HDR_SIZE = 20;

        public const short HEADER = 0x71ab;

        /// <summary>
        /// Porotocal ID
        /// </summary>
        protected short m_code;
        /// <summary>
        /// Client ID
        /// </summary>
        protected int m_cliendId;

        protected int m_parameter1;

        protected int m_parameter2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Size of the internal buffer</param>
        public GSPacketIn(byte[] buf, int size)
            : base(buf, size)
        {
        }

        public GSPacketIn(short code)
            : this(code, 0, 2048)
        {
        }

        public GSPacketIn(short code, int clientId)
            : this(code, clientId, 2048)
        {

        }

        public GSPacketIn(short code, int clientId, int size)
            : base(new byte[size], HDR_SIZE)
        {
            m_code = code;
            m_cliendId = clientId;
            m_offset = HDR_SIZE;
        }

        /// <summary>
        /// Protocal
        /// </summary>
        public short Code
        {
            get { return m_code; }
            set { m_code = value; }
        }

        /// <summary>
        /// Gets the Client id
        /// </summary>
        public int ClientID
        {
            get { return m_cliendId; }
            set { m_cliendId = value; }
        }

        public int Parameter1
        {
            get { return m_parameter1; }
            set { m_parameter1 = value; }
        }

        public int Parameter2
        {
            get { return m_parameter2; }
            set { m_parameter2 = value; }
        }

        /// <summary>
        /// ?
        /// </summary>
        public void ReadHeader()
        {
            ReadShort();
            m_length = ReadShort();
            ReadShort();
            m_code = ReadShort();
            m_cliendId = ReadInt();
            m_parameter1 = ReadInt();
            m_parameter2 = ReadInt();
        }

        public void WriteHeader()
        {
            lock (this)
            {
                int old = m_offset;
                m_offset = 0;
                base.WriteShort(HEADER);
                base.WriteShort((short)m_length); //reserved for size
                base.WriteShort(checkSum());
                base.WriteShort(m_code);
                base.WriteInt(m_cliendId);
                base.WriteInt(m_parameter1);
                base.WriteInt(m_parameter2);
                m_offset = old;
            }
            lock (this)
            {
                int old = m_offset;
                m_offset = 0;
                base.WriteShort(HEADER);
                base.WriteShort((short)m_length); //reserved for size
                base.WriteShort(checkSum());
                base.WriteShort(m_code);
                base.WriteInt(m_cliendId);
                base.WriteInt(m_parameter1);
                base.WriteInt(m_parameter2);
                m_offset = old;
            }
        }
        public short checkSum()
        {
            short val1 = 0x77;
            int i = 6;
            while (i < m_length)
            {
                val1 += m_buffer[i++];
            }
            var test = val1 & 0x7F7F;
            short test2 = (short)test;
            return test2;
        }
        public void WritePacket(GSPacketIn pkg)
        {
            pkg.WriteHeader();
            Write(pkg.Buffer, 0, pkg.Length);
        }

        public GSPacketIn ReadPacket()
        {
            byte[] buffer = ReadBytes();
            GSPacketIn pkg = new GSPacketIn(buffer, buffer.Length);
            pkg.ReadHeader();
            return pkg;
        }

        public void Compress()
        {

            byte[] temp = Marshal.Compress(m_buffer, HDR_SIZE, Length - HDR_SIZE);
            m_offset = HDR_SIZE;
            Write(temp);
            m_length = temp.Length + HDR_SIZE;
        }

        public void UnCompress()
        {

        }

        public void ClearContext()
        {
            m_offset = HDR_SIZE;
            m_length = HDR_SIZE;
        }

        public GSPacketIn Clone()
        {
            GSPacketIn pkg = new GSPacketIn(m_buffer, m_length);
            pkg.ReadHeader();
            pkg.Offset = m_length;
            return pkg;
        }


    }
}
