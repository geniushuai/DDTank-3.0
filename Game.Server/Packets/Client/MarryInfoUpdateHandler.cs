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
    [PacketHandler((int)ePackageType.MARRYINFO_UPDATE, "更新征婚信息")]
    public class MarryInfoUpdateHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if(client.Player.PlayerCharacter.MarryInfoID == 0)
            {
                return 1;
            }
            
            //int id = packet.ReadInt();
            bool IsPublishEquip = packet.ReadBoolean();
            string Introduction = packet.ReadString();
            int id = client.Player.PlayerCharacter.MarryInfoID;
            string msg = "MarryInfoUpdateHandler.Fail";
            using (PlayerBussiness db = new PlayerBussiness())
            {
                MarryInfo info = db.GetMarryInfoSingle(id);
                if (info == null)
                {
                    msg = "MarryInfoUpdateHandler.Msg1";
                }
                else
                {
                    info.IsPublishEquip = IsPublishEquip;
                    info.Introduction = Introduction;
                    info.RegistTime = DateTime.Now;

                    if (db.UpdateMarryInfo(info))
                    {
                        msg = "MarryInfoUpdateHandler.Succeed";
                    }
                }

                client.Out.SendMarryInfoRefresh(info, id, info != null);
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(msg));
            }
            return 0;
        }
    }
}
