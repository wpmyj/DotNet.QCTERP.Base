// --------------------------------------------------
// Copyright (C) 2015 版权所有
// 创 建 人：蔡少发
// 创建时间：2015-05-22
// 描述信息：用于管理本系统的所有采购订货单基本信息
// --------------------------------------------------

using System;

namespace Qct.Objects.Entities
{
	/// <summary>
	/// 采购明细单表
	/// </summary>
	[Serializable]
	public partial class IndentOrderList
	{
		/// <summary>
		/// 明细 ID
		/// [主键：√]
		/// [长度：10]
		/// [不允许为空]
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 采购单ID（订货单 ID）
		/// [长度：40]
		/// [不允许为空]
		/// </summary>
		public string IndentOrderId { get; set; }

		/// <summary>
		/// 商品条码
		/// [长度：30]
		/// [不允许为空]
		/// </summary>
		public string ProductCode { get; set; }

		/// <summary>
		/// 订货数量		
		/// [不允许为空]
		/// [默认值：((0))]
		/// </summary>
		public decimal IndentNum { get; set; }

		/// <summary>
		/// 进价
		/// [长度：19，小数位数：4]
		/// [不允许为空]
		/// [默认值：((0))]
		/// </summary>
		public decimal Price { get; set; }
        /// <summary>
        /// 系统售价
        /// </summary>
        public decimal? SysPrice { get; set; }
		/// <summary>
		/// 小计
		/// [长度：19，小数位数：4]
		/// [默认值：((0))]
		/// </summary>
        public decimal Subtotal { get { return IndentNum * Price; } set { subtotal = value; } }
        decimal subtotal = 0;
		/// <summary>
		/// 备注
		/// [长度：200]
		/// </summary>
		public string Memo { get; set; }

		/// <summary>
		/// 配送数量
		/// [默认值：((0))]
		/// </summary>
        public decimal DeliveryNum { get; set; }

		/// <summary>
		/// 收货数量
		/// [默认值：((0))]
		/// </summary>
        public decimal AcceptNum { get; set; }

		/// <summary>
        /// 状态（1: 未配送、 2:配送中、 3:已中止、4:已配送、 5:已收货）
		/// [长度：5]
		/// [不允许为空]
		/// [默认值：((2))]
		/// </summary>
        public short State { get { return state; } set { state = value; } }
        private short state = 2;
        /// <summary>
        /// 产品性质(1-赠品)
        /// </summary>
        public short Nature { get; set; }
        /// <summary>
        /// 隶属赠品货号(赠品使用)
        /// </summary>
        public string UnderProductCode { get; set; }
	}
}
