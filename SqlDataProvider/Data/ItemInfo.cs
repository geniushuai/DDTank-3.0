using System;
using System.Collections.Generic;


namespace SqlDataProvider.Data
{
    public class ItemInfo : DataObject
    {
        private ItemTemplateInfo _template;
        private ShopItemInfo _Goods;
        //public ItemInfo(ItemTemplateInfo temp, ShopItemInfo Goods)
        //{
        //    _template = temp;
        //    _Goods = Goods;
        //}

        public ItemInfo(ItemTemplateInfo temp)
        {
            _template = temp;
        }

        public ItemTemplateInfo Template
        {
            get
            {
                return _template;
            }
        }

        //public ShopItemInfo Goods
        //{
        //    get
        //    {
        //        return _Goods;
        //    }
        //}
        private int _itemID;
        public int ItemID
        {
            set
            {
                _itemID = value;
                _isDirty = true;
            }
            get
            {
                return _itemID;
            }
        }

        private int _userID;
        public int UserID
        {
            set
            {
                _userID = value;
                _isDirty = true;
            }
            get
            {
                return _userID;
            }
        }

        private int _bagType;
        public int BagType
        {
            set { _bagType = value; _isDirty = true; }
            get { return _bagType; }
        }

        private int _templateId;
        public int TemplateID
        {
            set { _templateId = value; _isDirty = true; }
            get { return _templateId; }
        }

        //private int _APlaceType;
        //public int APlaceType
        //{
        //    set { _APlaceType = value; _isDirty = true; }
        //    get { return _APlaceType; }
        //}

        //private int _BPlaceType;
        //public int BPlaceType
        //{
        //    set { _BPlaceType = value; _isDirty = true; }
        //    get { return _BPlaceType; }
        //}

        //private int _CPlaceType;
        //public int CPlaceType
        //{
        //    set { _CPlaceType = value; _isDirty = true; }
        //    get { return _CPlaceType; }
        //}

        //private int _AVaule;
        //public int AVaule
        //{
        //    set { _AVaule = value; _isDirty = true; }
        //    get { return _AVaule; }
        //}

        //private int _BVaule;
        //public int BVaule
        //{
        //    set { _BVaule = value; _isDirty = true; }
        //    get { return _BVaule; }
        //}

        //private int _CVaule;
        //public int CVaule
        //{
        //    set { _CVaule = value; _isDirty = true; }
        //    get { return _CVaule; }
        //}
        private int _place;
        public int Place
        {
            set { _place = value; _isDirty = true; }
            get { return _place; }
        }

        private int _count;
        public int Count
        {
            set { _count = value; _isDirty = true; }
            get { return _count; }
        }

        private bool _isJudage;
        public bool IsJudge
        {
            set { _isJudage = value; _isDirty = true; }
            get { return _isJudage; }
        }

        private string _color;
        public string Color
        {
            set { _color = value; _isDirty = true; }
            get { return _color; }
        }

        private bool _isExist;
        public bool IsExist
        {
            set { _isExist = value; _isDirty = true; }
            get { return _isExist; }
        }

        private int _strengthenLevel;
        public int StrengthenLevel
        {
            set { _strengthenLevel = value; _isDirty = true; }
            get { return _strengthenLevel; }
        }

        private int _attackCompose;
        public int AttackCompose
        {
            set { _attackCompose = value; _isDirty = true; }
            get { return _attackCompose; }
        }

        private int _defendCompose;
        public int DefendCompose
        {
            set { _defendCompose = value; _isDirty = true; }
            get { return _defendCompose; }
        }

        private int _luckCompose;
        public int LuckCompose
        {
            set { _luckCompose = value; _isDirty = true; }
            get { return _luckCompose; }
        }

        private int _agilityCompose;
        public int AgilityCompose
        {
            set { _agilityCompose = value; _isDirty = true; }
            get { return _agilityCompose; }
        }

        private bool _isBinds;
        public bool IsBinds
        {
            set
            {
                //if (value && !_isBinds)
                //    _beginDate = DateTime.Now;
                _isBinds = value;
                _isDirty = true;
            }
            get { return _isBinds; }
        }

        private bool _isUsed;
        public bool IsUsed
        {
            set
            {
                if (_isUsed != value)
                {
                    _isUsed = value;
                    _isDirty = true;
                }
            }
            get { return _isUsed; }
        }

        private string _skin;
        public string Skin
        {
            set { _skin = value; _isDirty = true; }
            get { return _skin; }
        }

        private DateTime _beginDate;
        public DateTime BeginDate
        {
            set { _beginDate = value; _isDirty = true; }
            get { return _beginDate; }
        }

        private int _validDate;
        public int ValidDate
        {
            set { _validDate = value; _isDirty = true; }
            get { return _validDate; }
        }

        private DateTime _removeDate;
        public DateTime RemoveDate
        {
            set { _removeDate = value; _isDirty = true; }
            get { return _removeDate; }
        }

        private int _removeType;
        public int RemoveType
        {
            set { _removeType = value; _removeDate = DateTime.Now; _isDirty = true; }
            get { return _removeType; }
        }

        private int _hole1;
        public int Hole1
        {
            get { return _hole1; }
            set { _hole1 = value; _isDirty = true; }
        }

        private int _hole2;
        public int Hole2
        {
            get { return _hole2; }
            set { _hole2 = value; _isDirty = true; }
        }

        private int _hole3;
        public int Hole3
        {
            get { return _hole3; }
            set { _hole3 = value; _isDirty = true; }
        }

        private int _hole4;
        public int Hole4
        {
            get { return _hole4; }
            set { _hole4 = value; _isDirty = true; }
        }

        private int _hole5;
        public int Hole5
        {
            get { return _hole5; }
            set { _hole5 = value; _isDirty = true; }
        }

        private int _hole6;
        public int Hole6
        {
            get { return _hole6; }
            set { _hole6 = value; _isDirty = true; }
        }


        public int Attack
        {
            get
            {
                //return (100 + _attackCompose) * _template.Attack / 100;
                return _attackCompose + _template.Attack;
            }
        }

        public int Defence
        {
            get
            {
                //return (100 + _defendCompose) * _template.Defence / 100;
                return _defendCompose + _template.Defence;
            }
        }

        public int Agility
        {
            get
            {
                //return (100 + _agilityCompose) * _template.Agility / 100;
                return _agilityCompose + _template.Agility;
            }
        }

        public int Luck
        {
            get
            {
                //return (100 + _luckCompose) * _template.Luck / 100;
                return _luckCompose + _template.Luck;
            }
        }

        public ItemInfo Clone()
        {
            ItemInfo c = new ItemInfo(_template);
            c._userID = _userID;
            c._validDate = _validDate;
            c._templateId = _templateId;
            c._strengthenLevel = _strengthenLevel;
            //c._APlaceType = _APlaceType;
            //c._BPlaceType = _BPlaceType;
            //c._CPlaceType = _CPlaceType;
            //c._AVaule = _AVaule;
            //c._BVaule = _BVaule;
            //c._CVaule = _CVaule;

            c._luckCompose = _luckCompose;
            c._itemID = 0;
            c._isJudage = _isJudage;
            c._isExist = _isExist;
            c._isBinds = _isBinds;
            c._isUsed = _isUsed;
            c._defendCompose = _defendCompose;
            c._count = _count;
            c._color = _color;
            c.Skin = _skin;
            c._beginDate = _beginDate;
            c._attackCompose = _attackCompose;
            c._agilityCompose = _agilityCompose;
            c._bagType = _bagType;
            c._isDirty = true;
            c._removeDate = _removeDate;
            c._removeType = _removeType;
            c._hole1 = _hole1;
            c._hole2 = _hole2;
            c._hole3 = _hole3;
            c._hole4 = _hole4;
            c._hole5 = _hole5;
            c._hole6 = _hole6;

            return c;
        }

        public bool IsValidItem()
        {
            if (_validDate != 0 && _isUsed)
            {
                return DateTime.Compare(_beginDate.AddDays(_validDate), DateTime.Now) > 0;
            }
            return true;
        }

        public bool CanStackedTo(ItemInfo to)
        {
            if (_templateId == to.TemplateID && Template.MaxCount > 1 && _isBinds == to.IsBinds && _isUsed == to._isUsed )
            {
                if (ValidDate == 0 || (BeginDate == to.BeginDate && ValidDate == ValidDate))
                {
                    return true;
                }
            }
            return false;
        }

        public int GetBagType()
        {
            switch (_template.CategoryID)
            {
                case 10:
                case 11:
                    return 1;
                case 12:
                    return 2;
                default:
                    return 0;
            }
        }

        public bool CanEquip()
        {
            if (_template.CategoryID < 10 || (_template.CategoryID >= 13 && _template.CategoryID <= 16))
                return true;

            return false;
        }

        public string GetBagName()
        {
            switch (_template.CategoryID)
            {
                case 10:
                case 11:
                    //return "道具";
                    return "Game.Server.GameObjects.Prop";
                case 12:
                    //return "任务";
                    return "Game.Server.GameObjects.Task";
                default:
                    //return "装备";
                    return "Game.Server.GameObjects.Equip";
            }
        }

        public static ItemInfo CreateFromTemplate(ItemTemplateInfo goods, int count, int type)
        {
            if (goods == null)
                return null;

            ItemInfo userGoods = new ItemInfo(goods);
            userGoods.AgilityCompose = 0;
            userGoods.AttackCompose = 0;
            userGoods.BeginDate = DateTime.Now;
            userGoods.Color = "";
            userGoods.Skin = "";
            userGoods.DefendCompose = 0;
            userGoods.IsBinds = false;
            userGoods.IsUsed = false;
            userGoods.IsDirty = false;
            userGoods.IsExist = true;
            userGoods.IsJudge = true;
            userGoods.LuckCompose = 0;
            userGoods.StrengthenLevel = 0;
            userGoods.TemplateID = goods.TemplateID;
            userGoods.ValidDate = 0;
            userGoods._template = goods;
            userGoods.Count = count;
            userGoods.IsBinds = goods.BindType == 1 ? true : false;
            userGoods._removeDate = DateTime.Now;
            userGoods._removeType = type;
            userGoods.Hole1 = -1;
            userGoods.Hole2 = -1;
            userGoods.Hole3 = -1;
            userGoods.Hole4 = -1;
            userGoods.Hole5 = -1;
            userGoods.Hole6 = -1;


            OpenHole(ref userGoods);
            return userGoods;
        }


        public static void FindSpecialItemInfo(ItemInfo info, ref int gold, ref int money, ref int giftToken)
        {            
            switch (info.TemplateID)
            {
                case -100:
                    gold += info.Count;
                    info=null;
                    break;
                case -200:
                    money += info.Count;
                    info=null;
                    break;
                case -300:
                    giftToken += info.Count;
                    info = null;
                    break;
                default:                    
                    break;
            }
        }


        /// <summary>
        /// 获取物品花费的金钱
        /// </summary>
        /// <param name="shop">商店物品</param>
        /// <param name="type">购买类型：以时间范围或以数量多少为单位</param>
        /// <param name="gold">花费金币</param>
        /// <param name="money">花费点券</param>
        /// <param name="offer">花费功勋</param>
        /// <param name="gifttoken">花费礼券</param>
        /// <param name="?">兑换物品（物品编号+数量）</param>
        /// <returns>返回</returns>
        public static List<int> SetItemType(ShopItemInfo shop, int type, ref int gold, ref int money, ref int offer, ref int gifttoken)
        {
            double pay = 0;
            int value = 0;
            int iTemplateID = 0;
            int iCount = 0;
            //gold = 0;
            //money = 0;
            //offer = 0;
            //gifttoken = 0;
            List<int> itemsInfo = new List<int>();

            if (type == 1)
            {
                GetItemPrice(shop.APrice1, shop.AValue1, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);

                if (iTemplateID > 0)
                {
                    itemsInfo.Add(iTemplateID);
                    itemsInfo.Add(iCount);
                }


                GetItemPrice(shop.APrice2, shop.AValue2, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);

                if (iTemplateID > 0)
                {
                    itemsInfo.Add(iTemplateID);
                    itemsInfo.Add(iCount);
                }

                GetItemPrice(shop.APrice3, shop.AValue3, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);

                if (iTemplateID > 0)
                {
                    itemsInfo.Add(iTemplateID);
                    itemsInfo.Add(iCount);
                }
            }
            if (type == 2)
            {
                GetItemPrice(shop.BPrice1, shop.BValue1, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);

                if (iTemplateID > 0)
                {
                    itemsInfo.Add(iTemplateID);
                    itemsInfo.Add(iCount);
                }


                GetItemPrice(shop.BPrice2, shop.BValue2, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);

                if (iTemplateID > 0)
                {
                    itemsInfo.Add(iTemplateID);
                    itemsInfo.Add(iCount);
                }

                GetItemPrice(shop.BPrice3, shop.BValue3, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);

                if (iTemplateID > 0)
                {
                    itemsInfo.Add(iTemplateID);
                    itemsInfo.Add(iCount);
                }

            }
            if (type == 3)
            {
                GetItemPrice(shop.CPrice1, shop.CValue1, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);

                if (iTemplateID > 0)
                {
                    itemsInfo.Add(iTemplateID);
                    itemsInfo.Add(iCount);
                }


                GetItemPrice(shop.CPrice2, shop.CValue2, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);

                if (iTemplateID > 0)
                {
                    itemsInfo.Add(iTemplateID);
                    itemsInfo.Add(iCount);
                }

                GetItemPrice(shop.CPrice3, shop.CValue3, shop.Beat, ref gold, ref money, ref offer, ref gifttoken, ref iTemplateID, ref iCount);

                if (iTemplateID > 0)
                {
                    itemsInfo.Add(iTemplateID);
                    itemsInfo.Add(iCount);
                }
            }
            return itemsInfo;
        }

        public static void GetItemPrice(int Prices, int Values, decimal beat, ref int gold, ref int money, ref int offer, ref int gifttoken, ref int iTemplateID, ref int iCount)
        {
            iTemplateID = 0;
            iCount = 0;
            switch (Prices)
            {
                case -1:  //-1表示点券
                    money += (int)(Values * beat);
                    break;
                case -2:  //-2表示金币
                    gold += (int)(Values * beat);
                    break;
                case -3:  //-3表示功勋
                    offer += (int)(Values * beat);
                    break;
                case -4:  //-4表示礼劵
                    gifttoken += (int)(Values * beat);
                    break;
                default:
                    if (Prices > 0)
                    {
                        iTemplateID = Prices;
                        iCount = Values;
                    }
                    break;
            }
        }





        public static ItemInfo SetItemType(ItemInfo item, int type, ref int gold, ref int money, ref int offer)
        {
            //double pay = 0;
            //int value = 0;
            //switch (type)
            //{
            //    case 2:                                        
            //        pay = item.Template.Price2 * item.Template.Agio2;
            //        value = item.Template.Value2;
            //        break;
            //    case 3:
            //        pay = item.Template.Price3 * item.Template.Agio3;
            //        value = item.Template.Value3;
            //        break;
            //    default:
            //        pay = item.Template.Price1 * item.Template.Agio1;
            //        value = item.Template.Value1;
            //        break;
            //}

            ////if (value < 0)
            //if (pay < 0 || value < 0)
            //{
            //    item = null;
            //    return null;
            //}

            //if (item.Template.PayType == 0)
            //{
            //    gold += (int)pay;
            //}
            //else if (item.Template.PayType == 1)
            //{
            //    money += (int)pay;
            //}
            //else
            //{
            //    offer += (int)pay;
            //}

            ////else
            ////{
            ////    if (item.Template.PayType == 1)
            ////    {
            ////        money += (int)pay;
            ////    }
            ////    else
            ////    {
            ////        offer += (int)pay;
            ////    }
            ////}

            //if (item.Template.BuyType == 0)
            //{
            //    item.ValidDate = value;
            //}
            //else
            //{
            //    item.Count = value;
            //}

            return item;
        }
        /// <summary>
        /// 设置开孔个数
        /// </summary>
        /// <param name="item"></param>
        public static void OpenHole(ref ItemInfo item)
        {
            string[] Hole = item.Template.Hole.Split('|');
            for (int i = 0; i < Hole.Length; i++)
            {
                string[] NeedLevel = Hole[i].Split(',');
                if (item.StrengthenLevel >= Convert.ToInt32(NeedLevel[0]) && Convert.ToInt32(NeedLevel[1]) != -1)
                {
                    switch (i)
                    {
                        case 0:
                            if (item.Hole1 < 0)
                                item.Hole1 = 0;
                            break;
                        case 1:
                            if (item.Hole2 < 0)
                                item.Hole2 = 0;
                            break;
                        case 2:
                            if (item.Hole3 < 0)
                                item.Hole3 = 0;
                            break;
                        case 3:
                            if (item.Hole4 < 0)
                                item.Hole4 = 0;
                            break;
                        case 4:
                            if (item.Hole5 < 0)
                                item.Hole5 = 0;
                            break;
                        case 5:
                            if (item.Hole6 < 0)
                                item.Hole6 = 0;
                            break;


                    }
                }

            }
        }
    }
}
