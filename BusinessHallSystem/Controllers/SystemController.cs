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
using System.IO;
using System.Configuration;
using Lib4Net.Logs;


namespace Web.Controllers
{
    /// <summary>
    /// 系统相关的接口
    /// </summary>
    public class SystemController : BaseController
    {
        //
        // GET: /System/

        /// <summary>
        /// 记录小程序错误日志
        /// </summary>
        /// <param name="error"></param>
        [ValidateInput(false)]
        public void WriteLog(string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            if (SystemServices.Instance.WriteLog(error) > 0)
            {
                result.success = true;
                result.info = "操作成功";
            }
            else
                result.info = "操作失败";
        }

        /// <summary>
        /// 获取平台关联信息
        /// </summary>
        public void GetPlatformContactInfo()
        {
            result.success = true;
            result.data = SystemServices.Instance.GetPlatformContactInfo();
        }


        /// <summary>
        /// 营业员ID
        /// </summary>
        /// <param name="WaiterId"></param>
        public void SetKfSessionKey(int? WaiterId)
        {
            if (WaiterId == null)
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            else
            {
                string sWeiXinNo=Nbh_UsersServices.Instance.GetWeiXinNo(WaiterId.Value);
                logger.Info("sWeiXinNo:" + sWeiXinNo);
                if (!string.IsNullOrEmpty(sWeiXinNo))
                {
                    string sKey=LoginStatus.OpenId;
                    RedisHelper.SetCharValue(CacheType.Session, sKey, sWeiXinNo);
                    result.success = true;
                }
                else
                {
                    result.info ="获取微信号失败";
                }
            }
        }
    
    }
}
