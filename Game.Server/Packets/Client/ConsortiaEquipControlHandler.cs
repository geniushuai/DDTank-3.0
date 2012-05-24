using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_EQUIP_CONTROL, "财富控制")]
    public class ConsortiaEquipControlHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;
            //Type 1表示商城，2表示铁匠铺

            bool result = false;
            string msg = "ConsortiaEquipControlHandler.Fail";
            ConsortiaEquipControlInfo info = new ConsortiaEquipControlInfo();
            info.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                for (int i = 0; i < 5; i++)
                {

                    info.Riches = packet.ReadInt();
                    info.Type = 1;
                    info.Level = i + 1;

                    db.AddAndUpdateConsortiaEuqipControl(info, client.Player.PlayerCharacter.ID, ref msg);
                }
                info.Riches = packet.ReadInt();
                info.Type = 2;
                info.Level = 0;
                db.AddAndUpdateConsortiaEuqipControl(info, client.Player.PlayerCharacter.ID, ref msg);
                msg = "ConsortiaEquipControlHandler.Success";
                result = true;
            }
            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
