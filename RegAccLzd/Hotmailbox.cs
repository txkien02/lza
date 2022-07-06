using EAGetMail;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using xNet;

namespace RegAccLzd
{
	public class MailHelper
	{
		public string username;
		public string password;
		public MailServer oServer;
		public MailClient oClient;
		public MailHelper(string username, string password)
		{
			this.username = username;
			this.password = password;
		}
		public void DeleteAllMail()
		{
			oServer = new MailServer("outlook.office365.com", username, password, ServerProtocol.Imap4);
			oServer.SSLConnection = true;
			oServer.Port = 993;
			oClient = new MailClient("TryIt");
			oClient.Connect(oServer);
			MailInfo[] info = oClient.GetMailInfos();
			foreach (MailInfo item in info)
			{
				oClient.Delete(item);
			}
		}
		public string getOTPHotmail()
		{
			oServer = new MailServer("outlook.office365.com", username, password, ServerProtocol.Imap4);
			oServer.SSLConnection = true;
			oServer.Port = 993;
			oClient = new MailClient("TryIt");
			oClient.Connect(oServer);
			string otp = null;
			MailInfo[] infos = oClient.GetMailInfos();
			if (infos.Length != 0)
			{
				foreach (MailInfo info in infos)
				{
					Mail oMail = oClient.GetMail(info);
					string fromMail = oMail.From.Name;
					if (fromMail == "Lazada Vietnam")
					{
						string patternM = @"\d{6}";
						otp = Regex.Match(oMail.TextBody, patternM).Value;
					}

				}
			}
			else
			{
				otp = null;
			}
			return otp;
		}

	}
	public class Hotmailbox
	{
		public string keyapi;
		public HttpRequest http;
		public Hotmailbox(string keyapi)
		{
			this.keyapi = keyapi;
			http = new HttpRequest();
		}
		public Emails[] Buymail(int quantity)
		{
			string html = http.Get($"https://api.hotmailbox.me/mail/buy?apikey={keyapi}&mailcode=OUTLOOK.TRUSTED&quantity={quantity}").ToString();
			if (html.IndexOf("Bạn đang") != -1)
			{
				Random r = new Random();
				Thread.Sleep(r.Next(1000, 3000));
				html = http.Get($"https://api.hotmailbox.me/mail/buy?apikey={keyapi}&mailcode=OUTLOOK.TRUSTED&quantity={quantity}").ToString();
			}
			EmailInfo info = JsonConvert.DeserializeObject<EmailInfo>(html);
			Emails[] listEmail = info.Data.Emails;
			return listEmail;
		}
	}

	public class EmailInfo
	{
		public int Code { get; set; }
		public string Message { get; set; }
		public Data Data { get; set; }
	}

	public class Data
	{
		public string TransId { get; set; }
		public string Product { get; set; }
		public int Quantity { get; set; }
		public float UnitPrice { get; set; }
		public float UnitPriceUsd { get; set; }
		public float TotalAmount { get; set; }
		public float TotalAmountUsd { get; set; }
		public Emails[] Emails { get; set; }
	}

	public class Emails
	{
		public string Email { get; set; }
		public string Password { get; set; }
	}
}
