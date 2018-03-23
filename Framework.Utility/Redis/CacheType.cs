using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Utility.Redis
{
    public enum CacheType
    {
        /// <summary>
        /// 前段用户信息
        /// </summary>
        UserInfo,

        /// <summary>
        /// 会话
        /// </summary>
        Session
    }
}
