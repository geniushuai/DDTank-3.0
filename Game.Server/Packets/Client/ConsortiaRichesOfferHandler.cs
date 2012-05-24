using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Enumerate;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.CONSORTIA_RICHES_OFFER, "捐献公会财富")]
    public class ConsortiaRichesOfferHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            int money = packet.ReadInt();
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {

                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 1;
            }

            if (money < 1 || client.Player.PlayerCharacter.Money < money)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaRichesOfferHandler.NoMoney"));
                return 1;
            }

            bool result = false;
            string msg = "ConsortiaRichesOfferHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                int riches = money / 2;
                if (db.ConsortiaRichAdd(client.Player.PlayerCharacter.ConsortiaID, ref riches, 5, client.Player.PlayerCharacter.NickName))
                {
                    result = true;
                    client.Player.PlayerCharacter.RichesOffer += riches;
                    //client.Player.SetMoney(-money);

                    client.Player.RemoveMoney(money);
                    LogMgr.LogMoneyAdd(LogMoneyType.Consortia, LogMoneyType.Consortia_Rich, client.Player.PlayerCharacter.ID, money, client.Player.PlayerCharacter.Money, 0, 0, (int)eSubConsumerType.Consortia_Riches_Offer, "", "", "");                    
                    msg = "ConsortiaRichesOfferHandler.Successed";
                    GameServer.Instance.LoginServer.SendConsortiaRichesOffer(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, riches);
                }
            }
            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 0;
        }
    }
}
