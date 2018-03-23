using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Utility.Attributes;

namespace Framework.Entity
{
    /// <summary>
    /// 业务活动实体
    /// </summary>
    public class Business_Promotion
    {
        /// <summary>
        /// 业务活动的ID
        /// </summary>
        [SEQ("SEQ_BUSINESSPROMOTION_AUTO_ID")]
        public int Promotion_Id
        {
            get;
            set;
        }

        /// <summary>
        /// 创建推广人员编号
        /// </summary>
        public int User_Id
        {
            get;
            set;
        }

        /// <summary>
        /// 所属营业厅
        /// </summary>
        public int Hall_Id
        {
            get;
            set;
        }

        /// <summary>
        /// 模板ID
        /// </summary>
        public int Template_Id
        {
            get;
            set;
        }

        /// <summary>
        /// 业务类型 1-营业员自建的2-平台的创建 3-营业员自建上传图片
        /// </summary>
        public int Promotion_Type
        {
            get;
            set;
        }

        /// <summary>
        /// 活动地址
        /// </summary>
        public string Img_Url
        {
            get;
            set;
        }

        /// <summary>
        /// 推广的标题
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// 推广的内容
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// 活动点击次数
        /// </summary>
        public int Click_Count
        {
            get;
            set;
        }

        /// <summary>
        /// 推广的开始时间
        /// </summary>
        public DateTime Start_Time
        {
            get;
            set;
        }

        /// <summary>
        /// 推广的结束时间
        /// </summary>
        public DateTime End_Time
        {
            get;
            set;
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Create_User
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Create_Time
        {
            get;
            set;
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Update_User
        {
            get;
            set;
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime Update_Time
        {
            get;
            set;
        }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort
        {
            get;
            set;
        }

        /// <summary>
        ///上线状态（0：上线，1：下线，）
        /// </summary>
        public int Status
        {
            get;
            set;
        }

        /// <summary>
        /// 审核状态（0：未审核，1：审核通过，2：审核未通过）
        /// </summary>
        public int Audit_State
        {
            get;
            set;
        }

        /// <summary>
        /// 推广编号
        /// </summary>
        public string Promotion_No
        {
            get;
            set;
        }
    }
}
