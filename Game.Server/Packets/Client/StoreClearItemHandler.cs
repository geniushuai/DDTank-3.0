using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using System.Configuration;
using Game.Server.Managers;
using Game.Server.Statics;
using Game.Server.GameObjects;
using Game.Server.GameUtils;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.CLEAR_STORE_BAG, "物品强化")]
    public class StoreClearItemHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //GSPacketIn pkg = packet.Clone();
            //pkg.ClearContext();
            PlayerInventory m_storeBag = client.Player.StoreBag2;
            PlayerEquipInventory m_mainBag = client.Player.MainBag;
            PlayerInventory m_propBag = client.Player.PropBag;

            for(int i=0;i<m_storeBag.Capalility;i++){
                if (m_storeBag.GetItemAt(i) != null)
                {
                    var item = m_storeBag.GetItemAt(i);
                    if (item.Template.CategoryID == 10 || item.Template.CategoryID == 11 || item.Template.CategoryID == 12)
                    {
                        m_storeBag.MoveToStore(m_storeBag, i, m_propBag.FindFirstEmptySlot(1), m_propBag, 999);
                    }
                    else
                    {
                        if (item.Template.CategoryID == 7&&m_mainBag.GetItemAt(6)==null)
                        {
                            m_storeBag.MoveToStore(m_storeBag, i, 6, m_mainBag, 999);
                        }else
                        m_storeBag.MoveToStore(m_storeBag, i, m_mainBag.FindFirstEmptySlot(32), m_mainBag, 999);
                    }
                }
                
            }
            //m_storeBag.ClearBag();
            return 0;
        }
    }
}
