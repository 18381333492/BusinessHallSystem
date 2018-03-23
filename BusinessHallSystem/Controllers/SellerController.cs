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
    /// 商家端
    /// </summary>
    public class SellerController :BaseController
    {
        //
        // GET: /Seller/

        /// <summary>
        /// 获取商家用户信息
        /// </summary>
        public void GetSellerInfo()
        {
            result.success = true;
            result.data = Nbh_UsersServices.Instance.GetSellerInfo(LoginStatus.Waiter_Id);
        }

        /// <summary>
        /// 分页获取正在推广的活动列表
        /// </summary>
        /// <param name="pageInfo"></param>
        public void GetExteningPromotionList(PageInfo pageInfo)
        {
            result.success = true;
            result.data = PromotionServices.Instance.GetExteningPromotionList(pageInfo,LoginStatus.Waiter_Id);
        }

        /// <summary>
        /// 分页获取推广历史记录
        /// </summary>
        /// <param name="pageInfo"></param>
        public void GetHistoryPromotionList(PageInfo pageInfo)
        {
            result.success = true;
            result.data = PromotionServices.Instance.GetHistoryPromotionList(pageInfo, LoginStatus.Waiter_Id);
        }

        /// <summary>
        /// 分页获取所有的模板数据列表
        /// </summary>
        /// <param name="pageInfo"></param>
        public void GetPromotionTemplateList(PageInfo pageInfo)
        {
            result.success = true;
            result.data = PromotionServices.Instance.GetPromotionTemplateList(pageInfo, LoginStatus.Waiter_Id);
        }

        /// <summary>
        /// 创建推广
        /// </summary>
        /// <param name="Promotion"></param>
        public void CreatePromotion(Business_Promotion Promotion, double ?StartStamp, double? EndStamp)
        {
            if (string.IsNullOrEmpty(Promotion.Title) ||StartStamp == null || EndStamp == null || Promotion.Promotion_Type==0)
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            if (Promotion.Promotion_Type == 1 && Promotion.Template_Id == 0)
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            if (Promotion.Promotion_Type == 3 && string.IsNullOrEmpty(Promotion.Img_Url))
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            if (!string.IsNullOrEmpty(Promotion.Content)&&Promotion.Content.Length > 2000)
            {
                result.info = "内容超出2000限制";
                return;
            }
            var status = Nbh_UsersServices.Instance.GetSellerStatus(LoginStatus.Waiter_Id);
            if (status == 1)
            {//判断用户状态
                result.info = "营业员已被禁用";
                result.data = new { status = status };
                return;
            }
            DateTime Time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            Promotion.Start_Time=Time.AddMilliseconds(StartStamp.Value);
            Promotion.End_Time = Time.AddMilliseconds(EndStamp.Value);
            if (PromotionServices.Instance.CreatePromotion(Promotion, LoginStatus.Waiter_Id, LoginStatus.HallId) > 0)
            {
                result.success = true;
                result.info = "创建成功";
            }
            else
            {
                result.info = "创建失败";
            }
        }

        /// <summary>
        /// 编辑活动
        /// </summary>
        /// <param name="Promotion"></param>
        /// <param name="StartStamp"></param>
        /// <param name="EndStamp"></param>
        public void UpdatePromotion(Business_Promotion Promotion, double? StartStamp, double? EndStamp)
        {
            if (string.IsNullOrEmpty(Promotion.Title) || StartStamp == null || EndStamp == null || Promotion.Promotion_Type == 0)
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            if (Promotion.Promotion_Type == 3 && string.IsNullOrEmpty(Promotion.Img_Url))
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            if (!string.IsNullOrEmpty(Promotion.Content) && Promotion.Content.Length > 2000)
            {
                result.info = "内容超出2000限制";
                return;
            }
            var status = Nbh_UsersServices.Instance.GetSellerStatus(LoginStatus.Waiter_Id);
            if (status == 1)
            {//判断用户状态
                result.info = "营业员已被禁用";
                result.data = new { status = status };
                return;
            }
            DateTime Time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            Promotion.Start_Time = Time.AddMilliseconds(StartStamp.Value);
            Promotion.End_Time = Time.AddMilliseconds(EndStamp.Value);
            if (PromotionServices.Instance.UpdatePromotion(Promotion) > 0)
            {
                result.success = true;
                result.info = "编辑成功";
            }
            else
            {
                result.info = "编辑失败";
            }
        }

        /// <summary>
        /// 认证营业员
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mobile"></param>
        public void SellerConfrim(string code, string mobile)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(mobile))
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            //根据手机号获取服务员
            dynamic waiter = Nbh_UsersServices.Instance.GetSellerByMobile(mobile);
            if (waiter != null)
            {
                string openid = waiter.openid;
                if (string.IsNullOrEmpty(openid))
                {//没有关联过
                    //获取opienid
                    string data = WeChatHelper.GetSession_key(code);
                    var obj = JsonHelper.Deserialize<dynamic>(data);
                    int UserId=Convert.ToInt32(waiter.waiterid);
                    openid = obj.openid;
                    if (!Nbh_UsersServices.Instance.isExitOpenid(openid))
                    {
                        //更新数据
                        if (Nbh_UsersServices.Instance.BingSellerOpenId(UserId, openid) > 0)
                        {
                            result.success = true;
                            result.info = "认证成功";
                        }
                        else
                        {
                            result.info = "认证失败";
                        }
                    }
                    else
                    {
                        result.info = "该微信用户已经认证过";
                    }
                }
                else{
                  result.info="该手机号已经认证过";
                }
            }
            else
            {
                result.info="查询不到手机号对应的信息";
            }

        }
    }
}
