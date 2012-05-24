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
    [PacketHandler((int)ePackageType.AUCTION_DELETE,"撤消拍卖")]
    public class AuctionDeleteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            string msg = LanguageMgr.GetTranslation("AuctionDeleteHandler.Fail");
            using (PlayerBussiness db = new PlayerBussiness())
            {
                if (db.DeleteAuction(id, client.Player.PlayerCharacter.ID, ref msg))
                {
                    client.Out.SendMailResponse(client.Player.PlayerCharacter.ID,eMailRespose.Receiver);
                    client.Out.SendAuctionRefresh(null, id, false,null);
                }
                else
                {
                    AuctionInfo info = db.GetAuctionSingle(id);
                    client.Out.SendAuctionRefresh(info, id, info != null,null);
                }

                client.Out.SendMessage(eMessageType.Normal, msg);
               // client.Out.SendMessage(eMessageType.ERROR,msg);
            }
            return 0;
        }
    }

}
