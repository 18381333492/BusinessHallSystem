using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Framework.DBAccess;
using DapperExtensions;

namespace Framework.DBAccess
{
    /// <summary>
    /// Oracle数据库的写操作
    /// </summary>
    public class WritingManager : WritingManagement,IWriting
    {

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert<T>(T model) where T:class,new ()
        {
            IDbConnection conn = null;
            try
            {
                conn = GetOracleConnection();
                if (conn == null) throw new ApplicationException("未获取到连接对象.");
                return DoInsert<T>(conn, model);
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                logger.Fatal(ex.Message, ex);
                return -1;
            }
            finally
            {
                CloseConnect(conn);
            }
        }


        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update<T>(T model) where T : class,new()
        {
            IDbConnection conn = null;
            try
            {
                conn = GetOracleConnection();
                if (conn == null) throw new ApplicationException("未获取到连接对象.");
                return DoUpdate<T>(conn, model);
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                logger.Fatal(ex.Message, ex);
                return -1;
            }
            finally
            {
                CloseConnect(conn);
            }
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public int ExcuteSql(string sqlCommand, object parameter)
        {
            IDbConnection conn = null;
            try
            {
                conn = GetOracleConnection();
                if (conn == null) throw new ApplicationException("未获取到连接对象.");
                return DoExcuteSql(conn, sqlCommand, parameter);
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                logger.Fatal(ex.Message, ex);
                return -1;
            }
            finally
            {
                CloseConnect(conn);
            }
        }

        /// <summary>
        /// 执行储存过程
        /// </summary>
        /// <param name="sProcedureName"></param>
        /// <param name="Parameters"></param>
        public int ExcuteProcedure(string sProcedureName, OracleDbParameters Parameters)
        {
            IDbConnection conn = null;
            try
            {
                conn = GetOracleConnection();
                if (conn == null) throw new ApplicationException("未获取到连接对象.");
                return DoExcuteProcedure(conn, sProcedureName, Parameters.GetParameters());
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                logger.Fatal(ex.Message, ex);
                return -1;
            }
            finally
            {
                CloseConnect(conn);
            }
        }
    }
}
