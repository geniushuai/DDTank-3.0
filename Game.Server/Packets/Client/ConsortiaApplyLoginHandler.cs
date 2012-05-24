using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_TRYIN, "申请进入")]
    public class ConsortiaApplyLoginHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
                return 0;

            int id = packet.ReadInt();
            bool result = false;
            string msg = "ConsortiaApplyLoginHandler.ADD_Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                ConsortiaApplyUserInfo info = new ConsortiaApplyUserInfo();
                info.ApplyDate = DateTime.Now;
                info.ConsortiaID = id;
                info.ConsortiaName = "";
                info.IsExist = true;
                info.Remark = "";
                info.UserID = client.Player.PlayerCharacter.ID;
                info.UserName = client.Player.PlayerCharacter.NickName;
                if (db.AddConsortiaApplyUsers(info, ref msg))
                {
                    msg = id != 0 ? "ConsortiaApplyLoginHandler.ADD_Success" : "ConsortiaApplyLoginHandler.DELETE_Success";
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
