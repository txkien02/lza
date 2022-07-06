using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xNet;

namespace RegAccLzd
{
    public class Viootp
    {
        public int oderid;
        public string keyapi;
        public Viootp(string apikey)
        {
            this.keyapi = apikey;
        }
        public String GetSDT(string type)
        {
            HttpRequest http = new HttpRequest();
            string phone = null;
            try
            {
                String html = http.Get("https://api.viotp.com/request/getv2?token=" + keyapi + $"&serviceId=2&network={type}").ToString();
                try
                {

                    var id = JsonConvert.DeserializeObject<IDVioOTP>(html);
                    if (id.status_code == 200)
                    {
                        phone = id.data.phone_number;
                        oderid = id.data.request_id;
                    }

                }
                catch
                {


                }
            }
            catch 
            {
                phone = null;

            }


            return phone;

        }
        public String OTP(int oderid)
        {
            HttpRequest http = new HttpRequest();
            string otp = null;
            String html = http.Get($"https://api.viotp.com/session/getv2?requestId={oderid}&token={this.keyapi}").ToString();
            try
            {
                var c = JsonConvert.DeserializeObject<OTPVioOTP>(html);
                if (c.data.Code != null)
                {
                    otp = (string)c.data.Code;
                }
                else
                {

                }
            }
            catch
            {


            }


            return otp;
        }
        public void ThueLaiSim(string phone)
        {
            phone = "0" + phone;
            HttpRequest http = new HttpRequest();
            string html = http.Get($"https://api.viotp.com/request/getv2?token={this.keyapi}&serviceId=2&number={phone}").ToString();
            var res = JsonConvert.DeserializeObject<IDVioOTP>(html);
            if (res.status_code == 200)
            {

                oderid = res.data.request_id;
            }

        }
    }

    public class IDVioOTP
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public DataPhone data { get; set; }
    }

    public class DataPhone
    {
        public string phone_number { get; set; }
        public string re_phone_number { get; set; }
        public string countryISO { get; set; }
        public string countryCode { get; set; }
        public int request_id { get; set; }
        public int balance { get; set; }
    }

    public class OTPVioOTP
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public DataOTP data { get; set; }
    }

    public class DataOTP
    {
        public int ID { get; set; }
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public int Price { get; set; }
        public object SmsContent { get; set; }
        public int Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsSound { get; set; }
        public object Code { get; set; }
        public string PhoneOriginal { get; set; }
        public string Phone { get; set; }
        public string CountryISO { get; set; }
        public string CountryCode { get; set; }
    }

}
