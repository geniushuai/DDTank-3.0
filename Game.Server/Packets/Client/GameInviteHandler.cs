using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using SqlDataProvider.Data;
using Bussiness;


namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.GAME_INVITE, "邀请")]
    public class GameInviteHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.CurrentRoom == null)
                return 0;


            int id = packet.ReadInt();
            GamePlayer player = Managers.WorldMgr.GetPlayerById(id);

            if (player == client.Player)
                return 0;

            List<GamePlayer> players = client.Player.CurrentRoom.GetPlayers();
            foreach (GamePlayer p in players)
            {
                if (p == player)
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("friendnotinthesameserver.Sameroom"));
                    return 0;
                }
            }

            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.GAME_INVITE);
            if (player != null && player.CurrentRoom == null)
            {

                pkg.WriteInt(client.Player.PlayerCharacter.ID);
                pkg.WriteInt(client.Player.CurrentRoom.RoomId);
                pkg.WriteInt(client.Player.CurrentRoom.MapId);
                pkg.WriteByte((byte)client.Player.CurrentRoom.TimeMode);
                pkg.WriteByte((byte)client.Player.CurrentRoom.RoomType);
                pkg.WriteByte((byte)client.Player.CurrentRoom.HardLevel);
                pkg.WriteByte((byte)client.Player.CurrentRoom.LevelLimits);
                pkg.WriteString(client.Player.PlayerCharacter.NickName);
                pkg.WriteBoolean(true);
                pkg.WriteInt(5);
                pkg.WriteString(client.Player.CurrentRoom.Name);
                pkg.WriteString(client.Player.CurrentRoom.Password);

                //TrieuLSL BN IMInvite barrierNum
                pkg.WriteInt(50);


                player.Out.SendTCP(pkg);
            }
            else
            {
                if (player != null && player.CurrentRoom != null && player.CurrentRoom != client.Player.CurrentRoom)
                {

                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("friendnotinthesameserver.Room"));
                }
                else
                {
                    client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("friendnotinthesameserver.Fail"));
                }
            }
            return 0;
        }
    }
}
