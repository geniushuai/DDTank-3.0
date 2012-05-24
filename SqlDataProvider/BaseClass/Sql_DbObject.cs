using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Xml;

using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Sql;

namespace SqlDataProvider.BaseClass
{
    #region Sql_DbObject数据库操作底层类
    public sealed class Sql_DbObject : IDisposable
    {
        #region 1、定义变量
        System.Data.SqlClient.SqlConnection _SqlConnection;
        System.Data.SqlClient.SqlCommand _SqlCommand;
        System.Data.SqlClient.SqlDataAdapter _SqlDataAdapter;
        #endregion

        #region 2、构造函数
        public Sql_DbObject()
        {
            _SqlConnection = new SqlConnection();
        }

        public Sql_DbObject(string Path_Source, string Conn_DB)
        {
            switch (Path_Source)
            {
                case "WebConfig":
                    _SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[Conn_DB].ConnectionString);                    
                    break;
                case "File":
                    _SqlConnection = new SqlConnection(Conn_DB);
                    break;
                case "AppConfig":
                    string str = System.Configuration.ConfigurationSettings.AppSettings[Conn_DB];
                    _SqlConnection = new SqlConnection(str);
                    break;
                default:
                    _SqlConnection = new SqlConnection(Conn_DB);
                    break;
            }
        }
        #endregion

        #region 3、打开一个数据库连接
        private static bool OpenConnection(System.Data.SqlClient.SqlConnection _SqlConnection)
        {
            bool result = false;
            try
            {
                if (_SqlConnection.State != ConnectionState.Open)
                {
                    _SqlConnection.Open();
                    result = true;
                }
                else
                {
                    result = true;
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                ApplicationLog.WriteError("打开数据库连接错误:" + ex.Message.Trim());
                result = false;
            }

            return result;
        }
        #endregion

        #region 4、执行sql语句，返回bool型执行结果
        /// <summary>
        ///  执行sql语句，返回bool型执行结果
        /// </summary>
        /// <param name="Sqlcomm">sql语句</param>
        /// <returns>bool型变量</returns>
        public bool Exesqlcomm(string Sqlcomm)
        {
            if (!OpenConnection(this._SqlConnection)) return false;

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.CommandType = CommandType.Text;
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandText = Sqlcomm;
                this._SqlCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行sql语句: " + Sqlcomm + "错误信息为: " + ex.Message.Trim());
                return false;
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }

            return true;
        }
        #endregion

        #region 5、执行sql语句，返回int型第一行第一列数值
        /// <summary>
        /// 执行sql语句，返回第一行第一列
        /// </summary>
        /// <param name="Sqlcomm">sql语句</param>
        /// <returns>返回的第一行第一列的数值</returns>
        public int GetRecordCount(string Sqlcomm)
        {
            int retval = 0;

            if (!OpenConnection(this._SqlConnection))
            {
                retval = 0;
            }
            else
            {
                try
                {
                    this._SqlCommand = new SqlCommand();
                    this._SqlCommand.Connection = this._SqlConnection;
                    this._SqlCommand.CommandType = CommandType.Text;
                    this._SqlCommand.CommandText = Sqlcomm;

                    if (this._SqlCommand.ExecuteScalar() == null) { retval = 0; }
                    else { retval = (int)this._SqlCommand.ExecuteScalar(); }
                }
                catch (SqlException ex)
                {
                    ApplicationLog.WriteError("执行sql语句: " + Sqlcomm + "错误信息为: " + ex.Message.Trim());
                }
                finally
                {
                    this._SqlConnection.Close();
                    this.Dispose(true);
                }
            }

            return retval;
        }
        #endregion

        #region 6、执行sql语句，返回DataTable类型数据集
        /// <summary>
        /// 执行sql语句。返回DataTable类型数据集
        /// </summary>
        /// <param name="TableName">数据表名</param>
        /// <param name="Sqlcomm">sql语句</param>
        /// <returns>返回DataTable类型数据集</returns>
        public DataTable GetDataTableBySqlcomm(string TableName, string Sqlcomm)
        {
            System.Data.DataTable ResultTable = new DataTable(TableName);

            if (!OpenConnection(this._SqlConnection)) return ResultTable;

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.Text;
                this._SqlCommand.CommandText = Sqlcomm;

                this._SqlDataAdapter = new SqlDataAdapter();
                this._SqlDataAdapter.SelectCommand = this._SqlCommand;

                this._SqlDataAdapter.Fill(ResultTable);
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行sql语句: " + Sqlcomm + "错误信息为: " + ex.Message.Trim());
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }

            return ResultTable;
        }
        #endregion

        #region 7、执行Sql语句，返回DataSet类型数据集
        /// <summary>
        /// 返回一个DataSet 
        /// </summary>
        /// <param name="TableName">传入表名</param>
        /// <param name="Sqlcomm">执行查询语句</param>
        /// <returns></returns>
        public DataSet GetDataSetBySqlcomm(string TableName, string Sqlcomm)
        {
            System.Data.DataSet ResultTable = new DataSet();
            if (!OpenConnection(this._SqlConnection)) return ResultTable;
            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.Text;
                this._SqlCommand.CommandText = Sqlcomm;
                this._SqlDataAdapter = new SqlDataAdapter();
                this._SqlDataAdapter.SelectCommand = this._SqlCommand;
                this._SqlDataAdapter.Fill(ResultTable);
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行Sql语句：" + Sqlcomm + "错误信息为：" + ex.Message.Trim());
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }
            return ResultTable;
        }
        #endregion

        #region 8、执行一个SQL语句 返回 MySqlDataReader数据集
        public bool FillSqlDataReader(ref SqlDataReader Sdr, string SqlComm)
        {
            if (!OpenConnection(this._SqlConnection))
            {
                return false;
            }
            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.Text;
                this._SqlCommand.CommandText = SqlComm;
                Sdr = this._SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                return true;
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行Sql语句：" + SqlComm + "错误信息为：" + ex.Message.Trim());

            }
            finally
            {

                this.Dispose(true);
            }
            return false;
        }
        #endregion

        #region 9、执行sql语句，返回DataTable类型数据集，提供分页
        /// <summary>
        /// 执行sql语句，返回DataTable类型数据集，提供分页
        /// </summary>
        /// <param name="TableName">数据表名</param>
        /// <param name="ProcedureName">sql语句</param>
        /// <param name="StartRecordNo">开始行数</param>
        /// <param name="PageSize">一页的大小</param>
        /// <returns>返回分页DataTable类型数据集</returns>
        public DataTable GetDataTableBySqlcomm(string TableName, string Sqlcomm, int StartRecordNo, int PageSize)
        {
            DataTable RetTable = new DataTable(TableName);

            if (!OpenConnection(this._SqlConnection))
            {
                RetTable.Dispose();
                this.Dispose(true);
                return RetTable;
            }

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.Text;
                this._SqlCommand.CommandText = Sqlcomm;

                this._SqlDataAdapter = new SqlDataAdapter();
                this._SqlDataAdapter.SelectCommand = this._SqlCommand;

                DataSet ds = new DataSet();
                ds.Tables.Add(RetTable);

                this._SqlDataAdapter.Fill(ds, StartRecordNo, PageSize, TableName);
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行sql语句: " + Sqlcomm + "错误信息为: " + ex.Message.Trim());
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }

            return RetTable;
        }
        #endregion

        #region 10、执行带参数存储过程，返回执行的bool型结果
        /// <summary>
        /// 执行带参数存储过程，返回执行的bool型结果
        /// </summary>
        /// <param name="ProcedureName">存储过程名</param>
        /// <param name="SqlParameters">参数数组</param>
        /// <returns>执行的bool型结果</returns>
        public bool RunProcedure(string ProcedureName, SqlParameter[] SqlParameters)
        {
            if (!OpenConnection(this._SqlConnection)) return false;

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;

                foreach (SqlParameter parameter in SqlParameters)
                {
                    this._SqlCommand.Parameters.Add(parameter);
                }

                this._SqlCommand.ExecuteNonQuery();
                //this._SqlCommand.BeginExecuteNonQuery(RunProcedureCallback, null);
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
                return false;
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }

            return true;
        }
        

        #endregion

        #region 11、执行不带参数存储过程，返回执行的bool型结果
        /// <summary>
        /// 执行不带参数存储过程，返回执行的bool型结果
        /// </summary>
        /// <param name="ProcedureName">存储过程名</param>
        /// <returns>执行的bool型结果</returns>
        public bool RunProcedure(string ProcedureName)
        {
            if (!OpenConnection(this._SqlConnection)) return false;

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;

                this._SqlCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
                return false;
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }

            return true;
        }
        #endregion

        #region 12、执行无参数存储过程，得到SqlDataReader类型数据集，返回bool型执行结果
        /// <summary>
        /// 执行无参数存储过程，得到SqlDataReader类型数据集，返回bool型执行结果  
        /// </summary>
        /// <param name="ResultDataReader">SqlDataReader类型数据集名称</param>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <returns>返回bool型执行结果</returns>
        public bool GetReader(ref SqlDataReader ResultDataReader, string ProcedureName)
        {
            if (!OpenConnection(this._SqlConnection)) return false;

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;

                ResultDataReader = this._SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
                return false;
            }

            return true;
        }
        #endregion

        #region 13、 执行有参数存储过程，得到SqlDataReader类型数据集，返回bool型执行结果
        /// <summary>
        /// 执行有参数存储过程，得到SqlDataReader类型数据集，返回bool型执行结果
        /// </summary>
        /// <param name="ResultDataReader">SqlDataReader类型数据集名称</param>
        /// <param name="ProcedureName">存储过程名</param>
        /// <param name="SqlParameters">存储过程参数数组</param>
        /// <returns>返回bool型执行结果</returns>
        public bool GetReader(ref SqlDataReader ResultDataReader, string ProcedureName, SqlParameter[] SqlParameters)
        {
            if (!OpenConnection(this._SqlConnection)) return false;

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;

                foreach (SqlParameter parameter in SqlParameters)
                {
                    this._SqlCommand.Parameters.Add(parameter);
                }


                ResultDataReader = this._SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
                return false;
            }

            return true;
        }
        #endregion

        #region 14、执行带参数的存储过程,返回DataSet类型数据集
        /// <summary>
        /// 执行带参数存储过程，返回DataSet类型数据集
        /// </summary>
        /// <param name="ProcedureName">存储过程名</param>
        /// <param name="MySqlParameters">存储过程参数</param>
        /// <returns>返回DataSet</returns>
        public DataSet GetDataSet(string ProcedureName, SqlParameter[] SqlParameters)
        {
            System.Data.DataSet FullDataSet = new DataSet();
            if (!OpenConnection(this._SqlConnection))
            {
                FullDataSet.Dispose();
                return FullDataSet;
            }
            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;
                foreach (SqlParameter parameter in SqlParameters)
                {
                    this._SqlCommand.Parameters.Add(parameter);
                }
                this._SqlDataAdapter = new SqlDataAdapter();
                this._SqlDataAdapter.SelectCommand = this._SqlCommand;
                this._SqlDataAdapter.Fill(FullDataSet);
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程：" + ProcedureName + "错信信息为：" + ex.Message.Trim());
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);

            }
            return FullDataSet;
        }
        #endregion

        #region 15、执行有参数存储过程，得到分页DataSet类型数据集，返回BOOL型执行结果
        /// <summary>
        /// 执行有参数存储过程，得到分页DataSet类型数据集，返回Bool型执行结果
        /// </summary>
        /// <param name="ResultDataSet">分页DataSet类型数据集</param>
        /// <param name="TableName">数据表名</param>
        /// <param name="ProcedureName">存储过程名</param>
        /// <param name="StartRecordNo">开始行数</param>
        /// <param name="PageSize">一页的大小</param>
        /// <param name="MySqlParameters">参数数组</param>
        /// <returns>返回BOOL型执行结果</returns>       
        public bool GetDataSet(ref DataSet ResultDataSet, ref int row_total, String TableName, string ProcedureName, int StartRecordNo, int PageSize, SqlParameter[] SqlParameters)
        {
            if (!OpenConnection(this._SqlConnection)) return false;
            try
            {
                row_total = 0;
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;

                foreach (SqlParameter parameter in SqlParameters)
                {
                    this._SqlCommand.Parameters.Add(parameter);
                }

                this._SqlDataAdapter = new SqlDataAdapter();
                this._SqlDataAdapter.SelectCommand = this._SqlCommand;
                DataSet ds = new DataSet();
                row_total = this._SqlDataAdapter.Fill(ds);
                this._SqlDataAdapter.Fill(ResultDataSet, StartRecordNo, PageSize, TableName);
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程：" + ProcedureName + "错误信息为：" + ex.Message.Trim());
                return false;
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }
            return true;
        }
        #endregion

        #region 16、执行带参数存储过程，返回DataSet类型数据集
        /// <summary>
        /// 执行带参数存储过程，返回DataSet类型数据集
        /// </summary>
        /// <param name="TableName">数据表名称</param>
        /// <param name="ProcedureName">存储过程名</param>
        /// <param name="MySqlParameters">参数数组</param>
        /// <returns>返回DataSet类型数据集</returns>
        public DataSet GetDateSet(string DatesetName, string ProcedureName, SqlParameter[] SqlParameters)
        {
            System.Data.DataSet FullDataSet = new DataSet(DatesetName);
            if (!OpenConnection(this._SqlConnection))
            {
                FullDataSet.Dispose();
                return FullDataSet;
            }
            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;

                foreach (SqlParameter parameter in SqlParameters)
                {
                    this._SqlCommand.Parameters.Add(parameter);
                }
                this._SqlDataAdapter = new SqlDataAdapter();
                this._SqlDataAdapter.SelectCommand = this._SqlCommand;
                this._SqlDataAdapter.Fill(FullDataSet);
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程：" + ProcedureName + "错信信息为：" + ex.Message.Trim());
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);

            }
            return FullDataSet;
        }
        #endregion

        #region 17、执行有参数存储过程，返回DataTable类型数据集
        /// <summary>
        /// 执行有参数存储过程，返回DataTable类型数据集
        /// </summary>
        /// <param name="TableName">数据表名称</param>
        /// <param name="ProcedureName">存储过程名</param>
        /// <param name="SqlParameters">参数数组</param>
        /// <returns>返回DataTable类型数据集</returns>
        public DataTable GetDataTable(string TableName, string ProcedureName, SqlParameter[] SqlParameters)
        {
            //System.Data.DataTable FullTable = new DataTable(TableName);

            //if (!OpenConnection(this._SqlConnection))
            //{
            //    FullTable.Dispose();
            //    this.Dispose(true);
            //    return FullTable;
            //}

            //try
            //{
            //    this._SqlCommand = new SqlCommand();
            //    this._SqlCommand.Connection = this._SqlConnection;
            //    this._SqlCommand.CommandType = CommandType.StoredProcedure;
            //    this._SqlCommand.CommandText = ProcedureName;

            //    foreach (SqlParameter parameter in SqlParameters)
            //    {
            //        this._SqlCommand.Parameters.Add(parameter);
            //    }

            //    this._SqlDataAdapter = new SqlDataAdapter();
            //    this._SqlDataAdapter.SelectCommand = this._SqlCommand;
            //    this._SqlDataAdapter.Fill(FullTable);
            //}
            //catch (SqlException ex)
            //{
            //    ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
            //}
            //finally
            //{
            //    this._SqlConnection.Close();
            //    this.Dispose(true);
            //}

            //return FullTable;

            return GetDataTable(TableName, ProcedureName, SqlParameters, -1);
        }

        /// <summary>
        /// 执行有参数存储过程，返回DataTable类型数据集
        /// </summary>
        /// <param name="TableName">数据表名称</param>
        /// <param name="ProcedureName">存储过程名</param>
        /// <param name="SqlParameters">参数数组</param>
        /// <returns>返回DataTable类型数据集</returns>
        public DataTable GetDataTable(string TableName, string ProcedureName, SqlParameter[] SqlParameters,int commandTimeout)
        {
            System.Data.DataTable FullTable = new DataTable(TableName);

            if (!OpenConnection(this._SqlConnection))
            {
                FullTable.Dispose();
                this.Dispose(true);
                return FullTable;
            }

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;
                if (commandTimeout >= 0)
                {
                    this._SqlCommand.CommandTimeout = commandTimeout;
                }

                foreach (SqlParameter parameter in SqlParameters)
                {
                    this._SqlCommand.Parameters.Add(parameter);
                }

                this._SqlDataAdapter = new SqlDataAdapter();
                this._SqlDataAdapter.SelectCommand = this._SqlCommand;
                this._SqlDataAdapter.Fill(FullTable);
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }

            return FullTable;
        }

        #endregion

        #region 18、 执行无参数存储过程，返回DataTable类型数据集
        /// <summary>
        /// 执行无参数存储过程，返回DataTable类型数据集
        /// </summary>
        /// <param name="TableName">数据表名称</param>
        /// <param name="ProcedureName">存储过程名</param>
        /// <returns>返回DataTable类型数据集</returns>
        public DataTable GetDataTable(string TableName, string ProcedureName)
        {

            System.Data.DataTable FullTable = new DataTable(TableName);

            if (!OpenConnection(this._SqlConnection))
            {
                FullTable.Dispose();
                this.Dispose(true);
                return FullTable;
            }

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;

                this._SqlDataAdapter = new SqlDataAdapter();
                this._SqlDataAdapter.SelectCommand = this._SqlCommand;

                this._SqlDataAdapter.Fill(FullTable);
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }

            return FullTable;
        }
        #endregion

        #region 19、执行无参数存储过程，返回DataTable类型数据集，提供分页
        /// <summary>
        /// 执行无参数存储过程，返回DataTable类型数据集，提供分页
        /// </summary>
        /// <param name="TableName">数据表名</param>
        /// <param name="ProcedureName">存储过程名</param>
        /// <param name="StartRecordNo">开始行数</param>
        /// <param name="PageSize">一页的大小</param>
        /// <returns>返回分页DataTable类型数据集</returns>
        public DataTable GetDataTable(string TableName, string ProcedureName, int StartRecordNo, int PageSize)
        {
            DataTable RetTable = new DataTable(TableName);

            if (!OpenConnection(this._SqlConnection))
            {
                RetTable.Dispose();
                this.Dispose(true);
                return RetTable;
            }

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;

                this._SqlDataAdapter = new SqlDataAdapter();
                this._SqlDataAdapter.SelectCommand = this._SqlCommand;

                DataSet ds = new DataSet();
                ds.Tables.Add(RetTable);

                this._SqlDataAdapter.Fill(ds, StartRecordNo, PageSize, TableName);
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }

            return RetTable;
        }
        #endregion

        #region 20、 执行有参数存储过程，返回DataTable类型数据集，提供分页
        /// <summary>
        /// 执行有参数存储过程，返回DataTable类型数据集，提供分页
        /// </summary>
        /// <param name="TableName">数据表名</param>
        /// <param name="ProcedureName">存储过程名</param>
        /// <param name="SqlParameters">参数数组</param>
        /// <param name="StartRecordNo">开始行数</param>
        /// <param name="PageSize">一页的大小</param>
        /// <returns>返回分页DataTable类型数据集</returns>
        public DataTable GetDataTable(string TableName, string ProcedureName, SqlParameter[] SqlParameters, int StartRecordNo, int PageSize)
        {

            DataTable RetTable = new DataTable(TableName);

            if (!OpenConnection(this._SqlConnection))
            {
                RetTable.Dispose();
                this.Dispose(true);
                return RetTable;
            }

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;

                foreach (SqlParameter parameter in SqlParameters)
                {
                    this._SqlCommand.Parameters.Add(parameter);
                }

                this._SqlDataAdapter = new SqlDataAdapter();
                this._SqlDataAdapter.SelectCommand = this._SqlCommand;

                DataSet ds = new DataSet();
                ds.Tables.Add(RetTable);

                this._SqlDataAdapter.Fill(ds, StartRecordNo, PageSize, TableName);
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }

            return RetTable;
        }
        #endregion

        #region 21、执行无参数存储过程，得到分页DataTable类型数据集，返回bool型执行结果
        /// <summary>
        /// 执行无参数存储过程，得到分页DataTable类型数据集，返回bool型执行结果
        /// </summary>
        /// <param name="ResultTable">分页DataTable类型数据集</param>
        /// <param name="TableName">数据表名</param>
        /// <param name="ProcedureName">存储过程名</param>
        /// <param name="StartRecordNo">开始行数</param>
        /// <param name="PageSize">一页的大小</param>
        /// <returns>返回bool型执行结果</returns>
        public bool GetDataTable(ref DataTable ResultTable, string TableName, string ProcedureName, int StartRecordNo, int PageSize)
        {
            ResultTable = null;

            if (!OpenConnection(this._SqlConnection)) return false;

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;

                this._SqlDataAdapter = new SqlDataAdapter();
                this._SqlDataAdapter.SelectCommand = this._SqlCommand;

                DataSet ds = new DataSet();
                ds.Tables.Add(ResultTable);

                this._SqlDataAdapter.Fill(ds, StartRecordNo, PageSize, TableName);
                ResultTable = ds.Tables[TableName];
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
                return false;
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }

            return true;
        }
        #endregion

        #region 22、执行有参数存储过程，得到分页DataTable类型数据集，返回bool型执行结果
        /// <summary>
        /// 执行有参数存储过程，得到分页DataTable类型数据集，返回bool型执行结果
        /// </summary>
        /// <param name="ResultTable">分页DataTable类型数据集</param>
        /// <param name="TableName">数据表名</param>
        /// <param name="ProcedureName">存储过程名</param>
        /// <param name="StartRecordNo">开始行数</param>
        /// <param name="PageSize">一页的大小</param>
        /// <param name="SqlParameters">参数数组</param>
        /// <returns>返回bool型执行结果</returns>
        public bool GetDataTable(ref DataTable ResultTable, string TableName, string ProcedureName, int StartRecordNo, int PageSize, SqlParameter[] SqlParameters)
        {
            if (!OpenConnection(this._SqlConnection)) return false;

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;

                foreach (SqlParameter parameter in SqlParameters)
                {
                    this._SqlCommand.Parameters.Add(parameter);
                }

                this._SqlDataAdapter = new SqlDataAdapter();
                this._SqlDataAdapter.SelectCommand = this._SqlCommand;

                DataSet ds = new DataSet();
                ds.Tables.Add(ResultTable);

                this._SqlDataAdapter.Fill(ds, StartRecordNo, PageSize, TableName);
                ResultTable = ds.Tables[TableName];
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
                return false;
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }

            return true;
        }
        #endregion

        #region 23、析构函数
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (this._SqlDataAdapter != null)
            {
                if (this._SqlDataAdapter.SelectCommand != null)
                {
                    if (this._SqlCommand.Connection != null)
                        this._SqlDataAdapter.SelectCommand.Connection.Dispose();
                    this._SqlDataAdapter.SelectCommand.Dispose();
                }
                this._SqlDataAdapter.Dispose();
                this._SqlDataAdapter = null;
            }
        }
        #endregion

        #region 24、异步执行带参数，不带返回结果的存储过程
        /// <summary>
        /// 异步执行带参数，不带返回结果的存储过程
        /// </summary>
        /// <param name="ProcedureName">存储过程名</param>
        public void BeginRunProcedure(string ProcedureName, SqlParameter[] SqlParameters)
        {
            if (!OpenConnection(this._SqlConnection)) return ;

            try
            {
                this._SqlCommand = new SqlCommand();
                this._SqlCommand.Connection = this._SqlConnection;
                this._SqlCommand.CommandType = CommandType.StoredProcedure;
                this._SqlCommand.CommandText = ProcedureName;

                foreach (SqlParameter parameter in SqlParameters)
                {
                    this._SqlCommand.Parameters.Add(parameter);
                }

                this._SqlCommand.BeginExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
            }
            finally
            {
                this._SqlConnection.Close();
                this.Dispose(true);
            }
        }
        #endregion

      


    }
    #endregion

    
}
