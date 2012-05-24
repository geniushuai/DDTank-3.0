using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Bussiness.Managers;
using Bussiness;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.CHECK_CODE, "验证码")]
    public class CheckCodeHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            
            //bool result = false;
            if (string.IsNullOrEmpty(client.Player.PlayerCharacter.CheckCode))
                return 1;

            //int check  = packet.ReadInt();

            string check = packet.ReadString();
            if (client.Player.PlayerCharacter.CheckCode.ToLower() == check.ToLower())
            {
                client.Player.PlayerCharacter.CheckCount = 0;

                //int rewardItemID = GameServer.Instance.Configuration.CheckRewardItem;
                //ItemTemplateInfo rewardItem = ItemMgr.GetSingleGoods(rewardItemID);
                //ItemInfo item = ItemInfo.CreateFromTemplate(rewardItem, 1, (int)Game.Server.Statics.ItemAddType.CheckCode);
                //if (item != null)
                //{
                //    item.IsBinds = true;
                //    if (client.Player.AddItem(item, Game.Server.Statics.ItemAddType.CheckCode) != -1)
                //    {
                //        client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("CheckCodeHandler.Msg1", item.Template.Name));
                //    }
                //    else
                //    {
                //        client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("CheckCodeHandler.Msg2"));
                //    }
                //}
                int GP = LevelMgr.GetGP(client.Player.PlayerCharacter.Grade);
                client.Player.AddGP(LevelMgr.IncreaseGP(client.Player.PlayerCharacter.Grade, client.Player.PlayerCharacter.GP));
               
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CheckCodeHandler.Msg1", client.Player.PlayerCharacter.Grade * 12));
                //result = true;
                packet.ClearContext();
                packet.WriteByte(1);
                packet.WriteBoolean(false);
                client.Out.SendTCP(packet);
            }
           // else if (client.Player.PlayerCharacter.CheckError < 1 && client.Player.PlayerCharacter.CheckCount < 20000)
            else if (client.Player.PlayerCharacter.CheckError < 9)
            {
                client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("CheckCodeHandler.Msg3"));
                client.Player.PlayerCharacter.CheckError++;
                client.Out.SendCheckCode();
               
            }
            else
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CheckCodeHandler.Msg3"));
                client.Disconnect();
              
            }
          
         
            return 0;
        }
    }
}
