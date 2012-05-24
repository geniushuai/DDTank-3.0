using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_DUTY_ADD, "添加职务")]
    public class ConsortiaDutyAddHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            //string dutyName = packet.ReadString();
            //bool result = false;
            //string msg = "添加职务失败!";
            //using (ConsortiaBussiness db = new ConsortiaBussiness())
            //{
            //    ConsortiaDutyInfo info = new ConsortiaDutyInfo();
            //    info.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
            //    info.DutyID = 0;
            //    info.DutyName = dutyName;
            //    info.IsChat = false;
            //    info.IsDiplomatism = false;
            //    info.IsDownGrade = false;
            //    info.IsEditorDescription = false;
            //    info.IsEditorPlacard = false;
            //    info.IsEditorUser = false;
            //    info.IsExist =true;
            //    info.IsExpel =false;
            //    info.IsInvite = false;
            //    info.IsManageDuty = false;
            //    info.IsRatify = false;
            //    info.IsUpGrade = false;
            //    info.Level = 0;
            //    if (db.AddConsortiaDuty(info,client.Player.PlayerCharacter.ID, ref msg))
            //    {
            //        msg = "添加职务成功!";
            //        result = true;
            //    }
            //}
            //packet.WriteBoolean(result);
            //packet.WriteString(msg);
            //client.Out.SendTCP(packet);

            return 0;
        }
    }
}
