using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Packets
{
    public enum eMailType
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,
        /// <summary>
        /// 普通邮件
        /// </summary>
        Common = 1,
        /// <summary>
        /// 拍卖成功
        /// </summary>
        AuctionSuccess = 2,
        /// <summary>
        /// 拍卖失败
        /// </summary>
        AuctionFail = 3,
        /// <summary>
        /// 竞标成功
        /// </summary>
        BidSuccess = 4,
        /// <summary>
        /// 竞标失败
        /// </summary>
        BidFail = 5,
        /// <summary>
        /// 返回付款
        /// </summary>
        ReturnPayment = 6,
        /// <summary>
        /// 付款退回
        /// </summary>
        PaymentCancel = 7,
        /// <summary>
        /// 购买物品
        /// </summary>
        BuyItem = 8,
        /// <summary>
        /// 物品过期
        /// </summary>
        ItemOverdue = 9,
        /// <summary>
        /// 赠送物品
        /// </summary>
        PresentItem = 10,

        /// <summary>
        /// 付款
        /// </summary>
        PaymentFinish = 11,

        /// <summary>
        /// 打开物品
        /// </summary>
        OpenUpArk = 12,
        /// <summary>
        /// 公会退回
        /// </summary>
        StoreCanel = 13,
        /// <summary>
        /// 结婚
        /// </summary>
        Marry = 14,
        /// <summary>
        /// 日常奖励
        /// </summary>
        DailyAward = 15,




        /// <summary>
        /// 后台管理
        /// </summary>
        Manage = 51,
        /// <summary>
        /// 活动
        /// </summary>
        Active = 52,


        /// <summary>
        /// 付款邮件
        /// </summary>
        Payment = 101,

    }

    public enum eMailRespose
    {
        /// <summary>
        /// 加载接收
        /// </summary>
        Receiver = 1,
        /// <summary>
        /// 加载发送
        /// </summary>
        Send = 2,
        /// <summary>
        /// 接收和发送
        /// </summary>
        ReceAndSend = 3,


    }
}
