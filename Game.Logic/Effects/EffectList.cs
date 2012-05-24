using System;
using System.Collections;
using System.Reflection;
using Bussiness.Managers;
using SqlDataProvider.Data;
using log4net;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace Game.Logic.Effects
{

    public class EffectList
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected ArrayList m_effects;

        protected readonly Living m_owner;

        protected volatile sbyte m_changesCount;

        protected int m_immunity;


        public ArrayList List
        {
            get { return m_effects; }
        }
        public EffectList(Living owner, int immunity)
        {
            m_owner = owner;
            m_effects = new ArrayList(3);
            m_immunity = immunity;
        }

        public bool CanAddEffect(int id)
        {
            if (id > 35 || id < 0)
            {
                return true;
            }
            else
            {
                return ((0x0001 << (id - 1)) & m_immunity) == 0;
            }
        }

        public virtual bool Add(AbstractEffect effect)
        {
            if (CanAddEffect(effect.TypeValue))
            {
                lock (m_effects)
                {
                    m_effects.Add(effect);
                }

                effect.OnAttached(m_owner);

                OnEffectsChanged(effect);

                return true;
            }

            if (effect.TypeValue == (int)eEffectType.IceFronzeEffect && m_owner is SimpleBoss)
            {
                m_owner.State = 0;
            }

            return false;
        }

        public virtual bool Remove(AbstractEffect effect)
        {
            int index = -1;
            lock (m_effects)
            {
                index = m_effects.IndexOf(effect);
                if (index < 0)
                    return false;
                m_effects.RemoveAt(index);
            }

            if (index != -1)
            {
                effect.OnRemoved(m_owner);
                OnEffectsChanged(effect);
                return true;
            }
            else
            {
                return false;
            }
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

        public virtual AbstractEffect GetOfType(eEffectType effectType)
        {
            lock (m_effects)
            {
                foreach (AbstractEffect effect in m_effects)
                    if (effect.Type == effectType) return effect;
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
