using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.CONSORTIA_CHAT, "公会聊天")]
    public class ConsortiaChatHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.ConsortiaID == 0)
                return 0;

            if (client.Player.PlayerCharacter.IsBanChat)
            {
                client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat"));
                return 1;
            }

            packet.ClientID = (client.Player.PlayerCharacter.ID);
            byte channel = packet.ReadByte();
            string nick = packet.ReadString();
            string msg = packet.ReadString();
            packet.WriteInt(client.Player.PlayerCharacter.ConsortiaID);

            GamePlayer[] players = WorldMgr.GetAllPlayers();
            foreach (GamePlayer p in players)
            {
                if (p.PlayerCharacter.ConsortiaID == client.Player.PlayerCharacter.ConsortiaID)
                    p.Out.SendTCP(packet);
            }

            GameServer.Instance.LoginServer.SendPacket(packet);

            return 0;
        }
    }
}
