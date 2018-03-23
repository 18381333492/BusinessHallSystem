using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Dapper;
using Framework.Utility.Model;

namespace Framework.DBAccess
{
    /// <summary>
    /// Oracle的读操作
    /// </summary>
    public class ReadingManager :ReadingManagement,IReading
    {
        /// <summary>
        /// 根据条件查询是否存在相应的数据
        /// </summary>
        /// <param name="sqlCommand">sql命令</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public bool? Any(string sqlCommand, object parameter = null)
        {
            IDbConnection conn = null;
            try
            {
                conn = GetOracleConnection();
                if (conn == null) throw new ApplicationException("未获取到连接对象。");
                return DoAny(conn, sqlCommand, parameter);
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                logger.Fatal(ex.Message, ex);
                return null;
            }
            finally
            {
                CloseConnect(conn);
            }
        }

        /// <summary>
        /// 根据主键ID查找实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ID"></param>
        /// <returns></returns>
        public T Find<T>(object ID) where T : new()
        {
            IDbConnection conn = null;
            try
            {
                conn = GetOracleConnection();
                if (conn == null) throw new ApplicationException("未获取到连接对象。");
                string sqlCommand = string.Format("SELECT * FROM {0} WHERE ID=:ID", typeof(T).Name);
                return DoSingleQuery<T>(conn, sqlCommand, new { ID = ID });
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                logger.Fatal(ex.Message, ex);
                return default(T);
            }
            finally
            {
                CloseConnect(conn);
            }
        }

        /// <summary>
        /// 单条查询
        /// </summary>
        /// <typeparam name="T">查询的对象类型</typeparam>
        /// <param name="sqlCommand">sql命令</param>
        /// <param name="parameter">参数</param>
        /// <returns>查询结果</returns>
        public T SingleQuery<T>(string sqlCommand, object parameter) where T : new()
        {
            IDbConnection conn = null;
            try
            {
                conn = GetOracleConnection();
                if (conn == null) throw new ApplicationException("未获取到连接对象。");
                return DoSingleQuery<T>(conn, sqlCommand, parameter);
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                logger.Fatal(ex.Message, ex);
                return default(T);
            }
            finally
            {
                CloseConnect(conn);
            }
        }

        /// <summary>
        /// 查询多条数据
        /// </summary>
        /// <typeparam name="T">查询的对象类型</typeparam>
        /// <param name="sqlCommand">sql命令</param>
        /// <param name="parameter">参数</param>
        /// <returns>查询结果</returns>
        public IList<T> QueryList<T>(string sqlCommand, object parameter) where T : new()
        {
            IDbConnection conn = null;
            try
            {
                conn = GetOracleConnection();
                if (conn == null) throw new ApplicationException("未获取到连接对象。");
                return DoQueryList<T>(conn, sqlCommand, parameter);
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                logger.Fatal(ex.Message, ex);
                return default(IList<T>);
            }
            finally
            {
                CloseConnect(conn);
            }
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="pageInfo"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public PageResult PaginationQuery<T>(string sqlCommand, PageInfo pageInfo, OracleDbParameters parameter = null)
        {
            IDbConnection conn = null;
            try
            {
                if (parameter == null) parameter = new OracleDbParameters();
                conn = GetOracleConnection();
                if (conn == null) throw new ApplicationException("未获取到连接对象。");
                return DoPaginationQuery<T>(conn, sqlCommand, pageInfo, parameter.GetParameters());
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                logger.Fatal(ex.Message, ex);
                return default(PageResult);
            }
            finally
            {
                CloseConnect(conn);
            }
        }
    }
}
