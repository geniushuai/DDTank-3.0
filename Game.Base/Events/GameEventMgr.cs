using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Collections.Specialized;
using log4net.Util;
using System.Reflection;
using System.Threading;

namespace Game.Base.Events
{
    /// <summary>
    /// This manager is used to handle all events of the game
    /// and of individual GameObjects
    /// </summary>
    public sealed class GameEventMgr
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Holds a list of event handler collections for single gameobjects
        /// </summary>
        private static readonly HybridDictionary m_GameObjectEventCollections = new HybridDictionary();
        /// <summary>
        /// A lock used to access the event collections of livings
        /// </summary>
        private static readonly System.Threading.ReaderWriterLock m_lock = new System.Threading.ReaderWriterLock();
        /// <summary>
        /// Timeout for lock operations
        /// </summary>
        private const int TIMEOUT = 3000;

        /// <summary>
        /// Holds a list of all global eventhandlers
        /// </summary>
        private static RoadEventHandlerCollection m_GlobalHandlerCollection = new RoadEventHandlerCollection();

        /// <summary>
        /// Registers some global events that are specified by attributes
        /// </summary>
        /// <param name="asm">The assembly to search through</param>
        /// <param name="attribute">The custom attribute to search for</param>
        /// <param name="e">The event to register in case the custom attribute is found</param>
        /// <exception cref="ArgumentNullException">If one of the parameters is null</exception>
        public static void RegisterGlobalEvents(Assembly asm, Type attribute, RoadEvent e)
        {
            if (asm == null)
                throw new ArgumentNullException("asm", "No assembly given to search for global event handlers!");
            if (attribute == null)
                throw new ArgumentNullException("attribute", "No attribute given!");
            if (e == null)
                throw new ArgumentNullException("e", "No event type given!");


            foreach (Type type in asm.GetTypes())
            {
                if (!type.IsClass) continue;
                foreach (MethodInfo mInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    object[] myAttribs = mInfo.GetCustomAttributes(attribute, false);
                    if (myAttribs.Length != 0)
                    {
                        try
                        {
                            m_GlobalHandlerCollection.AddHandler(e, (RoadEventHandler)Delegate.CreateDelegate(typeof(RoadEventHandler), mInfo));
                        }
                        catch (Exception ex)
                        {
                            if (log.IsErrorEnabled)
                                log.Error("Error registering global event. Method: " + type.FullName + "." + mInfo.Name, ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Registers a single global event handler.
        /// The global event handlers will be called for ALL events,
        /// so use them wisely, they might incure a big performance
        /// hit if used unwisely.
        /// </summary>
        /// <param name="e">The event type to register for</param>
        /// <param name="del">The event handler to register for this event type</param>
        /// <exception cref="ArgumentNullException">If one of the parameters is null</exception>
        public static void AddHandler(RoadEvent e, RoadEventHandler del)
        {
            AddHandler(e, del, false);
        }

        /// <summary>
        /// Registers a single global event handler.
        /// The global event handlers will be called for ALL events,
        /// so use them wisely, they might incure a big performance
        /// hit if used unwisely.
        /// If an equal handler has already been added, nothing will be done
        /// </summary>
        /// <param name="e">The event type to register for</param>
        /// <param name="del">The event handler to register for this event type</param>
        /// <exception cref="ArgumentNullException">If one of the parameters is null</exception>
        public static void AddHandlerUnique(RoadEvent e, RoadEventHandler del)
        {
            AddHandler(e, del, true);
        }

        /// <summary>
        /// Registers a single global event handler.
        /// The global event handlers will be called for ALL events,
        /// so use them wisely, they might incure a big performance
        /// hit if used unwisely.
        /// </summary>
        /// <param name="e">The event type to register for</param>
        /// <param name="del">The event handler to register for this event type</param>
        /// <param name="unique">Flag wether event shall be added unique or not</param>
        /// <exception cref="ArgumentNullException">If one of the parameters is null</exception>
        private static void AddHandler(RoadEvent e, RoadEventHandler del, bool unique)
        {
            if (e == null)
                throw new ArgumentNullException("e", "No event type given!");
            if (del == null)
                throw new ArgumentNullException("del", "No event handler given!");

            if (unique)
                m_GlobalHandlerCollection.AddHandlerUnique(e, del);
            else
                m_GlobalHandlerCollection.AddHandler(e, del);
        }

        /// <summary>
        /// Registers a single local event handler.
        /// Local event handlers will only be called if the
        /// "sender" parameter in the Notify method equals
        /// the object for which a local event handler was
        /// registered.
        /// </summary>
        /// <remarks>
        /// Certain events will never have a local event handler.
        /// This happens if the Notify method is called without
        /// a sender parameter for example!
        /// </remarks>
        /// <param name="obj">The object that needs to be the sender of events</param>
        /// <param name="e">The event type to register for</param>
        /// <param name="del">The event handler to register for this event type</param>
        /// <exception cref="ArgumentNullException">If one of the parameters is null</exception>
        public static void AddHandler(object obj, RoadEvent e, RoadEventHandler del)
        {
            AddHandler(obj, e, del, false);
        }

        /// <summary>
        /// Registers a single local event handler.
        /// Local event handlers will only be called if the
        /// "sender" parameter in the Notify method equals
        /// the object for which a local event handler was
        /// registered.
        /// If a equal handler has already been added, nothin will be done
        /// </summary>
        /// <remarks>
        /// Certain events will never have a local event handler.
        /// This happens if the Notify method is called without
        /// a sender parameter for example!
        /// </remarks>
        /// <param name="obj">The object that needs to be the sender of events</param>
        /// <param name="e">The event type to register for</param>
        /// <param name="del">The event handler to register for this event type</param>
        /// <exception cref="ArgumentNullException">If one of the parameters is null</exception>
        public static void AddHandlerUnique(object obj, RoadEvent e, RoadEventHandler del)
        {
            AddHandler(obj, e, del, true);
        }

        /// <summary>
        /// Registers a single local event handler.
        /// Local event handlers will only be called if the
        /// "sender" parameter in the Notify method equals
        /// the object for which a local event handler was
        /// registered.
        /// </summary>
        /// <remarks>
        /// Certain events will never have a local event handler.
        /// This happens if the Notify method is called without
        /// a sender parameter for example!
        /// </remarks>
        /// <param name="obj">The object that needs to be the sender of events</param>
        /// <param name="e">The event type to register for</param>
        /// <param name="del">The event handler to register for this event type</param>
        /// <param name="unique">Flag wether event shall be added unique or not</param>
        /// <exception cref="ArgumentNullException">If one of the parameters is null</exception>
        private static void AddHandler(object obj, RoadEvent e, RoadEventHandler del, bool unique)
        {
            //Test the parameters
            if (obj == null)
                throw new ArgumentNullException("obj", "No object given!");
            if (e == null)
                throw new ArgumentNullException("e", "No event type given!");
            if (del == null)
                throw new ArgumentNullException("del", "No event handler given!");

            if (!e.IsValidFor(obj))
                throw new ArgumentException("Object is not valid for this event type", "obj");

            try
            {
                m_lock.AcquireReaderLock(TIMEOUT);
                try
                {
                    RoadEventHandlerCollection col = (RoadEventHandlerCollection)m_GameObjectEventCollections[obj];
                    if (col == null)
                    {
                        col = new RoadEventHandlerCollection();
                        LockCookie lc = m_lock.UpgradeToWriterLock(TIMEOUT);
                        try
                        {
                            m_GameObjectEventCollections[obj] = col;
                        }
                        finally
                        {
                            m_lock.DowngradeFromWriterLock(ref lc);
                        }
                    }
                    if (unique)
                        col.AddHandlerUnique(e, del);
                    else
                        col.AddHandler(e, del);
                }
                finally
                {
                    m_lock.ReleaseReaderLock();
                }
            }
            catch (ApplicationException ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Failed to add local event handler!", ex);
            }
        }

        /// <summary>
        /// Removes a global event handler.
        /// You need to have registered the event before being able to remove it.
        /// </summary>
        /// <param name="e">The event type from which to deregister</param>
        /// <param name="del">The event handler to deregister for this event type</param>
        /// <exception cref="ArgumentNullException">If one of the parameters is null</exception>
        public static void RemoveHandler(RoadEvent e, RoadEventHandler del)
        {
            //Test the parameters
            if (e == null)
                throw new ArgumentNullException("e", "No event type given!");
            if (del == null)
                throw new ArgumentNullException("del", "No event handler given!");
            m_GlobalHandlerCollection.RemoveHandler(e, del);
        }

        /// <summary>
        /// Removes a single event handler from an object.
        /// You need to have registered the event before being
        /// able to remove it.
        /// </summary>
        /// <param name="obj">The object that needs to be the sender of events</param>
        /// <param name="e">The event type from which to deregister</param>
        /// <param name="del">The event handler to deregister for this event type</param>
        /// <exception cref="ArgumentNullException">If one of the parameters is null</exception>
        public static void RemoveHandler(object obj, RoadEvent e, RoadEventHandler del)
        {
            //Test the parameters
            if (obj == null)
                throw new ArgumentNullException("obj", "No object given!");
            if (e == null)
                throw new ArgumentNullException("e", "No event type given!");
            if (del == null)
                throw new ArgumentNullException("del", "No event handler given!");

            try
            {
                m_lock.AcquireReaderLock(TIMEOUT);
                try
                {
                    RoadEventHandlerCollection col = (RoadEventHandlerCollection)m_GameObjectEventCollections[obj];
                    if (col != null)
                        col.RemoveHandler(e, del);
                }
                finally
                {
                    m_lock.ReleaseReaderLock();
                }
            }
            catch (ApplicationException ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Failed to remove local event handler!", ex);
            }
        }

        /// <summary>
        /// Removes all global event handlers for a specific event type
        /// </summary>
        /// <param name="e">The event type from which to deregister all handlers</param>
        /// <param name="deep">Specifies if all local registered event handlers
        /// should be removed as well.</param>
        /// <exception cref="ArgumentNullException">If no event type given</exception>
        public static void RemoveAllHandlers(RoadEvent e, bool deep)
        {
            //Test the parameters
            if (e == null)
                throw new ArgumentNullException("e", "No event type given!");

            if (deep)
            {
                try
                {
                    m_lock.AcquireReaderLock(TIMEOUT);
                    try
                    {
                        foreach (RoadEventHandlerCollection col in m_GameObjectEventCollections.Values)
                            col.RemoveAllHandlers(e);
                    }
                    finally
                    {
                        m_lock.ReleaseReaderLock();
                    }
                }
                catch (ApplicationException ex)
                {
                    if (log.IsErrorEnabled)
                        log.Error("Failed to add local event handlers!", ex);
                }
            }
            m_GlobalHandlerCollection.RemoveAllHandlers(e);
        }

        /// <summary>
        /// Removes all event handlers of a specific type
        /// from a specific object
        /// </summary>
        /// <param name="obj">The object from which to remove the handlers</param>
        /// <param name="e">The event type from which to deregister all handlers</param>
        /// <exception cref="ArgumentNullException">If no event type given</exception>
        public static void RemoveAllHandlers(object obj, RoadEvent e)
        {
            //Test the parameters
            if (obj == null)
                throw new ArgumentNullException("obj", "No object given!");
            if (e == null)
                throw new ArgumentNullException("e", "No event type given!");

            try
            {
                m_lock.AcquireReaderLock(TIMEOUT);
                try
                {
                    RoadEventHandlerCollection col = (RoadEventHandlerCollection)m_GameObjectEventCollections[obj];
                    if (col != null)
                        col.RemoveAllHandlers(e);
                }
                finally
                {
                    m_lock.ReleaseReaderLock();
                }
            }
            catch (ApplicationException ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Failed to remove local event handlers!", ex);
            }
        }

        /// <summary>
        /// Removes all event handlers
        /// </summary>
        /// <param name="deep">Specifies if all local registered event handlers
        /// should also be removed</param>
        public static void RemoveAllHandlers(bool deep)
        {
            if (deep)
            {
                try
                {
                    m_lock.AcquireWriterLock(TIMEOUT);
                    try
                    {
                        m_GameObjectEventCollections.Clear();
                    }
                    finally
                    {
                        m_lock.ReleaseWriterLock();
                    }
                }
                catch (ApplicationException ex)
                {
                    if (log.IsErrorEnabled)
                        log.Error("Failed to remove all local event handlers!", ex);
                }
            }
            m_GlobalHandlerCollection.RemoveAllHandlers();
        }

        /// <summary>
        /// Notifies all global event handlers of the occurance of a specific
        /// event type.
        /// </summary>
        /// <param name="e">The event type that occured</param>
        /// <exception cref="ArgumentNullException">If no event type given</exception>
        public static void Notify(RoadEvent e)
        {
            Notify(e, null, null);
        }

        /// <summary>
        /// Notifies all global and local event handlers of the occurance
        /// of a specific event type.
        /// </summary>
        /// <param name="e">The event type that occured</param>
        /// <param name="sender">The sender of this event</param>
        /// <exception cref="ArgumentNullException">If no event type given</exception>
        public static void Notify(RoadEvent e, object sender)
        {
            Notify(e, sender, null);
        }

        /// <summary>
        /// Notifies all global event handlers of the occurance
        /// of a specific event type with some event arguments.
        /// </summary>
        /// <param name="e">The event type that occured</param>
        /// <param name="args">The event arguments</param>
        /// <exception cref="ArgumentNullException">If no event type given</exception>
        public static void Notify(RoadEvent e, EventArgs args)
        {
            Notify(e, null, args);
        }

        /// <summary>
        /// Notifies all global and local event handlers of the occurance 
        /// of a specific event type with some event arguments!
        /// </summary>
        /// <param name="e">The event type that occured</param>
        /// <param name="sender">The sender of this event</param>
        /// <param name="eArgs">The event arguments</param>
        /// <exception cref="ArgumentNullException">If no event type given</exception>
        /// <remarks>Overwrite the EventArgs class to set own arguments</remarks>
        public static void Notify(RoadEvent e, object sender, EventArgs eArgs)
        {
            //Test the parameters
            if (e == null)
                throw new ArgumentNullException("e", "No event type given!");

            //Notify the local event handlers first
            if (sender != null)
            {
                try
                {
                    RoadEventHandlerCollection col = null;
                    m_lock.AcquireReaderLock(TIMEOUT);
                    try
                    {
                        col = (RoadEventHandlerCollection)m_GameObjectEventCollections[sender];
                    }
                    finally
                    {
                        m_lock.ReleaseReaderLock();
                    }
                    if (col != null)
                        col.Notify(e, sender, eArgs);
                }
                catch (ApplicationException ex)
                {
                    if (log.IsErrorEnabled)
                        log.Error("Failed to notify local event handler!", ex);
                }
            }
            //Notify the global ones later
            m_GlobalHandlerCollection.Notify(e, sender, eArgs);
        }
    }
}
