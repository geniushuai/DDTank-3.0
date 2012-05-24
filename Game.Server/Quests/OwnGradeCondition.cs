using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using Game.Server.GameObjects;

namespace Game.Server.Quests
{
    /// <summary>
    /// 1、升级/空/等级数
    /// 触发条件：用户登陆、升级时触发
    /// </summary>
    public class OwnGradeCondition:BaseCondition
    {
        #region 1、升级/空/等级数
        public OwnGradeCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value) { }
        /// <summary>
        /// 添加一个用户升级消息
        /// </summary>
        /// <param name="player"></param>
        public override void AddTrigger(Game.Server.GameObjects.GamePlayer player)
        {
            
        }

        /// <summary>
        /// 用户升级
        /// </summary>
        /// <param name="grade"></param>
        void player_UpdateGrade(int grade)
        {
            
        }

        /// <summary>
        /// 移除一个事件
        /// </summary>
        /// <param name="player"></param>
        public override void RemoveTrigger(Game.Server.GameObjects.GamePlayer player)
        {
            
        }

        /// <summary>
        /// 判断是否完成
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public override bool IsCompleted(GamePlayer player)
        {
            if (player.Level >= m_info.Para2)
            {
                Value = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
