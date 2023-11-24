using System.Net.Mail;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EmailSender
{
    public static string SenderEmail { get; set; }
    public static string SenderPassword { get; set; }

    public static string SmptpHost { get; set; } = "smtp.mail.yahoo.com";

    public static void Send(MailMessage mailMessage)
    {
        try
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Port = 587;
            smtp.Host = SmptpHost;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(SenderEmail, SenderPassword);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.SendMailAsync(mailMessage).ContinueWith(task =>
            {
                if (task.Exception != null)
                { Debug.LogError(task.Exception); return; }
                Debug.Log("Email Has Sent!");
            });
        }
        catch (System.Exception e)
        { Debug.Log(e); }
    }
}
