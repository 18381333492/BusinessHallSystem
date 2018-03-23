using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Entity;
using Framework.Utility.Model;
using Framework.DBAccess;
using Framework.Utility.Tools;
using System.Data;

namespace Framework.Services
{
    /// <summary>
    /// 业务活动的相关逻辑
    /// </summary>
    public class BusinessHallServices : BaseServices
    {
        private static BusinessHallServices instance = null;
        public static BusinessHallServices Instance
        {
            get
            {
                if (instance == null)
                    instance = new BusinessHallServices();
                return instance;
            }
        }

        /// <summary>
        /// 分页获取首页营业厅数据
        /// </summary>
        /// <param name="lon">用户的精度</param>
        /// <param name="lat">用户的纬度</param>
        /// <param name="carrierno">营业厅类型</param>
        /// <param name="pageindex">当前页</param>
        /// <param name="rows">每页数据条数</param>
        /// <returns></returns>
        public object GetHallByPage(decimal lon, decimal lat, int carrierno, PageInfo pageinfo)
        {
            OracleDbParameters Parameters = new OracleDbParameters();
            Parameters.Add("prelatitude", lat - 1, DbType.Double);
            Parameters.Add("lastlatitude", lat + 1, DbType.Double);
            Parameters.Add("prelongitude", lon - 1, DbType.Double);
            Parameters.Add("lastlongitude", lon + 1, DbType.Double);
            Parameters.Add("lon", lon, DbType.Double);
            Parameters.Add("lat", lat, DbType.Double);
            StringBuilder sSql = new StringBuilder();
            string WhereString = "1=1";
            if (carrierno>= 0)
            {
                WhereString = "carrier_no=:carrierno";
                Parameters.Add("carrierno", carrierno);
            }
            sSql.AppendFormat(@"select t1.* from
                                 (select t.hall_id 'hallid',
                                         t.name 'hallname',
                                         t.identification_type 'identificationtype',
                                         t.hall_follow 'follow',
                                         t.picpath 'picpath',
                                         t.longitude 'longitude',
                                         t.latitude 'latitude',
                                        (select t2.title from 
                                (select t.hall_id, t.title  from business_promotion t where t.promotion_type!=2 and t.status=0 and t.start_time<=sysdate and t.end_time>=sysdate order by t.create_time desc) t2
                                        where t2.hall_id=t.hall_id and rownum<=1) 'title'
                                 from business_hall t where t.latitude>:prelatitude 
                                                      and   t.latitude<:lastlatitude
                                                      and   t.longitude>:prelongitude 
                                                      and   t.longitude<:lastlongitude
                                                      and   t.status=0 and {0} 
                                                      order by abs(t.longitude-(:lon))+abs(t.latitude-(:lat))) t1", WhereString);
            var pageResult = query.PaginationQuery<Dictionary<string, object>>(sSql.ToString(), pageinfo, Parameters);
            var list = pageResult.Rows as List<Dictionary<string, object>>;
            foreach (Dictionary<string, object> item in list)
            {
                double latitude = Convert.ToDouble(item["latitude"]);//营业厅纬度
                double longitude = Convert.ToDouble(item["longitude"]);//营业厅经度
                //计算距离
                double distance = MathHelper.GetDistanceByTude((double)lat, (double)lon, latitude, longitude);
                item.Add("distance", distance.ToString("f1"));
                item["picpath"] = item["picpath"] != null ? item["picpath"].ToString().Split(',')[0] : string.Empty;
                item["title"] = item["title"] == null ? "暂无活动" : item["title"];
                item.Add("imgurl", string.Empty);

            }
            if (list != null)
                list = list.OrderBy(m =>Convert.ToDouble(m["distance"])).ToList();
            pageResult.Rows = list;
            return pageResult;
        }

        /// <summary>
        /// 获取搜索分页数据
        /// </summary>
        /// <returns></returns>
        public object GetSearchDataByPage(decimal lon, decimal lat, int carrierno, string keywords,PageInfo pageinfo)
        {
            OracleDbParameters Parameters = new OracleDbParameters();
            Parameters.Add("keywords", string.Format("%{0}%",keywords));
            Parameters.Add("prelatitude", lat - 1, DbType.Double);
            Parameters.Add("lastlatitude", lat + 1, DbType.Double);
            Parameters.Add("prelongitude", lon - 1, DbType.Double);
            Parameters.Add("lastlongitude", lon + 1, DbType.Double);

            StringBuilder sSql = new StringBuilder();
            string WhereString = "1=1";
            string AWhereString = "1=1";
            if (carrierno >= 0)
            {//运营商的查询
                WhereString = "carrier_no=:carrierno";
                AWhereString = "r1.carrier_no=:carrierno";
                Parameters.Add("carrierno", carrierno);
            }
            sSql.AppendFormat(@"select t.*
                                       from (select hall_id   'id',
                                                    name      'name',
                                                    0         'type', --类型 0营业厅 2活动
                                                    address   'address',
                                                    name      'hallname',
                                                    hall_id   'hallid',
                                                    longitude 'lon',
                                                    latitude  'lat'
                                                    from business_hall where status=0 and {0}
                                        union select r.'id',
                                                     r.'name',
                                                     r.'type',
                                                     r1.address 'address',
                                                     r1.name    'hallname',
                                                     r1.hall_id 'hallid',
                                                     r1.longitude 'lon',
                                                     r1.latitude 'lat'
                                                     from
                                                     (select    t.promotion_id  'id',
                                                                t1.title        'name',
                                                                2               'type',
                                                                (case when t.bind_type=1 then t.hall_id 
                                                                    when t.bind_type=0 then t2.hall_id
                                                                end) hall_id
                                                                from business_bind t
                                                                left join business_promotion t1 on t.promotion_id=t1.promotion_id
                                                                left join user_waiter t2 on t2.waiter_id=t.user_id and t2.status=0
                                                                where t1.status=0 and t1.start_time<=sysdate and t1.end_time>=sysdate
                                                                --两种活动 平台自建和营业员自建
                                           union    select      t.promotion_id  'id',
                                                                t.title        'name',
                                                                2               'type',
                                                                t.hall_id
                                                                from business_promotion t
                                                                where t.status=0 and t.start_time<=sysdate and t.end_time>=sysdate and t.promotion_type!=2
                                                    ) r
                                                    left join business_hall r1 on r.hall_id=r1.hall_id
                                                    where {1}
                                             ) t 
                                            where t.'name' like :keywords
                                                      and   t.'lat'>:prelatitude 
                                                      and   t.'lat'<:lastlatitude
                                                      and   t.'lon'>:prelongitude 
                                                      and   t.'lon'<:lastlongitude", WhereString, AWhereString);

            var pageResult=query.PaginationQuery<Dictionary<string, object>>(sSql.ToString(), pageinfo, Parameters);
            var list = pageResult.Rows as List<Dictionary<string, object>>;
            foreach (var item in list)
            {
                double latitude = Convert.ToDouble(item["lat"]);//营业厅纬度
                double longitude = Convert.ToDouble(item["lon"]);//营业厅经度
                
                double distance = MathHelper.GetDistanceByTude((double)lat, (double)lon, latitude, longitude);
                item.Add("distance", distance.ToString("f1"));
                item.Remove("lon");
                item.Remove("lat");
            }
            if (list != null)
                list = list.OrderBy(m => m["distance"]).ToList();
            pageResult.Rows = list;
            pageResult.Data = keywords;
            return pageResult;
        }

        /// <summary>
        /// 获取营业厅的信息
        /// </summary>
        /// <param name="hallid"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public object GetHallInfo(int hallid, int UserId)
        {
            //获取营业厅的信息
            var hallInfo = query.SingleQuery<Dictionary<string, object>>(@"select name 'hallname',
                                                                                  address 'address',
                                                                                  latitude 'latitude',
                                                                                  longitude 'longitude',
                                                                                  identification_type 'identificationtype',
                                                                                  picpath 'picpath',
                                                                                  hall_follow 'hallfollow',
                                                                                  status 'status'
                                                                                  from business_hall where hall_id=:hallid", new { hallid = hallid });
            //获取营业厅下面的营业员
            var waiterList = query.QueryList<dynamic>(@"select waiter_id 'waiterid', 
                                                              waiter_name 'waitername',
                                                              img_url 'waiterimg'
                                                              from user_waiter where status=0 and hall_id=:hallid and openid is not null order by waiter_level desc", new { hallid = hallid });
            //查询当前用户是否已经关注改营业厅
            var follow = query.Any(@"select * from user_follow where user_id=:userid and follow_type=0 and follow_num=:hallid", new { UserId = UserId, hallid = hallid });

            hallInfo.Add("waiterlist", waiterList);
            hallInfo.Add("isfollow", follow);
            return hallInfo;
        }

        /// <summary>
        /// 分页获取用户关注的营业厅列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        public object GetHallListByPage(int UserId, PageInfo pageInfo)
        {
            OracleDbParameters Parameters = new OracleDbParameters();
            Parameters.Add("userid", UserId);
            var pageResult=query.PaginationQuery<dynamic>(@"select  t.hall_id 'hallid',
                                                                    t.name 'hallname',
                                                                    t.identification_type 'identificationtype',
                                                                    t.picpath 'imgpath',
                                                                    t.hall_follow 'hallfollow',
                                                                    t2.allpromotion 'allpromotion'
                                                                    from business_hall t
                                                                    left join (select * from 
                                                                                (select count(*) allpromotion,hall_id from 
                                                                                    (select promotion_id,hall_id from business_promotion 
                                                                                                        where hall_id in (select follow_num from user_follow where user_id=:userid and follow_type=0)
                                                                            union select promotion_id,hall_id from business_bind 
                                                                                                        where hall_id in (select follow_num from user_follow where user_id=:userid and follow_type=0)
                                                                                    ) t1 group by t1.hall_id)
                                                                                ) t2 on t2.hall_id=t.hall_id
                                                                    where t.hall_id in(select follow_num from user_follow where user_id=:userid and follow_type=0)", pageInfo, Parameters);
            var list = (pageResult.Rows as IEnumerable<dynamic>).Select(m => { m.imgpath = Convert.ToString(m.imgpath).Split(',')[0]; return m; });
            pageResult.Rows = list;
            return pageResult;
        }

        /// <summary>
        /// 获取营业厅的详情
        /// </summary>
        /// <param name="hallid"></param>
        /// <returns></returns>
        public object GetHallDetail(int hallid)
        {
            var halldetail = query.SingleQuery<dynamic>(@"select t.name 'hallname',
                                                                 t.address 'address',
                                                                 t.longitude 'lon',
                                                                 t.latitude  'lat',
                                                                 t.identification_type 'identificationtype',
                                                                 t.hall_follow 'hallfollow',
                                                                 t.picpath 'imgurl'   
                                                                 from business_hall t where t.hall_id=hallid", new { hallid = hallid });
            return halldetail;
        }


    }
}
