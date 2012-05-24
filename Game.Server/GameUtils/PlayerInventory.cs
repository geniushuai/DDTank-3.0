using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Bussiness;
using Game.Server.Packets;

namespace Game.Server.GameUtils
{
    public class PlayerInventory : AbstractInventory
    {
        protected GamePlayer m_player;

        private bool m_saveToDb;

        private List<ItemInfo> m_removedList = new List<ItemInfo>();

        public PlayerInventory(GamePlayer player, bool saveTodb, int capibility, int type, int beginSlot, bool autoStack)
            : base(capibility, type, beginSlot, autoStack)
        {
            m_player = player;
            m_saveToDb = saveTodb;
        }

        public virtual void LoadFromDatabase()
        {
            if (m_saveToDb)
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    ItemInfo[] list = pb.GetUserBagByType(m_player.PlayerCharacter.ID, BagType);

                    BeginChanges();

                    try
                    {
                        foreach (ItemInfo item in list)
                        {
                            AddItemTo(item, item.Place);
                        }
                    }
                    finally
                    {
                        CommitChanges();
                    }
                }
            }
        }

        public virtual void SaveToDatabase()
        {
            if (m_saveToDb)
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    lock (m_lock)
                    {
                        for (int i = 0; i < m_items.Length; i++)
                        {
                            ItemInfo item = m_items[i];
                            if (item != null && item.IsDirty)
                            {
                                if (item.ItemID > 0)
                                {
                                    pb.UpdateGoods(item);
                                }
                                else
                                {
                                    pb.AddGoods(item);
                                }
                            }
                        }
                    }

                    lock (m_removedList)
                    {
                        foreach (ItemInfo item in m_removedList)
                        {
                            if (item.ItemID > 0)
                            {
                                pb.UpdateGoods(item);
                            }
                        }
                        m_removedList.Clear();
                    }
                }
            }
        }

        public override bool AddItemTo(ItemInfo item, int place)
        {
            if (base.AddItemTo(item, place))
            {
                item.UserID = m_player.PlayerCharacter.ID;
                item.IsExist = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool TakeOutItem(ItemInfo item)
        {
            if (base.TakeOutItem(item))
            {
                if (m_saveToDb)
                {
                    lock (m_removedList)
                    {
                        m_removedList.Add(item);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool RemoveItem(ItemInfo item)
        {
            if (base.RemoveItem(item))
            {
                item.IsExist = false;

                if (m_saveToDb)
                {
                    lock (m_removedList)
                    {
                        m_removedList.Add(item);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void UpdateChangedPlaces()
        {
            int[] changedPlaces = m_changedPlaces.ToArray();
            m_player.Out.SendUpdateInventorySlot(this, changedPlaces);
            base.UpdateChangedPlaces();
        }

        /// <summary>
        /// 群发邮件
        /// </summary>
        public bool SendAllItemsToMail(string sender,string title,eMailType type)
        {
            if (m_saveToDb)
            {
                BeginChanges();
                try
                {
                    using (PlayerBussiness pb = new PlayerBussiness())
                    {
                        lock (m_lock)
                        {
                            List<ItemInfo> items = GetItems();

                            int count = items.Count;
                            for (int i = 0; i < count; i += 5)
                            {
                                MailInfo mail = new MailInfo();
                                mail.SenderID = 0;
                                mail.Sender = sender;
                                mail.ReceiverID = m_player.PlayerCharacter.ID;
                                mail.Receiver = m_player.PlayerCharacter.NickName;
                                mail.Title = title;
                                mail.Type = (int)type;
                                mail.Content = "";

                                List<ItemInfo> list = new List<ItemInfo>();
                                for(int j = 0; j < 5; j ++)
                                {
                                    int index = i * 5 + j;
                                    if(index < items.Count)
                                    {
                                        list.Add(items[index]);
                                    }
                                }
                                if (SendItemsToMail(list, mail, pb) == false)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Send Items Mail Error:" + ex);
                }
                finally
                {
                    SaveToDatabase();
                    CommitChanges();
                }
                m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
            }
            return true;
        }

        public bool SendItemsToMail(List<ItemInfo> items, MailInfo mail,PlayerBussiness pb)
        {
            if (mail == null)   return false;
            if (items.Count > 5) return false;

            if (m_saveToDb)
            {
                List<ItemInfo> sent = new List<ItemInfo>();
                StringBuilder annexRemark = new StringBuilder();
                annexRemark.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
                
                if (items.Count > 0 && TakeOutItem(items[0]))
                {
                    ItemInfo it = items[0];

                    mail.Annex1 = it.ItemID.ToString();
                    mail.Annex1Name = it.Template.Name;
                    annexRemark.Append("1、" + mail.Annex1Name + "x" + it.Count + ";");
                    sent.Add(it);
                }

                if (items.Count > 1 && TakeOutItem(items[1]))
                {
                    ItemInfo it = items[1];

                    mail.Annex2 = it.ItemID.ToString();
                    mail.Annex2Name = it.Template.Name;
                    annexRemark.Append("2、" + mail.Annex2Name + "x" + it.Count + ";");
                    sent.Add(it);
                }


                 if (items.Count > 2 && TakeOutItem(items[2]))
                {
                    ItemInfo it = items[2];

                    mail.Annex3 = it.ItemID.ToString();
                    mail.Annex3Name = it.Template.Name;
                    annexRemark.Append("3、" + mail.Annex3Name + "x" + it.Count + ";");
                     sent.Add(it);
                }

                if (items.Count > 3 && TakeOutItem(items[3]))
                {
                    ItemInfo it = items[3];

                    mail.Annex4 = it.ItemID.ToString();
                    mail.Annex4Name = it.Template.Name;
                    annexRemark.Append("4、" + mail.Annex4Name + "x" + it.Count + ";");
                    sent.Add(it);
                }

                if (items.Count > 4 && TakeOutItem(items[4]))
                {
                    ItemInfo it = items[4];

                    mail.Annex5 = it.ItemID.ToString();
                    mail.Annex5Name = it.Template.Name;
                    annexRemark.Append("5、" + mail.Annex5Name + "x" + it.Count + ";");
                    sent.Add(it);
                }

                mail.AnnexRemark = annexRemark.ToString();

                if (pb.SendMail(mail))
                {
                    return true;
                }
                else
                {
                    foreach (ItemInfo it in sent)
                    {
                        AddItem(it);
                    }
                }
            }
            return false;
 
        }
        /// <summary>
        /// 寄邮单个物品<存入数据库中>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool SendItemToMail(ItemInfo item)
        {
            if (m_saveToDb)
            {
                using (PlayerBussiness pb = new PlayerBussiness())
                {
                    return SendItemToMail(item, pb, null);
                }
            }
            return false;
        }

        /// <summary>
        /// 发送物品到邮箱中
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pb"></param>
        /// <returns></returns>
        public bool SendItemToMail(ItemInfo item, PlayerBussiness pb, MailInfo mail)
        {
            if (m_saveToDb && item.BagType == BagType)
            {
                if (mail == null)
                {
                    mail = new MailInfo();
                    mail.Annex1 = item.ItemID.ToString();
                    mail.Content = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title");
                    mail.Gold = 0;
                    mail.IsExist = true;
                    mail.Money = 0;
                    mail.Receiver = m_player.PlayerCharacter.NickName;
                    mail.ReceiverID = item.UserID;
                    mail.Sender = m_player.PlayerCharacter.NickName;
                    mail.SenderID = item.UserID;
                    mail.Title = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title");
                    mail.Type = (int)eMailType.ItemOverdue;
                }
                if (pb.SendMail(mail))
                {
                    RemoveItem(item);
                    item.IsExist = true;
                    return true;
                }
                return false;
            }
            return false;
        }

        


        public void MoveToStore(PlayerInventory bag, int fromSolt, int toSolt, PlayerInventory tobag, int maxCount)
        {

            ItemInfo item = bag.GetItemAt(fromSolt);
            ItemInfo toItem = tobag.GetItemAt(toSolt);


            if (item == null && toItem == null)
                return;

            if (toItem == null && (toSolt < 0 || toSolt > 80) || (tobag.BagType == 11 && toSolt >= maxCount))
            {
                if (tobag.BagType == 11 || tobag.BagType == 1)

                    toSolt = tobag.FindFirstEmptySlot(0);
                //else if(bag.BagType==12&&tobag.BagType==0){
                //    toSolt=tobag.FindFirstEmptySlot(7);
                //}
                else
                    toSolt = tobag.FindFirstEmptySlot(31);

            }

            if ((item.Template.CategoryID == 10 || item.Template.CategoryID == 11 || item.Template.CategoryID == 12) && tobag.BagType != 0 ||
                ((item.Template.CategoryID != 10 && item.Template.CategoryID != 11 && item.Template.CategoryID != 12) && tobag.BagType != 1))
            {
                if (toItem != null && item != null)
                {

                    int place = item.Place;
                    int bagtype = item.BagType;
                    int place1 = toItem.Place;
                    int bagtype1 = toItem.BagType;

                    bag.RemoveItem(item);
                    tobag.RemoveItem(toItem);

                    item.IsExist = true;
                    item.BagType = bagtype1;
                   
                    tobag.AddItemTo(item, place1);


                    toItem.IsExist = true;
                    toItem.BagType = bagtype;
                    if (place == -1 && (eBageType)bag.BagType == eBageType.MainBag)
                    {
                        place = bag.FindFirstEmptySlot(31);
                    }
                    if (place < 31) place = bag.FindFirstEmptySlot(31);
                    bag.AddItemTo(toItem, place);

                }
                else
                {
                    if (toItem != null)
                    {
                        tobag.RemoveItem(item);
                        bag.AddItemTo(item, toSolt);
                    }

                    if (item != null)
                    {
                        if (tobag.BagType == 11 && toSolt >= maxCount)
                        {
                            UpdateItem(item);
                            return;
                        }
                        else
                        {
                            bag.RemoveItem(item);
                            tobag.AddItemTo(item, toSolt);


                        }
                    }

                }
            }
            else
            {
                if (toItem != null)
                    UpdateItem(toItem);
                if (item != null)
                    UpdateItem(item);
            }
        }

    }
}
