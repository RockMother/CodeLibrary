using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace TreeFrogs.SqlServer
{
    public sealed class ScriptEngine
    {
        #region Static
        private SqlCommand GetCommand(string script, SqlConnection conn)
        {
            SqlTransaction transaction = null;
            if (Connection == conn && transactions.Count != 0)
            {
                transaction = transactions.Peek();
            }
            return new SqlCommand(script, conn, transaction) {CommandTimeout = ExecutionTimeout};
        }

        private static int ExecutionTimeout
        {
            get { return 5000; }
        }

        private SqlCommand GetCommandWithParameters(string name,
                SqlConnection connection,
                CommandType type,
                params SqlParameter[] parameters)
        {
            var cmd = GetCommand(name, connection);
            cmd.CommandType = type;
            foreach (var parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }
            return cmd;
        }
        #endregion
        #region Private fields
        private readonly SqlConnectionStringBuilder connBuilder;
        private SqlConnection sqlConn;
        private readonly Stack<SqlTransaction> transactions;
        #endregion
        #region Constructors
        /// <summary>
        ///     Instance constructor
        /// </summary>
        /// <param name="connection"> </param>
        /// <exception cref="ArgumentNullException">Argument is null.</exception>
        internal ScriptEngine(SqlConnection connection)
                : this(connection.ConnectionString)
        {
            #region Check
            if (ReferenceEquals(connection, null))
            {
                throw new ArgumentNullException("connection");
            }
            #endregion
            sqlConn = connection;
        }
        /// <summary>
        ///     Instance constructor
        /// </summary>
        /// <param name="connString"> </param>
        /// <exception cref="ArgumentNullException">
        ///     <c>conn</c>
        ///     is null.
        /// </exception>
        public ScriptEngine(string connString)
        {
            #region Check
            if (string.IsNullOrEmpty(connString))
            {
                throw new ArgumentNullException("connString");
            }
            #endregion
            connBuilder = new SqlConnectionStringBuilder(connString);
            transactions = new Stack<SqlTransaction>();
        }
        #endregion
        #region Public properties
        public bool IsConnectionOpen
        {
            [DebuggerStepThrough]
            get { return !ReferenceEquals(Connection, null) && Connection.State == ConnectionState.Open; }
        }
        #endregion
        #region Public methods
        public void BeginTransaction()
        {
            if (Connection == null)
            {
                throw new ApplicationException("Open connection first");
            }
            var transaction = Connection.BeginTransaction();
            transactions.Push(transaction);
        }

        public void CommitTransaction()
        {
            using (var transaction = PopCurrentTransaction())
            {
                transaction.Commit();
            }
        }

        public void RollbackTransaction()
        {
            using (var transaction = PopCurrentTransaction())
            {
                transaction.Rollback();
            }
        }

        private SqlTransaction PopCurrentTransaction()
        {
            if (transactions.Count == 0)
            {
                throw new ApplicationException("Begin transaction first");
            }
            return transactions.Pop();
        }

        public void ResetSchema()
        {
            ChangeSchema(string.Empty);
        }

        public void ChangeSchema(string schema)
        {
            connBuilder.InitialCatalog = schema;
            if (IsConnectionOpen)
            {
                try
                {
                    sqlConn.ChangeDatabase(string.IsNullOrEmpty(schema) ? "master" : schema);
                }
                finally
                {
                    CloseConnection();
                    OpenConnection();
                }
            }
        }

        public DataSet ExecuteDataAdapter(string script)
        {
            var ds = new DataSet();
            SqlConnection conn = null;
            try
            {
                conn = GetOpenConnection();

                var cmd = GetCommand(script, conn);
                var adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
            }
            finally
            {
                CloseConnection(conn);
            }
            return ds;
        }

        public void ExecuteProcedure(string name, params SqlParameter[] parameters)
        {
            SqlConnection conn = null;
            try
            {
                conn = GetOpenConnection();
                var cmd = GetCommandWithParameters(name, conn, CommandType.StoredProcedure, parameters);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                CloseConnection(conn);
            }
        }

        public object ExecuteProcedureScalar(string name, params SqlParameter[] parameters)
        {
            SqlConnection conn = null;
            try
            {
                conn = GetOpenConnection();
                var cmd = GetCommandWithParameters(name, conn, CommandType.StoredProcedure, parameters);
                var result = cmd.ExecuteScalar();
                return result;
            }
            finally
            {
                CloseConnection(conn);
            }
        }

        public DataTable GetProcedureResult(string name, params SqlParameter[] parameters)
        {
            var res = new DataTable();
            SqlConnection conn = null;
            try
            {
                conn = GetOpenConnection();
                var cmd = GetCommandWithParameters(name, conn, CommandType.StoredProcedure, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    for (var i = 0; i <= reader.FieldCount - 1; i++)
                    {
                        var dc = new DataColumn(reader.GetName(i), reader.GetFieldType(i));
                        res.Columns.Add(dc);
                    }

                    while (reader.Read())
                    {
                        var row = new object[reader.FieldCount];
                        for (var i = 0; i <= reader.FieldCount - 1; i++)
                        {
                            row[i] = reader[i];
                        }
                        res.Rows.Add(row);
                    }
                }
            }
            finally
            {
                CloseConnection(conn);
            }
            return res;
        }

        public void ExecuteNonQuery(string script)
        {
            SqlConnection conn = null;
            try
            {
                conn = GetOpenConnection();
                var cmd = GetCommand(script, conn);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                CloseConnection(conn);
            }
        }

        public void ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            SqlConnection conn = null;
            try
            {
                conn = GetOpenConnection();

                var cmd = GetCommand(sql, conn);
                foreach (var par in parameters)
                {
                    cmd.Parameters.Add(par);
                }
                cmd.ExecuteNonQuery();
            }
            finally
            {
                CloseConnection(conn);
            }
        }

        public DataTable ExecuteQuery(string script)
        {
            return ExecuteQuery(script, new SqlParameter[] {});
        }
        public DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
        {
            SqlConnection conn = null;
            var res = new DataTable();
            try
            {
                conn = GetOpenConnection();

                var cmd = GetCommand(sql, conn);
                foreach (var par in parameters)
                {
                    cmd.Parameters.Add(par);
                }
                using (var reader = cmd.ExecuteReader())
                {
                    for (var i = 0; i <= reader.FieldCount - 1; i++)
                    {
                        var dc = new DataColumn(reader.GetName(i), reader.GetFieldType(i));
                        res.Columns.Add(dc);
                    }
                    while (reader.Read())
                    {
                        var row = new object[reader.FieldCount];
                        for (var i = 0; i <= reader.FieldCount - 1; i++)
                        {
                            row[i] = reader[i];
                        }
                        res.Rows.Add(row);
                    }
                }
            }
            finally
            {
                CloseConnection(conn);
            }
            return res;
        }

        /// <summary>
        ///     Execute command
        /// </summary>
        /// <param name="script"> Query text </param>
        public string ExecuteNonQueryWithPrintMessage(string script)
        {
            SqlConnection conn = null;
            var retMessage = new StringBuilder();
            SqlInfoMessageEventHandler infoHandler = delegate(object sender, SqlInfoMessageEventArgs e) { retMessage.Append(e.Message); };
            try
            {
                conn = GetOpenConnection();
                conn.InfoMessage += infoHandler;
                var cmd = GetCommand(script, conn);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (conn != null)
                {
                    conn.InfoMessage -= infoHandler;
                }
                CloseConnection(conn);
            }
            return retMessage.ToString();
        }
        /// <summary>
        ///     Open <see cref="SqlDataReader" /> reader for script. ATTENTION:This command not close connection!
        /// </summary>
        /// <param name="script"> </param>
        /// <returns> </returns>
        public DbDataReader ExecuteReader(string script)
        {
            SqlDataReader reader;
            var conn = GetOpenConnection();
            var cmd = GetCommand(script, conn);
            reader = cmd.ExecuteReader();
            return reader;
        }

        /// <summary>
        ///     Open connection for run multiple statement
        /// </summary>
        public void OpenConnection()
        {
            if (Connection != null && (Connection.State != ConnectionState.Closed || Connection.State != ConnectionState.Broken))
            {
                throw new ApplicationException("Connection already opened");
            }
            CloseConnection(Connection);
            Connection = new SqlConnection(connBuilder.ToString());
            Connection.Open();
        }
        /// <summary>
        ///     Close connection after run multiple statement
        /// </summary>
        public void CloseConnection()
        {
            while (transactions.Count != 0)
            {
                var transaction = transactions.Pop();
                transaction.Rollback();
                transaction.Dispose();
            }
            CloseConnection(Connection, true);
            Connection = null;
        }

        public bool IsDBClientException(Exception exception)
        {
            return exception is SqlException;
        }

        public override string ToString()
        {
            return string.Format("{0}; IsOpened: {1}", connBuilder, IsConnectionOpen);
        }

        public void Dispose()
        {
            CloseConnection();
        }
        #endregion
        #region Private methods
        private SqlConnection Connection
        {
            [DebuggerStepThrough]
            get { return sqlConn; }
            [DebuggerStepThrough]
            set { sqlConn = value; }
        }

        private SqlConnection GetOpenConnection()
        {
            if (IsConnectionOpen)
            {
                return Connection;
            }
            var conn = new SqlConnection(connBuilder.ToString());
            conn.Open();
            return conn;
        }

        private void CloseConnection(SqlConnection connection, bool force = false)
        {
            if (connection == Connection && !force)
            {
                return;
            }
            if (connection != null &&
                connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        #endregion
    }
}