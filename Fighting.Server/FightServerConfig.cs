using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using Game.Base.Config;
using System.Net;
using System.Configuration;

namespace Fighting.Server
{
    public class FightServerConfig
    {
        public string LogConfigFile = "logconfig.xml";

        public IPAddress Ip = IPAddress.Any;

        public int Port = 9208;
        public int ZoneId = 1;
        public void Load()
        {
            LogConfigFile = ConfigurationSettings.AppSettings["Logconfig"];
            Ip = IPAddress.Parse(ConfigurationSettings.AppSettings["Ip"]);
            Port = int.Parse(ConfigurationSettings.AppSettings["Port"]);
            ZoneId = int.Parse(ConfigurationSettings.AppSettings["ServerID"]);
        }

    }
}
