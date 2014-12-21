using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeFrogs.SqlServer;
using TreeFrogs.SqlServerConnection.ViewModel;
using TreeFrogs.WFF;

namespace SqlServerConnection.ViewModel
{
    public class RunnerViewModel: NotifiedPropertyChangedModel
    {
        private CreateConnectionViewModel createConnectionViewModel;

        public RunnerViewModel()
        {
            CreateConnectionViewModel = new CreateConnectionViewModel(TestConnectionAction, GetDatabaseListAction);
        }

        private string[] GetDatabaseListAction(string connectionString)
        {
            var s = new ScriptEngine(connectionString);
            var result = s.ExecuteQuery("select name from sys.databases where HAS_DBACCESS(name) = 1");
            var dbList = new List<string>();
            foreach (DataRow row in result.Rows)
            {
                dbList.Add(row[0] as string);
            }
            return dbList.ToArray();
        }

        private void TestConnectionAction(string connectionString)
        {
            var s = new ScriptEngine(connectionString);
            try
            {
                s.OpenConnection();
            }
            finally
            {
                s.CloseConnection();
            }
        }

        public CreateConnectionViewModel CreateConnectionViewModel

        {
            get { return createConnectionViewModel; }
            set
            {
                createConnectionViewModel = value; 
                OnPropertyChanged();
            }
        }
    }
}
