using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using log4net;
using System.IO;
using System.Threading;
using System.Timers;
using Bussiness;
using System.Reflection;
using Game.Base.Packets;

namespace Center.Server.Managers
{
    public class MacroDropMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static System.Threading.ReaderWriterLock m_lock;

        private static Dictionary<int, DropInfo> m_DropInfo;

        private  static string FilePath;

        private static int counter;

        public static bool Init()
        {
            m_lock = new System.Threading.ReaderWriterLock();
            FilePath = Directory.GetCurrentDirectory() + @"\macrodrop\macroDrop.ini";
            return Reload();
        }

        public static bool Reload()
        {
            try
            {
                Dictionary<int, DropInfo> tempInfo = new Dictionary<int, DropInfo>();
                m_DropInfo = new Dictionary<int, DropInfo>();
                tempInfo = LoadDropInfo();
                if (tempInfo != null && tempInfo.Count > 0)
                {
                    Interlocked.Exchange(ref m_DropInfo, tempInfo);
                }
                return true;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("DropInfoMgr", e);
            }
            return false;
        }

        private static void MacroDropReset()
        {
            m_lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                foreach (KeyValuePair<int, DropInfo> kvp in m_DropInfo)
                {
                    int templateId = kvp.Key;
                    DropInfo dropInfo = kvp.Value;
                    if (counter > dropInfo.Time && dropInfo.Time > 0)
                    {
                        if (counter % dropInfo.Time == 0)
                        {
                            dropInfo.Count = dropInfo.MaxCount;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("DropInfoMgr MacroDropReset", e);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
        }

        private static void MacroDropSync()
        {
            bool syncMacroDrop = true;
            ServerClient[] serverClients = CenterServer.Instance.GetAllClients();
            foreach (ServerClient serverClient in serverClients)
            {
                if (!serverClient.NeedSyncMacroDrop)
                {
                    syncMacroDrop = false;
                    break;
                }
            }

            if (serverClients.Length > 0 && syncMacroDrop)
            {
                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.MACRO_DROP);
                int count = m_DropInfo.Count;
                pkg.WriteInt(count);
                m_lock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    foreach (KeyValuePair<int, DropInfo> kvp in m_DropInfo)
                    {
                        DropInfo di = kvp.Value;
                        pkg.WriteInt(di.ID);
                        pkg.WriteInt(di.Count);
                        pkg.WriteInt(di.MaxCount);
                    }
                }
                catch (Exception e)
                {
                    if (log.IsErrorEnabled)
                        log.Error("DropInfoMgr MacroDropReset", e);
                }
                finally
                {
                    m_lock.ReleaseReaderLock();
                }

                foreach (ServerClient serverClient in serverClients)
                {
                    serverClient.NeedSyncMacroDrop = false;
                    serverClient.SendTCP(pkg);
                }
            }
        }

        #region timer
        private static void OnTimeEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            counter++;
            //每小时刷新宏观掉落ini文件
            if (counter % 12 == 0)
            {
                MacroDropReset();
            }

            //同步宏观掉落
            MacroDropSync();
        }

        public static void Start()
        {
            counter = 0;
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(OnTimeEvent);
            timer.Interval = 300000;
            timer.Enabled = true;
        }
        #endregion

        #region LoadFromIni
        private static Dictionary<int, DropInfo> LoadDropInfo()
        {
            Dictionary<int, DropInfo> items = new Dictionary<int, DropInfo>();

            if (File.Exists(FilePath))
            {
                IniReader reader = new IniReader(FilePath);
                int i = 1;
                while (reader.GetIniString(i.ToString(), "TemplateId") != "")
                {
                    string section = i.ToString();
                    int id = Convert.ToInt32(reader.GetIniString(section, "TemplateId"));
                    int time = Convert.ToInt32(reader.GetIniString(section, "Time"));
                    int count = Convert.ToInt32(reader.GetIniString(section, "Count"));
                    DropInfo info = new DropInfo(id, time, count, count);
                    items.Add(info.ID, info);
                    i++;
                }
                return items;
            }

            return null;
        }
        #endregion

        public static void DropNotice(Dictionary<int, int> temp)
        {
            m_lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                foreach (KeyValuePair<int, int> kvp in temp)
                {
                    if (m_DropInfo.ContainsKey(kvp.Key))
                    {
                        DropInfo dropInfo = m_DropInfo[kvp.Key];
                        if (dropInfo.Count > 0)
                        {
                            dropInfo.Count -= kvp.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("DropInfoMgr CanDrop", ex);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
        }
    }
}
