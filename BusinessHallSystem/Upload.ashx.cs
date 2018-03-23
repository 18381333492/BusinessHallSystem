using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Framework.Utility.Tools;
using System.IO;
using Lib4Net.Logs;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using Web.Tools;

namespace Web
{
    /// <summary>
    /// Upload 的摘要说明
    /// </summary>
    public class Upload : IHttpHandler
    {
        private static int _iMaxSize = 2 * 1024;//文件最大不超过2M
        private static ILogger logger = LoggerManager.Instance.GetLogger("Web");
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            try
            {
                string format = context.Request["format"];//文件后缀名
                int sDirectorieType =string.IsNullOrEmpty(context.Request["sDirectorieType"]) ? -1 : Convert.ToInt32(context.Request["sDirectorieType"]);
                string KeyId = context.Request["KeyId"];
                //文件夹目录
                string sDirectorieName = "other\\";
                switch (sDirectorieType)
                {
                    case 0: sDirectorieName = "Promotion\\"; break;
                    case 1:
                        sDirectorieName = string.Format("Hall\\Hall_{0}\\", KeyId);
                        break;
                    case 2: sDirectorieName = string.Format("Hall\\Hall_{0}\\Waiter\\", KeyId);
                        break;
                }

                if (context.Request.InputStream.Length > 0)
                {
                    /*图片保存路径*/
                    string sPath = System.AppDomain.CurrentDomain.BaseDirectory + "images\\";
                    sPath = sPath + sDirectorieName;
                    if (!Directory.Exists(sPath))
                    {
                        Directory.CreateDirectory(sPath);
                    }
                    string sFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + format;
                    long length = context.Request.InputStream.Length;
                    if (length > 100 * 1024)
                    {
                        //超过100kb压缩
                        int flag = (int)Math.Ceiling((double)length / (double)(1024 * 100));
                        ImgHelper.GetPicThumbnail(context.Request.InputStream, sPath + sFileName, flag);
                    }
                    else
                    {
                        Image img = Bitmap.FromStream(context.Request.InputStream);
                        img.Save(sPath + sFileName);
                    }
                    string srcPicture = ConfigurationManager.AppSettings["domin"] + "/images/" + sDirectorieName.Replace(@"\", "/") + sFileName;
                    context.Response.Write(srcPicture);
                }
                else
                {
                    context.Response.Write("parameters error");
                }
            }
            catch (Exception e)
            {
                logger.Info(e.Message);
                logger.Fatal(e.Message, e);
                context.Response.Write("server error");
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