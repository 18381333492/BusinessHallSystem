using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using Lib4Net.Logs;

namespace Framework.DBAccess
{
    public class DbBase
    {
        /// <summary>
        /// 日志记录
        /// </summary>
        protected static ILogger logger = LoggerManager.Instance.GetLogger("dapper");

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected static string sConnectionString = ConfigurationManager.ConnectionStrings["Oracle_Connect"].ConnectionString;

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <returns></returns>
        protected IDbConnection GetOracleConnection()
        {
            try
            {
                OracleConnection conn = new OracleConnection(sConnectionString);
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                logger.Fatal(ex.Message,ex);
                return null;
            }
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <param name="conn"></param>
        protected void CloseConnect(IDbConnection conn)
        {
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
}
