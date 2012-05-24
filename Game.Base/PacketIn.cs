using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Game.Base.Packets;
using System.Threading;
using log4net;
using System.Reflection;

namespace Game.Base
{
    /// <summary>
    /// Class reads data from incoming incoming packets
    /// </summary>
    public class PacketIn
    {
        protected byte[] m_buffer;
       // protected byte[] m_buffer2;
        protected int m_length;
        protected int m_offset;
        public static int[] SEND_KEY = { 174, 191, 86, 120, 171, 205, 239, 241 };
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="buf">Buffer containing packet data to read from</param>
        /// <param name="start">Starting index into buf</param>
        /// <param name="size">Number of bytes to read from buf</param>
        public PacketIn(byte[] buf, int len)
        {
            m_buffer = buf;
           // m_buffer2 = new byte[10024];
            m_length = len;
            m_offset = 0;
        }

        public byte[] Buffer
        {
            get { return m_buffer; }
        }

        public int Length
        {
            get { return m_length; }
        }

        public int Offset
        {
            get { return m_offset; }
            set { m_offset = value; }
        }

        public int DataLeft
        {
            get { return m_length - m_offset; }
        }

        /// <summary>
        /// Skips 'num' bytes ahead in the stream
        /// </summary>
        /// <param name="num">Number of bytes to skip ahead</param>
        public void Skip(int num)
        {
            m_offset += num;
        }

        public virtual bool ReadBoolean()
        {
            return m_buffer[m_offset++] != 0;
        }

        public virtual byte ReadByte()
        {
            return m_buffer[m_offset++];
        }

        /// <summary>
        /// Reads in 2 bytes and converts it from network to host byte order
        /// </summary>
        /// <returns>A 2 byte (short) value</returns>
        public virtual short ReadShort()
        {
            byte v1 = (byte)ReadByte();
            byte v2 = (byte)ReadByte();
            return Marshal.ConvertToInt16(v1, v2);
        }

        /// <summary>
        /// Reads in 2 bytes
        /// </summary>
        /// <returns>A 2 byte (short) value in network byte order</returns>
        public virtual short ReadShortLowEndian()
        {
            byte v1 = (byte)ReadByte();
            byte v2 = (byte)ReadByte();
            return Marshal.ConvertToInt16(v2, v1);
        }

        /// <summary>
        /// Reads in 4 bytes and converts it from network to host byte order
        /// </summary>
        /// <returns>A 4 byte value</returns>
        public virtual int ReadInt()
        {
            byte v1 = (byte)ReadByte();
            byte v2 = (byte)ReadByte();
            byte v3 = (byte)ReadByte();
            byte v4 = (byte)ReadByte();
            return Marshal.ConvertToInt32(v1, v2, v3, v4);
        }

        /// <summary>
        /// Reads in 4 bytes and converts it from network to host byte order
        /// </summary>
        /// <returns>A 4 byte value</returns>
        public virtual float ReadFloat()
        {
            byte[] v = new byte[4];
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = (byte)ReadByte();
            }

            return BitConverter.ToSingle(v, 0);
        }

        /// <summary>
        /// Reads in 8 bytes and converts it from network to host byte order
        /// </summary>
        /// <returns>A 8 byte value</returns>
        public virtual double ReadDouble()
        {
            byte[] v = new byte[8];
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = (byte)ReadByte();
            }

            return BitConverter.ToDouble(v, 0);
        }

        /// <summary>
        /// Reads a null-terminated string from the stream
        /// </summary>
        /// <param name="maxlen">Maximum number of bytes to read in</param>
        /// <returns>A string of maxlen or less</returns>
        public virtual string ReadString()
        {
            short len = ReadShort();
            string temp = Encoding.UTF8.GetString(m_buffer, m_offset, len);
            m_offset += len;
            return temp.Replace("\0", "");
            //return temp;
        }

        public virtual byte[] ReadBytes(int maxLen)
        {
            byte[] data = new byte[maxLen];
            Array.Copy(m_buffer, m_offset, data, 0, maxLen);
            m_offset += maxLen;
            return data;
        }

        public virtual byte[] ReadBytes()
        {
            return ReadBytes(m_length - m_offset);
        }

        /// <summary>
        /// Read a datetime
        /// </summary>
        /// <returns></returns>
        public DateTime ReadDateTime()
        {
            return new DateTime(ReadShort(), ReadByte(), ReadByte(), ReadByte(), ReadByte(), ReadByte());
        }

        /// <summary>
        /// Copy data to target buffer,this doesn't move offset.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual int CopyTo(byte[] dst, int dstOffset, int offset)
        {
            int len = m_length - offset < dst.Length - dstOffset ? m_length - offset : dst.Length - dstOffset;
            if (len > 0)
            {
                System.Buffer.BlockCopy(m_buffer, offset, dst, dstOffset, len);
            }
            return len;
        }

        public virtual int CopyTo(byte[] dst, int dstOffset, int offset, int key)
        {
            int len = m_length - offset < dst.Length - dstOffset ? m_length - offset : dst.Length - dstOffset;

            if (len > 0)
            {
                key = (key & (0xff << 16)) >> 16;
                for (int i = 0; i < len; i++)
                {
                    dst[dstOffset + i] = (byte)(m_buffer[offset + i] ^ key);
                }
            }
            return len;
        }
        public volatile bool isSended = true;
        //public virtual void CreateKey(int dstOffset, int offset, byte[] key)
        //{
        //    if (!isSended) return;
        //    var len = m_length;
        //    m_sended = m_length;
        //    //for(m_buffer2
        //    for (int i = 0; i < m_length; i++)
        //    {
        //        m_buffer2[i] = 0;
        //    }
        //    lock (this)
        //    {
        //        m_buffer2[offset] = (byte)(m_buffer[offset % 8] ^ key[offset % 8]);
        //        for (int i = 1; i < len; i++)
        //        {
        //            key[(+i) % 8] = (byte)(key[i % 8] + m_buffer2[offset + i - 1] ^ i);
        //            m_buffer2[offset + i] = (byte)((m_buffer[offset + i] ^ key[offset + i % 8]) + m_buffer2[offset + i - 1]);
        //        }
        //        isSended = false;
        //    }

        //}
        public volatile int m_sended = 0;
        //public virtual int CopyTo3(byte[] dst, int dstOffset, int offset, byte[] key)
        //{
        //    int len = m_length - offset < dst.Length - dstOffset ? m_length - offset : dst.Length - dstOffset;
        //    var str = string.Empty;
        //    CreateKey(dstOffset, offset, key);
        //    if (len > 0)
        //    {
        //        lock (this)
        //        {
        //            m_sended -= len;
        //            for (int i = 0; i < len; i++)
        //            {
        //                dst[dstOffset + i] = (byte)(m_buffer2[offset + i]);
        //            }
        //            if (m_sended <= 0)
        //            {
        //                isSended = true;
        //            };
        //        }
        //        ////key = (key & (0xff << 16)) >> 16;
        //        //if (offset == 0)
        //        //{
        //        //    dst[dstOffset] = (byte)(m_buffer[offset] ^ key[offset % 8]);
        //        //}
        //        //else if (offset > 0)
        //        //{
        //        //    key[offset % 8] = (byte)(key[offset % 8] + dst[offset - 1] ^ offset);
        //        //    dst[dstOffset] = (byte)((m_buffer[offset] ^ key[offset % 8]) + dst[offset - 1]);
        //        //}
        //        //for (int i = 1; i < len; i++)
        //        //{
        //        //    //str += "keyold:" + key[i % 8] + "\n";
        //        //    key[(offset + i) % 8] = (byte)(key[(offset + i) % 8] + dst[(i - 1)] ^ i);
        //        //    //str += "keynew:" + key[i % 8] + "\n";
        //        //    dst[dstOffset + i] = (byte)((m_buffer[offset + i] ^ key[(offset + i) % 8]) + dst[i - 1]);

        //        //    //str += "src:" + (m_buffer[offset + i] + ",backvalue:" + m_buffer[offset + i - 1]) + ",dst:" + dst[dstOffset + i] + "\n";

        //        //}
        //    }


        //    return len;

        //}
        public  volatile int packetNum=0;
        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public virtual int CopyTo3(byte[] dst, int dstOffset, int offset, byte[] key,ref int packetArrangeSend)
        {
            int len = m_length - offset < dst.Length - dstOffset ? m_length - offset : dst.Length - dstOffset;
            var str = string.Empty;
            //CreateKey(dstOffset, offset, key);
            
            if (len > 0)
            {
                var indexBuffex = 0;
                var indexKey = 0;
                indexKey = m_sended + dstOffset;
                if (isSended)
                {
                    packetNum = Interlocked.Increment(ref packetArrangeSend);
                    packetArrangeSend = packetNum;
                    m_sended = 0;
                    isSended = false;
                    indexKey = m_sended + dstOffset;
                }
                else
                {
                    //neu chua goi xong thi phai bat dau` tu vi tri' cuoi cu data;
                   indexKey = 2048;
                }
                if (packetNum != packetArrangeSend)
                {
                   
                    //log.Info("Some thing Error happen");
                    return 0;

                }

                
               // m_sended += len;
                for (int i = 0; i < len; i++)
                {

                    indexBuffex = offset + i;
                    while (indexKey > 2048)
                    {
                        //log.Info("IndexKey" + indexKey+"i"+i+"m_sended:"+m_sended);
                        indexKey -= 2048;
                    }
                    if (m_sended == 0)
                    {
                        dst[dstOffset] = (byte)(m_buffer[indexBuffex] ^ key[m_sended % 8]);
                    }
                    else
                    {
                        key[m_sended % 8] = (byte)(key[m_sended % 8] + dst[indexKey - 1] ^ m_sended);
                        dst[dstOffset + i] = (byte)((m_buffer[indexBuffex] ^ key[m_sended % 8]) + dst[indexKey - 1]);
                    }
                    m_sended++;
                    indexKey++;


                }
            }
            return len;
        }


        public virtual int CopyFrom(byte[] src, int srcOffset, int offset, int count)
        {
            if (count < m_buffer.Length && count - srcOffset < src.Length)
            {
                System.Buffer.BlockCopy(src, srcOffset, m_buffer, offset, count);
                return count;
            }
            return -1;
        }


        public virtual int CopyFrom(byte[] src, int srcOffset, int offset, int count, int key)
        {
            if (count < m_buffer.Length && count - srcOffset < src.Length)
            {
                key = (key & (0xff << 16)) >> 16;
                for (int i = 0; i < count; i++)
                {
                    m_buffer[offset + i] = (byte)(src[srcOffset + i] ^ key);
                }
                return count;
            }
            return -1;
        }

        public virtual int[] CopyFrom3(byte[] src, int srcOffset, int offset, int count, byte[] key)
        {

            int[] result = new int[count];
            for (int i = 0; i < count; i++)
            {
                m_buffer[i] = (byte)src[i];
            }
            if (count < m_buffer.Length && count - srcOffset < src.Length)
            {


                m_buffer[0] = (byte)(src[srcOffset] ^ key[0]);
                for (int i = 1; i < count; i++)
                {
                    key[i % 8] = (byte)(key[i % 8] + src[(srcOffset + i - 1)] ^ i);
                    m_buffer[i] = (byte)(src[srcOffset + i] - src[(srcOffset + i - 1)] ^ key[i % 8]);
                    //  m_buffer[i] = (byte)(src[i] ^ 152);
                }
            }
            return result;

        }

        public virtual void WriteBoolean(bool val)
        {
            m_buffer[m_offset++] = val ? (byte)1 : (byte)0;
            m_length = m_offset > m_length ? m_offset : m_length;
        }

        public virtual void WriteByte(byte val)
        {
            m_buffer[m_offset++] = val;
            m_length = m_offset > m_length ? m_offset : m_length;
        }

        public virtual void Write(byte[] src)
        {
            Write(src, 0, src.Length);
        }

        public virtual void Write(byte[] src, int offset, int len)
        {
            Array.Copy(src, offset, m_buffer, m_offset, len);
            m_offset += len;
            m_length = m_offset > m_length ? m_offset : m_length;
        }

        /// <summary>
        /// Writes a 2 byte (short) value to the stream in network byte order
        /// </summary>
        /// <param name="val">Value to write</param>
        public virtual void WriteShort(short val)
        {
            WriteByte((byte)(val >> 8));
            WriteByte((byte)(val & 0xff));
        }

        /// <summary>
        /// Writes a 2 byte (short) value to the stream in host byte order
        /// </summary>
        /// <param name="val">Value to write</param>
        public virtual void WriteShortLowEndian(short val)
        {
            WriteByte((byte)(val & 0xff));
            WriteByte((byte)(val >> 8));
        }

        /// <summary>
        /// Writes a 4 byte value to the stream in host byte order
        /// </summary>
        /// <param name="val">Value to write</param>
        public virtual void WriteInt(int val)
        {
            WriteByte((byte)(val >> 24));
            WriteByte((byte)((val >> 16) & 0xff));
            WriteByte((byte)((val & 0xffff) >> 8));
            WriteByte((byte)((val & 0xffff) & 0xff));
        }

        /// <summary>
        /// Writes a 4 byte value to the stream in host byte order
        /// </summary>
        /// <param name="val">Value to write</param>
        public virtual void WriteFloat(float val)
        {
            byte[] src = BitConverter.GetBytes(val);
            Write(src);
        }

        /// <summary>
        /// Writes a 4 byte value to the stream in host byte order
        /// </summary>
        /// <param name="val">Value to write</param>
        public virtual void WriteDouble(double val)
        {
            byte[] src = BitConverter.GetBytes(val);
            Write(src);
        }

        /// <summary>
        /// Writes the supplied value to the stream for a specified number of bytes
        /// </summary>
        /// <param name="val">Value to write</param>
        /// <param name="num">Number of bytes to write</param>
        public virtual void Fill(byte val, int num)
        {
            for (int i = 0; i < num; ++i)
            {
                WriteByte(val);
            }
        }

        /// <summary>
        /// Writes a C-style string to the stream
        /// </summary>
        /// <param name="str">String to write</param>
        public virtual void WriteString(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                WriteShort((short)(bytes.Length + 1));
                Write(bytes, 0, bytes.Length);
                WriteByte(0x0);
            }
            else
            {
                WriteShort((short)1);
                WriteByte(0x0);
            }
        }


        /// <summary>
        /// Writes up to maxlen bytes to the stream from the supplied string
        /// </summary>
        /// <param name="str">String to write</param>
        /// <param name="maxlen">Maximum number of bytes to be written</param>
        public virtual void WriteString(string str, int maxlen)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            int len = bytes.Length < maxlen ? bytes.Length : maxlen;
            WriteShort((short)len);
            Write(bytes, 0, len);
        }

        /// <summary>
        ///  writes a dattime to buffer
        /// </summary>
        /// <param name="date"></param>
        public void WriteDateTime(DateTime date)
        {
            WriteShort((short)date.Year);
            WriteByte((byte)date.Month);
            WriteByte((byte)date.Day);
            WriteByte((byte)date.Hour);
            WriteByte((byte)date.Minute);
            WriteByte((byte)date.Second);
        }
    }
}
