using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace SendSms
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 4)
            {
                string myNum = args[0];
                string myPass = args[1];
                string toNum = args[2];
                string msg = HttpUtility.UrlEncode(args[3]);

                string url = string.Format("http://www.websafeguard.cn/action/sendfetion?sfetion={0}&password={1}&tfetion={2}&message={3}",myNum,myPass,toNum,msg);
                Console.WriteLine("Request:");
                Console.WriteLine(url);
                HttpWebResponse response = (HttpWebResponse)(WebRequest.Create(url).GetResponse());
                response.Close();
                Console.WriteLine("Send success!");

            }
            else
            {
                Console.WriteLine("sendsms fromNum fromPass toNum msg");
            }
        }
    }
}
