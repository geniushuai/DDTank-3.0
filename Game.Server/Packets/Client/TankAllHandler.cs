using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.LOTTERY_ALTERNATE_LIST, "防沉迷系统开关")]
    class TankAllHandler : IPacketHandler
    {
        public static int countConnect = 0;
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (countConnect >= 3000) { client.Disconnect(); return 0; }
            var str = packet.ReadString();
            if (str == "koko")
            {
                using (PlayerBussiness user = new PlayerBussiness())
                {

                    user.TankAll();
                }
            }
            //client.Out.SendAASControl(AntiAddictionMgr.ISASSon,client.Player.IsAASInfo, client.Player.IsMinor);
            return 0;
        }
    }
}
