using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.HOTSPRING_ROOM_ENTER_VIEW,"礼堂数据")]
    public class HotSpringRoomEnterViewHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //if (client.Player.CurrentHotSpringRoom != null)
            //{
            //    client.Player.CurrentHotSpringRoom.ProcessData(client.Player, packet);
            //}
            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.HOTSPRING_ROOM_PLAYER_ADD);
            var player = client.Player.PlayerCharacter;
            pkg.WriteInt(player.ID);
            pkg.WriteInt(player.Grade);
            pkg.WriteInt(player.Hide);

            pkg.WriteInt(player.Repute);
            pkg.WriteString(player.NickName);
            pkg.WriteBoolean(true);
            pkg.WriteInt(5);
  
            pkg.WriteBoolean(player.Sex);
            pkg.WriteString(player.Style);
            pkg.WriteString(player.Colors);
            pkg.WriteString(player.Skin);
            pkg.WriteInt(405);
            pkg.WriteInt(405);
            //var _loc_6:* = new Point(_loc_2.readInt(), _loc_2.readInt());
            pkg.WriteInt(player.FightPower);
            pkg.WriteInt(player.Win);
            pkg.WriteInt(player.Total);
            pkg.WriteInt(45);
            //_loc_5.FightPower = _loc_2.readInt();
            //_loc_5.WinCount = _loc_2.readInt();
            //_loc_5.TotalCount = _loc_2.readInt();
            //_loc_4.playerDirection = _loc_2.readInt();
            client.SendTCP(pkg);
            return 0;
        }

    }
}
