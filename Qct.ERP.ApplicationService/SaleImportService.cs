﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Qct.Objects.ValueObjects;
using Qct.ISevices;
using Qct.Objects.Entities;
using Qct.IRepository;
using Qct.Infrastructure.DI;
using Autofac;
using Qct.Infrastructure.Extensions;
using Qct.Services;
using Qct.Infrastructure.Log;
using Qct.IServices;

namespace Qct.ERP.ApplicationService
{
    /// <summary>
    /// 数据迁移，销售数据导入
    /// </summary>
    public class SaleImportService
    {
        readonly IImportSetService _importSetService;
        readonly ICacheService _cacheService;
        public SaleImportService(IImportSetService importSetService, ICacheService cacheService)
        {
            _importSetService = importSetService;
            _cacheService = cacheService;
        }
        public DataView SaleDataMoveList(string type, string apiTitle, string searchText, string searchField, ref object foots)
        {
            var dt = GetDataFromCache;
            DataView dv = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                dv = dt.DefaultView;
                //dv.Sort = "SaleDate asc";
                dv.RowFilter = "1=1";
                if (!type.IsNullOrEmpty())
                    dv.RowFilter += " and type=" + type;
                if (!apiTitle.IsNullOrEmpty())
                    dv.RowFilter += " and apiTitle like '%" + apiTitle + "%'";
                if (!searchText.IsNullOrEmpty())
                    dv.RowFilter += " and " + searchField + " like '%" + searchText + "%'";
                var saleList = dv.ToTable().AsEnumerable().GroupBy(o => new
                {
                    ApiOrderSN = o["ApiOrderSN"].ToString()
                }).ToList();
                decimal ApiCode_11 = 0, ApiCode_12 = 0, ApiCode_20 = 0, ApiCode_21 = 0, ApiCode_19 = 0, ApiCode_15 = 0, Change = 0, OrderDiscount = 0, WipeZero = 0, TotalAmount = 0, Receive = 0, SubTotal = 0, PurchaseNumber = 0;
                foreach (var sale in saleList)
                {
                    ApiCode_11 += sale.Max(o => o.GetValue("ApiCode_11").ToType<decimal>());
                    ApiCode_12 += sale.Max(o => o.GetValue("ApiCode_12").ToType<decimal>());
                    ApiCode_20 += sale.Max(o => o.GetValue("ApiCode_20").ToType<decimal>());
                    ApiCode_21 += sale.Max(o => o.GetValue("ApiCode_21").ToType<decimal>());
                    ApiCode_19 += sale.Max(o => o.GetValue("ApiCode_19").ToType<decimal>());
                    ApiCode_15 += sale.Max(o => o.GetValue("ApiCode_15").ToType<decimal>());
                    Change += sale.Max(o => o.GetValue("Change").ToType<decimal>());
                    OrderDiscount += sale.Max(o => o.GetValue("OrderDiscount").ToType<decimal>());
                    WipeZero += sale.Max(o => o.GetValue("OrderDiscount").ToType<decimal>());
                    TotalAmount += sale.Max(o => o.GetValue("TotalAmount").ToType<decimal>());
                    Receive += sale.Max(o => o.GetValue("Receive").ToType<decimal>());
                    SubTotal += sale.Sum(o => o.GetValue("SubTotal").ToType<decimal>());
                    PurchaseNumber += sale.Sum(o => o.GetValue("PurchaseNumber").ToType<decimal>());
                }
                foots = new List<object>(){new { ApiCode_11 = ApiCode_11, ApiCode_12 = ApiCode_12, ApiCode_20 = ApiCode_20, ApiCode_21 = ApiCode_21, ApiCode_19 = ApiCode_19, ApiCode_15 = ApiCode_15, PurchaseNumber=PurchaseNumber,
                    Change=Change, OrderDiscount=OrderDiscount, WipeZero=WipeZero, TotalAmount=TotalAmount,Receive=Receive,SubTotal=SubTotal,Type="-1",ApiTitle="合 计：" }};
            }
            return dv;
        }
        public OperateResult Import(ImportSet obj, System.Web.HttpFileCollectionBase httpFiles, string fieldName, string columnName)
        {
            var errLs = new List<string>();
            int count = 0, rowno = 0;
            var dt = new DataTable();
            try
            {
                Dictionary<string, char> fieldCols = null;
                var op = _importSetService.ImportSet(obj, httpFiles, fieldName, columnName, ref fieldCols, ref dt);
                if (!op.Successed) return op;
                var userRespository= AutofacBootstapper.CurrentContainer.Resolve<ISysUserRespository>();
                var users = userRespository.GetList(true);
                var barcodeIdx = Convert.ToInt32(fieldCols["Barcode"]) - 65;
                var barcodes = dt.AsEnumerable().Select(o => o[barcodeIdx].ToString()).Distinct().ToArray();
                var payments = new List<ConsumptionPayment>();
                foreach (var de in fieldCols)
                {
                    var idx = Convert.ToInt32(de.Value) - 65;
                    var col = dt.Columns[idx];
                    if (!dt.Columns.Contains(de.Key))
                        col.ColumnName = de.Key;
                }
                dt.Columns.Add("ApiTitle");
                dt.Columns.Add("ValuationType");
                dt.Columns.Add("ProductCode");
                dt.Columns.Add("BuyPrice");
                dt.Columns.Add("StoreId");
                dt.Columns.Add("SalesClassifyId2");
                dt.Columns.Add("CreateUID2");
                dt.Columns.Add("Salesman2");
                dt.Columns.Add("InInventory");
                dt.Columns.Add("Sort", typeof(int));
                object ApiOrderSN = "", ApiTitle = "", ApiCode_11 = 0, ApiCode_12 = 0, ApiCode_20 = 0, ApiCode_21 = 0, ApiCode_19 = 0, PreferentialPrice = 0, Change = 0, WipeZero = 0, TotalAmount = 0, Type = 0;
                int sort = 1;
                if (!dt.Columns.Contains("ApiOrderSN"))
                {
                    dt.Rows.Clear();
                    errLs.Add("销售流水号位置未指定!");
                }
                else
                {
                    var apiOrderSns = dt.AsEnumerable().Select(o => o["ApiOrderSN"].ToString()).Distinct().Where(o => !o.IsNullOrEmpty()).ToArray();
                    payments =AutofacBootstapper.CurrentContainer.Resolve<IConsumptionPaymentRepository>().GetListBysn(apiOrderSns);
                }
                if (!dt.Columns.Contains("SaleDate"))
                {
                    dt.Columns.Add("SaleDate", typeof(DateTime));
                    foreach (DataRow dr in dt.Rows)
                        dr["SaleDate"] = DateTime.Now;
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var dr = dt.Rows[i];
                    var ApiOrderSNCur = dr.GetValue("ApiOrderSN"); var ChangeCur = dr.GetValue("Change");
                    var ApiCode_11Cur = dr.GetValue("ApiCode_11"); var ApiCode_12Cur = dr.GetValue("ApiCode_12");
                    var ApiCode_20Cur = dr.GetValue("ApiCode_20"); var ApiCode_21Cur = dr.GetValue("ApiCode_21");
                    var ApiCode_19Cur = dr.GetValue("ApiCode_19"); var PreferentialPriceCur = dr.GetValue("PreferentialPrice");
                    var WipeZeroCur = dr.GetValue("WipeZero"); var TotalAmountCur = dr.GetValue("TotalAmount");
                    var TypeCur = dr.GetValue("Type");
                    var apiTitleCur = "";
                    if (!(ApiCode_11Cur is DBNull))
                        apiTitleCur += "现金,";
                    if (!(ApiCode_12Cur is DBNull))
                        apiTitleCur += "银联,";
                    if (!(ApiCode_20Cur is DBNull))
                        apiTitleCur += "支付宝,";
                    if (!(ApiCode_21Cur is DBNull))
                        apiTitleCur += "微信,";
                    if (!(ApiCode_19Cur is DBNull))
                        apiTitleCur += "即付宝,";
                    if (dr.GetValue("ApiOrderSN") is DBNull)
                    {
                        dr["ApiOrderSN"] = ApiOrderSN; dr.SetValue("Change", Change);
                        dr.SetValue("ApiCode_11", ApiCode_11); dr.SetValue("ApiCode_12", ApiCode_12);
                        dr.SetValue("ApiCode_20", ApiCode_20); dr.SetValue("ApiCode_21", ApiCode_21);
                        dr.SetValue("ApiCode_19", ApiCode_19); dr.SetValue("PreferentialPrice", PreferentialPrice);
                        dr.SetValue("WipeZero", WipeZero); dr.SetValue("TotalAmount", TotalAmount);
                        dr.SetValue("ApiTitle", ApiTitle); dr.SetValue("Type", Type);
                        sort++;
                    }
                    else
                    {
                        ApiOrderSN = ApiOrderSNCur; Change = ChangeCur;
                        ApiCode_11 = ApiCode_11Cur; ApiCode_12 = ApiCode_12Cur;
                        ApiCode_20 = ApiCode_20Cur; ApiCode_21 = ApiCode_21Cur;
                        ApiCode_19 = ApiCode_19Cur; PreferentialPrice = PreferentialPriceCur;
                        WipeZero = WipeZeroCur; TotalAmount = TotalAmountCur;
                        dr["ApiTitle"] = ApiTitle = apiTitleCur.TrimEnd(',');
                        sort = 1; Type = TypeCur;
                    }
                    //dr["Receive"] = (ApiCode_11Cur is DBNull ? 0 : ApiCode_11Cur.ToType<decimal>()) +
                    //    (ApiCode_12Cur is DBNull ? 0 : ApiCode_12Cur.ToType<decimal>()) +
                    //    (ApiCode_20Cur is DBNull ? 0 : ApiCode_20Cur.ToType<decimal>()) +
                    //    (ApiCode_21Cur is DBNull ? 0 : ApiCode_21Cur.ToType<decimal>()) +
                    //    (ApiCode_19Cur is DBNull ? 0 : ApiCode_19Cur.ToType<decimal>()) -
                    //    (ChangeCur is DBNull ? 0 : ChangeCur.ToType<decimal>());

                    dr["StoreId"] = System.Web.HttpContext.Current.Request["storeId"];
                    dr["InInventory"] = System.Web.HttpContext.Current.Request["InInventory"];
                    dr["Sort"] = sort;
                }

                var products = AutofacBootstapper.CurrentContainer.Resolve<IProductRepository>().FindProductByBars(barcodes);
                var removeDrs = new List<DataRow>();
                var apisns = dt.AsEnumerable().GroupBy(o => o["ApiOrderSN"].ToString()).Select(o => o.Key).ToList();
                count = apisns.Count;
                rowno = obj.MinRow;
                for (int i = 0; i < apisns.Count; i++)
                {
                    var apiSn = apisns[i];
                    var drs = dt.Select("ApiOrderSN='" + apiSn + "'");
                    if (payments.Any(o => o.ApiOrderSN == apiSn))
                    {
                        errLs.Add("行号[" + rowno + "]&nbsp;流水号[" + apiSn + "]已存在!");
                        removeDrs.AddRange(drs);
                        rowno += drs.Length;
                        continue;
                    }
                    var receive = drs.Max(o => o.GetValue("Receive").ToType<decimal>());
                    int j = 0;
                    decimal totalAmount = 0;
                    bool haszs = false;
                    foreach (DataRow dr in drs)
                    {
                        var text = dr["Barcode"].ToString();
                        if (text.IsNullOrEmpty())
                        {
                            errLs.Add("行号[" + (rowno + j) + "]条码不存在!");
                            removeDrs.AddRange(drs);
                            break;
                        }
                        var pro = products.FirstOrDefault(o => o.Barcode == text || ("," + o.Barcodes + ",").Contains("," + text + ","));
                        if (pro==null)
                        {
                            errLs.Add("行号[" + (rowno + j) + "]&nbsp;流水号[" + apiSn + "]&nbsp;条码[" + text + "]档案不存在!");
                            removeDrs.AddRange(drs);
                            break;
                        }
                        dr["ProductCode"] = pro.ProductCode;
                        dr["BuyPrice"] = pro.BuyPrice;
                        dr["ValuationType"] = pro.ValuationType;
                        text = dr.GetValue("Type").ToString();
                        dr.SetValue("Type", text == "换货" ? "1" : text == "退货" ? "2" : text == "退单" ? "3" : "0");
                        text = dr.GetValue("CreateUID").ToString();
                        if (!text.IsNullOrEmpty() && users.Any(o => o.FullName == text || o.UserCode == text))
                        {
                            var user = users.FirstOrDefault(o => o.FullName == text || o.UserCode == text);
                            dr["CreateUID2"] = user == null ? "" : user.UID;
                            if (user == null) dr["CreateUID"] = "";
                        }
                        else
                        {
                            errLs.Add("行号[" + (rowno + j) + "]&nbsp;流水号[" + apiSn + "]&nbsp;收银员" + (text.IsNullOrEmpty() ? "为空" : "[" + text + "]档案不存在!"));
                            removeDrs.AddRange(drs);
                            break;
                        }
                        text = dr.GetValue("Salesman").ToString();
                        if (!text.IsNullOrEmpty() && users.Any(o => o.FullName == text || o.UserCode == text))
                        {
                            var user = users.FirstOrDefault(o => o.FullName == text || o.UserCode == text);
                            dr["Salesman2"] = user == null ? "" : user.UID;
                            if (user == null) dr["Salesman"] = "";
                        }
                        else
                        {
                            errLs.Add("行号[" + (rowno + j) + "]&nbsp;流水号[" + apiSn + "]&nbsp;导购员" + (text.IsNullOrEmpty() ? "为空" : "[" + text + "]档案不存在!"));
                            removeDrs.AddRange(drs);
                            break;
                        }
                        text = dr.GetValue("SalesClassifyId").ToString();
                        dr["SalesClassifyId2"] = text == "赠送" ? "49" : "47";
                        if (text == "赠送")
                        {
                            dr.SetValue("SubTotal", 0);
                            dr.SetValue("ActualPrice", 0);
                            if (receive == 0)
                                dr.SetValue("TotalAmount", 0);
                            haszs = true;
                        }
                        totalAmount += dr.GetValue("ActualPrice").ToType<decimal>() * dr.GetValue("PurchaseNumber").ToType<decimal>();
                        j++;
                    }
                    if (receive > 0 && haszs)
                    {
                        drs.Each(dr =>
                        {
                            dr.SetValue("TotalAmount", totalAmount);//重算应付金额
                        });
                    }
                    rowno += drs.Length;
                }
                foreach (var dr in removeDrs)
                {
                    try { dt.Rows.Remove(dr); }
                    catch { }
                }
                _cacheService.SetObject(CacheKey,dt);
            }
            catch (Exception ex)
            {
                var user= SysUserService.CurrentUser;
                LoggerFactory.CreateWithSave<SysLog>(new SysLog(user.CompanyId, user.UserID).WriteError(ex, LogModule.统计报表));
                errLs.Add("导入出现异常!");
            }
            return CommonService.GenerateImportHtml(errLs, count);
        }
        public OperateResult SureImport()
        {
            try
            {
                var dt = _cacheService.GetObject<DataTable>(CacheKey);
                if (dt == null || dt.Rows.Count <= 0) return OperateResult.Fail("预览已过期，请重新选择导入！");
                var saleOrders = new List<SaleOrders>();
                var salelist = dt.AsEnumerable().GroupBy(o => new { ApiOrderSN = o["ApiOrderSN"].ToString() }).ToList();
                var dates = dt.AsEnumerable().GroupBy(o => o.GetValue("SaleDate").ToType<DateTime>().ToString("yyyy-MM-dd")).Select(o => o.Key).ToArray();
                var saleSvc = AutofacBootstapper.CurrentContainer.Resolve<ISaleOrderRepository>();
                var dicts = saleSvc.GetMaxNumByDate(dates);
                var companyId = SysUserService.CurrentUser.CompanyId;
                foreach (var sl in salelist)
                {
                    var sn = sl.Key.ApiOrderSN;
                    var datetime = sl.Max(o => Convert.ToDateTime(o["SaleDate"]));
                    var pre = "00" + datetime.ToString("yyyyMMdd");
                    if (dicts.ContainsKey(pre))
                        dicts[pre] += 1;
                    else
                        dicts[pre] = 1;
                    var sale = new SaleOrders();
                    sale.CustomOrderSn = pre + dicts[pre].ToString("0000");
                    sale.MachineSN = "00";
                    sale.CompanyId = companyId;
                    sale.CreateDT = datetime;
                    sale.PaySN = Guid.NewGuid().ToString("n");
                    sale.CreateUID = sl.Max(o => o.GetValue("CreateUID2").ToString());
                    sale.Salesman = sl.Max(o => o.GetValue("Salesman2").ToString());
                    sale.StoreId = sl.Max(o => o.GetValue("StoreId").ToString());
                    sale.Receive = sl.Max(o => o.GetValue("Receive").ToType<decimal>());
                    sale.TotalAmount = sl.Max(o => o.GetValue("TotalAmount").ToType<decimal>());
                    sale.Type = sl.Max(o => o.GetValue("Type").ToType<short>());
                    sale.OrderDiscount = sl.Max(o => o.GetValue("OrderDiscount").ToType<decimal>());
                    if (sl.Max(o => o.GetValue("InInventory").ToType<short>()) == 0)
                    {
                        sale.InInventory = 1;
                        sale.IsProcess = true;
                    }
                    sale.SaleDetails = new List<SaleDetail>();
                    sale.ConsumptionPayments = new List<ConsumptionPayment>();
                    sale.WipeZeros = new List<WipeZero>();
                    var ApiCode_11 = sl.Max(o => o.GetValue("ApiCode_11").ToString());
                    var ApiCode_12 = sl.Max(o => o.GetValue("ApiCode_12").ToString());
                    var ApiCode_20 = sl.Max(o => o.GetValue("ApiCode_20").ToString());
                    var ApiCode_21 = sl.Max(o => o.GetValue("ApiCode_21").ToString());
                    var ApiCode_19 = sl.Max(o => o.GetValue("ApiCode_19").ToString());
                    var ApiCode_15 = sl.Max(o => o.GetValue("ApiCode_15").ToString());
                    var WipeZero = sl.Max(o => o.GetValue("WipeZero").ToString());
                    if (!(ApiCode_11.IsNullOrEmpty() || ApiCode_11 == "0"))
                    {
                        var payment = new ConsumptionPayment()
                        {
                            ApiOrderSN = sn,
                            CompanyId = sale.CompanyId,
                            PaySN = sale.PaySN,
                            State = 1
                        };
                        payment.ApiCode = 11;
                        payment.Change = sl.Max(o => o.GetValue("Change").ToType<decimal>());
                        payment.Received = ApiCode_11.ToType<decimal>();
                        payment.Amount = payment.Received - payment.Change;
                        sale.ConsumptionPayments.Add(payment);
                    }
                    if (!(ApiCode_12.IsNullOrEmpty() || ApiCode_12 == "0"))
                    {
                        var payment = new ConsumptionPayment()
                        {
                            Amount = ApiCode_12.ToType<decimal>(),
                            ApiOrderSN = sn,
                            CompanyId = sale.CompanyId,
                            PaySN = sale.PaySN,
                            State = 1
                        };
                        payment.ApiCode = 12;
                        payment.Received = payment.Amount;
                        sale.ConsumptionPayments.Add(payment);
                    }
                    if (!(ApiCode_20.IsNullOrEmpty() || ApiCode_20 == "0"))
                    {
                        var payment = new ConsumptionPayment()
                        {
                            Amount = ApiCode_20.ToType<decimal>(),
                            ApiOrderSN = sn,
                            CompanyId = sale.CompanyId,
                            PaySN = sale.PaySN,
                            State = 1
                        };
                        payment.ApiCode = 20;
                        payment.Received = payment.Amount;
                        sale.ConsumptionPayments.Add(payment);
                    }
                    if (!(ApiCode_21.IsNullOrEmpty() || ApiCode_21 == "0"))
                    {
                        var payment = new ConsumptionPayment()
                        {
                            Amount = ApiCode_21.ToType<decimal>(),
                            ApiOrderSN = sn,
                            CompanyId = sale.CompanyId,
                            PaySN = sale.PaySN,
                            State = 1
                        };
                        payment.ApiCode = 21;
                        payment.Received = payment.Amount;
                        sale.ConsumptionPayments.Add(payment);
                    }
                    if (!(ApiCode_19.IsNullOrEmpty() || ApiCode_19 == "0"))
                    {
                        var payment = new ConsumptionPayment()
                        {
                            Amount = ApiCode_19.ToType<decimal>(),
                            ApiOrderSN = sn,
                            CompanyId = sale.CompanyId,
                            PaySN = sale.PaySN,
                            State = 1
                        };
                        payment.ApiCode = 19;
                        payment.Received = payment.Amount;
                        sale.ConsumptionPayments.Add(payment);
                    }
                    if (!(ApiCode_15.IsNullOrEmpty() || ApiCode_15 == "0"))
                    {
                        var payment = new ConsumptionPayment()
                        {
                            Amount = ApiCode_15.ToType<decimal>(),
                            ApiOrderSN = sn,
                            CompanyId = sale.CompanyId,
                            PaySN = sale.PaySN,
                            State = 1
                        };
                        payment.ApiCode = 15;
                        payment.Received = payment.Amount;
                        sale.ConsumptionPayments.Add(payment);
                    }
                    if (sale.ConsumptionPayments.Count <= 0)
                    {
                        var payment = new ConsumptionPayment()
                        {
                            ApiOrderSN = sn,
                            CompanyId = sale.CompanyId,
                            PaySN = sale.PaySN,
                            State = 1
                        };
                        payment.ApiCode = 11;
                        payment.Received = 0;
                        payment.Amount = 0;
                        payment.Change = 0;
                        sale.ConsumptionPayments.Add(payment);
                    }
                    if (!(WipeZero.IsNullOrEmpty()))
                    {
                        sale.WipeZeros.Add(new WipeZero()
                        {
                            CompanyId = sale.CompanyId,
                            PaySN = sale.PaySN,
                            Number = WipeZero.ToType<decimal>()
                        });
                    }
                    foreach (DataRow dr in dt.Select("ApiOrderSN='" + sn + "'"))
                    {
                        var detail = new SaleDetail()
                        {
                            ActualPrice = dr.GetValue("ActualPrice").ToType<decimal>(),
                            Barcode = dr.GetValue("Barcode").ToString(),
                            BuyPrice = dr.GetValue("BuyPrice").ToType<decimal>(),
                            CompanyId = sale.CompanyId,
                            PaySN = sale.PaySN,
                            ProductCode = dr.GetValue("ProductCode").ToString(),
                            PurchaseNumber = dr.GetValue("PurchaseNumber").ToType<decimal>(),
                            SalesClassifyId = dr.GetValue("SalesClassifyId2").ToType<int>(),
                            ScanBarcode = dr.GetValue("Barcode").ToString(),
                            SysPrice = dr.GetValue("SysPrice").ToType<decimal>(),
                            Title = dr.GetValue("Title").ToString(),
                        };
                        sale.ProductCount += dr["ValuationType"].ToString() == "2" ? 1 : detail.PurchaseNumber;
                        var subt = dr.GetValue("SubTotal");
                        detail.Total = subt is DBNull ? detail.PurchaseNumber * detail.ActualPrice : subt.ToType<decimal>();
                        detail.AveragePrice = detail.Total / detail.PurchaseNumber;
                        sale.SaleDetails.Add(detail);
                    }
                    sale.ApiCode = string.Join(",", sale.ConsumptionPayments.Select(o => o.ApiCode));
                    //var total = sale.SaleDetails.Sum(o => o.Total);
                    //var rate =total==0?0: sale.Receive / total;
                    //sale.SaleDetails.Each(o => o.AveragePrice = o.ActualPrice * rate);
                    saleOrders.Add(sale);
                }
                OperateResult op = saleSvc.AddOrUpdate(saleOrders.ToArray());
                if (op.Successed)
                {
                    _cacheService.RemoveObject<DataTable>(CacheKey);
                    var user = SysUserService.CurrentUser;
                    LoggerFactory.CreateWithSave<SysLog>(new SysLog(user.CompanyId, user.UserID).Analysis("销售数据导入",null,LogType.其他, LogModule.其他));
                    //var stores = string.Join(",", saleOrders.Select(o => o.StoreId).Distinct());
                    //Pharos.Infrastructure.Data.Redis.RedisManager.Publish("SyncDatabase", new Pharos.ObjectModels.DTOs.DatabaseChanged() { CompanyId = Sys.SysCommonRules.CompanyId, StoreId = stores, Target = "SalePackage" });
                }
                return op;
            }
            catch (Exception ex)
            {
                var user = SysUserService.CurrentUser;
                LoggerFactory.CreateWithSave<SysLog>(new SysLog(user.CompanyId, user.UserID).WriteError(ex,LogModule.其他));
                return OperateResult.Fail();
            }
        }
        public void ClearImport()
        {
            _cacheService.RemoveObject<DataTable>(CacheKey);
        }
        
        public DataTable GetDataFromCache
        {
            get
            {
                 return _cacheService.GetObject<DataTable>(CacheKey)??new DataTable();
            }
        }
        static string CacheKey { get { return SysUserService.CurrentUser.CompanyId+"_"+ SysUserService.CurrentUser.UserID; } }
    }
}
