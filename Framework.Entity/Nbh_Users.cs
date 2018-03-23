using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Utility.Attributes;

namespace Framework.Entity
{
    /// <summary>
    /// 用户表实体模型
    /// </summary>
    public class Nbh_Users
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SEQ("SEQ_NBHUSERS_AUTO_ID")]
        public int User_ID { get; set; }

        /// <summary>
        /// 微信用户的Openid
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 微信用户的UnionId
        /// </summary>
        public string UnionId { get; set; }

        /// <summary>
        /// 微信号
        /// </summary>
        public string WeiXin_Num { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nick_Name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 用户性别
        /// </summary>
        public int User_Sex { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 微信头像地址
        /// </summary>
        public string Pic_Url { get; set; }

        /// <summary>
        /// 用户详细地址
        /// </summary>
        public string Uer_Adress { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime Last_Login { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime Register_Time { get; set; }

        /// <summary>
        /// 用户状态 0-有效 1-禁用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 推荐营业厅ID
        /// </summary>
        public int Hall_Id { get; set; }

        /// <summary>
        /// 是否是平台指定管理员，0:不是，1:是
        /// </summary>
        public int IsPlatform
        {
            get;
            set;
        }
    }
}
