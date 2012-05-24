using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using SqlDataProvider.Data;
using System.Threading;
using log4net;
using System.Reflection;
using Game.Server.GameObjects;


namespace Game.Server.Managers
{
    public class RefineryMgr
    {
        private static Dictionary<int, RefineryInfo> m_Item_Refinery = new Dictionary<int, RefineryInfo>();

        //private static Dictionary<int, List<int>> m_Equip = new Dictionary<int, List<int>>();

        //private static Dictionary<int, string> m_Item = new Dictionary<int, string>();

        //private static Dictionary<int, List<int>> m_Reward = new Dictionary<int, List<int>>();

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static ThreadSafeRandom rand = new ThreadSafeRandom();

        public static bool Init()
        {
            return Reload();
        }

        public static bool Reload()
        {
            try
            {
                Dictionary<int, RefineryInfo> Temp_Refinery = new Dictionary<int, RefineryInfo>();

                Temp_Refinery = LoadFromBD();

                if (Temp_Refinery.Count > 0)
                {
                    Interlocked.Exchange(ref m_Item_Refinery, Temp_Refinery);
                }
                return true;
            }
            catch (Exception e)
            {
                log.Error("NPCInfoMgr", e);
            }
            return false;
        }



        public static Dictionary<int, RefineryInfo> LoadFromBD()
        {
            List<RefineryInfo> infos = new List<RefineryInfo>();
            Dictionary<int, RefineryInfo> Temp_Refinery = new Dictionary<int, RefineryInfo>();


            using (ProduceBussiness db = new ProduceBussiness())
            {

                infos = db.GetAllRefineryInfo();

                foreach (RefineryInfo info in infos)
                {


                    //List<int> list = new List<int>();

                    //list.Add(info.RefineryID);
                    //list.Add(info.Item1);
                    //list.Add(info.Item2);
                    //list.Add(info.Item3);
                    //list.Add(info.Item4);

                    //list.Sort();

                    //StringBuilder Items = new StringBuilder();

                    //foreach (int i in list)
                    //{
                    //    Items.Append(i);
                    //}

                    //string key = Items.ToString();

                    if (!Temp_Refinery.ContainsKey(info.RefineryID))
                    {
                        Temp_Refinery.Add(info.RefineryID, info);
                    }
                }
            }

            return Temp_Refinery;
        }


        public static ItemTemplateInfo Refinery(GamePlayer player, List<ItemInfo> Items, ItemInfo Item, bool Luck, int OpertionType, ref bool result, ref int defaultprobability, ref bool IsFormula)
        {
            ItemTemplateInfo TempItem = new ItemTemplateInfo();


            foreach (int i in m_Item_Refinery.Keys)
            {
                if (m_Item_Refinery[i].m_Equip.Contains(Item.TemplateID))
                {
                    IsFormula = true;
                    int j = 0;
                    List<int> Template = new List<int>();
                    foreach (ItemInfo info in Items)
                    {
                        if (info.TemplateID == m_Item_Refinery[i].Item1 && info.Count >= m_Item_Refinery[i].Item1Count && !Template.Contains(info.TemplateID))
                        {
                            Template.Add(info.TemplateID);
                            if (OpertionType != 0)
                            {
                                info.Count -= m_Item_Refinery[i].Item1Count;
                            }
                            j++;
                        }
                        if (info.TemplateID == m_Item_Refinery[i].Item2 && info.Count >= m_Item_Refinery[i].Item2Count && !Template.Contains(info.TemplateID))
                        {
                            Template.Add(info.TemplateID);
                            if (OpertionType != 0)
                            {
                                info.Count -= m_Item_Refinery[i].Item2Count;
                            }
                            j++;
                        }
                        if (info.TemplateID == m_Item_Refinery[i].Item3 && info.Count >= m_Item_Refinery[i].Item3Count && !Template.Contains(info.TemplateID))
                        {
                            Template.Add(info.TemplateID);
                            if (OpertionType != 0)
                            {
                                info.Count -= m_Item_Refinery[i].Item3Count;
                            }
                            j++;
                        }
                        //if (info.TemplateID == m_Item_Refinery[i].Item4 && info.Count >= m_Item_Refinery[i].Item4Count && !Template.Contains(info.TemplateID))
                        //{
                        //    Template.Add(info.TemplateID);
                        //    info.Count -= m_Item_Refinery[i].Item4Count;
                        //    j++;
                        //}
                    }

                    if (j == 3)
                    {
                        //foreach (int x in m_Item_Refinery[i].m_Reward)
                        //{
                        for (int m = 0; m < m_Item_Refinery[i].m_Reward.Count; m++)
                        {
                            if (Items[Items.Count - 1].TemplateID == m_Item_Refinery[i].m_Reward[m])
                            {
                                int TempItemID;
                                //  int defaultprobability = 25;
                                if (Luck)
                                {
                                    defaultprobability += 20;
                                }

                                if (OpertionType == 0)
                                {
                                    TempItemID = m_Item_Refinery[i].m_Reward[m + 1];

                                    return Bussiness.Managers.ItemMgr.FindItemTemplate(TempItemID);
                                }
                                else
                                {
                                    if (rand.Next(100) < defaultprobability)
                                    {
                                        TempItemID = m_Item_Refinery[i].m_Reward[m + 1];

                                        result = true;

                                        return Bussiness.Managers.ItemMgr.FindItemTemplate(TempItemID);
                                    }
                                }
                                // }


                            }
                        }
                    }

                }

                else
                {
                    IsFormula = false;
                }


            }
            return null;
        }

        public static ItemTemplateInfo RefineryTrend(int Operation, ItemInfo Item, ref bool result)
        {
            if (Item != null)
            {
                foreach (int i in m_Item_Refinery.Keys)
                {
                    if (m_Item_Refinery[i].m_Reward.Contains(Item.TemplateID))
                    {
                        for (int j = 0; j < m_Item_Refinery[i].m_Reward.Count; j++)
                        {
                            if (m_Item_Refinery[i].m_Reward[j] == Operation)
                            {
                                int TemplateId = m_Item_Refinery[i].m_Reward[j + 2];

                                result = true;

                                return Bussiness.Managers.ItemMgr.FindItemTemplate(TemplateId);
                            }

                        }

                    }
                }
            }

            return null;
        }

       

    }
}
