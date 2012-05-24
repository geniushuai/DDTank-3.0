using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Base
{
    /// <summary>
    /// This class is a weakreference wrapper
    /// because mono gc crashes with null targets
    /// </summary>
    public class WeakRef : WeakReference
    {
        private class NullValue { };
        private static readonly NullValue NULL = new NullValue();

        /// <summary>
        /// Creates a new weak reference wrapper for MONO because
        /// MONO gc crashes with null targets
        /// </summary>
        /// <param name="target">The target of this weak reference</param>
        public WeakRef(object target)
            : base((target == null) ? NULL : target)
        {
        }

        /// <summary>
        /// Creates a new weak reference wrapper for MONO because
        /// MONO gc crashes with null targets
        /// </summary>
        /// <param name="target">The target of this weak reference</param>
        /// <param name="trackResurrection">Track the resurrection of the target</param>
        public WeakRef(object target, bool trackResurrection)
            : base((target == null) ? NULL : target, trackResurrection)
        {
        }

        /// <summary>
        /// Gets or sets the target of this weak reference
        /// </summary>
        public override object Target
        {
            get
            {
                object o = base.Target;
                return ((o == NULL) ? null : o);
            }
            set
            {
                base.Target = (value == null) ? NULL : value;
            }
        }
    }
}
