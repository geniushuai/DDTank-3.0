using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    /// <summary>
    /// 掉落条件表
    /// </summary>
    public class DropCondiction
    {
        /// <summary>
        /// 掉落物品编号
        /// </summary>
        public int DropId { get; set; }

        /// <summary>
        /// 掉落类型
        /// </summary>
        public int CondictionType { get; set; }
 

        /// <summary>
        /// 条件值1
        /// </summary>
        public string Para1 { get; set; }

        /// <summary>
        /// 条件值2
        /// </summary>
        public string Para2 { get; set; }

    }
}
