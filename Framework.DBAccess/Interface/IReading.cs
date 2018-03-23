using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Utility.Model;

namespace Framework.DBAccess
{
    /// <summary>
    /// 数据库查询操作接口
    /// </summary>
    public interface IReading
    {
        /// <summary>
        /// 根据主键ID查询实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ID"></param>
        /// <returns></returns>
        T Find<T>(object ID) where T : new();

        /// <summary>
        /// 根据条件查询是否存在相应的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlCommand"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        bool? Any(string sqlCommand, object parameter = null);

        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="sqlCommand">sql</param>
        /// <param name="parameter">参数</param>
        /// <returns>查询结果</returns>
        T SingleQuery<T>(string sqlCommand, object parameter = null) where T : new();

        /// <summary>
        /// 查询多条数据
        /// </summary>
        /// <typeparam name="T">映射类型</typeparam>
        /// <param name="sqlCommand">sql</param>
        /// <param name="parameter">参数</param>
        /// <returns>查询结果</returns>
        IList<T> QueryList<T>(string sqlCommand, object parameter = null) where T : new();

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="pageInfo"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        PageResult PaginationQuery<T>(string sqlCommand, PageInfo pageInfo, OracleDbParameters parameter=null);
    }
}
