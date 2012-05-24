using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.FRIEND_STATE, "改变状态")]
    class FriendStateChangeHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //packet.ClientID=(client.Player.PlayerCharacter.ID);
            //bool state = packet.ReadBoolean();
            ////client.Player.PlayerCharacter.State = state ? 1 : 2;
            ////client.Player.SaveIntoDatabase();
            //GameServer.Instance.LoginServer.SendPacket(packet);
            //Managers.WorldMgr.ChangePlayerState(packet.ClientID, state,client.Player.PlayerCharacter.ConsortiaID);
            return 0;
        }
    }
}
