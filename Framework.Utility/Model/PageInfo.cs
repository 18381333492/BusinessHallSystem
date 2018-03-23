using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Framework.Utility.Model
{
    public class PageInfo
    {

        private int _PageIndex = 1;
        private int _PageSize = 10;

        /// <summary>
        /// 当前页索引
        /// </summary>
        
        public int PageIndex
        {
            get { return _PageIndex; }
            set { _PageIndex = value; }
        }

        /// <summary>
        /// 页面数据的大小
        /// </summary>
        public int PageSize
        {
            get { return _PageSize; }
            set { _PageSize = value; }
        }
    }
}
