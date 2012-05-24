using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;
using Game.Server.SceneMarryRooms.TankHandle;

namespace Game.Server.SceneMarryRooms.TankHandle
{
    [MarryCommandAttbute((byte)MarryCmdType.INVITE)]
    public class InviteCommand : IMarryCommandHandler
    {
        public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
        {
            if (player.CurrentMarryRoom == null || player.CurrentMarryRoom.RoomState != eRoomState.FREE)
            {
                return false;
            }

            if (player.CurrentMarryRoom.Info.GuestInvite == false)
            {
                if( player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
                {
                    return false;
                }
            }
            
            GSPacketIn pkg = packet.Clone();
            pkg.ClearContext();

            int id = packet.ReadInt();
            GamePlayer invitedplayer = Managers.WorldMgr.GetPlayerById(id);
            if (invitedplayer != null && invitedplayer.CurrentRoom == null && invitedplayer.CurrentMarryRoom == null)
            {
                pkg.WriteByte((byte)MarryCmdType.INVITE);
                pkg.WriteInt(player.PlayerCharacter.ID);
                pkg.WriteString(player.PlayerCharacter.NickName);
                pkg.WriteInt(player.CurrentMarryRoom.Info.ID);
                //pkg.WriteInt(player.CurrentMarryRoom.Info.MapIndex);
                pkg.WriteString(player.CurrentMarryRoom.Info.Name);
                pkg.WriteString(player.CurrentMarryRoom.Info.Pwd);
                pkg.WriteInt(player.MarryMap);

                invitedplayer.Out.SendTCP(pkg);

                return true;
            }

            return false;

        }
    }
}
