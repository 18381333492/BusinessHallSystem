using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Lib4Net.Data;
using Lib4Net.Logs;

namespace Framework.Utility.Redis
{
    /// <summary>
    /// 对redis操作的封装
    /// </summary>
    public class RedisHelper
    {
        private static string _prix = "US";
        private static string strConn = ConfigurationManager.AppSettings["redisConn"];
        private static redis.RedisCommand command = new redis.RedisCommand(strConn, 3000);

        /// <summary>
        /// 设置key的超时时间
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <param name="isCreate"></param>
        public static void SetKeyTimeout(CacheType cacheType, string key, bool isCreate)
        {
            string typeName = Enum.GetName(typeof(CacheType), cacheType);
            ECacheConfig model = CacheConfigReader.GetConfig(typeName);
            if (model.TimeOut.HasValue && model.TimeOut.Value > 0 && (isCreate == true || model.AutoDely == true))
            {
                List<string> lstOutput = new List<string>();
                command.Exec(new List<string> { "EXPIRE", key, model.TimeOut.Value.ToString() }, lstOutput);
            }
        }

        /// <summary>
        /// 移除缓存Key及内容
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        public static void RemoveKey(CacheType cacheType, string key)
        {
            key = GetIdentifier(cacheType, key);
            List<string> lstOutput = new List<string>();
            string location = command.Exec(new List<string> { "DEL", key }, lstOutput);
        }

        /// <summary>
        /// 判断缓存Key是否存在
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ExistsKey(CacheType cacheType, string key)
        {
            key = GetIdentifier(cacheType, key);
            List<string> lstOutput = new List<string>();
            string location = command.Exec(new List<string> { "EXISTS", key }, lstOutput);
            if (lstOutput.Count > 0 && Int32.Parse(lstOutput[0]) > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置字符串类型
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetCharValue(CacheType cacheType, string key, string value)
        {
            key = GetIdentifier(cacheType, key);
            List<string> lstOutput = new List<string>();
            string location = command.Exec(new List<string> { "SET", key, value }, lstOutput);
            SetKeyTimeout(cacheType, key, true);
        }

        /// <summary>
        /// 获取字符串类型
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetCharValue(CacheType cacheType, string key)
        {
            key = GetIdentifier(cacheType, key);
            List<string> lstOutput = new List<string>();
            string location = command.Exec(new List<string> { "GET", key }, lstOutput);
            if (lstOutput.Count > 0)
            {
                SetKeyTimeout(cacheType, key, false);
                return lstOutput[0];
            }
            return null;
        }

        /// <summary>
        /// 设置Hash对象(Class)类型
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expire"></param>
        public static void SetHashValue<T>(CacheType cacheType, string key, T data)
        {
            string jsonData = JsonData.JavaScriptSerialize(data);
            SetCharValue(cacheType, key, jsonData);
        }

        /// <summary>
        /// 获取Hash对象(class)类型
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetHashValue<T>(CacheType cacheType, string key) where T : class
        {
            string jsonVal = GetCharValue(cacheType, key);
            if (!string.IsNullOrEmpty(jsonVal))
            {
                T obj = JsonData.JavaScriptDeserialize<T>(jsonVal);
                return obj;
            }
            return null;
        }

        /// <summary>
        /// 设置字符串集合类型队列
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expire"></param>
        public static void SetStringCollect(CacheType cacheType, string key, List<string> data, int expire = 0)
        {
            key = GetIdentifier(cacheType, key);
            List<string> lstOutput = new List<string>();
            //插入key
            data.Insert(0, key);
            //插入命令
            data.Insert(0, "SADD");
            string location = command.Exec(data, lstOutput);
            SetKeyTimeout(cacheType, key, true);
        }

        /// <summary>
        /// 获取字符串集合类型队列
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<string> GetStringCollect(CacheType cacheType, string key)
        {
            key = GetIdentifier(cacheType, key);
            List<string> lstOutput = new List<string>();
            string location = command.Exec(new List<string> { "SMEMBERS", key }, lstOutput);
            SetKeyTimeout(cacheType, key, false);
            return lstOutput;
        }

        /// <summary>
        /// 判断字符串集合列表队列是否存在该值
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool StringCollectExists(CacheType cacheType, string key, string val)
        {
            key = GetIdentifier(cacheType, key);
            List<string> lstOutput = new List<string>();
            string location = command.Exec(new List<string> { "SISMEMBER", key, val }, lstOutput);
            if (lstOutput.Count > 0 && Int32.Parse(lstOutput[0]) > 0)
            {
                return true;
            }
            SetKeyTimeout(cacheType, key, false);
            return false;
        }

        /// <summary>
        /// 设置object格式集合类型队列
        /// </summary>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expire"></param>
        public static void SetObjectCollect<T>(CacheType cacheType, string key, List<T> data)
        {
            key = GetIdentifier(cacheType, key);
            List<string> execData = new List<string>();
            //1.转换object为json字符串
            foreach (var item in data)
            {
                string strJson = JsonData.JavaScriptSerialize(item);
                execData.Add(strJson);
            }
            //插入key
            execData.Insert(0, key);
            //插入命令
            execData.Insert(0, "SADD");
            List<string> lstOutput = new List<string>();

            string location = command.Exec(execData, lstOutput);
            SetKeyTimeout(cacheType, key, true);
        }

        /// <summary>
        /// 获取object格式集合类型队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> GetObjectCollect<T>(CacheType cacheType, string key)
        {
            key = GetIdentifier(cacheType, key);
            List<string> lstOutput = new List<string>();
            List<T> retList = new List<T>();
            string location = command.Exec(new List<string> { "SMEMBERS", key }, lstOutput);
            foreach (var item in lstOutput)
            {
                T objVal = JsonData.JavaScriptDeserialize<T>(item);
                retList.Add(objVal);
            }
            SetKeyTimeout(cacheType, key, false);
            return retList;
        }

        /// <summary>
        /// 判断Object集合列表队列是否存在该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheType"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool ObjectCollectExists<T>(CacheType cacheType, string key, T val)
        {
            key = GetIdentifier(cacheType, key);
            List<string> lstOutput = new List<string>();
            string strJson = JsonData.JavaScriptSerialize(val);
            string location = command.Exec(new List<string> { "SISMEMBER", key, strJson }, lstOutput);
            if (lstOutput.Count > 0 && Int32.Parse(lstOutput[0]) > 0)
            {
                return true;
            }
            SetKeyTimeout(cacheType, key, false);
            return false;
        }

        /// <summary>
        /// 设置队列数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetQuenData(string key, string value, ILogger logger)
        {
            List<string> lstOutput = new List<string>();
            string location = command.Exec(new List<string> { "lpush", key, value }, lstOutput);
            logger.Info("redis返回结果:" + lstOutput[0]);
            logger.Info("location:" + location);
            return true;
        }

        private static string GetIdentifier(CacheType cacheType, string key)
        {
            string typeName = Enum.GetName(typeof(CacheType), cacheType);
            if (string.IsNullOrEmpty(key))
            {
                string msg = string.Format("读取或者设置MemoryCache时,key不允许为空.缓存类型:{0}", typeName);
                throw new Exception(msg);
            }
            return key;//string.Format("{0}:{1}:{2}", _prix, typeName, key);
        }
    }
}
