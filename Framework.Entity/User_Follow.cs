using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Utility.Attributes;

namespace Framework.Entity
{
    /// <summary>
    /// 用户关注实体
    /// </summary>
    public class User_Follow
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SEQ("SEQ_USERFOLLOW_AUTO_ID")]
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int User_Id
        {
            get;
            set;
        }

        /// <summary>
        /// 关注类型（0：营业厅，1：营业员）
        /// </summary>
        public int Follow_Type
        {
            get;
            set;
        }

        /// <summary>
        /// 关注对象编号（营业厅或营业员）
        /// </summary>
        public int Follow_Num
        {
            get;
            set;
        }

        /// <summary>
        /// 关注时间
        /// </summary>
        public DateTime Follow_Time
        {
            get;
            set;
        }
    }
}
