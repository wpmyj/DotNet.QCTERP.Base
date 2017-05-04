﻿/*----------------------------------------------------------------
// 功能描述：导出Excel文件
 * 创 建 人：蔡少发
// 创建时间：2015-04-23
//----------------------------------------------------------------*/

using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Web;

namespace Qct.Infrastructure.Export
{
    /// <summary>
    /// 导出Excel业务逻辑
    /// </summary>
    public class ExportExcel
    {
        /// <summary>
        /// 导出示例
        /// </summary>
        /// <returns>true:成功，false:失败</returns>
        public void DaoChuTest()
        {
            DataTable dt = new DataTable(); //要导出的数据表

            string[] fields = { "SN", "Branch", "Department", "FullName", "Position", "Tel", "MobilePhone", "Email", "QQ", "WeiXin", "Money" };
            string[] names = { "序号", "分公司", "部门", "姓名", "职务", "电话/分机", "手机", "E-Mail", "QQ", "微信号", "钱包" };

            //合并第1列（分公司）的相同值，及第n列的相同值，并以第2列（部门）为最小合并参照单位（最小参照单位须放在第一位）
            int[] merger = { 2, 1, 5 };

            //合计指定列的值
            int[] totalColumn = { 10 };

            //直接输出下载（若采用返回url下载地址方式，须去掉该行）
            this.IsBufferOutput = true;

            this.ToExcel("员工通讯录", dt, fields, names, merger);
        }


        #region 文档属性、单元格赋值、输出Excel

        /// <summary>
        /// 过滤HTML代码（默认true）
        /// </summary>
        public bool IsFilterHTML
        {
            get { return _isFilterHtml; }
            set { _isFilterHtml = value; }
        }
        private bool _isFilterHtml = true;

        /// <summary>
        /// 是否直接缓冲输出文件下载（默认false）
        /// </summary>
        public bool IsBufferOutput { get; set; }
        /// <summary>
        /// 自定义标题内容
        /// </summary>
        public string HeaderText { get; set; }
        /// <summary>
        /// 导出Excel文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="dt">源数据</param>
        /// <param name="merger">合并列相同的值，用于上下行单元格重复合并（多个列间用半角逗号间隔）</param>
        /// <returns>返回生成后的URL下载地址</returns>
        public string ToExcel(string fileName, DataTable dt, int[] merger = null, int[] totalColumn = null)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                int count = dt.Columns.Count;
                string[] fields = new string[count];
                string[] names = new string[count];
                for (int i = 0; i < count; i++)
                {
                    fields[i] = dt.Columns[i].ColumnName;
                    names[i] = fields[i];
                }

                return this.ToExcel(fileName, dt, fields, names, merger, totalColumn);
            }
            return string.Empty;
        }

        /// <summary>
        /// 导出Excel文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="dt">源数据</param>
        /// <param name="fields">字段名（与 names 列名顺序一致，含个数）</param>
        /// <param name="names">列名（与 fields 字段名顺序一致，含个数）</param>
        /// <param name="merger">合并列相同的值，用于上下行单元格重复合并（多个列间用半角逗号间隔）</param>
        /// <param name="totalColumn">合计列的值，仅对数字、货币类型有效（在最后一行自动合计显示）</param>
        /// <param name="columnWidth">自定义列宽，格式：<key, value> 参数：key表示列索引号，value表示列宽</param>
        /// <returns>返回生成后的URL下载地址</returns>
        public string ToExcel(string fileName, DataTable dt, string[] fields, string[] names, int[] merger, int[] totalColumn = null, Dictionary<int, int> columnWidth = null, bool hasHeaderText = true, bool formatFileName = true)
        {
            if (dt != null && dt.Rows.Count > 0 && fields.Length == names.Length)
            {
                //创建Excel文件
                string sheetName = fileName;
                string headerText = HeaderText ?? fileName;
                if (formatFileName)
                {
                    fileName = string.Format("{0}_{1}.xls", fileName, DateTime.Now.ToString("yyyy-MM-dd"));
                }
                else
                {
                    fileName = string.Format("{0}.xls", fileName);
                }
                HSSFWorkbook book = new HSSFWorkbook();
                book.DocumentSummaryInformation = DSI;
                book.SummaryInformation = SummaryInfo(fileName);

                //设置表
                ISheet iSheet;
                //创建数据格式
                IDataFormat iDataFormat = book.CreateDataFormat();
                //设置日期格式
                ICellStyle dataStyle = book.CreateCellStyle();

                int namesCount = names.Length;
                int fieldsCount = fields.Length;

                #region //自适应列宽

                int colCount = names.Length;
                int[] colWidth = new int[colCount];
                for (int i = 0; i < namesCount; i++)
                {
                    if (columnWidth != null && columnWidth.ContainsKey(i))
                    {
                        colWidth[i] = (columnWidth[i] < 5) ? 5 : columnWidth[i] + 2;
                    }
                    else
                    {
                        colWidth[i] = Encoding.GetEncoding(936).GetBytes(names[i]).Length;
                        colWidth[i] = (colWidth[i] < 5) ? 5 : colWidth[i] + 2;
                    }
                }

                DataRowCollection drc = dt.Rows;
                int rowCount = drc.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        int tmp = Encoding.GetEncoding(936).GetBytes(drc[i][fields[j]].ToString()).Length;
                        if (tmp > colWidth[j])
                        {
                            colWidth[j] = tmp;
                        }
                    }
                }

                if (rowCount > this.MaxRows)
                {
                    iSheet = book.CreateSheet(string.Format("{0}(1)", sheetName));
                }
                else
                {
                    iSheet = book.CreateSheet(sheetName);
                }

                #endregion

                IFont titleFont = book.CreateFont();
                IFont headFont = book.CreateFont();
                IFont textFont = book.CreateFont();
                ICellStyle css = book.CreateCellStyle();
                //
                int cur = 0;
                int tab = 2;
                decimal dec = 0;
                Dictionary<int, decimal> heji = new Dictionary<int, decimal>();

                for (int index = 0; index < rowCount; index++)
                {
                    #region 设置表头、列名、样式

                    if ((index == 0 || index % this.MaxRows == 0))
                    {
                        if (cur != 0)
                        {
                            iSheet = book.CreateSheet(string.Format("{0}({1})", sheetName, tab));
                            tab++;
                        }
                        cur = 1;
                        //表头
                        IRow header = iSheet.CreateRow(0);
                        header.HeightInPoints = 25;
                        //样式
                        ICellStyle cellStyle = book.CreateCellStyle();
                        cellStyle.Alignment = HorizontalAlignment.Center;
                        cellStyle.VerticalAlignment = VerticalAlignment.Center;

                        cellStyle.BorderTop = BorderStyle.Thin;
                        cellStyle.BorderBottom = BorderStyle.Thin;
                        cellStyle.BorderLeft = BorderStyle.Thin;
                        cellStyle.BorderRight = BorderStyle.Thin;

                        titleFont.FontHeightInPoints = 14;
                        cellStyle.SetFont(titleFont);
                        if (hasHeaderText)
                        {
                            if (!string.IsNullOrWhiteSpace(headerText))
                            {
                                header.CreateCell(0).SetCellValue(headerText);
                                header.GetCell(0).CellStyle = cellStyle;
                                CellRangeAddress ra = new CellRangeAddress(0, 0, 0, colCount - 1);
                                iSheet.AddMergedRegion(ra);
                                ((HSSFSheet)iSheet).SetEnclosedBorderOfRegion(ra, BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);

                                header = iSheet.CreateRow(1);//列头
                                cur = 2;
                            }
                        }
                        header.HeightInPoints = 18;
                        cellStyle = book.CreateCellStyle();
                        cellStyle.Alignment = HorizontalAlignment.Center;
                        cellStyle.VerticalAlignment = VerticalAlignment.Center;

                        cellStyle.BorderTop = BorderStyle.Thin;
                        cellStyle.BorderBottom = BorderStyle.Thin;
                        cellStyle.BorderLeft = BorderStyle.Thin;
                        cellStyle.BorderRight = BorderStyle.Thin;

                        headFont.FontHeightInPoints = 12;
                        cellStyle.SetFont(headFont);

                        for (int i = 0; i < namesCount; i++)
                        {
                            header.CreateCell(i).SetCellValue(names[i]);
                            header.GetCell(i).CellStyle = cellStyle;
                            iSheet.SetColumnWidth(i, (int)Math.Ceiling((double)((colWidth[i] + 1) * 256)));
                        }


                    }
                    #endregion

                    #region 填充内容

                    IRow rows = iSheet.CreateRow(cur);
                    rows.HeightInPoints = 18;

                    for (int i = 0; i < fieldsCount; i++)
                    {
                        ICell cell = rows.CreateCell(i);
                        dataStyle.Alignment = css.Alignment = HorizontalAlignment.Left;
                        dataStyle.VerticalAlignment = css.VerticalAlignment = VerticalAlignment.Center;
                        dataStyle.WrapText = css.WrapText = true;

                        dataStyle.BorderTop = css.BorderTop = BorderStyle.Thin;
                        dataStyle.BorderBottom = css.BorderBottom = BorderStyle.Thin;
                        dataStyle.BorderLeft = css.BorderLeft = BorderStyle.Thin;
                        dataStyle.BorderRight = css.BorderRight = BorderStyle.Thin;

                        textFont.FontHeightInPoints = 10;
                        css.SetFont(textFont);
                        dataStyle.SetFont(textFont);
                        cell.CellStyle = css;

                        if (index > 0 && IsExistMerger(merger, i) && (index % this.MaxRows != 0))
                        {
                            if (string.Compare(Convert.ToString(drc[index][fields[i]]), Convert.ToString(drc[index - 1][fields[i]])) == 0
                                && string.Compare(Convert.ToString(drc[index][fields[merger[0]]]), Convert.ToString(drc[index - 1][fields[merger[0]]])) == 0)
                            {
                                iSheet.AddMergedRegion(new CellRangeAddress(cur - 1, cur, i, i));
                            }
                        }

                        string val = Convert.ToString(drc[index][fields[i]]);
                        val = this.IsFilterHTML ? FilterHtml.Remove(val) : val;
                        dec = 0;
                        this.SetCellValue(cell, iDataFormat, dataStyle, drc[index][fields[i]].GetType().ToString(), val, out dec);
                        iSheet.SetColumnWidth(i, (int)Math.Ceiling((double)((colWidth[i] + 1) * 256)));

                        if (IsExistMerger(totalColumn, i))
                        {
                            if (!heji.ContainsKey(i))
                            {
                                heji.Add(i, dec);
                            }
                            else
                            {
                                if (!cell.IsMergedCell)
                                {
                                    dec += heji[i];
                                    heji.Remove(i);
                                    heji.Add(i, dec);
                                }
                            }
                        }
                    }

                    #endregion

                    cur++;
                }

                #region 对指定列进行合计

                int beginColumn = 0;
                if (totalColumn != null && totalColumn.Length > 0)
                {
                    IRow rows = iSheet.CreateRow(cur);
                    rows.HeightInPoints = 18;

                    for (int i = 0; i < fieldsCount; i++)
                    {
                        ICell cell = rows.CreateCell(i);
                        css.Alignment = HorizontalAlignment.Left;
                        css.VerticalAlignment = VerticalAlignment.Center;
                        css.WrapText = true;

                        css.BorderTop = BorderStyle.Thin;
                        css.BorderBottom = BorderStyle.Thin;
                        css.BorderLeft = BorderStyle.Thin;
                        css.BorderRight = BorderStyle.Thin;

                        textFont.FontHeightInPoints = 10;
                        css.SetFont(textFont);
                        cell.CellStyle = css;

                        dec = 0;
                        if (IsExistMerger(totalColumn, i))
                        {
                            this.SetCellValue(cell, iDataFormat, dataStyle, heji[i].GetType().ToString(), heji[i].ToString(), out dec);
                            if (beginColumn == 0)
                            {
                                beginColumn = i;
                            }
                        }
                        else
                        {
                            this.SetCellValue(cell, iDataFormat, dataStyle, "System.String", "", out dec);
                        }

                        iSheet.SetColumnWidth(i, (int)Math.Ceiling((double)((colWidth[i] + 2) * 256)));
                    }
                    if (beginColumn > 0)//合计单独计算
                    {
                        var cell = rows.GetCell(0);

                        css.Alignment = HorizontalAlignment.Right;
                        css.SetFont(textFont);
                        cell.CellStyle = css;

                        iSheet.AddMergedRegion(new CellRangeAddress(cur, cur, 0, beginColumn - 1));
                        this.SetCellValue(cell, iDataFormat, dataStyle, "System.String", "合计：", out dec);
                        beginColumn = -1;
                    }
                }

                #endregion

                //输出文件
                return this.OutputFile(book, fileName);
            }
            return string.Empty;
        }

        /// <summary>
        /// 若单页超出5000行（最多可设65535行)，则另外新建表
        /// </summary>
        private int MaxRows { get { return 5000; } }

        private bool IsExistMerger(int[] merger, int curCol)
        {
            if (merger != null)
            {
                foreach (int c in merger)
                {
                    if (c == curCol)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 文档属性
        /// </summary>
        private DocumentSummaryInformation DSI
        {
            get
            {
                DocumentSummaryInformation si = PropertySetFactory.CreateDocumentSummaryInformation();
                si.Company = "CAISHAOFA";
                return si;
            }
        }

        /// <summary>
        /// 属性摘要信息
        /// </summary>
        /// <param name="fileName">文件名称</param>
        private SummaryInformation SummaryInfo(string fileName)
        {
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();

            si.Author = "CAISHAOFA";
            si.LastAuthor = si.Author;
            si.Comments = si.Author;

            si.ApplicationName = fileName;
            si.Title = fileName;
            si.Subject = fileName;
            si.CreateDateTime = DateTime.Now;

            return si;
        }

        /// <summary>
        /// 填充单元格数据
        /// </summary>
        /// <param name="cell">单元格对象</param>
        /// <param name="df">数据格式</param>
        /// <param name="style">样式</param>
        /// <param name="type">数据类型</param>
        /// <param name="value">数据值</param>
        /// <param name="val">输出数字或货币值（非数字或非货币类型的统一为0）</param>
        private void SetCellValue(ICell cell, IDataFormat df, ICellStyle style, string type, string value, out decimal val)
        {
            val = 0;

            switch (type)
            {
                case "System.String":
                    cell.SetCellValue(value);
                    break;
                case "System.DateTime":
                    DateTime dt;
                    if (DateTime.TryParse(value, out dt))
                    {
                        style.DataFormat = dt.Hour > 0 ? df.GetFormat("yyyy-MM-dd HH:mm:ss") : df.GetFormat("yyyy-MM-dd");
                        cell.CellStyle = style;
                        cell.SetCellValue(dt);
                    }
                    else
                    {
                        cell.SetCellValue("");
                    }
                    break;
                case "System.Boolen":
                    bool bl = false;
                    if (bool.TryParse(value, out bl))
                    {
                        cell.SetCellValue(bl);
                    }
                    else
                    {
                        cell.SetCellValue("");
                    }
                    break;
                case "System.Byte":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                    int i = 0;
                    if (int.TryParse(value, out i))
                    {
                        cell.SetCellValue(i);
                        val = i;
                    }
                    else
                    {
                        cell.SetCellValue(0);
                    }
                    break;
                case "System.Decimal":
                case "System.Double":
                    double dbl = 0;
                    if (double.TryParse(value, out dbl))
                    {
                        cell.SetCellValue(dbl);
                        val = Convert.ToDecimal(value);
                    }
                    else
                    {
                        cell.SetCellValue(0);
                    }
                    break;
                case "System.DBNull":
                default:
                    cell.SetCellValue("");
                    break;
            }
        }

        /// <summary>
        /// 输出并保存文件
        /// </summary>
        /// <param name="book">HSSFWorkbook 实例</param>
        /// <param name="fileName">文件全称含扩展名</param>
        /// <returns>返回生成后的URL下载地址</returns>
        private string OutputFile(HSSFWorkbook book, string fileName)
        {
            String savePath = HttpContext.Current.Server.MapPath(@"/Temp/Excel/");
            string newFileName = Path.Combine(savePath, fileName);

            if (!Directory.Exists(savePath))
            {
                FileIOPermission ioPermission = new FileIOPermission(FileIOPermissionAccess.AllAccess, savePath);
                ioPermission.Demand();
                Directory.CreateDirectory(savePath);
            }
            using (FileStream fs = new FileStream(newFileName, FileMode.Create, FileAccess.ReadWrite))
            {
                book.Write(fs);
                fs.Dispose();
                fs.Close();
            }

            if (this.IsBufferOutput)
            {
                ResponseFile(newFileName, fileName);
            }

            return newFileName;
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fullFileName">物理地址</param>
        /// <param name="fileName">文件名称</param>
        public static void ResponseFile(string fullFileName, string fileName)
        {
            FileStream stream = new FileStream(fullFileName, FileMode.Open);
            byte[] bytes = new byte[(int)stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            HttpResponse rp = HttpContext.Current.Response;
            rp.Clear();
            rp.ClearHeaders();
            rp.Buffer = true;
            rp.BufferOutput = true;
            rp.ContentType = "application/octet-stream";
            if (HttpContext.Current.Request.Browser.Browser.Equals("InternetExplorer", StringComparison.CurrentCultureIgnoreCase))
            {
                rp.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
            }
            else
            {
                rp.AppendHeader("Content-Disposition", "attachment;fileName=" + fileName);
            }
            rp.BinaryWrite(bytes);
            rp.Flush();
            rp.End();
        }
        #endregion

        #region 将Excel导出DataTable
        public DataTable ToDataTable(Stream stream, int minRow = 0, int maxRow = 0)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            //var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = null;
            try
            {
                workbook = new XSSFWorkbook(stream);//2007
            }
            catch (Exception)
            {
                try
                {
                    workbook = new HSSFWorkbook(stream);//2003
                }
                catch
                {
                    throw new Exception("读取Excel文件失败！");
                }
            }
            sheet = workbook.GetSheetAt(0);
            if (sheet != null)
            {
                if (minRow != 0) startRow = minRow - 1;//真实序号

                IRow firstRow = sheet.GetRow(startRow);
                int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                for (int i = 0; i < cellCount; ++i)
                {
                    DataColumn column = new DataColumn(i.ToString());
                    data.Columns.Add(column);
                }

                //最后一列的标号
                int rowCount = sheet.LastRowNum;
                //if (maxRow != 0 && rowCount > maxRow) rowCount = maxRow - 1;
                if (maxRow != 0) rowCount = maxRow - 1;
                for (int i = startRow; i <= rowCount; ++i)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null || row.FirstCellNum < 0) continue; //没有数据的行默认是null　　　　　　　
                    DataRow dataRow = data.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                    {
                        //同理，没有数据的单元格都默认是null
                        if (row.GetCell(j) != null)
                        {
                            if (row.GetCell(j).CellType == CellType.Formula)
                            {
                                //公式类型
                                sheet.ForceFormulaRecalculation = true;
                                HSSFFormulaEvaluator eva = new HSSFFormulaEvaluator(workbook);
                                dataRow[j] = eva.EvaluateInCell(row.GetCell(j));
                            }
                            else if (row.GetCell(j).CellType == CellType.Numeric)
                            {
                                if (HSSFDateUtil.IsCellDateFormatted(row.GetCell(j)))
                                {
                                    //日期类型
                                    dataRow[j] = row.GetCell(j).DateCellValue;
                                }
                                else
                                {
                                    //其他数字类型
                                    dataRow[j] = row.GetCell(j).NumericCellValue;
                                }
                            }
                            else if (row.GetCell(j).CellType == CellType.Blank)
                            {
                                //空数据类型
                                dataRow[j] = "";
                            }
                            else
                            {
                                dataRow[j] = row.GetCell(j).StringCellValue;
                            }
                        }
                    }
                    data.Rows.Add(dataRow);
                }
                //fs.Close();
            }
            return data;
        }
        #endregion
    }

}
