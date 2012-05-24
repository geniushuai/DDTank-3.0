using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_ALLY_APPLY_ADD, "申请同盟")]
    public class ConsortiaApplyAllyAddHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            int id = packet.ReadInt();
            bool isAlly = packet.ReadBoolean();
            bool result = false;
            string msg = "ConsortiaApplyAllyAddHandler.Add_Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                ConsortiaApplyAllyInfo info = new ConsortiaApplyAllyInfo();
                info.Consortia1ID = client.Player.PlayerCharacter.ConsortiaID;
                info.Consortia2ID = id;
                info.Date = DateTime.Now;
                info.State = 0;// isAlly ? 1 : 0;
                info.Remark = "";
                info.IsExist = true;
                if (db.AddConsortiaApplyAlly(info, client.Player.PlayerCharacter.ID, ref msg))
                {
                    msg = "ConsortiaApplyAllyAddHandler.Add_Success";
                    result = true;
                }
            }
            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
