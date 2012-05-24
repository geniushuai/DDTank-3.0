using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using log4net;
using System.Collections;
using SqlDataProvider.Data;
using System.Threading;
using System.Reflection;
using Bussiness;
using Game.Base.Packets;


namespace Game.Server.Buffer
{
    public class BufferList
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private object m_lock;

        protected List<AbstractBuffer> m_buffers;

        protected ArrayList m_clearList;

        protected volatile sbyte m_changesCount;

        private GamePlayer m_player;

        public BufferList(GamePlayer player)
        {
            m_player = player;
            m_lock = new object();
            m_buffers = new List<AbstractBuffer>();
            m_clearList = new ArrayList();
        }

        /// <summary>
        /// 从数据库中加载
        /// </summary>
        /// <param name="playerId"></param>
        public void LoadFromDatabase(int playerId)
        {
            lock (m_lock)
            {
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    BufferInfo[] infos = db.GetUserBuffer(playerId);
                    BeginChanges();

                    foreach (BufferInfo info in infos)
                    {
                        AbstractBuffer buffer = CreateBuffer(info);
                        if (buffer != null)
                        {
                            buffer.Start(m_player);
                        }
                    }

                    CommitChanges();
                }

                //更新状态
                Update();
            }
        }

        /// <summary>
        /// 保存到数据库中
        /// </summary>
        public void SaveToDatabase()
        {
            lock (m_lock)
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    foreach (AbstractBuffer buffer in m_buffers)
                    {
                        pb.SaveBuffer(buffer.Info);
                    }

                    foreach (BufferInfo info in m_clearList)
                    {
                        pb.SaveBuffer(info);
                    }
                    m_clearList.Clear();
                }
            }
        }


        public bool AddBuffer(AbstractBuffer buffer)
        {
            lock (m_buffers)
            {
                m_buffers.Add(buffer);
            }

            OnBuffersChanged(buffer);

            return true;
        }


        public bool RemoveBuffer(AbstractBuffer buffer)
        {
            lock (m_buffers)
            {
                if (m_buffers.Remove(buffer))
                {
                    m_clearList.Add(buffer.Info);
                }
            }

            OnBuffersChanged(buffer);

            return true;
        }

        public void UpdateBuffer(AbstractBuffer buffer)
        {
            OnBuffersChanged(buffer);
        }


        #region BeginChanges/CommiteChanges/UpdateChanges

        protected ArrayList m_changedBuffers = new ArrayList();

        private int m_changeCount;

        protected void OnBuffersChanged(AbstractBuffer buffer)
        {
            if (m_changedBuffers.Contains(buffer) == false)

                m_changedBuffers.Add(buffer);

            if (m_changeCount <= 0 && m_changedBuffers.Count > 0)
            {
                UpdateChangedBuffers();
            }
        }

        public void BeginChanges()
        {
            Interlocked.Increment(ref m_changeCount);
        }


        public void CommitChanges()
        {

            int changes = Interlocked.Decrement(ref m_changeCount);

            if (changes < 0)
            {
                if (log.IsErrorEnabled)
                    log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
                Thread.VolatileWrite(ref m_changeCount, 0);
            }
            if (changes <= 0 && m_changedBuffers.Count > 0)
            {
                UpdateChangedBuffers();
            }
        }

        public void UpdateChangedBuffers()
        {

            List<BufferInfo> m_changbuffers = new List<BufferInfo>();

            foreach (AbstractBuffer buffer in m_changedBuffers)
            {
                m_changbuffers.Add(buffer.Info);

            }
            BufferInfo[] changeBuffers = m_changbuffers.ToArray();
            GSPacketIn pkg = m_player.Out.SendUpdateBuffer(m_player, changeBuffers);
            if (m_player.CurrentRoom != null)
            {
                m_player.CurrentRoom.SendToAll(pkg, m_player);
            }
            m_changedBuffers.Clear();

        }


        #endregion

        #region Buffers Method

        public virtual AbstractBuffer GetOfType(Type bufferType)
        {
            lock (m_buffers)
            {
                foreach (AbstractBuffer buffer in m_buffers)
                    if (buffer.GetType().Equals(bufferType))
                        return buffer;
            }
            return null;
        }

        public List<AbstractBuffer> GetAllBuffer()
        {
            List<AbstractBuffer> list = new List<AbstractBuffer>();
            lock (m_lock)
            {
                foreach (AbstractBuffer buffer in m_buffers)
                {
                    list.Add(buffer);
                }
            }
            return list;
        }

        public void Update()
        {
            List<AbstractBuffer> buffers = GetAllBuffer();
            foreach (AbstractBuffer buffer in buffers)
            {
                try
                {
                    if (buffer.Check() == false)
                    {
                        buffer.Stop();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        #endregion

        #region Static Methods CreateBuffer

        /// <summary>
        /// 创建Buff,以天为单位
        /// </summary>
        /// <param name="template"></param>
        /// <param name="ValidDate"></param>
        /// <returns></returns>
        public static AbstractBuffer CreateBuffer(ItemTemplateInfo template  , int ValidDate)
        {
            BufferInfo buffer = new BufferInfo();
            buffer.BeginDate = DateTime.Now;
            buffer.ValidDate = ValidDate * 24 * 60;
            buffer.Value = template.Property2;
            buffer.Type = template.Property1;
            buffer.IsExist = true;
            return CreateBuffer(buffer);
        }
        /// <summary>
        /// 创建Buff,以小时为单位
        /// </summary>
        /// <param name="template"></param>
        /// <param name="ValidHour"></param>
        /// <returns></returns>
        public static AbstractBuffer CreateBufferHour(ItemTemplateInfo template, int ValidHour)
        {
            BufferInfo buffer = new BufferInfo();
            buffer.BeginDate = DateTime.Now;
            buffer.ValidDate = ValidHour * 60;
            buffer.Value = template.Property2;
            buffer.Type = template.Property1;
            buffer.IsExist = true;
            return CreateBuffer(buffer);            
        }

        public static AbstractBuffer CreateBuffer(BufferInfo info)
        {
            AbstractBuffer buffer = null;
            switch (info.Type)
            {
                case 11:
                    buffer = new KickProtectBuffer(info);
                    break;
                case 12:
                    buffer = new OfferMultipleBuffer(info);
                    break;
                case 13:
                    buffer = new GPMultipleBuffer(info);
                    break;
                case 15:
                    buffer = new PropsBuffer(info);
                    break;
            }
            return buffer;
        }



        #endregion
    }
}
