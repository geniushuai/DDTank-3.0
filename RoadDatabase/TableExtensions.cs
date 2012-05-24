using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Linq.Expressions;

namespace RoadDatabase
{
    public static class TableExtensions
    {
        public static int Delete<TEntity>(this Table<TEntity> table, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            string tableName = table.Context.Mapping.GetTable(typeof(TEntity)).TableName;
            string command = String.Format("DELETE FROM {0}", tableName);

            ConditionBuilder conditionBuilder = new ConditionBuilder();
            conditionBuilder.Build(predicate.Body);

            if (!String.IsNullOrEmpty(conditionBuilder.Condition))
            {
                command += " WHERE " + conditionBuilder.Condition;
            }

            return table.Context.ExecuteCommand(command, conditionBuilder.Arguments);
        }

        //public static int Update<TEntity>(this Table<TEntity> table,
        //    Expression<Func<TEntity, TEntity>> evaluator, Expression<Func<TEntity, bool>> predicate)
        //    where TEntity : class
        //{
        //    return 0;
        //}
    }
}
