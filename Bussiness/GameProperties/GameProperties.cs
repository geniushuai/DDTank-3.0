using System.Collections;
using System.Globalization;
using System;
using System.Reflection;
using log4net;
using Bussiness;
using SqlDataProvider.Data;
using Game.Base;
using Game.Base.Config;

namespace Bussiness
{
	public abstract class GameProperties
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ConfigProperty("Edition","当前游戏版本","11000")]
        public static readonly string EDITION;

        [ConfigProperty("MustComposeGold", "合成消耗金币价格", 1000)]
        public static readonly int PRICE_COMPOSE_GOLD;

        [ConfigProperty("MustFusionGold", "熔炼消耗金币价格", 1000)]
        public static readonly int PRICE_FUSION_GOLD;

        [ConfigProperty("MustStrengthenGold", "强化金币消耗价格", 1000)]
        public static readonly int PRICE_STRENGHTN_GOLD;

        [ConfigProperty("CheckRewardItem", "验证码奖励物品", 11001)]
        public static readonly int CHECK_REWARD_ITEM;

        [ConfigProperty("CheckCount", "最大验证码失败次数", 2)]
        public static readonly int CHECK_MAX_FAILED_COUNT;

        [ConfigProperty("HymenealMoney", "求婚的价格", 1000)]
        public static readonly int PRICE_PROPOSE;

        [ConfigProperty("DivorcedMoney", "离婚的价格", 1000)]
        public static readonly int PRICE_DIVORCED;

        [ConfigProperty("MarryRoomCreateMoney", "结婚房间的价格,2小时、3小时、4小时用逗号分隔", "2000,2700,3400")]
        public static readonly string PRICE_MARRY_ROOM;

        [ConfigProperty("BoxAppearCondition", "箱子物品提示的等级", 4)]
        public static readonly int BOX_APPEAR_CONDITION;

        [ConfigProperty("DisableCommands", "禁止使用的命令", "")]
        public static readonly string DISABLED_COMMANDS;

        [ConfigProperty("AssState","防沉迷系统的开关,True打开,False关闭",false)]
        public static bool ASS_STATE;

        [ConfigProperty("DailyAwardState","每日奖励开关,True打开,False关闭",true)]
        public static bool DAILY_AWARD_STATE;

        [ConfigProperty("Cess","交易扣税",0.10)]
        public static readonly double Cess;

        [ConfigProperty("BeginAuction", "拍买时起始随机时间", 20)]
        public static int BeginAuction;

        [ConfigProperty("EndAuction", "拍买时结束随机时间", 40)]
        public static int EndAuction;

		private static void Load(Type type)
		{
            using (ServiceBussiness sb = new ServiceBussiness())
            {
                foreach (FieldInfo f in type.GetFields())
                {
                    if (!f.IsStatic)
                        continue;
                    object[] attribs = f.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
                    if (attribs.Length == 0)
                        continue;
                    ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)attribs[0];
                    f.SetValue(null, GameProperties.LoadProperty(attrib, sb));
                }
            }
		}

        private static void Save(Type type)
        {
            using (ServiceBussiness sb = new ServiceBussiness())
            {
                foreach (FieldInfo f in type.GetFields())
                {
                    if (!f.IsStatic)
                        continue;
                    object[] attribs = f.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
                    if (attribs.Length == 0)
                        continue;
                    ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)attribs[0];
                    SaveProperty(attrib, sb, f.GetValue(null));
                }
            }
        }

        private static object LoadProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb)
        {
            String key = attrib.Key;
            ServerProperty property = sb.GetServerPropertyByKey(key);
            if (property == null)
            {
                property = new ServerProperty();
                property.Key = key;
                property.Value = attrib.DefaultValue.ToString();
                log.Error("Cannot find server property " + key + ",keep it default value!");
            }
            log.Debug("Loading " + key + " Value is " + property.Value);
            try
            {
                return Convert.ChangeType(property.Value, attrib.DefaultValue.GetType());
            }
            catch (Exception e)
            {
                log.Error("Exception in GameProperties Load: ", e);
                return null;
            }
        }

        private static void SaveProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb,object value)
        {
            try
            {
                sb.UpdateServerPropertyByKey(attrib.Key, value.ToString());
            }
            catch (Exception ex)
            {
                log.Error("Exception in GameProperties Save: ", ex);
            }
        }

		public static void Refresh()
		{
            log.Info("Refreshing game properties!");
            Load(typeof(GameProperties));
		}

        public static void Save()
        {
            log.Info("Saving game properties into db!");
            Save(typeof(GameProperties));
        }
	}
}
