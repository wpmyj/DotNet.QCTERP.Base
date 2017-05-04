﻿using Qct.Infrastructure.Data.EntityInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qct.Domain.Objects
{
    public partial class Members : IntegerIdEntity
    {
        public int Id { get; set; }
        public string MemberId { get; set; }
        /// <summary>
        /// 门店ID
        /// [长度：3]
        /// [不允许为空]
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// 会员编号
        /// [长度：100]
        /// [不允许为空]
        /// </summary>
        public string MemberNo { get; set; }


        /// <summary>
        /// 会员姓名
        /// [长度：20]
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 性别（ -1: 未知、 0:女、 1: 男）
        /// </summary>
        public short Sex { get { return sex; } set { sex = value; } }
        short sex = -1;
        /// <summary>
        /// 微信号
        /// </summary>
        public string Weixin { get; set; }
        /// <summary>
        /// 支付宝
        /// </summary>
        public string Zhifubao { get; set; }
        /// <summary>
        /// 手机号（全局唯一）
        /// [长度：11]
        /// </summary>
        public string MobilePhone { get; set; }


        /// <summary>
        /// Email（全局唯一）
        /// [长度：100]
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// QQ
        /// [长度：100]
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 生日（yyyy-MM-dd）
        /// [长度：10]
        /// </summary>
        public string Birthday { get; set; }


        /// <summary>
        /// 会员等级
        /// [长度：10]
        /// [默认值：((0))]
        /// </summary>
        public string MemberLevelId { get; set; }

        /// <summary>
        /// 会员分组
        /// [长度：10]
        /// [默认值：((0))]
        /// </summary>
        public string MemberGroupId { get; set; }

        /// <summary>
        /// 所在省份ID
        /// [长度：10]
        /// </summary>
        public int CurrentProvinceId { get; set; }

        /// <summary>
        /// 所在城市ID
        /// [长度：10]
        /// </summary>
        public int CurrentCityId { get; set; }

        /// <summary>
        /// 所在区县ID
        /// [长度：10]
        /// </summary>
        public int CurrentCountyId { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// IDNumber
        /// </summary>
        public string IDNumber { get; set; }

        /// <summary>
        /// 内部人员
        /// [长度：40]
        /// </summary>
        public bool Insider { get; set; }
        /// <summary>
        /// 默认优惠方式
        /// </summary>
        public int DefaultPreferentialId { get; set; }
        /// <summary>
        /// 默认积分方式
        /// </summary>
        public int DefaultIntegrationId { get; set; }
        /// <summary>
        /// 创建时间
        /// [长度：23，小数位数：3]
        /// [不允许为空]
        /// </summary>
        public DateTime CreateDT { get; set; }

        /// <summary>
        /// 创建人UID
        /// [长度：40]
        /// [不允许为空]
        /// </summary>
        public string CreateUID { get; set; }

        /// <summary>
        /// 状态（ 0: 禁用、 1:可用、 2: 无效）
        /// </summary>
        public short Status { get; set; }
        /// <summary>
        /// 推荐人
        /// </summary>
        public string ReferrerUID { get; set; }

        /// <summary>
        /// 可用积分
        /// [长度：10]
        /// [默认值：((0))]
        /// </summary>
        public decimal UsableIntegral { get; set; }


        public byte[] SyncItemVersion { get; set; }
        public Guid SyncItemId { get; set; }
    }
}
