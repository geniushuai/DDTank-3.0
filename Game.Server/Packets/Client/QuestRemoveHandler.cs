using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Quests;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.QUEST_REMOVE,"删除任务")]
    public class QuestRemoveHandler:IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int id = packet.ReadInt();//传入用户任务编号
            BaseQuest quest = client.Player.QuestInventory.FindQuest(id);
            if (quest != null)
            {
                client.Player.QuestInventory.RemoveQuest(quest);
            }
            return 0;
        }
    }
}
