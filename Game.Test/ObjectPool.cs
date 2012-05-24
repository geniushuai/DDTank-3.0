//#define DISABLE_POOL
using System;
using System.Collections.Generic;
using System.Text;

namespace Road.Test
{
    public interface IObjectPoolMethods : IDisposable
    {
        /// <summary>
        /// This method is used to setup an instance of
        /// an object.
        /// </summary>
        /// <remarks>
        /// Use an empty constructor in the class and
        /// entirely rely on this method to setup and
        /// initialize the object if the class is going
        /// to be used for object pooling. Do not call this
        /// method from the constructor or it will be called
        /// twice and will affect performance.
        /// </remarks>
        /// <param name="setupParameters"></param>
        void SetupObject(params object[] setupParameters);
        /// <summary>
        /// This event must be fired when dispose is called
        /// or the Object Pool will not be able to work
        /// </summary>
        event EventHandler Disposing;
    }

    /// <summary>
    /// A generic class that provide object pooling functionality
    /// to classes that implement the IObjectPoolMethods interface
    /// </summary>
    /// <remarks>
    /// As long as the lock(this) statements remain this class is thread
    /// </remarks>
    /// <typeparam name="T">
    /// The type of the object that should be pooled
    /// </typeparam>
    public class ObjectPool<T> where T : IObjectPoolMethods, new()
    {
        /// <summary>
        /// The MAX_POOL does not restrict the maximum
        /// number of objects in the object pool
        /// instead it restricts the number of
        /// disposed objects that the pool will maintain
        /// a reference too and attempt to pool. This
        /// number should be set based on a memory and
        /// usage anaylsis based on the types of objects
        /// being pooled. I have found the best results
        /// by setting this number to average peak objects
        /// in use at one time.
        /// </summary>
        /// <value>100</value>
        private const int MAX_POOL = 50000;
        /// <summary>
        /// The static instance of the object pool. Thankfully the
        /// use of generics eliminates the need for a hash for each
        /// different pool type
        /// </summary>
        private static ObjectPool<T> me = new ObjectPool<T>();
        /// <summary>
        /// Using a member for the max pool count allows different
        /// types to have a different value based on usage analysis
        /// </summary>
        private int mMaxPool = ObjectPool<T>.MAX_POOL;
        /// <summary>
        /// A Linked List of the WeakReferences to the objects in the pool
        /// When the count of the list goes beyond the max pool count
        /// items are removed from the end of the list. The objects at the
        /// end of the list are also most likely to have already 
        /// been collected by the garbage collector.
        /// </summary>
        private LinkedList<WeakReference> objectPool =
                new LinkedList<WeakReference>();
        /// <summary>
        /// Return the singleton instance
        /// </summary>
        /// <returns>
        /// The single instance allowed for the given type
        /// </returns>
        public static ObjectPool<T> GetInstance()
        {
            return me;
        }
        /// <summary>
        /// This method gets called on the object disposing
        /// event for objects created from this object pool class.
        /// If the implementing class does not fire this event on
        /// dispose the object pooling will not work
        /// </summary>
        /// <param name="sender">
        /// The class instance that is pooled
        /// </param>
        /// <param name="e">
        /// An empty event args parameters
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the type of the object in the weak reference 
        /// does not match the type of this generic instance
        /// </exception>
        private void Object_Disposing(object sender, EventArgs e)
        {
            //The lock is required here because this method may
            //be called from events on different threads
            lock (this)
            {
                Add(new WeakReference(sender));
            }
        }
        /// <summary>
        /// This method will add a new instance to be tracked by
        /// this object pooling class. This should be a reference
        /// to an object that was just disposed otherwise it makes no
        /// sense and will likely cause some serious problems.
        /// </summary>
        /// <param name="weak">
        /// WeakReference to the object that was disposed
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the type of the object in the weak reference 
        /// does not match the type of this generic instance
        /// </exception>
        private void Add(WeakReference weak)
        {
            objectPool.AddFirst(weak);
            if (objectPool.Count >= mMaxPool)
            {
                objectPool.RemoveLast();
            }
        }

        /// <summary>
        /// Remove the reference from the pool. This should
        /// only be called when the object in the pool
        /// is being reactivated or if the weak reference has
        /// been determined to be expired.
        /// </summary>
        /// <param name="weak">
        /// A reference to remove
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the type of the object in the weak reference 
        /// does not match the type of this generic instance
        /// </exception>
        private void Remove(WeakReference weak)
        {
            objectPool.Remove(weak);
        }

        ///<summary>
        /// This method will verify that the type of the weak
        /// reference is valid for this generic instance. I haven't
        /// figured out a way do it automatically yet
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the type of the object in the weak reference 
        /// does not match the type of this generic instance
        /// </exception>
        private void TypeCheck(WeakReference weak)
        {
            if (weak.Target != null &&
                weak.Target.GetType() != typeof(T))
            {
                throw new ArgumentException
                    ("Target type does not match pool type", "weak");
            }
        }

        /// <summary>
        /// This method will return an object reference that is fully
        /// setup. It will either retrieve the object from the object
        /// pool if there is a disposed object available or it will
        /// create a new instance.
        /// </summary>
        /// <param name="setupParameters">
        /// The setup parameters required for the
        /// IObjectPoolMethods.SetupObject method. Need to find a
        /// way to check for all of the types efficiently and at
        /// compile time.
        /// </param>
        /// <returns>
        /// The reference to the object
        /// </returns>
        public T GetObjectFromPool(params object[] setupParameters)
        {
            T result = default(T);
#if DISABLE_POOL
            result = new T();
            result.SetupObject(setupParameters);
            return result;
#else
            lock (this)
            {
                WeakReference remove = null;
                foreach (WeakReference weak in objectPool)
                {
                    object o = weak.Target;
                    if (o != null)
                    {
                        remove = weak;
                        result = (T)o;
                        GC.ReRegisterForFinalize(result);
                        break;
                    }
                }
                if (remove != null)
                {
                    objectPool.Remove(remove);
                }
            }
            if (result == null)
            {
                result = new T();
                result.Disposing += new EventHandler(Object_Disposing);
            }
            result.SetupObject(setupParameters);
            return result;
#endif
        }

        public void Setup()
        {
            for (int i = 0; i < 50000; i++)
            {
                T item = new T();
                item.Disposing += new EventHandler(Object_Disposing);
                item.Dispose();
            }
        }
    }
}
