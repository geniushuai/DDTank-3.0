using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Util;
using System.Collections.Specialized;
using Game.Base;
using System.Reflection;

namespace Game.Base.Events
{
    /// <summary>
	/// The callback method for DOLEvents
	/// </summary>
	/// <remarks>Override the EventArgs class to give custom parameters</remarks>
	public delegate void RoadEventHandler(RoadEvent e, object sender, EventArgs arguments);

	/// <summary>
	/// This class represents a collection of event handlers. You can add and remove
	/// handlers from this list and fire events with parameters which will be routed
	/// through all handlers.
	/// </summary>
	/// <remarks>This class is lazy initialized, meaning as long as you don't add any
	/// handlers, the memory usage will be very low!</remarks>
    public class RoadEventHandlerCollection
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// How long to wait for a lock before failing!
        /// </summary>
        protected const int TIMEOUT = 3000;
        /// <summary>
        /// A reader writer lock used to lock event list
        /// </summary>
        protected readonly System.Threading.ReaderWriterLock m_lock = null;
        /// <summary>
        ///We use a HybridDictionary here to hold all event delegates
        /// </summary>
        protected readonly HybridDictionary m_events = null;

        /// <summary>
        /// Constructs a new DOLEventHandler collection
        /// </summary>
        public RoadEventHandlerCollection()
        {
            m_lock = new System.Threading.ReaderWriterLock();
            m_events = new HybridDictionary();
        }
        /// <summary>
        /// Adds an event handler to the list
        /// </summary>
        /// <param name="e">The event from which we add a handler</param>
        /// <param name="del">The callback method</param>
        public void AddHandler(RoadEvent e, RoadEventHandler del)
        {
            try
            {
                m_lock.AcquireWriterLock(TIMEOUT);
                try
                {
                    WeakMulticastDelegate deleg = (WeakMulticastDelegate)m_events[e];
                    if (deleg == null)
                    {
                        m_events[e] = new WeakMulticastDelegate(del);
                    }
                    else
                    {
                        m_events[e] = WeakMulticastDelegate.Combine(deleg, del);
                    }
                }
                finally
                {
                    m_lock.ReleaseWriterLock();
                }
            }
            catch (ApplicationException ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Failed to add event handler!", ex);
            }
        }

        /// <summary>
        /// Adds an event handler to the list, if it's already added do nothing
        /// </summary>
        /// <param name="e">The event from which we add a handler</param>
        /// <param name="del">The callback method</param>
        public void AddHandlerUnique(RoadEvent e, RoadEventHandler del)
        {
            try
            {
                m_lock.AcquireWriterLock(TIMEOUT);
                try
                {
                    WeakMulticastDelegate deleg = (WeakMulticastDelegate)m_events[e];
                    if (deleg == null)
                    {
                        m_events[e] = new WeakMulticastDelegate(del);
                    }
                    else
                    {
                        m_events[e] = WeakMulticastDelegate.CombineUnique(deleg, del);
                    }
                }
                finally
                {
                    m_lock.ReleaseWriterLock();
                }
            }
            catch (ApplicationException ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Failed to add event handler!", ex);
            }
        }

        /// <summary>
        /// Removes an event handler from the list
        /// </summary>
        /// <param name="e">The event from which to remove the handler</param>
        /// <param name="del">The callback method to remove</param>
        public void RemoveHandler(RoadEvent e, RoadEventHandler del)
        {
            try
            {
                m_lock.AcquireWriterLock(TIMEOUT);
                try
                {
                    WeakMulticastDelegate deleg = (WeakMulticastDelegate)m_events[e];
                    if (deleg != null)
                    {
                        deleg = WeakMulticastDelegate.Remove(deleg, del);
                        if (deleg == null)
                            m_events.Remove(e);
                        else
                            m_events[e] = deleg;
                    }
                }
                finally
                {
                    m_lock.ReleaseWriterLock();
                }
            }
            catch (ApplicationException ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Failed to remove event handler!", ex);
            }
        }

        /// <summary>
        /// Removes all callback handlers for a given event
        /// </summary>
        /// <param name="e">The event from which to remove all handlers</param>
        public void RemoveAllHandlers(RoadEvent e)
        {
            try
            {
                m_lock.AcquireWriterLock(TIMEOUT);
                try
                {
                    m_events.Remove(e);
                }
                finally
                {
                    m_lock.ReleaseWriterLock();
                }
            }
            catch (ApplicationException ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Failed to remove event handlers!", ex);
            }
        }

        /// <summary>
        /// Removes all event handlers
        /// </summary>
        public void RemoveAllHandlers()
        {
            try
            {
                m_lock.AcquireWriterLock(TIMEOUT);
                try
                {
                    m_events.Clear();
                }
                finally
                {
                    m_lock.ReleaseWriterLock();
                }
            }
            catch (ApplicationException ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Failed to remove all event handlers!", ex);
            }
        }

        /// <summary>
        /// Notifies all registered event handlers of the occurance of an event!
        /// </summary>
        /// <param name="e">The event that occured</param>
        public void Notify(RoadEvent e)
        {
            Notify(e, null, null);
        }


        /// <summary>
        /// Notifies all registered event handlers of the occurance of an event!
        /// </summary>
        /// <param name="e">The event that occured</param>
        /// <param name="sender">The sender of this event</param>
        public void Notify(RoadEvent e, object sender)
        {
            Notify(e, sender, null);
        }

        /// <summary>
        /// Notifies all registered event handlers of the occurance of an event!
        /// </summary>
        /// <param name="e">The event that occured</param>
        /// <param name="args">The event arguments</param>
        public void Notify(RoadEvent e, EventArgs args)
        {
            Notify(e, null, args);
        }

        /// <summary>
        /// Notifies all registered event handlers of the occurance of an event!
        /// </summary>
        /// <param name="e">The event that occured</param>
        /// <param name="sender">The sender of this event</param>
        /// <param name="eArgs">The event arguments</param>
        /// <remarks>Overwrite the EventArgs class to set own arguments</remarks>
        public void Notify(RoadEvent e, object sender, EventArgs eArgs)
        {
            try
            {
                m_lock.AcquireReaderLock(TIMEOUT);
                WeakMulticastDelegate eventDelegate;
                try
                {
                    eventDelegate = (WeakMulticastDelegate)m_events[e];
                }
                finally
                {
                    m_lock.ReleaseReaderLock();
                }
                if (eventDelegate == null) return;
                eventDelegate.InvokeSafe(new object[] { e, sender, eArgs });
            }
            catch (ApplicationException ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Failed to notify event handler!", ex);
            }
        }
    }
}
