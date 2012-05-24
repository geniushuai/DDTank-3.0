using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Game.Base.Config;
using System.IO;

namespace Game.Base
{
    /// <summary>
    /// This is a server configuration
    /// </summary>
    public class BaseServerConfiguration
    {
        /// <summary>
        /// The port the server should listen to
        /// </summary>
        protected ushort _port;
        /// <summary>
        /// The ip address the server should use for listening
        /// </summary>
        protected IPAddress _ip;

        /// <summary>
        /// Loads the config values from a specific config element
        /// </summary>
        /// <param name="root">the root config element</param>
        protected virtual void LoadFromConfig(ConfigElement root)
        {
            string ip = root["Server"]["IP"].GetString("any");
            if (ip == "any")
                _ip = IPAddress.Any;
            else
                _ip = IPAddress.Parse(ip);
            _port = (ushort)root["Server"]["Port"].GetInt(_port);

        }

        /// <summary>
        /// Load the configuration from a XML source file
        /// </summary>
        /// <param name="configFile">The file to load from</param>
        public void LoadFromXMLFile(FileInfo configFile)
        {
            XMLConfigFile xmlConfig = XMLConfigFile.ParseXMLFile(configFile);
            LoadFromConfig(xmlConfig);
        }

        /// <summary>
        /// Saves the values into a specific config element
        /// </summary>
        /// <param name="root">the root config element</param>
        protected virtual void SaveToConfig(ConfigElement root)
        {
            root["Server"]["Port"].Set(_port);
            root["Server"]["IP"].Set(_ip);
        }

        /// <summary>
        /// Save the configuration to a XML file
        /// </summary>
        /// <param name="configFile">The file to save</param>
        public void SaveToXMLFile(FileInfo configFile)
        {
            if (configFile == null)
                throw new ArgumentNullException("configFile");

            XMLConfigFile config = new XMLConfigFile();
            SaveToConfig(config);
            config.Save(configFile);
        }

        /// <summary>
        /// Constructs a server configuration with default values
        /// </summary>
        public BaseServerConfiguration()
        {
            _port = 7000;
            _ip = IPAddress.Any;
        }

        /// <summary>
        /// Sets or gets the port for the server
        /// </summary>
        public ushort Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Sets or gets the IP address for the server
        /// </summary>
        public IPAddress Ip
        {
            get { return _ip; }
            set { _ip = value; }
        }
    }
}
