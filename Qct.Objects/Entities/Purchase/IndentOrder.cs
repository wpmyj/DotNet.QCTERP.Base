﻿using Qct.Objects.ValueObjects;
using System;

namespace Qct.Objects.Entities
{
    public partial class IndentOrder
    {
        /// <summary>
        /// 记录 ID
        /// [主键：√]
        /// [长度：40]
        /// [不允许为空]
        /// </summary>
        public int Id { get; set; }
        public int CompanyId { get; set; }
        /// <summary>
        /// 采购单ID(订货单ID)
        /// [长度：40]
        /// [不允许为空] 
        /// </summary>
        public string IndentOrderId { get; set; }
        /// <summary>
        /// 订货门店 ID
        /// [长度：3]
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// 供货单位 ID
        /// [长度：40]
        /// [不允许为空]
        /// </summary>
        public string SupplierID { get; set; }
        /// <summary>
        /// 收货人ID
        /// [长度：40]
        /// </summary>
        public string RecipientsUID { get; set; }

        /// <summary>
        /// 订单总额
        /// [长度：19，小数位数：4]
        /// [不允许为空]
        /// </summary>
        public decimal OrderTotal { get; set; }

        /// <summary>
        /// 收货地址
        /// [长度：100]
        /// </summary>
        public string ShippingAddress { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 交货日期
        /// [长度：10]
        /// [不允许为空]
        /// </summary>
        public string DeliveryDate { get; set; }
        /// <summary>
        /// 配送开始日期
        /// [长度：10]
        /// </summary>
        public string PeiSongStartDate { get; set; }
        /// <summary>
        /// 配送完成日期
        /// [长度：10]
        /// </summary>
        public string PeiSongEndDate { get; set; }
        /// <summary>
        /// 创建时间
        /// [长度：23，小数位数：3]
        /// [不允许为空]
        /// </summary>
        public DateTime CreateDT { get; set; }
        /// <summary>
        /// 已收货日期
        /// </summary>
        public DateTime? ReceivedDT { get; set; }
        /// <summary>
        /// 创建人 UID
        /// [长度：40]
        /// [不允许为空]
        /// </summary>
        public string CreateUID { get; set; }
        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? ApproveDT { get; set; }
        /// <summary>
        /// 状态（ -1:未提交、0:未审核、 1:已审核（ 未配送）、 2:配送中、 3:已中止、4:已配送、 5:已收货）
        /// [长度：5]
        /// [不允许为空]
        /// [默认值：((0))]
        /// </summary>
        public short State { get; set; }
    }
}
