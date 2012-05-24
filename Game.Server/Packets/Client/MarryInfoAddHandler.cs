using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Bussiness;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.MARRYINFO_ADD, "添加征婚信息")]
    public class MarryInfoAddHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.MarryInfoID != 0)
            {
                return 1;
            }
            
            bool IsPublishEquip = packet.ReadBoolean();
            string Introduction = packet.ReadString();
            int UserID = client.Player.PlayerCharacter.ID;
            eMessageType eMsg = eMessageType.Normal;
            string msg = "MarryInfoAddHandler.Fail";

            int needGold = 10000;
            if (needGold > client.Player.PlayerCharacter.Gold)
            {
                eMsg = eMessageType.ERROR;
                msg = "MarryInfoAddHandler.Msg1";
            }
            else
            {
                client.Player.SaveIntoDatabase();
                MarryInfo info = new MarryInfo();
                info.UserID = UserID;
                info.IsPublishEquip = IsPublishEquip;
                info.Introduction = Introduction;
                info.RegistTime = DateTime.Now;

                using (PlayerBussiness db = new PlayerBussiness())
                {
                    if (db.AddMarryInfo(info))
                    {
                        client.Player.RemoveGold(needGold);
                        msg = "MarryInfoAddHandler.Msg2";
                        client.Player.PlayerCharacter.MarryInfoID = info.ID;
                        client.Out.SendMarryInfoRefresh(info, info.ID, true);
                    }
                }
            }


            client.Out.SendMessage(eMsg, LanguageMgr.GetTranslation(msg));

            return 0;
        }
    }
}
