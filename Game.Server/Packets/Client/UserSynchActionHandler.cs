using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.SYNCH_ACTION,"用户同步动作")]
    public class UserSynchActionHandler:IPacketHandler
    {
        //修改:  Xiaov 
        //时间:  2009-11-7
        //描述:  用户同步动作<未用到>    
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int toUser = packet.ClientID;
            GamePlayer player = Managers.WorldMgr.GetPlayerById(toUser);
            if (player != null)
            {
                packet.Code = (short)ePackageType.AC_ACTION;
                packet.ClientID = (client.Player.PlayerCharacter.ID);
                player.Out.SendTCP(packet);
            }
            return 1;
        }
    }
}
