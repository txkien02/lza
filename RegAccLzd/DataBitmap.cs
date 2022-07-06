using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegAccLzd
{
    public class DataBitmap
    {
        public Bitmap SlideToContinuePhone;
        public Bitmap checkPhoneNumberPhone;
        public Bitmap XacMinhDiDong;
        public Bitmap ButtonSwipePhone;
        public Bitmap SettingPhone;
        public Bitmap OpenPhone;
        public Bitmap buyMoreH5Phone;
        public Bitmap ThemEmailPhone;
        public Bitmap report;


        public DataBitmap()
        {
            SlideToContinuePhone = (Bitmap)Bitmap.FromFile("Img//SlideToContinuePhone.png");
            checkPhoneNumberPhone = (Bitmap)Bitmap.FromFile("Img//checkPhoneNumberPhone.png");
            XacMinhDiDong = (Bitmap)Bitmap.FromFile("Img//xacminhdidong.png");
            ButtonSwipePhone = (Bitmap)Bitmap.FromFile("Img//ButtonSwipePhone.png");
            SettingPhone = (Bitmap)Bitmap.FromFile("Img//SettingPhone.png");
            OpenPhone = (Bitmap)Bitmap.FromFile("Img//OpenPhone.png");
            buyMoreH5Phone = (Bitmap)Bitmap.FromFile("Img//buyMoreH5Phone.png");
            ThemEmailPhone = (Bitmap)Bitmap.FromFile("Img//ThemEmailPhone.png");
            report = (Bitmap)Bitmap.FromFile("Img//report.png");
        }
    }
}
