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
    [PacketHandler((byte)ePackageType.CONSORTIA_CREATE,"创建公会")]
    public class ConsortiaCreateHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID != 0)
                return 0;

            ConsortiaLevelInfo levelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(1);
            string name = packet.ReadString();
            if (string.IsNullOrEmpty(name) || System.Text.Encoding.Default.GetByteCount(name) > 12)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaCreateHandler.Long"));
                return 1;
            }

            bool result = false;
            int id = 0;
            int mustGold = levelInfo.NeedGold;
            int mustLevel = 5;
            string msg = "ConsortiaCreateHandler.Failed";
            ConsortiaDutyInfo dutyInfo = new ConsortiaDutyInfo();

            if (!string.IsNullOrEmpty(name) && client.Player.PlayerCharacter.Gold >= mustGold && client.Player.PlayerCharacter.Grade >= mustLevel)
            {
                using (ConsortiaBussiness db = new ConsortiaBussiness())
                {
                    ConsortiaInfo info = new ConsortiaInfo();
                    info.BuildDate = DateTime.Now;
                    info.CelebCount = 0;
                    info.ChairmanID = client.Player.PlayerCharacter.ID;
                    info.ChairmanName = client.Player.PlayerCharacter.NickName;
                    info.ConsortiaName = name;
                    info.CreatorID = info.ChairmanID;
                    info.CreatorName = info.ChairmanName;
                    info.Description = "";
                    info.Honor = 0;
                    info.IP = "";
                    info.IsExist = true;
                    info.Level = levelInfo.Level;
                    info.MaxCount = levelInfo.Count;
                    info.Riches = levelInfo.Riches;
                    info.Placard = "";
                    info.Port = 0;
                    info.Repute = 0;
                    info.Count = 1;
                    if (db.AddConsortia(info, ref msg, ref dutyInfo))
                    {
                        client.Player.PlayerCharacter.ConsortiaID = info.ConsortiaID;
                        client.Player.PlayerCharacter.ConsortiaName = info.ConsortiaName;
                        client.Player.PlayerCharacter.DutyLevel = dutyInfo.Level;
                        client.Player.PlayerCharacter.DutyName = dutyInfo.DutyName;
                        client.Player.PlayerCharacter.Right = dutyInfo.Right;
                        client.Player.PlayerCharacter.ConsortiaLevel = levelInfo.Level;
                        client.Player.RemoveGold(mustGold);
                        msg = "ConsortiaCreateHandler.Success";
                        result = true;
                        id = info.ConsortiaID;
                        GameServer.Instance.LoginServer.SendConsortiaCreate(id, client.Player.PlayerCharacter.Offer,info.ConsortiaName);
                    }
                }

            }
            packet.WriteBoolean(result);
            packet.WriteInt(id);
            packet.WriteString(name);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            packet.WriteInt(dutyInfo.Level);
            packet.WriteString(dutyInfo.DutyName == null ? "" : dutyInfo.DutyName);
            packet.WriteInt(dutyInfo.Right);
            client.Out.SendTCP(packet);

            //client.Out.SendMessage(eMessageType.Normal, msg);

            return 0;
        }
    }
}
