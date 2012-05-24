using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Game.Service
{
    /// <summary>
    /// Zusammenfassung f黵 GameServerServiceInstaller.
    /// </summary>
    [RunInstaller(true)]
    public class GameServerServiceInstaller : Installer
    {
        private ServiceInstaller m_gameServerServiceInstaller;
        private ServiceProcessInstaller m_gameServerServiceProcessInstaller;

        public GameServerServiceInstaller()
        {
            // Instantiate installers for process and services.
            m_gameServerServiceProcessInstaller = new ServiceProcessInstaller();
            m_gameServerServiceProcessInstaller.Account = ServiceAccount.LocalSystem;

            m_gameServerServiceInstaller = new ServiceInstaller();
            m_gameServerServiceInstaller.StartType = ServiceStartMode.Manual;
            m_gameServerServiceInstaller.ServiceName = "ROAD";

            Installers.Add(m_gameServerServiceProcessInstaller);
            Installers.Add(m_gameServerServiceInstaller);
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            Context.Parameters["assemblyPath"] += " --SERVICERUN " + Context.Parameters["commandline"];
            base.Install(stateSaver);
        }

    }
}
