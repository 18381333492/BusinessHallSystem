using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Utility.Model
{
    public class ResponeResult
    {
        private bool _success=false;
        private bool _login = true;

        /// <summary>
        /// 本次请求的成功的标识
        /// </summary>
        public bool success
        {
            get{return _success; }
            set{_success= value;}
        } 

        /// <summary>
        /// 消息描述
        /// </summary>
        public string info
        {
            get;
            set;
        } 

        /// <summary>
        /// 返回的数据
        /// </summary>
        public object data
        {
            get;
            set;
        }

        /// <summary>
        /// 是否登录
        /// </summary>
        public bool login
        {
            get { return _login; }
            set { _login = value; }
        }
    }
}
