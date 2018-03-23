using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Framework.Utility.Tools;
using Framework.Utility.Model;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Web.Tools
{
    /// <summary>
    /// 微信请求的相关业务
    /// </summary>
    public class WeChatHelper
    {
        /// <summary>
        /// 小程序的appid
        /// </summary>
        public static readonly string sAppId = ConfigurationManager.AppSettings["sAppId"];

        /// <summary>
        /// 小程序的secret
        /// </summary>
        public static readonly string sSecret = ConfigurationManager.AppSettings["sSecret"];

        /// <summary>
        /// 缓存微信接口调用凭证
        /// </summary>
        public static string access_token = string.Empty;

        /// <summary>
        /// 凭证过期时间
        /// </summary>
        public static DateTime OverTime = DateTime.Now;


        /// <summary>
        /// 根据code获取Session_key
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetSession_key(string code)
        {
            string sUrl = string.Format("https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code", sAppId, sSecret, code);
            string result=HttpHelper.HttpGet(sUrl);
            return result;
        }

        /// <summary>
        /// 获取微信接口调用凭证
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            if (OverTime > DateTime.Now)
            {
                return access_token;
            }
            else
            {//过期重新获取
                string sUrl = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", sAppId, sSecret);
                string result=HttpHelper.HttpGet(sUrl);
                var obj=JObject.Parse(result);
                if (obj["access_token"] != null)
                {
                    access_token =Convert.ToString(obj["access_token"]);
                    OverTime = DateTime.Now.AddSeconds(Convert.ToInt32(obj["expires_in"]) - 200);//重新设置过期时间
                    return access_token;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 根据微信好获取客户ID
        /// </summary>
        /// <returns></returns>
        public static string GetKfAccount(string sWeiXinNo, string access_token)
        {
            string sUrl = string.Format("https://api.weixin.qq.com/cgi-bin/customservice/getkflist?access_token={0}", access_token);
            string result = HttpHelper.HttpGet(sUrl);
            JObject job = JObject.Parse(result);
            if (job["kf_list"] != null)
            {
                JArray KfArray = JArray.Parse(Convert.ToString(job["kf_list"]));
                JToken kfItem = KfArray.Where(m => Convert.ToString(m["kf_wx"]) == sWeiXinNo).FirstOrDefault();
                if (kfItem != null)
                {
                    return Convert.ToString(kfItem["kf_account"]);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
                return string.Empty;
        }


        /// <summary>
        ///  发送模板消息
        /// </summary>
        /// <param name="content">发送的内容</param>
        /// <param name="access_token">接口调用凭证</param>
        /// <returns></returns>
        public static bool SendMessage(string content, string access_token)
        {
            var ret = false;
            string sUrl = string.Format("https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token={0}", access_token);
            var result = HttpHelper.HttpPost(sUrl, content);
            if (!string.IsNullOrEmpty(result))
            {
                var ParamData = JObject.Parse(result);
                if (ParamData["errcode"].ToString() == "0")
                {//发送成功
                    ret = true;
                }
            }
            return ret;
        } 

        /// <summary>
        /// 解密获取用户加密信息
        /// </summary>
        /// <param name="encryptedData">加密数据</param>
        /// <param name="iv">初始向量</param>
        /// <param name="sessionKey">SessionKey</param>
        /// <returns></returns>
        public static WxUserInfo Decrypt(string encryptedData, string iv, string sessionKey)
        {
            WxUserInfo userInfo;
            //创建解密器生成工具实例
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            //设置解密器参数
            aes.Mode = CipherMode.CBC;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            //格式化待处理字符串
            byte[] byte_encryptedData = Convert.FromBase64String(encryptedData);
            byte[] byte_iv = Convert.FromBase64String(iv);
            byte[] byte_sessionKey = Convert.FromBase64String(sessionKey);
            aes.IV = byte_iv;
            aes.Key = byte_sessionKey;
            ICryptoTransform transform = aes.CreateDecryptor();
            //解密
            byte[] final = transform.TransformFinalBlock(byte_encryptedData, 0, byte_encryptedData.Length);

            string result = Encoding.UTF8.GetString(final);
            userInfo = JsonHelper.Deserialize<WxUserInfo>(result);
            return userInfo;
        }


    }
}