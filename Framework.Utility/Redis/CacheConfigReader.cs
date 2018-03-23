using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;

namespace Framework.Utility.Redis
{
    public static class CacheConfigReader
    {
        private static Dictionary<string, ECacheConfig> _list;
        private static string _configPath = ConfigurationManager.AppSettings["CacheConfigPath"];
        private static object _lockObj = new object();


        public static ECacheConfig GetConfig(string cacheName)
        {
            if (string.IsNullOrEmpty(cacheName))
                throw new Exception("读取缓存配置信息时,缓存名称不存在");
            lock (_lockObj)
            {
                if (_list == null)
                {
                    _list = GetConfig();
                }
            }
            if (_list.Keys.Contains(cacheName))
            {
                return _list[cacheName];
            }
            else
            {
                throw new Exception("读取缓存配置信息时,不存在的配置名(cacheName)");
            }
        }


        private static Dictionary<string, ECacheConfig> GetConfig()
        {
            Dictionary<string, ECacheConfig> list = new Dictionary<string, ECacheConfig>();
            XmlDocument xmlDoc = new XmlDocument();
            string path = System.Web.HttpContext.Current.Server.MapPath(_configPath);
            xmlDoc.Load(path);
            XmlNodeList nodes = xmlDoc.SelectNodes("/Items/Item");
            ECacheConfig model = null;
            string timeOut;
            string autoDelay;
            foreach (XmlNode node in nodes)
            {
                model = new ECacheConfig();
                timeOut = node.Attributes["TimeOut"].Value;
                autoDelay = node.Attributes["AutoDely"].Value;
                if (string.IsNullOrEmpty(timeOut) || timeOut.ToLower() == "null")
                {
                    model.TimeOut = null;
                }
                else
                {
                    model.TimeOut = int.Parse(timeOut);
                }
                model.CacheName = node.Attributes["CacheName"].Value;
                if (autoDelay.ToLower() == "true")
                {
                    model.AutoDely = true;
                }
                else
                {
                    model.AutoDely = false;
                }

                if (string.IsNullOrEmpty(model.CacheName))
                {
                    throw new Exception("缓存配置信息出错,缓存配置名(CacheName)不允许为空");
                }
                if (list.Keys.Contains(model.CacheName))
                {
                    throw new Exception(string.Format("缓存配置信息出现重复项,path:{0},Item:{1}", _configPath, model.CacheName));
                }
                list.Add(model.CacheName, model);
            }
            return list;
        }
    }
}
