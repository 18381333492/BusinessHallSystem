using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Dapper;
using Framework.Utility.Model;
using Framework.Utility.Tools;

namespace Framework.DBAccess
{
    public class ReadingManagement : DbBase
    {

        /// <summary>
        /// 获取SQl
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string GetSql(string sql)
        {
            sql=sql.Replace("'","\"");
            sql = sql.Replace(@"\", "'");
            return sql;
        }

        /// <summary>
        /// 判断泛型是否是字典类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private bool DoIsDictionary<T>()
        {
            return typeof(T).GetInterface("IDictionary") == null ? false : true;
        }

        /// <summary>
        /// 根据条件查询是否存在相应的数据
        /// </summary>
        /// <param name="conn">数据库连接字符串对象</param>
        /// <param name="sqlCommand">sql命令</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        protected bool DoAny(IDbConnection conn, string sqlCommand, object parameter)
        {
            sqlCommand = GetSql(sqlCommand);
            var ret = conn.Query(sqlCommand, parameter, null, true, null, CommandType.Text);
            return ret.Count() > 0 ? true : false;
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn">数据库连接字符串对象</param>
        /// <param name="sqlCommand">Sql语句</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        protected T DoSingleQuery<T>(IDbConnection conn, string sqlCommand, Object parameter = null) where T : new()
        {
            sqlCommand = GetSql(sqlCommand);
            if (DoIsDictionary<T>())
            {//泛型是字典类型
                var ret = conn.Query(sqlCommand, parameter, null, true, null, CommandType.Text)
                       .Select(m => ((IDictionary<string, object>)m)
                       .ToDictionary(k => k.Key, v => v.Value))
                       .FirstOrDefault<IDictionary<string, object>>();
                //类型转化
                return (T)ret;
            }
            else
            {
                var ret = conn.QueryFirstOrDefault<T>(sqlCommand, parameter, null, null, CommandType.Text);
                return ret;
            }
        }

        /// <summary>
        /// 查询多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn">数据库连接字符串对象</param>
        /// <param name="sqlCommand">Sql语句</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        protected IList<T> DoQueryList<T>(IDbConnection conn, string sqlCommand, Object parameter = null)
        {
            sqlCommand = GetSql(sqlCommand);
            if (DoIsDictionary<T>())
            {//泛型是字典类型
                var ret = conn.Query(sqlCommand, parameter, null, true, null, CommandType.Text)
                       .Select(m => ((IDictionary<string, object>)m)
                       .ToDictionary(k => k.Key, v => v.Value))
                       .ToList<IDictionary<string, object>>();
                return ret.Select(m => (T)m).ToList();
            }
            else
            {
                var ret = conn.Query<T>(sqlCommand, parameter, null, true, null, CommandType.Text).ToList();
                return ret;
            }
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sqlCommand"></param>
        /// <param name="pageInfo"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected PageResult DoPaginationQuery<T>(IDbConnection conn, string sqlCommand, PageInfo pageInfo, DynamicParameters parameter = null)
        {
            sqlCommand = GetSql(sqlCommand);
            //统计的SQl
            string CountSql = string.Format("SELECT COUNT(*) FROM ({0})", sqlCommand);
            var Total = conn.Query<int>(CountSql.ToString(), parameter, null, true, null, CommandType.Text);

            //声明动态参数
            parameter.Add("PageIndex", pageInfo.PageIndex,DbType.Int32);
            parameter.Add("PageSize", pageInfo.PageSize, DbType.Int32);
            //结果集的SQl
            string sSql = string.Format(@"SELECT * FROM 
                                       (SELECT ROWNUM LINENUM,t.* FROM ({0}) t WHERE ROWNUM<=(:PageIndex*:PageSize)) 
                                       WHERE LINENUM>(:PageIndex-1)*:PageSize", sqlCommand);
            var result = new PageResult();
            if (DoIsDictionary<T>())
            {
                var ret = conn.Query(sSql, parameter, null, true, null, CommandType.Text)
                         .Select(m => ((IDictionary<string, object>)m)
                         .ToDictionary(k => k.Key, v => v.Value))
                         .ToList<Dictionary<string, object>>();
                result.Rows = ret;
            }
            else
            {
                var ret = conn.Query<T>(sSql, parameter, null, true, null, CommandType.Text).ToList();
                result.Rows = ret;
            }
            result.PageIndex = pageInfo.PageIndex;
            result.Total = Total.FirstOrDefault();
            return result;
        }
    }
}
