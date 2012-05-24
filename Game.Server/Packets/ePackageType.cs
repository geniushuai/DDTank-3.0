namespace Game.Server.Packets
{
    public enum ePackageType
    {

        /**************************************系统*********************************************
         * */
        /// <summary>
        /// 服务器Ready
        /// </summary>
        SERVER_READY = 0x00,
        /* 
        * 登陆 
        */
        LOGIN = 0x01,

        BUFF_INFO=0x00,
        /**
         * 用户被踢下线 
         */
        KIT_USER = 0x02,

        /**
         * 系统消息 
         */
        SYS_MESS = 0x03,

        /**
         * ping 
         */
        PING = 0x04,

        /// <summary>
        /// 同步系统时间
        /// </summary>
        SYS_DATE = 0x05,

        /// <summary>
        /// 网络状态
        /// </summary>
        NETWORK = 0x06,

        /// <summary>
        /// RSA密钥
        /// </summary>
        RSAKEY = 0x07,

        /// <summary>
        /// 客户端日记
        /// </summary>
        CLIENT_LOG = 0x08,

        /// <summary>
        /// 充值
        /// </summary>
        CHARGE_MONEY = 0x09,


        /// <summary>
        /// 系统公告
        /// </summary>
        SYS_NOTICE = 0x0a,

        /// <summary>
        /// 活动领取
        /// </summary>
        ACTIVE_PULLDOWN = 0x0b,

        /// <summary>
        /// 版本错误
        /// </summary>
        EDITION_ERROR = 0x0c,

        /// <summary>
        /// 日常奖励
        /// </summary>
        DAILY_AWARD = 0x0d,

        /// <summary>
        ///  获得物品提示
        /// </summary>
        GET_ITEM_MESS = 0x0e,

        /// <summary>
        /// 新手答题
        /// </summary>
        USER_ANSWER=0x0f,

        /*************************************场景*********************************************
         * 登陆场景 
         */
        SCENE_LOGIN = 0x10,

        /**
         * 场景准备
         */
        SCENE_READY = 0x11,

        /**
         * 场景中加入用户 
         */
        SCENE_ADD_USER = 0x12,

        /**
         * 场景聊天
         */
        SCENE_CHAT = 0x13,
        /**
         * 场景表情
         */
        SCENE_SMILE = 0x14,

        /**
         * 场景中离开用户 
         */
        SCENE_REMOVE_USER = 0x15,

        /**
         * 场景中游戏开始 
         */
        SCENE_GAME_START = 0x16,

        /**
         * 场景中游戏结束 
         */
        SCENE_GAME_STOP = 0x17,
        /**
		 * 更改场景聊天频道
		 */
        SCENE_CHANNEL_CHANGE = 0x18,



        
        LOTTERY_OPEN_BOX=0x1a,
        LOTTERY_RANDOM_SELECT=0x1b,
        LOTTERY_FINISH=0x1c,
        LOTTERY_ALTERNATE_LIST=0x1d,
        LOTTERY_GET_ITEM=0x1e,
        /**************************************人物********************************************
        * 人物改变状态 
        */
        CHANGE_STATE = 0x20,

        /**
         * 更改昵称
         */
        CHANGE_NICKNAME = 0x21,

        /**
         * 人物动作 
         */
        AC_ACTION = 0x23,

        /**
         * 同步动作 
         */
        SYNCH_ACTION = 0x24,

        /**
         * 个人聊天
         */
        CHAT_PERSONAL = 0x25,
        /// <summary>
        /// 更新分数
        /// </summary>
        UPDATE_PRIVATE_INFO = 0x26,

        /// <summary>
        /// 保存形象
        /// </summary>
        SAVE_STYLE = 0x29,

        /// <summary>
        /// 删除物品
        /// </summary>
        DELETE_ITEM = 0x2a,

        /// <summary>
        /// 添加物品
        /// </summary>
        ADD_ITEM = 0x2b,

        /// <summary>
        /// 购买物品
        /// </summary>
        BUY_ITEM = 0x2c,

        /// <summary>
        /// 解除物品
        /// </summary>
        UNCHAIN_ITEM = 0x2f,
        /// <summary>
        /// 出售物品
        /// </summary>
        SEll_ITEM = 0x30,
        /// <summary>
        /// 改变物品位置
        /// </summary>
        CHANGE_PLACE_ITEM = 0x31,
        /// <summary>
        /// 拆分物品
        /// </summary>
        BREAK_ITEM = 0x32,
        /// <summary>
        /// 物品更新
        /// </summary>
        UPDATE_ITEM = 0x33,
        /// <summary>
        /// 装备物品
        /// </summary>
        CHAIN_ITEM = 0x34,

        /// <summary>
        /// 用户维护物品
        /// </summary>
        REPAIR_GOODS = 0x35,

        /// <summary>
        /// 购买道具
        /// </summary>
        PROP_BUY = 0x36,
        /// <summary>
        /// 出售道具
        /// </summary>
        PROP_SELL = 0x37,
        /// <summary>
        /// 道具装备
        /// </summary>
        PROP_CHAIN = 0x38,

        /// <summary>
        /// 赠送物品
        /// </summary>
        GOODS_PRESENT = 0x39,

        /// <summary>
        /// 物品合成
        /// </summary>
        ITEM_COMPOSE = 0x3a,

        /// <summary>
        /// 物品强化
        /// </summary>
        ITEM_STRENGTHEN = 0x3b,

        /// <summary>
        /// 隐藏装备
        /// </summary>
        ITEM_HIDE = 0x3c,

        /// <summary>
        /// 物品转移
        /// </summary>
        ITEM_TRANSFER = 0x3d,

        /// <summary>
        /// 续费
        /// </summary>
        ITEM_CONTINUE = 0x3e,

        /// <summary>
        /// 打开物品
        /// </summary>
        ITEM_OPENUP = 0x3f,

        /// <summary>
        /// 格子物品
        /// </summary>
        GRID_GOODS = 0x40,

        /// <summary>
        /// 格子辅助道具
        /// </summary>
        GRID_PROP = 0x41,

        /// <summary>
        /// 改变装备
        /// </summary>
        EQUIP_CHANGE = 0x42,


        /// <summary>
        /// 更新样式
        /// </summary>
        UPDATE_PlAYER_INFO = 0x43,

        /// <summary>
        /// 场景用户列表
        /// </summary>
        SCENE_USERS_LIST = 0x45,

        /// <summary>
        /// 邀请进入游戏
        /// </summary>
        GAME_INVITE = 0x46,

        /// <summary>
        /// 小喇叭
        /// </summary>
        S_BUGLE = 0x47,

        /// <summary>
        /// 大喇叭
        /// </summary>
        B_BUGLE = 0x48,

        /// <summary>
        /// 获取所有物品
        /// </summary>
        ITEM_OBTAIN = 0x49,

        /// <summary>
        /// 获取装备
        /// </summary>
        ITEM_EQUIP = 0x4a,

        /// <summary>
        /// 道具删除
        /// </summary>
        PROP_DELETE = 0x4b,

        /// <summary>
        /// 熔化预览
        /// </summary>
        ITEM_FUSION_PREVIEW = 0x4c,

        /// <summary>
        /// 熔化
        /// </summary>
        ITEM_FUSION = 0x4e,

        /// <summary>
        /// 物品过期
        /// </summary>
        ITEM_OVERDUE = 0x4d,

        /// <summary>
        ///物品储存
        /// </summary>
        ITEM_STORE = 0x4f,


        UPDATE_GP=0x44,

        ///<summary>
        ///物品炼化
        ///</summary>
        ITEM_REFINERY = 0x6e,


        ///<summary>
        ///炼化预览
        ///</summary>
        ITEM_REFINERY_PREVIEW = 0x6f,

        ///<summary>
        ///物品比较
        ///</summary>
        LINKREQUEST_GOODS = 0x77,

        ///<summary>
        ///物品倾向转移
        ///</summary>
        ITEM_TREND = 0x78,

        /// <summary>
        /// 物品镶嵌
        /// </summary>
        ITEM_INLAY = 0x79,

        //TrieuLSL
        REClAIM_GOODS = 0x7f,
        ITEM_EMBED_BACKOUT=0x7d,
        /************************************游戏****************************************
         * 组聊天
         */
        CHAT_GROUP = 0x50,

        GAME_ROOM_LOGIN = 0x51,

        /// <summary>
        /// 玩家进入
        /// </summary>
        GAME_PLAYER_ENTER = 0x52,

        /// <summary>
        /// 玩家退出
        /// </summary>
        GAME_PLAYER_EXIT = 0x53,

        /// <summary>
        /// 游戏开始
        /// </summary>
        GAME_START = 0x56,

        PLAYER_STATE = 0x57,

        /// <summary>
        /// PVE中点券不足
        /// </summary>
        INSUFFICIENT_MONEY = 0x58,

        GAME_VISITOR_DATA = 0x5d,

        /// <summary>
        /// 创建房间
        /// </summary>
        GAME_ROOM_CREATE = 0x5e,

        GAME_ROOMLIST_UPDATE = 0x5f,

        GAME_ROOM_CLEAR = 0x60,

        GAME_ROOM_HOST = 0x61,

        GAME_ROOM_KICK = 0x62,

        /// <summary>
        /// 更新房间位置
        /// </summary>
        GAME_ROOM_UPDATE_PLACE = 0x64,

        //GAME_ROOM_INFO = 0x65,

        GAME_TEAM = 0x66,

        //PLAY_INFO_IN_GAME = 0x67,

        GAME_CHANGE_MAP = 0x68,

        GAME_TEAM_TYPE = 0x69,

        /// <summary>
        /// 更新房间设置
        /// </summary>
        //GAME_ROOM_SETUP = 0x6b,
        GAME_ROOM_SETUP_CHANGE = 0x6b,

        GAME_TAKE_TEMP = 0x6c,

        GAME_ROOM_LIST = 0x6d,

        /************************************邮件****************************************
          * 删除邮件
          */
        DELETE_MAIL = 0x70,

        /// <summary>
        /// 获取邮件的附件到背包
        /// </summary>
        GET_MAIL_ATTACHMENT = 0x71,

        /// <summary>
        ///  修改邮件的已读未读标志
        /// </summary>
        UPDATE_MAIL = 0x72,

        /// <summary>
        ///  更新邮件
        /// </summary>
        UPDATE_NEW_MAIL = 0x73,

        /// <summary>
        /// 发送邮件
        /// </summary>
        SEND_MAIL = 0x74,

        /// <summary>
        /// 邮件响应
        /// </summary>
        MAIL_RESPONSE = 0x75,

        /// <summary>
        /// 取消付款邮件
        /// </summary>
        MAIL_CANCEL = 0x76,

        CLEAR_STORE_BAG= 0x7a,


        /*
         * 背包解锁
        */
        PASSWORD_TWO = 0x19,
        /**************************************公会********************************************
        /**
		 * 申请进入公会
		 */
        CONSORTIA_TRYIN = 0x81,
        /**
         * 申请创建公会
         */
        CONSORTIA_CREATE = 0x82,
        /// <summary>
        /// 解散公会
        /// </summary>
        CONSORTIA_DELETE = 0x83,
        /// <summary>
        /// 脱离公会
        /// </summary>renegade
        CONSORTIA_USERS_DELETE = 0x84,
        /**
        * 通过申请进入
        */
        CONSORTIA_TRYIN_PASS = 0x85,
        /**
         * 删除进入申请
         */
        CONSORTIA_TRYIN_DEL = 0x86,
        /**
         * 捐献财富
         */
        CONSORTIA_RICHES_OFFER = 0x87,
        ///**
        // * 成员降级
        // */
        //CONSORTIA_MEMBER_DESGRADE = 0x88,
        /// <summary>
        /// 公会申请状态
        /// </summary>
        CONSORTIA_APPLY_STATE = 0x88,
        /**
         * 添加职务
         */
        CONSORTIA_DUTY_ADD = 0x89,
        /**
         * 删除职务 
         */
        CONSORTIA_DUTY_DELETE = 0x8a,
        /**
         * 编辑职称
         */
        CONSORTIA_DUTY_UPDATE = 0x8b,
        /// <summary>
        /// 邀请
        /// </summary>
        CONSORTIA_INVITE = 0x8c,
        /// <summary>
        /// 邀请通过
        /// </summary>
        CONSORTIA_INVITE_PASS = 0x8e,
        /// <summary>
        /// 邀请删除
        /// </summary>
        CONSORTIA_INVITE_DELETE = 0x8f,
        /// <summary>
        /// 添加盟友申请
        /// </summary>
        CONSORTIA_ALLY_APPLY_ADD = 0x90,
        /// <summary>
        /// 盟友申请更新
        /// </summary>
        CONSORTIA_ALLY_APPLY_UPDATE = 0x91,
        /// <summary>
        /// 盟友申请删除
        /// </summary>
        CONSORTIA_ALLY_APPLY_DELETE = 0x92,
        /// <summary>
        /// 添加盟友
        /// </summary>
        CONSORTIA_ALLY_ADD = 0x93,
        /// <summary>
        /// 盟友删除
        /// </summary>
        CONSORTIA_ALLY_DELETE = 0x94,
        /// <summary>
        /// 更新公会介绍
        /// </summary>
        CONSORTIA_DESCRIPTION_UPDATE = 0x95,
        /// <summary>
        /// 更新公会公告
        /// </summary>
        CONSORTIA_PLACARD_UPDATE = 0x96,
        /// <summary>
        /// 禁言
        /// </summary>
        CONSORTIA_BANCHAT_UPDATE = 0x97,
        /// <summary>
        /// 用户备注
        /// </summary>
        CONSORTIA_USER_REMARK_UPDATE = 0x98,
        /// <summary>
        /// 用户职位
        /// </summary>
        CONSORTIA_USER_GRADE_UPDATE = 0x99,
        /// <summary>
        /// 转让会长
        /// </summary>
        CONSORTIA_CHAIRMAN_CHAHGE = 0x9a,

        /// <summary>
        /// 聊天
        /// </summary>
        CONSORTIA_CHAT = 0x9b,
        ///// <summary>
        ///// 公会贡献
        ///// </summary>
        //CONSORTIA_OFFER = 0x9c,
        ///// <summary>
        ///// 公会财富
        ///// </summary>
        //CONSORTIA_RICHES = 0x9d,

        /// <summary>
        /// 公会银行升级
        /// </summary>
        CONSORTIA_STORE_UPGRADE = 0x9c,

        /// <summary>
        /// 公会铁匠升级
        /// </summary>
        CONSORTIA_SMITH_UPGRADE = 0x9d,

        /// <summary>
        /// 公会商城升级
        /// </summary>
        CONSORTIA_SHOP_UPGRADE = 0x9e,

        ///// <summary>
        ///// 公会战
        ///// </summary>
        //CONSORTIA_FIGHT = 0x9e,

        /// <summary>
        /// 公会升级
        /// </summary>
        CONSORTIA_UPGRADE = 0x9f,


        /**************************************好友********************************************/
        /// <summary>
        /// 添加好友
        /// </summary>
        FRIEND_ADD = 0xa0,
        /// <summary>
        /// 删除好友
        /// </summary>
        FRIEND_REMOVE = 0xa1,
        /// <summary>
        /// 更新好友备注
        /// </summary>
        FRIEND_UPDATE = 0xa2,
        /// <summary>
        /// 好友登陆
        /// </summary>
        FRIEND_LOGIN = 0xa3,
        /// <summary>
        /// 好友离开
        /// </summary>
        FRIEND_LOGOUT = 0xa4,
        /// <summary>
        /// 好友状态
        /// </summary>
        FRIEND_STATE = 0xa5,

        /// <summary>
        /// 被添加好友响应
        /// </summary>
        FRIEND_RESPONSE = 0xa6,

        /// <summary>
        /// 公会设备控制
        /// </summary>
        CONSORTIA_EQUIP_CONTROL = 0xaa,


        /**************************************任务********************************************/
        /// <summary>
        /// 添加
        /// </summary>
        QUEST_ADD = 0xb0,
        /// <summary>
        /// 删除
        /// </summary>
        QUEST_REMOVE = 0xb1,
        /// <summary>
        /// 更新
        /// </summary>
        QUEST_UPDATE = 0xb2,

        /// <summary>
        /// 完成
        /// </summary>
        QUEST_FINISH = 0xb3,

        /// <summary>
        /// 获取全部<未用到>
        /// </summary>
        QUSET_OBTAIN = 0xb4,

        /// <summary>
        /// 客服端查检<未用到>
        /// </summary>
        QUEST_CHECK = 0xb5,



        /**************************************buff********************************************/
        /// <summary>
        /// 卡片使用
        /// </summary>
        ITEM_CHANGE_COLOR = 0xb6,
        /// <summary>
        /// 卡片使用
        /// </summary>
        CARD_USE = 0xb7,
        /// <summary>
        /// 添加BUFF
        /// </summary>
        BUFF_ADD = 0xb8,
        /// <summary>
        /// 更新BUFF
        /// </summary>
        BUFF_UPDATE = 0xb9,

        /// <summary>
        /// 获取全部
        /// </summary>
        BUFF_OBTAIN = 0xba,

        /**************************************拍卖********************************************/
        /// <summary>
        /// 添加拍卖
        /// </summary>
        AUCTION_ADD = 0xc0,
        /// <summary>
        /// 更新拍卖
        /// </summary>
        AUCTION_UPDATE = 0xc1,
        /// <summary>
        /// 撤消拍卖
        /// </summary>
        AUCTION_DELETE = 0xc2,
        /// <summary>
        /// 刷新拍卖
        /// </summary>
        AUCTION_REFRESH = 0xc3,

        /**************************************验证码********************************************/

        /// <summary>
        /// 验证码
        /// </summary>
        CHECK_CODE = 0xc8,

        /**************************************撮合系统********************************************/
        /// <summary>
        /// 撮合等待
        /// </summary>
        GAME_PAIRUP_START = 0xd0,
        /// <summary>
        /// 撮合失败
        /// </summary>
        GAME_PAIRUP_FAILED = 0xd1,
        /// <summary>
        /// 撮合取消
        /// </summary>
        GAME_PAIRUP_CANCEL = 0xd2,
        /// <summary>
        /// 撮合房间设置
        /// </summary>
        GAME_PAIRUP_ROOM_SETUP = 0xd3,

        /**************************************防沉迷系统********************************************/
        /// <summary>
        /// 获取防沉迷系统状态
        /// </summary>
        AAS_STATE_GET = 0xe0,
        /// <summary>
        /// 设置防沉迷系统信息
        /// </summary>
        AAS_INFO_SET = 0xe0,
        /// <summary>
        /// 身份证信息验证
        /// </summary>
        AAS_IDNUM_CHECK = 0xe2,
        /// <summary>
        /// 防沉迷系统开关
        /// </summary>
        AAS_CTRL = 0xe3,
        CADDY_SELL_ALL_GOODS=0xe8,
        /// <summary>
        /// 结婚场景切换
        /// </summary>
        MARRY_SCENE_CHANGE = 0xe9,

        /// <summary>
        /// 获取结婚相关信息
        /// </summary>
        MARRYPROP_GET = 0xea,

        /**************************************民政大厅********************************************/
        /// <summary>
        /// 获取征婚信息
        /// </summary>
        MARRYINFO_GET = 0xeb,
        /// <summary>
        /// 添加征婚信息
        /// </summary>
        MARRYINFO_ADD = 0xec,
        /// <summary>
        /// 更新征婚信息
        /// </summary>
        MARRYINFO_UPDATE = 0xed,
        /// <summary>
        /// 撤消征婚信息
        /// </summary>
        MARRYINFO_DELETE = 0xee,
        /// <summary>
        /// 刷新征婚信息
        /// </summary>
        MARRYINFO_REFRESH = 0xef,


        /**************************************结婚系统********************************************/
        /// <summary>
        /// 登陆结婚列表场景
        /// </summary>
        MARRY_SCENE_LOGIN = 0xf0,
        /// <summary>
        /// 礼堂创建
        /// </summary>
        MARRY_ROOM_CREATE = 0xf1,

        /// <summary>
        /// 进入礼堂
        /// </summary>
        MARRY_ROOM_LOGIN = 0xf2,

        /// <summary>
        /// 通知玩家进入
        /// </summary>
        PLAYER_ENTER_MARRY_ROOM = 0xf3,

        /// <summary>
        /// 玩家退出礼堂
        /// </summary>a
        PLAYER_EXIT_MARRY_ROOM = 0xf4,


        CADDY_GET_AWARDS=0xf5,


        /// <summary>
        /// 结婚状态
        /// </summary>
        MARRY_STATUS = 0xf6,
        /// <summary>
        /// 求婚
        /// </summary>
        MARRY_APPLY = 0xf7,
        /// <summary>
        /// 离婚
        /// </summary>
        DIVORCE_APPLY = 0xf8,
        /// <summary>
        /// 礼堂命令
        /// </summary>
        MARRY_CMD = 0xf9,
        /// <summary>
        /// 求婚答复
        /// </summary>
        MARRY_APPLY_REPLY = 0xfa,

        /// <summary>
        /// 当前场景状态
        /// </summary>
        SCENE_STATE = 0xfb,

        /// <summary>
        /// 是否创建结婚礼堂
        /// </summary>
        MARRY_ROOM_STATE = 0xfc,

        /// <summary>
        /// 更新礼堂信息
        /// </summary>
        MARRY_ROOM_INFO_UPDATE = 0xfd,

        /// <summary>
        /// 结婚礼堂销毁
        /// </summary>
        MARRY_ROOM_DISPOSE = 0xfe,

        /// <summary>
        /// 礼堂信息更新
        /// </summary>
        MARRY_ROOM_UPDATE = 0xff,
        //HOTSPRING FUNCTION;
        HOTSPRING_CMD = 0xbf,
        HOTSPRING_ENTER = 187,
        HOTSPRING_ROOM_CREATE = 175,
        HOTSPRING_ROOM_REMOVE = 174,
        HOTSPRING_ROOM_ADD_OR_UPDATE = 173,
        HOTSPRING_ROOM_LIST_GET = 197,
        HOTSPRING_ROOM_QUICK_ENTER = 190,
        HOTSPRING_ROOM_ENTER = 202,
        HOTSPRING_ROOM_ENTER_VIEW = 201,
        HOTSPRING_ROOM_PLAYER_ADD = 198,
        HOTSPRING_ROOM_PLAYER_REMOVE = 169,
        HOTSPRING_ROOM_PLAYER_REMOVE_NOTICE = 199,
        HOTSPRING_ROOM_ENTER_CONFIRM = 212,
        USER_GET_GIFTS = 218,
        USER_SEND_GIFTS = 221,
        USER_UPDATE_GIFT = 220,
        USER_RELOAD_GIFT = 214,
        CARDS_DATA = 216,
        CARD_RESET = 196
        //OPTION_UPDATE = 64,
        //GET_PLAYER_CARD = 18

    }
}
