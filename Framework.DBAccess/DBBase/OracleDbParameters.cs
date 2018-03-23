using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using System.Data;

namespace Framework.DBAccess
{
    public class OracleDbParameters
    {
        private DynamicParameters DbParameters;

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public OracleDbParameters()
        {
            DbParameters = new DynamicParameters();
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <returns></returns>
        public DynamicParameters GetParameters()
        {
            return DbParameters;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        public void Add(string name, object value, DbType? dbType = null, ParameterDirection? direction = null, int? size = null)
        {
            DbParameters.Add(name, value, dbType, direction, size);
        }

        /// <summary>
        /// 获取输出参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public object Get(string name)
        {
            var ret = DbParameters.Get<object>(name);
            return ret;
        }

        /// <summary>
        /// 获取输出参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get<T>(string name)
        {
            var ret = DbParameters.Get<T>(name);
            return ret;
        }
    }
}
