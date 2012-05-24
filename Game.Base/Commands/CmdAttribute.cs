using System;

namespace Game.Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CmdAttribute : Attribute
    {
        private string m_cmd;
        private string[] m_cmdAliases;
        private uint m_lvl;
        private string m_description;
        private string[] m_usage;

        public CmdAttribute(string cmd, string[] alias, ePrivLevel lvl, string desc, params string[] usage)
        {
            m_cmd = cmd;
            m_cmdAliases = alias;
            m_lvl = (uint)lvl;
            m_description = desc;
            m_usage = usage;
        }

        public CmdAttribute(string cmd, ePrivLevel lvl, string desc, params string[] usage)
            : this(cmd, null, lvl, desc, usage)
        {
        }

        public string Cmd
        {
            get
            {
                return m_cmd;
            }
        }

        public string[] Aliases
        {
            get
            {
                return m_cmdAliases;
            }
        }

        public uint Level
        {
            get
            {
                return m_lvl;
            }
        }

        public string Description
        {
            get
            {
                return m_description;
            }
        }

        public string[] Usage
        {
            get
            {
                return m_usage;
            }
        }
    }
}