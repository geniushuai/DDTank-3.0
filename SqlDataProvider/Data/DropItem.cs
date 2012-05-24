using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    /// <summary>
    /// 掉落物品表
    /// </summary>
    public class DropItem
    {
        #region 掉落物品表
        
        /// <summary>
        /// 序号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 掉落物品
        /// </summary>
        public int DropId { get; set; }

        /// <summary>
        /// 物品编号
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public int ValueDate { get; set; }

        /// <summary>
        /// 是否绑定
        /// </summary>
        public bool IsBind { get; set; }

        /// <summary>
        /// 机率
        /// </summary>
        public int Random { get; set; }

        /// <summary>
        /// 开始数量
        /// </summary>
        public int BeginData { get; set; }

        /// <summary>
        /// 结束数量
        /// </summary>
        public int EndData { get; set; }

        #endregion
    }
}
