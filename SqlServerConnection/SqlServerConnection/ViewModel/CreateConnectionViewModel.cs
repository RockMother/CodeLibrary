using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Windows.Input;
using TreeFrogs.WFF;

namespace TreeFrogs.SqlServerConnection.ViewModel
{
    public class CreateConnectionViewModel : NotifiedPropertyChangedModel
    {
        private string datasource;
        private string userName;
        private string password;
        private bool isWinAuthSelected;
        private ICommand testConnectionCommand;
        private readonly Action<string> testConnectionAction;
        private Func<string, string[]> getDatabaseListAction;
        private bool? testingSuccess;
        private IList<string> databaseList;

        public CreateConnectionViewModel(Action<string> testConnectionAction, Func<string, string[]> getDatabaseListAction)
        {
            this.testConnectionAction = testConnectionAction;
            this.getDatabaseListAction = getDatabaseListAction;

            IsWinAuthSelected = true;
            var currentIdentity = WindowsIdentity.GetCurrent();
            if (currentIdentity != null)
            {
                UserName = currentIdentity.Name;
            }
        }

        public string ConnectionString
        {
            get { return GetConnectionString(); }
        }

        private string GetConnectionString()
        {
            var sb = new SqlConnectionStringBuilder
            {
                DataSource = Datasource,
                IntegratedSecurity = IsWinAuthSelected

            };
            if (!IsWinAuthSelected)
            {
                sb.Password = Password;
                sb.UserID = UserName;
                
            }
            return sb.ToString();
        }

        public string Datasource
        {
            get { return datasource; }
            set
            {
                datasource = value;
                OnPropertyChanged();
            }
        }

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }
        public IList<string> DatabaseList
        {
            get
            {
                return databaseList;
            }
            set { databaseList = value; }
        }

        public bool IsWinAuthSelected
        {
            get { return isWinAuthSelected; }
            set
            {
                isWinAuthSelected = value;
                OnPropertyChanged();
            }
        }

        public bool? TestingSuccess
        {
            get { return testingSuccess; }
            set
            {
                testingSuccess = value; 
                OnPropertyChanged();
            }
        }

        public ICommand TestConnectionCommand
        {
            get { return testConnectionCommand ?? (testConnectionCommand = new ActionCommand(o => TestConnection())); }
        }

        private void TestConnection()
        {
            try
            {
                testConnectionAction(GetConnectionString());
                TestingSuccess = true;
            }
            catch (Exception)
            {
                TestingSuccess = false;
            }
        }

    }
}
