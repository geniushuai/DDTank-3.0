using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base;
using Game.Base.Config;
using System.Net;
using System.Reflection;
using System.IO;

namespace Game.Server
{
    //public class GameServerConfiguration:BaseServerConfiguration
    //{
    //    /// <summary>
    //    /// holds the server root directory
    //    /// </summary>
    //    protected string _rootDirectory;

    //    /// <summary>
    //    /// Holds the log configuration file path
    //    /// </summary>
    //    protected string _logConfigFile;

    //    /// <summary>
    //    /// Name of the scripts compilation target
    //    /// </summary>
    //    protected string _scriptCompilationTarget;

    //    /// <summary>
    //    /// The assemblies to include when compiling the scripts
    //    /// </summary>
    //    protected string _scriptAssemblies;

    //    /// <summary>
    //    /// The game server name
    //    /// </summary>
    //    protected string _serverName;

    //    /// <summary>
    //    /// The short server name, shown in /loc command
    //    /// </summary>
    //    protected string _serverNameShort;

    //    /// <summary>
    //    /// The count of server cpu
    //    /// </summary>
    //    protected int _cpuCount;

    //    /// <summary>
    //    /// The max client count.
    //    /// </summary>
    //    protected int _maxClientCount;

    //    /// <summary>
    //    /// The path to the XML database folder
    //    /// </summary>
    //    protected string _dbConnectionString;

    //    /// <summary>
    //    /// The auto save interval in minutes
    //    /// </summary>
    //    protected int _saveInterval;

    //    /// <summary>
    //    /// The auto check ping.
    //    /// </summary>
    //    protected int _pingCheckInterval;

    //    protected int _loginServerPort;

    //    protected string _loginSeverIp;

    //    protected int _serverId;

    //    #region Load/Save

    //    /// <summary>
    //    /// Loads the config values from a specific config element
    //    /// </summary>
    //    /// <param name="root">the root config element</param>
    //    protected override void LoadFromConfig(ConfigElement root)
    //    {
    //        base.LoadFromConfig(root);


    //        _logConfigFile = root["Server"]["LogConfigFile"].GetString(_logConfigFile);

    //        _scriptCompilationTarget = root["Server"]["ScriptCompilationTarget"].GetString(_scriptCompilationTarget);
    //        _scriptAssemblies = root["Server"]["ScriptAssemblies"].GetString(_scriptAssemblies);
    //        _serverId = root["Server"]["ServerId"].GetInt(_serverId);
    //        _serverName = root["Server"]["ServerName"].GetString(_serverName);
    //        _serverNameShort = root["Server"]["ServerNameShort"].GetString(_serverNameShort);

    //        _dbConnectionString = root["Server"]["DBConnectionString"].GetString(_dbConnectionString);
    //        _saveInterval = root["Server"]["DBAutosaveInterval"].GetInt(_saveInterval);
    //        _pingCheckInterval = root["Server"]["PingCheckInterval"].GetInt(_pingCheckInterval);
    //        _maxClientCount = root["Server"]["MaxClientCount"].GetInt(_maxClientCount);
    //        _cpuCount = root["Server"]["CpuCount"].GetInt(_cpuCount);
    //        if (_cpuCount < 1)
    //            _cpuCount = 1;

    //        _loginServerPort = root["Server"]["LoginServerPort"].GetInt(_loginServerPort);
    //        _loginSeverIp = root["Server"]["LoginServerIp"].GetString(_loginSeverIp);
    //    }

    //    /// <summary>
    //    /// Saves the values into a specific config element
    //    /// </summary>
    //    /// <param name="root">the root config element</param>
    //    protected override void SaveToConfig(ConfigElement root)
    //    {
    //        base.SaveToConfig(root);
    //        root["Server"]["ServerId"].Set(_serverId);
    //        root["Server"]["ServerName"].Set(_serverName);
    //        root["Server"]["ServerNameShort"].Set(_serverNameShort);
    //        root["Server"]["LogConfigFile"].Set(_logConfigFile);

    //        root["Server"]["ScriptCompilationTarget"].Set(_scriptCompilationTarget);
    //        root["Server"]["ScriptAssemblies"].Set(_scriptAssemblies);

    //        root["Server"]["DBConnectionString"].Set(_dbConnectionString);
    //        root["Server"]["DBAutosaveInterval"].Set(_saveInterval);
    //        root["Server"]["PingCheckInterval"].Set(_pingCheckInterval);

    //        root["Server"]["LoginServerPort"].Set(_loginServerPort);
    //        root["Server"]["LoginServerIp"].Set(_loginSeverIp);
    //    }
    //    #endregion
    //    #region Constructors
    //    /// <summary>
    //    /// Constructs a server configuration with default values
    //    /// </summary>
    //    public GameServerConfiguration()
    //        : base()
    //    {
    //        _serverId = 1;
    //        _serverName = "7Road Server";
    //        _serverNameShort = "7Road";
            //if (Assembly.GetEntryAssembly() != null)
            //    _rootDirectory = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            //else
            //    _rootDirectory = new FileInfo(Assembly.GetAssembly(typeof(GameServer)).Location).DirectoryName;

    //        _logConfigFile = "." + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + "logconfig.xml";

    //        _scriptCompilationTarget = "." + Path.DirectorySeparatorChar + "lib" + Path.DirectorySeparatorChar + "GameServerScripts.dll";
    //        _scriptAssemblies = "Game.Base.dll,Road.Database.dll,Game.Server.dll";
    //        _dbConnectionString = "Data Source=192.168.0.4;Initial Catalog=Db_Tank;Persist Security Info=True;User ID=sa;Password=123456;Asynchronous Processing=true;";
    //        _saveInterval = 10;
    //        _pingCheckInterval = 3;
    //        _maxClientCount = 8000;
            
    //        // Get count of CPUs
    //        _cpuCount = Environment.ProcessorCount;
    //        if (_cpuCount < 1)
    //        {
    //            try
    //            {
    //                _cpuCount = int.Parse(Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));
    //            }
    //            catch { _cpuCount = -1; }
    //        }
    //        if (_cpuCount < 1)
    //            _cpuCount = 1;

    //        _loginSeverIp = "127.0.0.1";
    //        _loginServerPort = 9202;
    //    }

    //    #endregion

    //    /// <summary>
    //    /// Gets or sets the root directory of the server
    //    /// </summary>
    //    public string RootDirectory
    //    {
    //        get { return _rootDirectory; }
    //        set { _rootDirectory = value; }
    //    }

    //    /// <summary>
    //    /// Gets or sets the log configuration file of this server
    //    /// </summary>
    //    public string LogConfigFile
    //    {
    //        get
    //        {
    //            if (Path.IsPathRooted(_logConfigFile))
    //                return _logConfigFile;
    //            else
    //                return Path.Combine(_rootDirectory, _logConfigFile);
    //        }
    //        set { _logConfigFile = value; }
    //    }

    //    /// <summary>
    //    /// Gets or sets the script compilation target
    //    /// </summary>
    //    public string ScriptCompilationTarget
    //    {
    //        get { return _scriptCompilationTarget; }
    //        set { _scriptCompilationTarget = value; }
    //    }

    //    /// <summary>
    //    /// Gets or sets the script assemblies to be included in the script compilation
    //    /// </summary>
    //    public string ScriptAssemblies
    //    {
    //        get { return _scriptAssemblies; }
    //        set { _scriptAssemblies = value; }
    //    }

    //    public int ServerId
    //    {
    //        get { return _serverId; }
    //        set { _serverId = value; }
    //    }

    //    /// <summary>
    //    /// Gets or sets the server name
    //    /// </summary>
    //    public string ServerName
    //    {
    //        get { return _serverName; }
    //        set { _serverName = value; }
    //    }

    //    /// <summary>
    //    /// Gets or sets the short server name
    //    /// </summary>
    //    public string ServerNameShort
    //    {
    //        get { return _serverNameShort; }
    //        set { _serverNameShort = value; }
    //    }

    //    /// <summary>
    //    /// Gets or sets the xml database path
    //    /// </summary>
    //    public string DBConnectionString
    //    {
    //        get { return _dbConnectionString; }
    //        set { _dbConnectionString = value; }
    //    }

    //    /// <summary>
    //    /// Gets or sets the autosave interval
    //    /// </summary>
    //    public int SaveInterval
    //    {
    //        get { return _saveInterval; }
    //        set { _saveInterval = value; }
    //    }

    //    /// <summary>
    //    /// Gets or sets the autosave interval
    //    /// </summary>
    //    public int PingCheckInterval
    //    {
    //        get { return _pingCheckInterval; }
    //        set { _pingCheckInterval = value; }
    //    }

    //    /// <summary>
    //    /// Gets or sets the server cpu count
    //    /// </summary>
    //    public int CpuCount
    //    {
    //        get { return _cpuCount; }
    //        set { _cpuCount = value; }
    //    }

    //    /// <summary>
    //    /// Gets or sets the max cout of clients allowed
    //    /// </summary>
    //    public int MaxClientCount
    //    {
    //        get { return _maxClientCount; }
    //        set { _maxClientCount = value; }
    //    }

    //    public string LoginServerIp
    //    {
    //        get
    //        {
    //            return _loginSeverIp;
    //        }
    //        set
    //        {
    //            _loginSeverIp = value;
    //        }
    //    }

    //    public int LoginServerPort
    //    {
    //        get
    //        {
    //            return _loginServerPort;
    //        }
    //        set
    //        {
    //            _loginServerPort = value;
    //        }
    //    }
    //}
}
