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
    /// 前端用户的相关接口
    /// </summary>
    public class UserController : BaseController
    {

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="code"></param>
        /// <param name="encryptedData">用户加密数据</param>
        /// <param name="iv">加密算法的初始向量</param>
        [NoVerify]
        public void VerifyLogin(string code, string encryptedData, string iv)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(encryptedData) || string.IsNullOrEmpty(iv))
            {
                result.info =ErrorType.MISS_PARAMETER;
                return ;
            }
            string data = WeChatHelper.GetSession_key(code);
            var obj = JsonHelper.Deserialize<dynamic>(data);
            if (obj.openid != null)
            {//获取成功
                int res = 1;//
                //获取普通用户
                Nbh_Users User = Nbh_UsersServices.Instance.GetNbhUsers(Convert.ToString(obj.openid));
                //获取营业员
                var Waiter = Nbh_UsersServices.Instance.GetUserWaiter(Convert.ToString(obj.openid));
                if (User == null && Waiter==null)
                {//不存在
                    WxUserInfo wxUser = WeChatHelper.Decrypt(encryptedData, iv, Convert.ToString(obj.session_key));
                    User = new Nbh_Users()
                    {
                        OpenId = obj.openid,
                        UnionId = obj.unionid == null ? "0" : obj.unionid,
                        Nick_Name = wxUser.nickName,
                        City=wxUser.city,
                        User_Sex=Convert.ToInt32(wxUser.gender),
                        Province=wxUser.province,
                        Pic_Url=wxUser.avatarUrl,
                        Status=0,
                        Register_Time=DateTime.Now,
                        Last_Login=DateTime.Now,
                        IsPlatform=1
                    };
                    res = Nbh_UsersServices.Instance.Register(User);
                }
                if (res == 1)
                {
                    if (Waiter != null)
                    {
                        LoginStatus.Waiter_Id =Convert.ToInt32(Waiter.WAITER_ID);
                        LoginStatus.UserType = 1;
                        LoginStatus.HallId =Convert.ToInt32(Waiter.HALL_ID);

                    }     
                    LoginStatus.User_Id = User.User_ID;
                    LoginStatus.sessionId = Guid.NewGuid().ToString("N");
                    LoginStatus.OpenId = obj.openid;
                    LoginStatus.SessionKey = obj.session_key;
                    //将信息存入redis
                    RedisHelper.SetCharValue(CacheType.UserInfo, LoginStatus.sessionId, JsonHelper.ToJsonString(LoginStatus));
                    result.success = true;
                    result.data = new { sessionId = LoginStatus.sessionId, UserType = LoginStatus.UserType, IsPlatform = User.IsPlatform };
                }
                else
                {
                    result.info = "用户注册失败";
                }
            }
            else
            {
                result.info = obj.errmsg;
            }
        }

        /// <summary>
        /// 用户关注接口
        /// </summary>
        /// <param name="followId">关注的对象ID</param>
        /// <param name="followtype">关注的类型（0：营业厅，1：营业员）</param>
        /// <returns></returns>
        public void Follow(int? followId, int? followtype)
        {
            if (followId == null || followtype == null)
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            string weixinno = string.Empty;
            if (Nbh_UsersServices.Instance.Follow(followId.Value, followtype.Value, LoginStatus.User_Id, out weixinno) > 0)
            {
                if (followtype.Value == 1)
                    //关注营业员返回微信号
                    result.data = weixinno;
                result.success = true;
                result.info = "关注成功";
            }
            else
            {
                result.info = "关注失败";
            }
        }
   
        /// 获取营业员的基本信息
        /// </summary>
        /// <param name="waiterid"></param>
        /// <returns></returns>
        public void GetWaiterInfo(int? waiterid)
        {
            if (waiterid == null)
            {
                result.info = ErrorType.MISS_PARAMETER;
                return;
            }
            result.success = true;
            result.data = Nbh_UsersServices.Instance.GetWaiterInfo(waiterid.Value, LoginStatus.User_Id);
        }

        /// <summary>
        /// 分页获取已关注营业员列表
        /// </summary>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        public void GetWaiterListByPage(PageInfo pageInfo)
        {
            result.success = true;
            result.data = Nbh_UsersServices.Instance.GetWaiterListByPage(LoginStatus.User_Id, pageInfo);
        }
    }
}
