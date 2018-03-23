using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Utility.Model
{
    /// <summary>
    /// 前段缓存用户信息的实体
    /// </summary>
    [Serializable]
    public class UserInfo
    {

        /// <summary>
        /// 用户主键ID
        /// </summary>
        public int User_Id
        {
            get;
            set;
        }

        /// <summary>
        /// 营业员的Id
        /// </summary>
        public int Waiter_Id
        {
            get;
            set;
        }

        /// <summary>
        /// sessionId
        /// </summary>
        public string sessionId
        {
            get;
            set;
        }

        /// <summary>
        /// 用户openid信息
        /// </summary>
        public string OpenId
        {
            get;
            set;
        }

        /// <summary>
        /// 缓存的SessionKey
        /// </summary>
        public string SessionKey
        {
            get;
            set;
        }

        /// <summary>
        /// 缓存的UnionId
        /// </summary>
        public string UnionId
        {
            get;
            set;
        }

        /// <summary>
        /// 0-普通用户,1-营业员
        /// </summary>
        public int UserType
        {
            get;
            set;
        }

        /// <summary>
        /// 营业厅Id
        /// </summary>
        public int HallId
        {
            get;
            set;
        }
    }
}
