using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.DBAccess;

namespace Framework.Services
{
    public class BaseServices
    {
        /// <summary>
        /// DB的查询接口
        /// </summary>
        protected static IReading query
        {
            get;
            set;
        }

        /// <summary>
        ///  DB的操作接口
        /// </summary>
        protected static IWriting excute
        {
            get;
            set;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        static BaseServices()
        {
            //实例化DB的查询接口
            query = new ReadingManager();

            //实例化DB操作接口
            excute = new WritingManager();
        }
    }
}
