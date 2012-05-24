using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;

namespace Bussiness.Interface
{
    public class QYInterface : BaseInterface
    {
        //public override PlayerInfo Login(string name, string password, ref string message, ref bool isFirst,string IP)
        //{
        //    try
        //    {
        //        int time = ConvertDateTimeInt(DateTime.Now);
        //        string pass = md5(password);
        //        string v = md5(name + pass + time.ToString() + GetLoginKey);
        //        string Url = LoginUrl + "?u=" + name + "&p=" + pass + "&t=" + time.ToString() + "&v=" + v;
        //        string result = RequestContent(Url);

        //        switch (result)
        //        {
        //            case "state=100":
        //                using (PlayerBussiness db = new PlayerBussiness())
        //                {
        //                    PlayerInfo info = db.LoginWeb(name, password, ref isFirst);
        //                    if (info == null)
        //                    {
        //                        if (!db.ActivePlayer(ref info, name, password, true, ActiveGold, ActiveMoney,IP))
        //                        {
        //                            info = null;
        //                            message = "用户激活失败!";
        //                        }
        //                    }
        //                    return info;
        //                }
        //            default:
        //                message = "用户名或密码错误!";
        //                break;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("登陆失败", ex);
        //    }
        //    return null;
        //}

        //public override string[] UnEncryptCharge(string content)
        //{
        //    try
        //    {
        //        string[] str = content.Split('|');

        //        if (str.Length == 6)
        //        {
        //            string v = md5(str[0] + str[1] + str[2] + str[3] + str[4] + GetChargeKey);
        //            if (v == str[5])
        //                return str;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("充值失败", ex);
        //    }

        //    return new string[0];
        //}

    }
}
