using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Statics
{
    public enum LogMoneyType
    {
        /// <summary>
        /// 拍买
        /// </summary>
        Auction = 1,

        /// <summary>
        /// 邮寄
        /// </summary>
        Mail = 2,

        /// <summary>
        /// 商城
        /// </summary>
        Shop = 3,

        /// <summary>
        /// 结婚
        /// </summary>
        Marry = 4,

        /// <summary>
        /// 工会
        /// </summary>
        Consortia = 5,

        /// <summary>
        /// 物品
        /// </summary>
        Item = 6,

        /// <summary>
        /// 充值
        /// </summary>
        Charge = 7,

        /// <summary>
        /// 奖励
        /// </summary>
        Award = 8,

        /// <summary>
        /// 开箱子
        /// </summary>
        Box = 9,

        /// <summary>
        /// 游戏战斗
        /// </summary>
        Game = 10,

        /// <summary>
        /// 拍卖更新
        /// </summary>
        Auction_Update = 101,

        /// <summary>
        /// 邮件中提取点卷
        /// </summary>
        Mail_Money = 201,

        /// <summary>
        /// 付费邮件
        /// </summary>
        Mail_Pay = 202,

        /// <summary>
        /// 邮件点卷
        /// </summary>
        Mail_Send = 203,
        /// <summary>
        /// 商城购买
        /// </summary>
        Shop_Buy = 301,

        /// <summary>
        /// 商城继费
        /// </summary>
        Shop_Continue = 302,

        /// <summary>
        /// 卡片使用
        /// </summary>
        Shop_Card = 303,

        /// <summary>
        /// 商店赠送
        /// </summary>
        Shop_Present = 304,

        /// <summary>
        /// 求婚
        /// </summary>
        Marry_Spark = 401,

        /// <summary>
        /// 举行婚礼
        /// </summary>
        Marry_Stage = 402,

        /// <summary>
        /// 发放礼金
        /// </summary>
        Marry_Gift = 403,

        /// <summary>
        /// 结婚房间继费
        /// </summary>
        Marry_Follow = 404,

        /// <summary>
        /// 离婚
        /// </summary>
        Marry_Unmarry = 405,

        /// <summary>
        /// 创建礼堂
        /// </summary>
        Marry_Room = 406,

        /// <summary>
        /// 礼堂加时
        /// </summary>
        Marry_RoomAdd = 407,

        /// <summary>
        /// 婚礼烟花
        /// </summary>
        Marry_Flower = 408,

        /// <summary>
        /// 结婚典礼
        /// </summary>
        Marry_Hymeneal = 410,

        /// <summary>
        /// 公会捐赠
        /// </summary>
        Consortia_Rich = 412,
        /// <summary>
        /// 物品转移
        /// </summary>
        Item_Move = 601,

        /// <summary>
        /// 物品颜色
        /// </summary>
        Item_Color = 602,
        /// <summary>
        /// 在线充值
        /// </summary>
        Charge_RMB = 701,

        /// <summary>
        /// 每日奖励
        /// </summary>
        Award_Daily = 801,

        /// <summary>
        /// 任务奖励
        /// </summary>
        Award_Quest = 802,

        /// <summary>
        /// 掉落
        /// </summary>
        Award_Drop = 803,

        /// <summary>
        /// 答题
        /// </summary>
        Award_Answer = 804,

        /// <summary>
        /// Boss掉落
        /// </summary>
        Award_BossDrop = 805,

        /// <summary>
        /// 翻牌
        /// </summary>
        Award_TakeCard = 806,

        /// <summary>
        /// 开箱子
        /// </summary>
        Box_Open = 901,

        /// <summary>
        /// boos战
        /// </summary>
        Game_Boos = 1001,

        /// <summary>
        /// 付费翻牌
        /// </summary>
        Game_PaymentTakeCard = 1002,

        /// <summary>
        /// 再试一次
        /// </summary>
        Game_TryAgain = 1003,

        /// <summary>
        /// 战斗中其他支付
        /// </summary>
        Game_Other = 1004,

        /// <summary>
        /// 开炮掉钱
        /// </summary>
        Game_Shoot = 1005,
    }
}
