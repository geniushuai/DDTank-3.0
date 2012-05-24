using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_DUTY_UPDATE, "更新职务")]
    public class ConsortiaDutyUpdateHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            int dutyID = packet.ReadInt();
            int updateType = packet.ReadByte();
            //int level = packet.ReadInt();
            bool result = false;
            string msg = "ConsortiaDutyUpdateHandler.Failed";

            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                ConsortiaDutyInfo info = new ConsortiaDutyInfo();
                info.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
                info.DutyID = dutyID;
                info.IsExist = true;
                info.DutyName = "";
                switch (updateType)
                {
                    case 1:
                        return 1;
                    case 2:
                        info.DutyName = packet.ReadString();
                        if (string.IsNullOrEmpty(info.DutyName) || System.Text.Encoding.Default.GetByteCount(info.DutyName) > 10)
                        {
                            client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaDutyUpdateHandler.Long"));
                            return 1;
                        }
                        info.Right = packet.ReadInt();
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                }

                if (db.UpdateConsortiaDuty(info, client.Player.PlayerCharacter.ID, updateType, ref msg))
                {
                    dutyID = info.DutyID;
                    msg = "ConsortiaDutyUpdateHandler.Success";
                    result = true;
                    GameServer.Instance.LoginServer.SendConsortiaDuty(info, updateType, client.Player.PlayerCharacter.ConsortiaID);
                }
            }
            packet.WriteBoolean(result);
            packet.WriteInt(dutyID);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
