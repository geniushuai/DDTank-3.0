using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.SCENE_USERS_LIST,"用户列表")]
    public class SceneUsersListHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();

            byte page = packet.ReadByte();
            byte count = packet.ReadByte();

            GamePlayer[] players = Managers.WorldMgr.GetAllPlayersNoGame();

            int total = players.Length;
            byte length = total > count ? count : (byte)total;

            pkg.WriteByte(length);
            for (int i = page * count; i < page * count + length; i++)
            {
                PlayerInfo info = players[i % total].PlayerCharacter;
                pkg.WriteInt(info.ID);
                pkg.WriteString(info.NickName == null ? "" : info.NickName);
                //Isvip
                pkg.WriteBoolean(true);
                pkg.WriteInt(5);
                pkg.WriteBoolean(info.Sex);
                pkg.WriteInt(info.Grade);
                pkg.WriteInt(info.ConsortiaID);
                pkg.WriteString(info.ConsortiaName == null ? "" : info.ConsortiaName);
                pkg.WriteInt(info.Offer);
                pkg.WriteInt(info.Win);
                pkg.WriteInt(info.Total);
                pkg.WriteInt(info.Escape);
                pkg.WriteInt(info.Repute);
                pkg.WriteInt(info.FightPower);

            }

            client.Out.SendTCP(pkg);

            return 0;
        }
    }
}
