﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Qct.Infrastructure.Helpers
{
    /// <summary>
    /// 发送邮件服务
    /// </summary>
    public class MailHelper
    {
        /// <summary>
        /// smtp服务器
        /// </summary>
        public string Smtp { get; set; }
        /// <summary>
        /// 发送者(邮件地址)
        /// </summary>
        public string Sender { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string ShowName { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// smtp登陆名密码
        /// </summary>
        public string Password { get; set; }

        static object objLock = new object();
        /// <summary>
        /// 发送Email
        /// </summary>
        /// <param name="body">邮件主体</param>
        /// <param name="title">显示的标题</param>
        /// <param name="email">接收方的邮件地址列表</param>
        /// <param name="attachs">附件</param>
        public void SendMessage(string body, string title,string[] emails,params Attachment[] attachs)
        {
            lock (objLock)
            {
                var message = new MailMessage();
                foreach (var email in emails)
                {
                    message.To.Add(new MailAddress(email));
                }
                message.Subject = title;
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                message.Body = body;
                message.Priority = MailPriority.Normal;
                foreach(var attach in attachs)
                    message.Attachments.Add(attach);

                if (!string.IsNullOrEmpty(Sender))
                {
                    message.Sender = new MailAddress(Sender,ShowName);
                    message.From = message.Sender;
                }

                var client = new SmtpClient(Smtp, Port ?? 25)
                {
                    Credentials = new NetworkCredential(Sender, Password),
                    EnableSsl = false
                };
                client.Send(message);
            }
        }
    }
}
