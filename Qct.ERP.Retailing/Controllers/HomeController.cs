﻿using Qct.IRepository;
using Qct.IServices;
using Qct.Objects.ValueObjects;
using System.Collections.Generic;
using System.Web.Mvc;
using Qct.Infrastructure.Helpers;
using Qct.Services;
using System;
using Qct.Infrastructure.Json;

namespace Qct.ERP.Retailing.Controllers
{
    public class HomeController : Controller
    {

        #region 首页
        readonly ISysMenuService _sysMenuService = null;
        readonly IIndentOrderRepository _orderRepository = null;
        readonly INoticeRepository _noticeRepository = null;
        readonly ISysWebSettingRepository _sysWebSettingRepository = null;
        public HomeController(INoticeRepository noticeRepository, ISysMenuService sysMenuService, IIndentOrderRepository orderRepository,
            ISysWebSettingRepository sysWebSettingRepository)
        {
            _orderRepository = orderRepository;
            _sysMenuService = sysMenuService;
            _noticeRepository = noticeRepository;
            _sysWebSettingRepository = sysWebSettingRepository;
        }
        public ActionResult Index()
        {
            
            if (!SysUserService.IsLogin)
            {
                return RedirectToAction("Login", "Account"); 
            }
            
            #region 验证
            /*
            //1为单商户版本，其他值为多商户版本
            string ver = Config.GetAppSettings("ver");
            //单商户版本
            if (ver == "1")
            {
                if (!SysUserService.IsLogin)
                {
                    Response.Redirect("/Account/Login");
                }
            }
            //多商户版本
            else
            {
                //二级域名
                string d = "";

                //二级域名
                string dom = "";
                if (!RouteData.Values["dom"].IsNullOrEmpty())
                {
                    dom = RouteData.Values["dom"].ToString();
                }
                //一级域名
                string d1 = "";
                if (!RouteData.Values["d1"].IsNullOrEmpty())
                {
                    d1 = RouteData.Values["d1"].ToString();
                }
                //顶级域名
                string d0 = "";
                if (!RouteData.Values["d0"].IsNullOrEmpty())
                {
                    d0 = RouteData.Values["d0"].ToString();
                }

                if (!d0.IsNullOrEmpty())
                {
                    if (!dom.IsNullOrEmpty())
                    {
                        d = dom;
                    }
                }

                //输入保留二级域名:store
                if (!RouteData.Values["cid"].IsNullOrEmpty())
                {
                    d = "store";
                }
                
                //localhost访问、ip访问
                if ((dom.ToLower().Trim() == "localhost") || (dom.IsNullOrEmpty() && d1.IsNullOrEmpty() && d0.IsNullOrEmpty()))
                {
                    if (!SysUserService.IsLogin)
                    {
                        Response.Redirect("/Account/Login");
                        return null;
                    }
                }
                //域名访问
                else
                {
                    //API的CID
                    int cID = Authorize.getCID(d);

                    //输入保留二级域名:store
                    if (d.ToLower().Trim().Contains("store") && cID == -1)
                    {
                        //门店
                        Response.Redirect("/Store/Index");
                        return null;
                    }
                    //请求API发生错误
                    else if (cID == -2)
                    {
                        Response.Redirect("/Account/error");
                        return null;
                    }
                    //输入的二级域名是空
                    else if (cID == -1)
                    {
                        Response.Redirect("/Account/noBusiness");
                        return null;
                    }
                    //输入的域名不存在商户
                    else if (cID == 0)
                    {
                        Response.Redirect("/Account/noBusiness");
                        return null;
                    }
                    //输入的域名是保留二级域名
                    else if (cID == -3)
                    {
                        //在crm里面
                        if (d.ToLower() == "erp")
                        {
                            if (!SysUserService.IsLogin )
                            {
                                Response.Redirect("/Account/Login");
                                return null;
                            }
                        }
                        //不在crm里面
                        else
                        {
                            Response.Redirect("/Account/noBusiness");
                            return null;
                        }
                    }
                    //输入的域名存在商户
                    else if(cID>0)
                    {
                        var obj = UserInfoService.Find(o => o.CompanyId == cID);
                        //CID在目前项目不存在
                        if (obj == null)
                        {
                            Response.Redirect("/Account/noUser?cid=" + cID);
                            return null;
                        }
                        else
                        {
                            if (!SysUserService.IsLogin )
                            {
                                Response.Redirect("/Account/Login");
                                return null;
                            }
                            else
                            {
                                if (Cookies.IsExist("remuc"))
                                {
                                    //cookie的CID
                                    string cid = Cookies.Get("remuc", "_cid");
                                    if (cid.IsNullOrEmpty())
                                    {
                                        cid = "0";
                                    }

                                    if (cID != Convert.ToInt32(cid))
                                    {
                                        Response.Redirect("/Account/Login");
                                        return null;
                                    }
                                }
                            }
                        }

                    }
                }
            }*/
            #endregion

            var user = SysUserService.CurrentUser;
            //获取活动列表 
            //var activityList = CommodityPromotionService.GetNewestActivity(3);
            //获取公告列表
            var noticeList = _noticeRepository.GetNewestNotice(3, user.StoreId);
            //采购订单列表
            ViewBag.OrderList = _orderRepository.GetNewOrder(3, user.StoreId);
            
            List<ActivityNoticeModel> activityNoticeList = new List<ActivityNoticeModel>();
            //if (activityList != null)
            //{
            //    foreach (var activity in activityList)
            //    {
            //        activityNoticeList.Add(new ActivityNoticeModel(activity.Id, Enum.GetName(typeof(PromotionType), activity.PromotionType),
            //            DateTime.Parse(activity.StartDate.ToString()).ToString("yyyy-MM-dd") + "至" + DateTime.Parse(activity.EndDate.ToString()).ToString("yyyy-MM-dd"),
            //            Enum.GetName(typeof(SaleState), activity.State), activity.CreateDT, 1));
            //    }
            //}
            //if (noticeList != null)
            //{
            //    foreach (var notice in noticeList)
            //    {
            //        activityNoticeList.Add(new ActivityNoticeModel(notice.Id.ToString(), notice.Theme, notice.BeginDate.ToString("yyyy-MM-dd") + "至" + notice.ExpirationDate.ToString("yyyy-MM-dd"),
            //            notice.State == 1 ? "已发布":"未发布",notice.CreateDT,2));
            //    }
            //}
            //activityNoticeList = activityNoticeList.OrderByDescending(o => o.CreateDT).Take(3).ToList();
            //if (activityNoticeList == null)
            //    activityNoticeList = new List<ActivityNoticeModel>();
            ViewBag.activityNoticeList = activityNoticeList;//活动公告


            ////todo: 模拟数据
            //string mode = Request["mode"];
            //ViewBag.accessCount = 0;

            ViewBag.WelcomeText = "欢迎光临";
            ViewBag.CurUserName = user.FullName;
            ViewBag.CurLoginName = user.Account;

            //近3天数据
            //var beginTime = DateTime.Parse(DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"));
            //var endTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            ViewBag.newMemberNumber = 10; //MembersService.GetNewMemberNumber(beginTime, endTime);//新增会员数量
            ViewBag.newSalesVolume = 20;// ReportBLL.GetSalesVolume(beginTime, endTime);//新增销售量
            //var saleOrderList3Day = SaleOrdersService.GetIndexSaleOrder(beginTime, endTime);//3天内的销售订单
            ViewBag.newSaleOrderNumber = 0;// saleOrderList3Day.Count();//新增客单量
            decimal newSaleTotal = 0;
            //newSaleTotal = saleOrderList3Day.Sum(o => o.Receive);
            ViewBag.newSaleTotal = newSaleTotal;//新增销售额

            //近7天数据
            var dayTitleList = new List<string>();
            var saleTotalList = new List<decimal>();
            var saleOederNumberList = new List<int>();
            var hotProductNameList = new List<string>();
            var hotProductSaleNumList = new List<int>();
            //for (int i = 6; i >= 0; i--)
            //{
            //    var time1 = DateTime.Parse(DateTime.Now.AddDays(0 - i).ToString("yyyy-MM-dd"));
            //    var time2 = DateTime.Parse(DateTime.Now.AddDays(0 - i + 1).ToString("yyyy-MM-dd"));
            //    var saleOrderList = SaleOrdersService.GetIndexSaleOrder(time1, time2);

            //    dayTitleList.Add(int.Parse(DateTime.Now.AddDays(0 - i).ToString("dd")) + "日");
            //    saleTotalList.Add(saleOrderList.Sum(o => o.Receive));
            //    saleOederNumberList.Add(saleOrderList.Count());
            //}

            var hotProductBeginTime = DateTime.Parse(DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd"));
            var hotProductEndTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));

            //ReportBLL.GetHotProduct(hotProductBeginTime, hotProductEndTime, out hotProductNameList, out hotProductSaleNumList);

            //近7天热销商品
            ViewBag.hotProductNameList = hotProductNameList.ToJson();
            ViewBag.hotProductSaleNumList = hotProductSaleNumList.ToJson();
            ViewBag.hotProductNameListNotJson = hotProductNameList;

            ViewBag.dayTitleList = dayTitleList.ToJson();
            ViewBag.saleTotalList = saleTotalList.ToJson();//近7天销售额
            ViewBag.saleOederNumberList = saleOederNumberList.ToJson();//近7天客单量
            
            var list = new List<MenuModel>();
            list = _sysMenuService.GetHomeMenusByUID(user.UserID);
            ViewBag.Menus = list;
            var set= _sysWebSettingRepository.GetWebSetting();
            ViewBag.comptitle = set == null ? "ERP管理平台" : set.SysName;
            return View(set);
        }
        #endregion

        public ActionResult Logout()
        {
            SysUserService.Exit();
            //if(SysUserService.StoreId=="sup")//供应平台
            //    return RedirectToAction("Login", "Supplier");
            Session.Clear();
            Session.Abandon();
            CookieHelper.Remove("ASP.NET_SessionId");//重新会话ID
            return RedirectToAction("Login", "Account");
        }
        /// <summary>
        /// 获得首页弹窗提醒
        /// </summary>
        /// <param name="type">提醒类型</param>
        /// <returns>提醒列表（最多6条）</returns>
        [HttpPost]
        public ActionResult GetRemind(string type)
        {
            return null;
            /*
            List<RemindModel> rmList = new List<RemindModel>();
            switch (type.ToLower())
            {
                case "stockout"://缺货提醒
                    var datas = CommodityService.GetStockout().GroupBy(o => o.Key);
                    foreach (var item in datas)
                    {
                        rmList.Add(new RemindModel(item.Key + "缺货提醒", item.Key + "以下商品缺货：<br/>" + string.Join(",", item.Select(o => o.Value))));
                    }
                    break;
                case "activity"://活动提醒
                    Dictionary<short, string> promotionTypeDict = new Dictionary<short, string>();
                    //1:单品折扣、 2:捆绑促销、 3:组合促销、4:买赠促销、 5:满元促销
                    promotionTypeDict.Add(1, "单品折扣");
                    promotionTypeDict.Add(2, "捆绑促销");
                    promotionTypeDict.Add(3, "组合促销");
                    promotionTypeDict.Add(4, "买赠促销");
                    promotionTypeDict.Add(5, "满元促销");
                    var promotions = CommodityPromotionService.GetNewestActivity(10);
                    foreach (var item in promotions)
                    {
                        var storeids = item.StoreId.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        var stores = WarehouseService.FindList(o => storeids.Contains(o.StoreId)).Select(o => o.Title);
                        rmList.Add(
                            new RemindModel(
                           string.Format(
                                 "{1}~{2} {0}",
                                 promotionTypeDict[item.PromotionType], (item.StartDate ?? new DateTime()).ToString("yyyy-MM-dd"),
                                (item.EndDate ?? new DateTime()).ToString("yyyy-MM-dd")),
                         item.Id));
                    }
                    break;
                case "receive"://收货提醒
                    var orderDatas = OrderDistributionService.GetReceivedOrder();
                    foreach (var item in orderDatas)
                    {
                        rmList.Add(new RemindModel(string.Format("{0}有订单发货，请注意查收！", item.Store), string.Format("<br/>门店：{0}<br/>配送批次号：{1}<br/>订单编号：{2}<br/>", item.Store, item.DistributionBatch, item.IndentOrderId)));
                    }
                    break;
                case "expiration"://保质期到期提醒
                    var commodities = CommodityService.GetExpiresProduct();
                    foreach (var item in commodities)
                    {
                        rmList.Add(new RemindModel(string.Format("{0}已过期或将要过期", item.Key), string.Format("{0}将要过期<br/>过期时间：{1}", item.Key, item.Value.ExpirationDate)));
                    }
                    break;
                case "contract"://合同提醒
                    var contracts = ContractSerivce.GetContractRemind();
                    foreach (var item in contracts)
                    {
                        rmList.Add(new RemindModel(string.Format("<span style=\"width:120px;display:inline-block;\">{0}</span><span style=\"width:110px;display:inline-block;\">{1}</span><span style=\"width:110px;display:inline-block;\">{2}</span>", item.ContractSN, item.SupplierTitle, item.EndDate), 
                                  string.Format("合同编号：{0}<br/>供应商：{1}<br/>结束日期：{2}",item.ContractSN,item.SupplierTitle,item.EndDate)));
                    }
                    break;
            }

            return new JsonNetResult(rmList);*/
        }

        /// <summary>
        /// 各角色首页
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminIndex()
        {
            return View();
        }
        
    }
}
