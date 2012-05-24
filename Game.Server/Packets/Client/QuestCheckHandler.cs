using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Quests;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.QUEST_CHECK, "客服端任务检查")]
    public class QuestCheckHandler : IPacketHandler 
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int questId = packet.ReadInt();
            int conditionId = packet.ReadInt();
            int value = packet.ReadInt();

            BaseQuest quest = client.Player.QuestInventory.FindQuest(questId);
            if (quest != null)
            {
                ClientModifyCondition cd = quest.GetConditionById(conditionId) as ClientModifyCondition;
                if (cd != null)
                {
                    cd.Value =value;
                }
            }
            return 0;
        }
    }
}
