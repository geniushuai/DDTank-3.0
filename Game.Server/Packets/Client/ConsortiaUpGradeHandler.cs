using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Bussiness.Managers;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.CONSORTIA_UPGRADE, "公会升级")]
    public class ConsortiaUpGradeHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            int bagType = packet.ReadByte();
            int place = packet.ReadInt();

            bool result = false;
            string msg = "ConsortiaUpGradeHandler.Failed";
            using (ConsortiaBussiness db = new ConsortiaBussiness())
            {
                ConsortiaInfo info = db.GetConsortiaSingle(client.Player.PlayerCharacter.ConsortiaID);
                if (info == null)
                {
                    msg = "ConsortiaUpGradeHandler.NoConsortia";
                }
                else
                {
                    ConsortiaLevelInfo levelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(info.Level + 1);
                    //ItemTemplateInfo temp = ItemMgr.GetSingleGoods(levelInfo.NeedItem);

                    if (levelInfo == null)
                    {
                        msg = "ConsortiaUpGradeHandler.NoUpGrade";
                    }
                    //else if (levelInfo.NeedItem != 0 && client.Player.GetAllItemCount(levelInfo.NeedItem) < 1)
                    //{
                    //    msg = "ConsortiaUpGradeHandler.NoItem";
                    //}
                    else if (levelInfo.NeedGold > client.Player.PlayerCharacter.Gold)
                    {
                        msg = "ConsortiaUpGradeHandler.NoGold";
                    }
                    else
                    {
                        using (ConsortiaBussiness cb = new ConsortiaBussiness())
                        {
                            if (cb.UpGradeConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
                            {
                                info.Level++;
                                client.Player.RemoveGold(levelInfo.NeedGold);
                                // client.Player.RemoveItemCount(levelInfo.NeedItem, 1);
                                GameServer.Instance.LoginServer.SendConsortiaUpGrade(info);
                                msg = "ConsortiaUpGradeHandler.Success";
                                result = true;
                            }
                        }
                    }
                }
                if (info.Level >= 5)
                {
                    string msg1 = LanguageMgr.GetTranslation("ConsortiaUpGradeHandler.Notice", info.ConsortiaName, info.Level);

                    GSPacketIn pkg = new GSPacketIn((byte)ePackageType.SYS_NOTICE);
                    pkg.WriteInt(2);
                    pkg.WriteString(msg1);

                    GameServer.Instance.LoginServer.SendPacket(pkg);

                    GamePlayer[] players = Game.Server.Managers.WorldMgr.GetAllPlayers();

                    foreach (GamePlayer p in players)
                    {
                        if (p != client.Player && p.PlayerCharacter.ConsortiaID != client.Player.PlayerCharacter.ConsortiaID)
                            p.Out.SendTCP(pkg);
                    }
                }

            }


            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);

            return 1;
        }
    }

}
