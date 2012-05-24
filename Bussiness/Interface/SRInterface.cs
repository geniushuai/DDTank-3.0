using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;

namespace Bussiness.Interface
{
    public class SRInterface:BaseInterface
    {
        //public override PlayerInfo Login(string name, string password,ref string message,ref bool isFirst,string IP)
        //{
        //    try
        //    {
        //        WebLogin.PassPortSoapClient login = new WebLogin.PassPortSoapClient();
        //        int result = int.Parse(login.ChenckValidate(name, password));

        //        if (result != 0)
        //        {
        //            bool sex = (bool)login.Get_UserSex(name);
        //            using (PlayerBussiness db = new PlayerBussiness())
        //            {
        //                PlayerInfo info = db.LoginWeb(name, password, ref isFirst);
        //                if (info == null)
        //                {
        //                    if (!db.ActivePlayer(ref info, name, password, sex, ActiveGold, ActiveMoney,IP))
        //                    {
        //                        info = null;
        //                        message = "用户激活失败!";
        //                    }
        //                }
        //                return info;
        //            }                    
        //        }
        //        else
        //        {
        //            message = "用户名或密码错误!";
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
        //    return base.UnEncryptCharge(content);
        //}

        public override bool GetUserSex(string name)
        {
            try
            {
                WebLogin.PassPortSoapClient login = new WebLogin.PassPortSoapClient();
                return (bool)login.Get_UserSex(string.Empty,name);
            }
            catch (Exception ex)
            {
                log.Error("获取性别失败", ex);
                return true;
            }
        }
    }
}
