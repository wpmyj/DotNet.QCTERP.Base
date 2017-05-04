﻿namespace Qct.ERP.Retailing
{
    /// <summary>
    /// 操作按钮信息
    /// </summary>
    public class OpBtnInfo
    {
        public string AddText { get; set; }
        public string EditText { get; set; }
        public string DelText { get; set; }

        public bool HideDel { get; set; }
        public bool HideAdd { get; set; }
        public bool HideEdit { get; set; }
        public bool HideSearch { get; set; }
        public int SearchHeight { get; set; }
        public bool FirstLoadData { get; set; }
        /// <summary>
        /// 操作基本按钮
        /// </summary>
        /// <param name="addText">新增按钮文本</param>
        /// <param name="editText"></param>
        /// <param name="delText"></param>
        /// <param name="hideDel"></param>
        /// <param name="hideAdd"></param>
        /// <param name="hideEdit"></param>
        /// <param name="searchHeight">查询栏高度</param>
        /// <param name="firstLoadData">打开界面加载数据</param>
        public OpBtnInfo(string addText = "新增", string editText = "修改", string delText = "删除", bool hideDel = false, bool hideAdd = false, bool hideEdit = false, bool hideSearch = false, int searchHeight = 43, bool firstLoadData=true)
        {
            AddText = addText;
            EditText = editText;
            DelText = delText;
            HideDel = hideDel;
            HideAdd = hideAdd;
            HideEdit = hideEdit;
            HideSearch = hideSearch;
            SearchHeight = searchHeight;
            FirstLoadData = firstLoadData;
        }
    }
}