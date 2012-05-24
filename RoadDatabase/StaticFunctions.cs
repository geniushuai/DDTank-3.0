using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Data;

namespace RoadDatabase
{
    public static class StaticFunctions
    {
        public static IAsyncResult BeginExecuteQuery(this DataContext dataContext, IQueryable query, bool withNoLock, AsyncCallback callback, object asyncState)
        {
            SqlCommand command = dataContext.GetCommand(query, withNoLock);

            AsyncResult<DbDataReader> asyncResult =
                new AsyncResult<DbDataReader>(asyncState);
            command.BeginExecuteReader(ar =>
            {
                try
                {
                    asyncResult.Result = command.EndExecuteReader(ar);
                }
                catch (Exception e)
                {
                    asyncResult.Exception = e;
                }
                finally
                {
                    asyncResult.Complete();
                    if (callback != null) callback(asyncResult);
                }
            }, null);

            return asyncResult;
        }

        public static List<T> EndExecuteQuery<T>(this DataContext dataContext, IAsyncResult ar)
        {
            AsyncResult<DbDataReader> asyncResult = (AsyncResult<DbDataReader>)ar;

            if (!asyncResult.IsCompleted)
            {
                asyncResult.AsyncWaitHandle.WaitOne();
            }

            if (asyncResult.Exception != null)
            {
                throw asyncResult.Exception;
            }

            using (DbDataReader reader = asyncResult.Result)
            {
                return dataContext.Translate<T>(reader).ToList();
            }
        }

        public static List<T> ExecuteQuery<T>(this DataContext dataContext, IQueryable query, bool withNoLock)
        {
            DbCommand command = dataContext.GetCommand(query, withNoLock);

            dataContext.OpenConnection();

            using (DbDataReader reader = command.ExecuteReader())
            {
                return dataContext.Translate<T>(reader).ToList();
            }
        }

        private static Regex s_withNoLockRegex =
            new Regex(@"(] AS \[t\d+\])", RegexOptions.Compiled);

        private static string AddWithNoLock(string cmdText)
        {
            IEnumerable<Match> matches =
                s_withNoLockRegex.Matches(cmdText).Cast<Match>()
                .OrderByDescending(m => m.Index);
            foreach (Match m in matches)
            {
                int splitIndex = m.Index + m.Value.Length;
                cmdText =
                    cmdText.Substring(0, splitIndex) + " WITH (NOLOCK)" +
                    cmdText.Substring(splitIndex);
            }

            return cmdText;
        }

        private static SqlCommand GetCommand(
            this DataContext dataContext, IQueryable query, bool withNoLock)
        {
            SqlCommand command = (SqlCommand)dataContext.GetCommand(query);

            if (withNoLock)
            {
                command.CommandText = AddWithNoLock(command.CommandText);
            }

            return command;
        }

        public static List<T> ExecuteQuery<T>(this DataContext dataContext, IQueryable query)
        {
            DbCommand command = dataContext.GetCommand(query);
            dataContext.OpenConnection();

            using (DbDataReader reader = command.ExecuteReader())
            {
                return dataContext.Translate<T>(reader).ToList();
            }
        }

        private static void OpenConnection(this DataContext dataContext)
        {
            if (dataContext.Connection.State == ConnectionState.Closed)
            {
                dataContext.Connection.Open();
            }
        }

        public static void SubmitChangesWithOutError(this DataContext ctx)
        {
            try
            {
                ctx.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException)
            {

                foreach (ObjectChangeConflict cc in ctx.ChangeConflicts)
                {
                    cc.Resolve(RefreshMode.OverwriteCurrentValues); // 放弃当前更新，所有更新以原先更新为准
                }

            }

            ctx.SubmitChanges();   
        }

    }
}
