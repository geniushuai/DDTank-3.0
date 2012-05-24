using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Server.HotSpringRooms;
using Game.Server.HotSpringRooms.TankHandle;

namespace Game.Server.HotSpringRooms.TankHandle
{
    [HotSpringCommandAttribute((byte)HotSpringCmdType.HOTSPRING_ROOM_INVITE)]
    public class InviteCommand : IHotSpringCommandHandler
    {
        public bool HandleCommand(TankHotSpringLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentHotSpringRoom == null || player.CurrentHotSpringRoom.RoomState != eRoomState.FREE)
            {
                return false;
            }

            if (player.CurrentHotSpringRoom.Info.GuestInvite == false)
            {
                if( player.PlayerCharacter.ID != player.CurrentHotSpringRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentHotSpringRoom.Info.BrideID)
                {
                    return false;
                }
            }
            
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();

            int id = packet.ReadInt();
            GamePlayer invitedplayer = Managers.WorldMgr.GetPlayerById(id);
            if (invitedplayer != null && invitedplayer.CurrentRoom == null && invitedplayer.CurrentHotSpringRoom == null)
            {
                pkg.WriteByte((byte)HotSpringCmdType.HOTSPRING_ROOM_INVITE);
                pkg.WriteInt(player.PlayerCharacter.ID);
                pkg.WriteString(player.PlayerCharacter.NickName);
                pkg.WriteInt(player.CurrentHotSpringRoom.Info.ID);
                //pkg.WriteInt(player.CurrentHotSpringRoom.Info.MapIndex);
                pkg.WriteString(player.CurrentHotSpringRoom.Info.Name);
                pkg.WriteString(player.CurrentHotSpringRoom.Info.Pwd);
                pkg.WriteInt(player.MarryMap);

                invitedplayer.Out.SendTCP(pkg);

                return true;
            }

            return false;

        }
    }
}
