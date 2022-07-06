using KAutoHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RegAccLzd
{
    public class Phone
    {
        public string deviceID;
        public RandomHelper random;
        public DataBitmap dataBitmap;
        public Random r;
        public Phone(string deviceID)
        {
            r = new Random();
            dataBitmap = new DataBitmap();
            this.deviceID = deviceID;
            random = new RandomHelper();
        }
        public void FakeIP()
        {
            KAutoHelper.ADBHelper.ExecuteCMD("adb -s " + deviceID + @" shell su -c settings put global airplane_mode_on 1");
            Thread.Sleep(100);
            KAutoHelper.ADBHelper.ExecuteCMD("adb -s " + deviceID + @" shell su -c am broadcast -a android.intent.action.AIRPLANE_MODE");
            Thread.Sleep(100);//mở máy bay
            KAutoHelper.ADBHelper.ExecuteCMD("adb -s " + deviceID + @" shell su -c settings put global airplane_mode_on 0");
            Thread.Sleep(100);
            KAutoHelper.ADBHelper.ExecuteCMD("adb -s " + deviceID + @" shell su -c am broadcast -a android.intent.action.AIRPLANE_MODE");
        }
        public void FakeImei()
        {
            KAutoHelper.ADBHelper.ExecuteCMD("adb -s " + deviceID + " shell am start -n zone.bytesreverser.xposeddeviceidmaskerlite/zone.bytesreverser.xposeddeviceidmaskerlite.ID");
            Thread.Sleep(2500);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 51.4, 24.8);
            Thread.Sleep(500);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 49.3, 36.9);
            Thread.Sleep(500);
            KAutoHelper.ADBHelper.ExecuteCMD("adb -s " + deviceID + " shell am start -n com.google.android.reborn/com.google.android.reborn.MainActivity");
            Thread.Sleep(1500);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 48.0, 6.9);
            Thread.Sleep(500);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 58.8, 6.4);
            Thread.Sleep(1500);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 83.2, 88.2);
            Thread.Sleep(200);
        }
        public void SetNewUserAgent()
        {
            KAutoHelper.ADBHelper.ExecuteCMD("adb -s " + deviceID + " shell pm clear mark.via.gp");
            string nameDevice = random.randomNum(30);
            string buildNumber = random.randomNum(30);
            string userAgent = $"Mozilla/5.0(Linux; U; Android 2.2; en-gb;  LG-{nameDevice} Build/{buildNumber})   AppleWebKit/{r.Next(100,999)}.0 (KHTML, like Gecko) Version/4.0 Mobile Safari/{r.Next(100, 999)}.1";
            String Data = File.ReadAllText($"AppRoot\\settings.xml");
            string FileNew = Data.Replace("randomUser", userAgent);
            File.WriteAllText($"AppRoot\\SettingNew\\settings.xml", FileNew);
            KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} push " + "AppRoot\\SettingNew\\settings.xml " + @" /sdcard/settings.xml");
            Thread.Sleep(500);
            KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell");
            Thread.Sleep(3000);
            KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell am force-stop mark.via.gp");
            Thread.Sleep(300);
            KAutoHelper.ADBHelper.ExecuteCMD(@"adb -s " + deviceID + " shell su -c " + @"""cp /sdcard/settings.xml /data/data/mark.via.gp/shared_prefs/settings.xml""");
            Thread.Sleep(300);
        }
        public Account RegAcc(string apikey)
        {
            Viootp simcode = new Viootp(apikey);

            string phone = "";
            int oderid = 0;
            bool checkbreak = false;
            while (checkbreak == false)
            {
                KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://member.lazada.vn/user/forget-password?");
                phone = simcode.GetSDT("VIETTEL|VINAPHONE");
                oderid = simcode.oderid;
                ClickImgForTime(dataBitmap.SlideToContinuePhone, 5);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 10.4, 24.7);
                Thread.Sleep(100);
                KAutoHelper.ADBHelper.InputText(deviceID, phone);
                Thread.Sleep(100);
                KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 11.7, 40.6, 94.0, 999, 800);
                Thread.Sleep(3000);
                ClickImgForTime(dataBitmap.checkPhoneNumberPhone, 5);
                if (IshaveImg(dataBitmap.XacMinhDiDong))
                {
                    checkbreak = false;
                }
                else
                {
                    checkbreak = true;
                }
            }
            Thread.Sleep(1000);
            KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell pm clear com.lazada.android");
            Thread.Sleep(300);
            KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell am start -n com.lazada.android/com.lazada.activities.EnterActivity");
            Thread.Sleep(5000);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 43.9, 65.3);
            Thread.Sleep(300);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 57.5, 53.5);
            Thread.Sleep(1000);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.7, 90.0);
            Thread.Sleep(4000);
            KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell su -c am start -n com.lazada.android/com.lazada.android.login.user.MobileSignUpActivity");
            Thread.Sleep(1000);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 82.2, 52.9);//click login
            Thread.Sleep(300);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 60.8, 26.0); //click sms
            Thread.Sleep(300);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 35.1, 35.7); // click truyen sdt
            Thread.Sleep(300);
            KAutoHelper.ADBHelper.InputText(deviceID, phone);
            Thread.Sleep(200);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 75.4, 43.5); // gui otp
            string otp = null;
            Thread.Sleep(10000);
            int countotp = 0;
            while (otp == null && countotp < 10)
            {
                otp = simcode.OTP(oderid);
                if (otp != null)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(1000);
                    countotp++;
                }
            }
            if (otp != null)
            {
                KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://member.lazada.vn/user/register?");
                Thread.Sleep(2000);
                ClickImgForTime(dataBitmap.ButtonSwipePhone, 5);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 10.7, 21.1);
                Thread.Sleep(200);
                KAutoHelper.ADBHelper.InputText(deviceID, phone);
                Thread.Sleep(200);
                KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 11.4, 30.0, 92.0, 999, 800);
                Thread.Sleep(200);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.4, 29.4);
                Thread.Sleep(300);
                KAutoHelper.ADBHelper.InputText(deviceID, otp);
                Thread.Sleep(200);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.3, 39.1); // tiếp tục
                Thread.Sleep(300);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 72.4, 55.5); // xac nhận
                Thread.Sleep(3000);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 91.7, 20.9);
                Thread.Sleep(100);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.7, 21.6);
                string name = random.getName();
                sendTen(name);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.4, 29.2);
                Thread.Sleep(200);
                KAutoHelper.ADBHelper.InputText(deviceID, "123123w");
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 47.0, 44.5); //click tiep theo
                Thread.Sleep(4000);
                Account acc = new Account(phone, "123123w");
                return acc;
            }
            else
            {
                Account acc = new Account("ko ve otp", "123123w");
                return acc;
            }
        }
        public bool getVoucher()
        {
            bool check = false;
            KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://s.lazada.vn/s.cGoDK");
            //KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://s.lazada.vn/s.cTWKm");
            Thread.Sleep(12000);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 94.7, 66.0); //click X
            Thread.Sleep(200);



            KAutoHelper.ADBHelper.TapByPercent(deviceID, 27.6, 87.0);
            //đối tác
            //KAutoHelper.ADBHelper.TapByPercent(deviceID, 27.3, 87.3);//thu thập đối tác
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 81.5, 67.3);
            //zalo
            //KAutoHelper.ADBHelper.TapByPercent(deviceID, 28.7, 67.2);//thu thập zalo
            Thread.Sleep(2000);
            int Count7 = 0;
            bool checkTiger = false;
            while (checkTiger == false && Count7 < 30)
            {
                //KAutoHelper.ADBHelper.TapByPercent(deviceID, 29.0, 66.8);
                //Thread.Sleep(500);
                KAutoHelper.ADBHelper.SwipeByPercent(deviceID, 22.2, 60.7, 81.2, 999, 800);
                Thread.Sleep(1000);
                checkTiger = IshaveImg(dataBitmap.OpenPhone);
                Count7++;
            }
            //đối tác
            //KAutoHelper.ADBHelper.TapByPercent(deviceID, 27.3, 87.3);//thu thập đối tác
            //KAutoHelper.ADBHelper.TapByPercent(deviceID, 81.5, 67.3);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 27.6, 87.0);
            ////zalo
            //KAutoHelper.ADBHelper.TapByPercent(deviceID, 28.7, 67.2);//thu thập zalo
            Thread.Sleep(5000);
            if (IshaveImg(dataBitmap.buyMoreH5Phone))
            {
                check = true;
            }

            return check;
        }
        public bool AddPhone(string apikey,string phone)
        {
            try
            {
                Viootp simcode = new Viootp(apikey);
                simcode.ThueLaiSim(phone);
                int oderid = simcode.oderid;
                KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell su -c am start -n com.lazada.android/com.lazada.android.login.user.MobileSignUpActivity");
                Thread.Sleep(2000);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 82.2, 52.9);//click login
                Thread.Sleep(300);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 60.8, 26.0); //click sms
                Thread.Sleep(300);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 35.1, 35.7); // click truyen sdt
                Thread.Sleep(300);
                KAutoHelper.ADBHelper.InputText(deviceID, phone);
                Thread.Sleep(200);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 75.4, 43.5); // gui otp
                Thread.Sleep(20000);
                string otp = simcode.OTP(oderid); ;
                if (otp != null)
                {
                    KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell pm clear com.lazada.android");
                    Thread.Sleep(200);
                    KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell am start -n mark.via.gp/mark.via.Shell " + "https://my-m.lazada.vn/member/account-info?");
                    Thread.Sleep(4000);
                    ClickImgForTime(dataBitmap.ThemEmailPhone, 5);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 82.5, 40.4);
                    Thread.Sleep(2000);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.3, 65.3);
                    Thread.Sleep(1000);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 87.9, 46.0);
                    Thread.Sleep(200);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 14.4, 47.5);
                    KAutoHelper.ADBHelper.InputText(deviceID, otp);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 17.5, 47.9);
                    Thread.Sleep(200);
                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 49.7, 57.5);
                    return true;
                }
                else
                {
                    KAutoHelper.ADBHelper.ExecuteCMD($"adb -s {deviceID} shell pm clear com.lazada.android");
                    Thread.Sleep(200);
                    // simcode.CancelOder(oderid);
                    return false;
                }
            }
            catch 
            {
                return false;
            }
            //Chothuesimcode simcode = new Chothuesimcode(apikey);
           
        }
        public Emails AddMail(Emails mailbuy)
        {
            MailHelper mailhelper = new MailHelper(mailbuy.Email, mailbuy.Password);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 11.4, 36.5);
            KAutoHelper.ADBHelper.InputText(deviceID, mailbuy.Email);
            Thread.Sleep(200);
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 88.3, 46.0);
            Thread.Sleep(15000);
            string otp2 = mailhelper.getOTPHotmail();
            int countemailotp = 0;
            while (otp2 == null && countemailotp < 1)
            {
                Thread.Sleep(40000);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 88.3, 46.0); // gui otp ve mail
                Thread.Sleep(12000);
                otp2 = mailhelper.getOTPHotmail();
                if (otp2 != null)
                {
                    break;
                }
                else
                {
                    countemailotp++;
                }
            }
            KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.1, 46.8);
            if (otp2 != null)
            {

                KAutoHelper.ADBHelper.InputText(deviceID, otp2);
                Thread.Sleep(500);
                KAutoHelper.ADBHelper.TapByPercent(deviceID, 51.4, 59.0);
                Thread.Sleep(5000);
                return mailbuy;
            }
            else
            {
                return null;
            }
        }
        
        private void ClickImgForTime(Bitmap Img, int time)
        {
            int timecount = 0;
            while (timecount < time)
            {
                if (IshaveImg(Img))
                {
                  //  Tapimg(Img);
                    break;
                }
                else
                {
                    timecount++;
                    Thread.Sleep(1000);
                }
            }

        }
        void Tapimg(Bitmap ImgFind)
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
        public bool IshaveImg(Bitmap ImgFind)
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
        void sendTen(string name)
        {
            string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(name));
            KAutoHelper.ADBHelper.ExecuteCMD("adb -s " + deviceID + " shell ime set com.android.adbkeyboard/.AdbIME");
            Thread.Sleep(1000);
            KAutoHelper.ADBHelper.ExecuteCMD("adb -s " + deviceID + " shell am broadcast -a ADB_INPUT_B64 --es msg " + str);
        }
    }
}
