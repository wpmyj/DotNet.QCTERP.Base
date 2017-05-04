﻿using System;

namespace Qct.Objects.ValueObjects
{
    public class ActivityNoticeModel
    {
        public ActivityNoticeModel() { }

        public ActivityNoticeModel(string id, string title, string timeStr, string stateStr, DateTime createDT, int type) 
        {
            this.Id = id;
            this.Title = title;
            this.TimeStr = timeStr;
            this.StateStr = stateStr;
            this.CreateDT = createDT;
            this.Type = type;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string TimeStr { get; set; }
        public string StateStr { get; set; }
        public DateTime CreateDT { get; set; }
        /// <summary>
        /// 类型（1：活动，2：公告）
        /// </summary>
        public int Type { get; set; }
    }
}