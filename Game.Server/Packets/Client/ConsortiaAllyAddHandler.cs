using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{     
    [PacketHandler((byte)ePackageType.CONSORTIA_ALLY_ADD, "添加敌对")]
    public class ConsortiaAllyAddHandler:IPacketHandler
    {
  
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {

          
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            int id = packet.ReadInt();
            bool isFight = packet.ReadBoolean();

            bool result = false;
            string msg = "ConsortiaAllyAddHandler.Add_Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                ConsortiaAllyInfo info = new ConsortiaAllyInfo();
                info.Consortia1ID = client.Player.PlayerCharacter.ConsortiaID;
                info.Consortia2ID = id;
                info.Date = DateTime.Now;
                info.IsExist = true;
                info.State = 2;//isFight ? 2 : 0;
                info.ValidDate = 0;
                if (db.AddConsortiaAlly(info, client.Player.PlayerCharacter.ID, ref msg))
                {
                    msg = isFight ? "ConsortiaAllyAddHandler.Add_Success2" : "ConsortiaAllyAddHandler.Add_Success1";

                    result = true;
                    GameServer.Instance.LoginServer.SendConsortiaAlly(info.Consortia1ID, info.Consortia2ID, info.State);
                }
            }
            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
