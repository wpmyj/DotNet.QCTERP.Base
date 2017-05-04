﻿using Qct.Infrastructure.Data.EntityInterface;
using System;

namespace Qct.Objects.Entities
{
    public partial class SysStoreUserInfo : IntegerIdEntity
    {
        /// <summary>
        /// 记录ID
        /// [主键：√]
        /// [长度：10]
        /// [不允许为空]
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 公司ID
        /// </summary>
        public int CompanyId { get; set; }


        /// <summary>
        /// 用户ID
        /// [长度：40]
        /// [不允许为空]
        /// </summary>
        public string UID { get; set; }

        /// <summary>
        /// 姓名
        /// [长度：50]
        /// [不允许为空]
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 员工编号
        /// [长度：10]
        /// [不允许为空]
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 员工编号
        /// [长度：10]
        /// [不允许为空]
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 登录密钥
        /// [长度：50]
        /// [不允许为空]
        /// </summary>
        public string LoginPwd { get; set; }

        /// <summary>
        /// 性别（0:女、1:男）
        /// [长度：1]
        /// [默认值：((1))]
        /// </summary>
        public bool Sex { get; set; }

        /// <summary>
        /// 本店角色（1:店长、2:营业员、3:收银员、4:数据维护），格式：门店ID,角色ID|门店ID,角色ID
        /// [长度：2000]
        /// [不允许为空]
        /// </summary>
        public string OperateAuth { get; set; }

        /// <summary>
        /// 状态（1:正常、2:锁定、3:注销）
        /// [长度：5]
        /// [不允许为空]
        /// </summary>
        public short Status { get; set; }

        /// <summary>
        /// 最近登录时间
        /// [长度：23，小数位数：3]
        /// [不允许为空]
        /// [默认值：(getdate())]
        /// </summary>
        public DateTime LoginDT { get; set; }

        /// <summary>
        /// 创建时间
        /// [长度：23，小数位数：3]
        /// [不允许为空]
        /// [默认值：(getdate())]
        /// </summary>
        public DateTime CreateDT { get; set; }
        /// <summary>
        /// 数据版本
        /// </summary>
        public byte[] SyncItemVersion { get; set; }
        /// <summary>
        /// 同步ID
        /// </summary>
        public Guid SyncItemId { get; set; }


    }
}
