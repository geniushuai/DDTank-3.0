using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using log4net;
using System.Reflection;

namespace Bussiness
{
    public class StaticFunction
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool UpdateConfig(string fileName, string name, string value)
        {
            try
            {
                ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
                filemap.ExeConfigFilename = fileName;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None);
                config.AppSettings.Settings[name].Value = value;
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("UpdateConfig", e);
            }

            return false;

        }
    }
}
