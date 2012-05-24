using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.QUEST_ADD,"添加任务")]
    public class QuestAddHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {           
            string msg;
            int count = packet.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int id = packet.ReadInt();
                QuestInfo info=Bussiness.Managers.QuestMgr.GetSingleQuest(id);                
                client.Player.QuestInventory.AddQuest(info,  out msg);
 
            }

            return 0;
        }
    }
}
