using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Packets;
using Game.Server.ChatServer;
using System.Timers;
using log4net;
using System.Reflection;
using System.Threading;
using Game.Logic;

namespace Game.Server.Managers
{
    public class MacroDropMgr
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected static System.Threading.ReaderWriterLock m_lock = new System.Threading.ReaderWriterLock();

        public static bool Init()
        {
            m_lock = new System.Threading.ReaderWriterLock();
            return ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                DropInfoMgr.DropInfo = new Dictionary<int, MacroDropInfo>();
                return true;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("DropInfoMgr", e);
            }
            return false;
        }

        //每两分钟向服务器端报告一次掉落物品数量
        private static void OnTimeEvent(object source, ElapsedEventArgs e)
        {
            Dictionary<int, int> tempDic = new Dictionary<int, int>();
            m_lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                foreach (KeyValuePair<int, MacroDropInfo> kvp in DropInfoMgr.DropInfo)
                {
                    int templateId = kvp.Key;
                    MacroDropInfo macroDropInfo = kvp.Value;
                    if (macroDropInfo.SelfDropCount > 0)
                    {
                        tempDic.Add(templateId, macroDropInfo.SelfDropCount);
                        macroDropInfo.SelfDropCount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("DropInfoMgr OnTimeEvent", ex);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
            if(tempDic.Count > 0)
            {
                GSPacketIn pkg = new GSPacketIn((byte)eChatServerPacket.MACRO_DROP);
                pkg.WriteInt(tempDic.Count);
                foreach (KeyValuePair<int, int> kvp in tempDic)
                {
                    pkg.WriteInt(kvp.Key);
                    pkg.WriteInt(kvp.Value);
                }
                GameServer.Instance.LoginServer.SendPacket(pkg);
            }
        }

        public static void Start()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(OnTimeEvent);
            timer.Interval = 5000;
            timer.Enabled = true;
        }

        public static void UpdateDropInfo(Dictionary<int, MacroDropInfo> temp)
        {
            m_lock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                foreach (KeyValuePair<int, MacroDropInfo> kvp in temp)
                {
                    if (DropInfoMgr.DropInfo.ContainsKey(kvp.Key))
                    {
                        DropInfoMgr.DropInfo[kvp.Key].DropCount = kvp.Value.DropCount;
                        DropInfoMgr.DropInfo[kvp.Key].MaxDropCount = kvp.Value.MaxDropCount;
                    }
                    else
                    {
                        DropInfoMgr.DropInfo.Add(kvp.Key, kvp.Value);
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("MacroDropMgr UpdateDropInfo", e);
            }
            finally
            {
                m_lock.ReleaseWriterLock();
            }
        }
    }
}
