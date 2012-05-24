using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Server.Packets
{
    public enum eDutyRightType
    {
        /// <summary>
        /// 审核申请
        /// </summary>
        _1_Ratify = 0x0001,
        /// <summary>
        /// 邀请加入
        /// </summary>
        _2_Invite = 0x0002,
        /// <summary>
        /// 禁止/允许发言
        /// </summary>
        _3_BanChat = 0x0004,
        /// <summary>
        /// 编辑公告
        /// </summary>
        _4_Notice = 0x0008,
        /// <summary>
        /// 修改宣言
        /// </summary>
        _5_Enounce = 0x0010,
        /// <summary>
        /// 逐出公会
        /// </summary>
        _6_Expel = 0x0020,
        /// <summary>
        /// 工会外交
        /// </summary>
        _7_Diplomatism = 0x0040,
        /// <summary>
        /// 职位管理
        /// </summary>
        _8_Manage = 0x0080,
        /// <summary>
        /// 工会升级
        /// </summary>
        _9_ConsortiaUp = 0x0100,
        /// <summary>
        /// 转让会长
        /// </summary>
        _10_ChangeMan = 0x0200,
        /// <summary>
        /// 解散工会
        /// </summary>
        _11_Disband = 0x0400,
        /// <summary>
        /// 升级/降级
        /// </summary>
        _12_UpGrade = 0x0800,
        /// <summary>
        /// 退出公会
        /// </summary>
        _13_Exit = 0x1000,
    }
}
