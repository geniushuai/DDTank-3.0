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
    [PacketHandler((byte)ePackageType.CONSORTIA_SHOP_UPGRADE, "公会商城升级")]
    public class ConsortiaShopUpGradeHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;


            bool result = false;
            string msg = "ConsortiaShopUpGradeHandler.Failed";
            //using (ConsortiaBussiness db = new ConsortiaBussiness())
            //{
            //    ConsortiaInfo info = db.GetConsortiaSingle(client.Player.PlayerCharacter.ConsortiaID);
            ConsortiaInfo info = Managers.ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
            if (info == null)
            {
                msg = "ConsortiaShopUpGradeHandler.NoConsortia";
            }
            else
            {
                using (ConsortiaBussiness cb = new ConsortiaBussiness())
                {
                    if (cb.UpGradeShopConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
                    {
                        info.ShopLevel++;
                        GameServer.Instance.LoginServer.SendConsortiaShopUpGrade(info);
                        msg = "ConsortiaShopUpGradeHandler.Success";
                        result = true;
                    }
                }
            }
            if (info.ShopLevel >= 2)
            {
                string msg1 = LanguageMgr.GetTranslation("ConsortiaShopUpGradeHandler.Notice", client.Player.PlayerCharacter.ConsortiaName, info.ShopLevel);

                GSPacketIn pkg = new GSPacketIn((byte)ePackageType.SYS_NOTICE);
                pkg.WriteInt(2);
                pkg.WriteString(msg1);

                GameServer.Instance.LoginServer.SendPacket(pkg);

                GamePlayer[] players = Game.Server.Managers.WorldMgr.GetAllPlayers();

                foreach (GamePlayer p in players)
                {
                    if (p != client.Player)
                        p.Out.SendTCP(pkg);
                }
            }
            //}


            packet.WriteBoolean(result);
            packet.WriteString(LanguageMgr.GetTranslation(msg));
            client.Out.SendTCP(packet);
            return 1;
        }
    }
}
