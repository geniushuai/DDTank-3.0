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

namespace Bussiness
{
    public class BaseBussiness : IDisposable
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected Sql_DbObject db;
        public BaseBussiness()
        {
            db = new Sql_DbObject("AppConfig", "conString");
        }

        public DataTable GetPage(string queryStr, string queryWhere,int pageCurrent, int pageSize,string fdShow,string fdOreder,string fdKey, ref int total)
        {
            try
            {
                SqlParameter[] para = new SqlParameter[8];
                para[0] = new SqlParameter("@QueryStr", queryStr);
                para[1] = new SqlParameter("@QueryWhere", queryWhere);
                para[2] = new SqlParameter("@PageSize", pageSize);
                para[3] = new SqlParameter("@PageCurrent", pageCurrent);
                para[4] = new SqlParameter("@FdShow", fdShow);
                para[5] = new SqlParameter("@FdOrder", fdOreder);
                para[6] = new SqlParameter("@FdKey", fdKey);
                para[7] = new SqlParameter("@TotalRow", total);
                para[7].Direction = ParameterDirection.Output;
                DataTable dt = db.GetDataTable(queryStr, "SP_CustomPage", para, 120);
                total = (int)para[7].Value;
                return dt;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("Init", e);
            }
            finally
            {
            }

            return new DataTable(queryStr);
        }


        public void Dispose()
        {
            db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
