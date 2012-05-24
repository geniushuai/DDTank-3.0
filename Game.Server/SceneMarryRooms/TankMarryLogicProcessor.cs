using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using System.Collections;
using log4net;
using System.Reflection;
using Game.Server.Managers;
using System.Drawing;
using Game.Server.SceneMarryRooms.TankHandle;

namespace Game.Server.SceneMarryRooms
{
    [MarryProcessor(9, "礼堂逻辑")]
    public class TankMarryLogicProcessor : AbstractMarryProcessor
    {

        public TankMarryLogicProcessor()
        {
            _commandMgr = new MarryCommandMgr();
        }

        private MarryCommandMgr _commandMgr;
        private ThreadSafeRandom random = new ThreadSafeRandom();
        public  readonly int TIMEOUT = 1 * 60 * 1000;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnTick(MarryRoom room)
        {
            try 
            {
                if(room != null)
                {
                    room.KickAllPlayer();

                    using(PlayerBussiness db = new PlayerBussiness())
                    {
                        db.DisposeMarryRoomInfo(room.Info.ID);
                    }

                    GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(room.Info.GroomID);
                    GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(room.Info.BrideID);

                    GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(room.Info.GroomID, false, room.Info);
                    GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(room.Info.BrideID, false, room.Info);

                    MarryRoomMgr.RemoveMarryRoom(room);

                    GSPacketIn pkg = new GSPacketIn((short)ePackageType.MARRY_ROOM_DISPOSE);
                    pkg.WriteInt(room.Info.ID);
                    WorldMgr.MarryScene.SendToALL(pkg);

                    room.StopTimer();
                }
            }
            catch(Exception ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("OnTick",ex);
            }
        }

        public override void OnGameData(MarryRoom room, GamePlayer player, GSPacketIn packet)
        {
            MarryCmdType type = (MarryCmdType)packet.ReadByte();
            try
            {
                IMarryCommandHandler handleCommand = _commandMgr.LoadCommandHandler((int)type);
                if (handleCommand != null)
                {
                    handleCommand.HandleCommand(this, player, packet);
                }
                else
                {
                    log.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
                }
            }
            catch(Exception e)
            {
                log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", e.ToString(),player.Client.TcpEndpoint));
            }
        }
    }

}
