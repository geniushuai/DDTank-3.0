using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using SqlDataProvider.BaseClass;
using System.Data;
using System.Data.SqlClient;
using DAL;
using log4net;
using System.Reflection;
using log4net.Util;
using Bussiness.Managers;
using Bussiness.CenterService;

namespace Bussiness
{
    public class OrderBussiness : BaseBussiness
    {

        public bool AddOrder(string order, double amount, string username, string payWay, string serverId)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[6];
                para[0] = new SqlParameter("@Order", order);
                para[1] = new SqlParameter("@Amount", amount);
                para[2] = new SqlParameter("@Username", username);
                para[3] = new SqlParameter("@PayWay", payWay);
                para[4] = new SqlParameter("@ServerId", serverId);
                para[5] = new SqlParameter("@Result", System.Data.SqlDbType.Int);
                para[5].Direction = ParameterDirection.ReturnValue;
                db.RunProcedure("SP_Charge_Order", para);
                int returnValue = (int)para[5].Value;
                result = returnValue == 0;

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            return result;
        }

        public string GetOrderToName(string order, ref string serverId)
        {
            SqlDataReader reader = null;

            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@Order", order);

                db.GetReader(ref reader, "SP_Charge_Order_Single", para);
                while (reader.Read())
                {
                    serverId = reader["ServerId"] == null ? "" : reader["ServerId"].ToString();
                    string userName = reader["UserName"] == null ? "" : reader["UserName"].ToString();
                    return userName;
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return "";
        }
    }
}
