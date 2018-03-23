using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Utility.Model
{
    public class PageResult
    {
         /// <summary>
        /// 当前页码数
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 数据总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 分页的结果
        /// </summary>
        public object Rows { get; set; }

        /// <summary>
        /// 其它数据
        /// </summary>
        public object Data { get; set; }

    }
}
