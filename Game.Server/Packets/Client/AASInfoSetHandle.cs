using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Bussiness;
using SqlDataProvider.Data;
using System.Text.RegularExpressions;
using Game.Server.GameUtils;
using Game.Server.Statics;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.AAS_INFO_SET, "设置防沉迷系统信息")]
    class AASInfoSetHandle : IPacketHandler
    {
        private static Regex _objRegex1 = new Regex("/^[1-9]\\d{7}((0\\d)|(1[0-2]))(([0|1|2]\\d)|3[0-1])\\d{3}$/");
        private static Regex _objRegex2 = new Regex("/^[1-9]\\d{5}[1-9]\\d{3}((0\\d)|(1[0-2]))(([0|1|2]\\d)|3[0-1])\\d{4}$/");
        private static Regex _objRegex = new Regex("\\d{18}|\\d{15}" );
        private static string[] cities = { null,null,null,null,null,null,null,null,null,null,null,
	                                     "北京","天津","河北","山西","内蒙古",null,null,null,null,null,
	                                     "辽宁","吉林","黑龙江",null,null,null,null,null,null,null,
	                                     "上海","江苏","浙江","安微","福建","江西","山东",null,null,null,
	                                     "河南","湖北","湖南","广东","广西","海南",null,null,null,"重庆",
	                                     "四川","贵州","云南","西藏",null,null,null,null,null,null,"陕西",
	                                     "甘肃","青海","宁夏","新疆",null,null,null,null,null,"台湾",null,
	                                     null,null,null,null,null,null,null,null,"香港","澳门",null,null,
	                                     null,null,null,null,null,null,"国外"};
        private static int[] WI = { 7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2 };
        private static char[] checkCode = { '1','0','X','9','8','7','6','5','4','3','2' };

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            AASInfo info = new AASInfo();
            info.UserID = client.Player.PlayerCharacter.ID;
            bool result = false;
            bool rlt = false;

            bool isclosed = packet.ReadBoolean();
            if (isclosed)
            {
                info.Name = "";
                info.IDNumber = "";
                info.State = 0;
                result = true;
            }
            else 
            {
                info.Name = packet.ReadString();
                info.IDNumber = packet.ReadString();
                result = CheckIDNumber(info.IDNumber);
                if (info.IDNumber != "")
                {
                    client.Player.IsAASInfo = true;
                    //result = false;
                    int Age = Convert.ToInt32(info.IDNumber.Substring(6, 4));
                    int month = Convert.ToInt32(info.IDNumber.Substring(10, 2));
                    if (DateTime.Now.Year.CompareTo(Age + 18) > 0 || (DateTime.Now.Year.CompareTo(Age + 18) == 0 && DateTime.Now.Month.CompareTo(month) >= 0))
                        client.Player.IsMinor = false;

                }
                if (info.Name != "" && result)
                {
                    info.State = 1;
                }
                else
                {
                    info.State = 0;
                }

            }

            if (result)
            {
                client.Out.SendAASState(false);
                client.Out.SendAASControl(false, client.Player.IsAASInfo, client.Player.IsMinor);
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    rlt = db.AddAASInfo(info);
                    client.Out.SendAASInfoSet(rlt);
                }
            }

            if (rlt && (info.State == 1) )
            {
                ItemTemplateInfo rewardItem = Bussiness.Managers.ItemMgr.FindItemTemplate(11019);
                if (rewardItem != null)
                {
                    ItemInfo item = ItemInfo.CreateFromTemplate(rewardItem, 1, (int)ItemAddType.Other);
                    if (item != null)
                    {
                        item.IsBinds = true;
                        AbstractInventory bg = client.Player.GetItemInventory(item.Template);
                        if (bg.AddItem(item, bg.BeginSlot))
                        {
                            client.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ASSInfoSetHandle.Success", item.Template.Name));
                            
                        }
                        else
                        {
                            client.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ASSInfoSetHandle.NoPlace"));
                        }
                    }
                }

            }
            return 0;
        }

        private bool CheckIDNumber(string IDNum)
        {
            bool result = false;
            //格式验证
            //if (!_objRegex1.IsMatch(IDNum) && !_objRegex2.IsMatch(IDNum) )
            //{
            //    return false;
            //}

            if (!_objRegex.IsMatch(IDNum))
            {
                return false;
            }

            //省份地址验证
            int province = int.Parse(IDNum.Substring(0,2));
            if (cities[province] == null)
            {
                return false;
            }

            //校验码验证
            if(IDNum.Length == 18)
            {
                int sum = 0;
                int y = 0;
                for (int i = 0; i < 17;i++ )
                {
                    sum += int.Parse(IDNum[i].ToString()) * WI[i];
                }

                y = sum % 11;
                if (IDNum[17] == checkCode[y])
                {
                    result = true;
                }

            }
            return result;
        }
    }
}
