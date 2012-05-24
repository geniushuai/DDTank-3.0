using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Bussiness;
using Game.Server.Packets;
using Game.Server.Statics;
using Bussiness.Managers;
using Game.Logic.Phy.Object;

namespace Game.Server.GameUtils
{
    public class PlayerEquipInventory : PlayerInventory
    {
        private const int BAG_START = 31;

        public PlayerEquipInventory(GamePlayer player)
            : base(player, true, 81, 0, BAG_START, true)
        {
        }

        /// <summary>
        /// 从数据库中加载物品，到指定的格子。
        /// </summary>
        public override void LoadFromDatabase()
        {
            bool response = false;

            BeginChanges();
            try
            {
                base.LoadFromDatabase();

                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    for (int i = 0; i < BAG_START; i++)
                    {
                        ItemInfo item = m_items[i];
                        if (m_items[i] != null && !m_items[i].IsValidItem())
                        {
                            int slot = FindFirstEmptySlot(BAG_START);

                            if (slot >= 0)
                            {
                                MoveItem(item.Place, slot, item.Count);
                            }
                            else
                            {
                                response = response || SendItemToMail(item, pb, null);
                            }
                        }
                    }
                }
            }
            finally
            {
                CommitChanges();
            }

            if (response)
            {
                m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
            }
        }

        public override bool MoveItem(int fromSlot, int toSlot, int count)
        {
            if (m_items[fromSlot] == null) return false;


            if (IsEquipSlot(fromSlot) && !IsEquipSlot(toSlot) && m_items[toSlot] != null && m_items[toSlot].Template.CategoryID != m_items[fromSlot].Template.CategoryID)
            {
                if (!CanEquipSlotContains(fromSlot, m_items[toSlot].Template))
                    toSlot = FindFirstEmptySlot(BAG_START);
                //return false;

                //if (!m_player.CanEquip(m_items[toSlot].Template))
                //    return false;
                //return this.AddItemTo(m_items[fromSlot], toSlot);
            }
            else
            {
                if (IsEquipSlot(toSlot))
                {
                    if (!CanEquipSlotContains(toSlot, m_items[fromSlot].Template))
                    {
                        UpdateItem(m_items[fromSlot]);
                        return false;
                    }

                    if (!(m_player.CanEquip(m_items[fromSlot].Template) && m_items[fromSlot].IsValidItem()))
                    {
                        UpdateItem(m_items[fromSlot]);
                        return false;
                    }
                }
                if (IsEquipSlot(fromSlot))
                {
                    if (m_items[toSlot] != null && !CanEquipSlotContains(fromSlot, m_items[toSlot].Template))
                    {
                        UpdateItem(m_items[toSlot]);
                        return false;
                    }
                }
            }

            return base.MoveItem(fromSlot, toSlot, count);
        }

        public override void UpdateChangedPlaces()
        {
            int[] changedSlot = m_changedPlaces.ToArray();


            bool updateStyle = false;

            foreach (int i in changedSlot)
            {
                if (IsEquipSlot(i))
                {

                    ItemInfo item = GetItemAt(i);
                    if (item != null)
                    {
                        m_player.OnUsingItem(GetItemAt(i).TemplateID);//触发任务<使用道具>
                        item.IsBinds = true;
                        if (!item.IsUsed)
                        {
                            item.IsUsed = true;
                            item.BeginDate = DateTime.Now;
                        }
                    }
                    updateStyle = true;
                    break;
                }
            }
            base.UpdateChangedPlaces();
            if (updateStyle)
            {
                UpdatePlayerProperties();
            }
        }

        #region Build Player Style/Properties

        //private static readonly int[] StyleIndex = new int[] { 1, 2, 3, 4, 5, 6, 11, 13, 14 };
        //DDTAnk
        private static readonly int[] StyleIndex = new int[] { 1, 2, 3, 4, 5, 6, 11, 13, 14, 15, 16, 17, 18, 19, 20 };
        public void UpdatePlayerProperties()
        {
            m_player.BeginChanges();

            try
            {
                int attack = 0;
                int defence = 0;
                int agility = 0;
                int lucky = 0;
                int strengthenLevel = 0;
                string style = "";
                string color = "";
                string skin = "";
                ItemInfo weapon = null;

                lock (m_lock)
                {
                    style = m_items[0] == null ? "" : m_items[0].TemplateID.ToString();
                    color = m_items[0] == null ? "" : m_items[0].Color;
                    skin = m_items[5] == null ? "" : m_items[5].Skin;
                    weapon = m_items[6];

                    for (int i = 0; i < BAG_START; i++)
                    {
                        ItemInfo item = m_items[i];
                        if (item != null)
                        {
                            attack += item.Attack;
                            defence += item.Defence;
                            agility += item.Agility;
                            lucky += item.Luck;

                            //item.IsBinds=true;
                            strengthenLevel = strengthenLevel > item.StrengthenLevel ? strengthenLevel : item.StrengthenLevel;
                            AddProperty(item, ref  attack, ref  defence, ref  agility, ref   lucky);
                        }
                    }

                    EquipBuffer();
                    for (int i = 0; i < StyleIndex.Length; i++)
                    {
                        style += ",";
                        color += ",";
                        if (m_items[StyleIndex[i]] != null)
                        {
                            style += m_items[StyleIndex[i]].TemplateID;
                            color += m_items[StyleIndex[i]].Color;
                        }
                    }
                }

                m_player.UpdateBaseProperties(attack, defence, agility, lucky);
                m_player.UpdateStyle(style, color, skin);
                GetUserNimbus();
                m_player.ApertureEquip(strengthenLevel);
                m_player.UpdateWeapon(m_items[6]);
                m_player.UpdateSecondWeapon(m_items[15]);
                m_player.UpdateFightPower();
            }
            finally
            {
                m_player.CommitChanges();
            }
        }

        #endregion

        #region EquipSlot GetItemEpuipSlot/IsEquipSlot/CanEquipSlotContains

        /// <summary>
        /// 获取物品的装备位置
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int FindItemEpuipSlot(ItemTemplateInfo item)
        {
            switch (item.CategoryID)
            {
                case 8:
                    if (m_items[7] == null)
                    {
                        return 7;
                    }
                    else
                    {
                        return 8;
                    }
                case 9:
                    if (m_items[9] == null)
                    {
                        return 9;
                    }
                    else
                    {
                        return 10;
                    }
                case 13:
                    return 11;
                case 14:
                    return 12;
                case 15:
                    return 13;
                case 16:
                    return 14;
                default:
                    return item.CategoryID - 1;
            }
        }

        /// <summary>
        /// 装备的位置是否能装备此物品。
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="temp"></param>
        /// <returns></returns>
        public bool CanEquipSlotContains(int slot, ItemTemplateInfo temp)
        {
            if (temp.CategoryID == 8)
            {
                return slot == 7 || slot == 8;
            }
            else if (temp.CategoryID == 9)
            {
                if (temp.TemplateID == 9022 || temp.TemplateID == 9122 || temp.TemplateID == 9222 || temp.TemplateID == 9322 || temp.TemplateID == 9422 || temp.TemplateID == 9522)
                    return slot == 9 || slot == 10 || slot == 16;
                else
                    return slot == 9 || slot == 10;
            }
            else if (temp.CategoryID == 13)
            {
                return slot == 11;
            }
            else if (temp.CategoryID == 14)
            {
                return slot == 12;
            }
            else if (temp.CategoryID == 15)
            {
                return slot == 13;
            }
            else if (temp.CategoryID == 16)
            {
                return slot == 14;
            }
            else if (temp.CategoryID == 17)
            {
                return slot == 15;
            }
            else
            {
                return temp.CategoryID - 1 == slot;
            }
        }

        /// <summary>
        /// 是否为身上的插槽
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool IsEquipSlot(int slot)
        {
            return slot >= 0 && slot < BAG_START;
        }


        public void GetUserNimbus()
        {

            int i = 0;
            int j = 0;
            for (int m = 0; m < BAG_START; m++)
            {
                ItemInfo item = GetItemAt(m);
                if (item != null)
                {

                    if (item.StrengthenLevel >= 5 && item.StrengthenLevel <= 8)
                    {
                        if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
                        { i = i > 01 ? i : 01; }
                        if (item.Template.CategoryID == 7)
                        { j = j > 01 ? j : 01; }

                    }
                    if (item.StrengthenLevel == 9)
                    {
                        if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
                        { i = i > 01 ? i : 02; }
                        if (item.Template.CategoryID == 7)
                        { j = j > 01 ? j : 02; }

                    }
                    if (item.StrengthenLevel == 12)
                    {
                        if (item.Template.CategoryID == 1 || item.Template.CategoryID == 5)
                        { i = i > 01 ? i : 03; }
                        if (item.Template.CategoryID == 7)
                        { j = j > 01 ? j : 03; }

                    }

                }
                continue;

            }
            m_player.PlayerCharacter.Nimbus = i * 100 + j;
            //m_player.Out.SendUpdateAllSorce();//更新光环
            m_player.Out.SendUpdatePublicPlayer(m_player.PlayerCharacter);
        }


        /// <summary>
        /// 玩家装备Buffer
        /// </summary>
        public void EquipBuffer()
        {
            m_player.EquipEffect.Clear();//EquipBufferList.Clear();
            for (int m = 0; m < BAG_START; m++)
            {
                ItemInfo item = GetItemAt(m);
                if (item != null)
                {

                    if (item.Hole1 > 0)
                        m_player.EquipEffect.Add(item.Hole1);

                    if (item.Hole2 > 0)
                        m_player.EquipEffect.Add(item.Hole2);

                    if (item.Hole3 > 0)
                        m_player.EquipEffect.Add(item.Hole3);

                    if (item.Hole4 > 0)
                        m_player.EquipEffect.Add(item.Hole4);

                    if (item.Hole5 > 0)
                        m_player.EquipEffect.Add(item.Hole5);

                    if (item.Hole6 > 0)
                        m_player.EquipEffect.Add(item.Hole6);

                }
            }
        }

        public void AddProperty(ItemInfo item, ref int attack, ref int defence, ref int agility, ref  int lucky)
        {
            if (item != null)
            {

                if (item.Hole1 > 0)

                    AddBaseProperty(item.Hole1, ref attack, ref defence, ref agility, ref lucky);

                if (item.Hole2 > 0)
                    AddBaseProperty(item.Hole2, ref attack, ref defence, ref agility, ref lucky);

                if (item.Hole3 > 0)
                    AddBaseProperty(item.Hole3, ref attack, ref defence, ref agility, ref lucky);

                if (item.Hole4 > 0)
                    AddBaseProperty(item.Hole4, ref attack, ref defence, ref agility, ref lucky);

                if (item.Hole5 > 0)
                    AddBaseProperty(item.Hole5, ref attack, ref defence, ref agility, ref lucky);

                if (item.Hole6 > 0)
                    AddBaseProperty(item.Hole6, ref attack, ref defence, ref agility, ref lucky);

            }

        }

        public void AddBaseProperty(int templateid, ref int attack, ref int defence, ref int agility, ref  int lucky)
        {
            ItemTemplateInfo temp = ItemMgr.FindItemTemplate(templateid);
            if (temp != null)
            {
                if (temp.CategoryID == 11 && temp.Property1 == 31 && temp.Property2 == 3)
                {
                    attack += temp.Property3;
                    defence += temp.Property4;
                    agility += temp.Property5;
                    lucky += temp.Property6;
                }

            }
        }


        #endregion

    }
}
