﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using Qct.Infrastructure.Helpers;
using Qct.Infrastructure.Extensions;

namespace Qct.ERP.Retailing
{
    public static class ControlExtends
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="list"></param>
        /// <param name="name">提交表单名称</param>
        /// <param name="cols">null-不限制</param>
        /// <returns></returns>
        public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, IList<SelectListItem> list, string name, int? cols, object htmlAttributes, object checkValue)
        {
            string table = @"<table cellpadding='0' {1} cellspacing='3'>{0}</table>";
            if (list == null || list.Count <= 0) return MvcHtmlString.Empty; 
            if (!cols.HasValue) cols = 100;
            //var obj = htmlHelper.ViewData.ModelMetadata.Properties.Where(o => o.PropertyName == name).FirstOrDefault();

            var htmls = (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            string attrHtml = "", id = name, val = "";
            foreach (var attr in htmls)
            {
                if (string.Equals(attr.Key, "id", StringComparison.CurrentCultureIgnoreCase))
                    id = Convert.ToString(attr.Value);
                else if (string.Equals(attr.Key, "checkvalue", StringComparison.CurrentCultureIgnoreCase))
                    val = Convert.ToString(attr.Value);
                else
                    attrHtml += attr.Key + "=\"" + attr.Value + "\" ";
            }
            if (checkValue.IsNullOrEmpty())
            {
                checkValue =val.IsNullOrEmpty()? htmlHelper.GetModelValue(name):val;
            }
            if (!checkValue.IsNullOrEmpty())
            {
                var stateValue = checkValue.ToString();
                foreach (var item in list)
                {
                    item.Selected = false;
                    if (stateValue == item.Text || stateValue == item.Value)
                        item.Selected = true;
                }
            }
            int i = 0, num = 0;
            string row = "";
            if (htmlHelper.ViewContext.HttpContext.Request["isdetail"] == "1")
            {
                var select = list.FirstOrDefault(o => o.Selected);
                if (select != null)
                    row += "<td style='padding:0px 3px;' name='"+name+"'>" + (select.Text??select.Value) + "</td>";
            }
            if(row=="")
            {
                foreach (var item in list)
                {
                    num++;
                    var text = item.Text ?? item.Value;
                    row += "<td style='padding:0px 3px;'>" +
                        "<input type='radio' id='" + name + "_" + num + "' name='" + name + "' value='" + item.Value + "' " + (item.Selected ? "checked='checked'" : "") + " style='vertical-align:middle'/>" +
                        "<label for='" + name + "_" + num + "'  style='vertical-align:middle;padding-left:3px;'>" + text + "</label></td>";
                    if (i == cols)
                    {
                        row += "</tr><tr>";
                        i = 0; continue;
                    }
                    i++;
                }
            }
            row = "<tr>" + row + "</tr>";
            table = string.Format(table, row, (string.IsNullOrEmpty(attrHtml) ? "" : attrHtml));
            return MvcHtmlString.Create(table);
        }
        public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, IList<SelectListItem> list, string name, object htmlAttributes)
        {
            return RadioButtonList(htmlHelper, list, name, null, htmlAttributes, null);
        }
        public static MvcHtmlString RadioButtonList(this HtmlHelper htmlHelper, IList<SelectListItem> list, string name)
        {
            return RadioButtonList(htmlHelper, list, name, null);
        }
        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IList<SelectListItem> list)
        {
            return RadioButtonListFor(htmlHelper, expression, list, null);
        }
        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IList<SelectListItem> list, object htmlAttributes)
        {
            return RadioButtonListFor(htmlHelper, expression, list, htmlAttributes, null);
        }
        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IList<SelectListItem> list, object htmlAttributes, int? cols)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            var value = metadata.Model;
            return RadioButtonList(htmlHelper, list, ExpressionHelper.GetExpressionText(expression), cols, htmlAttributes, value);
        }
        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, IList<SelectListItem> list, string name, int? cols,object htmlAttributes,object value)
        {
            string table = @"<table cellpadding='0' {1} cellspacing='3'>{0}</table>";
            if (list == null || list.Count <= 0) return MvcHtmlString.Empty;
            if (!cols.HasValue) cols = 100;


            var htmls = (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            string attrHtml = "";
            foreach (var attr in htmls)
            {
                attrHtml += attr.Key + "=\"" + attr.Value + "\" ";
            }

            if(value.IsNullOrEmpty())
                value = htmlHelper.GetModelValue(name);

            if (!value.IsNullOrEmpty())
            {
                var stateValue = value.ToString();
                var values = stateValue.Split(',');
                foreach (var item in list)
                {
                    item.Selected = false;
                    if (values.Where(o => o == item.Text || o == item.Value).FirstOrDefault() != null)
                        item.Selected = true;
                    if (values.Contains("-1"))
                        item.Selected = true;
                }
            }
            int i =0, num = 0;
            string row = "";
            if (htmlHelper.ViewContext.HttpContext.Request["isdetail"] == "1")
            {
                foreach(var select in list.Where(o => o.Selected && !o.Value.IsNullOrEmpty()))
                    row += "<td style='padding:0px 3px;'>" + (select.Text??select.Value) + "</td>";
            }
            if (row == "")
            {
                foreach (var item in list)
                {
                    num++; 
                    var text = item.Text ?? item.Value;
                    row += "<td style='padding:0px 3px;'>" +
                        "<input type='checkbox' id='" + name + "_" + num + "' name='" + name + "' value='" + item.Value + "' " + (item.Selected ? "checked='checked'" : "") + " style='vertical-align:middle'/>" +
                        "<label for='" + name + "_" + num + "' style='vertical-align:middle;padding-left:3px;'>" + text + "</label></td>";
                    if (i == cols)
                    {
                        row += "</tr><tr>";
                        i = 0; continue;
                    }
                    i++; 
                }
            }
            row = "<tr>" + row + "</tr>";
            table = string.Format(table, row, (string.IsNullOrEmpty(attrHtml) ? "" : attrHtml));
            return MvcHtmlString.Create(table);
        }
        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, IList<SelectListItem> list, string name, object htmlAttributes)
        {
            return CheckBoxList(htmlHelper, list, name, null, htmlAttributes, null);
        }
        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, IList<SelectListItem> list, string name)
        {
            return CheckBoxList(htmlHelper, list, name, null);
        }
        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IList<SelectListItem> list)
        {
            return CheckBoxListFor(htmlHelper, expression, list, null);
        }
        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IList<SelectListItem> list, object htmlAttributes)
        {
            return CheckBoxListFor(htmlHelper, expression, list, htmlAttributes, null);
        }
        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IList<SelectListItem> list, object htmlAttributes, int? cols)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            var value = metadata.Model;
            return CheckBoxList(htmlHelper, list, ExpressionHelper.GetExpressionText(expression), cols, htmlAttributes, value);
        }
        public static MvcHtmlString TimeBoxList(this HtmlHelper htmlHelper, IList<string> list, string name,object htmlAttributes=null)
        {
            return TimeBoxList(htmlHelper, list, name, null, htmlAttributes);
        }
        public static MvcHtmlString TimeBoxList(this HtmlHelper htmlHelper, IList<string> list, string name, int? cols, object htmlAttributes)
        {
            string table = @"<table cellpadding='0' cellspacing='3' {1}>{0}</table>";
            if (list == null || list.Count <= 0) return MvcHtmlString.Empty;
            if (!cols.HasValue) cols = 100;


            var htmls = (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            string attrHtml = "";
            foreach (var attr in htmls)
            {
                attrHtml += attr.Key + "=\"" + attr.Value + "\" ";
            }
            int i = 0, num = 1,j=0;
            string row = "";
            if (htmlHelper.ViewContext.HttpContext.Request["isdetail"] == "1")
            {
                foreach (var val in list)
                {
                    if (val.IsNullOrEmpty()) continue;
                    row += "<td style=''>"+val+"</td>";
                    if (j == 1)
                    {
                        row += "<td style='width:10px'>&nbsp;</td>";
                        j = 0;
                    }
                    else
                    {
                        row += "<td style='line-height:28px;'>-</td>";
                        j++;
                    }
                    if (i == cols && num != list.Count)
                    {
                        row += "</tr><tr>";
                        i = 0;
                    }
                    i++;
                    num++;
                }
            }
            if (row == "")
            {
                i = 0; num = 1; j = 0;
                foreach (var val in list)
                {
                    row += "<td><input type='text' id='" + name + "_" + num + "' name='" + name + "' value='" + val + "' class='easyui-timespinner' style='width:80px;' /></td>";
                    if (j == 1)
                    {
                        row += "<td style='width:10px'>&nbsp;</td>";
                        j = 0;
                    }
                    else
                    {
                        row += "<td style='padding-left:5px;padding-right:5px;line-height:28px;'>-</td>";
                        j++;
                    }
                    if (i == cols && num != list.Count)
                    {
                        row += "</tr><tr>";
                        i = 0;
                    }
                    i++;
                    num++;
                }
            }
            row = "<tr>" + row + "</tr>";
            table = string.Format(table, row,attrHtml);
            return MvcHtmlString.Create(table);
        }
        public static MvcHtmlString DisplayTextFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,string format)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            var value = metadata.Model;
            return DisplayText(htmlHelper, ExpressionHelper.GetExpressionText(expression), format);
        }
        public static MvcHtmlString DisplayText(this HtmlHelper htmlHelper, string name, string format)
        {
            var obj= htmlHelper.ViewData.Eval(name);
            var val = htmlHelper.FormatValue(obj, format);
            return MvcHtmlString.Create(val);
        }
        static object GetModelValue(this HtmlHelper htmlHelper, string name)
        {
            ModelState state;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out state) && (state.Value != null))
            {
                return state.Value.RawValue;
            }
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var val= htmlHelper.ViewData.Eval(name);
            return val;
        }
    }
}