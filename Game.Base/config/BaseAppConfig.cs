using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using log4net;

namespace Game.Base.Config
{
    public abstract class BaseAppConfig
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BaseAppConfig() { }

        protected virtual void Load(Type type)
        {
            ConfigurationManager.RefreshSection("appSettings");

            foreach (FieldInfo f in type.GetFields())
            {
                object[] attribs = f.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
                if (attribs.Length == 0)
                    continue;
                ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)attribs[0];
                f.SetValue(this, LoadConfigProperty(attrib));
            }
        }

        private object LoadConfigProperty(ConfigPropertyAttribute attrib)
        {
            string key = attrib.Key;
            string value = ConfigurationSettings.AppSettings[key];
            if (value == null)
            {
                value = attrib.DefaultValue.ToString();
                log.Warn("Loading " + key + " value is null,using default vaule:"+value);
            }
            else
            {
                log.Debug("Loading " + key + " Value is " + value);
            }
            try
            {
                return Convert.ChangeType(value, attrib.DefaultValue.GetType());
            }
            catch (Exception e)
            {
                log.Error("Exception in ServerProperties Load: ", e);
                return null;
            }
        }
    }
}
