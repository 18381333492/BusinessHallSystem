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
    /// 营业厅的相关的接口
    /// </summary>
    public class BusinessHallController : BaseController
    {
        //
        // GET: /BusinessHall/

        /// <summary>
        /// 分页获取营业厅数据列表
        /// </summary>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="carrierno">营业厅类型</param>
        /// <param name="pageinfo">分页参数</param>
        /// <returns></returns>
        public void GetHallListByPage(decimal? lon, decimal? lat, int? carrierno, PageInfo pageinfo)
        {
            if (lon == null || lat == null || carrierno == null)
            {
                result.success = false;
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            result.success = true;
            result.data=BusinessHallServices.Instance.GetHallByPage(lon.Value, lat.Value, carrierno.Value, pageinfo);
        }

        /// <summary>
        /// 获取营业厅的信息
        /// </summary>
        /// <param name="hallid"></param>
        /// <returns></returns>
        public void GetHallInfo(int? hallid)
        {
            if (hallid==null)
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            result.success = true;
            result.data = BusinessHallServices.Instance.GetHallInfo(hallid.Value,LoginStatus.User_Id);
        }

        /// <summary>
        /// 分页获取关注的营业厅列表
        /// </summary>
        /// <param name="pageinfo"></param>
        public void GetFollowHallListByPage(PageInfo pageinfo)
        {
            result.success = true;
            result.data=BusinessHallServices.Instance.GetHallListByPage(LoginStatus.User_Id,pageinfo);
        }

        /// 获取放大地图营业厅的详情
        /// </summary>
        /// <param name="hallid"></param>
        /// <returns></returns>
        public void GetHallDetail(int? hallid)
        {
            if (hallid == null)
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            result.success = true;
            result.data = BusinessHallServices.Instance.GetHallDetail(hallid.Value);
        }

        /// <summary>
        /// 分页获取搜索数据
        /// </summary>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="carrierno">运营商</param>
        /// <param name="keywords">关键字</param>
        /// <param name="pageinfo">分页参数</param>
        public void GetSearchDataByPage(decimal? lon, decimal? lat, int? carrierno, string keywords, PageInfo pageinfo)
        {
            if (lon == null || lat == null || carrierno == null || string.IsNullOrWhiteSpace(keywords))
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            result.success = true;
            result.data = BusinessHallServices.Instance.GetSearchDataByPage(lon.Value, lat.Value, carrierno.Value, keywords, pageinfo);
        }
    }
}
