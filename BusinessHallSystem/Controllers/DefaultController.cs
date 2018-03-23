using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Framework.Services;
using Framework.Utility.Tools;
using Framework.Utility.Model;

namespace Web.Controllers
{
    public class DefaultController : Controller
    {
        //
        // GET: /Default/

        public  ActionResult  Index()
        {
            var res=PromotionServices.Instance.GetPlatformPromotion();
            return Json(res);
           //var ss=JsonHelper.ToJsonString(res);

          //  BusinessHallServices.Instance.GetHallByPage(103.92377m, 30.57447m, 0, new PageInfo() );

          //var res=BusinessHallServices.Instance.GetHallListByPage(24, new PageInfo());
          //var t= JsonHelper.ToJsonString(res);
          //return Content(t);
            //Nbh_UsersServices.Instance.Follow(1,1,24);
           // Nbh_UsersServices.Instance.GetSellerInfo(24);
            //Nbh_UsersServices.Instance.GetSellerStatus(24);
        }

    }
}
