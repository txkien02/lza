using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegAccLzd
{
    public class RandomHelper
    {
        public string randomNum(int stringlength) //
        {
            Random r = new Random();
            string chars = "0123458245012356789abcdeftriuytbybbghijkhfghfglmnopqfsdretervdfvdfrstuvwxtz";
            string nums = null;
            for (int i = 0; i < stringlength; i++)
            {
                nums += chars.Substring(r.Next(0, chars.Length - 1), 1);
            }
            return nums;
        }
        public List<String> getAccNox()
        {
            List<String> listAccount = new List<String>();
            StreamReader read = new StreamReader($"25000ten.txt");
            String data = read.ReadToEnd();
            read.Close();
            if (data == "")
            {

            }
            else
            {
                String[] arrData = data.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                for (int i = 0; i < arrData.Length; i++)
                {
                    listAccount.Add(arrData[i]);
                }
            }
            return listAccount;
        }
        public String getName()
        {

            List<String> listAccount = getAccNox();
            Random r = new Random();
            int a = r.Next(0, listAccount.Count);
            return listAccount[a];
        }
    }
}
