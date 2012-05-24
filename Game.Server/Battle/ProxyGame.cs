using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base;
using Game.Logic;
using Game.Base.Packets;
using Game.Server.Managers;

namespace Game.Server.Battle
{
    public class ProxyGame:AbstractGame
    {

        private FightServerConnector m_fightingServer;

        public ProxyGame(int id,FightServerConnector fightServer,eRoomType roomType, eGameType gameType,int timeType)
            :base(id,roomType,gameType,timeType)
        {
            m_fightingServer = fightServer;
            m_fightingServer.Disconnected += new Game.Base.ClientEventHandle(m_fightingServer_Disconnected);
        }

        void m_fightingServer_Disconnected(BaseClient client)
        {
            Stop();
        }

        public override void ProcessData(GSPacketIn pkg)
        {
            m_fightingServer.SendToGame(Id, pkg);
        }
    }
}
