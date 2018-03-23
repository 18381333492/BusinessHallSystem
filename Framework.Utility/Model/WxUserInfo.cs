using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Utility.Model
{
    /// <summary>
    /// 微信用户信息实体Model
    /// </summary>
    public class WxUserInfo
    {
        public string openId { get; set; }
        public string nickName { get; set; }
        public string gender { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string country { get; set; }
        public string avatarUrl { get; set; }
        public string unionId { get; set; }
        public Watermark watermark { get; set; }   
    }

    public class Watermark
    {
        public string appid { get; set; }
        public string timestamp { get; set; }
    }
}
