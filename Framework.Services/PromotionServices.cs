using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Entity;
using Framework.Utility.Model;
using Framework.DBAccess;
using Framework.Utility.Tools;
using System.Threading.Tasks;

namespace Framework.Services
{
    /// <summary>
    /// 业务活动的相关逻辑
    /// </summary>
    public class PromotionServices : BaseServices
    {
        private static PromotionServices instance = null;
        public static PromotionServices Instance
        {
            get 
            {
                if (instance == null)
                    instance = new PromotionServices();
                return instance;         
            }
        }

        /// <summary>
        /// 获取平台设置的活动
        /// </summary>
        /// <returns></returns>
        public object GetPlatformPromotion()
        {
            //获取平台有效的活动推送
            var movement = query.SingleQuery<dynamic>(@"select movement_id 'movementid'  from business_movement 
                                                                                           where start_time<=sysdate
                                                                                             and end_time>=sysdate
                                                                                             and status=0");
           if (movement != null)
           {//存在有效的活动
               //获取推送下面的活动
               var movementDetailList = query.QueryList<Dictionary<string, object>>(@"select distinct t.promotion_id 'objectId',
                                                                                             t.rate 'rate'
                                                                                            from movement_detail t
                                                                                            inner join business_bind t1 on t1.promotion_id=t.promotion_id
                                                                                            where t.movement_id=:movement_id order by t.rate asc", new { movement_id = movement.movementid }).ToList();
               int promotion_id = 0;//活动Id
               if (movementDetailList != null && movementDetailList.Count > 0)
               {//存在活动
                   if (movementDetailList.Count == 1)
                   {
                       promotion_id = Convert.ToInt32(movementDetailList[0]["objectId"]);
                   }
                   if (movementDetailList.Count > 1)
                   {//存在多条的情况
                       //按权重分配
                       promotion_id = GetRandom(movementDetailList);
                   }
                   //获取活动信息
                   var result = query.SingleQuery<dynamic>(@"select promotion_id 'promotion_id',
                                                                    title  'title',
                                                                    img_url  'img_url'
                                                                    from business_promotion 
                                                                    where promotion_id=:promotion_id", new { promotion_id = promotion_id });
                   return result;
               }
               else
               {//不存在活动
                   return null;
               }
           }
           else
           {//不存在有效的活动推送
//               var promotion = query.SingleQuery<dynamic>(@"select t.promotion_id 'promotion_id',
//                                                                   t.title 'title',
//                                                                   t.img_url 'img_url'
//                                                                   from business_promotion t
//                                                                   inner join business_bind t1 on t.promotion_id=t1.promotion_id
//                                                                   where t.start_time<=sysdate
//                                                                   and t.end_time>=sysdate
//                                                                   and t.status=0
//                                                                   and t.promotion_type=2 order by sort desc");
//               return promotion;
               return null;
           }
        }

        /// <summary>
        /// 分页获取营业厅的活动列表
        /// </summary>
        /// <param name="hallid"></param>
        /// <returns></returns>
        public object GetHallPromotionListByPage(int hallid,PageInfo pageInfo)
        {
            OracleDbParameters Parameters=new OracleDbParameters();
            Parameters.Add("hallid",hallid);
            var pageResult=query.PaginationQuery<dynamic>(@"select t.promotion_id 'promotionid',
                                                                   (case when t.promotion_type=1 then t1.pic_url 
                                                                         else t.img_url
                                                                    end ) 'imgpath',
                                                                    t.title  'title',
                                                                    t.click_count 'num',
                                                                    t.promotion_type 'promotiontype',
                                                                    t.sort 'sort',
                                                                    t.create_time 'createtime'
                                                                    from business_promotion t 
                                                                    left join promotion_template t1 on t1.template_id=t.template_id
                                                                    where t.status=0 
                                                                    and t.end_time>=sysdate
                                                                    and t.promotion_id in
                                                                    (select t1.promotion_id from business_bind t1 where t1.hall_id=:hallid and t1.bind_type=1 --平台绑定营业厅
                                                               union select t2.promotion_id from business_promotion t2 where t2.hall_id=:hallid and t2.promotion_type!=2 --该营业厅下营业员自建的
                                                               union select t3.promotion_id from business_bind t3 where t3.bind_type=0 and t3.user_id in 
                                                                                                            (select waiter_id from user_waiter where hall_id=:hallid)
                                                                    ) order by t.create_time desc", pageInfo, Parameters);
            return pageResult;
        }

        /// <summary>
        /// 分页获取营业员的活动列表
        /// </summary>
        /// <param name="waiterid">服务员Id</param>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        public object GetWaiterPromotionListByPage(int waiterid, PageInfo pageInfo)
        {
            OracleDbParameters Parameters = new OracleDbParameters();
            Parameters.Add("waiterid", waiterid);
            var pageResult = query.PaginationQuery<dynamic>(@"select t.promotion_id 'promotionid',
                                                                     t.title 'title',
                                                                     (case when t.promotion_type=1 then t1.pic_url
                                                                           else t.img_url 
                                                                      end ) 'imgpath',
                                                                     t.promotion_type 'promotiontype',
                                                                     t.sort 'sort',
                                                                     t.create_time 'createtime'
                                                                    from business_promotion t 
                                                                    left join promotion_template t1 on t1.template_id=t.template_id
                                                                    where t.status=0
                                                                      and t.end_time>=sysdate
                                                                      and t.promotion_id in
                                                                      (select t1.promotion_id from business_bind t1 where t1.user_id=:waiterid and t1.bind_type=0 --平台绑定给营业员
                                                                 union select t2.promotion_id from business_promotion t2 where t2.promotion_type!=2 and t2.user_id in --该营业厅下所有营业员自建的
                                                                       (select waiter_id from user_waiter where hall_id in (select hall_id from user_waiter where waiter_id=:waiterid ))
                                                                 union select t3.promotion_id from business_bind t3 where t3.bind_type=1 and t3.hall_id in --绑定给营业员所属营业厅的
                                                                                                   (select hall_id from user_waiter where waiter_id=:waiterid)
                                                                      ) order by t.create_time desc", pageInfo, Parameters);
            return pageResult;
        }

        /// <summary>
        /// 获取业务活动详情
        /// </summary>
        /// <param name="promotionid">活动ID</param>
        /// <param name="UserId">用户ID</param>
        /// <param name="WaiterId">指定的服务员的ID</param>
        /// <param name="HallId">指定的营业厅的ID</param>
        /// <returns></returns>
        public Dictionary<string, object> GetPromotionDetail(int promotionid, int UserId, int? WaiterId,int ? HallId)
        {
            //获取活动信息
            var obj = query.SingleQuery<Dictionary<string, object>>(@"select  t.title 'title',
                                                                              t.content 'content',
                                                                              t.user_id 'waiterid',
                                                                              t.promotion_type 'promotiontype',
                                                                              (case  when t.promotion_type=1 then t1.pic_url
                                                                                     else t.img_url
                                                                               end) 'imgurl',
                                                                              t.start_time 'starttime',
                                                                              t.end_time 'endtime',
                                                                              t.status 'status'
                                                                              from business_promotion t
                                                                              left join promotion_template t1 on t1.template_id=t.template_id 
                                                                              where t.promotion_id=:promotionid", new { promotionid = promotionid });
            if (obj == null)
            {//不存在有效的活动
                return obj;
            }
            DateTime start_time = Convert.ToDateTime(obj["starttime"]);//活动开始时间
            DateTime endtime = Convert.ToDateTime(obj["endtime"]);//活动结束时间
            bool online =DateTime.Now <= endtime ? true : false;
            obj.Add("online", online);//活动是否过期
            //处理时间格式
            obj["starttime"] = start_time.ToString("yyyy-MM-dd HH:mm");
            obj["endtime"] = endtime.ToString("yyyy-MM-dd HH:mm");

            //异步更新活动点击数
            if (online)
            {//有效才更新点击数
                Task.Factory.StartNew(() =>
                {
                    excute.ExcuteSql(@"update business_promotion set click_count=click_count+1 where promotion_id=:promotionid", new { promotionid = promotionid });
                });
            }
            //业务的类型 1-营业员自建的 2-平台的创建 3营业员自建的上传图片
            int promotiontype = Convert.ToInt32(obj["promotiontype"]);
            //活动绑定的营业员
            int waiterid = 0;
            if (WaiterId == null)
            {//没有指定营业员
                if (promotiontype == 1 || promotiontype == 3)
                {
                    waiterid = Convert.ToInt32(obj["waiterid"]);
                }
                if (promotiontype == 2)
                {//平台的创建的活动 判断绑定的类型

                    var bingList = query.QueryList<Dictionary<string, object>>(@"select (case when t.bind_type=0 then t.user_id
                                                                                          when t.bind_type=1 then t.hall_id 
                                                                                        end) 'objectId',
                                                                                        t.rate 'rate',
                                                                                        t.bind_type 'bingtype', --绑定类型0：服务员，1：营业厅
                                                                                        t1.hall_id  'hallid'    --营业厅Id
                                                                                        from business_bind t 
                                                                                        left join user_waiter t1 on t1.waiter_id=t.user_id 
                                                                                        where promotion_id=:promotionid", new { promotionid = promotionid }).ToList();

                    if (bingList.Count == 0)
                    {
                        obj.Remove("waiterid");
                        obj.Add("waiterInfo", null);
                        return obj;
                    }
                    int bingtype = Convert.ToInt32(bingList[0]["bingtype"]);//0：服务员，1：营业厅
                    int objectId = Convert.ToInt32(bingList[0]["objectId"]);//绑定的对象ID
                    List<int> array = new List<int>();
                    if (bingtype == 0)
                    {//绑定的营业员
                        if (bingList.Count == 1)
                        {
                            waiterid = objectId;
                        }
                        else
                        {//绑定多个营业员
                            //权重比例分配
                            if (HallId != null)
                            {//指定了营业厅
                                bingList = bingList.Where(m => Convert.ToString(m["hallid"]) == HallId.Value.ToString()).ToList();
                                if (bingList.Count > 1)
                                    waiterid = GetRandom(bingList);
                                else
                                    waiterid = Convert.ToInt32(bingList[0]["objectId"]);
                            }
                            else
                            {
                                waiterid = GetRandom(bingList);
                            }
                        }
                    }
                    else
                    {//绑定的营业厅
                        if (bingList.Count == 1)
                        {//绑定一个营业厅
                            //获取营业厅下面的营业员
                            waiterid = GetWaiterIdByHallId(objectId);
                        }
                        else
                        {//绑定多个营业厅
                            int hallid = 0;
                            if (HallId != null)
                            {//指定了营业厅
                                hallid = HallId.Value;
                            }
                            else
                            {
                                hallid = GetRandom(bingList);
                            }
                            waiterid = GetWaiterIdByHallId(hallid);
                        }
                    }
                }
            }
            else
            {//固定的营业员
                waiterid = WaiterId.Value;
            }
            //获取营业员信息
            var waiterInfo = query.SingleQuery<Dictionary<string, object>>(@"select        t.waiter_id   'waiterid',
                                                                                           t.waiter_name 'waitername',
                                                                                           t.img_url     'waiterimg',
                                                                                           (case when t.user_follow is null
                                                                                                 then 0
                                                                                                 else t.user_follow
                                                                                           end) 'waiterfollow',
                                                                                           t.weixin_num  'weixinno',
                                                                                           t2.address    'address',
                                                                                           (case when t1.id is null
                                                                                                 then  0
                                                                                                 else  1
                                                                                           end) 'isfollow',
                                                                                           t.isdisply_wxno 'isdisply_wxno',
                                                                                           t2.name 'hallname',
                                                                                           t.mobile 'mobile'
                                                                                           from user_waiter t
                                                                                           left join user_follow t1
                                                                                           on t1.follow_num=t.waiter_id and t1.user_id=:userid and t1.follow_type=1
                                                                                           left join business_hall t2 on t2.hall_id=t.hall_id
                                                                                           where t.waiter_id=:waiterid", new { waiterid = waiterid, userid = UserId });
            obj.Remove("waiterid");
            waiterInfo["isfollow"] = Convert.ToInt32(waiterInfo["isfollow"]) == 0 ? false : true;
            waiterInfo["isdisply_wxno"] = waiterInfo["isdisply_wxno"].ToString() == "0" ? true : false;
            obj.Add("waiterInfo", waiterInfo);
            return obj;
        }

        /// <summary>
        /// 根据营业厅的ID获取营业厅下面的营业员
        /// </summary>
        /// <param name="hallid"></param>
        /// <returns></returns>
        private int GetWaiterIdByHallId(int hallid)
        {
            var waiterList = query.QueryList<Dictionary<string, object>>(@"select t.waiter_id 'objectId',
                                                                                  t.waiter_level 'rate'
                                                                                  from user_waiter t 
                                                                                  where t.hall_id=:hallid and t.status=0", new { hallid = hallid }).ToList();
            int waiterid = GetRandom(waiterList);
            return waiterid;
        }

        /// <summary>
        /// 权重算法
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private int GetRandom(List<Dictionary<string,object>> list)
        {
            List<int> array = new List<int>();
            foreach (var item in list)
            {
                int value = Convert.ToInt32(item["objectId"]);
                int count = Convert.ToInt32(item["rate"]);
                for (var i = 0; i < count; i++)
                {
                    array.Add(value);
                }
            }
            int result = 0;
            if (array.Count != 0)
            {
                result = array[new Random().Next(0, array.Count)];
            }
            return result;
        }

        /// <summary>
         /// 分页获取正在推广的活动列表
         /// </summary>
         /// <param name="pageInfo"></param>
         /// <param name="SellerId"></param>
         /// <returns></returns>
        public object GetExteningPromotionList(PageInfo pageInfo, int SellerId)
        {
            OracleDbParameters Parameters = new OracleDbParameters();
            Parameters.Add("waiterid", SellerId);
            var pageResult = query.PaginationQuery<dynamic>(@"select t.promotion_id 'promotionid',
                                                                     t.title 'title',
                                                                     (case when t.promotion_type=1 then t1.pic_url 
                                                                           else t.img_url
                                                                      end ) 'picurl',
                                                                     t.promotion_type 'promotiontype',
                                                                     t.promotion_no 'promotionno',
                                                                     t.click_count 'clickcount',
                                                                     t.sort 'sort',
                                                                     t.create_time  'createtime',
                                                                     (case when t.promotion_type=1 then t1.share_img
                                                                           when t.promotion_type=2 then t.share_img
                                                                           when t.promotion_type=3 then t.img_url
                                                                     end) 'shareimg'
                                                                    from business_promotion t 
                                                                    left join promotion_template t1 on t1.template_id=t.template_id
                                                                    where t.status=0
                                                                      and t.end_time>=sysdate
                                                                      and t.promotion_id in
                                                                      (select t1.promotion_id from business_bind t1 where t1.user_id=:waiterid and t1.bind_type=0 --平台绑定给营业员
                                                                 union select t2.promotion_id from business_promotion t2 where t2.promotion_type!=2 and t2.user_id in --该营业厅下所有营业员自建的
                                                                       (select waiter_id from user_waiter where hall_id in (select hall_id from user_waiter where waiter_id=:waiterid ))
                                                                 union select t3.promotion_id from business_bind t3 where t3.bind_type=1 and t3.hall_id in --绑定给营业员所属营业厅的
                                                                                                   (select hall_id from user_waiter where waiter_id=:waiterid)
                                                                      ) order by t.create_time desc", pageInfo, Parameters);
            return pageResult;
        }

        /// <summary>
        /// 分页获取推广的历史记录
        /// </summary>
        /// <param name="pageInfo"></param>
        /// <param name="SellerId"></param>
        /// <returns></returns>
        public object GetHistoryPromotionList(PageInfo pageInfo, int SellerId)
        {
            OracleDbParameters Parameters = new OracleDbParameters();
            Parameters.Add("waiterid", SellerId);
            var pageResult = query.PaginationQuery<dynamic>(@"select t.promotion_id 'promotionid',
                                                                     t.title 'title',
                                                                     (case when t.promotion_type=1 then t1.pic_url 
                                                                           else t.img_url
                                                                      end ) 'picurl',
                                                                     t.promotion_type 'promotiontype',
                                                                     t.promotion_no 'promotionno',
                                                                     t.click_count 'clickcount',
                                                                     t.sort 'sort',
                                                                     t.create_time 'createtime'
                                                                    from business_promotion t 
                                                                    left join promotion_template t1 on t1.template_id=t.template_id
                                                                    where t.status=0 
                                                                      and t.end_time<sysdate
                                                                      and t.promotion_id in
                                                                      (select t1.promotion_id from business_bind t1 where t1.user_id=:waiterid and t1.bind_type=0 --平台绑定给营业员
                                                                 union select t2.promotion_id from business_promotion t2 where t2.promotion_type!=2 and t2.user_id in --该营业厅下所有营业员自建的
                                                                       (select waiter_id from user_waiter where hall_id in (select hall_id from user_waiter where waiter_id=:waiterid ))
                                                                 union select t3.promotion_id from business_bind t3 where t3.bind_type=1 and t3.hall_id in --绑定给营业员所属营业厅的
                                                                                                   (select hall_id from user_waiter where waiter_id=:waiterid)
                                                                      ) order by t.create_time desc", pageInfo, Parameters);
            pageResult.Data = new { status = Nbh_UsersServices.Instance.GetSellerStatus(SellerId) };
            return pageResult;
        }

        /// <summary>
        /// 分页获取模板数据列表
        /// </summary>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        public object GetPromotionTemplateList(PageInfo pageInfo, int SellerId)
        {
            var pageResult = query.PaginationQuery<dynamic>(@"select t.template_id 'templateid',
                                                                     t.template_name 'templatename',
                                                                     t.pic_url 'picurl'
                                                                     from promotion_template t where t.status=0 order by t.sort asc", pageInfo);
            pageResult.Data = new { status = Nbh_UsersServices.Instance.GetSellerStatus(SellerId) };
            return pageResult;
        }

        /// <summary>
        /// 创建推广
        /// </summary>
        /// <param name="Promotion"></param>
        /// <param name="UserId"></param>
        /// <param name="HallId"></param>
        /// <returns></returns>
        public int CreatePromotion(Business_Promotion Promotion,int UserId,int HallId)
        {
            Promotion.User_Id = UserId;
            Promotion.Hall_Id = HallId;
            Promotion.Status = 0;
            Promotion.Audit_State = 1;//审核通过
            Promotion.Create_Time = DateTime.Now;
            Promotion.Update_Time = DateTime.Now;
            Promotion.Create_User = UserId.ToString();
            Promotion.Update_User = UserId.ToString();
            Promotion.Click_Count = 0;
            Promotion.Promotion_No = "ACD" + MathHelper.RandomCodeNum(9);
            int res = excute.Insert<Business_Promotion>(Promotion);
            return res;
        }

        /// <summary>
        /// 编辑推广
        /// </summary>
        /// <returns></returns>
        public int UpdatePromotion(Business_Promotion Promotion)
        {
            string sSql = string.Empty;
            if (Promotion.Promotion_Type == 1)
            {
                sSql = @"update business_promotion set title=:title,
                                                       content=:content,
                                                       start_time=:start_time,
                                                       end_time=:end_time,
                                                       update_time=:update_time
                                                       where promotion_id=:id";

                return excute.ExcuteSql(sSql, new
                 {
                     title = Promotion.Title,
                     content = Promotion.Content,
                     start_time = Promotion.Start_Time,
                     end_time = Promotion.End_Time,
                     update_time = DateTime.Now,
                     id = Promotion.Promotion_Id
                 });
            }
            else
            {
                sSql = @"update business_promotion set title=:title,
                                                       img_url=:img_url,
                                                       start_time=:start_time,
                                                       end_time=:end_time,
                                                       update_time=:update_time
                                                       where promotion_id=:id";

                return excute.ExcuteSql(sSql, new
                {
                    title = Promotion.Title,
                    img_url = Promotion.Img_Url,
                    start_time = Promotion.Start_Time,
                    end_time = Promotion.End_Time,
                    update_time = DateTime.Now,
                    id = Promotion.Promotion_Id
                });
            }
        }
    }
}
