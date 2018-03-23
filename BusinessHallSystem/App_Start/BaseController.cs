using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lib4Net.Logs;
using System.IO;
using System.Text;
using Framework.Utility.Model;
using Framework.Utility.Tools;
using Framework.Utility.Attributes;
using Framework.Utility.Redis;

namespace Web.App_Start
{
    public class BaseController:Controller
    {
        public static ILogger logger = LoggerManager.Instance.GetLogger("Web");

        /// <summary>
        /// 请求的返回结果集
        /// </summary>
        protected ResponeResult result = null;

        /// <summary>
        /// 用户的登录状态
        /// </summary>
        protected UserInfo LoginStatus = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseController()
        {
            result = new ResponeResult();
            //实例化用户状态
            LoginStatus = new UserInfo();
        }

        /// <summary>
        /// action之前验证登录
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.HttpMethod.ToUpper() == "GET")
            {
                result.login = false;
                result.info = "Get requests are not allowed";
                filterContext.Result = Content(JsonHelper.ToJsonString(result));
            }
            else
            {
                logger.Info("请求的接口地址:" + filterContext.HttpContext.Request.Url.AbsoluteUri);
                logger.Info("请求的接口参数");
                foreach (var key in filterContext.HttpContext.Request.Form.AllKeys)
                {
                    logger.Info(key+":"+filterContext.HttpContext.Request.Form[key]);
                }
                //是否需要验证
                bool needVerify = filterContext.ActionDescriptor.GetCustomAttributes(typeof(NoVerifyAttribute), true).Length == 1 ? false : true;
                if (needVerify)
                {
                        string sessionId =filterContext.HttpContext.Request["sessionId"];
                        if (sessionId!=null)
                        {
                            if (sessionId != string.Empty)
                            {
                                string CacheString = RedisHelper.GetCharValue(CacheType.UserInfo, sessionId);
                                if (!string.IsNullOrEmpty(CacheString))
                                {
                                    LoginStatus = JsonHelper.Deserialize<UserInfo>(CacheString);
                                    //重新刷新Key的缓存时间
                                    RedisHelper.SetKeyTimeout(CacheType.UserInfo, sessionId, true);
                                }
                                else
                                {
                                    result.success = false;
                                    result.info = ErrorType.LOGIN_OVER;
                                    result.login = false;
                                    filterContext.Result = Json(result);
                                }
                            }
                            else
                            {
                                result.success = false;
                                result.info = ErrorType.LOGIN_OVER;
                                result.login = false;
                                filterContext.Result = Json(result);
                            }
                        }
                        else
                        {
                            result.success = false;
                            result.info = ErrorType.MISS_PARAMETER;
                            filterContext.Result = Json(result);
                        }
                }
            }
        }

        /// <summary>
        /// action执行之后
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string res = JsonHelper.ToJsonString(result);
            logger.Info("返回的结果:" + res);
            filterContext.Result = Content(res);
        }

        /// <summary>
        /// 扑捉全局异常
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            //表示对异常已经处理过,不会对客户端在抛出异常
            filterContext.ExceptionHandled = true;
            //异常的打印
            logger.Info("Controller的异常信息:"+filterContext.Exception.Message);
            logger.Fatal("Controller的异常信息:"+filterContext.Exception.Message, filterContext.Exception);
            result.info = "Server Error";
            result.success = false;
            filterContext.Result = Content(JsonHelper.ToJsonString(result));
        }
    }
}