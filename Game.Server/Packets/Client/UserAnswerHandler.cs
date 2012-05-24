using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Logic;
using SqlDataProvider.Data;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.USER_ANSWER, "New User Answer Question")]
    public class UserAnswerHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            
            int id = packet.ReadInt();                
            if (client.Player.PlayerCharacter.AnswerSite < id)
            {                
                List<ItemInfo> infos=null;
                if (DropInventory.AnswerDrop(id, ref infos) == true)
                {
                    client.Player.PlayerCharacter.AnswerSite = id;
                    if (infos != null)
                    {
                        int gold=0;
                        int money = 0;
                        int giftToken = 0;
                        foreach (ItemInfo info in infos)
                        {
                            ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken);
                            if (info != null)
                            {
                                if (info.Template.BagType == eBageType.PropBag)
                                {
                                    client.Player.MainBag.AddTemplate(info, info.Count);
                                }                            
                            }
                            client.Player.AddGold(gold);
                            client.Player.AddMoney(money);
                            client.Player.AddGiftToken(giftToken);
                            LogMgr.LogMoneyAdd(LogMoneyType.Award, LogMoneyType.Award_Answer, client.Player.PlayerCharacter.ID, money, client.Player.PlayerCharacter.Money, gold, 0, 0, "", "", "");
                        }
                    }           
                }
            }
            //GSPacketIn pkg = packet.Clone();
            //pkg.ClearContext();
            //pkg.WriteByte(0);
            ////pkg.WriteInt(0);
            //for (int i = 0; i < 100; i++)
            //{
            //    pkg.WriteByte(1);
            //}
            //pkg.WriteInt(client.Player.PlayerCharacter.AnswerSite);
            //client.Player.Out.SendTCP(pkg);
            return 1;
        }
    }
}
