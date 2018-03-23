using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.App_Start;
using Framework.Entity;
using Framework.Services;
using Framework.Utility.Redis;
using Framework.Utility.Attributes;
using Web.Tools;
using Framework.Utility.Tools;
using Framework.Utility.Model;

namespace Web.Controllers
{
    /// <summary>
    /// 活动业务相关
    /// </summary>
    public class PromotionController :BaseController
    {
        //
        // GET: /Promotion/

        /// <summary>
        /// 获取平台活动
        /// </summary>
        /// <returns></returns>
        public void GetPlatformPromotion()
        {
            result.success = true;
            result.data= PromotionServices.Instance.GetPlatformPromotion();
        }


        /// <summary>
        /// 分页获取营业厅的活动列表
        /// </summary>
        /// <param name="hallid"></param>
        /// <returns></returns>
        public void GetHallPromotionListByPage(int? hallid, PageInfo pageInfo)
        {
            if (hallid == null)
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            result.success = true;
            result.data = PromotionServices.Instance.GetHallPromotionListByPage(hallid.Value, pageInfo);
        }

        /// <summary>
        /// 分页获取营业员的活动列表
        /// </summary>
        /// <param name="waiterid"></param>
        /// <param name="pageInfo"></param>
        public void GetWaiterPromotionListByPage(int? waiterid, PageInfo pageInfo)
        {
            if (waiterid == null)
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            result.success = true;
            result.data = PromotionServices.Instance.GetWaiterPromotionListByPage(waiterid.Value, pageInfo);
        }

        /// <summary>
        /// 获取业务活动详情
        /// </summary>
        /// <param name="promotionid"></param>
        public void GetPromotionDetail(int? promotionid, int? WaiterId, int? HallId)
        {
            if (promotionid == null)
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            var obj = PromotionServices.Instance.GetPromotionDetail(promotionid.Value, LoginStatus.User_Id, WaiterId, HallId);
            if (obj == null)
            {
                result.info = "活动不存在";
            }
            else
            {
                obj.Add("usertype", LoginStatus.UserType);// 0-普通用户,1-营业员
                result.success = true;
                result.data = obj;
            }
        }
    }
}
