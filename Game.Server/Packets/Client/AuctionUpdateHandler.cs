using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Bussiness;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using Game.Server.Packets;
using Game.Server.Statics;


namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.AUCTION_UPDATE,"更新拍卖")]
    public class AuctionUpdateHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            int price = packet.ReadInt();
            bool result = false;
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();

            string msg = "AuctionUpdateHandler.Fail";
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {

                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 0;
            }
            using (PlayerBussiness db = new PlayerBussiness())
            {
                AuctionInfo info = db.GetAuctionSingle(id);
                if (info == null)
                {
                    msg = "AuctionUpdateHandler.Msg1";
                }
                else if (info.PayType == 0 && price > client.Player.PlayerCharacter.Gold)
                {
                    msg = "AuctionUpdateHandler.Msg2";
                }
                else if (info.PayType == 1 && price > client.Player.PlayerCharacter.Money)
                {
                    msg = "AuctionUpdateHandler.Msg3";
                }
                else if (info.BuyerID == 0 && info.Price > price)
                {
                    msg = "AuctionUpdateHandler.Msg4";
                }
                else if (info.BuyerID != 0 && info.Price + info.Rise > price && (info.Mouthful == 0 || info.Mouthful > price))
                {
                    msg = "AuctionUpdateHandler.Msg5";
                }
                else
                {
                    int oldBuyerID = info.BuyerID;
                    info.BuyerID = client.Player.PlayerCharacter.ID;
                    info.BuyerName = client.Player.PlayerCharacter.NickName;
                    info.Price = price;
                    if (info.Mouthful != 0 && price >= info.Mouthful)
                    {
                        info.Price = info.Mouthful;
                        info.IsExist = false;
                    }
                    if (db.UpdateAuction(info))
                    {
                        if (info.PayType == 0)
                        {
                            client.Player.RemoveGold(info.Price);
                        }
                        else
                        {
                            client.Player.RemoveMoney(info.Price);
                            LogMgr.LogMoneyAdd(LogMoneyType.Auction, LogMoneyType.Auction_Update, client.Player.PlayerCharacter.ID, info.Price, client.Player.PlayerCharacter.Money, 0, 0, 0, "", "", "");
                        }

                        if (info.IsExist)
                        {
                            msg = "AuctionUpdateHandler.Msg6";
                        }
                        else
                        {
                            msg = "AuctionUpdateHandler.Msg7";
                            client.Out.SendMailResponse(info.AuctioneerID, eMailRespose.Receiver);
                            client.Out.SendMailResponse(info.BuyerID, eMailRespose.Receiver);
                        }

                        if (oldBuyerID != 0)
                        {
                            client.Out.SendMailResponse(oldBuyerID, eMailRespose.Receiver);//通知老买主价格被超出
                        }
                        result = true;
                    }
                }

                client.Out.SendAuctionRefresh(info, id, info != null ? info.IsExist : false, null);
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(msg));
            }
            pkg.WriteBoolean(result);
            pkg.WriteInt(id);
            client.Out.SendTCP(pkg);
            return 0;
        }
    }
}
