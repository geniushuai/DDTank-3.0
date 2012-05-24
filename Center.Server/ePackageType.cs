using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Center.Server
{
    public enum ePackageType
    {
        /// <summary>
        /// RSA密钥
        /// </summary>
        RSAKey = 0x0,

        /// <summary>
        /// 用户登陆
        /// </summary>
        LOGIN = 0x1,

        /// <summary>
        /// 踢用户下线
        /// </summary>
        KITOFF_USER = 0x2,

        /// <summary>
        /// 允许用户登陆
        /// </summary>
        ALLOW_USER_LOGIN = 0x3,

        /// <summary>
        /// 用户下线
        /// </summary>
        USER_OFFLINE = 0x4,

        /// <summary>
        /// 用户上线
        /// </summary>
        USER_ONLINE = 0x5,

        /// <summary>
        /// 更新用户状态
        /// </summary>
        USER_STATE = 0x6,

        /// <summary>
        /// update ass state
        /// </summary>
        UPDATE_ASS = 0x07,

        /// <summary>
        /// 更改配置文件状态
        /// </summary>
        UPDATE_CONFIG_STATE= 0x08,

        /// <summary>
        /// 充值
        /// </summary>
        CHARGE_MONEY = 0x09,

        /// <summary>
        /// 场景聊天
        /// </summary>
        SCENE_CHAT = 0x13,

        /// <summary>
        /// 系统公告
        /// </summary>
        SYS_NOTICE = 0x0a,

        /// <summary>
        /// 重新加载
        /// </summary>
        SYS_RELOAD = 0x0b,

        /// <summary>
        /// 网络测试
        /// </summary>
        PING = 0x0c,


        /// <summary>
        /// 更新人物结婚属性
        /// </summary>
        UPDATE_PLAYER_MARRIED_STATE = 0x0d,

        /// <summary>
        /// 发送结婚房间信息
        /// </summary>
        MARRY_ROOM_INFO_TO_PLAYER = 0x0e,

        /// <summary>
        /// 关闭服务器
        /// </summary>
        SHUTDOWN = 0x0f,

        /// <summary>
        /// 私聊
        /// </summary>
        CHAT_PERSONAL = 0x25,

        /// <summary>
        /// 系统消息转发
        /// </summary>
        SYS_MESS = 0x26,

        /// <summary>
        /// 大喇叭
        /// </summary>
        B_BUGLE = 0x48,

        /// <summary>
        /// 邮件响应
        /// </summary>
        MAIL_RESPONSE = 0x75,

        /**************************************公会********************************************/
        /// <summary>
        /// 公会响应
        /// </summary>
        CONSORTIA_RESPONSE = 0x80,
        /// <summary>
        /// 公会创建
        /// </summary>
        CONSORTIA_CREATE = 0x82,
        /// <summary>
        /// 解散公会
        /// </summary>
        CONSORTIA_DELETE = 0x83,
        /// <summary>
        /// 添加盟友
        /// </summary>
        CONSORTIA_ALLY_ADD = 0x93,
        /// <summary>
        /// 聊天
        /// </summary>
        CONSORTIA_CHAT = 0x9b,
        /// <summary>
        /// 公会贡献
        /// </summary>
        CONSORTIA_OFFER = 0x9c,
        /// <summary>
        /// 公会财富
        /// </summary>
        CONSORTIA_RICHES = 0x9d,

        /// <summary>
        /// 公会战
        /// </summary>
        CONSORTIA_FIGHT=0x9e,

        /// <summary>
        /// 公会升级
        /// </summary>
        CONSORTIA_UPGRADE = 0x9f,

        /// <summary>
        /// 好友状态
        /// </summary>
        FRIEND_STATE = 0xa5,

        /// <summary>
        /// 被添加好友响应
        /// </summary>
        FRIEND_RESPONSE = 0xa6,

        /// <summary>
        /// 倍率调整,(经验,工会战财富,功勋)
        /// </summary>
        Rate = 0xb1,

        /// <summary>
        /// 武平掉落宏观控制
        /// </summary>
        MACRO_DROP = 0xb2,

        /// <summary>
        /// 服务器的IP，端口
        /// </summary>
        IP_PORT = 0xf0,


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
