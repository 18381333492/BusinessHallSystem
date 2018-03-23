using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.DBAccess;

namespace Framework.Services
{
    /// <summary>
    /// 系统的相关的业务逻辑
    /// </summary>
    public class SystemServices:BaseServices
    {
        private static SystemServices instance = null;
        public static SystemServices Instance
        {
            get
            {
                if (instance == null)
                    instance = new SystemServices();
                return instance;
            }
        }

        /// <summary>
        /// 纪录系统错误日志
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public int WriteLog(string error)
        {
           int res=excute.ExcuteSql(@"insert into ex_detail (ex_id,ex_info,create_time)values(seq_exdetail_auto_id.nextval,:info,sysdate)", new { info = error });
           return res;
        }


        /// <summary>
        /// 获取平台相关信息
        /// </summary>
        /// <returns></returns>
        public object GetPlatformContactInfo()
        {
           var obj=query.SingleQuery<dynamic>(@"select t.imgurl 'imgurl',
                                                t.wxno  'wxno',
                                                t.mobile 'mobile'
                                                from platform_contact t where status=0 order by sort desc");
           return obj;
        }
    }
}
