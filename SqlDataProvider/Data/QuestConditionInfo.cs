using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    #region 任务条件
    public class QuestConditionInfo
    {
        #region 任务条件
        
        /// <summary>
        /// 任务编号
        /// </summary>
        public int QuestID {get;set;}

        /// <summary>
        /// 条件序号
        /// </summary>
        public int CondictionID { get; set; }

        /// <summary>
        /// 条件描述
        /// </summary>
        public string CondictionTitle { get; set; }

        /// <summary>
        /// 条件类型
        /// </summary>
        public int CondictionType {get;set;}
        
        /// <summary>
        /// 控制字段1
        /// </summary>
        public int Para1{get;set;}

        /// <summary>
        /// 控制字段2
        /// </summary>
        public int Para2{get;set;}

        
        #endregion Model
    }
    #endregion
}
