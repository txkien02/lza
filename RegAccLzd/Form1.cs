using Auto_LDPlayer;
using EAGetMail;
using KAutoHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;
using System.Net.Mail;
using MailServer = EAGetMail.MailServer;

namespace RegAccLzd
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string GET_proxy(string key)
        {
            string proxy = null;
            while (proxy == null)
            {
                try
                {
                    HttpRequest http = new HttpRequest();
                    string html = http.Get("http://proxy.tinsoftsv.com/api/changeProxy.php?key=" + key + "&location=0").ToString();
                    var result = JsonConvert.DeserializeObject<TinsoftProxy>(html);
                    if (result.success)
                    {
                        proxy = result.proxy;
                        break;
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            rbNoiDung.Text += "Chờ " + result.next_change + '\n';
                        }));
                        Thread.Sleep(int.Parse(result.next_change) * 1000);

                    }


                }
                catch (Exception err)
                {
                    Invoke(new Action(() =>
                    {
                        rbNoiDung.Text += err.ToString();
                    }));
                }

            }
            return proxy;
        }
        Random r = new Random();
        LDPlayer ldplayer = new LDPlayer();
        Object locker = new object();
        int nextIndex = 0;
        int AccFail = 0;
        int otp2fail = 0;
        Writer AccRegXong = new Writer(@"AccReg//Acc_" + DateTime.Now.ToString("dd_MM") + ".txt");
        Writer OTP2Fail = new Writer(@"AccReg//AccFailOTP2_" + DateTime.Now.ToString("dd_MM") + ".txt");
        Writer Otp1Null = new Writer("Acc//OTP1 Null.txt");
        Writer Otp2Null = new Writer("Acc//OTP2 Null.txt");
        Writer Account = new Writer("Acc//Account.txt");
        Writer AllAccount = new Writer("Acc//All account.txt");
        Writer failedlogin = new Writer("Acc//Failed login.txt");
        Writer KXacDinh = new Writer("Acc//KhongXacDinh.txt");
        Writer SetTach = new Writer("Acc//Sai ngôn ngữ.txt");
        private void button1_Click(object sender, EventArgs e)
        {

            StreamReader readproxy = new StreamReader("broxy.txt");
            String rbKeyProxy = readproxy.ReadToEnd();
            readproxy.Close();

            string[] keyProxys = rbKeyProxy.Split(
                   new[] { "\r\n", "\r", "\n" },
                   StringSplitOptions.None
               );

            List<String> Devices = KAutoHelper.ADBHelper.GetDevices();
            int dem = 0;

            String[] Device = new String[Devices.Count];
            foreach (var item in Devices)
            {
                Device[dem] = item;
                dem++;
            }

            for (int i = 0; i < Device.Length; i++)
            {

                int iThread = i;
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    while (true)
                    {

                        try
                        {


                            RegAccLZD(Device[iThread], keyProxys[iThread]);

                        }
                        catch
                        {

                        }






                    }



                }).Start();


            }
        }

        void RegAccLZD(String deviceID, string keyProxy)
        {
            try
            {

            BackAll:
                string proxy = "";
                if (CbTinSoft.Checked)
                {
                    proxy = GET_proxy(keyProxy);
                }
                if (cbXproxy.Checked)
                {
                    proxy = GetXproxy(keyProxy);
                }
                if (CbTM.Checked)
                {
                    proxy = GetProxy_TM(keyProxy);
                }
                if (cbShopLike.Checked)
                {
                    proxy = ShopLike(keyProxy);
                }
                String[] ProxyParse = proxy.Split(':');



                Fakeip(deviceID, ProxyParse[0], ProxyParse[1]);
                AdbCommand("adb -s " + deviceID + " shell pm clear mark.via.gp");
                AdbCommand($"adb -s {deviceID} shell am start -n zone.bytesreverser.xposeddeviceidmaskerlite/zone.bytesreverser.xposeddeviceidmaskerlite.ID");
                Thread.Sleep(2000);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 54.5, 30.4);
                Thread.Sleep(500);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 54.2, 45.3);
                Thread.Sleep(500);
                AdbCommand($"adb -s {deviceID} shell am start -n com.google.android.reborn/com.google.android.reborn.MainActivity");
                Thread.Sleep(2500);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 37.1, 8.4);
                Thread.Sleep(500);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 53.6, 8.9);
                Thread.Sleep(1500);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 80.4, 93.1);
                Thread.Sleep(200);
                string nameDevice = randomNum(10);
                string buildNumber = randomNum(10);
                string userAgent = $"Mozilla/5.0(Linux; U; Android 2.2; en-gb;  LG-{nameDevice} Build/{buildNumber})   AppleWebKit/403.0 (KHTML, like Gecko) Version/4.0 Mobile Safari/403.1";
                //AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell");
                String Data = File.ReadAllText($"AppRoot\\settings.xml");
                string FileNew = Data.Replace("randomUser", userAgent);
                File.WriteAllText($"AppRoot\\SettingNew\\settings.xml", FileNew);
                AdbCommand($"adb -s {deviceID} remount");
                AdbCommand($"adb -s {deviceID} push AppRoot\\SettingNew\\settings.xml /data/data/mark.via.gp/shared_prefs/settings.xml");
                Thread.Sleep(100);
            numberBack:
                AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://member.lazada.vn/user/forget-password?");
                Chothuesimcode Chothuesimcoede1 = new Chothuesimcode();
                string SDT = Chothuesimcoede1.GetSDT(ApiKey.Text, "3,2");
                int id = Chothuesimcoede1.idress;
                CheckImg(deviceID, SlideToContinue, 500, 20);
                bool checkproxy = IshaveImg(deviceID, ProxyDie) || IshaveImg(deviceID, ProxyDie2);
                if (checkproxy)
                {
                    Chothuesimcoede1.Cancel(ApiKey.Text, id);
                    goto BackAll;

                }
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 15.1, 31.3);
                GoTiengViet(deviceID, SDT);
                int Count = 0;
                bool swipeVery = false;
                while (swipeVery == false && Count < 10)
                {
                    KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 10.6, 51.8, 93.3, 999, 800);
                    Thread.Sleep(3000);
                    swipeVery = IshaveImg(deviceID, checkPhoneNumber);
                    Count++;
                }
                Thread.Sleep(2000);
                bool checkNumber = IshaveImg(deviceID, SDTOLD);
                if (checkNumber)
                {
                    Chothuesimcoede1.Cancel(ApiKey.Text, id);
                    goto numberBack;

                }
                AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://member.lazada.vn/user/register?");
                CheckImg(deviceID, ButtonSwipe, 500, 15);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 9.7, 26.2);
                KAutoHelper.ADBHelper.InputText(deviceID, SDT);
                int Count1 = 0;
                bool swipeReg = false;
                while (swipeReg == false && Count1 < 5)
                {
                    KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 12.7, 38.0, 92.0, 999, 800);
                    Thread.Sleep(3000);
                    swipeReg = IshaveImg(deviceID, GuiLai);
                    Count1++;
                }
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.1, 36.9);
                String OTP = null;
                int CountOTP = 0;
                Thread.Sleep(5000);

                while (OTP == null && CountOTP < 8)
                {
                    OTP = Chothuesimcoede1.OTP(ApiKey.Text, id);

                    if (OTP == null)
                    {
                        Thread.Sleep(1000);
                        CountOTP++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (OTP != null)
                {
                    int id2 = Chothuesimcoede1.GetSDT_again(ApiKey.Text, SDT);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 17.2, 36.7);
                    Thread.Sleep(300);
                    KAutoHelper.ADBHelper.InputText(deviceID, OTP);
                    Thread.Sleep(300);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 47.9, 49.6);
                    Thread.Sleep(1000);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 69.6, 60.2);
                    Thread.Sleep(500);
                    CheckImg(deviceID, Continue, 500, 20);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 90.6, 26.5);
                    GoTiengViet(deviceID, randomName());
                    Thread.Sleep(300);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.7, 36.5);
                    KAutoHelper.ADBHelper.InputText(deviceID, Pass.Text);
                    Thread.Sleep(300);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 52.1, 56.5);
                    Thread.Sleep(4000);

                    AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://s.lazada.vn/s.cTWKm");
                    CheckImg(deviceID, Setting, 500, 20);
                    Thread.Sleep(500);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 93.6, 63.3);
                    Thread.Sleep(2000);


                    int Count7 = 0;
                    bool checkTiger = false;
                    while (checkTiger == false && Count7 < 30)
                    {
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 75.0, 75.0);
                        Thread.Sleep(500);
                        KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 16.3, 58.7, 89.4, 999, 800);
                        Thread.Sleep(1000);
                        checkTiger = IshaveImg(deviceID, Open);
                        Count7++;
                    }
                    Thread.Sleep(2000);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 75.0, 75.0);
                    Thread.Sleep(1000);
                    AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://s.lazada.vn/s.cTWKm");
                    CheckImg(deviceID, Setting, 500, 20);
                    Thread.Sleep(1000);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 93.3, 63.8);
                    Thread.Sleep(500);

                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 77.4, 91.2);

                    CheckImg(deviceID, AddOrder, 500, 15);
                    Thread.Sleep(500);
                    if (IshaveImg(deviceID, buyMoreH5))
                    {
                        AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://my-m.lazada.vn/member/account-info?");

                        CheckImg(deviceID, ThemEmail, 500, 20);
                        int Count2 = 0;
                        bool checkAddMail = false;
                        while (checkAddMail == false && Count2 < 5)
                        {
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 23.2, 46.5);
                            Thread.Sleep(2000);
                            checkAddMail = IshaveImg(deviceID, VeryAddSDT);
                            Count2++;
                        }
                        Thread.Sleep(1000);
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 48.2, 75.5);
                        Thread.Sleep(1500);

                        int CountGetOTP2 = 0;
                        bool GetOTP2 = false;
                        while (GetOTP2 == false && CountGetOTP2 < 5)
                        {
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 89.7, 53.3);//Click Send Code
                            String OTP2 = null;
                            int CountOTP2 = 0;
                            Thread.Sleep(5000);
                            while (OTP2 == null && CountOTP2 < 15)
                            {
                                OTP2 = Chothuesimcoede1.OTP(ApiKey.Text, id2);

                                if (OTP2 == null)
                                {
                                    Thread.Sleep(1000);
                                    CountOTP2++;

                                }
                                else
                                {
                                    GetOTP2 = true;
                                    break;
                                }

                            }
                            if (OTP2 != null)
                            {
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.4, 54.5);
                                Thread.Sleep(200);
                                KAutoHelper.ADBHelper.InputText(deviceID, OTP2);
                                Thread.Sleep(200);
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.3, 66.8);
                            }
                            else
                            {
                                AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://my-m.lazada.vn/member/account-info?");
                                Thread.Sleep(2000);
                                CheckImg(deviceID, ThemEmail, 500, 20);
                                int Count6 = 0;
                                bool checkAddMail2 = false;
                                while (checkAddMail2 == false && Count6 < 5)
                                {
                                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 23.2, 46.5);
                                    Thread.Sleep(2000);
                                    checkAddMail2 = IshaveImg(deviceID, VeryAddSDT);
                                    Count6++;
                                }
                                Thread.Sleep(2000);
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 48.2, 75.5);
                                Thread.Sleep(1000);


                            }

                            CountGetOTP2++;

                        }
                    BuyMail:
                        string TaiKhoan = "";
                        string MatKhau = "";
                        try
                        {
                            HttpRequest http = new HttpRequest();
                            //                
                            String HotMail = http.Get("https://api.hotmailbox.me/mail/buy?apikey=" + tbKeyHotmail.Text + "&mailcode=OUTLOOK.TRUSTED&quantity=1").ToString();
                            Thread.Sleep(1000);
                            var result = JsonConvert.DeserializeObject<Maxclone>(HotMail);
                            TaiKhoan = result.Data.Emails[0].Email;
                            MatKhau = result.Data.Emails[0].Password;
                        }
                        catch
                        {
                            goto BuyMail;
                        }
                        try
                        {
                            DeleteAllMail(TaiKhoan, MatKhau);
                        }
                        catch
                        {
                            goto BackAll;
                        }
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.4, 41.8);
                        KAutoHelper.ADBHelper.InputText(deviceID, TaiKhoan);
                        Thread.Sleep(200);
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 88.8, 53.6);
                        String OTPmail = null;
                        int Count3 = 0;
                        while (OTPmail == null && Count3 < 8)
                        {
                            OTPmail = getOTPHotmail(TaiKhoan, MatKhau);

                            if (OTPmail == null)
                            {
                                Thread.Sleep(1000);
                                Count3++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (OTPmail != null)
                        {
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 11.5, 54.6);
                            Thread.Sleep(200);
                            KAutoHelper.ADBHelper.InputText(deviceID, OTPmail);
                            Thread.Sleep(1000);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 48.5, 66.3);
                            Thread.Sleep(5000);
                            AccRegXong.WriteToFileLine(SDT + "|" + Pass.Text + "|" + TaiKhoan + "|" + MatKhau + "|" + NgayGio());
                            string[] fileArray = File.ReadAllLines(@"AccReg//Acc_" + DateTime.Now.ToString("dd_MM") + ".txt");
                            string AccThanhCong = fileArray.Length.ToString();
                            Invoke((new Action(() =>
                            {

                                lbsuccessful.Text = AccThanhCong;
                            })));
                        }
                        else
                        {
                            OTP2Fail.WriteToFileLine(SDT + "|" + Pass.Text + "|" + NgayGio());
                            Invoke((new Action(() =>
                            {

                                otp2fail++;
                                lbOTP2.Text = otp2fail.ToString();
                            })));
                        }
                    }
                    else
                    {

                        Invoke((new Action(() =>
                        {

                            AccFail++;
                            label3.Text = AccFail.ToString();
                        })));
                    }


                }
                else
                {
                    Chothuesimcoede1.Cancel(ApiKey.Text, id);
                }


            }
            catch
            {

            }


        }
        void RegRequestne(String deviceID, string acc, String pass, string AccHotmail, string PassHotmail, int iThread)
        {
            try
            {
                AdbCommand("adb -s " + deviceID + " shell am start -n mark.via.gp/mark.via.Shell" + " https://m.facebook.com");
                Thread.Sleep(9000);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 78.6, 72.9);
                Thread.Sleep(200);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 14.5, 30.1);
                Thread.Sleep(500);
                KAutoHelper.ADBHelper.InputText(deviceID, acc);
                Thread.Sleep(100);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.1, 38.4);
                Thread.Sleep(100);
                KAutoHelper.ADBHelper.InputText(deviceID, pass);
                Thread.Sleep(100);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 47.6, 46.8);
                Thread.Sleep(10000);

                bool loginfbokimg = IshaveImg(deviceID, LoginOK);
                if (loginfbokimg == false)
                {

                    RemoveIp(deviceID);
                    Thread.Sleep(2000);
                    AdbCommand("adb -s " + deviceID + " shell pm clear mark.via.gp");
                    failedlogin.WriteToFileLine(acc + "|" + pass + "|" + AccHotmail + "|" + PassHotmail + "|" + NgayGio());

                }
                else
                {
                    Thread.Sleep(500);
                    AdbCommand("adb -s " + deviceID + " shell am start -n mark.via.gp/mark.via.Shell" + " https://member-m.lazada.vn/user/login");
                    Thread.Sleep(5000);
                    int countss1 = 0;
                    bool loginlaz1 = false;
                    Thread.Sleep(3000);
                    while (loginlaz1 == false && countss1 < 5)
                    {
                        loginlaz1 = IshaveImg(deviceID, NutLoginLzd);
                        if (loginlaz1)
                        {
                            break;
                        }
                        loginlaz1 = IshaveImg(deviceID, LoginEn);
                        if (loginlaz1)
                        {
                            break;
                        }
                        countss1++;
                        Thread.Sleep(1000);

                    }
                    KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 49.1, 83.1, 51.8, 29.2);
                    Thread.Sleep(400);
                    Tapimg(deviceID, Facebook);
                    Thread.Sleep(100);
                    Tapimg(deviceID, Facebook);

                    int countss2 = 0;
                    bool CheckEdit = false;

                    while (CheckEdit == false && countss2 < 5)
                    {
                        CheckEdit = IshaveImg(deviceID, ChinhSua);
                        countss2++;
                        Thread.Sleep(1000);
                        countss2++;
                    }
                    if (IshaveImg(deviceID, ChinhSua))
                    {
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 40.4, 43.1);//tap chinh sửa
                        Thread.Sleep(300);
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 90.9, 43.3);
                        Thread.Sleep(300);
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 42.2, 57.9);//tiep tuc
                        Thread.Sleep(3000);
                    }

                    int countss3 = 0;
                    bool CHeckDienSau = false;
                    while (CHeckDienSau == false && countss3 < 5)
                    {
                        CHeckDienSau = IshaveImg(deviceID, DienSau);

                        Thread.Sleep(1000);
                        countss3++;
                    }
                    bool CheckTaiKhoanCuaToi = false;

                    int wait1 = 0;

                    while (CheckTaiKhoanCuaToi == false && wait1 < 5)
                    {

                        CheckTaiKhoanCuaToi = IshaveImg(deviceID, TaiKhoanCuaToi);
                        if (CheckTaiKhoanCuaToi)
                        {
                            break;
                        }
                        Tapimg(deviceID, DienSau);

                        Thread.Sleep(3000);
                        CheckTaiKhoanCuaToi = IshaveImg(deviceID, TaiKhoanCuaToi);
                        wait1++;
                    }
                    bool ca = IshaveImg(deviceID, TaiKhoanCuaToi);
                    if (ca == false)
                    {
                        RemoveIp(deviceID);
                        Thread.Sleep(2000);
                        AdbCommand("adb -s " + deviceID + " shell pm clear mark.via.gp");
                        failedlogin.WriteToFileLine(acc + "|" + pass + "|" + AccHotmail + "|" + PassHotmail + "|" + NgayGio());

                    }
                    else
                    {
                        AdbCommand("adb -s " + deviceID + " shell am start -n mark.via.gp/mark.via.Shell" + " https://my-m.lazada.vn/member/account-info?");
                        Thread.Sleep(3000);
                        bool CheckEmail = false;
                        int wait3 = 0;
                        while (CheckEmail == false && wait3 < 5)
                        {


                            Tapimg(deviceID, ThemEmail);
                            Thread.Sleep(3000);
                            CheckEmail = IshaveImg(deviceID, ThemEmailDo);

                            wait3++;
                        }
                        DeleteAllMail(AccHotmail, PassHotmail);
                        Thread.Sleep(100);

                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 18.1, 42.3);
                        Thread.Sleep(500);
                        KAutoHelper.ADBHelper.InputText(deviceID, AccHotmail);
                        Thread.Sleep(100);
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 86.1, 53.3);
                        Thread.Sleep(400);

                        String otp = null;
                        int dem = 0;
                        Thread.Sleep(5000);

                        while (otp == null && dem < 5)
                        {
                            otp = getOTPHotmail(AccHotmail, PassHotmail);

                            if (otp == null)
                            {
                                Thread.Sleep(2000);
                                dem++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (otp != null)
                        {
                            Invoke(new Action(() =>
                            {
                                rbNoiDung.Text += "Thiết bị " + deviceID + " có otp1 là: " + otp + '\n';

                            }));
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 13.6, 54.3);
                            Thread.Sleep(400);
                            KAutoHelper.ADBHelper.InputText(deviceID, otp);
                            Thread.Sleep(200);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 51.5, 66.7);
                            Thread.Sleep(3000);
                            AdbCommand("adb -s " + deviceID + " shell am start -n mark.via.gp/mark.via.Shell" + " https://member-m.lazada.vn/user/forget-password");
                            int countss = 0;
                            bool xacthucimg = false;
                            while (xacthucimg == false && countss < 5)
                            {
                                Thread.Sleep(2000);
                                countss++;
                            }
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 17.2, 29.9);
                            Thread.Sleep(200);
                            KAutoHelper.ADBHelper.InputText(deviceID, AccHotmail);
                            Thread.Sleep(1000);
                            int dem1 = 0;
                            bool KeoXacMinh = false;
                            while (KeoXacMinh == false && dem1 < 12)
                            {
                                KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 10.6, 51.8, 93.3, 300, 500);
                                Thread.Sleep(3000);
                                KeoXacMinh = IshaveImg(deviceID, XacMinhQMK);
                                if (KeoXacMinh)
                                {
                                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 56.3, 72.6);
                                    Tapimg(deviceID, XacMinhQMK);
                                }
                                dem1++;
                            }
                            Thread.Sleep(400);
                            DeleteAllMail(AccHotmail, PassHotmail);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 82.8, 57.5);
                            Thread.Sleep(200);
                            String otp1 = null;
                            int dem2 = 0;

                            Thread.Sleep(5000);
                            while (dem2 < 5 && otp1 == null)
                            {
                                otp1 = getOTPHotmail(AccHotmail, PassHotmail); ;
                                if (otp1 == null)
                                {
                                    Thread.Sleep(2000);
                                    dem2++;
                                }
                                else break;
                            }
                            if (otp1 != null)
                            {
                                Invoke(new Action(() =>
                                {
                                    rbNoiDung.Text += "Thiết bị " + deviceID + " có otp2 là: " + otp1 + '\n';
                                }));
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 16.6, 58.9);
                                Thread.Sleep(200);
                                KAutoHelper.ADBHelper.InputText(deviceID, otp1);
                                Thread.Sleep(200);
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 48.8, 74.6);
                                Thread.Sleep(2000);
                                int countss4 = 0;
                                bool Checkdatlai = false;
                                while (Checkdatlai == false && countss4 < 7)
                                {
                                    Checkdatlai = IshaveImg(deviceID, Gui);
                                    countss4++;
                                    Thread.Sleep(2000);
                                }
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 17.8, 46.2);
                                Thread.Sleep(300);
                                KAutoHelper.ADBHelper.InputText(deviceID, tbPass.Text);
                                Thread.Sleep(300);
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 13.6, 56.0);
                                Thread.Sleep(300);
                                KAutoHelper.ADBHelper.InputText(deviceID, tbPass.Text);
                                Thread.Sleep(300);
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 48.2, 68.4);
                                Account.WriteToFileLine(AccHotmail + "|" + tbPass.Text + "|" + NgayGio());
                                Thread.Sleep(100);
                                AllAccount.WriteToFileLine(acc + "|" + pass + "|" + AccHotmail + "|" + PassHotmail + "|" + NgayGio());
                                Thread.Sleep(3000);

                            }
                            else
                            {
                                Otp2Null.WriteToFileLine(acc + "|" + pass + "|" + AccHotmail + "|" + PassHotmail + "|" + NgayGio());
                            }


                        }
                        else
                        {
                            Otp1Null.WriteToFileLine(acc + "|" + pass + "|" + AccHotmail + "|" + PassHotmail + "|" + NgayGio());
                        }
                    }
                }


            }
            catch
            {
                KXacDinh.WriteToFileLine(acc + "|" + pass + "|" + AccHotmail + "|" + PassHotmail + "|" + NgayGio());
            }
        }
        Bitmap LoginOK;
        Bitmap NutLoginLzd;
        Bitmap Facebook;
        Bitmap ChinhSua;
        Bitmap DienSau;
        Bitmap TaiKhoanCuaToi;
        Bitmap LoginEn;
        Bitmap ThemEmail;
        Bitmap ThemEmailDo;

        Bitmap SlideToContinue;
        Bitmap XacMinhQMK;
        Bitmap Gui;
        Bitmap ProxyDie;
        Bitmap SDTOLD;
        Bitmap DatLaiMK;
        Bitmap loginFacebook;
        Bitmap TEN;
        Bitmap BaCham;
        Bitmap TiengAnh;
        Bitmap TiengViet;
        Bitmap Hello;
        Bitmap XinChao;
        Bitmap DungLuong;
        Bitmap LoginFB;
        Bitmap checkPhoneNumber;
        Bitmap ButtonSwipe;
        Bitmap GuiLai;
        Bitmap Continue;
        Bitmap Setting;
        Bitmap VeryAddSDT;
        Bitmap Open;
        Bitmap ProxyDie2;
        Bitmap AddOrder;
        Bitmap buyMoreH5;
        Bitmap SDTOLDPhone;
        Bitmap checkPhoneNumberPhone;
        Bitmap SlideToContinuePhone;
        Bitmap ButtonSwipePhone;
        Bitmap GuiLaiPhone;
        Bitmap ContinuePhone;
        Bitmap SettingPhone;
        Bitmap OpenPhone;
        Bitmap AddOrderPhone;
        Bitmap buyMoreH5Phone;
        Bitmap ThemEmailPhone;
        Bitmap VeryAddSDTPhone;
        private void Form1_Load(object sender, EventArgs e)
        {
            VeryAddSDTPhone = (Bitmap)Bitmap.FromFile("Img//VeryAddSDTPhone.png");
            ThemEmailPhone = (Bitmap)Bitmap.FromFile("Img//ThemEmailPhone.png");
            buyMoreH5Phone = (Bitmap)Bitmap.FromFile("Img//buyMoreH5Phone.png");
            AddOrderPhone = (Bitmap)Bitmap.FromFile("Img//AddOrderPhone.png");
            OpenPhone = (Bitmap)Bitmap.FromFile("Img//OpenPhone.png");
            SettingPhone = (Bitmap)Bitmap.FromFile("Img//SettingPhone.png");
            ContinuePhone = (Bitmap)Bitmap.FromFile("Img//ContinuePhone.png");
            GuiLaiPhone = (Bitmap)Bitmap.FromFile("Img//GuiLaiPhone.png");
            ButtonSwipePhone = (Bitmap)Bitmap.FromFile("Img//ButtonSwipePhone.png");
            checkPhoneNumberPhone = (Bitmap)Bitmap.FromFile("Img//checkPhoneNumberPhone.png");
            SDTOLDPhone = (Bitmap)Bitmap.FromFile("Img//SDTOLDPhone.png");
            SlideToContinuePhone = (Bitmap)Bitmap.FromFile("Img//SlideToContinuePhone.png");
            buyMoreH5 = (Bitmap)Bitmap.FromFile("Img//buyMoreH5.png");
            AddOrder = (Bitmap)Bitmap.FromFile("Img//AddOrder.png");
            Open = (Bitmap)Bitmap.FromFile("Img//Open.png");
            Setting = (Bitmap)Bitmap.FromFile("Img//Setting.png");
            VeryAddSDT = (Bitmap)Bitmap.FromFile("Img//VeryAddSDT.png");
            Continue = (Bitmap)Bitmap.FromFile("Img//Continue.png");
            ProxyDie2 = (Bitmap)Bitmap.FromFile("Img//ProxyDie2.png");
            GuiLai = (Bitmap)Bitmap.FromFile("Img//GuiLai.png");
            ButtonSwipe = (Bitmap)Bitmap.FromFile("Img//ButtonSwipe.png");
            SDTOLD = (Bitmap)Bitmap.FromFile("Img//SDTOLD.png");
            checkPhoneNumber = (Bitmap)Bitmap.FromFile("Img//checkPhoneNumber.png");
            ProxyDie = (Bitmap)Bitmap.FromFile("Img//ProxyDie.png");
            LoginFB = (Bitmap)Bitmap.FromFile("Img//LoginFB.png");
            DungLuong = (Bitmap)Bitmap.FromFile("Img//DungLuong.png");
            XinChao = (Bitmap)Bitmap.FromFile("Img//XinChao.png");
            Hello = (Bitmap)Bitmap.FromFile("Img//Hello.png");
            TiengViet = (Bitmap)Bitmap.FromFile("Img//TiengViet.png");
            TiengAnh = (Bitmap)Bitmap.FromFile("Img//TiengAnh.png");
            BaCham = (Bitmap)Bitmap.FromFile("Img//BaCham.png");
            TEN = (Bitmap)Bitmap.FromFile("Img//TEN.png");
            loginFacebook = (Bitmap)Bitmap.FromFile("Img//loginFacebook.png");
            tbPass.Text = "123456aa";
            XacMinhQMK = (Bitmap)Bitmap.FromFile("Img//XacMinhQMK.png");
            Gui = (Bitmap)Bitmap.FromFile("Img//Gui.png");
            DatLaiMK = (Bitmap)Bitmap.FromFile("Img//DatLaiMK.png");
            SlideToContinue = (Bitmap)Bitmap.FromFile("Img//SlideToContinue.png");

            LoginEn = (Bitmap)Bitmap.FromFile("Img//LoginEn.png");
            TaiKhoanCuaToi = (Bitmap)Bitmap.FromFile("Img//TaiKhoanCuaToi.png");
            DienSau = (Bitmap)Bitmap.FromFile("Img//DienSau.png");
            LoginOK = (Bitmap)Bitmap.FromFile("Img//LoginOK.png");
            NutLoginLzd = (Bitmap)Bitmap.FromFile("Img//NutLoginLzd.png");
            Facebook = (Bitmap)Bitmap.FromFile("Img//Facebook.png");
            ThemEmail = (Bitmap)Bitmap.FromFile("Img//ThemEmail.png");
            ThemEmailDo = (Bitmap)Bitmap.FromFile("Img//ThemEmailDo.png");
            ChinhSua = (Bitmap)Bitmap.FromFile("Img//ChinhSua.png");
            string output = RunCmd("wmic diskdrive get serialNumber");
            using (StreamWriter HDD = new StreamWriter("HDD.txt", true))
            {
                HDD.WriteLine(output);
                HDD.Close();
            }
            string[] lines = File.ReadAllLines("HDD.txt");
            File.Delete("HDD.txt");
            string str = Regex.Replace(lines[2], @"\s", "");
            string text = null;
            try
            {
                HttpRequest kiemtrakey = new HttpRequest();
                text = kiemtrakey.Get("https://docs.google.com/document/d/119QOhyNo6E9-bD4cMfqpihvcmqwUg4qE81wmiWls9dU/edit?usp=sharing", null).ToString();
            }
            catch
            {

                while (true)
                {

                }
            }
            if (text.Contains(str))
            {

            }
            else
            {

                while (true)
                {

                }

            }
        }
        void Fakeip(String devices, String ip, String port)
        {
            AdbCommand("adb -s " + devices + " shell settings put global http_proxy " + ip + ":" + port);
        }
        void RemoveIp(String devices)
        {
            AdbCommand("adb -s " + devices + " shell settings put global http_proxy :0");
        }
        String randomten()
        {
            string FileHo = File.ReadAllText("Ho.txt");
            String[] FileHoNe = FileHo
                .Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);

            string FileDemTen = File.ReadAllText("DemTen.txt");
            String[] FileDemTenNe = FileDemTen
                .Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);

            string Hone = null;
            for (int i = 0; i < FileHoNe.Length; i++)
            {
                Hone = FileHoNe[r.Next(0, FileHoNe.Length)];
            }
            string tenne = null;
            for (int i = 0; i < FileDemTenNe.Length; i++)
            {
                tenne = FileDemTenNe[r.Next(0, FileDemTenNe.Length)];
            }
            return Hone + " " + tenne;
        }
        String sonha150(int chieusau)
        {

            string sdt = null;
            string chars = "0123456789";

            string nums = null;
            for (int i = 0; i < chieusau; i++)
            {
                nums += chars.Substring(r.Next(0, chars.Length - 1), 1);
            }

            return nums;
        }
        void GoTiengViet(string deviceID, string Text)
        {

            string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(Text));
            AdbCommand("adb -s " + deviceID + " shell ime set com.android.adbkeyboard/.AdbIME");
            AdbCommand("adb -s " + deviceID + " shell am broadcast -a ADB_INPUT_B64 --es msg " + str);

        }
        void CaiKeyBoard(string deviceID)
        {
            AdbCommand("adb -s " + deviceID + " install " + "keyboard.apk");
            AdbCommand("adb -s " + deviceID + " shell ime set com.android.adbkeyboard/.AdbIME");
        }
        String randomName()
        {

            Random r = new Random();

            string fileten = File.ReadAllText("25000ten1.txt");
            String[] filetenne = fileten
                .Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);
            string Name = null;
            for (int i = 0; i < filetenne.Length; i++)
            {
                Name = filetenne[r.Next(0, filetenne.Length)];
            }

            return Name;
        }
        String ramdomKdau()
        {


            string fileten = File.ReadAllText("DemTen.txt");
            String[] filetenne = fileten
                .Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None);
            string hone = null;
            for (int i = 0; i < filetenne.Length; i++)
            {
                hone = filetenne[r.Next(0, filetenne.Length)];
            }

            return hone;
        }
        string randomStr(int stringlength)
        {

            string chars = "0123456789";
            string nums = null;
            for (int i = 0; i < stringlength; i++)
            {
                nums += chars.Substring(r.Next(0, chars.Length - 1), 1);
            }
            return nums;
        }
        string randomNum(int stringlength) //
        {


            string chars = "0123456789abcdefghijklmnopqrstuvwxtz";
            string nums = null;
            for (int i = 0; i < stringlength; i++)
            {
                nums += chars.Substring(r.Next(0, chars.Length - 1), 1);
            }
            return nums;
        }
        void AdbCommand(string command) // FASTBOOT
        {
            string cmdCommand = command;

            Process cmd = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;

            cmd.StartInfo = startInfo;
            cmd.Start();

            cmd.StandardInput.WriteLine(cmdCommand);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
        }
        string AdbCommandstr(string command) // FASTBOOT
        {
            string cmdCommand = command;

            Process cmd = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;

            cmd.StartInfo = startInfo;
            cmd.Start();

            cmd.StandardInput.WriteLine(cmdCommand);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            if (string.IsNullOrEmpty(cmdCommand))
                return " ";
            return cmdCommand;

        }
        string RunCmd(string cmd)
        {

            Process cmdProcess;
            cmdProcess = new Process();
            cmdProcess.StartInfo.FileName = "cmd.exe";
            cmdProcess.StartInfo.Arguments = "/c " + cmd;
            cmdProcess.StartInfo.RedirectStandardOutput = true;
            cmdProcess.StartInfo.UseShellExecute = false;
            cmdProcess.StartInfo.CreateNoWindow = true;

            cmdProcess.Start();
            string output = cmdProcess.StandardOutput.ReadToEnd();
            cmdProcess.WaitForExit();
            if (string.IsNullOrEmpty(output))
                return "";
            return output;

        }
        string NgayGio()
        {
            var HH = DateTime.Now.ToString("HH");
            var mm = DateTime.Now.ToString("mm");
            var ss = DateTime.Now.ToString("ss");
            var tt = DateTime.Now.ToString("s");
            var dd = DateTime.Now.ToString("dd");
            var MM = DateTime.Now.ToString("MM");
            var yyyy = DateTime.Now.ToString("yyyy");

            return HH + ":" + mm + ":" + ss + "-" + dd + "/" + MM + "/" + yyyy;
        }
        String GetProxy_TM(String key)
        {
            string proxy = null;
            while (proxy == null)
            {
                try
                {
                    HttpRequest http = new HttpRequest();
                    String Data = @"{""api_key"":""" + key + @""",""id_location"":1}";
                    string html = http.Post("https://tmproxy.com/api/proxy/get-new-proxy", Data, "application/json").ToString();
                    var result = JsonConvert.DeserializeObject<TmProxy>(html);
                    if (result.code == 0)
                    {
                        proxy = result.data.https;
                        break;
                    }
                    else
                    {


                        int time = TachSo(result.message);

                        Invoke(new Action(() =>
                        {

                            rbNoiDung.Text += "Chờ " + time + '\n';
                        }));
                        Thread.Sleep(time * 1000);
                    }


                }
                catch
                {


                }
            }
            return proxy;
        }

        void CheckImg(string deviceID, Bitmap ImgFind, int Time, int loop)
        {
            int wait;

            bool Check = false;

            wait = 0;
            while (Check == false && wait < loop)
            {

                Check = IshaveImg(deviceID, ImgFind);
                Thread.Sleep(Time);
                wait++;
            }
        }
        int TachSo(string input)
        {
            int i = 1;
            string[] numbers = Regex.Split(input, @"\D+");
            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    i = int.Parse(value);
                }
            }
            return i;
        }
        bool IshaveImg(String deviceID, Bitmap ImgFind)
        {
            bool Check = false;
            try
            {

                Bitmap bm = (Bitmap)ImgFind.Clone();
                var screen = ADBHelper.ScreenShoot(deviceID);
                var Point = ImageScanOpenCV.FindOutPoint(screen, bm);
                if (Point != null)
                {
                    Check = true;
                }


            }
            catch
            {


            }
            return Check;
        }
        void Tapimg(String deviceID, Bitmap ImgFind)
        {
            {

                Bitmap bm = (Bitmap)ImgFind.Clone();
                var screen = ADBHelper.ScreenShoot(deviceID);
                var Point = ImageScanOpenCV.FindOutPoint(screen, bm);

                if (Point != null)
                {

                    ADBHelper.Tap(deviceID, Point.Value.X + 14, Point.Value.Y + 3);

                    return;
                }



            }
        }
        void DeleteAllMail(String username, String password)
        {
            MailServer oServer = new MailServer("imap-mail.outlook.com", username, password, ServerProtocol.Imap4);
            oServer.SSLConnection = true;
            oServer.Port = 993;
            MailClient oClient = new MailClient("TryIt");
            oClient.Connect(oServer);

            MailInfo[] infos = oClient.GetMailInfos();
            for (int i = 0; i < infos.Length; i++)
            {
                MailInfo info = infos[i];

                oClient.Delete(info);
            }
        }
        String getOTPHotmail(String username, String password)
        {
            String otp = null;

            MailServer oServer = new MailServer("imap-mail.outlook.com", username, password, ServerProtocol.Imap4);
            oServer.SSLConnection = true;
            oServer.Port = 993;
            MailClient oClient = new MailClient("TryIt");
            oClient.Connect(oServer);

            MailInfo[] infos = oClient.GetMailInfos();
            if (infos.Length > 0)
            {
                for (int i = 0; i < infos.Length; i++)
                {
                    MailInfo info = infos[i];
                    Mail oMail = oClient.GetMail(info);
                    if (oMail.From.Name == "Lazada Vietnam")
                    {
                        otp = Regex.Match(oMail.TextBody, @"\d{6}").ToString();
                    }

                    // Mark email as deleted from IMAP4 server.
                    oClient.Delete(info);
                }
            }
            else
            {
                otp = null;
            }

            return otp;
        }
        String GetXproxy(string KeyProxyne)
        {
            string proxy = null;
            while (proxy == null)
            {
                try
                {
                    int vitri = int.Parse(KeyProxyne.Substring(KeyProxyne.Length - 2, 2));
                    HttpRequest http = new HttpRequest();
                    String Data = @"{""wan"":""eth1"",""position"":""" + vitri + @"""}";
                    string html = http.Post(@"http://" + IpPort.Text + "/v2/rotate_ip", Data, "application/json;charset=UTF-8").ToString();
                    var result = JsonConvert.DeserializeObject<Xproxy>(html);
                    if (result.status == true)
                    {
                        proxy = KeyProxyne;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(int.Parse("20000"));


                    }


                }
                catch
                {


                }
            }
            return proxy;
        }
        String GetOTPfacebook(String username, String password)
        {
            String otp = null;
            String XuatOTP = null;
            MailServer oServer = new MailServer("imap-mail.outlook.com", username, password, ServerProtocol.Imap4);
            oServer.SSLConnection = true;
            oServer.Port = 993;
            MailClient oClient = new MailClient("TryIt");
            oClient.Connect(oServer);

            MailInfo[] infos = oClient.GetMailInfos();
            if (infos.Length > 0)
            {
                for (int i = 0; i < infos.Length; i++)
                {
                    MailInfo info = infos[i];
                    Mail oMail = oClient.GetMail(info);
                    if (oMail.From.Name == "Facebook")
                    {

                        otp = Regex.Match(oMail.TextBody, @"Mở Facebook và nhập mã này: \d{5}").ToString();
                        XuatOTP = Regex.Match(otp, @"\d{5}").ToString();

                        while (XuatOTP == "")
                        {
                            otp = Regex.Match(oMail.TextBody, @"開啟 Facebook 並輸入以下代碼：\d{5}").ToString();
                            XuatOTP = Regex.Match(otp, @"\d{5}").ToString();
                        }

                    }

                    // Mark email as deleted from IMAP4 server.
                    oClient.Delete(info);
                }
            }
            else
            {
                XuatOTP = null;
            }

            return XuatOTP;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < int.Parse(tbSoLuong.Text); i++)
            {
                ldplayer.Open("index", i.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ldplayer.CloseAll();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<String> Devices = KAutoHelper.ADBHelper.GetDevices();
            int dem = 0;
            String[] Device = new String[Devices.Count];
            foreach (var item in Devices)
            {
                Device[dem] = item;
                dem++;
            }
            for (int i = 0; i < Device.Length; i++)
            {
                int iThread = i;
                new Thread(() =>
                {


                    AdbCommand("adb -s " + Device[iThread] + " shell pm clear mark.via.gp");

                }).Start();
            }

        }


        void chupanh(string deviceID)
        {
            //var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID, false);
            //var findimg = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, appphone);
            //var findimgon = KAutoHelper.ImageScanOpenCV.Find(screen, appphone);
            //findimgon.Save("findimgon1.png");



        }

        //private void button6_Click(object sender, EventArgs e)
        //{

        //    AdbCommand("adb");
        //    List<String> Devices = KAutoHelper.ADBHelper.GetDevices();
        //    int dem = 0;
        //    String[] Device = new String[Devices.Count];
        //    foreach (var item in Devices)
        //    {
        //        Device[dem] = item;
        //        dem++;
        //    }
        //    for (int i = 0; i < Device.Length; i++)
        //    {
        //        int iThread = i;
        //        new Thread(() =>
        //        {



        //            //chupanh(Device[iThread]);

        //            //test(Device[iThread]);
        //            chupanh(Device[iThread]);


        //        }).Start();

        //    }
        //    void chupanh(string deviceID)
        //    {
        //        var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID, false);
        //        var findimg = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, XacMinhQMK);
        //        var findimgon = KAutoHelper.ImageScanOpenCV.Find(screen, XacMinhQMK);
        //        findimgon.Save("findimgon1.png");



        //    }
        //}

        private void button7_Click(object sender, EventArgs e)
        {

            List<String> Devices = KAutoHelper.ADBHelper.GetDevices();
            int dem = 0;
            String[] Device = new String[Devices.Count];
            foreach (var item in Devices)
            {
                Device[dem] = item;
                dem++;
            }
            for (int i = 0; i < Device.Length; i++)
            {
                int iThread = i;
                new Thread(() =>
                {



                }).Start();
            }
        }

        void TimQua(string deviceID, string acc, string pass)
        {
            AdbCommand("adb -s " + deviceID + " shell am start -n mark.via.gp/mark.via.Shell" + " https://pages.lazada.vn/wow/camp/lazada/channel/vn/users/sharepocket-s?");
            Thread.Sleep(5000);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 78.6, 72.9);
            Thread.Sleep(200);
        }



        //private void button12_Click(object sender, EventArgs e)
        //{



        //    string tenmaytinh = Environment.MachineName.ToString();
        //    HttpRequest http2 = new HttpRequest();
        //    HttpRequest http1 = new HttpRequest();
        //    HttpRequest http3 = new HttpRequest();
        //    String Getip = http2.Get("http://kiemtraip.com/raw.php").ToString();
        //    var Cookie = "PHPSESSID=b42gfum4eh25eifi49qf73nrrp; _ga=GA1.2.49096434.1632199818; _gid=GA1.2.913032249.1632199818";
        //    http1.AddHeader("Cookie", Cookie);
        //    http1.AddHeader("origin", "https://notepad.vn");
        //    http1.AddHeader("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36");
        //    http1.AddHeader("referer", "https://notepad.vn/XHizjZ164");

        //    String html1 = http1.Get("https://notepad.vn/XHizjZ164").ToString();
        //    string laystr1 = Regex.Match(html1, @"name=""description"" content=""(.*?"")").ToString();

        //    string[] laystr2 = Regex.Split(laystr1, @"""(.*?"")");

        //    //https://notepad.vn/XHizjZ164



        //    http3.AddHeader("Cookie", Cookie);
        //    http3.AddHeader("origin", "https://notepad.vn");
        //    http3.AddHeader("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36");
        //    http3.AddHeader("referer", "https://notepad.vn/XHizjZ164");
        //    string noidung = "Ten may tinh: "+tenmaytinh + '\n'+ "Ip: "+Getip+'\n';
        //    String Data = @"content=+"+ laystr2[3]+ "\n"+noidung;

        //    String html = http3.Post("https://notepad.vn/update_data/XHizjZ164", Data, "application/x-www-form-urlencoded; charset=UTF-8").ToString();
        //}
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }




        void guimail(string form, string to, string subject, string message)
        {
            MailMessage mess = new MailMessage(form, to, subject, message);

            SmtpClient client = new SmtpClient("smtp.mail.yahoo.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("ducchi953@yahoo.com", "0981281932wW@");
            client.Send(mess);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdbCommand("broxy.txt");



        }
        //private void button6_Click(object sender, EventArgs e)
        //{
        //    string[] keyProxys = rbKeyProxy.Text.Split(
        //             new[] { "\r\n", "\r", "\n" },
        //             StringSplitOptions.None
        //         );
        //    string[] ListAcc = rbKeyAccFB.Text.Split(
        //        new[] { "\r\n", "\r", "\n" },
        //            StringSplitOptions.None);

        //    List<String> Devices = KAutoHelper.ADBHelper.GetDevices();
        //    int dem = 0;
        //    String[] Device = new String[Devices.Count];
        //    foreach (var item in Devices)
        //    {
        //        Device[dem] = item;
        //        dem++;
        //    }
        //    for (int i = 0; i < keyProxys.Length; i++)
        //    {

        //        int iThread = i;
        //        new Thread(() =>
        //        {
        //            Thread.CurrentThread.IsBackground = true;
        //            while (true)
        //            {

        //                try
        //                {
        //                    string userName = null;
        //                    lock (locker)
        //                    {
        //                        if (nextIndex >= ListAcc.Length) return;
        //                        userName = ListAcc[nextIndex];
        //                        nextIndex++;
        //                    }
        //                    String[] Parse = userName.Split('|');
        //                    string proxy = "";
        //                    if (CbTinSoft.Checked)
        //                    {
        //                        proxy = GET_proxy(keyProxys[iThread]);
        //                    }

        //                    if (CbTM.Checked)
        //                    {

        //                        proxy = GetProxy_TM(keyProxys[iThread]);

        //                    }
        //                    String[] ProxyParse = proxy.Split(':');
        //                    Fakeip(Device[iThread], ProxyParse[0], ProxyParse[1]);

        //                    va(Device[iThread], Parse[0], Parse[1]);

        //                    //    RegRequestne(Device[iThread], Parse[0], Parse[1], Parse[2], Parse[3], iThread);




        //                    RemoveIp(Device[iThread]);
        //                    Thread.Sleep(2000);
        //                    AdbCommand("adb -s " + Device[iThread] + " shell pm clear mark.via.gp");
        //                    AdbCommand("adb -s " + Device[iThread] + " shell pm clear mark.via.gp");
        //                    Thread.Sleep(2000);
        //                    ldplayer.Close("name", Device[iThread]);
        //                    Thread.Sleep(5000);
        //                    String imei = "865160" + randomStrNum(9);
        //                    ldplayer.Change_Property("name", Device[iThread], "--imei " + imei + " --model " + imei + " --manufacturer " + imei);
        //                    ldplayer.Open("name", Device[iThread]);
        //                    Thread.Sleep(30000);
        //                }
        //                catch
        //                {
        //                    RemoveIp(Device[iThread]);
        //                    Thread.Sleep(2000);
        //                    AdbCommand("adb -s " + Device[iThread] + " shell pm clear com.android.browser");
        //                    Thread.Sleep(1000);
        //                    AdbCommand("adb -s " + Device[iThread] + " shell pm clear com.android.browser");
        //                    Thread.Sleep(2000);
        //                    Invoke(new Action(() =>
        //                    {
        //                        rbNoiDung.Text += "Dong LD " + Device[iThread] + '\n';
        //                    }));
        //                    ldplayer.Close("name", Device[iThread]);
        //                    Thread.Sleep(5000);
        //                    String imei = "86516" + randomStrNum(11);
        //                    Invoke(new Action(() =>
        //                    {
        //                        rbNoiDung.Text += "change property " + imei + '\n';
        //                    }));
        //                    ldplayer.Change_Property("name", Device[iThread], "--imei " + imei + " --model " + imei + " --manufacturer " + imei);
        //                    Invoke(new Action(() =>
        //                    {
        //                        rbNoiDung.Text += "Mo LD " + imei + '\n';
        //                    }));
        //                    ldplayer.Open("name", Device[iThread]);
        //                    Invoke(new Action(() =>
        //                    {
        //                        rbNoiDung.Text += "Cho 30s " + '\n';
        //                    }));
        //                    Thread.Sleep(28000); // roi ok

        //                }






        //            }



        //        }).Start();
        //    }
        //    void va(String deviceID, string acc, String pass)
        //    {
        //        try
        //        {
        //            AdbCommand("adb -s " + deviceID + " shell am start -n mark.via.gp/mark.via.Shell" + " https://www.facebook.com/");
        //            Thread.Sleep(9000);
        //            KAutoHelper.ADBHelper.TapByPercent(deviceID, 78.6, 72.9);
        //            Thread.Sleep(400);
        //            KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.4, 30.4);
        //            Thread.Sleep(100);
        //            KAutoHelper.ADBHelper.InputText(deviceID, acc);
        //            Thread.Sleep(100);
        //            KAutoHelper.ADBHelper.TapByPercent(deviceID, 13.6, 38.2);
        //            Thread.Sleep(100);
        //            KAutoHelper.ADBHelper.InputText(deviceID, pass);
        //            Thread.Sleep(500);
        //            KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.0, 46.7);
        //            Thread.Sleep(9000);

        //            AdbCommand("adb -s " + deviceID + " shell am start -n mark.via.gp/mark.via.Shell" + " https://member-m.lazada.vn/user/login");
        //            Thread.Sleep(5000);
        //            KAutoHelper.ADBHelper.TapByPercent(deviceID, 78.6, 72.9);
        //            Thread.Sleep(200);
        //            int countss1 = 0;
        //            bool loginlaz1 = false;
        //            Thread.Sleep(3000);
        //            while (loginlaz1 == false && countss1 < 5)
        //            {
        //                loginlaz1 = IshaveImg(deviceID, NutLoginLzd);
        //                if (loginlaz1)
        //                {
        //                    break;
        //                }
        //                loginlaz1 = IshaveImg(deviceID, LoginEn);
        //                if (loginlaz1)
        //                {
        //                    break;
        //                }
        //                countss1++;
        //                Thread.Sleep(2000);

        //            }
        //            KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 49.1, 83.1, 51.8, 29.2);
        //            Thread.Sleep(1000);
        //            Tapimg(deviceID, Facebook);

        //            Thread.Sleep(1500);
        //            if (IshaveImg(deviceID, NutLoginLzd) || IshaveImg(deviceID, LoginEn))
        //            {
        //                KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 49.1, 83.1, 51.8, 29.2);
        //                Thread.Sleep(1000);
        //                Tapimg(deviceID, Facebook);
        //            }
        //            Tapimg(deviceID, Facebook);
        //            Thread.Sleep(4000);

        //            int countss2 = 0;
        //            bool CheckEdit = false;

        //            while (CheckEdit == false && countss2 < 5)
        //            {
        //                CheckEdit = IshaveImg(deviceID, ChinhSua);
        //                countss2++;
        //                Thread.Sleep(1000);

        //            }
        //            if (IshaveImg(deviceID, ChinhSua))
        //            {
        //                KAutoHelper.ADBHelper.TapByPercent(deviceID, 40.4, 43.1);//tap chinh sửa
        //                Thread.Sleep(300);
        //                KAutoHelper.ADBHelper.TapByPercent(deviceID, 90.9, 43.3);
        //                Thread.Sleep(300);
        //                KAutoHelper.ADBHelper.TapByPercent(deviceID, 42.2, 57.9);//tiep tuc
        //                Thread.Sleep(3000);
        //            }

        //            int countss3 = 0;
        //            bool CHeckDienSau = false;
        //            while (CHeckDienSau == false && countss3 < 5)
        //            {
        //                CHeckDienSau = IshaveImg(deviceID, DienSau);

        //                Thread.Sleep(1000);
        //                countss3++;
        //            }
        //            bool CheckTaiKhoanCuaToi = false;

        //            int wait1 = 0;

        //            while (CheckTaiKhoanCuaToi == false && wait1 < 5)
        //            {

        //                CheckTaiKhoanCuaToi = IshaveImg(deviceID, TaiKhoanCuaToi);
        //                if (CheckTaiKhoanCuaToi)
        //                {
        //                    break;
        //                }
        //                Tapimg(deviceID, DienSau);

        //                Thread.Sleep(3000);
        //                CheckTaiKhoanCuaToi = IshaveImg(deviceID, TaiKhoanCuaToi);
        //                wait1++;
        //            }
        //            bool ca = IshaveImg(deviceID, TaiKhoanCuaToi);
        //            if (ca == false)
        //            {
        //                RemoveIp(deviceID);
        //                Thread.Sleep(2000);
        //                AdbCommand("adb -s " + deviceID + " shell pm clear mark.via.gp");

        //            }
        //            else
        //            {
        //                AdbCommand("adb -s " + deviceID + " shell am start -n mark.via.gp/mark.via.Shell" + " https://member-m.lazada.vn/address?spm=a2o4n.myaccount.address-book.1.5eb6448dSCxn7R#/create/shipping");
        //                Thread.Sleep(7000);
        //                KAutoHelper.ADBHelper.TapByPercent(deviceID, 18.1, 27.9);
        //                string Diachi = sonha150(2) + "/" + sonha150(2) + "/" + sonha150(2) + "/" + sonha150(2) + "/" + sonha150(2) + "/" + sonha150(2) + "/" + sonha150(2) + randomTenCoDau();
        //                string tenne = randomTenCoDau();
        //                Thread.Sleep(500);
        //                KAutoHelper.ADBHelper.InputText(deviceID, tenne);
        //                Thread.Sleep(500);
        //                KAutoHelper.ADBHelper.TapByPercent(deviceID, 25.3, 39.1);
        //                Thread.Sleep(500);
        //                KAutoHelper.ADBHelper.InputText(deviceID, Diachi);
        //                Thread.Sleep(500);
        //                KAutoHelper.ADBHelper.TapByPercent(deviceID, 28.3, 51.9);
        //                Thread.Sleep(500);
        //                KAutoHelper.ADBHelper.TapByPercent(deviceID, 38.6, 22.3);
        //                Thread.Sleep(1000);
        //                KAutoHelper.ADBHelper.TapByPercent(deviceID, 20.8, 62.4);
        //                Thread.Sleep(1000);
        //                KAutoHelper.ADBHelper.TapByPercent(deviceID, 37.4, 73.4);
        //                Thread.Sleep(1000);
        //                KAutoHelper.ADBHelper.TapByPercent(deviceID, 19.0, 73.9);
        //            }
        //        }
        //        catch
        //        {
        //            KXacDinh.WriteToFileLine(acc + "|" + pass + "|" + NgayGio());
        //        }


        //    }
        //}

        private void button8_Click(object sender, EventArgs e)
        {
            int a = 0;
            while (a < 5)
            {
                MessageBox.Show(a.ToString());
                a++;
            }


        }

        public class TinsoftProxy
        {
            public bool success { get; set; }
            public string proxy { get; set; }
            public int location { get; set; }
            public string next_change { get; set; }
            public int timeout { get; set; }
        }
        public class TinSoftFail
        {
            public bool success { get; set; }
            public string description { get; set; }
            public int next_change { get; set; }
        }
        public class TmProxy
        {
            public int code { get; set; }
            public string message { get; set; }
            public DataProxy data { get; set; }
        }

        public class Xproxy
        {
            public bool status { get; set; }
            public string msg { get; set; }
        }

        public class DataProxy
        {
            public string ip_allow { get; set; }
            public string location_name { get; set; }
            public string socks5 { get; set; }
            public string https { get; set; }
            public int timeout { get; set; }
            public int next_request { get; set; }
            public string expired_at { get; set; }
        }
        public class Token2Fa
        {
            public string token { get; set; }
        }



        public class Writer
        {
            public string Filepath { get; set; }
            private static object locker = new Object();

            public Writer(string filepath)
            {
                this.Filepath = filepath;
            }

            public void WriteToFile(string text)
            {
                lock (locker)
                {
                    using (FileStream file = new FileStream(Filepath, FileMode.Append, FileAccess.Write, FileShare.Read))
                    using (StreamWriter writer = new StreamWriter(file, Encoding.Unicode))
                    {
                        writer.Write(text.ToString());
                    }
                }

            }
            public void WriteToFileLine(string text)
            {
                WriteToFile(text + "\r\n");

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

            //string[] keyProxys = File.ReadAllLines("KeyProxy123.txt");
            ////string[] keyProxys = richTextBox1.Text.Split(
            //         new[] { "\r\n", "\r", "\n" },
            //         StringSplitOptions.None


            //List<String> Devices = KAutoHelper.ADBHelper.GetDevices();
            //int dem = 0;
            //String[] Device = new String[Devices.Count];
            //foreach (var item in Devices)
            //{
            //    Device[dem] = item;
            //    dem++;
            //}
            //for (int i = 0; i < keyProxys.Length; i++)
            //{

            //    int iThread = i;
            //    new Thread(() =>
            //    {
            //        Thread.CurrentThread.IsBackground = true;
            //        while (true)
            //        {

            //            try
            //            {
            //                string userName = null;
            //                lock (locker)
            //                {
            //                    if (nextIndex >= ListAcc.Length) return;
            //                    userName = ListAcc[nextIndex];
            //                    nextIndex++;
            //                }
            //                String[] Parse = userName.Split('|');
            //                string proxy = "";
            //                if (CbTinSoft.Checked)
            //                {
            //                    proxy = GET_proxy(keyProxys[iThread]);
            //                }
            //                if (cbXproxy.Checked)
            //                {

            //                    proxy = GetXproxy(keyProxys[iThread]);

            //                }
            //                if (CbTM.Checked)
            //                {

            //                    proxy = GetProxy_TM(keyProxys[iThread]);

            //                }


            //            }
            //            catch
            //            {

            //            }






            //        }



            //    }).Start();


            //}
        }

        private void button9_Click(object sender, EventArgs e)
        {
            AdbCommand("sourceacc.txt");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            List<String> Devices = KAutoHelper.ADBHelper.GetDevices();
            int dem = 0;

            String[] Device = new String[Devices.Count];
            foreach (var item in Devices)
            {
                Device[dem] = item;
                dem++;
            }

            for (int i = 0; i < Device.Length; i++)
            {

                int iThread = i;
                new Thread(() =>
                {


                    try
                    {


                        GoTiengViet(Device[iThread], "OK");

                    }
                    catch
                    {

                    }










                }).Start();


            }

        }
        String ShopLike(String key)
        {
            string proxy = null;
            while (proxy == null)
            {
                try
                {
                    HttpRequest http = new HttpRequest();

                    string html = http.Get("https://proxy.shoplike.vn/Api/getNewProxy?access_token=" + key + "&location=&provider=").ToString();
                    var result = JsonConvert.DeserializeObject<ClassShopLie>(html);
                    if (result.status == "success")
                    {
                        proxy = result.data.proxy;
                        break;
                    }
                    else
                    {


                        int time = TachSo(html);

                        Invoke(new Action(() =>
                        {

                            rbNoiDung.Text += "Chờ " + time + '\n';
                        }));
                        Thread.Sleep(time * 1000);
                    }


                }
                catch
                {


                }
            }
            return proxy;
        }
        public class Chothuesimcode
        {

            public int idress;
            public String GetSDT(String keyapi, string Number)
            {
                string phone = "";
                try
                {
                    HttpRequest http = new HttpRequest();


                    String a = http.Get("https://chothuesimcode.com/api?act=number&apik=" + keyapi + "&appId=1007&number=&carrier=" + Number + "&prefix=").ToString();

                    var id = JsonConvert.DeserializeObject<IDChothuesimcode>(a);
                    phone = id.Result.Number;
                    idress = id.Result.Id;

                }
                catch
                {
                    phone = "";
                    idress = 0;
                }
                return phone;
            }
            public String OTP(String keyapi, int oderid)
            {
                string otp = "";
                oderid = idress;
                try
                {

                    HttpRequest http = new HttpRequest();
                    String b = http.Get("https://chothuesimcode.com/api?act=code&apik=" + keyapi + "&id=" + idress.ToString()).ToString();


                    var c = JsonConvert.DeserializeObject<GetOTPsimcode>(b);
                    otp = c.Result.Code;
                }
                catch
                {
                    otp = "";
                }

                return otp;
            }
            //https://chothuesimcode.com/api?act=number&apik=92a1c60b&appId=1007&number=829935590&carrier=&prefix=
            public int GetSDT_again(String keyapi, string Number_again)
            {

                try
                {
                    HttpRequest http = new HttpRequest();
                    String a = http.Get("https://chothuesimcode.com/api?act=number&apik=" + keyapi + "&appId=1007&number=" + Number_again + "&carrier=&prefix=").ToString();
                    var id = JsonConvert.DeserializeObject<GetOTPAgain>(a);
                    idress = id.Result.Id;

                }
                catch
                {

                    idress = 0;
                }
                return idress;
            }
            public void Cancel(String keyapi, int idorder)
            {
                try
                {
                    HttpRequest http = new HttpRequest();
                    String a = http.Get("https://chothuesimcode.com/api?act=expired&apik=" + keyapi + "&id=" + idorder).ToString();


                }
                catch
                {


                }
            }
        }
        public class Viotp
        {

            public int idress;
            public String GetSDT(String keyapi, string Number)
            {
                string phone = "";
                try
                {
                    HttpRequest http = new HttpRequest();
                    String a = http.Get("https://api.viotp.com/request/getv2?token=" + keyapi + "&serviceId=2&network=" + Number).ToString();

                    var d = JsonConvert.DeserializeObject<IDViOTP>(a);
                    phone = d.data.phone_number;
                    idress = d.data.request_id;

                }
                catch
                {
                    phone = "";
                    idress = 0;
                }
                return phone;
            }
            public String OTP(String keyapi, int oderid)
            {
                string otp = "";
                oderid = idress;
                try
                {

                    HttpRequest http = new HttpRequest();
                    String b = http.Get("https://api.viotp.com/session/getv2?requestId=" + oderid + "&token=" + keyapi).ToString();


                    var c = JsonConvert.DeserializeObject<GetOTPViOTP>(b);
                    otp = c.data.Code;
                }
                catch
                {
                    otp = "";
                }

                return otp;
            }
            //https://chothuesimcode.com/api?act=number&apik=92a1c60b&appId=1007&number=829935590&carrier=&prefix=
            public int GetSDT_again(String keyapi, string Number_again)
            {

                try
                {
                    HttpRequest http = new HttpRequest();
                    String a = http.Get("https://api.viotp.com/request/getv2?token=" + keyapi + "&serviceId=2&number=0" + Number_again).ToString();
                    var id = JsonConvert.DeserializeObject<IDViOTP>(a);
                    idress = id.data.request_id;

                }
                catch
                {

                    idress = 0;
                }
                return idress;
            }
            //public void Cancel(String keyapi, int idorder)
            //{
            //    try
            //    {
            //        HttpRequest http = new HttpRequest();
            //        String a = http.Get("https://chothuesimcode.com/api?act=expired&apik=" + keyapi + "&id=" + idorder).ToString();


            //    }
            //    catch
            //    {


            //    }
            //}
        }

        public class IDViOTP
        {
            public int status_code { get; set; }
            public string message { get; set; }
            public bool success { get; set; }
            public Data3 data { get; set; }
        }

        public class Data3
        {
            public string phone_number { get; set; }
            public string re_phone_number { get; set; }
            public string countryISO { get; set; }
            public string countryCode { get; set; }
            public int request_id { get; set; }
            public int balance { get; set; }
        }

        public class GetOTPViOTP
        {
            public int status_code { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
            public Data4 data { get; set; }
        }

        public class Data4
        {
            public int ID { get; set; }
            public int ServiceID { get; set; }
            public string ServiceName { get; set; }
            public int Status { get; set; }
            public int Price { get; set; }
            public string Phone { get; set; }
            public string SmsContent { get; set; }
            public string IsSound { get; set; }
            public DateTime CreatedTime { get; set; }
            public string Code { get; set; }
            public string PhoneOriginal { get; set; }
            public string CountryISO { get; set; }
            public string CountryCode { get; set; }
        }


        public class IDChothuesimcode
        {
            public int ResponseCode { get; set; }
            public string Msg { get; set; }
            public Result Result { get; set; }
        }

        public class Result
        {
            public int Id { get; set; }
            public string Number { get; set; }
            public string App { get; set; }
            public float Cost { get; set; }
            public float Balance { get; set; }
        }
        public class GetOTPsimcode

        {
            public int ResponseCode { get; set; }
            public string Msg { get; set; }
            public Results Result { get; set; }
        }

        public class Results
        {
            public string SMS { get; set; }
            public float Cost { get; set; }
            public string Code { get; set; }
        }
        public class GetOTPAgain
        {
            public int ResponseCode { get; set; }
            public string Msg { get; set; }
            public Result1 Result { get; set; }
        }

        public class Result1
        {
            public int Id { get; set; }
            public string Number { get; set; }
            public string App { get; set; }
            public float Cost { get; set; }
            public float Balance { get; set; }
        }
        public class Maxclone
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
            public EmailHA[] Emails { get; set; }
        }

        public class EmailHA
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class ClassShopLie
        {
            public string status { get; set; }
            public Data2 data { get; set; }
        }

        public class Data2
        {
            public string location { get; set; }
            public string proxy { get; set; }
            public string auth { get; set; }
            public string mess { get; set; }
        }

        private void tbKeyHotmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            List<String> Devices = KAutoHelper.ADBHelper.GetDevices();
            int dem = 0;

            String[] Device = new String[Devices.Count];
            foreach (var item in Devices)
            {
                Device[dem] = item;
                dem++;
            }

            for (int i = 0; i < Device.Length; i++)
            {

                int iThread = i;
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    while (true)
                    {

                        try
                        {


                            RegAccLZDPhone(Device[iThread]);

                        }
                        catch
                        {

                        }






                    }



                }).Start();


            }
        }
        void RegAccLZDPhone(string deviceID)
        {
            try
            {

            BackAll:
                AdbCommand("adb -s " + deviceID + " shell pm clear mark.via.gp");
                Thread.Sleep(200);
                AdbCommand("adb -s " + deviceID + @" shell su -c settings put global airplane_mode_on 1");
                Thread.Sleep(100);
                AdbCommand("adb -s " + deviceID + @" shell su -c am broadcast -a android.intent.action.AIRPLANE_MODE");
                Thread.Sleep(100);//mở máy bay
                AdbCommand("adb -s " + deviceID + @" shell su -c settings put global airplane_mode_on 0");
                Thread.Sleep(100);
                AdbCommand("adb -s " + deviceID + @" shell su -c am broadcast -a android.intent.action.AIRPLANE_MODE");
                Thread.Sleep(100);//tắt ,máy bay
                AdbCommand("adb -s " + deviceID + " shell am start -n zone.bytesreverser.xposeddeviceidmaskerlite/zone.bytesreverser.xposeddeviceidmaskerlite.ID");
                Thread.Sleep(2500);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 51.4, 24.8);
                Thread.Sleep(500);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 49.3, 36.9);
                Thread.Sleep(500);
                AdbCommand("adb -s " + deviceID + " shell am start -n com.google.android.reborn/com.google.android.reborn.MainActivity");
                Thread.Sleep(1500);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 48.0, 6.9);
                Thread.Sleep(500);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 58.8, 6.4);
                Thread.Sleep(1500);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 83.2, 88.2);
                Thread.Sleep(200);
                string nameDevice = randomNum(15);
                string buildNumber = randomNum(15);
                string userAgent = $"Mozilla/5.0(Linux; U; Android 2.2; en-gb;  LG-{nameDevice} Build/{buildNumber})   AppleWebKit/403.0 (KHTML, like Gecko) Version/4.0 Mobile Safari/403.1";
                String Data = File.ReadAllText($"AppRoot\\settings.xml");
                string FileNew = Data.Replace("randomUser", userAgent);
                File.WriteAllText($"AppRoot\\SettingNew\\settings.xml", FileNew);
                AdbCommand($"adb -s {deviceID} push " + "AppRoot\\SettingNew\\settings.xml " + @" /sdcard/settings.xml");
                Thread.Sleep(300);
                AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell");
                Thread.Sleep(1500);
                AdbCommand($"adb -s {deviceID} shell am force-stop mark.via.gp");
                Thread.Sleep(300);
                AdbCommand(@"adb -s " + deviceID + " shell su -c " + @"""cp /sdcard/settings.xml /data/data/mark.via.gp/shared_prefs/settings.xml""");
                Thread.Sleep(300);
            numberBack:
               AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://member.lazada.vn/user/forget-password?");
                string SDT = "";
                int id = 0;
                Chothuesimcode Chothuesimcoede1 = new Chothuesimcode();
                Viotp viotp1 = new Viotp();
                if (cbChothuesimcode.Checked)
                {

                    SDT = Chothuesimcoede1.GetSDT(ApiKey.Text, textBox2.Text);
                    id = Chothuesimcoede1.idress;
                }
                if (cbViotp.Checked)
                {
                    SDT = viotp1.GetSDT(ApiKey.Text, "VINAPHONE|VIETTEL");
                    id = viotp1.idress;
                }

                CheckImg(deviceID, SlideToContinuePhone, 500, 20);

                KAutoHelper.ADBHelper.TapByPercent(deviceID, 10.4, 24.7);
                GoTiengViet(deviceID, SDT);
                int Count = 0;
                bool swipeVery = false;
                while (swipeVery == false && Count < 10)
                {
                    KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 11.7, 40.6, 94.0, 999, 800);
                    Thread.Sleep(3000);
                    swipeVery = IshaveImg(deviceID, checkPhoneNumberPhone);
                    Count++;
                }
                Thread.Sleep(2000);
                bool checkNumber = IshaveImg(deviceID, SDTOLDPhone);
                if (checkNumber)
                {
                    if (cbChothuesimcode.Checked)
                    {
                        Chothuesimcoede1.Cancel(ApiKey.Text, id);
                    }

                    goto numberBack;

                }










                AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://member.lazada.vn/user/register?");
                CheckImg(deviceID, ButtonSwipePhone, 500, 15);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 10.7, 21.1);
                Thread.Sleep(200);
                KAutoHelper.ADBHelper.InputText(deviceID, SDT);
                int Count1 = 0;
                bool swipeReg = false;
                while (swipeReg == false && Count1 < 5)
                {
                    KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 11.4, 30.0, 92.0, 999, 800);
                    Thread.Sleep(3000);
                    swipeReg = IshaveImg(deviceID, GuiLaiPhone);
                    Count1++;
                }
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.4, 29.4);
                String OTP = null;
                int CountOTP = 0;

                int CountGetOTP = 0;
                bool GetOTP = false;
                while (GetOTP == false && CountGetOTP < 3)
                {
                    Thread.Sleep(5000);
                    while (OTP == null && CountOTP < 10)
                    {
                        if (cbChothuesimcode.Checked)
                        {
                            OTP = Chothuesimcoede1.OTP(ApiKey.Text, id);
                        }

                        if (cbViotp.Checked)
                        {
                            OTP = viotp1.OTP(ApiKey.Text, id);
                        }

                        if (OTP == null)
                        {
                            Thread.Sleep(1000);
                            CountOTP++;
                        }
                        else
                        {
                            GetOTP = true;
                            break;
                        }

                    }
                    if (OTP != null)
                    {

                    }
                    else
                    {
                        AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://member.lazada.vn/user/register?");
                        CheckImg(deviceID, ButtonSwipePhone, 500, 15);
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 10.7, 21.1);
                        Thread.Sleep(200);
                        KAutoHelper.ADBHelper.InputText(deviceID, SDT);
                        int CountReg = 0;
                        bool swipeReg1 = false;
                        while (swipeReg1 == false && CountReg < 5)
                        {
                            KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 11.4, 30.0, 92.0, 999, 800);
                            Thread.Sleep(3000);
                            swipeReg1 = IshaveImg(deviceID, GuiLaiPhone);
                            CountReg++;
                        }
                    }

                    CountGetOTP++;

                }
                if (OTP == null)
                {
                    Chothuesimcoede1.Cancel(ApiKey.Text, id);
                    
                }
                else
                {

                    int id2 = 0;
                    if (cbChothuesimcode.Checked)
                    {
                        id2 = Chothuesimcoede1.GetSDT_again(ApiKey.Text, SDT);
                    }
                    if (cbViotp.Checked)
                    {
                        id2 = viotp1.GetSDT_again(ApiKey.Text, SDT);
                    }

                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.4, 29.4);
                    Thread.Sleep(300);
                    KAutoHelper.ADBHelper.InputText(deviceID, OTP);
                    Thread.Sleep(300);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 51.0, 38.9);
                    Thread.Sleep(1000);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 77.8, 55.7);
                    Thread.Sleep(300);
                    CheckImg(deviceID, ContinuePhone, 500, 20);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 92.0, 20.9);
                    GoTiengViet(deviceID, randomName());
                    Thread.Sleep(300);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 9.0, 29.6);
                    Thread.Sleep(100);
                    KAutoHelper.ADBHelper.InputText(deviceID, Pass.Text);
                    Thread.Sleep(300);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 48.6, 45.0);
                    Thread.Sleep(4000);

                    AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://s.lazada.vn/s.cTWKm");
                    CheckImg(deviceID, SettingPhone, 500, 20);
                    Thread.Sleep(500);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 31.0, 85.6);
                    Thread.Sleep(2000);


                    int Count7 = 0;
                    bool checkTiger = false;
                    while (checkTiger == false && Count7 < 30)
                    {
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 29.0, 66.8);
                        Thread.Sleep(500);
                        KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 22.2, 60.7, 81.2, 999, 800);
                        Thread.Sleep(1000);
                        checkTiger = IshaveImg(deviceID, OpenPhone);
                        Count7++;
                    }
                    Thread.Sleep(2000);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 75.0, 75.0);
                    Thread.Sleep(1000);
                    AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://s.lazada.vn/s.cTWKm");
                    CheckImg(deviceID, SettingPhone, 500, 20);
                    Thread.Sleep(1000);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 31.0, 85.6);
                    Thread.Sleep(500);

                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 29.7, 79.5);

                    CheckImg(deviceID, AddOrderPhone, 500, 15);
                    Thread.Sleep(500);
                    if (IshaveImg(deviceID, buyMoreH5Phone))
                    {
                        AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://my-m.lazada.vn/member/account-info?");

                        CheckImg(deviceID, ThemEmailPhone, 500, 20);
                        int Count2 = 0;
                        bool checkAddMail = false;
                        while (checkAddMail == false && Count2 < 5)
                        {
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 13.4, 40.1);
                            Thread.Sleep(2000);
                            checkAddMail = IshaveImg(deviceID, VeryAddSDTPhone);
                            Count2++;
                        }
                        Thread.Sleep(1000);
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 42.5, 66.0);
                        Thread.Sleep(1500);

                        int CountGetOTP2 = 0;
                        bool GetOTP2 = false;
                        while (GetOTP2 == false && CountGetOTP2 < 4)
                        {
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 88.6, 46.0);//Click Send Code
                            String OTP2 = null;
                            int CountOTP2 = 0;
                            Thread.Sleep(5000);
                            while (OTP2 == null && CountOTP2 < 15)
                            {
                                if (cbChothuesimcode.Checked)
                                {
                                    OTP2 = Chothuesimcoede1.OTP(ApiKey.Text, id2);
                                }
                                if (cbViotp.Checked)
                                {
                                    OTP2 = viotp1.OTP(ApiKey.Text, id2);
                                }

                                if (OTP2 == null)
                                {
                                    Thread.Sleep(1000);
                                    CountOTP2++;

                                }
                                else
                                {
                                    GetOTP2 = true;
                                    break;
                                }

                            }
                            if (OTP2 != null)
                            {
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 10.7, 47.4);
                                Thread.Sleep(200);
                                KAutoHelper.ADBHelper.InputText(deviceID, OTP2);
                                Thread.Sleep(200);
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 49.3, 57.7);
                            }
                            else
                            {
                                AdbCommand($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://my-m.lazada.vn/member/account-info?");
                                Thread.Sleep(2000);
                                CheckImg(deviceID, ThemEmailPhone, 500, 20);
                                int Count6 = 0;
                                bool checkAddMail2 = false;
                                while (checkAddMail2 == false && Count6 < 5)
                                {
                                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 13.4, 40.1);
                                    Thread.Sleep(2000);
                                    checkAddMail2 = IshaveImg(deviceID, VeryAddSDTPhone);
                                    Count6++;
                                }
                                Thread.Sleep(1000);
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 42.5, 66.0);
                                Thread.Sleep(1500);


                            }

                            CountGetOTP2++;

                        }
                    BuyMail:
                        string TaiKhoan = "";
                        string MatKhau = "";
                        try
                        {
                            HttpRequest http = new HttpRequest();
                            //                
                            String HotMail = http.Get("https://api.hotmailbox.me/mail/buy?apikey=" + tbKeyHotmail.Text + "&mailcode=OUTLOOK.TRUSTED&quantity=1").ToString();
                            Thread.Sleep(1000);
                            var result = JsonConvert.DeserializeObject<Maxclone>(HotMail);
                            TaiKhoan = result.Data.Emails[0].Email;
                            MatKhau = result.Data.Emails[0].Password;
                        }
                        catch
                        {
                            goto BuyMail;
                        }
                        try
                        {
                            DeleteAllMail(TaiKhoan, MatKhau);
                        }
                        catch
                        {
                            goto BackAll;
                        }
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 11.4, 36.5);
                        KAutoHelper.ADBHelper.InputText(deviceID, TaiKhoan);
                        Thread.Sleep(200);
                        KAutoHelper.ADBHelper.TapByPercent(deviceID, 88.3, 46.0);
                        String OTPmail = null;
                        int Count3 = 0;
                        while (OTPmail == null && Count3 < 8)
                        {
                            OTPmail = getOTPHotmail(TaiKhoan, MatKhau);

                            if (OTPmail == null)
                            {
                                Thread.Sleep(1000);
                                Count3++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (OTPmail != null)
                        {
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.1, 46.8);
                            Thread.Sleep(200);
                            KAutoHelper.ADBHelper.InputText(deviceID, OTPmail);
                            Thread.Sleep(500);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 51.4, 59.0);
                            Thread.Sleep(5000);
                            AccRegXong.WriteToFileLine(SDT + "|" + Pass.Text + "|" + TaiKhoan + "|" + MatKhau + "|" + NgayGio());
                            string[] fileArray = File.ReadAllLines(@"AccReg//Acc_" + DateTime.Now.ToString("dd_MM") + ".txt");
                            string AccThanhCong = fileArray.Length.ToString();
                            Invoke((new Action(() =>
                            {

                                lbsuccessful.Text = AccThanhCong;
                            })));
                        }
                        else
                        {
                            OTP2Fail.WriteToFileLine(SDT + "|" + Pass.Text + "|" + NgayGio());
                            Invoke((new Action(() =>
                            {

                                otp2fail++;
                                lbOTP2.Text = otp2fail.ToString();
                            })));
                        }
                    }
                    else
                    {

                        Invoke((new Action(() =>
                        {

                            AccFail++;
                            label3.Text = AccFail.ToString();
                        })));
                    }



                }
            }
            catch
            {

            }

        }
        object lockerUser = new object();
        private void button9_Click_1(object sender, EventArgs e)
        {
            Writer writer = new Writer("Account/account.txt");
            Writer writer2 = new Writer("Account/accountKoMail.txt");
            Writer writer3 = new Writer("Account/accountNhatLoi.txt");
            List<String> listDevice = KAutoHelper.ADBHelper.GetDevices();
            string sim = textBox1.Text;
            foreach (var item in listDevice)
            {
               
                    Thread t = new Thread(() =>
                    {
                        Phone p = new Phone(item);
                        while (true)
                        {
                            try
                            {
                                p.FakeIP();




                                Thread.Sleep(2000);
                                p.FakeImei();
                                Thread.Sleep(2000);
                                lock (lockerUser)
                                {
                                    p.SetNewUserAgent();
                                }
                                Account acc = p.RegAcc("1a30f1c09b044e6a94db205931d98402");
                                if (acc.acc != "ko ve otp")
                                {
                                    bool checkget = p.getVoucher();
                                    if (checkget)
                                    {
                                        bool check = p.AddPhone("1a30f1c09b044e6a94db205931d98402", acc.acc);
                                        if (check)
                                        {
                                            Emails email;
                                            lock (lockerbuymail)
                                            {
                                                email = BuyMail();
                                            }
                                            Emails mail = p.AddMail(email);
                                            if (mail != null)
                                            {
                                                string date = DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt");
                                                writer.WriteToFileLine($"{acc.acc}|123123w|{mail.Email}|{mail.Password}|{date}");
                                            }
                                            else
                                            {
                                                string date = DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt");
                                                writer2.WriteToFileLine($"{acc.acc}|123123w|{date}");
                                            }
                                        }
                                        else
                                        {
                                            string date = DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt");
                                            writer2.WriteToFileLine($"{acc.acc}|123123w|{date}");
                                        }
                                    }
                                    else
                                    {
                                        writer3.WriteToFileLine($"{acc.acc}");

                                    }

                                }
                            }
                            catch 
                            {

                                
                            }
                           
                        }
                        
                    });
                    t.Start();
                    t.IsBackground = true;
               
            }
        }
        Hotmailbox hotmail = new Hotmailbox("7f41c99b4cf34cdeb61b767e7ea178f8");

        Emails BuyMail()
        {
            Emails mailbuy = hotmail.Buymail(1)[0];
            Thread.Sleep(1000);
            return mailbuy;
        }
        object lockerbuymail = new object();
    }


}
