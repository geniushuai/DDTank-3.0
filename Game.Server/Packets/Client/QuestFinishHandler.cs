using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Quests;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.QUEST_FINISH,"任务完成")]
    public class QuestFinishHandler:IPacketHandler 
    {
      
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {


            int id = packet.ReadInt();
            int rewardItemID = packet.ReadInt();
            BaseQuest _baseQuest= client.Player.QuestInventory.FindQuest(id);
            int i=0;//未完成
            if (_baseQuest!= null)
            {
                i= client.Player.QuestInventory.Finish(_baseQuest, rewardItemID) ? 1 : 0;
            }
            if (i == 1)
            {
                packet.WriteInt(id);
                client.Out.SendTCP(packet);
            }
            
            return i;
        }
    }
}
