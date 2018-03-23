using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Dapper;
using DapperExtensions;
using DapperExtensions.Enum;

namespace Framework.DBAccess
{
    public class WritingManagement : DbBase
    {
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected int DoInsert<T>(IDbConnection conn, T model) where T : class,new()
        {
            var res=conn.Insert<T>(model, null, null,DatabaseType.Oracle);
            return Convert.ToInt32(res);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected int DoUpdate<T>(IDbConnection conn, T model) where T : class,new()
        {
            var res = conn.Update<T>(model, null, null, DatabaseType.Oracle);
            return Convert.ToInt32(res);
        }

        /// <summary>
        /// 执行Sql语句
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sqlCommand"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected int DoExcuteSql(IDbConnection conn, string sqlCommand, object parameter)
        {
            int res = conn.Execute(sqlCommand, parameter,null, null, CommandType.Text);
            return res;
        }

        /// <summary>
        /// 执行储存过程
        /// </summary>
        /// <param name="conn">数据库连接字符串对象</param>
        /// <param name="sProcedureName">存储过程名称</param>
        /// <param name="Parameters">参数</param>
        protected int DoExcuteProcedure(IDbConnection conn, string sProcedureName, DynamicParameters Parameters)
        {
            int res = conn.Execute(sProcedureName, Parameters, null, null, CommandType.StoredProcedure);
            return res;
        }
    }
}
