using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Text;
using System.IO;
using Web.Tools;
using Framework.Utility.Tools;
using Framework.Utility.Redis;
using Framework.Services;
using Newtonsoft.Json.Linq;
using Lib4Net.Logs;

namespace Web.Controllers
{
    public class WeiXinController : Controller
    {
        private static readonly string token = ConfigurationManager.AppSettings["sToken"];//获取成为开发者token
        private static readonly string sAppId = ConfigurationManager.AppSettings["sAppId"];
        private static readonly string sAppsSecret = ConfigurationManager.AppSettings["sSecret"];
        private static readonly string sEncodingAESKey = ConfigurationManager.AppSettings["sEncodingAESKey"];
        private static readonly string template_id = ConfigurationManager.AppSettings["template_id"];//模板Id
        private static ILogger logger = LoggerManager.Instance.GetLogger("WeChat");
        /// <summary>
        /// 微信验证成为开发者
        /// </summary>
        public void Check()
        {
            try
            {
                if (Request.HttpMethod.ToUpper() == "GET")
                {
                    string result = string.Empty;
                    string signature = Request["signature"];//微信加密签名
                    string timestamp = Request["timestamp"];//时间戳
                    string nonce = Request["nonce"];        //随机数
                    string echostr = Request["echostr"];    //随机字符串

                    // 开发者通过检验signature对请求进行校验（下面有校验方式）。若确认此次GET请求来自微信服务器，请原样返回echostr参数内容，则接入生效，成为开发者成    功，否则接入失败。加密 / 校验流程如下：
                    //1）将token、timestamp、nonce三个参数进行字典序排序
                    //2）将三个参数字符串拼接成一个字符串进行sha1加密
                    //3）开发者获得加密后的字符串可与signature对比，标识该请求来源于微信

                    string[] array = { token, timestamp, nonce };
                    Array.Sort(array);//字典排序
                    string newSignature = SHA1(string.Join("", array));
                    if (newSignature == signature.ToUpper())
                        result = echostr;
                    else
                        result = "Valiate Fail";
                    logger.Info("验证成为开发者:" + result);
                    Response.Write(result);
                }
                else
                {//接收微信加密消息
                    string msg_signature = Request["msg_signature"];//签名
                    string timestamp = Request["timestamp"];
                    string nonce = Request["nonce"];
                    StreamReader sr = new StreamReader(Request.InputStream, Encoding.UTF8);
                    string requestXmlMessage = sr.ReadToEnd();
                    logger.Info("接收微信消息:" + requestXmlMessage);


                    WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(token, sEncodingAESKey, sAppId);
                    string sMsg = string.Empty;
                    wxcpt.DecryptMsg(msg_signature, timestamp, nonce, requestXmlMessage, ref sMsg);
                    logger.Info("解密后的消息:" + sMsg);
                    string sMsgType = XmlHelper.getTextByNode(sMsg, "MsgType");
                    if (sMsgType.ToUpper() != "EVENT")
                    {
                        string sOpenId = XmlHelper.getTextByNode(sMsg, "FromUserName");//获取openId
                        List<string> WaiterInfo = RedisHelper.GetCharValue(CacheType.Session, sOpenId).Split(',').ToList();
                        if (!string.IsNullOrEmpty(WaiterInfo[0]))
                        {
                            //获取客户账号
                            string OriginId = XmlHelper.getTextByNode(sMsg, "ToUserName");//获取公众号原始ID
                            long CreateTime = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
                            string access_token = WeChatHelper.GetAccessToken();
                            string KfAccount = WeChatHelper.GetKfAccount(WaiterInfo[0], access_token);
                            string responeData = string.Format(@"<xml>
                                                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                                                    <CreateTime>{2}</CreateTime>
                                                                    <MsgType><![CDATA[transfer_customer_service]]></MsgType>
                                                                    <TransInfo>
                                                                    <KfAccount><![CDATA[{3}]]></KfAccount>
                                                                    </TransInfo>
                                                                </xml>", sOpenId, OriginId, CreateTime, KfAccount);
                            logger.Info("返回的消息:" + responeData);
                            //发送模板消息
                            string openId = WaiterInfo[1];//营业员的OpenId
                            string Content = XmlHelper.getTextByNode(sMsg, "Content");
                            SendMessage(openId, Content);

                            string sEncryptMsg = ""; //xml格式的密文
                            wxcpt.EncryptMsg(responeData, timestamp, nonce, ref sEncryptMsg);//加密
                            logger.Info("返回的加密消息:" + sEncryptMsg);
                            Response.Write(sEncryptMsg);
                        }
                    }
                    else
                    {
                        Response.Write(string.Empty);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Info("WeiXinController:"+e.Message);
                logger.Info(e.Message,e);
                Response.Write("System Error!");
            } 
        }

        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="sOpenId"></param>
        /// <param name="sTemplateId"></param>
        /// <returns></returns>
        private void SendMessage(string sOpenId, string Content)
        {
            JObject job = new JObject();
            job.Add(new JProperty("touser", sOpenId));
            job.Add(new JProperty("template_id", template_id));
            //job.Add(new JProperty("form_id", form_id));
            JObject childData = new JObject();
            childData.Add(new JProperty("keyword1", new JObject(new JProperty("value", Content))));
            childData.Add(new JProperty("keyword2", new JObject(new JProperty("value", "请尽快联系用户!"))));
            job.Add("data", childData);
            string access_token = WeChatHelper.GetAccessToken();
            WeChatHelper.SendMessage(job.ToString(), access_token);
        }
        

        /// <summary>
        /// sha加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private  string SHA1(string input)
        {
            System.Security.Cryptography.SHA1 shaHash = System.Security.Cryptography.SHA1.Create();
            byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString().ToUpper();
        }

    }
}
