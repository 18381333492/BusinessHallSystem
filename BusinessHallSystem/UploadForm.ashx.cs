using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Lib4Net.Logs;
using Framework.Utility.Tools;
using System.Configuration;
using Web.Tools;

namespace Web
{
    /// <summary>
    /// 表单的形式上传图片
    /// </summary>
    public class UploadForm : IHttpHandler
    {
        private static int _iMaxSize = 1 * 1024;//文件最大不超过1M
        private static ILogger logger = LoggerManager.Instance.GetLogger("Web");
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string sDirectorieName = "Promotion\\";
                HttpFileCollection ImgList = context.Request.Files;
                List<string> UrlList = new List<string>();
                for (int i = 0; i < ImgList.Count; i++)
                {
                    HttpPostedFile image = ImgList[i];
                    if (image != null && image.ContentLength > 0)
                    {
                        string[] sExtension = { ".gif", ".jpg", ".jpeg", ".png", ".bmp" };
                     
                        /*图片保存路径*/
                        string sPath = System.AppDomain.CurrentDomain.BaseDirectory + "Images\\";
                        sPath = sPath + sDirectorieName;
                        if (!Directory.Exists(sPath))
                        {
                            Directory.CreateDirectory(sPath);
                        }
                        string format = System.IO.Path.GetExtension(image.FileName);//后缀名
                        /*组装文件名*/
                        string sFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + format;
                        /*上传的文件后缀名格式错误*/
                        if (!sExtension.Contains(format))
                        {
                            context.Response.Write(JsonHelper.ToJsonString(new
                            {
                                error = 1,
                                message = "上传的图片文件格式错误!"
                            }));
                            return;
                        }
                        /*压缩图片*/
                        if (image.ContentLength > (100 * 1024))
                        {//超过100kb压缩
                            int flag=(int)Math.Ceiling((double)(image.ContentLength)/(double)(1024*100));
                            ImgHelper.GetPicThumbnail(image.InputStream, sPath + sFileName, flag);
                        }
                        else
                        {
                            /*保存图片到本地*/
                            image.SaveAs(sPath + sFileName);
                        }
                        string sUrl = ConfigurationManager.AppSettings["domin"] + "/images/" + sDirectorieName.Replace(@"\", "/") + sFileName;//图片保存路径
                        UrlList.Add(sUrl);
                    }
                }
                if (UrlList.Count > 0)
                {
                    context.Response.Write(JsonHelper.ToJsonString(new
                    {
                        error = 0,
                        url = UrlList
                    }));
                }
                else
                {
                    context.Response.Write(JsonHelper.ToJsonString(new
                    {
                        error = 1,
                        message = "参数错误"
                    }));
                }
            }
            catch (Exception e)
            {
                logger.Info("客户端上传图片异常:"+e.Message);
                logger.Fatal(e.Message, e);
                context.Response.Write(JsonHelper.ToJsonString(new
                {
                    error = 1,
                    message = "上传失败!"
                }));
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}