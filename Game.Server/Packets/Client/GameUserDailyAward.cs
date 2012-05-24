using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using log4net.Util;
using Game.Server.GameObjects;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Statics;
using Game.Server.Packets;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{

    [PacketHandler((int)ePackageType.DAILY_AWARD, "日常奖励")]
    public class GameUserDailyAward : IPacketHandler
    {
        #region IPacketHandler Members
        public int HandlePacket(GameClient client, Game.Base.Packets.GSPacketIn packet)
        {
            if (Managers.AwardMgr.AddDailyAward(client.Player) == true)
            {
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    if (db.UpdatePlayerLastAward(client.Player.PlayerCharacter.ID) == true)
                    {
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("GameUserDailyAward.Success"));
                    }
                    else
                    {
                        client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("GameUserDailyAward.Fail"));
                    }
                }

            }
            else
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("GameUserDailyAward.Fail1"));
            }
            return 2;
        }

        #endregion

    }
}
