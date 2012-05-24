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
    [PacketHandler((byte)ePackageType.CONSORTIA_STORE_UPGRADE, "银行升级")]
    public class ConsortiaStoreUpGradeHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            bool result = false;
            string msg = "ConsortiaStoreUpGradeHandler.Failed";
            //using (ConsortiaBussiness db = new ConsortiaBussiness())
            //{
            //    ConsortiaInfo info = db.GetConsortiaSingle(client.Player.PlayerCharacter.ConsortiaID);

            ConsortiaInfo info = Managers.ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
            if (info == null)
            {
                msg = "ConsortiaStoreUpGradeHandler.NoConsortia";
            }
            else
            {
                using (ConsortiaBussiness cb = new ConsortiaBussiness())
                {
                    if (cb.UpGradeStoreConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
                    {
                        info.StoreLevel++;
                        GameServer.Instance.LoginServer.SendConsortiaStoreUpGrade(info);
                        msg = "ConsortiaStoreUpGradeHandler.Success";
                        result = true;
                    }
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
