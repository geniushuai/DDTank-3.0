using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlDataProvider.Data;
using System.Data.SqlClient;
using SqlDataProvider.BaseClass;
using System.Data;
namespace Bussiness
{
    public class ItemRecordBussiness : BaseBussiness
    {
   

        public  void  PropertyString(ItemInfo item, ref string Property)
        {
            if (item != null)
                Property = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", item.StrengthenLevel, item.Attack, item.Defence,
                           item.Agility, item.Luck, item.AttackCompose, item.DefendCompose, item.AgilityCompose, item.LuckCompose);
        
        }

        public void FusionItem(ItemInfo item, ref string Property)
        {
            if (item != null)
                Property += string.Format("{0}:{1},{2}", item.ItemID,item.Template.Name, Convert.ToInt32(item.IsBinds)) + "|";
        }

        /// <summary>
        /// 铁匠铺记录保存到数据库中
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool LogItemDb(DataTable dt)
        {
            bool result = false;
            if (dt == null)
                return result;
            System.Data.SqlClient.SqlBulkCopy sqlbulk = new System.Data.SqlClient.SqlBulkCopy(System.Configuration.ConfigurationSettings.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
            try
            {                
                sqlbulk.NotifyAfter = dt.Rows.Count;
                sqlbulk.DestinationTableName = "Log_Item";
                sqlbulk.ColumnMappings.Add(0, "ApplicationId");
                sqlbulk.ColumnMappings.Add(1, "SubId");
                sqlbulk.ColumnMappings.Add(2, "LineId");
                sqlbulk.ColumnMappings.Add(3, "EnterTime");
                sqlbulk.ColumnMappings.Add(4, "UserId");
                sqlbulk.ColumnMappings.Add(5, "Operation");
                sqlbulk.ColumnMappings.Add(6, "ItemName");
                sqlbulk.ColumnMappings.Add(7, "ItemID");
                sqlbulk.ColumnMappings.Add(8, "AddItem");
                sqlbulk.ColumnMappings.Add(9, "BeginProperty");
                sqlbulk.ColumnMappings.Add(10, "EndProperty");
                sqlbulk.ColumnMappings.Add(11, "Result");
                sqlbulk.WriteToServer(dt);                
                result = true;
                dt.Clear();
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Smith Log Error:" + ex.ToString());
            }
            finally
            {
                sqlbulk.Close();
            }
            return result;
        }

        /// <summary>
        /// 用户消费日志
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool LogMoneyDb(DataTable dt)
        {
            bool result = false;
            if (dt == null)
                return result;
            System.Data.SqlClient.SqlBulkCopy sqlbulk = new System.Data.SqlClient.SqlBulkCopy(System.Configuration.ConfigurationSettings.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
            try
            {                
                sqlbulk.NotifyAfter = dt.Rows.Count;
                sqlbulk.DestinationTableName = "Log_Money";
                sqlbulk.ColumnMappings.Add(0, "ApplicationId");
                sqlbulk.ColumnMappings.Add(1, "SubId");
                sqlbulk.ColumnMappings.Add(2, "LineId");
                sqlbulk.ColumnMappings.Add(3, "MastType");
                sqlbulk.ColumnMappings.Add(4, "SonType");
                sqlbulk.ColumnMappings.Add(5, "UserId");
                sqlbulk.ColumnMappings.Add(6, "EnterTime");
                sqlbulk.ColumnMappings.Add(7, "Moneys");
                sqlbulk.ColumnMappings.Add(8, "Gold");
                sqlbulk.ColumnMappings.Add(9, "GiftToken");
                sqlbulk.ColumnMappings.Add(10, "Offer");
                sqlbulk.ColumnMappings.Add(11, "OtherPay");
                sqlbulk.ColumnMappings.Add(12, "GoodId");
                sqlbulk.ColumnMappings.Add(13, "ShopId");
                sqlbulk.ColumnMappings.Add(14, "Datas");
                sqlbulk.WriteToServer(dt);

                //ApplicationId	int	Unchecked
                //SubId	int	Checked
                //LineId	int	Checked
                //MastType	int	Checked
                //SonType	int	Checked
                //UserId	int	Checked
                //EnterTime	datetime	Checked
                //Moneys	int	Checked
                //Gold	int	Checked
                //GiftToken	int	Checked
                //Offer	int	Checked
                //OtherPay	varchar(400)	Checked
                //GoodId	int	Checked
                //ShopId	int	Checked
                //Datas	int	Checked
                result = true;
                
            }
            catch (Exception ex)
            {
                //TrieuLSL
                //if (log.IsErrorEnabled)                
                //    log.Error("Money Log Error:" + ex.ToString());
                
            }
            finally
            {
                sqlbulk.Close();
                dt.Clear();
            }
            return result;
        }

        /// <summary>
        /// 游戏战斗日志
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool LogFightDb(DataTable dt)
        {
            bool result = false;
            if (dt == null)
                return result;
            System.Data.SqlClient.SqlBulkCopy sqlbulk = new System.Data.SqlClient.SqlBulkCopy(System.Configuration.ConfigurationSettings.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
            try
            {
                sqlbulk.NotifyAfter = dt.Rows.Count;
                sqlbulk.DestinationTableName = "Log_Fight";
                sqlbulk.ColumnMappings.Add(0, "ApplicationId");
                sqlbulk.ColumnMappings.Add(1, "SubId");
                sqlbulk.ColumnMappings.Add(2, "LineId");
                sqlbulk.ColumnMappings.Add(3, "RoomId");
                sqlbulk.ColumnMappings.Add(4, "RoomType");
                sqlbulk.ColumnMappings.Add(5, "FightType");
                sqlbulk.ColumnMappings.Add(6, "ChangeTeam");
                sqlbulk.ColumnMappings.Add(7, "PlayBegin");
                sqlbulk.ColumnMappings.Add(8, "PlayEnd");
                sqlbulk.ColumnMappings.Add(9, "UserCount");
                sqlbulk.ColumnMappings.Add(10, "MapId");
                sqlbulk.ColumnMappings.Add(11, "TeamA");
                sqlbulk.ColumnMappings.Add(12, "TeamB");
                sqlbulk.ColumnMappings.Add(13, "PlayResult");
                sqlbulk.ColumnMappings.Add(14, "WinTeam");
                sqlbulk.ColumnMappings.Add(15, "Detail");
                sqlbulk.WriteToServer(dt);
                result = true;
                
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Fight Log Error:" + ex.ToString());
            }
            finally
            {
                sqlbulk.Close();
                dt.Clear();
            }
            return result;
        }

        /// <summary>
        /// 服务器在线人数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool LogServerDb(DataTable dt)
        {
            bool result = false;
            if (dt == null)
                return result;
            System.Data.SqlClient.SqlBulkCopy sqlbulk = new System.Data.SqlClient.SqlBulkCopy(System.Configuration.ConfigurationSettings.AppSettings["countDb"], SqlBulkCopyOptions.UseInternalTransaction);
            try
            {
                sqlbulk.NotifyAfter = dt.Rows.Count;
                sqlbulk.DestinationTableName = "Log_Server";
                sqlbulk.ColumnMappings.Add(0, "ApplicationId");
                sqlbulk.ColumnMappings.Add(1, "SubId");
                sqlbulk.ColumnMappings.Add(2, "EnterTime");
                sqlbulk.ColumnMappings.Add(3, "Online");
                sqlbulk.ColumnMappings.Add(4, "Reg");
                sqlbulk.WriteToServer(dt);
                result = true;
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("Server Log Error:" + ex.ToString());
            }
            finally
            {
                sqlbulk.Close();
                dt.Clear();
            }
            return result;
        }
    }

}
