﻿/*----------------------------------------------------------------
 * 功能描述：URL 常规处理
 * 创 建 人：蔡少发
 * 创建时间：2015-05-11
//----------------------------------------------------------------*/

using System.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qct.Infrastructure.Helpers
{
    /// <summary>
    /// URL 常规处理
    /// </summary>
    public class Url
    {
        #region 获取当前请求的原始URL

        /// <summary>
        /// 获取当前请求的原始URL
        /// <![CDATA[
        /// 如：http://www.Caisf.test/admin/index.aspx?id=123
        /// 返回：/admin/index.aspx?id=123
        /// ]]>
        /// </summary>
        public static string RawUrl
        {
            get
            {
                return HttpContext.Current.Request.RawUrl;
            }
        }

        #endregion

        #region 获取HTTP连接头（即 Https或Http）

        /// <summary>
        /// 获取HTTP连接头（即 Https或Http）
        /// </summary>
        public static string Http
        {
            get
            {
                return HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://";
            }
        }

        #endregion

        #region 获取当前请求的主域名称

        /// <summary>
        /// 获取当前请求的主域名称
        /// </summary>
        public static string CurDomain
        {
            get { return HttpContext.Current.Request.Url.Host; }
        }

        #endregion

        #region 获取当前请求主域中的端口号

        /// <summary>
        /// 获取当前请求主域中的端口号
        /// </summary>
        public static int CurPort
        {
            get { return HttpContext.Current.Request.Url.Port; }
        }

        #endregion

        #region 根据URL提取域名

        /// <summary>
        /// 根据URL提取域名
        /// </summary>
        /// <param name="url">URL</param>
        public static string GetDomain(string url)
        {
            if (string.IsNullOrEmpty(url)) { return string.Empty; }

            url = url.Replace(@"\", @"/");

            if (url.StartsWith(@"http://"))
            {
                url = url.Substring(7);
            }
            else if (url.StartsWith(@"https://"))
            {
                url = url.Substring(8);
            }

            if (url.IndexOf(@"/") > 0)
            {
                return url.Substring(0, url.IndexOf(@"/"));
            }

            return url;
        }

        #endregion

        #region 根据URL提取路径

        /// <summary>
        /// 根据URL提取路径
        /// </summary>
        /// <param name="url">URL</param>
        public static string GetPath(string url)
        {
            if (string.IsNullOrEmpty(url)) { return string.Empty; }

            url = url.Replace(@"\", @"/");

            string remove = GetFileNameAndParam(url);

            return url.Substring(0, url.Length - remove.Length);
        }

        #endregion

        #region 根据URL提取文件名称及参数

        /// <summary>
        /// 根据URL提取文件全称及参数
        /// </summary>
        /// <param name="url">URL</param>
        public static string GetFileNameAndParam(string url)
        {
            if (string.IsNullOrEmpty(url)) { return string.Empty; }
            url = url.Replace(@"\", @"/");

            if (url.IndexOf(@"/") < 0) { return url; }

            int len = url.Length;
            int last = (url.IndexOf("?") > 0) ? url.IndexOf("?") : url.Length;

            string tmp = url.Substring(0, last);
            int end = tmp.LastIndexOf(@"/");

            tmp = tmp.Substring(end + 1);

            return (tmp + url.Substring(last));
        }

        /// <summary>
        /// 根据URL提取文件全称（含扩展名）
        /// </summary>
        /// <param name="url">URL</param>
        public static string GetFileFullName(string url)
        {
            if (string.IsNullOrEmpty(url)) { return string.Empty; }

            string file = GetFileNameAndParam(url);

            int end = file.IndexOf(@"?", 0);

            return (end > 0) ? file.Substring(0, end) : file;
        }

        /// <summary>
        /// 根据URL提取文件名称（不含扩展名）
        /// </summary>
        /// <param name="url">URL</param>
        public static string GetFileName(string url)
        {
            if (string.IsNullOrEmpty(url)) { return string.Empty; }

            string file = GetFileFullName(url);

            int end = file.LastIndexOf(@".");

            return (end > 0) ? file.Substring(0, end) : file;
        }

        #endregion

        #region 根据URL得到文件的格式名

        /// <summary>
        /// 根据URL得到文件的格式名
        /// </summary>
        /// <param name="fileName">文件全名</param>
        /// <returns>返回格式名</returns>
        public static string GetFormat(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return string.Empty; }

            return (fileName.LastIndexOf(".") < 0) ? string.Empty : fileName.Substring(fileName.LastIndexOf("."));
        }

        #endregion

        #region 生成URL参数

        /// <summary>
        /// 生成URL参数
        /// </summary>
        /// <param name="parms"><![CDATA[ Dictionary<key, value>参数集合 ]]></param>
        /// <returns>返回生成URL格式参数</returns>
        public static string ToUrlParms(Dictionary<string, string> parms)
        {
            if (parms == null || parms.Count < 1) { return string.Empty; }

            string url = string.Empty;

            foreach (KeyValuePair<string, string> kv in parms)
            {
                url += string.Format("{0}={1}&", kv.Key, kv.Value);
            }

            return url.Substring(0, url.Length - 1);
        }

        /// <summary>
        /// 生成URL参数
        /// </summary>
        /// <param name="parms"><![CDATA[ Dictionary<key, value>参数集合 ]]></param>
        /// <param name="isEncrypt">是否需要编码处理</param>
        /// <returns>返回生成URL格式参数</returns>
        public static string ToUrlParms(Dictionary<string, string> parms, bool isEncrypt)
        {
            if (parms == null || parms.Count < 1) { return string.Empty; }

            string url = string.Empty;

            foreach (KeyValuePair<string, string> kv in parms)
            {
                url += string.Format("{0}={1}&", kv.Key, isEncrypt ? HttpUtility.UrlEncode(kv.Value) : kv.Value);
            }

            return url.Substring(0, url.Length - 1);
        }

        /// <summary>
        /// 生成URL参数
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="parms"><![CDATA[ Dictionary<key, value>参数集合 ]]></param>
        /// <returns>返回生成URL格式参数</returns>
        public static string ToUrlParms(string url, Dictionary<string, string> parms)
        {
            if (string.IsNullOrEmpty(url) || url.Length < 5) { return url; }
            if (parms == null || parms.Count < 1) { return url; }

            string tmp = string.Empty;

            foreach (KeyValuePair<string, string> kv in parms)
            {
                tmp += string.Format("{0}={1}&", kv.Key, kv.Value);
            }

            return (url + "?" + tmp.Substring(0, tmp.Length - 1));
        }

        /// <summary>
        /// 生成URL参数
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="parms"><![CDATA[ Dictionary<key, value>参数集合 ]]></param>
        /// <param name="isEncrypt">是否需要编码处理</param>
        /// <returns>返回生成URL格式参数</returns>
        public static string ToUrlParms(string url, Dictionary<string, string> parms, bool isEncrypt)
        {
            if (string.IsNullOrEmpty(url) || url.Length < 5) { return url; }
            if (parms == null || parms.Count < 1) { return url; }

            string tmp = string.Empty;

            foreach (KeyValuePair<string, string> kv in parms)
            {
                tmp += string.Format("{0}={1}&", kv.Key, isEncrypt ? HttpUtility.UrlEncode(kv.Value) : kv.Value);
            }

            return (url + "?" + tmp.Substring(0, tmp.Length - 1));
        }

        #endregion

        #region 获取URL中Key对应的值

        /// <summary>
        /// 获取URL中Key对应的值
        /// </summary>
        /// <param name="url">URL字符串</param>
        /// <param name="key">键</param>
        /// <returns>返回键对应的值</returns>
        public static string GetUrlParms(string url, string key)
        {
            url = (!url.StartsWith("&") && !url.StartsWith("?")) ? ("?" + url) : url;

            MatchCollection mc = Regex.Matches(url, @"(\\?|&)" + key + "=([^&]*)", RegexOptions.IgnoreCase);

            if (mc.Count > 0)
            {
                return (mc[0].Value.IndexOf("=") > 0) ? mc[0].Value.Substring(mc[0].Value.IndexOf("=") + 1) : mc[0].Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取URL字符串中的所有参数
        /// </summary>
        /// <param name="url">URL字符串</param>
        /// <returns>返回参数字典</returns>
        public static Dictionary<string, string> GetUrlParms(string url)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (url.IndexOf("?") > 0 || url.IndexOf("&") > 0)
            {
                if (url.StartsWith("?") || url.StartsWith("&"))
                {
                    url = url.Substring(1);
                }

                string[] str = url.Split('&');

                foreach (string s in str)
                {
                    string[] kv = s.Split('=');
                    dic.Add(kv[0], kv[1]);
                }

                return dic;
            }

            return dic;
        }

        #endregion

        #region 获取虚拟路径
        /// <summary>
        /// 获取虚拟根路径
        /// </summary>
        public static string GetApplicationPath
        {
            get { return HttpContext.Current.Request.ApplicationPath == "/" ? "/" : HttpContext.Current.Request.ApplicationPath + "/"; }
        }
        #endregion
    }
}
