using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.DBAccess;

namespace Framework.DBAccess
{
    /// <summary>
    /// Db的写操作接口
    /// </summary>
    public interface IWriting
    {

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        int Insert<T>(T model) where T : class,new();

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        int Update<T>(T model) where T: class,new();

        /// <summary>
        /// 执行操作语句
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        int ExcuteSql(string sqlCommand, object Parameters=null);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sProcedureName"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        int ExcuteProcedure(string sProcedureName, OracleDbParameters Parameters=null);
    }
}
