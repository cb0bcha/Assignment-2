using System;
using System.Data;
using System.Data.Common;

namespace Assignment2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
        }

        public static object[] GetRowWithDateTime(DbCommand command, string sql)
        {
            if (Logger.Default.IsTraceEnabled)
            {
                Log.Debug(sql);
            }

            ExecuteQuery(command, sql, false);
        }

        public static object[] GetRowWithDateTime(DbCommand command, string sql, DbCommon db, DbType[] types, object[] values)
        {
            if (Logger.Default.IsTraceEnabled)
            {
                Log.Debug(sql, types, values);
            }

            ExecuteQuery(command, sql, db, types, values, false);
        }

        public static object[] GetRowWithNoDateTime(DbCommand command, string sql)
        {
            if (Logger.Default.IsTraceEnabled)
            {
                Log.Debug(sql);
            }

            ExecuteQuery(command, sql, true);
        }

        public static object[] GetRowWithNoDateTime(DbCommand command, string sql, DbCommon db, DbType[] types, object[] values)
        {
            if (Logger.Default.IsTraceEnabled)
            {
                Log.Debug(sql, types, values);
            }

            ExecuteQuery(command, sql, db, types, values, true);
        }

        private static object[] ExecuteQuery(DbCommand command, string sql, bool noDateTime)
        {
            try
            {
                command.CommandText = sql;

                ExecuteReader(command, noDateTime);
            }
            catch (Exception ex)
            {
                Log.WarnFormat("ExecuteQuery({0}) failed: {1}", sql, ex.Message); // TODO - log stacktrace, types, values, noDateTime
                throw;
            }
        }

        private static object[] ExecuteQuery(DbCommand command, string sql, DbCommon db, DbType[] types, object[] values, bool noDateTime)
        {
            try
            {
                HookDbParameters(ref db, ref command, sql, types, values);

                ExecuteReader(command, noDateTime);
            }
            catch (Exception ex)
            {
                Log.WarnFormat("ExecuteQuery({0}) failed: {1}", sql, ex.Message); // TODO - log stacktrace, types, values, noDateTime
                throw;
            }
        }

        private static object[] ExecuteReader(DbCommand command, bool noDateTime)
        {
            using (var reader = command.ExecuteReader())
            {
                var length = reader.FieldCount;

                while (reader.Read())
                {
                    var row = new object[length];

                    for (var i = 0; i < length; i++)
                    {
                        row[i] = reader.IsDBNull(i) ? null : reader.GetValue(i);

                        if (noDateTime && row[i] is DateTime dt)
                        {
                            row[i] = dt.ToOADate();
                        }
                    }

                    return row;
                }
            }

            return null;
        }
    }
}