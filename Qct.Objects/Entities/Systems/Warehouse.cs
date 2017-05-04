// --------------------------------------------------
// Copyright (C) 2015 版权所有
// 创 建 人：蔡少发
// 创建时间：2015-05-22
// 描述信息：用于管理本系统的所有仓库基本信息（仓库等同于门店）
// --------------------------------------------------

using Qct.Infrastructure.Data.EntityInterface;
using System;

namespace Qct.Objects.Entities
{
	/// <summary>
	/// 仓库信息
	/// </summary>
	public class Warehouse:CompanyEntity
	{
		/// <summary>
		/// 记录ID
		/// [主键：√]
		/// [长度：10]
		/// [不允许为空]
		/// </summary>
		public int Id { get; set; }
        public int CompanyId { get; set; }
		/// <summary>
		/// 仓库（门店）编号（全局唯一）
		/// [长度：3]
		/// [不允许为空]
		/// </summary>
		public string StoreId { get; set; }

		/// <summary>
		/// 仓库名称
		/// [长度：20]
		/// [不允许为空]
		/// </summary>
		public string Title { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

		/// <summary>
		/// 存储品类ID（多个ID以,号间隔）
		/// [长度：500]
		/// [不允许为空]
		/// </summary>
		public string CategorySN { get; set; }

		/// <summary>
		/// 状态（0:停用、1:可用）
		/// [长度：5]
		/// [不允许为空]
		/// </summary>
		public short State { get; set; }

		/// <summary>
		/// 创建时间
		/// [长度：23，小数位数：3]
		/// [不允许为空]
		/// [默认值：(getdate())]
		/// </summary>
		public DateTime CreateDT { get; set; }

		/// <summary>
		/// 创建人UID
		/// [长度：40]
		/// [不允许为空]
		/// </summary>
		public string CreateUID { get; set; }
        /// <summary>
        /// 后台管理开放
        /// </summary>
        public short AdminState { get; set; }

	}
}
