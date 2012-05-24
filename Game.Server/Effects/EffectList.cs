using System;
using System.Collections;
using System.Reflection;

using log4net;
using Phy.Object;

namespace Game.Server.Effects
{

	public class EffectList 
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected ArrayList m_effects;

		protected readonly Player m_owner;

		protected volatile sbyte m_changesCount;

		public EffectList(Player owner)
		{
			m_owner = owner;
            m_effects = new ArrayList(3);
		}

		public virtual bool Add(AbstractEffect effect)
		{
			lock (m_effects)
			{
				m_effects.Add(effect);
			}

			OnEffectsChanged(effect);

			return true;
		}

		public virtual bool Remove(AbstractEffect effect)
		{
			lock (m_effects) 
			{
				int index = m_effects.IndexOf(effect);
				if (index < 0)
					return false;
				m_effects.RemoveAt(index);
			}

			BeginChanges();
			OnEffectsChanged(effect);
			CommitChanges();
			return true;
		}

		public virtual void OnEffectsChanged(AbstractEffect changedEffect)
		{
			if (m_changesCount > 0)
				return;
			UpdateChangedEffects();
		}

		public void BeginChanges()
		{
			m_changesCount++;
		}

		public virtual void CommitChanges()
		{
			bool update;

			if (--m_changesCount < 0)
			{
				if (log.IsWarnEnabled)
					log.Warn("changes count is less than zero, forgot BeginChanges()?\n" + Environment.StackTrace);
				m_changesCount = 0;
			}

			update = m_changesCount == 0;

			if (update)
				UpdateChangedEffects();
		}

		protected virtual void UpdateChangedEffects()
		{
		}

		public virtual AbstractEffect GetOfType(Type effectType)
		{
			lock (m_effects)
			{
                foreach (AbstractEffect effect in m_effects)
					if (effect.GetType().Equals(effectType)) return effect;
			}
			return null;
		}

		public virtual IList GetAllOfType(Type effectType)
		{
			ArrayList list = new ArrayList();
			lock (m_effects) 
			{
				foreach (AbstractEffect effect in m_effects)
					if (effect.GetType().Equals(effectType)) list.Add(effect);
			}
			return list;
		}

        public void StopEffect(Type effectType)
        {
            IList fx = GetAllOfType(effectType);
            BeginChanges();
            foreach (AbstractEffect effect in fx)
            {
                effect.Stop();
            }
            CommitChanges();
        }
    }
}
