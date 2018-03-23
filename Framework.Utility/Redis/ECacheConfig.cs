using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Utility.Redis
{
    public class ECacheConfig
    {
        /// <summary>
        /// 缓存名称
        /// </summary>
        public string CacheName { get; set; }

        /// <summary>
        /// 超时时间,分钟数(如果为null则永不过期)
        /// </summary>
        public int? TimeOut { get; set; }

        /// <summary>
        /// 是否自动延时.(如果自动延时,TimeOut无效)
        /// </summary>
        public bool AutoDely { get; set; }
    }
}
