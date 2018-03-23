using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Entity;
using Framework.Utility.Model;
using Framework.DBAccess;
using System.Data;

namespace Framework.Services
{
    /// <summary>
    /// 用户表的相关逻辑
    /// </summary>
    public class Nbh_UsersServices : BaseServices
    {
        private static Nbh_UsersServices instance = null;
        public static Nbh_UsersServices Instance
        {
            get 
            {
                if (instance == null)
                    instance= new Nbh_UsersServices();
                return instance;         
            }
        }

        /// <summary>
        /// 根据OpenId获取用户信息
        /// </summary>
        /// <returns></returns>
        public Nbh_Users GetNbhUsers(string OpenId)
        {
            var User=query.SingleQuery<Nbh_Users>(@"select * from Nbh_Users where OpenId=:OpenId", new { OpenId = OpenId });
            return User;
        }

        /// <summary>
        /// 根据OpenId获取营业员
        /// </summary>
        /// <param name="OpenId"></param>
        /// <returns></returns>
        public dynamic GetUserWaiter(string OpenId)
        {
            var Waiter = query.SingleQuery<dynamic>(@"select * from User_Waiter where OpenId=:OpenId and status=0", new { OpenId = OpenId });
            return Waiter;
        }

        /// <summary>
        /// 用户的注册
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public int Register(Nbh_Users User)
        {
            int res = excute.Insert<Nbh_Users>(User);
            return res;
        }

        /// <summary>
        /// 用户关注
        /// </summary>
        /// <param name="followId">关注的对象的ID</param>
        /// <param name="followtype">关注的类型（0：营业厅，1：营业员）</param>
        /// <param name="UserId">用户ID</param>
        /// <returns></returns>
        public int Follow(int followId, int followtype,int UserId,out string weixinno)
        {
            OracleDbParameters Parameters = new OracleDbParameters();
            Parameters.Add("v_user_id", UserId, DbType.Int32);
            Parameters.Add("v_follow_id", followId, DbType.Int32);
            Parameters.Add("v_follow_type", followtype, DbType.Int32);
            Parameters.Add("v_code",null,DbType.Int32,ParameterDirection.Output);
            Parameters.Add("v_weixin_no",null,DbType.String, ParameterDirection.Output,50);
            //执行关注存储过程
            excute.ExcuteProcedure("sp_user_follow", Parameters);
            int res = Parameters.Get<int>("v_code")==100?1:0;
            weixinno = Parameters.Get<string>("v_weixin_no");
            return res;
        }

        /// <summary>
        /// 获取营业员的基本信息
        /// </summary>
        /// <param name="WaiterId"></param>
        /// <returns></returns>
        public object GetWaiterInfo(int WaiterId,int UserId)
        {
            var WaiterInfo = query.SingleQuery<Dictionary<string,object>>(@"select      t.img_url 'waiterimg',
                                                                                        t.waiter_name 'waitername',
                                                                                        (case when t.user_follow is null
                                                                                                then 0
                                                                                                else t.user_follow
                                                                                        end) 'waiterfollow',
                                                                                        t.weixin_num 'weixinno',
                                                                                        t1.address 'address',
                                                                                        t1.name 'hallname',
                                                                                        (case when t2.id is null
                                                                                                then  0
                                                                                            else  1
                                                                                         end) 'isfollow',
                                                                                        t.isdisply_wxno 'isdisply_wxno',
                                                                                        t.abstract 'abstract',
                                                                                        t.mobile 'mobile'
                                                                                        from user_waiter t 
                                                                                        left join business_hall t1 on t1.hall_id=t.hall_id
                                                                                        left join user_follow t2 on t2.follow_num=:waiterid and t2.user_id=:userid and follow_type=1
                                                                                        where t.waiter_id=:waiterid", new { waiterid = WaiterId, userid = UserId });
            WaiterInfo["isfollow"] = WaiterInfo["isfollow"].ToString() == "0" ? false : true;
            WaiterInfo["isdisply_wxno"] = WaiterInfo["isdisply_wxno"].ToString() == "0" ? true : false;
            return WaiterInfo;
        }

        /// <summary>
        /// 分页获取已关注营业员列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public object GetWaiterListByPage(int UserId, PageInfo pageInfo)
        {
            OracleDbParameters Parameters = new OracleDbParameters();
            Parameters.Add("userid", UserId);
           var pageResult=query.PaginationQuery<dynamic>(@" select  t1.waiter_id 'waiterid',
                                                                    t1.waiter_name 'waitername',
                                                                    t1.img_url 'imgpath',
                                                                    t1.user_follow 'waiterfollow',
                                                                    t1.weixin_num 'weixinno',
                                                                    t2.name 'name',
                                                                    t1.mobile 'mobile'
                                                                    from user_follow t
                                                                    left join user_waiter t1 on t1.waiter_id=t.follow_num
                                                                    left join business_hall t2 on t2.hall_id=t1.hall_id
                                                                    where t.follow_type=1 and t.user_id=:userid and t1.status=0
                                                                    order by t.follow_time desc", pageInfo, Parameters);
           return pageResult;
        }

        /// <summary>
        /// 获取商家信息
        /// </summary>
        /// <returns></returns>
        public object GetSellerInfo(int SellerId)
        {
            //获取商家基本信息
            var seller = query.SingleQuery<dynamic>(@"select t.waiter_id 'waiterid',
                                                             t.waiter_name 'waitername',
                                                             t.weixin_num 'weixinnum',
                                                             t.img_url   'imgurl',
                                                             (case when t.user_follow is null then 0
                                                                    else t.user_follow
                                                             end) 'userfollow',  
                                                             t.status 'status',
                                                             t1.address 'address',
                                                             t1.name 'hallname',
                                                             (case when t2.'follower' is null then 0
                                                                   else  t2.'follower'
                                                              end) 'nowfollower'
                                                             from user_waiter t 
                                                             left join business_hall t1 on t.hall_id=t1.hall_id
                                                             left join (select count(*)   'follower',
                                                                               follow_num 'waiterid' 
                                                                               from user_follow 
                                                                              where follow_type=1 
                                                                                and follow_time>=to_date(to_char(sysdate,\yyyy-mm-dd\)||\ 00:00:00\,\yyyy-mm-dd hh24:mi:ss\)
                                                                                and follow_time<=to_date(to_char(sysdate,\yyyy-mm-dd\)||\ 23:59:59\,\yyyy-mm-dd hh24:mi:ss\)
                                                                              group by follow_num
                                                                        ) t2 on t2.'waiterid'= t.waiter_id 
                                                            where t.waiter_id=:waiter_id", new { waiter_id = SellerId });
            return seller;
        }

        /// <summary>
        /// 根据手机号码查询营业员信息
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public dynamic GetSellerByMobile(string mobile)
        {
            var waiter = query.SingleQuery<dynamic>(@"select waiter_id 'waiterid',openid 'openid' from user_waiter where status=0 and mobile=:mobile", new { mobile = mobile });
            return waiter;
        }

        /// <summary>
        /// 根据openid判断营业员是否存在
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool isExitOpenid(string openid)
        {
           var result=query.Any(@"select * from user_waiter where openid=:openid", new { openid = openid });
           return result.Value;
        }

        /// <summary>
        /// 绑定营业员的OpenId
        /// </summary>
        /// <param name="waiterid"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public int BingSellerOpenId(int waiterid, string openid)
        {
            int res = excute.ExcuteSql(@"update user_waiter set openid=:openid where waiter_id=:waiterid", new { openid = openid, waiterid = waiterid });
            return res;
        }

        /// <summary>
        /// 获取服务员的状态
        /// </summary>
        /// <returns></returns>
        public int GetSellerStatus(int waiterid)
        {
            var res = query.SingleQuery<int>("select status from user_waiter where waiter_id=:waiter_id", new { waiter_id = waiterid });
            return res;
        }

        /// <summary>
        /// 根据营业员ID获取微信号
        /// </summary>
        /// <param name="waiterid"></param>
        /// <returns></returns>
        public string GetWeiXinNo(int waiterid)
        {
            var obj = query.SingleQuery<dynamic>(@"select  weixin_num 'weixin_num', openid 'openid' from user_waiter where waiter_id=:waiterid", new { waiterid = waiterid });
            if (obj != null)
            {
                return string.Format("{0},{1}", obj.weixin_num, obj.openid);
            }
            else
                return string.Empty;
        }

    }
}
